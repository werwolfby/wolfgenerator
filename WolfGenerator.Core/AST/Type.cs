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
 *   14.02.2009 12:02 - GenericParameters must be not null.
 *                      if genericParameters argument in constructor is null
 *                      create empty array[Type].
 *   14.02.2009 13:16 - FIX: ToString, check if generic parameters is empty.
 *   14.02.2009 13:21 - Add Type constructor with `params` attribute argument list, to support hand initialize.
 *   28.02.2009 10:11 - Add inheritance from Statement class.
 *   24.04.2012 17:20 - [+] Add [emptyListGenericParameters] for empty generic list.
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Type = ident { "." ident } [ "&lt;" Type { "," Type } "&gt;" ].
	/// </summary>
	public class Type : Statement
	{
		private static readonly IList<Type> emptyListGenericParameters = new List<Type>( 0 );

		private readonly string typeName;
		private readonly IList<Type> genericParameters;

		public Type( StatementPosition position, string typeName, params Type[] genericParameters )
			: this( position, typeName, (IList<Type>)genericParameters ) {}

		public Type( StatementPosition position, string typeName, IList<Type> genericParameters ) : base( position )
		{
			this.typeName = typeName;
			this.genericParameters = genericParameters ?? emptyListGenericParameters;
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

			if (this.genericParameters.Count > 0)
			{
				var generic = new string[this.genericParameters.Count];

				for (var i = 0; i < this.genericParameters.Count; i++)
					generic[i] = this.genericParameters[i].ToString();

				builder.AppendFormat( "<{0}>", string.Join( ",", generic ) );
			}

			return builder.ToString();
		}
	}
}