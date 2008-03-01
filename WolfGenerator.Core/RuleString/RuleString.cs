/*****************************************************
 *
 * Created by: WerWolf
 * Created: 11.05.2007 10:46:09
 *
 * File: RuleString.cs
 * Remarks:
 * 
 * History:
 *   18.06.2007 - Add 'fileName', 'lineNumber', 'columnNumber'
 *
 *****************************************************/

namespace WolfGenerator.Core
{
	public abstract class RuleString
	{
		private string text;
		private string fileName;
		private int lineNumber;
		private int columNumber;

		public RuleString( string text, string fileName, int lineNumber, int columNumber )
		{
			this.text = text;
			this.columNumber = columNumber;
			this.lineNumber = lineNumber;
			this.fileName = fileName;
		}

		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public int ColumNumber
		{
			get
			{
				return this.columNumber;
			}
		}
	}
}
