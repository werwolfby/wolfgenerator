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
 *   21.04.2012 23:18 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using NUnit.Framework;
using WolfGenerator.Core.Writer;

namespace UnitTest.WolfGenerator.Writer
{
	[TestFixture]
	public class LineUnitTest
	{
		[Test]
		public void IndentLineTest()
		{
			const int n = 3;
			const string expectedEnd = "Test";
			const char indent = '\t';

			var expected = new string( indent, n ) + expectedEnd;

			var line = new Line( n, expectedEnd );

			Assert.That( line.ToString( indent.ToString() ), Is.EqualTo( expected ) );
		}

		[Test]
		public void AppendTextTest()
		{
			const int n = 3;
			const string expectedEnd1 = "Test";
			const string expectedEnd2 = "Simple";
			const char indent = '\t';

			var expected = new string( indent, n ) + expectedEnd1 + expectedEnd2;

			var line = new Line( n, expectedEnd1 );
			line.Append( expectedEnd2 );

			Assert.That( line.ToString( indent.ToString() ), Is.EqualTo( expected ) );
		}
	}
}