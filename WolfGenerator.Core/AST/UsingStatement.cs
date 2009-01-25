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
 *   25.01.2009 08:39 - Add EBNF.
 *   25.01.2009 08:48 - Override ToString method.
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Using = <%using ident { .  ident } %>.
	/// </summary>
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

		public override string ToString()
		{
			return "<%using " + @namespace + "%>";
		}
	}
}