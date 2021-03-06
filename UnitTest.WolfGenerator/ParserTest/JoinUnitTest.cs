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
 *   21.04.2012 23:25 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using System.Text;
using NUnit.Framework;
using WolfGenerator.Core.AST;
using WolfGenerator.Core.Writer;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class JoinUnitTest
	{
		private static readonly JoinStatement[] statements = new[]
		                                                     {
		                                                     	new JoinStatement( Helper.EmptyPosition, @"\r\n",
		                                                     	                   AppendType.EmptyLastLine,
		                                                     	                   new RuleStatement[]
		                                                     	                   {
		                                                     	                   	ApplyUnitTest.simpleApply,
		                                                     	                   	ApplyUnitTest.fromApply,
		                                                     	                   	ApplyUnitTest.extendedApply,
		                                                     	                   	ValueUnitTest.values[0],
		                                                     	                   } ),
		                                                     };

		[Test]
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