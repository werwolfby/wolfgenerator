// Compiled by vsCoco on 27.01.2009 1:39:41
/*----------------------------------------------------------------------
Compiler Generator Coco/R,
Copyright (c) 1990, 2004 Hanspeter Moessenboeck, University of Linz
extended by M. Loeberbauer & A. Woess, Univ. of Linz
with improvements by Pat Terry, Rhodes University

This program is free software; you can redistribute it and/or modify it 
under the terms of the GNU General Public License as published by the 
Free Software Foundation; either version 2, or (at your option) any 
later version.

This program is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License 
for more details.

You should have received a copy of the GNU General Public License along 
with this program; if not, write to the Free Software Foundation, Inc., 
59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.

As an exception, it is allowed to write an extension of Coco/R that is
used as a plugin in non-free software.

If not otherwise stated, any source code generated by Coco/R (other than 
Coco/R itself) does not fall under the GNU General Public License.
----------------------------------------------------------------------*/
using System;
using System.Text;
using System.Collections.Generic;
using WolfGenerator.Core.AST;

//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by COCO from Parser.frame.
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections;

namespace WolfGenerator.Core {


	public class Parser {
	const int _EOF = 0;
	const int _ident = 1;
	const int _number = 2;
	const int _string = 3;
	const int _badString = 4;
	const int _char = 5;
	const int maxT = 27;

		const bool T = true;
		const bool x = false;
		const int minErrDist = 2;
		
		public Scanner scanner;
		public Errors  errors;

		public Token t;    // last recognized token
		public Token la;   // lookahead token
		int errDist = minErrDist;

public RuleClassStatement ruleClassStatement;
	
	int AddStatement( bool isStart, int startPos, List<RuleStatement> statements, bool ifEnd )
	{
		string text = scanner.buffer.GetString( startPos, la.pos );
		if (!string.IsNullOrEmpty( text ))
		{
			int startIndex = 0;
			int endIndex = 0;
			if (isStart)
			{
				if (text.StartsWith( "\r\n" )) startIndex = 2;
				else if (text.StartsWith( "\n" )) startIndex = 1;
			}
			if (ifEnd)
			{
				if (text.EndsWith( "\r\n" )) endIndex = 2;
				else if (text.EndsWith( "\n" )) endIndex = 1;
			}
			if (text.Length - endIndex - startIndex > 0)
			{
				if (startIndex > 0 || endIndex > 0) text = text.Substring( startIndex, text.Length - endIndex - startIndex );
				statements.Add( new TextStatement( text ) );
			}
		}
		return la.pos;
	}


		public Parser(Scanner scanner) {
			this.scanner = scanner;
			errors = new Errors(scanner);
		}

		public Parser(System.IO.Stream str) {
			scanner = new Scanner(str);
			errors = new Errors(scanner);
		}
		
		public Parser(string source) {
			MemoryStream memIn = new MemoryStream();
			byte[] b=System.Text.Encoding.ASCII.GetBytes(source);
			memIn.Write(b,0,b.Length);
			memIn.Seek(0,0);
			this.scanner = new Scanner(memIn);
			errors = new Errors(scanner);
		}

		void SynErr (int n) {
			if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
			errDist = 0;
		}

		public void SemErr (string msg) {
			if (errDist >= minErrDist) errors.Error(t.line, t.col, msg);
			errDist = 0;
		}
		
		void Get () {
			for (;;) {
				t = la;
				la = scanner.Scan();
				if (la.kind <= maxT) { ++errDist; break; }

				la = t;
			}
		}
		
		void Expect (int n) {
			if (la.kind==n) Get(); else { SynErr(n); }
		}
		
		bool StartOf (int s) {
			return set[s, la.kind];
		}
		
		void ExpectWeak (int n, int follow) {
			if (la.kind == n) Get();
			else {
				SynErr(n);
				while (!StartOf(follow)) Get();
			}
		}
		
		bool WeakSeparator (int n, int syFol, int repFol) {
			bool[] s = new bool[maxT+1];
			if (la.kind == n) { Get(); return true; }
			else if (StartOf(repFol)) return false;
			else {
				for (int i=0; i <= maxT; i++) {
					s[i] = set[syFol, i] || set[repFol, i] || set[0, i];
				}
				SynErr(n);
				while (!s[la.kind]) Get();
				return StartOf(syFol);
			}
		}
		
