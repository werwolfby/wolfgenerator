/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 03.02.2009 23:43
 *
 * File: Scanner.cs
 * Remarks:
 * 
 * History:
 *   03.02.2009 23:43 - Create Wireframe
 *
 *******************************************************/

using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace WolfGenerator.Core
{
	public class Token
	{
		public int kind; // token kind
		public int pos; // token position in the source text (starting at 0)
		public int col; // token column (starting at 0)
		public int line; // token line (starting at 1)
		public string val; // token value
		public Token next; // ML 2005-03-11 Tokens are kept in linked list
	}

	public class Buffer
	{
		public const char EOF = (char)256;
		private const int MAX_BUFFER_LENGTH = 64 * 1024; // 64KB
		private readonly byte[] buf; // input buffer
		private int bufStart; // position of first byte in buffer relative to input stream
		private int bufLen; // length of buffer
		private readonly int fileLen; // length of input stream
		private int pos; // current position in buffer
		private Stream stream; // input stream (seekable)
		private readonly bool isUserStream; // was the stream opened by the user?

		protected Buffer( Buffer b )
		{
			// called in UTF8Buffer constructor
			this.buf = b.buf;
			this.bufStart = b.bufStart;
			this.bufLen = b.bufLen;
			this.fileLen = b.fileLen;
			this.pos = b.pos;
			this.stream = b.stream;
			// keep destructor from closing the stream
			b.stream = null;
			this.isUserStream = b.isUserStream;
		}

		public Buffer( Stream s, bool isUserStream )
		{
			this.stream = s;
			this.isUserStream = isUserStream;
			this.fileLen = this.bufLen = (int)s.Length;
			if (this.stream.CanSeek && this.bufLen > MAX_BUFFER_LENGTH) this.bufLen = MAX_BUFFER_LENGTH;
			this.buf = new byte[this.bufLen];
			this.bufStart = Int32.MaxValue; // nothing in the buffer so far
			this.Pos = 0; // setup buffer to position 0 (start)
			if (this.bufLen == this.fileLen) this.Close();
		}

		~Buffer()
		{
			this.Close();
		}

		private void Close()
		{
			if (!this.isUserStream && this.stream != null)
			{
				this.stream.Close();
				this.stream = null;
			}
		}

		public virtual int Read()
		{
			if (this.pos < this.bufLen) return this.buf[this.pos++];
			else if (this.Pos < this.fileLen)
			{
				this.Pos = this.Pos; // shift buffer start to Pos
				return this.buf[this.pos++];
			}
			else return EOF;
		}

		public int Peek()
		{
			if (this.pos < this.bufLen) return this.buf[this.pos];
			else if (this.Pos < this.fileLen)
			{
				this.Pos = this.Pos; // shift buffer start to Pos
				return this.buf[this.pos];
			}
			else return EOF;
		}

		public string GetString( int beg, int end )
		{
			var len = end - beg;
			var buf = new char[len];
			var oldPos = this.Pos;
			this.Pos = beg;
			for (var i = 0; i < len; i++) buf[i] = (char)this.Read();
			this.Pos = oldPos;
			return new String( buf );
		}

		public int Pos
		{
			get { return this.pos + this.bufStart; }
			set
			{
				if (value < 0) value = 0;
				else if (value > this.fileLen) value = this.fileLen;
				if (value >= this.bufStart && value < this.bufStart + this.bufLen)
				{
					// already in buffer
					this.pos = value - this.bufStart;
				}
				else if (this.stream != null)
				{
					// must be swapped in
					this.stream.Seek( value, SeekOrigin.Begin );
					this.bufLen = this.stream.Read( this.buf, 0, this.buf.Length );
					this.bufStart = value;
					this.pos = 0;
				}
				else this.pos = this.fileLen - this.bufStart; // make Pos return fileLen
			}
		}
	}

	//-----------------------------------------------------------------------------------
	// UTF8Buffer
	//-----------------------------------------------------------------------------------
	public class UTF8Buffer : Buffer
	{
		public UTF8Buffer( Buffer b ) : base( b ) {}

		public override int Read()
		{
			int ch;
			do
			{
				ch = base.Read();
				// until we find a uft8 start (0xxxxxxx or 11xxxxxx)
			} while ((ch >= 128) && ((ch & 0xC0) != 0xC0) && (ch != EOF));
			if (ch < 128 || ch == EOF)
			{
				// nothing to do, first 127 chars are the same in ascii and utf8
				// 0xxxxxxx or end of file character
			}
			else if ((ch & 0xF0) == 0xF0)
			{
				// 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
				var c1 = ch & 0x07;
				ch = base.Read();
				var c2 = ch & 0x3F;
				ch = base.Read();
				var c3 = ch & 0x3F;
				ch = base.Read();
				var c4 = ch & 0x3F;
				ch = (((((c1 << 6) | c2) << 6) | c3) << 6) | c4;
			}
			else if ((ch & 0xE0) == 0xE0)
			{
				// 1110xxxx 10xxxxxx 10xxxxxx
				var c1 = ch & 0x0F;
				ch = base.Read();
				var c2 = ch & 0x3F;
				ch = base.Read();
				var c3 = ch & 0x3F;
				ch = (((c1 << 6) | c2) << 6) | c3;
			}
			else if ((ch & 0xC0) == 0xC0)
			{
				// 110xxxxx 10xxxxxx
				var c1 = ch & 0x1F;
				ch = base.Read();
				var c2 = ch & 0x3F;
				ch = (c1 << 6) | c2;
			}
			return ch;
		}
	}

	public class Waiter
	{
		public Func<Token, bool> IsFinish;
		public Func<Token, bool> IsStart;
	}

	public class Scanner
	{
		private const char EOL = '\n';
		private const int eofSym = 0; /* pdt */

		private const int charSetSize = 256;
		private const int maxT = 28;
		private const int noSym = 28;

		private readonly short[] start = {
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	0, 0, 63, 0, 61, 31, 0, 55, 62, 47, 0, 0, 46, 0, 45, 0,
		                                 	52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 0, 0, 60, 0, 51, 0,
		                                 	0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		                                 	1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 49, 0, 0,
		                                 	0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		                                 	1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		                                 	-1
		                                 };

		public Buffer buffer; // scanner buffer
		public string srcFile;

		private Token t; // current token
		private char ch; // current input character
		private int pos; // column number of current character
		private int line; // line number of current character
		private int lineStart; // start position of current line
		private int oldEols; // EOLs that appeared in a comment;
		private BitArray ignore; // set of characters to be ignored by the scanner
		private Token tokens; // list of tokens already peeked (first token is a dummy)
		private Token pt; // current peek token

		private char[] tval = new char[128]; // text of current token
		private int tlen; // length of current token

		public Scanner( string fileName )
		{
			this.srcFile = fileName;
			try
			{
				Stream stream = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
				this.buffer = new Buffer( stream, false );
				this.Init();
			}
			catch (IOException)
			{
				throw new Exception( String.Format( "--- Cannot open file {0}", fileName ) );
			}
		}

		public Scanner( Stream s )
		{
			this.buffer = new Buffer( s, true );
			this.Init();
		}

		public virtual void WriteLine( string s )
		{
			Console.WriteLine( s );
		}

		public virtual void Write( string s )
		{
			Console.Write( s );
		}

		public virtual void WriteError( string fmt, string file, int lin, int col, string err )
		{
			Console.WriteLine( string.Format( fmt, new object[] { file, lin, col, err } ) );
		}

		private void Init()
		{
			this.pos = -1;
			this.line = 1;
			this.lineStart = 0;
			this.oldEols = 0;
			this.NextCh();
			if (this.ch == 0xEF)
			{
				// check optional byte order mark for UTF-8
				this.NextCh();
				int ch1 = this.ch;
				this.NextCh();
				int ch2 = this.ch;
				if (ch1 != 0xBB || ch2 != 0xBF)
					throw new Exception( String.Format( "illegal byte order mark: EF {0,2:X} {1,2:X}", ch1, ch2 ) );
				this.buffer = new UTF8Buffer( this.buffer );
				this.NextCh();
			}
			this.ignore = new BitArray( charSetSize + 1 );
			this.ignore[' '] = true; // blanks are always white space
			this.ignore[9] = true;
			this.ignore[10] = true;
			this.ignore[13] = true;
			this.pt = this.tokens = new Token(); // first token is a dummy
		}

		private void NextCh()
		{
			if (this.oldEols > 0)
			{
				this.ch = EOL;
				this.oldEols--;
			}
			else
			{
				this.ch = (char)this.buffer.Read();
				this.pos++;
				// replace isolated '\r' by '\n' in order to make
				// eol handling uniform across Windows, Unix and Mac
				if (this.ch == '\r' && this.buffer.Peek() != '\n') this.ch = EOL;
				if (this.ch == EOL)
				{
					this.line++;
					this.lineStart = this.pos + 1;
				}
			}
		}

		private void AddCh()
		{
			if (this.tlen >= this.tval.Length)
			{
				var newBuf = new char[2 * this.tval.Length];
				Array.Copy( this.tval, 0, newBuf, 0, this.tval.Length );
				this.tval = newBuf;
			}
			this.tval[this.tlen++] = this.ch;
			this.NextCh();
		}

		private void CheckLiteral()
		{
			switch (this.t.val)
			{
				case "from":
					this.t.kind = 15;
					break;
				default:
					break;
			}
		}

		private readonly Waiter[] waiters = new[]
		                                    {
		                                    	new Waiter
		                                    	{
		                                    		IsStart = token => token.val == "<%rule",
		                                    		IsFinish = token => token.val == "%>"
		                                    	},
		                                    	new Waiter
		                                    	{
		                                    		IsStart = token => token.val == "<%join",
		                                    		IsFinish = token => token.val == "<%end%>"
		                                    	},
		                                    	new Waiter
		                                    	{
		                                    		IsStart = token => token.val == "<%=",
		                                    		IsFinish = token => token.val == "%>"
		                                    	},
		                                    	new Waiter
		                                    	{
		                                    		IsStart = token => token.val == "<%call",
		                                    		IsFinish = token => token.val == "%>"
		                                    	},
		                                    	new Waiter
		                                    	{
		                                    		IsStart = token => token.val == "<%$",
		                                    		IsFinish = token => token.val == "$%>" || token.val == "$-%>"
		                                    	},
		                                    };

		private Waiter waitForEndToken;
		private bool readTextToken = false;

		private Token NextToken()
		{
			if (this.readTextToken)
			{
				this.readTextToken = false;
				return this.GetTextToken();
			}

			this.NextTokenDefault();

			if (this.waitForEndToken != null)
			{
				if (this.waitForEndToken.IsFinish( t ))
				{
					this.readTextToken = true;
					this.waitForEndToken = null;
				}
			}
			else this.waitForEndToken = this.waiters.SingleOrDefault( w => w.IsStart( t ) );

			return t;
		}

		private Token GetTextToken()
		{
			this.tlen = 0;
			this.AddCh();
			var end = false;

			this.t = new Token { pos = this.pos, col = (this.pos - this.lineStart + 1), line = this.line };

			while (!end)
			{
				if (this.ch == '<' && this.buffer.Peek() == '%') end = true;
				else this.AddCh();
			}

			this.t.kind = 28;
			this.t.val = new String( this.tval, 0, this.tlen );
			return this.t;
		}

		private Token NextTokenDefault()
		{
			while (this.ignore[this.ch]) this.NextCh();

			this.t = new Token();
			this.t.pos = this.pos;
			this.t.col = this.pos - this.lineStart + 1;
			this.t.line = this.line;
			int state = this.start[this.ch];
			this.tlen = 0;
			this.AddCh();

			switch (state)
			{
				case -1:
				{
					this.t.kind = eofSym;
					break;
				} // NextCh already done
				case 0:
				{
					this.t.kind = noSym;
					break;
				} // NextCh already done
				case 1:
					if ((this.ch >= '0' && this.ch <= '9' || this.ch >= 'A' && this.ch <= 'Z' || this.ch >= 'a' && this.ch <= 'z'))
					{
						this.AddCh();
						goto case 1;
					}
					else
					{
						this.t.kind = 1;
						this.t.val = new String( this.tval, 0, this.tlen );
						this.CheckLiteral();
						return this.t;
					}
				case 2:
					if (this.ch == 'l')
					{
						this.AddCh();
						goto case 3;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 3:
					if (this.ch == 'a')
					{
						this.AddCh();
						goto case 4;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 4:
					if (this.ch == 's')
					{
						this.AddCh();
						goto case 5;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 5:
					if (this.ch == 's')
					{
						this.AddCh();
						goto case 6;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 6:
				{
					this.t.kind = 2;
					break;
				}
				case 7:
					if (this.ch == 'o')
					{
						this.AddCh();
						goto case 8;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 8:
					if (this.ch == 'i')
					{
						this.AddCh();
						goto case 9;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 9:
					if (this.ch == 'n')
					{
						this.AddCh();
						goto case 10;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 10:
				{
					this.t.kind = 4;
					break;
				}
				case 11:
					if (this.ch == 'p')
					{
						this.AddCh();
						goto case 12;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 12:
					if (this.ch == 'p')
					{
						this.AddCh();
						goto case 13;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 13:
					if (this.ch == 'l')
					{
						this.AddCh();
						goto case 14;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 14:
					if (this.ch == 'y')
					{
						this.AddCh();
						goto case 15;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 15:
				{
					this.t.kind = 5;
					break;
				}
				case 16:
					if (this.ch == 'a')
					{
						this.AddCh();
						goto case 17;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 17:
					if (this.ch == 'l')
					{
						this.AddCh();
						goto case 18;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 18:
					if (this.ch == 'l')
					{
						this.AddCh();
						goto case 19;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 19:
				{
					this.t.kind = 6;
					break;
				}
				case 20:
					if (this.ch == 'e')
					{
						this.AddCh();
						goto case 21;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 21:
					if (this.ch == 't')
					{
						this.AddCh();
						goto case 22;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 22:
					if (this.ch == 'h')
					{
						this.AddCh();
						goto case 23;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 23:
					if (this.ch == 'o')
					{
						this.AddCh();
						goto case 24;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 24:
					if (this.ch == 'd')
					{
						this.AddCh();
						goto case 25;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 25:
				{
					this.t.kind = 7;
					break;
				}
				case 26:
					if (this.ch == 's')
					{
						this.AddCh();
						goto case 27;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 27:
					if (this.ch == 'i')
					{
						this.AddCh();
						goto case 28;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 28:
					if (this.ch == 'n')
					{
						this.AddCh();
						goto case 29;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 29:
					if (this.ch == 'g')
					{
						this.AddCh();
						goto case 30;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 30:
				{
					this.t.kind = 8;
					break;
				}
				case 31:
					if (this.ch == '>')
					{
						this.AddCh();
						goto case 32;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 32:
				{
					this.t.kind = 9;
					break;
				}
				case 33:
					if (this.ch == 'n')
					{
						this.AddCh();
						goto case 34;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 34:
					if (this.ch == 'd')
					{
						this.AddCh();
						goto case 35;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 35:
					if (this.ch == '%')
					{
						this.AddCh();
						goto case 36;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 36:
					if (this.ch == '>')
					{
						this.AddCh();
						goto case 37;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 37:
				{
					this.t.kind = 10;
					break;
				}
				case 38:
				{
					this.t.kind = 11;
					break;
				}
				case 39:
				{
					this.t.kind = 12;
					break;
				}
				case 40:
					if (this.ch == '>')
					{
						this.AddCh();
						goto case 41;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 41:
				{
					this.t.kind = 13;
					break;
				}
				case 42:
					if (this.ch == '%')
					{
						this.AddCh();
						goto case 43;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 43:
					if (this.ch == '>')
					{
						this.AddCh();
						goto case 44;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 44:
				{
					this.t.kind = 14;
					break;
				}
				case 45:
				{
					this.t.kind = 16;
					break;
				}
				case 46:
				{
					this.t.kind = 17;
					break;
				}
				case 47:
				{
					this.t.kind = 19;
					break;
				}
				case 48:
				{
					this.t.kind = 20;
					break;
				}
				case 49:
					if (this.ch == ')')
					{
						this.AddCh();
						goto case 50;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 50:
				{
					this.t.kind = 21;
					break;
				}
				case 51:
				{
					this.t.kind = 23;
					break;
				}
				case 52:
					if ((this.ch >= '0' && this.ch <= '9'))
					{
						this.AddCh();
						goto case 52;
					}
					else
					{
						this.t.kind = 24;
						break;
					}
				case 53:
				{
					this.t.kind = 25;
					break;
				}
				case 54:
				{
					this.t.kind = 26;
					break;
				}
				case 55:
					if ((this.ch <= 9 || this.ch >= 11 && this.ch <= 12 || this.ch >= 14 && this.ch <= '&' ||
					     this.ch >= '(' && this.ch <= '[' || this.ch >= ']' && this.ch <= 255))
					{
						this.AddCh();
						goto case 56;
					}
					else if (this.ch == 92)
					{
						this.AddCh();
						goto case 57;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 56:
					if (this.ch == 39)
					{
						this.AddCh();
						goto case 59;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 57:
					if ((this.ch >= ' ' && this.ch <= '~'))
					{
						this.AddCh();
						goto case 58;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 58:
					if ((this.ch >= '0' && this.ch <= '9' || this.ch >= 'a' && this.ch <= 'f'))
					{
						this.AddCh();
						goto case 58;
					}
					else if (this.ch == 39)
					{
						this.AddCh();
						goto case 59;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 59:
				{
					this.t.kind = 27;
					break;
				}
				case 60:
					if (this.ch == '%')
					{
						this.AddCh();
						goto case 64;
					}
					else
					{
						this.t.kind = 22;
						break;
					}
				case 61:
					if (this.ch == '%')
					{
						this.AddCh();
						goto case 40;
					}
					else if (this.ch == '-')
					{
						this.AddCh();
						goto case 42;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 62:
					if (this.ch == '[')
					{
						this.AddCh();
						goto case 48;
					}
					else
					{
						this.t.kind = 18;
						break;
					}
				case 63:
					if ((this.ch <= 9 || this.ch >= 11 && this.ch <= 12 || this.ch >= 14 && this.ch <= '!' ||
					     this.ch >= '#' && this.ch <= '[' || this.ch >= ']' && this.ch <= 255))
					{
						this.AddCh();
						goto case 63;
					}
					else if ((this.ch == 10 || this.ch == 13))
					{
						this.AddCh();
						goto case 54;
					}
					else if (this.ch == '"')
					{
						this.AddCh();
						goto case 53;
					}
					else if (this.ch == 92)
					{
						this.AddCh();
						goto case 65;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 64:
					if (this.ch == 'r')
					{
						this.AddCh();
						goto case 66;
					}
					else if (this.ch == 'j')
					{
						this.AddCh();
						goto case 7;
					}
					else if (this.ch == 'a')
					{
						this.AddCh();
						goto case 11;
					}
					else if (this.ch == 'c')
					{
						this.AddCh();
						goto case 16;
					}
					else if (this.ch == 'm')
					{
						this.AddCh();
						goto case 20;
					}
					else if (this.ch == 'u')
					{
						this.AddCh();
						goto case 26;
					}
					else if (this.ch == 'e')
					{
						this.AddCh();
						goto case 33;
					}
					else if (this.ch == '=')
					{
						this.AddCh();
						goto case 38;
					}
					else if (this.ch == '$')
					{
						this.AddCh();
						goto case 39;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 65:
					if ((this.ch >= ' ' && this.ch <= '~'))
					{
						this.AddCh();
						goto case 63;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 66:
					if (this.ch == 'u')
					{
						this.AddCh();
						goto case 67;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 67:
					if (this.ch == 'l')
					{
						this.AddCh();
						goto case 68;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 68:
					if (this.ch == 'e')
					{
						this.AddCh();
						goto case 69;
					}
					else
					{
						this.t.kind = noSym;
						break;
					}
				case 69:
					if (this.ch == 'c')
					{
						this.AddCh();
						goto case 2;
					}
					else
					{
						this.t.kind = 3;
						break;
					}
			}
			this.t.val = new String( this.tval, 0, this.tlen );
			return this.t;
		}

		// get the next token (possibly a token already seen during peeking)
		public Token Scan()
		{
			if (this.tokens.next == null) return this.NextToken();
			else
			{
				this.pt = this.tokens = this.tokens.next;
				return this.tokens;
			}
		}

		// peek for the next token, ignore pragmas
		public Token Peek()
		{
			if (this.pt.next == null)
			{
				do
				{
					this.pt = this.pt.next = this.NextToken();
				} while (this.pt.kind > maxT); // skip pragmas
			}
			else
			{
				do
				{
					this.pt = this.pt.next;
				} while (this.pt.kind > maxT);
			}
			return this.pt;
		}

		// make sure that peeking starts at the current scan position
		public void ResetPeek()
		{
			this.pt = this.tokens;
		}
	} // end Scanner
}