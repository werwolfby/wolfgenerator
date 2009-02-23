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
 *   27.01.2009 00:03 - BugFix: AppendLines - check if appending text contains any line.
 *   27.01.2009 00:45 - BugFix: AppendLines - ufff... tired...
 *   23.02.2009 00:15 - Rewrite all Append method with help of InnerAppend method.
 *   23.02.2009 00:37 - BugFix: ToString - if line is empty, don't add indent string just break line.
 *   23.02.2009 23:16 - Add AppendType property.
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;
using WolfGenerator.Core.AST;
using WolfGenerator.Core.Writer.Exception;

namespace WolfGenerator.Core.Writer
{
	public class CodeWriter
	{
		#region Fields
        private readonly List<Line> lines = new List<Line>();
		private AppendType appendType = AppendType.EmptyLastLine;
		private string appendString = "";
		private Line lastLine;
		#endregion

		#region Constructors
		public CodeWriter()
		{
			this.Lines = this.lines.AsReadOnly();
			this.IndentString = "\t";
		}
		#endregion

		#region Properties
		public int Indent { get; set; }

		public string IndentString { get; set; }

		public IList<Line> Lines { get; private set; }
		#endregion

		public AppendType AppendType
		{
			get { return this.appendType; }
			set
			{
				this.appendType = value;
				switch (appendType)
				{
					case AppendType.EmptyLastLine:
						appendString = "";
						break;
					case AppendType.CloneLastLine:
						appendString = this.lastLine == null ? "" : this.lastLine.GetText();
						break;
					case AppendType.SpaceLastLine:
						appendString = this.lastLine == null ? "" : new string( ' ', this.lastLine.GetText().Length );
						break;
					default:
						throw new UnexpectedAppendTypeWriterException( appendType );
				}
			}
		}

		public void AppendText( string text )
		{
			bool cropLastLine;
            var textLines = TextStatement.ExtractLines( text, out cropLastLine );

			this.AppendLines( textLines, cropLastLine );
		}

		public void AppendLine()
		{
			InnerAppend( "", true );
		}

		public void AppendLine( string text )
		{
			InnerAppend( text, true );
		}

		public void Append( string text )
		{
			InnerAppend( text, false );
		}

		public void Append( CodeWriter codeWriter )
		{
			AppendLines( codeWriter.Lines, false );
		}

		private void AppendLines( IList<Line> textLines, bool cropLastLine ) 
		{
			var oldInden = this.Indent;

			for (var i = 0; i < textLines.Count; i++)
			{
				this.Indent = oldInden + textLines[i].Indent;
				InnerAppend( textLines[i].GetText(), cropLastLine || i < textLines.Count - 1 );
			}

			this.Indent = oldInden;
		}

		private void InnerAppend( string lineSegment, bool newLine )
		{
			if (lastLine == null)
			{
				lastLine = new Line( Indent, appendString );
				this.lines.Add( lastLine );
			}
			lastLine.Append( lineSegment );

			if (newLine) lastLine = null;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			for (var i = 0; i < this.lines.Count; i++)
			{
				var line = this.lines[i];

				if (line == EmptyLine.Instance || line.GetText() == "")
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