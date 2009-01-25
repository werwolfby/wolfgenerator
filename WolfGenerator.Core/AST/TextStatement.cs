/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:27
 *
 * File: TextStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 08:27 - Create Wireframe
 *   25.01.2009 10:51 - Override ToString method.
 *   25.01.2009 23:11 - Add split text on lines & support check for crop last line.
 *   25.01.2009 23:20 - Fix: if crop last line don't add this empty line to lines.
 *   25.01.2009 23:29 - Fix: counting indent.
 *   26.01.2009 00:37 - Implement Generate method.
 *   26.01.2009 00:48 - Some fixes int Generate method.
 *
 *******************************************************/

using System.Collections.Generic;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	public class TextStatement : RuleStatement
	{
		private readonly string text;
		private readonly List<Line> lines = new List<Line>();
		private readonly bool cropLastLine;

		public TextStatement( string text )
		{
			this.text = text;
			this.Lines = this.lines.AsReadOnly();

			var tmpLines = text.Replace( "\r\n", "\n" ).Split( '\n' );
			if (tmpLines[tmpLines.Length - 1] == "") this.cropLastLine = true;

			for (var i = 0; i < (this.cropLastLine ? tmpLines.Length - 1 : tmpLines.Length); i++)
			{
				if (tmpLines[i] == "") this.lines.Add( EmptyLine.Instance );
				else
				{
					var indent = 0;
					while (indent < tmpLines[i].Length && tmpLines[i][indent] == '\t')
						indent++;
					this.lines.Add( new Line( indent, tmpLines[i].Substring( indent ) ) );
				}
			}
		}

		public string Text
		{
			get { return this.text; }
		}

		public IList<Line> Lines { get; private set; }

		public bool CropLastLine
		{
			get { return this.cropLastLine; }
		}

		public override void Generate( Writer.CodeWriter writer, string innerWriter )
		{
			for (var i = 0; i < this.lines.Count; i++)
			{
				var line = this.lines[i];

				writer.Append( innerWriter );
				if (i == this.lines.Count - 1 && cropLastLine) writer.Append( ".Append( \"" );
				else writer.Append( ".AppendLine( \"" );
				writer.Append( line.GetText().Replace( "\"", "\\\"" ) );
				writer.AppendLine( "\" );" );
			}
		}

		public override string ToString()
		{
			return this.text;
		}
	}
}