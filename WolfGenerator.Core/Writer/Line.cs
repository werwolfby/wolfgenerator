/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 21:55
 *
 * File: Line.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 21:55 - Create Wireframe
 *   25.01.2009 22:14 - Add Clone method.
 *
 *******************************************************/

using System.Text;

namespace WolfGenerator.Core.Writer
{
	public class Line
	{
		private readonly int indent;
		private readonly StringBuilder builder = new StringBuilder();

		public Line( int indent ) : this( indent, null ) {}

		public Line( int indent, string startText )
		{
			this.indent = indent;
			if (startText != null)
			{
				Append( startText );
			}
		}

		public int Indent
		{
			get { return this.indent; }
		}

		public void Append( string text )
		{
			builder.Append( text );
		}

		public void Append( int spaces, string text )
		{
			for (var i = 0; i < spaces; i++) builder.Append( ' ' );
			Append( text );
		}

		public virtual string GetText()
		{
			return builder.ToString();
		}

		public virtual Line Clone( int newIndent )
		{
			return new Line( newIndent, builder.ToString() );
		}
	}
}