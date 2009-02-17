/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 14.02.2009 12:52
 *
 * File: ParserHelper.cs
 * Remarks:
 * 
 * History:
 *   14.02.2009 12:52 - Create Wireframe
 *   14.02.2009 21:53 - Add ParseVariable method.
 *   14.02.2009 22:24 - Add base parser method `Parse`.
 *   14.02.2009 22:27 - Add ParseValue and use all old methods by using `Parse` method.
 *   15.02.2009 10:42 - Add ParseApply method.
 *   15.02.2009 11:25 - Add ParseCode method.
 *   15.02.2009 11:50 - Add ParseJoin method.
 *   15.02.2009 14:20 - Add ParseMatch method.
 *   15.02.2009 14:20 - Add ParseRuleMethodStart method.
 *   15.02.2009 16:28 - Add ParseUsing method.
 *   15.02.2009 16:33 - Change parse methods from anonymous method to delegate.
 *   18.02.2009 00:10 - Add ParseClass method.
 *
 *******************************************************/

using System.Collections.Generic;
using WolfGenerator.Core;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator
{
	public class ParserHelper
	{
		private delegate void ParseDelegate<T>( Parser_Accessor p, out T t );

		public static Type ParseType( string statement )
		{
			return Parse( statement, ( Parser_Accessor p, out Type t ) => p.Type( out t ) );
		}

		public static Variable ParseVariable( string statement )
		{
			return Parse( statement, ( Parser_Accessor p, out Variable t ) => p.Var( out t ) );
		}

		public static ValueStatement ParseValue( string statement )
		{
			return Parse( statement, ( Parser_Accessor p, out ValueStatement t ) => p.Value( out t ) );
		}

		public static ApplyStatement ParseApply( string statement )
		{
			return Parse( statement, ( Parser_Accessor p, out ApplyStatement t ) => p.Apply( out t ) );
		}

		public static CodeStatement ParseCode( string statement, out bool isStart )
		{
			var tempIsStart = false;
			var codeStatement = Parse( statement, ( Parser_Accessor p, out CodeStatement t ) => p.Code( out t, ref tempIsStart ) );
			isStart = tempIsStart;
			return codeStatement;
		}

		public static MatchMethodStatement ParseMatch( string statement )
		{
			return Parse( statement, ( Parser_Accessor p, out MatchMethodStatement t ) => p.MatchMethod( out t ) );
		}

		public static JoinStatement ParseJoin( string statementText )
		{
			return Parse( statementText, ( Parser_Accessor p, out JoinStatement t ) => p.Join( out t ) );
		}

		public static void ParseRuleMethodStart( string statement, out string name, out IList<Variable> variables )
		{
			string n = null;
			IList<Variable> var = null;
			Parse( statement, delegate( Parser_Accessor p, out RuleStatement s )
			                  {
			                  	s = null;
			                  	p.RuleMethodStart( out n, out var );
			                  } );
			name = n;
			variables = var;
		}

		public static RuleMethodStatement ParseRuleMethod( string statement )
		{
			return Parse( statement, ( Parser_Accessor p, out RuleMethodStatement t ) => p.RuleMethod( out t ) );
		}

		public static UsingStatement ParseUsing( string statement )
		{
			return Parse( statement, ( Parser_Accessor p, out UsingStatement t ) => p.Using( out t ) );
		}

		public static RuleClassStatement ParseClass( string statement )
		{
			var parser = new Parser( statement );
			parser.Parse();
			return parser.ruleClassStatement;
		}

		private static T Parse<T>( string statement, ParseDelegate<T> parseMethod )
		{
			T t;

			var parser = new Parser_Accessor( statement );

			parser.InitParse();
			parseMethod( parser, out t );

			return t;
		}
	}
}