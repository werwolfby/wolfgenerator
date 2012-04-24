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
 *   21.04.2012 22:14 - [*] Rename [EmptyAndNullParametersTest] to [NullOrWhitespaceParametersTest] and add testing for whitespace.
 *   21.04.2012 23:21 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using NUnit.Framework;
using WolfGenerator.Core.AST;
using WolfGenerator.Core.CodeGenerator;
using WolfGenerator.Core.Invoker;
using WolfGenerator.Core.Writer;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class CallTest
	{
		[Test]
		public void SimpleTest()
		{
			var generator = new Generator();
			var dynamicInvoker = new DynamicInvoker( generator );

			var callStatement = new CallStatement( Helper.EmptyPosition, "Name", "param1, param2" );

			var invokeCodeWriter = dynamicInvoker.Invoke<CodeWriter>( "GenerateInvoke", callStatement );

			Assert.IsNotNull( invokeCodeWriter );

			var invokeText = invokeCodeWriter.ToString().Replace( " ", "" );

			Assert.That( invokeText, Is.EqualTo( "this.Invoke(\"Name\",param1,param2)" ) );
		}

		[Test]
		public void NullOrWhitespaceParametersTest()
		{
			var generator = new Generator();
			var dynamicInvoker = new DynamicInvoker( generator );

			EmptyAndNullParametersTestHelper( dynamicInvoker, "" );
			EmptyAndNullParametersTestHelper( dynamicInvoker, null );
			EmptyAndNullParametersTestHelper( dynamicInvoker, "    " );
			EmptyAndNullParametersTestHelper( dynamicInvoker, "\r\n" );
			EmptyAndNullParametersTestHelper( dynamicInvoker, "\t\t\t" );
		}

		private static void EmptyAndNullParametersTestHelper( DynamicInvoker dynamicInvoker, string parameters )
		{
			var callStatement = new CallStatement( Helper.EmptyPosition, "Name", parameters );

			var invokeCodeWriter = dynamicInvoker.Invoke<CodeWriter>( "GenerateInvoke", callStatement );

			Assert.IsNotNull( invokeCodeWriter );

			var invokeText = invokeCodeWriter.ToString().Replace( " ", "" );

			Assert.That( invokeText, Is.EqualTo( "this.Invoke(\"Name\")" ) );
		}
	}
}