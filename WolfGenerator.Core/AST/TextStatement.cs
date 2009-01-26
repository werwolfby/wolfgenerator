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
 *   26.01.2009 00:48 - Some fixes in Generate method.
 *   26.01.2009 10:11 - Some changes in Generate method.
 *   26.01.2009 10:30 - Extract method ExtractLines.
 *   27.01.2009 00:07 - ExtractLines: BugFix: now support empty line.
 *
 *******************************************************/

using System.Collections.Generic;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	public class TextStatement : RuleStatement
	{
		private readonly string text;
		private readonly List<Line> lines;
		private readonly bool cropLastLine;

		public TextStatement( string text )
		{
			this.text = text;
			this.lines = ExtractLines( text, out this.cropLastLine );
			this.Lines = this.lines.AsReadOnly();
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
			if (this.lines.Count == 0) return;

			var oldIndent = this.lines[0].Indent;
			writer.Append( innerWriter );
			writer.Append( ".Indent = " );
			writer.Append( oldIndent.ToString() );
			writer.AppendLine( ";" );

			for (var i = 0; i < this.lines.Count; i++)
			{
				var line = this.lines[i];

				if (oldIndent != line.Indent)
				{
					writer.Append( innerWriter );
					writer.Append( ".Indent = " );
					writer.Append( line.Indent.ToString() );
					writer.AppendLine( ";" );

					oldIndent = line.Indent;
				}

				var generatedText = line.GetText().Replace( "\"", "\\\"" );

				if (i == this.lines.Count - 1 && !cropLastLine)
				{
					if (generatedText == "") continue;
					writer.Append( innerWriter );
					writer.Append( ".Append" );
				}
				else
				{
					writer.Append( innerWriter );
					writer.Append( ".AppendLine" );
				}
				if (generatedText == "")
				{
					writer.AppendLine( "();" );
				}
				else
				{
					writer.Append( "( \"" );
					writer.Append( generatedText );
					writer.AppendLine( "\" );" );
				}
			}
		}

		public override void GenerateJoin( Writer.CodeWriter writer, string innerWriter )
		{
			throw new System.NotSupportedException();
		}

		public override string ToString()
		{
			return this.text;
		}

		public static List<Line> ExtractLines( string text, out bool cropLastLine )
		{
			cropLastLine = false;

			var lines = new List<Line>();

			var tmpLines = text.Replace( "\r\n", "\n" ).Split( '\n' );
			if (tmpLines.Length > 1 && tmpLines[tmpLines.Length - 1] == "") cropLastLine = true;

			for (var i = 0; i < (cropLastLine ? tmpLines.Length - 1 : tmpLines.Length); i++)
			{
				if (tmpLines[i] == "") lines.Add( EmptyLine.Instance );
				else
				{
					var indent = 0;
					while (indent < tmpLines[i].Length && tmpLines[i][indent] == '\t')
						indent++;
					lines.Add( new Line( indent, tmpLines[i].Substring( indent ) ) );
				}
			}

			return lines;
		}
	}
}