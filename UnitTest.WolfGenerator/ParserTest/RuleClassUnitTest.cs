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
 *   21.04.2012 23:27 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using System.Linq;
using NUnit.Framework;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class RuleClassUnitTest
	{
		[Test]
		public void RuleClassTest()
		{
			const string expectedName = "TestClass";

			var ruleClassStatementText = "<%ruleclass " + expectedName + "%>\r\n" +
			                             UsingUnitTest.statements.Select( s => UsingUnitTest.UsingToString( s, " " ) ).
			                             	Aggregate( ( s1, s2 ) => s1 + "\r\n" + s2 ) + "\r\n\r\n" +
			                             "<%end%>";

			var ruleClass = ParserHelper.ParseClass( ruleClassStatementText );

			Assert.That( ruleClass.Name, Is.EqualTo( expectedName ), "Class Name" );
			Assert.That( ruleClass.UsingStatements.ToArray(), Is.EquivalentTo( UsingUnitTest.statements ), "UsingStatements" );

			Assert.That( ruleClass.RuleMethodStatements, Is.Not.Null.And.Count.EqualTo( 0 ), "RuleMethodStatements" );
			Assert.That( ruleClass.MatchMethodGroups, Is.Not.Null.And.Count.EqualTo( 0 ), "MatchMethodStatements" );
		}
	}
}