	void WolfGenerator() {
		List<UsingStatement> usingStatementList = null; 
		List<RuleMethodStatement> ruleMethodStatementList = null; 
		string name; 
		RuleClassStart(out name);
		while (la.kind == 9) {
			UsingStatement usingStatement; 
			Using(out usingStatement);
			if (usingStatementList == null) usingStatementList = new List<UsingStatement>();
			usingStatementList.Add( usingStatement ); 
		}
		while (la.kind == 11) {
			RuleMethodStatement ruleMethod; 
			RuleMethod(out ruleMethod);
			if (ruleMethodStatementList == null) 
			   ruleMethodStatementList = new List<RuleMethodStatement>();
			ruleMethodStatementList.Add( ruleMethod ); 
		}
		RuleClassEnd();
		ruleClassStatement = new RuleClassStatement( name, usingStatementList, ruleMethodStatementList ); 
	}

	void RuleClassStart(out string name) {
		Expect(6);
		Expect(1);
		name = t.val; 
		Expect(7);
	}

	void Using(out UsingStatement usingStatement) {
		List<string> namespaceList = new List<string>(); 
		Expect(9);
		Expect(1);
		namespaceList.Add( t.val ); 
		while (la.kind == 10) {
			Get();
			Expect(1);
			namespaceList.Add( t.val ); 
		}
		string namespaceName = string.Join( ".", namespaceList.ToArray() );
		usingStatement = new UsingStatement( namespaceName ); 
		Expect(7);
	}

	void RuleMethod(out RuleMethodStatement statement) {
		string methodName; IList<Variable> variables;
		List<RuleStatement> statements = new List<RuleStatement>();
		bool isStart = true;
		ValueStatement valueStatement;
		JoinStatement joinStatement;
		CodeStatement codeStatement;
		CallStatement callStatement; 
		RuleMethodStart(out methodName, out variables);
		int startPos = t.pos + t.val.Length; 
		while (StartOf(1)) {
			if (StartOf(2)) {
				Get();
			} else if (la.kind == 15) {
				AddStatement( isStart, startPos, statements, false ); 
				Value(out valueStatement);
				statements.Add( valueStatement ); isStart = false; startPos = t.pos + t.val.Length; 
			} else if (la.kind == 16) {
				AddStatement( isStart, startPos, statements, false ); 
				Join(out joinStatement);
				statements.Add( joinStatement ); isStart = false; startPos = t.pos + t.val.Length; 
			} else if (la.kind == 21) {
				AddStatement( isStart, startPos, statements, false ); 
				isStart = false; 
				Code(out codeStatement, ref isStart);
				statements.Add( codeStatement ); startPos = t.pos + t.val.Length; 
			} else {
				AddStatement( isStart, startPos, statements, false ); 
				Call(out callStatement);
				statements.Add( callStatement ); isStart = false; startPos = t.pos + t.val.Length; 
			}
		}
		AddStatement( isStart, startPos, statements, true ); 
		RuleMethodEnd();
		statement = new RuleMethodStatement( methodName, variables, statements ); 
	}

	void RuleClassEnd() {
		Expect(8);
	}

	void RuleMethodStart(out string name, out IList<Variable> variables ) {
		Variable var = null; 
		List<Variable> variableList = null; 
		Expect(11);
		Expect(1);
		name = t.val; 
		Expect(12);
		if (la.kind == 1) {
			Var(out var);
			if (variableList == null) variableList = new List<Variable>();
			variableList.Add( var ); 
			while (la.kind == 13) {
				Get();
				Var(out var);
				variableList.Add( var ); 
			}
		}
		ExpectWeak(14, 3);
		while (!(la.kind == 0 || la.kind == 7)) {SynErr(28); Get();}
		Expect(7);
		variables = variableList.AsReadOnly(); 
	}

	void Value(out ValueStatement valueStatement) {
		Expect(15);
		int pos = t.pos + t.val.Length; 
		while (StartOf(4)) {
			Get();
		}
		while (!(la.kind == 0 || la.kind == 7)) {SynErr(29); Get();}
		Expect(7);
		int endPos = t.pos;
		string value = scanner.buffer.GetString( pos, endPos );
		valueStatement = new ValueStatement( value.Trim() );  
	}

