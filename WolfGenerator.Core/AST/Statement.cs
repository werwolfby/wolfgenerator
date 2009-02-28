/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 28.02.2009 09:18
 *
 * File: Statement.cs
 * Remarks:
 * 
 * History:
 *   28.02.2009 09:18 - Create Wireframe
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	public class Statement
	{
		public Statement( StatementPosition statementPosition )
		{
			this.StatementPosition = statementPosition;
		}

		public StatementPosition StatementPosition { get; private set; }
	}
}