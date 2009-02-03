/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 04.02.2009 01:05
 *
 * File: GeneratorBase.cs
 * Remarks:
 * 
 * History:
 *   04.02.2009 01:05 - Create Wireframe
 *   04.02.2009 01:15 - Make constructor protected.
 *
 *******************************************************/

using System;
using System.Reflection;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core
{
	public class GeneratorBase
	{
		protected GeneratorBase() {}

		public CodeWriter Invoke( string name, params object[] parameters )
		{
			return (CodeWriter)this.GetType().InvokeMember( name, BindingFlags.Instance | BindingFlags.InvokeMethod |
			                                                      BindingFlags.Public,
			                                                Type.DefaultBinder, this, parameters );
		}
	}
}