	void Join(out JoinStatement joinStatement) {
		string @string;
		List<RuleStatement> statements = null;
		ValueStatement valueStatement;
		ApplyStatement applyStatement;
		CallStatement callStatement; 
		Expect(16);
		Expect(3);
		@string = t.val.Substring( 1, t.val.Length - 2 ); 
		Expect(7);
		while (la.kind == 15 || la.kind == 17 || la.kind == 24) {
			if (la.kind == 15) {
				Value(out valueStatement);
				if (statements == null) statements = new List<RuleStatement>();
				statements.Add( valueStatement ); 
			} else if (la.kind == 17) {
				Apply(out applyStatement);
				if (statements == null) statements = new List<RuleStatement>();
				statements.Add( applyStatement ); 
			} else {
				Call(out callStatement);
				if (statements == null) statements = new List<RuleStatement>();
				statements.Add( callStatement ); 
			}
		}
		while (!(la.kind == 0 || la.kind == 8)) {SynErr(30); Get();}
		Expect(8);
		joinStatement = new JoinStatement( @string, statements ); 
	}

	void Code(out CodeStatement codeStatement, ref bool isStart) {
		Expect(21);
		int startPos = t.pos + t.val.Length; 
		while (StartOf(5)) {
			Get();
		}
		string value = scanner.buffer.GetString( startPos, la.pos ); 
		if (la.kind == 22) {
			Get();
		} else if (la.kind == 23) {
			Get();
			isStart = true; 
		} else SynErr(31);
		codeStatement = new CodeStatement( value.Trim() ); 
	}

	void Call(out CallStatement callStatement) {
		string methodName; string parameters = null; 
		Expect(24);
		Expect(1);
		methodName = t.val; 
		int startPos = -1; int endPos = -1; 
		if (la.kind == 18) {
			Get();
			startPos = t.pos + t.val.Length; 
			while (StartOf(6)) {
				Get();
			}
			endPos = la.pos; 
			Expect(19);
		} else if (la.kind == 12) {
			Get();
			startPos = t.pos + t.val.Length; 
			while (StartOf(7)) {
				Get();
			}
			endPos = la.pos; 
			Expect(14);
		} else SynErr(32);
		if (startPos > 0 && endPos > 0)
		   parameters = scanner.buffer.GetString( startPos, endPos ).Trim(); 
		while (!(la.kind == 0 || la.kind == 7)) {SynErr(33); Get();}
		Expect(7);
		callStatement = new CallStatement( methodName, parameters ); 
	}

	void RuleMethodEnd() {
		while (!(la.kind == 0 || la.kind == 8)) {SynErr(34); Get();}
		Expect(8);
	}

	void Var(out Variable var) {
		WolfGenerator.Core.AST.Type type; 
		Type(out type);
		Expect(1);
		var = new Variable( t.val, type ); 
	}

	void Apply(out ApplyStatement applyStatement) {
		string methodName; string parameters = null; string from; 
		Expect(17);
		Expect(1);
		methodName = t.val; 
		int startPos = -1; int endPos = -1; 
		if (la.kind == 18) {
			Get();
			startPos = t.pos + t.val.Length; 
			while (StartOf(6)) {
				Get();
			}
			endPos = la.pos; 
			Expect(19);
		} else if (la.kind == 12) {
			Get();
			startPos = t.pos + t.val.Length; 
			while (StartOf(7)) {
				Get();
			}
			endPos = la.pos; 
			Expect(14);
		} else SynErr(35);
		if (startPos > 0 && endPos > 0)
		   parameters = scanner.buffer.GetString( startPos, endPos ).Trim(); 
		while (!(la.kind == 0 || la.kind == 20)) {SynErr(36); Get();}
		Expect(20);
		Expect(1);
		from = t.val; 
		while (!(la.kind == 0 || la.kind == 7)) {SynErr(37); Get();}
		Expect(7);
		applyStatement = new ApplyStatement( methodName, parameters, from ); 
	}

