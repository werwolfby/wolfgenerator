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

	public class Scanner {
		const char EOL = '\n';
		const int eofSym = 0; /* pdt */
	const int charSetSize = 256;
	const int maxT = 29;
	const int noSym = 29;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0, 66,  0, 64, 34,  0, 58, 65, 50,  0,  0, 49,  0, 48,  0,
	 55, 55, 55, 55, 55, 55, 55, 55, 55, 55,  0,  0, 63,  0, 54,  0,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  0,  0, 52,  0,  0,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  -1};


		public Buffer buffer; // scanner buffer
		public string srcFile;
		
		Token t;          // current token
		char ch;          // current input character
		int pos;          // column number of current character
		int line;         // line number of current character
		int lineStart;    // start position of current line
		int oldEols;      // EOLs that appeared in a comment;
		BitArray ignore;  // set of characters to be ignored by the scanner
		Token tokens;     // list of tokens already peeked (first token is a dummy)
		Token pt;         // current peek token
		
		char[] tval = new char[128]; // text of current token
		int tlen;         // length of current token
		
		public Scanner (string fileName) {
			srcFile=fileName;
			try {
				Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				buffer = new Buffer(stream, false);
				Init();
			} catch (IOException) {
				throw new Exception(String.Format("--- Cannot open file {0}", fileName));
			}
		}
		
		public Scanner (Stream s) {
			buffer = new Buffer(s, true);
			Init();
		}
		
		public virtual void WriteLine(string s) 
		{
			Console.WriteLine(s);
		}
		
		public virtual void Write(string s) 
		{
			Console.Write(s);
		}
		
		public virtual void WriteError(string fmt,string file, int lin, int col, string err)
		{
			Console.WriteLine(string.Format(fmt,new object[] {file, lin, col, err}));
		}

		
		void Init() {
			pos = -1; line = 1; lineStart = 0;
			oldEols = 0;
			NextCh();
			if (ch == 0xEF) { // check optional byte order mark for UTF-8
				NextCh(); int ch1 = ch;
				NextCh(); int ch2 = ch;
				if (ch1 != 0xBB || ch2 != 0xBF) {
					throw new Exception(String.Format("illegal byte order mark: EF {0,2:X} {1,2:X}", ch1, ch2));
				}
				buffer = new UTF8Buffer(buffer);
				NextCh();
			}
			ignore = new BitArray(charSetSize+1);
			ignore[' '] = true;  // blanks are always white space
		ignore[9] = true; ignore[10] = true; ignore[13] = true; 
			pt = tokens = new Token();  // first token is a dummy
		}
		
		void NextCh() {
			if (oldEols > 0) { ch = EOL; oldEols--; } 
			else {
				ch = (char)buffer.Read(); pos++;
				// replace isolated '\r' by '\n' in order to make
				// eol handling uniform across Windows, Unix and Mac
				if (ch == '\r' && buffer.Peek() != '\n') ch = EOL;
				if (ch == EOL) { line++; lineStart = pos + 1; }
			}

		}

		void AddCh() {
			if (tlen >= tval.Length) {
				char[] newBuf = new char[2 * tval.Length];
				Array.Copy(tval, 0, newBuf, 0, tval.Length);
				tval = newBuf;
			}
		tval[tlen++] = ch;
			NextCh();
		}




		void CheckLiteral() {
		switch (t.val) {
			case "from": t.kind = 16; break;
			default: break;
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
		                                    		IsStart = token => token.val == "<%match",
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

			this.t.kind = 29;
			this.t.val = new String( this.tval, 0, this.tlen );
			return this.t;
		}

		Token NextTokenDefault() {
			while (ignore[ch]) NextCh();

			t = new Token();
			t.pos = pos; t.col = pos - lineStart + 1; t.line = line; 
			int state = start[ch];
			tlen = 0; AddCh();
			
			switch (state) {
				case -1: { t.kind = eofSym; break; } // NextCh already done
				case 0: { t.kind = noSym; break; }   // NextCh already done
			case 1:
				if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z')) {AddCh(); goto case 1;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 2:
				if (ch == 'l') {AddCh(); goto case 3;}
				else {t.kind = noSym; break;}
			case 3:
				if (ch == 'a') {AddCh(); goto case 4;}
				else {t.kind = noSym; break;}
			case 4:
				if (ch == 's') {AddCh(); goto case 5;}
				else {t.kind = noSym; break;}
			case 5:
				if (ch == 's') {AddCh(); goto case 6;}
				else {t.kind = noSym; break;}
			case 6:
				{t.kind = 2; break;}
			case 7:
				if (ch == 'o') {AddCh(); goto case 8;}
				else {t.kind = noSym; break;}
			case 8:
				if (ch == 'i') {AddCh(); goto case 9;}
				else {t.kind = noSym; break;}
			case 9:
				if (ch == 'n') {AddCh(); goto case 10;}
				else {t.kind = noSym; break;}
			case 10:
				{t.kind = 4; break;}
			case 11:
				if (ch == 'p') {AddCh(); goto case 12;}
				else {t.kind = noSym; break;}
			case 12:
				if (ch == 'p') {AddCh(); goto case 13;}
				else {t.kind = noSym; break;}
			case 13:
				if (ch == 'l') {AddCh(); goto case 14;}
				else {t.kind = noSym; break;}
			case 14:
				if (ch == 'y') {AddCh(); goto case 15;}
				else {t.kind = noSym; break;}
			case 15:
				{t.kind = 5; break;}
			case 16:
				if (ch == 'a') {AddCh(); goto case 17;}
				else {t.kind = noSym; break;}
			case 17:
				if (ch == 'l') {AddCh(); goto case 18;}
				else {t.kind = noSym; break;}
			case 18:
				if (ch == 'l') {AddCh(); goto case 19;}
				else {t.kind = noSym; break;}
			case 19:
				{t.kind = 6; break;}
			case 20:
				if (ch == 't') {AddCh(); goto case 21;}
				else {t.kind = noSym; break;}
			case 21:
				if (ch == 'h') {AddCh(); goto case 22;}
				else {t.kind = noSym; break;}
			case 22:
				if (ch == 'o') {AddCh(); goto case 23;}
				else {t.kind = noSym; break;}
			case 23:
				if (ch == 'd') {AddCh(); goto case 24;}
				else {t.kind = noSym; break;}
			case 24:
				{t.kind = 7; break;}
			case 25:
				if (ch == 't') {AddCh(); goto case 26;}
				else {t.kind = noSym; break;}
			case 26:
				if (ch == 'c') {AddCh(); goto case 27;}
				else {t.kind = noSym; break;}
			case 27:
				if (ch == 'h') {AddCh(); goto case 28;}
				else {t.kind = noSym; break;}
			case 28:
				{t.kind = 8; break;}
			case 29:
				if (ch == 's') {AddCh(); goto case 30;}
				else {t.kind = noSym; break;}
			case 30:
				if (ch == 'i') {AddCh(); goto case 31;}
				else {t.kind = noSym; break;}
			case 31:
				if (ch == 'n') {AddCh(); goto case 32;}
				else {t.kind = noSym; break;}
			case 32:
				if (ch == 'g') {AddCh(); goto case 33;}
				else {t.kind = noSym; break;}
			case 33:
				{t.kind = 9; break;}
			case 34:
				if (ch == '>') {AddCh(); goto case 35;}
				else {t.kind = noSym; break;}
			case 35:
				{t.kind = 10; break;}
			case 36:
				if (ch == 'n') {AddCh(); goto case 37;}
				else {t.kind = noSym; break;}
			case 37:
				if (ch == 'd') {AddCh(); goto case 38;}
				else {t.kind = noSym; break;}
			case 38:
				if (ch == '%') {AddCh(); goto case 39;}
				else {t.kind = noSym; break;}
			case 39:
				if (ch == '>') {AddCh(); goto case 40;}
				else {t.kind = noSym; break;}
			case 40:
				{t.kind = 11; break;}
			case 41:
				{t.kind = 12; break;}
			case 42:
				{t.kind = 13; break;}
			case 43:
				if (ch == '>') {AddCh(); goto case 44;}
				else {t.kind = noSym; break;}
			case 44:
				{t.kind = 14; break;}
			case 45:
				if (ch == '%') {AddCh(); goto case 46;}
				else {t.kind = noSym; break;}
			case 46:
				if (ch == '>') {AddCh(); goto case 47;}
				else {t.kind = noSym; break;}
			case 47:
				{t.kind = 15; break;}
			case 48:
				{t.kind = 17; break;}
			case 49:
				{t.kind = 18; break;}
			case 50:
				{t.kind = 20; break;}
			case 51:
				{t.kind = 21; break;}
			case 52:
				if (ch == ')') {AddCh(); goto case 53;}
				else {t.kind = noSym; break;}
			case 53:
				{t.kind = 22; break;}
			case 54:
				{t.kind = 24; break;}
			case 55:
				if ((ch >= '0' && ch <= '9')) {AddCh(); goto case 55;}
				else {t.kind = 25; break;}
			case 56:
				{t.kind = 26; break;}
			case 57:
				{t.kind = 27; break;}
			case 58:
				if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '&' || ch >= '(' && ch <= '[' || ch >= ']' && ch <= 255)) {AddCh(); goto case 59;}
				else if (ch == 92) {AddCh(); goto case 60;}
				else {t.kind = noSym; break;}
			case 59:
				if (ch == 39) {AddCh(); goto case 62;}
				else {t.kind = noSym; break;}
			case 60:
				if ((ch >= ' ' && ch <= '~')) {AddCh(); goto case 61;}
				else {t.kind = noSym; break;}
			case 61:
				if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f')) {AddCh(); goto case 61;}
				else if (ch == 39) {AddCh(); goto case 62;}
				else {t.kind = noSym; break;}
			case 62:
				{t.kind = 28; break;}
			case 63:
				if (ch == '%') {AddCh(); goto case 67;}
				else {t.kind = 23; break;}
			case 64:
				if (ch == '%') {AddCh(); goto case 43;}
				else if (ch == '-') {AddCh(); goto case 45;}
				else {t.kind = noSym; break;}
			case 65:
				if (ch == '[') {AddCh(); goto case 51;}
				else {t.kind = 19; break;}
			case 66:
				if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '[' || ch >= ']' && ch <= 255)) {AddCh(); goto case 66;}
				else if ((ch == 10 || ch == 13)) {AddCh(); goto case 57;}
				else if (ch == '"') {AddCh(); goto case 56;}
				else if (ch == 92) {AddCh(); goto case 68;}
				else {t.kind = noSym; break;}
			case 67:
				if (ch == 'r') {AddCh(); goto case 69;}
				else if (ch == 'j') {AddCh(); goto case 7;}
				else if (ch == 'a') {AddCh(); goto case 11;}
				else if (ch == 'c') {AddCh(); goto case 16;}
				else if (ch == 'm') {AddCh(); goto case 70;}
				else if (ch == 'u') {AddCh(); goto case 29;}
				else if (ch == 'e') {AddCh(); goto case 36;}
				else if (ch == '=') {AddCh(); goto case 41;}
				else if (ch == '$') {AddCh(); goto case 42;}
				else {t.kind = noSym; break;}
			case 68:
				if ((ch >= ' ' && ch <= '~')) {AddCh(); goto case 66;}
				else {t.kind = noSym; break;}
			case 69:
				if (ch == 'u') {AddCh(); goto case 71;}
				else {t.kind = noSym; break;}
			case 70:
				if (ch == 'e') {AddCh(); goto case 20;}
				else if (ch == 'a') {AddCh(); goto case 25;}
				else {t.kind = noSym; break;}
			case 71:
				if (ch == 'l') {AddCh(); goto case 72;}
				else {t.kind = noSym; break;}
			case 72:
				if (ch == 'e') {AddCh(); goto case 73;}
				else {t.kind = noSym; break;}
			case 73:
				if (ch == 'c') {AddCh(); goto case 2;}
				else {t.kind = 3; break;}

			}
			t.val = new String(tval, 0, tlen);
			return t;
		}
		
		// get the next token (possibly a token already seen during peeking)
		public Token Scan () {
			if (tokens.next == null) {
				return NextToken();
			} else {
				pt = tokens = tokens.next;
				return tokens;
			}
		}

		// peek for the next token, ignore pragmas
		public Token Peek () {
			if (pt.next == null) {
				do {
					pt = pt.next = NextToken();
				} while (pt.kind > maxT); // skip pragmas
			} else {
				do {
					pt = pt.next;
				} while (pt.kind > maxT);
			}
			return pt;
		}
		
		// make sure that peeking starts at the current scan position
		public void ResetPeek () { pt = tokens; }

	} // end Scanner
}