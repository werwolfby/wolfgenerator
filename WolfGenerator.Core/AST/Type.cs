/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:54
 *
 * File: Type.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 08:58 - Create Wireframe
 *   25.01.2009 09:01 - Override ToString method.
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Type = ident { "." ident } [ "&lt;" Type { "," Type } "&gt;" ].
	/// </summary>
	public class Type
	{
		private readonly string typeName;
		private readonly IList<Type> genericParameters;

		public Type( string typeName, IList<Type> genericParameters )
		{
			this.typeName = typeName;
			this.genericParameters = genericParameters;
		}

		public string TypeName
		{
			get { return this.typeName; }
		}

		public IList<Type> GenericParameters
		{
			get { return this.genericParameters; }
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append( this.typeName );

			if (this.genericParameters != null)
			{
				var generic = new string[this.genericParameters.Count];

				for (var i = 0; i < this.genericParameters.Count; i++)
					generic[i] = this.genericParameters[i].ToString();

				builder.AppendFormat( "<{0}>", string.Join( ", ", generic ) );
			}

			return builder.ToString();
		}
	}
}