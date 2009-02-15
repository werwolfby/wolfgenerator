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
		[TestMethod]
		public void SimpleRuleMethodStartTest()
		{
			var expectedName = "Test";
			var expectedVariables = new[] { VariableUnitTest.simpleVariable, VariableUnitTest.listVariable };
			var ruleMethodStartStatementText = RuleMethodStartToString( expectedName, expectedVariables, ",", "<", ">" );

			string name;
			IList<Variable> variables;
			ParserHelper.ParseRuleMethodStart( ruleMethodStartStatementText, out name, out variables );

			Assert.AreEqual( expectedName, name, "Rule name dismatch" );
			AssertHelper.AssertVariables( expectedVariables, variables );
		}

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
			       expectedVariables
			       	.Select( variable => VariableUnitTest.VariableToString( variable, ag, sg, eg ) )
			       	.Aggregate( ( s1, s2 ) => s1 + ag + s2 )
			       + " )%>";
		}
	}
}