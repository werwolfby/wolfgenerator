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
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.Compiler;

namespace UnitTest.WolfGenerator.CompilerTests
{
	[TestClass]
	public class CompilerTest
	{
		[TestMethod]
		[DeploymentItem("CompilerTests\\Test1.rule", "CompilerTests")]
		public void SimpleTest()
		{
			var compiler = new Compiler( "CompilerTests\\Test1.rule" );

			var result = compiler.Compile("2.cs");

			Assert.AreEqual( 0, result.Errors.Count );
		}
	}
}