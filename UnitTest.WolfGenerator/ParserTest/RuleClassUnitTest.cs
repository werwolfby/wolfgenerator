/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 18.02.2009 00:06
 *
 * File: RuleClassUnitTest.cs
 * Remarks:
 * 
 * History:
 *   18.02.2009 00:06 - Create Wireframe
 *   18.02.2009 00:22 - Finish first implementation
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class RuleClassUnitTest
	{
		[TestMethod]
		public void RuleClassTest()
		{
			var expectedName = "TestClass";
			var ruleClassStatementText = "<%ruleclass " + expectedName + "%>\r\n" +
			                             UsingUnitTest.statements.Select( s => UsingUnitTest.UsingToString( s, " " ) ).
			                             	Aggregate( ( s1, s2 ) => s1 + "\r\n" + s2 ) + "\r\n\r\n" + 
											"<%end%>";

			var ruleClass = ParserHelper.ParseClass( ruleClassStatementText );

			Assert.AreEqual( expectedName, ruleClass.Name, "Class Name" );
			CollectionAssert.AreEquivalent( UsingUnitTest.statements, ruleClass.UsingStatements.ToArray(), "UsingStatements" );

			Assert.IsNotNull( ruleClass.RuleMethodStatements, "RuleMethodStatements" );
			Assert.IsNotNull( ruleClass.MatchMethodGroups, "RuleMethodStatements" );

			Assert.AreEqual( 0, ruleClass.RuleMethodStatements.Count, "RuleMethodStatements.Count" );
			Assert.AreEqual( 0, ruleClass.MatchMethodGroups.Count, "MatchMethodGroups.Count" );
		}
	}
}