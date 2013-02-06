/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 15.02.2009 14:23
 *
 * File: RuleMethodUnitTest.cs
 * Remarks:
 * 
 * History:
 *   15.02.2009 14:23 - Create Wireframe
 *   15.02.2009 14:39 - Implement RuleMethodStart check
 *   15.02.2009 14:39 - Implement RuleMethod test
 *   15.02.2009 16:23 - Add StatelessRuleMethodTest - check that variables & statements is empty collection.
 *   21.04.2012 23:30 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using NUnit.Framework;
using WolfGenerator.Core.AST;
using System.Linq;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class RuleMethodUnitTest
	{
		public abstract class RuleStatementChecker 
		{
			protected RuleStatementChecker( string text )
			{
				this.Text = text;
			}

			public string Text { get; private set; }

			public abstract void CheckAssert( RuleStatement statement );
		}

		private class TextRuleStatementChecker : RuleStatementChecker
		{
			public TextRuleStatementChecker( string text ) : base( text ) {}

			public override void CheckAssert( RuleStatement statement )
			{
				Assert.That( statement, Is.TypeOf<TextStatement>() );
				var textStatement = (TextStatement)statement;
				Assert.That( textStatement.Text, Is.EqualTo( this.Text ) );
			}
		}

		private class ValueRuleStatementChecker : RuleStatementChecker
		{
			private readonly string value;

			public ValueRuleStatementChecker( string value, string spaces ) : base( "<%=" + spaces + value + spaces + " %>" )
			{
				this.value = value;
			}

			public override void CheckAssert( RuleStatement statement )
			{
				Assert.That( statement, Is.TypeOf<ValueStatement>() );
				var valueStatement = (ValueStatement)statement;
				Assert.That( valueStatement.Value.Trim(), Is.EqualTo( this.value.Trim() ) );
			}
		}

		[Test]
		public void StatelessRuleMethodTest()
		{
			var ruleMethodStatementText = RuleMethodStartToString( "Test", new Variable[0], ",", "<", ">" ) + "\r\n" + "<%end%>";
			var ruleMethodStatement = ParserHelper.ParseRuleMethod( ruleMethodStatementText );

			Assert.That( ruleMethodStatement.Variables, Is.Not.Null.And.Count.EqualTo( 0 ), "Variables must be not null" );
			Assert.That( ruleMethodStatement.Statements, Is.Not.Null.And.Count.EqualTo( 0 ), "Statements must be not null" );
		}

		[Test]
		public void RuleMethodTest()
		{
			const string expectedName = "Test";
			var expectedVariables = new[] { VariableUnitTest.simpleVariable, VariableUnitTest.listVariable };

			var statements = new RuleStatementChecker[]
			                 {
			                 	new TextRuleStatementChecker( "text" ),
			                 	new ValueRuleStatementChecker( "value", " " ),
			                 	new TextRuleStatementChecker( "text" ),
			                 };

			var ruleMethodStatement = RuleMethodStartToString( expectedName, expectedVariables, ",", "<", ">" ) +
			                          "\r\n" + statements.Select( t => t.Text ).Aggregate( ( s1, s2 ) => s1 + s2 ) +
			                          "\r\n" + "<%end%>";

			var actualRuleMethod = ParserHelper.ParseRuleMethod( ruleMethodStatement );

			Assert.That( actualRuleMethod.Name, Is.EqualTo( expectedName ) );
			AssertHelper.AssertVariables( expectedVariables, actualRuleMethod.Variables );

			for (var i = 0; i < statements.Length; i++)
			{
				var statement = statements[i];
				statement.CheckAssert( actualRuleMethod.Statements[i] );
			}
		}

		public static string RuleMethodStartToString( string expectedName, Variable[] expectedVariables, string ag, string sg, string eg )
		{
			return "<%rule " + expectedName + "( " +
			       (expectedVariables.Length > 0
			        	? expectedVariables
			        	  	.Select( variable => VariableUnitTest.VariableToString( variable, ag, sg, eg ) )
			        	  	.Aggregate( ( s1, s2 ) => s1 + ag + s2 )
			        	: "")
			       + " )%>";
		}
	}
}