﻿/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 24.04.2012 15:50
 *
 * File: ParserAccessor.cs
 * Remarks:
 * 
 * History:
 *   24.04.2012 15:50 - Create Wireframe
 *   24.04.2012 16:43 - [*] Extract common [Call] method.
 *   24.04.2012 16:48 - [+] Add other methods.
 *   24.04.2012 17:13 - [+] Delegate some other methods.
 *   24.04.2012 17:15 - [+] Add not working [Code] method.
 *
 *******************************************************/

using System.Reflection;
using WolfGenerator.Core.AST;
using WolfGenerator.Core.Parsing;

namespace UnitTest.WolfGenerator.Accessors
{
	public class ParserAccessor
	{
		private readonly Parser parser;
		private readonly System.Type parserType = typeof(Parser);

		public ParserAccessor( string source )
		{
			this.parser = new Parser( source );
		}

		public void InitParse()
		{
			this.parser.InitParse();
		}

		public void Parse()
		{
			this.parser.Parse();
		}

		public RuleClassStatement RuleClassStatement
		{
			get { return this.parser.ruleClassStatement; }
		}

		public Errors Errors
		{
			get { return this.parser.errors; }
		}

		public void Type( out Type statement )
		{
			this.Call( "Type", out statement );
		}

		public void Value( out ValueStatement statement )
		{
			this.Call( "Value", out statement );
		}

		public void Var( out Variable statement )
		{
			this.Call( "Var", out statement );
		}

		public void Apply( out ApplyStatement statement )
		{
			this.Call( "Apply", out statement );
		}

		public void Code( out CodeStatement statement, ref bool isStart )
		{
			var method = this.parserType.GetMethod( "Code", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			var args = new object[2];
			method.Invoke( this.parser, args );
			statement = (CodeStatement)args[0];
			isStart = (bool)args[1];
		}

		public void MatchMethod( out MatchMethodStatement statement )
		{
			this.Call( "MatchMethod", out statement );
		}

		public void Join( out JoinStatement statement )
		{
			this.Call( "Join", out statement );
		}

		public void RuleMethod( out RuleMethodStatement statement )
		{
			this.Call( "RuleMethod", out statement );
		}

		public void Using( out UsingStatement statement )
		{
			this.Call( "Using", out statement );
		}

		private void Call<T>( string name, out T type )
		{
			var method = this.parserType.GetMethod( name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			var args = new object[1];
			method.Invoke( this.parser, args );
			type = (T)args[0];
		}
	}
}