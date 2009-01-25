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
			this.Lines = lines.AsReadOnly();

			var tmpLines = text.Replace( "\r\n", "\n" ).Split( '\n' );
			for (var i = 0; i < tmpLines.Length; i++)
			{
				if (tmpLines[i] == "") lines.Add( EmptyLine.Instance );
				else
				{
					var indent = 0;
					while (tmpLines[i][indent] == '\t')
						indent++;
					lines.Add( new Line( indent, tmpLines[i].Substring( indent ) ) );
				}
			}
			if (tmpLines[tmpLines.Length - 1] == "") cropLastLine = true;
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

		public override string ToString()
		{
			return text;
		}
	}
}