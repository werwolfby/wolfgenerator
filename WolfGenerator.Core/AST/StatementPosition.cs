/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 28.02.2009 09:06
 *
 * File: StatementPosition.cs
 * Remarks:
 * 
 * History:
 *   28.02.2009 09:06 - Create Wireframe
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	public struct StatementPosition
	{
		public StatementPosition( int startLine, int endLine, int startColumn, int endColumn, int startPos, int endPos ) : this()
		{
			this.StartLine = startLine;
			this.EndLine = endLine;
			this.StartColumn = startColumn;
			this.EndColumn = endColumn;
			this.StartPos = startPos;
			this.EndPos = endPos;
		}

		public int StartLine { get; private set; }

		public int EndLine { get; private set; }

		public int StartColumn { get; private set; }

		public int EndColumn { get; private set; }

		public int StartPos { get; private set; }

		public int EndPos { get; private set; }
	}
}