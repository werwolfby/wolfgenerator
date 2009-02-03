// Compiled by vsCoco on 04.02.2009 0:43:11
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
	const int _ruleClass = 2;
	const int _rule = 3;
	const int _join = 4;
	const int _apply = 5;
	const int _call = 6;
	const int _method = 7;
	const int _uses = 8;
	const int _endStatement = 9;
	const int _end = 10;
	const int _value = 11;
	const int _code = 12;
	const int _codeEnd1 = 13;
	const int _codeEnd2 = 14;
	const int _from = 15;
	const int _dot = 16;
	const int _comma = 17;
	const int _openB = 18;
	const int _closB = 19;
	const int _openB1 = 20;
	const int _closB1 = 21;
	const int _openG = 22;
	const int _closG = 23;
	const int _number = 24;
	const int _string = 25;
	const int _badString = 26;
	const int _char = 27;
	const int _text = 28;
	const int maxT = 29;

		const bool T = true;
		const bool x = false;
		const int minErrDist = 2;
		
		public Scanner scanner;
		public Errors  errors;

		public Token t;    // last recognized token
		public Token la;   // lookahead token
		int errDist = minErrDist;

public RuleClassStatement ruleClassStatement;
	
	string ExtractString( bool isStart, bool ifEnd, int startPos, int endPos )
	{
		string text = scanner.buffer.GetString( startPos, endPos );
		return ExtractString( isStart, ifEnd, text );
	}
	
	string ExtractString( bool isStart, bool ifEnd, string text )
	{
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
				if (startIndex > 0 || endIndex > 0) return text.Substring( startIndex, text.Length - endIndex - startIndex );				
				else return text;
			}
		}
		return "";
	}
	
	int AddStatement( bool isStart, int startPos, List<RuleStatement> statements, bool ifEnd )
	{
		string text = ExtractString( isStart, ifEnd, startPos, la.pos );
		if (!string.IsNullOrEmpty( text )) statements.Add( new TextStatement( text ) );
		return la.pos;
	}
	
	int AddStatement( bool isStart, List<RuleStatement> statements, bool ifEnd, string text )
	{
		text = ExtractString( isStart, ifEnd, text );
		if (!string.IsNullOrEmpty( text )) statements.Add( new TextStatement( text ) );
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
		List<RuleClassMethodStatement> ruleMethodStatementList = null; 
		string name; 
		RuleClassStart(out name);
		while (la.kind == 8) {
			UsingStatement usingStatement; 
			Using(out usingStatement);
			if (usingStatementList == null) usingStatementList = new List<UsingStatement>();
			usingStatementList.Add( usingStatement ); 
		}
		while (la.kind == 3 || la.kind == 7) {
			if (la.kind == 3) {
				RuleMethodStatement ruleMethod; 
				RuleMethod(out ruleMethod);
				if (ruleMethodStatementList == null) 
				   ruleMethodStatementList = new List<RuleClassMethodStatement>();
				ruleMethodStatementList.Add( ruleMethod ); 
			} else {
				MethodStatement method; 
				Method(out method);
				if (ruleMethodStatementList == null) 
				   ruleMethodStatementList = new List<RuleClassMethodStatement>();
				ruleMethodStatementList.Add( method ); 
			}
		}
		RuleClassEnd();
		ruleClassStatement = new RuleClassStatement( name, usingStatementList, ruleMethodStatementList ); 
	}

	void RuleClassStart(out string name) {
		Expect(2);
		Expect(1);
		name = t.val; 
		Expect(9);
	}

	void Using(out UsingStatement usingStatement) {
		List<string> namespaceList = new List<string>(); 
		Expect(8);
		Expect(1);
		namespaceList.Add( t.val ); 
		while (la.kind == 16) {
			Get();
			Expect(1);
			namespaceList.Add( t.val ); 
		}
		string namespaceName = string.Join( ".", namespaceList.ToArray() );
		usingStatement = new UsingStatement( namespaceName ); 
		Expect(9);
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
			if (la.kind == 28) {
				Get();
				AddStatement( isStart, statements, la.kind == _end, t.val ); 
			} else if (la.kind == 11) {
				Value(out valueStatement);
				statements.Add( valueStatement ); isStart = false; startPos = t.pos + t.val.Length; 
			} else if (la.kind == 4) {
				Join(out joinStatement);
				statements.Add( joinStatement ); isStart = false; startPos = t.pos + t.val.Length; 
			} else if (la.kind == 12) {
				isStart = false; 
				Code(out codeStatement, ref isStart);
				statements.Add( codeStatement ); startPos = t.pos + t.val.Length; 
			} else {
				Call(out callStatement);
				statements.Add( callStatement ); isStart = false; startPos = t.pos + t.val.Length; 
			}
		}
		RuleMethodEnd();
		statement = new RuleMethodStatement( methodName, variables, statements ); 
	}

	void Method(out MethodStatement methodStatement) {
		WolfGenerator.Core.AST.Type returnType;
		List<Variable> variables = null;
		int startPos = -1; string name; 
		Expect(7);
		Type(out returnType);
		Expect(1);
		name = t.val; 
		Expect(18);
		Variable currentVariable; 
		if (la.kind == 1) {
			Var(out currentVariable);
			if (variables == null) variables = new List<Variable>();
			   variables.Add( currentVariable ); 
			while (la.kind == 17) {
				Get();
				Var(out currentVariable);
				variables.Add( currentVariable ); 
			}
		}
		Expect(19);
		Expect(9);
		startPos = t.pos + t.val.Length; 
		while (StartOf(2)) {
			Get();
		}
		Expect(10);
		methodStatement = new MethodStatement( returnType, name, variables, ExtractString( true, false, startPos, t.pos ) ); 
	}

	void RuleClassEnd() {
		Expect(10);
	}

	void RuleMethodStart(out string name, out IList<Variable> variables ) {
		Variable var = null; 
		List<Variable> variableList = null; 
		Expect(3);
		Expect(1);
		name = t.val; 
		Expect(18);
		if (la.kind == 1) {
			Var(out var);
			if (variableList == null) variableList = new List<Variable>();
			variableList.Add( var ); 
			while (la.kind == 17) {
				Get();
				Var(out var);
				variableList.Add( var ); 
			}
		}
		ExpectWeak(19, 3);
		while (!(la.kind == 0 || la.kind == 9)) {SynErr(30); Get();}
		Expect(9);
		variables = variableList == null ? null : variableList.AsReadOnly(); 
	}

	void Value(out ValueStatement valueStatement) {
		Expect(11);
		int pos = t.pos + t.val.Length; 
		while (StartOf(4)) {
			Get();
		}
		while (!(la.kind == 0 || la.kind == 9)) {SynErr(31); Get();}
		Expect(9);
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
		Expect(4);
		Expect(25);
		@string = t.val.Substring( 1, t.val.Length - 2 ); 
		Expect(9);
		while (la.kind == 5 || la.kind == 6 || la.kind == 11) {
			if (la.kind == 11) {
				Value(out valueStatement);
				if (statements == null) statements = new List<RuleStatement>();
				statements.Add( valueStatement ); 
			} else if (la.kind == 5) {
				Apply(out applyStatement);
				if (statements == null) statements = new List<RuleStatement>();
				statements.Add( applyStatement ); 
			} else {
				Call(out callStatement);
				if (statements == null) statements = new List<RuleStatement>();
				statements.Add( callStatement ); 
			}
		}
		while (!(la.kind == 0 || la.kind == 10)) {SynErr(32); Get();}
		Expect(10);
		joinStatement = new JoinStatement( @string, statements ); 
	}

	void Code(out CodeStatement codeStatement, ref bool isStart) {
		Expect(12);
		int startPos = t.pos + t.val.Length; 
		while (StartOf(5)) {
			Get();
		}
		string value = scanner.buffer.GetString( startPos, la.pos ); 
		if (la.kind == 13) {
			Get();
		} else if (la.kind == 14) {
			Get();
			isStart = true; 
		} else SynErr(33);
		codeStatement = new CodeStatement( value.Trim() ); 
	}

	void Call(out CallStatement callStatement) {
		string methodName; string parameters = null; 
		Expect(6);
		Expect(1);
		methodName = t.val; 
		int startPos = -1; int endPos = -1; 
		if (la.kind == 20) {
			Get();
			startPos = t.pos + t.val.Length; 
			while (StartOf(6)) {
				Get();
			}
			endPos = la.pos; 
			Expect(21);
		} else if (la.kind == 18) {
			Get();
			startPos = t.pos + t.val.Length; 
			while (StartOf(7)) {
				Get();
			}
			endPos = la.pos; 
			Expect(19);
		} else SynErr(34);
		if (startPos > 0 && endPos > 0)
		   parameters = scanner.buffer.GetString( startPos, endPos ).Trim(); 
		while (!(la.kind == 0 || la.kind == 9)) {SynErr(35); Get();}
		Expect(9);
		callStatement = new CallStatement( methodName, parameters ); 
	}

	void RuleMethodEnd() {
		while (!(la.kind == 0 || la.kind == 10)) {SynErr(36); Get();}
		Expect(10);
	}

	void Var(out Variable var) {
		WolfGenerator.Core.AST.Type type; 
		Type(out type);
		Expect(1);
		var = new Variable( t.val, type ); 
	}

	void Type(out WolfGenerator.Core.AST.Type type) {
		StringBuilder name = new StringBuilder(); 
		List<WolfGenerator.Core.AST.Type> genericParameters = null; 
		Expect(1);
		name.Append( t.val ); 
		while (la.kind == 16) {
			Get();
			Expect(1);
			name.Append( '.' ); name.Append( t.val ); 
		}
		if (la.kind == 22) {
			genericParameters = new List<WolfGenerator.Core.AST.Type>();
			WolfGenerator.Core.AST.Type generic; 
			Get();
			Type(out generic);
			genericParameters.Add( generic ); 
			while (la.kind == 17) {
				Get();
				Type(out generic);
				genericParameters.Add( generic ); 
			}
			Expect(23);
		}
		type = new WolfGenerator.Core.AST.Type( name.ToString(), genericParameters ); 
	}

	void Apply(out ApplyStatement applyStatement) {
		string methodName; string parameters = null; string from; 
		Expect(5);
		Expect(1);
		methodName = t.val; 
		int startPos = -1; int endPos = -1; 
		if (la.kind == 20) {
			Get();
			startPos = t.pos + t.val.Length; 
			while (StartOf(6)) {
				Get();
			}
			endPos = la.pos; 
			Expect(21);
		} else if (la.kind == 18) {
			Get();
			startPos = t.pos + t.val.Length; 
			while (StartOf(7)) {
				Get();
			}
			endPos = la.pos; 
			Expect(19);
		} else SynErr(37);
		if (startPos > 0 && endPos > 0)
		   parameters = scanner.buffer.GetString( startPos, endPos ).Trim(); 
		while (!(la.kind == 0 || la.kind == 15)) {SynErr(38); Get();}
		Expect(15);
		Expect(1);
		from = t.val; 
		while (!(la.kind == 0 || la.kind == 9)) {SynErr(39); Get();}
		Expect(9);
		applyStatement = new ApplyStatement( methodName, parameters, from ); 
	}


		public void Parse() {
			la = new Token();
			la.val = "";		
			Get();
		WolfGenerator();

		Expect(0);
		}
		
		bool[,] set = {
		{T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, T,x,T,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x},
		{T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,x}

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
			case 2: s = "ruleClass expected"; break;
			case 3: s = "rule expected"; break;
			case 4: s = "join expected"; break;
			case 5: s = "apply expected"; break;
			case 6: s = "call expected"; break;
			case 7: s = "method expected"; break;
			case 8: s = "uses expected"; break;
			case 9: s = "endStatement expected"; break;
			case 10: s = "end expected"; break;
			case 11: s = "value expected"; break;
			case 12: s = "code expected"; break;
			case 13: s = "codeEnd1 expected"; break;
			case 14: s = "codeEnd2 expected"; break;
			case 15: s = "from expected"; break;
			case 16: s = "dot expected"; break;
			case 17: s = "comma expected"; break;
			case 18: s = "openB expected"; break;
			case 19: s = "closB expected"; break;
			case 20: s = "openB1 expected"; break;
			case 21: s = "closB1 expected"; break;
			case 22: s = "openG expected"; break;
			case 23: s = "closG expected"; break;
			case 24: s = "number expected"; break;
			case 25: s = "string expected"; break;
			case 26: s = "badString expected"; break;
			case 27: s = "char expected"; break;
			case 28: s = "text expected"; break;
			case 29: s = "??? expected"; break;
			case 30: s = "this symbol not expected in RuleMethodStart"; break;
			case 31: s = "this symbol not expected in Value"; break;
			case 32: s = "this symbol not expected in Join"; break;
			case 33: s = "invalid Code"; break;
			case 34: s = "invalid Call"; break;
			case 35: s = "this symbol not expected in Call"; break;
			case 36: s = "this symbol not expected in RuleMethodEnd"; break;
			case 37: s = "invalid Apply"; break;
			case 38: s = "this symbol not expected in Apply"; break;
			case 39: s = "this symbol not expected in Apply"; break;

				default: s = "error " + n; break;
			}
			Error(line, col, s);
		}

		public virtual void Error (int lin, int col, string err) {
			scanner.WriteError(errMsgFormat,scanner.srcFile,lin,col,err);
			count++;
		}

	} // Errors

}