/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 21.04.2012 22:20
 *
 * File: CompilerTest.cs
 * Remarks:
 * 
 * History:
 *   21.04.2012 22:20 - Create Wireframe
 *   21.04.2012 23:00 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using NUnit.Framework;
using WolfGenerator.Core.Compiler;

namespace UnitTest.WolfGenerator.CompilerTests
{
	[TestFixture]
	public class CompilerTest
	{
		[Test]
		public void SimpleTest()
		{
			var compiler = new Compiler( "CompilerTests\\Test1.rule" );

			var result = compiler.Compile();

			Assert.AreEqual( 0, result.Errors.Count );
		}
	}
}