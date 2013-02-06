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
 *   21.04.2012 21:35 - [*] Test [DynamicInvoker] instead of [GeneratorBase].
 *   21.04.2012 23:01 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using NUnit.Framework;
using WolfGenerator.Core.Invoker;

namespace UnitTest.WolfGenerator.InvokerTests
{
	[TestFixture]
	public class InvokeTests
	{
		[Test]
		public void SimpleInvokeTest()
		{
			var testGenerator = new TestGenerator();
			var invoker = new DynamicInvoker( testGenerator );

			var typeProperty = new TypeProperty { Name = "Property1", Type = typeof(int) };

			invoker.Invoke<object>( "DefineProperty", typeProperty );

			Assert.That( testGenerator.TypeDefinePropertyCalls, Is.EqualTo( 1 ) );
			Assert.That( testGenerator.NavigationDefinePropertyCalls, Is.EqualTo( 0 ) );

			testGenerator.TypeDefinePropertyCalls = 0;

			var navigationProperty = new ComplexProperty { Name = "Property2", Type = typeof(string) };

			invoker.Invoke<object>( "DefineProperty", navigationProperty );

			Assert.That( testGenerator.TypeDefinePropertyCalls, Is.EqualTo( 0 ) );
			Assert.That( testGenerator.ComplexDefinePropertyCalls, Is.EqualTo( 1 ) );
		}

		[Test]
		public void MatchMethodTest()
		{
			var testGenerator = new TestGenerator();
			var invoker = new DynamicInvoker( testGenerator );

			var typeProperty = new TypeProperty { Name = "TypeProperty1", Type = typeof(int) };

			var navigationProperty1 = new NavigationProperty { Name = "Property1", Property = typeProperty, IsCollection = true };
			var navigationProperty2 = new NavigationProperty { Name = "Property2", Property = typeProperty, IsCollection = false };

			invoker.Invoke<object>( "DefineProperty", navigationProperty1 );
			invoker.Invoke<object>( "DefineProperty", navigationProperty2 );

			Assert.That( testGenerator.TypeDefinePropertyCalls, Is.EqualTo( 0 ) );
			Assert.That( testGenerator.NavigationDefinePropertyCalls, Is.EqualTo( 2 ) );
			Assert.That( testGenerator.NavigationListDefinePropertyCalls, Is.EqualTo( 1 ) );
			Assert.That( testGenerator.NavigationNotListDefinePropertyCalls, Is.EqualTo( 1 ) );
		}
	}
}