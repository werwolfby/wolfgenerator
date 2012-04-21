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
using WolfGenerator.Core.Invoker;
using WolfGenerator.Core.Writer;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class CallTest
	{
		[TestMethod]
		public void SimpleTest()
		{
			var generator = new Generator();
			var dynamicInvoker = new DynamicInvoker( generator );

			var callStatement = new CallStatement( Helper.EmptyPosition, "Name", "param1, param2" );

			var invokeCodeWriter = dynamicInvoker.Invoke<CodeWriter>( "GenerateInvoke", callStatement );

			Assert.IsNotNull( invokeCodeWriter );

			var invokeText = invokeCodeWriter.ToString().Replace( " ", "" );

			Assert.AreEqual( "this.Invoke(\"Name\",param1,param2)", invokeText );
		}

		[TestMethod]
		public void EmptyAndNullParametersTest()
		{
			var generator = new Generator();
			var dynamicInvoker = new DynamicInvoker( generator );

			EmptyAndNullParametersTestHelper( dynamicInvoker, "" );
			EmptyAndNullParametersTestHelper( dynamicInvoker, null );
		}

		private static void EmptyAndNullParametersTestHelper( DynamicInvoker dynamicInvoker, string parameters )
		{
			var callStatement = new CallStatement( Helper.EmptyPosition, "Name", parameters );

			var invokeCodeWriter = dynamicInvoker.Invoke<CodeWriter>( "GenerateInvoke", callStatement );

			Assert.IsNotNull( invokeCodeWriter );

			var invokeText = invokeCodeWriter.ToString().Replace( " ", "" );

			Assert.AreEqual( "this.Invoke(\"Name\")", invokeText );
		}
	}
}