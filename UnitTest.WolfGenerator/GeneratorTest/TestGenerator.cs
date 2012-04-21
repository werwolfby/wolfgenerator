/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 21.04.2012 08:29
 *
 * File: TestGenerator.cs
 * Remarks:
 * 
 * History:
 *   21.04.2012 08:29 - Create Wireframe
 *
 *******************************************************/

using System;
using WolfGenerator.Core;

namespace UnitTest.WolfGenerator.GeneratorTest
{
	public abstract class Property
	{
		public string Name { get; set; }
	}

	public class TypeProperty : Property
	{
		public Type Type { get; set; }
	}

	public class NavigationProperty : Property
	{
		public Property Property { get; set; }

		public bool IsCollection { get; set; }
	}

	public class TestGenerator : GeneratorBase
	{
		public int TypeDefinePropertyCalls { get; set; }
		
		public int NavigationDefinePropertyCalls { get; set; }

		public void DefineProperty( TypeProperty typeProperty )
		{
			this.TypeDefinePropertyCalls++;
		}

		public void DefineProperty( NavigationProperty navigationProperty )
		{
			this.NavigationDefinePropertyCalls++;
		}

		public void CallDefineProperty( Property property )
		{
			this.Invoke( "DefineProperty", property );
		}
	}
}