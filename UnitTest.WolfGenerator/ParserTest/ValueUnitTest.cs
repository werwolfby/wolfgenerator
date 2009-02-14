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
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class ValueUnitTest
	{
		public static string[] values = new[]
		                                {
		                                	"var",
		                                	"codeWriter.ToString()",
		                                	"\"String\""
		                                };

		[TestMethod]
		public void ValueTest()
		{
			foreach (var value in values)
			{
				var valueStatementText = "<%= " + value + " %>";
				var valueStatement = ParserHelper.ParseValue( valueStatementText );
				Assert.IsNotNull( valueStatement.Value, "Value can't be null" );
				Assert.AreEqual( value.Trim(), valueStatement.Value.Trim() );
			}
		}
	}
}