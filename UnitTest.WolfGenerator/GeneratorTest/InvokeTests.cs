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

			var navigationProperty = new ComplexProperty { Name = "Property2", Type = typeof(string) };

			testGenerator.CallDefineProperty( navigationProperty );

			Assert.AreEqual( 0, testGenerator.TypeDefinePropertyCalls );
			Assert.AreEqual( 1, testGenerator.ComplexDefinePropertyCalls );
		}

		[TestMethod]
		public void MatchMethodTest()
		{
			var testGenerator = new TestGenerator();

			var typeProperty = new TypeProperty { Name = "TypeProperty1", Type = typeof(int) };

			var navigationProperty1 = new NavigationProperty { Name = "Property1", Property = typeProperty, IsCollection = true };
			var navigationProperty2 = new NavigationProperty { Name = "Property2", Property = typeProperty, IsCollection = false };

			testGenerator.CallDefineProperty( navigationProperty1 );
			testGenerator.CallDefineProperty( navigationProperty2 );

			Assert.AreEqual( 0, testGenerator.TypeDefinePropertyCalls );
			Assert.AreEqual( 2, testGenerator.NavigationDefinePropertyCalls );
			Assert.AreEqual( 1, testGenerator.NavigationListDefinePropertyCalls );
			Assert.AreEqual( 1, testGenerator.NavigationNotListDefinePropertyCalls );
		}
	}
}