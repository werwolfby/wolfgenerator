/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 18.02.2009 00:35
 *
 * File: LineUnitTest.cs
 * Remarks:
 * 
 * History:
 *   18.02.2009 00:35 - Create Wireframe
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.Writer;

namespace UnitTest.WolfGenerator.Writer
{
	[TestClass]
	public class LineUnitTest
	{
		[TestMethod]
		public void IndentLineTest()
		{
			var n = 3;
			var expectedEnd = "Test";
            var indent = '\t';

			var expected = new string( indent, n ) + expectedEnd;
            
			var line = new Line( n, expectedEnd );

			Assert.AreEqual( expected, line.ToString( indent.ToString() ) );
		}

		[TestMethod]
		public void AppendTextTest()
		{
			var n = 3;
			var expectedEnd1 = "Test";
			var expectedEnd2 = "Simple";
            var indent = '\t';

			var expected = new string( indent, n ) + expectedEnd1 + expectedEnd2;
            
			var line = new Line( n, expectedEnd1 );
			line.Append( expectedEnd2 );

			Assert.AreEqual( expected, line.ToString( indent.ToString() ) );
		}
	}
}