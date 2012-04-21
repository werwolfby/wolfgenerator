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
 *
 *******************************************************/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core;
using WolfGenerator.Core.Parsing;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserErrorTest
{
	[TestClass]
	public class EndStatementUnitTest
	{
		private delegate T ParseDelegate<T>( string statement, out Parser_Accessor parser, bool assertErrorsCount );

		[TestMethod]
		public void RuleClassEndTest()
		{
			var text = "<%ruleclass Test%>";
			MainTest<RuleClassStatement>( text, text.Length + 1, ParserHelper.ParseClass,
			                              ( accessor, statement ) => Assert.AreEqual( "Test", statement.Name ) );
		}

		[TestMethod]
		public void RuleMethodEndTest()
		{
			var text = "<%rule Test()%>";
			MainTest<RuleMethodStatement>( text, text.Length + 1, ParserHelper.ParseRuleMethod,
			                               ( accessor, statement ) => Assert.AreEqual( "Test", statement.Name ) );
		}

		[TestMethod]
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

		private static void MainTest<T>( string text, int column, ParseDelegate<T> parseDelegate, Action<Parser_Accessor, T> someOtherTest )
		{
			Parser_Accessor parser;
			var statement = parseDelegate( text, out parser, false );

			if (someOtherTest != null)
			{
				someOtherTest( parser, statement );
			}
			
			Assert.AreEqual( 1, parser.errors.count, "Expected one error" );	
			AssertHelper.AssertError( parser.errors.ErrorDatas[0], 1, column, "end expected" );
		}
	}
}