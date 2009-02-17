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
 *   18.02.2009 00:36 - Throw exception when try append multi line text.
 *   18.02.2009 00:39 - Override ToString method.
 *
 *******************************************************/

using System.Text;
using WolfGenerator.Core.Writer.Exception;

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
			if (text.Contains( "\r" ) || text.Contains( "\n" )) throw new MultiLineException( text );
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

		public string ToString( string indentString )
		{
			var toStringBuilder = new StringBuilder();

			for (var i = 0; i < Indent; i++)
				toStringBuilder.Append( indentString );

			toStringBuilder.Append( GetText() );

			return toStringBuilder.ToString();
		}

		public override string ToString()
		{
			return ToString( "\t" );
		}
	}
}