/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 21:55
 *
 * File: CodeWriter.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 21:55 - Create Wireframe
 *   25.01.2009 22:18 - Add EmptyLine support.
 *   26.01.2009 00:40 - When AppendLine() if lastLine exist just complete it line,
 *                      else add EmptyLine.
 *   26.01.2009 10:39 - Add method AppendText.
 *   26.01.2009 11:02 - Fix: AppendText.
 *   26.01.2009 11:07 - Add method AppendLines. Fix: Append( CodeWriter ) use AppendLines.
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;
using WolfGenerator.Core.AST;

namespace WolfGenerator.Core.Writer
{
	public class CodeWriter
	{
		private readonly List<Line> lines = new List<Line>();
		private Line lastLine;

		public CodeWriter()
		{
			this.Lines = this.lines.AsReadOnly();
			this.IndentString = "\t";
		}

		public int Indent { get; set; }

		public string IndentString { get; set; }

		public IList<Line> Lines { get; private set; }

		public void AppendText( string text )
		{
			bool cropLastLine;
            var textLines = TextStatement.ExtractLines( text, out cropLastLine );

			this.AppendLines( textLines, cropLastLine );
		}

		private void AppendLines( IList<Line> textLines, bool cropLastLine ) 
		{
			var startIndex = 0;

			if (this.lastLine != null)
			{
				this.lastLine.Append( textLines[0].GetText() );
				startIndex = 1;
			}

			var oldInden = this.Indent;

			for (var i = startIndex; i < textLines.Count; i++)
			{
				this.Indent = oldInden + textLines[i].Indent;
				if (textLines[i] == EmptyLine.Instance)
				{
					if (i == textLines.Count - 1 && cropLastLine) continue;
					this.AppendLine();
				}
				else
				{
					if (i == textLines.Count - 1 && !cropLastLine) this.Append( textLines[i].GetText() );
					else this.AppendLine( textLines[i].GetText() );
				}
			}

			if (cropLastLine) this.lastLine = null;

			this.Indent = oldInden;
		}

		public void AppendLine()
		{
			if (this.lastLine != null) this.lastLine = null;
			else this.lines.Add( EmptyLine.Instance );
		}

		public void AppendLine( string text )
		{
			if (this.lastLine != null)
			{
				this.lastLine.Append( text );
				this.lastLine = null;
			}
			else this.lines.Add( new Line( this.Indent, text ) );
		}

		public void Append( string text )
		{
			if (this.lastLine == null)
			{
				this.lastLine = new Line( this.Indent );
				this.lines.Add( this.lastLine );
			}
			this.lastLine.Append( text );
		}

		public void Append( CodeWriter codeWriter )
		{
			AppendLines( codeWriter.Lines, false );
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			for (var i = 0; i < this.lines.Count; i++)
			{
				var line = this.lines[i];

				if (line == EmptyLine.Instance)
				{
					builder.AppendLine();
				}
				else
				{
					for (var j = 0; j < line.Indent; j++) builder.Append( IndentString );

					var text = line.GetText();

					if (i < this.lines.Count - 1) builder.AppendLine( text );
					else builder.Append( text );
				}
			}

			return builder.ToString();
		}
	}
}