	void Type(out WolfGenerator.Core.AST.Type type) {
		StringBuilder name = new StringBuilder(); 
		List<WolfGenerator.Core.AST.Type> genericParameters = null; 
		Expect(1);
		name.Append( t.val ); 
		while (la.kind == 10) {
			Get();
			Expect(1);
			name.Append( '.' ); name.Append( t.val ); 
		}
		if (la.kind == 25) {
			genericParameters = new List<WolfGenerator.Core.AST.Type>();
			WolfGenerator.Core.AST.Type generic; 
			Get();
			Type(out generic);
			genericParameters.Add( generic ); 
			while (la.kind == 13) {
				Get();
				Type(out generic);
				genericParameters.Add( generic ); 
			}
			Expect(26);
		}
		type = new WolfGenerator.Core.AST.Type( name.ToString(), genericParameters ); 
	}


		public void Parse() {
			la = new Token();
			la.val = "";		
			Get();
		WolfGenerator();

		Expect(0);
		}
		
		bool[,] set = {
		{T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, x,T,T,T, T,T,T,x, x,T,T,T, T,x,T,T, x,T,T,T, x},
		{T,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, x}

		};
	} // end Parser


	public class Errors {
		public int count = 0;                                    // number of errors detected
		public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text
		private Scanner scanner;

		public Errors(Scanner scanner)
		{
			this.scanner=scanner;
		}
		
		public void SynErr (int line, int col, int n) {
			string s;
			switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "string expected"; break;
			case 4: s = "badString expected"; break;
			case 5: s = "char expected"; break;
			case 6: s = "\"<%ruleclass\" expected"; break;
			case 7: s = "\"%>\" expected"; break;
			case 8: s = "\"<%end%>\" expected"; break;
			case 9: s = "\"<%using\" expected"; break;
			case 10: s = "\".\" expected"; break;
			case 11: s = "\"<%rule\" expected"; break;
			case 12: s = "\"(\" expected"; break;
			case 13: s = "\",\" expected"; break;
			case 14: s = "\")\" expected"; break;
			case 15: s = "\"<%=\" expected"; break;
			case 16: s = "\"<%join\" expected"; break;
			case 17: s = "\"<%apply\" expected"; break;
			case 18: s = "\"([\" expected"; break;
			case 19: s = "\"])\" expected"; break;
			case 20: s = "\"from\" expected"; break;
			case 21: s = "\"<%$\" expected"; break;
			case 22: s = "\"$%>\" expected"; break;
			case 23: s = "\"$-%>\" expected"; break;
			case 24: s = "\"<%call\" expected"; break;
			case 25: s = "\"<\" expected"; break;
			case 26: s = "\">\" expected"; break;
			case 27: s = "??? expected"; break;
			case 28: s = "this symbol not expected in RuleMethodStart"; break;
			case 29: s = "this symbol not expected in Value"; break;
			case 30: s = "this symbol not expected in Join"; break;
			case 31: s = "invalid Code"; break;
			case 32: s = "invalid Call"; break;
			case 33: s = "this symbol not expected in Call"; break;
			case 34: s = "this symbol not expected in RuleMethodEnd"; break;
			case 35: s = "invalid Apply"; break;
			case 36: s = "this symbol not expected in Apply"; break;
			case 37: s = "this symbol not expected in Apply"; break;

				default: s = "error " + n; break;
			}
			Error(line, col, s);
		}

		public virtual void Error (int lin, int col, string err) {
			scanner.WriteError(errMsgFormat,scanner.srcFile,lin,col,err);
			count++;
		}

	} // Errors

}/*----------------------------------------------------------------------
Compiler Generator Coco/R,
Copyright (c) 1990, 2004 Hanspeter Moessenboeck, University of Linz
extended by M. Loeberbauer & A. Woess, Univ. of Linz
with improvements by Pat Terry, Rhodes University

This program is free software; you can redistribute it and/or modify it 
under the terms of the GNU General Public License as published by the 
Free Software Foundation; either version 2, or (at your option) any 
later version.

This program is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License 
for more details.

You should have received a copy of the GNU General Public License along 
with this program; if not, write to the Free Software Foundation, Inc., 
59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.

As an exception, it is allowed to write an extension of Coco/R that is
used as a plugin in non-free software.

If not otherwise stated, any source code generated by Coco/R (other than 
Coco/R itself) does not fall under the GNU General Public License.
-----------------------------------------------------------------------*/

