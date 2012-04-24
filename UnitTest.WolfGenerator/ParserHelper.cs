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
 *   24.04.2012 17:16 - [*] Use [ParserAccessor] instead of generated [Parser_Accessor].
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.WolfGenerator.Accessors;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator
{
	public class ParserHelper
	{
		private delegate void ParseDelegate<T>( ParserAccessor p, out T t );

		public static Type ParseType( string statement )
		{
			ParserAccessor parser;
			return ParseType( statement, out parser, true );
		}

		public static Type ParseType( string statement, out ParserAccessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( ParserAccessor p, out Type t ) => p.Type( out t ), out parser, asserErrorCount );
		}

		public static Variable ParseVariable( string statement )
		{
			ParserAccessor parser;
			return ParseVariable( statement, out parser, true );
		}

		public static Variable ParseVariable( string statement, out ParserAccessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( ParserAccessor p, out Variable t ) => p.Var( out t ), out parser, asserErrorCount );
		}

		public static ValueStatement ParseValue( string statement )
		{
			ParserAccessor parser;
			return ParseValue( statement, out parser, true );
		}

		public static ValueStatement ParseValue( string statement, out ParserAccessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( ParserAccessor p, out ValueStatement t ) => p.Value( out t ), out parser, asserErrorCount );
		}

		public static ApplyStatement ParseApply( string statement )
		{
			ParserAccessor parser;
			return ParseApply( statement, out parser, true );
		}

		public static ApplyStatement ParseApply( string statement, out ParserAccessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( ParserAccessor p, out ApplyStatement t ) => p.Apply( out t ), out parser, asserErrorCount );
		}

		public static CodeStatement ParseCode( string statement, out bool isStart )
		{
			ParserAccessor parser;
			return ParseCode( statement, out isStart, out parser, true );
		}

		public static CodeStatement ParseCode( string statement, out ParserAccessor parser, bool asserErrorCount )
		{
			bool isStart;
			return ParseCode( statement, out isStart, out parser, asserErrorCount );
		}

		public static CodeStatement ParseCode( string statement, out bool isStart, out ParserAccessor parser, bool asserErrorCount )
		{
			var tempIsStart = false;
			var codeStatement = Parse( statement, ( ParserAccessor p, out CodeStatement t ) => p.Code( out t, ref tempIsStart ), out parser, asserErrorCount );
			isStart = tempIsStart;
			return codeStatement;
		}

		public static MatchMethodStatement ParseMatch( string statement )
		{
			ParserAccessor parser;
			return ParseMatch( statement, out parser, true );
		}

		public static MatchMethodStatement ParseMatch( string statement, out ParserAccessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( ParserAccessor p, out MatchMethodStatement t ) => p.MatchMethod( out t ), out parser, asserErrorCount );
		}

		public static JoinStatement ParseJoin( string statement )
		{
			ParserAccessor parser;
			return ParseJoin( statement, out parser, true );
		}

		public static JoinStatement ParseJoin( string statementText, out ParserAccessor parser, bool asserErrorCount )
		{
			return Parse( statementText, ( ParserAccessor p, out JoinStatement t ) => p.Join( out t ), out parser, asserErrorCount );
		}

		public static RuleMethodStatement ParseRuleMethod( string statement )
		{
			ParserAccessor parser;
			return ParseRuleMethod( statement, out parser, true );
		}

		public static RuleMethodStatement ParseRuleMethod( string statement, out ParserAccessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( ParserAccessor p, out RuleMethodStatement t ) => p.RuleMethod( out t ), out parser, asserErrorCount );
		}

		public static UsingStatement ParseUsing( string statement )
		{
			ParserAccessor parser;
			return ParseUsing( statement, out parser, true );
		}

		public static UsingStatement ParseUsing( string statement, out ParserAccessor parser, bool asserErrorCount )
		{
			return Parse( statement, ( ParserAccessor p, out UsingStatement t ) => p.Using( out t ), out parser, asserErrorCount );
		}

		public static RuleClassStatement ParseClass( string statement )
		{
			ParserAccessor parser;
			return ParseClass( statement, out parser, true );
		}

		public static RuleClassStatement ParseClass( string statement, out ParserAccessor parser, bool asserErrorCount )
		{
			parser = new ParserAccessor( statement );
			if (asserErrorCount) Assert.AreEqual( 0, parser.Errors.count, "Parser has errors: " + parser.Errors.count );
			parser.Parse();
			return parser.RuleClassStatement;
		}

		private static T Parse<T>( string statement, ParseDelegate<T> parseMethod, out ParserAccessor parser, bool asserErrorCount )
		{
			T t;

			parser = new ParserAccessor( statement );

			parser.InitParse();
			parseMethod( parser, out t );

			if (asserErrorCount) Assert.AreEqual( 0, parser.Errors.count, "Parser has errors: " + parser.Errors.count );

			return t;
		}
	}
}