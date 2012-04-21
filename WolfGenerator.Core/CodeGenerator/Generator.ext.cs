/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.02.2009 20:23
 *
 * File: Generator.Ext
 * Remarks:
 * 
 * History:
 *   25.02.2009 20:23 - Create Wireframe
 *
 *******************************************************/

namespace WolfGenerator.Core.CodeGenerator
{
	public partial class Generator
	{
		private static readonly string[] defaultNamespaces = new[]
		                                                     {
		                                                     	"System",
		                                                     	"System.Collections.Generic",
		                                                     	"WolfGenerator.Core.Writer",
		                                                     	"WolfGenerator.Core",
		                                                     };

		/// <summary>
		/// Exported from .NET Framework 4.0 sources
		/// </summary>
		/// <param name="value">string</param>
		/// <returns>true is string is null or whitespace</returns>
		public static bool IsNullOrWhiteSpace( string value )
		{
			if (value == null) return true;

			for (var i = 0; i < value.Length; i++) 
				if (!char.IsWhiteSpace( value[i] )) return false;

			return true;
		}
	}
}