//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by COCO from Scanner.frame.
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

namespace WolfGenerator.Core {

	public class Token {
		public int kind;    // token kind
		public int pos;     // token position in the source text (starting at 0)
		public int col;     // token column (starting at 0)
		public int line;    // token line (starting at 1)
		public string val;  // token value
		public Token next;  // ML 2005-03-11 Tokens are kept in linked list
	}

	public class Buffer {
		public const char EOF = (char)256;
		const int MAX_BUFFER_LENGTH = 64 * 1024; // 64KB
		byte[] buf;         // input buffer
		int bufStart;       // position of first byte in buffer relative to input stream
		int bufLen;         // length of buffer
		int fileLen;        // length of input stream
		int pos;            // current position in buffer
		Stream stream;      // input stream (seekable)
		bool isUserStream;  // was the stream opened by the user?
		
	
		protected Buffer(Buffer b) { // called in UTF8Buffer constructor
			buf = b.buf;
			bufStart = b.bufStart;
			bufLen = b.bufLen;
			fileLen = b.fileLen;
			pos = b.pos;
			stream = b.stream;
			// keep destructor from closing the stream
			b.stream = null;
			isUserStream = b.isUserStream;
		}
		
		public Buffer (Stream s, bool isUserStream) {
			stream = s; this.isUserStream = isUserStream;
			fileLen = bufLen = (int) s.Length;
			if (stream.CanSeek && bufLen > MAX_BUFFER_LENGTH) bufLen = MAX_BUFFER_LENGTH;
			buf = new byte[bufLen];
			bufStart = Int32.MaxValue; // nothing in the buffer so far
			Pos = 0; // setup buffer to position 0 (start)
			if (bufLen == fileLen) Close();
		}
		
		~Buffer() { Close(); }
		
		void Close() {
			if (!isUserStream && stream != null) {
				stream.Close();
				stream = null;
			}
		}
		
		public virtual int Read () {
			if (pos < bufLen) {
				return buf[pos++];
			} else if (Pos < fileLen) {
				Pos = Pos; // shift buffer start to Pos
				return buf[pos++];
			} else {
				return EOF;
			}
		}

		public int Peek () {
			if (pos < bufLen) {
				return buf[pos];
			} else if (Pos < fileLen) {
				Pos = Pos; // shift buffer start to Pos
				return buf[pos];
			} else {
				return EOF;
			}
		}
		
		public string GetString (int beg, int end) {
			int len = end - beg;
			char[] buf = new char[len];
			int oldPos = Pos;
			Pos = beg;
			for (int i = 0; i < len; i++) buf[i] = (char) Read();
			Pos = oldPos;
			return new String(buf);
		}

		public int Pos {
			get { return pos + bufStart; }
			set {
				if (value < 0) value = 0; 
				else if (value > fileLen) value = fileLen;
				if (value >= bufStart && value < bufStart + bufLen) { // already in buffer
					pos = value - bufStart;
				} else if (stream != null) { // must be swapped in
					stream.Seek(value, SeekOrigin.Begin);
					bufLen = stream.Read(buf, 0, buf.Length);
					bufStart = value; pos = 0;
				} else {
					pos = fileLen - bufStart; // make Pos return fileLen
				}
			}
		}
	}

//-----------------------------------------------------------------------------------
// UTF8Buffer
//-----------------------------------------------------------------------------------
public class UTF8Buffer: Buffer {
	public UTF8Buffer(Buffer b): base(b) {}

