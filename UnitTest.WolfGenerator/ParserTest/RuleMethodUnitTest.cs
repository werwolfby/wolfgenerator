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
 *
 *******************************************************/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.AST;
using System.Linq;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
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
				Assert.IsInstanceOfType( statement, typeof(TextStatement) );
				var textStatement = (TextStatement)statement;
				Assert.AreEqual( this.Text, textStatement.Text );
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
				Assert.IsInstanceOfType( statement, typeof(ValueStatement) );
				var valueStatement = (ValueStatement)statement;
				Assert.AreEqual( this.value.Trim(), valueStatement.Value.Trim() );
			}
		}

		[TestMethod]
		public void StatelessRuleMethodTest()
		{
			var ruleMethodStatementText = RuleMethodStartToString( "Test", new Variable[0], ",", "<", ">" ) + "\r\n" + "<%end%>";
			var ruleMethodStatement = ParserHelper.ParseRuleMethod( ruleMethodStatementText );

			Assert.IsNotNull( ruleMethodStatement.Variables, "Variables must be not null" );
			Assert.IsNotNull( ruleMethodStatement.Statements, "Statements must be not null" );

			Assert.AreEqual( 0, ruleMethodStatement.Variables.Count, "Such rule method mustn't contains any variable" );
			Assert.AreEqual( 0, ruleMethodStatement.Statements.Count, "Such rule method mustn't contains any statement" );
		}

		[TestMethod]
		public void RuleMethodTest()
		{
			var expectedName = "Test";
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

			Assert.AreEqual( expectedName, actualRuleMethod.Name );
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