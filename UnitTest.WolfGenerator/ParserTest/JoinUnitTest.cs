/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 15.02.2009 11:37
 *
 * File: JoinUnitTest.cs
 * Remarks:
 * 
 * History:
 *   15.02.2009 11:37 - Create Wireframe
 *
 *******************************************************/

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class JoinUnitTest
	{
		private static readonly JoinStatement[] statements = new[]
		                                                     {
		                                                     	new JoinStatement( @"\r\n", new RuleStatement[]
		                                                     	                           {
		                                                     	                           	ApplyUnitTest.simpleApply,
		                                                     	                           	ApplyUnitTest.fromApply,
		                                                     	                           	ApplyUnitTest.extendedApply,
																							ValueUnitTest.values[0],
		                                                     	                           } ),
		                                                     };

		[TestMethod]
		public void JoinTest()
		{
			var joinStatementText = JoinToString( statements[0], " ", "\r\n\t" );

			var actualJoin = ParserHelper.ParseJoin( joinStatementText );

			AssertHelper.AssertJoin( statements[0], actualJoin );
		}

		public static string JoinToString( JoinStatement joinStatement, string spaces, string itemSpaces )
		{
			var builder = new StringBuilder();

			builder.Append( "<%join \"" );
			builder.Append( joinStatement.String );
			builder.Append( "\"%>" );

			foreach (var statement in joinStatement.Statements)
			{
				if (statement is ValueStatement)
					builder.Append( ValueUnitTest.ValueToString( (ValueStatement)statement, spaces ) );
				else if (statement is ApplyStatement)
					builder.Append( ApplyUnitTest.ApplyToString( (ApplyStatement)statement, spaces, true ) );
				else
					Assert.Fail( "Join can contain only statements of type: {0}, {1}", typeof(ValueStatement), typeof(ApplyStatement) );

				builder.Append( itemSpaces );
			}

			builder.Append( "<%end%>" );

			return builder.ToString();
		}
	}
}