/*****************************************************
 *
 * Created by: WerWolf
 * Created: 05.05.2007 17:51:01
 *
 * File: RuleNode.cs
 * Remarks:
 * 
 * History:
 *   05.05.2007 17:51 - Create wireframe
 *
 *****************************************************/

namespace WolfGenerator.Core
{
	public abstract class RuleNode
	{
		private string fileName;
		private int lineNumber;
		private int columnNumber;

		protected RuleNode( string fileName, int lineNumber, int columnNumber )
		{
			this.fileName = fileName;
			this.lineNumber = lineNumber;
			this.columnNumber = columnNumber;
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

		public int ColumnNumber
		{
			get
			{
				return this.columnNumber;
			}
		}

		public abstract void Workaround( CodeGenerator generator, string prefixName );

		public abstract void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName );

		public abstract string GenerateCallMethod( CodeGenerator generator, string prefixName );
	}
}