/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 21.04.2012 08:28
 *
 * File: InvokeTests.cs
 * Remarks:
 * 
 * History:
 *   21.04.2012 08:28 - Create Wireframe
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.WolfGenerator.GeneratorTest
{
	[TestClass]
	public class InvokeTests
	{
		[TestMethod]
		public void SimpleInvokeTest()
		{
			var testGenerator = new TestGenerator();

			var typeProperty = new TypeProperty { Name = "Property1", Type = typeof(int) };

			testGenerator.CallDefineProperty( typeProperty );

			Assert.AreEqual( 1, testGenerator.TypeDefinePropertyCalls );
			Assert.AreEqual( 0, testGenerator.NavigationDefinePropertyCalls );

			testGenerator.TypeDefinePropertyCalls = 0;

			var navigationProperty = new NavigationProperty { Name = "Property2", Property = typeProperty, IsCollection = true };

			testGenerator.CallDefineProperty( navigationProperty );

			Assert.AreEqual( 0, testGenerator.TypeDefinePropertyCalls );
			Assert.AreEqual( 1, testGenerator.NavigationDefinePropertyCalls );
		}
	}
}