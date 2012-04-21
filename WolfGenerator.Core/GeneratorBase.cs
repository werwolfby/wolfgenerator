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
 *   11.02.2009 20:51 - Add InnerInvoke method.
 *   11.02.2009 21:41 - Finish first implementation of match methods.
 *   12.02.2009 20:55 - Change field type of some inner classes from MethodInfo to string, 
 *                      because we use only name of method.
 *   21.04.2012 08:26 - [+] Add [CheckParams] method, for check [MatchMethod] parameters before call the match method.
 *   21.04.2012 20:30 - [*] Move all code that make dynamic invoke in [DynamicInvoker] class.
 *
 *******************************************************/

using WolfGenerator.Core.Invoker;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core
{
	public class GeneratorBase
	{
		private readonly DynamicInvoker dynamicInvoker;

		protected GeneratorBase()
		{
			this.dynamicInvoker = new DynamicInvoker( this );
		}

		public CodeWriter Invoke( string name, params object[] parameters )
		{
			return dynamicInvoker.Invoke<CodeWriter>( name, parameters );
		}
	}
}