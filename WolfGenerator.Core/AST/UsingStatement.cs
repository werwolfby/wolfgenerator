/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:24
 *
 * File: UsingStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 08:24 - Create Wireframe
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	public class UsingStatement
	{
		private readonly string @namespace;

		public UsingStatement( string @namespace )
		{
			this.@namespace = @namespace;
		}

		public string Namespace
		{
			get { return this.@namespace; }
		}
	}
}