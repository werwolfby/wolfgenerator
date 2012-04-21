/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 21.04.2012 21:52
 *
 * File: CallTest.cs
 * Remarks:
 * 
 * History:
 *   21.04.2012 21:52 - Create Wireframe
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.AST;
using WolfGenerator.Core.CodeGenerator;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class CallTest
	{
		[TestMethod]
		public void SimpleTest()
		{
			var generator = new Generator();

			var callStatement = new CallStatement( Helper.EmptyPosition, "Name", "param1, param2" );

			var invokeCodeWriter = generator.GenerateInvoke( callStatement );

			Assert.IsNotNull( invokeCodeWriter );

			var invokeText = invokeCodeWriter.ToString().Replace( " ", "" );

			Assert.AreEqual( "this.Invoke(\"Name\",param1,param2)", invokeText );
		}
	}
}