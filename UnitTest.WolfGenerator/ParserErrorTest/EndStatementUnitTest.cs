/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 28.02.2009 15:12
 *
 * File: EndStatementUnitTest.cs
 * Remarks:
 * 
 * History:
 *   28.02.2009 15:12 - Create Wireframe
 *   21.04.2012 23:05 - [*] Migrate to [NUnit].
 *   24.04.2012 17:16 - [*] Use [ParserAccessor] instead of generated [Parser_Accessor].
 *
 *******************************************************/

using System;
using NUnit.Framework;
using UnitTest.WolfGenerator.Accessors;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserErrorTest
{
	[TestFixture]
	public class EndStatementUnitTest
	{
		private delegate T ParseDelegate<T>( string statement, out ParserAccessor parser, bool assertErrorsCount );

		[Test]
		public void RuleClassEndTest()
		{
			var text = "<%ruleclass Test%>";
			MainTest<RuleClassStatement>( text, text.Length + 1, ParserHelper.ParseClass,
			                              ( accessor, statement ) => Assert.AreEqual( "Test", statement.Name ) );
		}

		[Test]
		public void RuleMethodEndTest()
		{
			var text = "<%rule Test()%>";
			MainTest<RuleMethodStatement>( text, text.Length + 1, ParserHelper.ParseRuleMethod,
			                               ( accessor, statement ) => Assert.AreEqual( "Test", statement.Name ) );
		}

		[Test]
		public void JoinEndTest()
		{
			var text = "<%join \"\\r\\n\"%><%= value %><%apply Test( item ) from from %>";
			MainTest<JoinStatement>( text, text.Length + 1, ParserHelper.ParseJoin,
			                         ( accessor, statement ) =>
			                         {
			                         	Assert.AreEqual( "\\r\\n", statement.String );
			                         	Assert.AreEqual( 2, statement.Statements.Count );
			                         } );
		}

		private static void MainTest<T>( string text, int column, ParseDelegate<T> parseDelegate, Action<ParserAccessor, T> someOtherTest )
		{
			ParserAccessor parser;
			var statement = parseDelegate( text, out parser, false );

			if (someOtherTest != null)
			{
				someOtherTest( parser, statement );
			}

			Assert.That( parser.Errors.count, Is.EqualTo( 1 ), "Expected one error" );
			AssertHelper.AssertError( parser.Errors.ErrorDatas[0], 1, column, "end expected" );
		}
	}
}