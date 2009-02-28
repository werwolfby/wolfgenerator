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
 *   28.02.2009 15:16 - Add to Parse method out parser parameter.
 *   28.02.2009 15:22 - Add overloaded methods Parse* that can return created Parser_Accessor class.
 *   28.02.2009 15:30 - Add asserErrorCount to check if errors after parse.
 *   28.02.2009 16:09 - Add ParseCode with same signature as others with ignore isStartParameter.
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator
{
	public class ParserHelper
	{
		private delegate void ParseDelegate<T>( Parser_Accessor p, out T t );

		public static Type ParseType( string statement )
		{
			Parser_Accessor parser;
			return ParseType( statement, out parser, true );
		}

		public static Type ParseType( string statement, out Parser_Accessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( Parser_Accessor p, out Type t ) => p.Type( out t ), out parser, asserErrorCount );
		}

		public static Variable ParseVariable( string statement )
		{
			Parser_Accessor parser;
			return ParseVariable( statement, out parser, true );
		}

		public static Variable ParseVariable( string statement, out Parser_Accessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( Parser_Accessor p, out Variable t ) => p.Var( out t ), out parser, asserErrorCount );
		}

		public static ValueStatement ParseValue( string statement )
		{
			Parser_Accessor parser;
			return ParseValue( statement, out parser, true );
		}

		public static ValueStatement ParseValue( string statement, out Parser_Accessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( Parser_Accessor p, out ValueStatement t ) => p.Value( out t ), out parser, asserErrorCount );
		}

		public static ApplyStatement ParseApply( string statement )
		{
			Parser_Accessor parser;
			return ParseApply( statement, out parser, true );
		}

		public static ApplyStatement ParseApply( string statement, out Parser_Accessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( Parser_Accessor p, out ApplyStatement t ) => p.Apply( out t ), out parser, asserErrorCount );
		}

		public static CodeStatement ParseCode( string statement, out bool isStart )
		{
			Parser_Accessor parser;
			return ParseCode( statement, out isStart, out parser, true );
		}

		public static CodeStatement ParseCode( string statement, out Parser_Accessor parser, bool asserErrorCount )
		{
			bool isStart;
			return ParseCode( statement, out isStart, out parser, asserErrorCount );
		}

		public static CodeStatement ParseCode( string statement, out bool isStart, out Parser_Accessor parser, bool asserErrorCount )
		{
			var tempIsStart = false;
			var codeStatement = Parse( statement, ( Parser_Accessor p, out CodeStatement t ) => p.Code( out t, ref tempIsStart ), out parser, asserErrorCount );
			isStart = tempIsStart;
			return codeStatement;
		}

		public static MatchMethodStatement ParseMatch( string statement )
		{
			Parser_Accessor parser;
			return ParseMatch( statement, out parser, true );
		}

		public static MatchMethodStatement ParseMatch( string statement, out Parser_Accessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( Parser_Accessor p, out MatchMethodStatement t ) => p.MatchMethod( out t ), out parser, asserErrorCount );
		}

		public static JoinStatement ParseJoin( string statement )
		{
			Parser_Accessor parser;
			return ParseJoin( statement, out parser, true );
		}

		public static JoinStatement ParseJoin( string statementText, out Parser_Accessor parser, bool asserErrorCount )
		{
			return Parse( statementText, ( Parser_Accessor p, out JoinStatement t ) => p.Join( out t ), out parser, asserErrorCount );
		}

		public static RuleMethodStatement ParseRuleMethod( string statement )
		{
			Parser_Accessor parser;
			return ParseRuleMethod( statement, out parser, true );
		}

		public static RuleMethodStatement ParseRuleMethod( string statement, out Parser_Accessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( Parser_Accessor p, out RuleMethodStatement t ) => p.RuleMethod( out t ), out parser, asserErrorCount );
		}

		public static UsingStatement ParseUsing( string statement )
		{
			Parser_Accessor parser;
			return ParseUsing( statement, out parser, true );
		}

		public static UsingStatement ParseUsing( string statement, out Parser_Accessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( Parser_Accessor p, out UsingStatement t ) => p.Using( out t ), out parser, asserErrorCount );
		}

		public static RuleClassStatement ParseClass( string statement )
		{
			Parser_Accessor parser;
			return ParseClass( statement, out parser, true );
		}

		public static RuleClassStatement ParseClass( string statement, out Parser_Accessor parser, bool asserErrorCount )
		{
			parser = new Parser_Accessor( statement );
			if (asserErrorCount) Assert.AreEqual( 0, parser.errors.count, "Parser has errors: " + parser.errors.count );
			parser.Parse();
			return parser.ruleClassStatement;
		}

		private static T Parse<T>( string statement, ParseDelegate<T> parseMethod, out Parser_Accessor parser, bool asserErrorCount )
		{
			T t;

			parser = new Parser_Accessor( statement );

			parser.InitParse();
			parseMethod( parser, out t );

			if (asserErrorCount) Assert.AreEqual( 0, parser.errors.count, "Parser has errors: " + parser.errors.count );

			return t;
		}
	}
}