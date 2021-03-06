﻿/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 21.04.2012 08:29
 *
 * File: TestGenerator.cs
 * Remarks:
 * 
 * History:
 *   21.04.2012 08:29 - Create Wireframe
 *   21.04.2012 21:34 - [-] Remove inheritance from [GeneratorBase].
 *
 *******************************************************/

using System;
using WolfGenerator.Core;

namespace UnitTest.WolfGenerator.InvokerTests
{
	public abstract class Property
	{
		public string Name { get; set; }
	}

	public class TypeProperty : Property
	{
		public Type Type { get; set; }
	}

	public class ComplexProperty : Property
	{
		public Type Type { get; set; }
	}

	public class NavigationProperty : Property
	{
		public Property Property { get; set; }

		public bool IsCollection { get; set; }
	}

	public class TestGenerator
	{
		public int TypeDefinePropertyCalls { get; set; }
		
		public int ComplexDefinePropertyCalls { get; set; }
		
		public int NavigationDefinePropertyCalls { get; set; }
		
		public int NavigationListDefinePropertyCalls { get; set; }
		
		public int NavigationNotListDefinePropertyCalls { get; set; }

		[RuleMethod("DefineProperty", null, null)]
		public void DefineProperty( TypeProperty typeProperty )
		{
			this.TypeDefinePropertyCalls++;
		}

		[MatchMethod("DefineProperty", "IsList", null)]
		private bool MatchDefinePropertyIsList( NavigationProperty navigationProperty )
		{
			return navigationProperty.IsCollection;
		}

		[RuleMethod("DefineProperty", "IsList", null)]
		public void DefinePropertyIsList( NavigationProperty navigationProperty )
		{
			this.NavigationDefinePropertyCalls++;
			this.NavigationListDefinePropertyCalls++;
		}

		[MatchMethod("DefineProperty", "IsNotList", null)]
		private bool MatchDefinePropertyIsNotList( NavigationProperty navigationProperty )
		{
			return !navigationProperty.IsCollection;
		}

		[RuleMethod("DefineProperty", "IsNotList", null)]
		public void DefinePropertyIsNotList( NavigationProperty navigationProperty )
		{
			this.NavigationDefinePropertyCalls++;
			this.NavigationNotListDefinePropertyCalls++;
		}

		[RuleMethod("DefineProperty", null, null)]
		public void DefineProperty( ComplexProperty navigationProperty )
		{
			this.ComplexDefinePropertyCalls++;
		}
	}
}