/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 28.02.2009 16:17
 *
 * File: StatementPositionUnitTest.cs
 * Remarks:
 * 
 * History:
 *   28.02.2009 16:17 - Create Wireframe
 *   21.04.2012 23:31 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using System.Linq;
using NUnit.Framework;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class StatementPositionUnitTest
	{
		[Test]
		public void StatementPositionTest()
		{
			const string temp = "Test 123123123 ";
			var strings = new[]
			              {
			              	"<%ruleclass Test%>",   // 0
			              	"<%rule Temp()%>",      // 1
			              	temp + "<%= value %>",  // 2 0, 1
			              	"Pre End Text",         // 3 2
			              	"<%= value %>",         // 4 3
			              	"End Text",             // 5 4
			              	"<%end%>",              // 6
			              };

			const string endLine = "\r\n";
			var statementText = string.Join( endLine, strings );

			var ruleClassStatement = ParserHelper.ParseClass( statementText );

			var textStatement1StartPos = string.Join( endLine, strings.Take(2).ToArray() ).Length + endLine.Length;
			var textStatement1EndPos = textStatement1StartPos + temp.Length - 1;

			var ruleMethodStatement = (RuleMethodStatement)ruleClassStatement.RuleMethodStatements[0];
			AssertHelper.AssertStatementPosition( textStatement1StartPos, textStatement1EndPos,
			                                      ruleMethodStatement.Statements[0].StatementPosition );

			// Извратный подсчёт, для UnitTest'а плохой вариант, но так впадлу вручную считать
			AssertHelper.AssertStatementPosition( 3, 3, temp.Length + 1, strings[2].Length, textStatement1EndPos + 1,
			                                      textStatement1EndPos + strings[2].Length - temp.Length,
			                                      ruleMethodStatement.Statements[1].StatementPosition );

			TestText( endLine, strings, 3, 2, ruleMethodStatement.Statements[2].StatementPosition );
			
			TestText( endLine, strings, 5, 1, ruleMethodStatement.Statements[4].StatementPosition );

			AssertHelper.AssertStatementPosition( 1, strings.Length, 1, strings.Last().Length, 0, statementText.Length - 1,
			                                      ruleClassStatement.StatementPosition );
		}

		private static void TestText( string endLine, string[] strings, int index, int endCount, StatementPosition position ) 
		{
			var textStatement2StartPos = string.Join( endLine, strings.Take( index ).ToArray() ).Length;
			var textStatement2EndPos = textStatement2StartPos + strings[index].Length + endCount * endLine.Length - 1;

			AssertHelper.AssertStatementPosition( textStatement2StartPos, textStatement2EndPos, position );
		}
	}
}