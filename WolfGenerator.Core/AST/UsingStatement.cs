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
 *   18.02.2009 00:13 - Override Equals method.
 *   28.02.2009 10:02 - Add inheritance from Statement class.
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Using = &lt;%using ident { .  ident } %&gt;.
	/// </summary>
	public class UsingStatement : Statement
	{
		private readonly string @namespace;

		public UsingStatement( StatementPosition position, string @namespace ) : base( position )
		{
			this.@namespace = @namespace;
		}

		public string Namespace
		{
			get { return this.@namespace; }
		}

		public override string ToString()
		{
			return "<%using " + this.@namespace + "%>";
		}

		public bool Equals( UsingStatement obj )
		{
			if (ReferenceEquals( null, obj )) return false;
			if (ReferenceEquals( this, obj )) return true;
			return Equals( obj.@namespace, this.@namespace );
		}

		public override bool Equals( object obj )
		{
			if (ReferenceEquals( null, obj )) return false;
			if (ReferenceEquals( this, obj )) return true;
			if (obj.GetType() != typeof(UsingStatement)) return false;
			return this.Equals( (UsingStatement)obj );
		}

		public override int GetHashCode()
		{
			return (this.@namespace != null ? this.@namespace.GetHashCode() : 0);
		}
	}
}