	public override int Read() {
		int ch;
		do {
			ch = base.Read();
			// until we find a uft8 start (0xxxxxxx or 11xxxxxx)
		} while ((ch >= 128) && ((ch & 0xC0) != 0xC0) && (ch != EOF));
		if (ch < 128 || ch == EOF) {
			// nothing to do, first 127 chars are the same in ascii and utf8
			// 0xxxxxxx or end of file character
		} else if ((ch & 0xF0) == 0xF0) {
			// 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
			int c1 = ch & 0x07; ch = base.Read();
			int c2 = ch & 0x3F; ch = base.Read();
			int c3 = ch & 0x3F; ch = base.Read();
			int c4 = ch & 0x3F;
			ch = (((((c1 << 6) | c2) << 6) | c3) << 6) | c4;
		} else if ((ch & 0xE0) == 0xE0) {
			// 1110xxxx 10xxxxxx 10xxxxxx
			int c1 = ch & 0x0F; ch = base.Read();
			int c2 = ch & 0x3F; ch = base.Read();
			int c3 = ch & 0x3F;
			ch = (((c1 << 6) | c2) << 6) | c3;
		} else if ((ch & 0xC0) == 0xC0) {
			// 110xxxxx 10xxxxxx
			int c1 = ch & 0x1F; ch = base.Read();
			int c2 = ch & 0x3F;
			ch = (c1 << 6) | c2;
		}
		return ch;
	}
}

