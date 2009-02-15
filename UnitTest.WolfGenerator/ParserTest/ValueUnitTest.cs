/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 14.02.2009 22:17
 *
 * File: ValueUnitTest.cs
 * Remarks:
 * 
 * History:
 *   14.02.2009 22:17 - Create Wireframe
 *   15.02.2009 11:41 - Make values field array of ValueStatement.
 *   15.02.2009 13:28 - Extract AssertValue and move it to AssertHelper.
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class ValueUnitTest
	{
		public static ValueStatement[] values = new[]
		                                        {
		                                        	new ValueStatement( "var" ),
		                                        	new ValueStatement( "codeWriter.ToString()" ),
		                                        	new ValueStatement( "\"String\"" ),
		                                        };

		[TestMethod]
		public void ValueTest()
		{
			foreach (var value in values)
			{
				var valueStatementText = ValueToString( value, " " );
				var valueStatement = ParserHelper.ParseValue( valueStatementText );
				AssertHelper.AssertValue( value, valueStatement );
			}
		}

		public static string ValueToString( ValueStatement valueStatement, string spaces )
		{
			return "<%=" + spaces + valueStatement.Value + spaces + "%>";
		}
	}
}