	public class Scanner {
		const char EOL = '\n';
		const int eofSym = 0; /* pdt */
	const int charSetSize = 256;
	const int maxT = 27;
	const int noSym = 27;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0, 10,  0, 58, 17,  0,  5, 57, 31,  0,  0, 30,  0, 29,  0,
	  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  0,  0, 56,  0, 55,  0,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  0,  0, 43,  0,  0,
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
			case "from": t.kind = 20; break;
			default: break;
		}
		}

		Token NextToken() {
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
				if ((ch >= '0' && ch <= '9')) {AddCh(); goto case 2;}
				else {t.kind = 2; break;}
			case 3:
				{t.kind = 3; break;}
			case 4:
				{t.kind = 4; break;}
			case 5:
				if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '&' || ch >= '(' && ch <= '[' || ch >= ']' && ch <= 255)) {AddCh(); goto case 6;}
				else if (ch == 92) {AddCh(); goto case 7;}
				else {t.kind = noSym; break;}
			case 6:
				if (ch == 39) {AddCh(); goto case 9;}
				else {t.kind = noSym; break;}
			case 7:
				if ((ch >= ' ' && ch <= '~')) {AddCh(); goto case 8;}
				else {t.kind = noSym; break;}
			case 8:
				if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f')) {AddCh(); goto case 8;}
				else if (ch == 39) {AddCh(); goto case 9;}
				else {t.kind = noSym; break;}
			case 9:
				{t.kind = 5; break;}
			case 10:
				if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '[' || ch >= ']' && ch <= 255)) {AddCh(); goto case 10;}
				else if ((ch == 10 || ch == 13)) {AddCh(); goto case 4;}
				else if (ch == '"') {AddCh(); goto case 3;}
				else if (ch == 92) {AddCh(); goto case 11;}
				else {t.kind = noSym; break;}
			case 11:
				if ((ch >= ' ' && ch <= '~')) {AddCh(); goto case 10;}
				else {t.kind = noSym; break;}
			case 12:
				if (ch == 'l') {AddCh(); goto case 13;}
				else {t.kind = noSym; break;}
			case 13:
				if (ch == 'a') {AddCh(); goto case 14;}
				else {t.kind = noSym; break;}
			case 14:
				if (ch == 's') {AddCh(); goto case 15;}
				else {t.kind = noSym; break;}
			case 15:
				if (ch == 's') {AddCh(); goto case 16;}
				else {t.kind = noSym; break;}
			case 16:
				{t.kind = 6; break;}
			case 17:
				if (ch == '>') {AddCh(); goto case 18;}
				else {t.kind = noSym; break;}
			case 18:
				{t.kind = 7; break;}
			case 19:
				if (ch == 'n') {AddCh(); goto case 20;}
				else {t.kind = noSym; break;}
			case 20:
				if (ch == 'd') {AddCh(); goto case 21;}
				else {t.kind = noSym; break;}
			case 21:
				if (ch == '%') {AddCh(); goto case 22;}
				else {t.kind = noSym; break;}
			case 22:
				if (ch == '>') {AddCh(); goto case 23;}
				else {t.kind = noSym; break;}
			case 23:
				{t.kind = 8; break;}
			case 24:
				if (ch == 's') {AddCh(); goto case 25;}
				else {t.kind = noSym; break;}
			case 25:
				if (ch == 'i') {AddCh(); goto case 26;}
				else {t.kind = noSym; break;}
			case 26:
				if (ch == 'n') {AddCh(); goto case 27;}
				else {t.kind = noSym; break;}
			case 27:
				if (ch == 'g') {AddCh(); goto case 28;}
				else {t.kind = noSym; break;}
			case 28:
				{t.kind = 9; break;}
			case 29:
				{t.kind = 10; break;}
			case 30:
				{t.kind = 13; break;}
			case 31:
				{t.kind = 14; break;}
			case 32:
				{t.kind = 15; break;}
			case 33:
				if (ch == 'o') {AddCh(); goto case 34;}
				else {t.kind = noSym; break;}
			case 34:
				if (ch == 'i') {AddCh(); goto case 35;}
				else {t.kind = noSym; break;}
			case 35:
				if (ch == 'n') {AddCh(); goto case 36;}
				else {t.kind = noSym; break;}
			case 36:
				{t.kind = 16; break;}
			case 37:
				if (ch == 'p') {AddCh(); goto case 38;}
				else {t.kind = noSym; break;}
			case 38:
				if (ch == 'p') {AddCh(); goto case 39;}
				else {t.kind = noSym; break;}
			case 39:
				if (ch == 'l') {AddCh(); goto case 40;}
				else {t.kind = noSym; break;}
			case 40:
				if (ch == 'y') {AddCh(); goto case 41;}
				else {t.kind = noSym; break;}
			case 41:
				{t.kind = 17; break;}
			case 42:
				{t.kind = 18; break;}
			case 43:
				if (ch == ')') {AddCh(); goto case 44;}
				else {t.kind = noSym; break;}
			case 44:
				{t.kind = 19; break;}
			case 45:
				{t.kind = 21; break;}
			case 46:
				if (ch == '>') {AddCh(); goto case 47;}
				else {t.kind = noSym; break;}
			case 47:
				{t.kind = 22; break;}
			case 48:
				if (ch == '%') {AddCh(); goto case 49;}
				else {t.kind = noSym; break;}
			case 49:
				if (ch == '>') {AddCh(); goto case 50;}
				else {t.kind = noSym; break;}
			case 50:
				{t.kind = 23; break;}
			case 51:
				if (ch == 'a') {AddCh(); goto case 52;}
				else {t.kind = noSym; break;}
			case 52:
				if (ch == 'l') {AddCh(); goto case 53;}
				else {t.kind = noSym; break;}
			case 53:
				if (ch == 'l') {AddCh(); goto case 54;}
				else {t.kind = noSym; break;}
			case 54:
				{t.kind = 24; break;}
			case 55:
				{t.kind = 26; break;}
			case 56:
				if (ch == '%') {AddCh(); goto case 59;}
				else {t.kind = 25; break;}
			case 57:
				if (ch == '[') {AddCh(); goto case 42;}
				else {t.kind = 12; break;}
			case 58:
				if (ch == '%') {AddCh(); goto case 46;}
				else if (ch == '-') {AddCh(); goto case 48;}
				else {t.kind = noSym; break;}
			case 59:
				if (ch == 'r') {AddCh(); goto case 60;}
				else if (ch == 'e') {AddCh(); goto case 19;}
				else if (ch == 'u') {AddCh(); goto case 24;}
				else if (ch == '=') {AddCh(); goto case 32;}
				else if (ch == 'j') {AddCh(); goto case 33;}
				else if (ch == 'a') {AddCh(); goto case 37;}
				else if (ch == '$') {AddCh(); goto case 45;}
				else if (ch == 'c') {AddCh(); goto case 51;}
				else {t.kind = noSym; break;}
			case 60:
				if (ch == 'u') {AddCh(); goto case 61;}
				else {t.kind = noSym; break;}
			case 61:
				if (ch == 'l') {AddCh(); goto case 62;}
				else {t.kind = noSym; break;}
			case 62:
				if (ch == 'e') {AddCh(); goto case 63;}
				else {t.kind = noSym; break;}
			case 63:
				if (ch == 'c') {AddCh(); goto case 12;}
				else {t.kind = 11; break;}

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