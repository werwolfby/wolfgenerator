/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:25
 *
 * File: RuleMethodStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 08:25 - Create Wireframe
 *   25.01.2009 08:27 - Add statements. Add Constructor.
 *
 *******************************************************/

using System.Collections.Generic;

namespace WolfGenerator.Core.AST
{
	public class RuleMethodStatement
	{
		private readonly IList<RuleStatement> statements;

		public RuleMethodStatement( IList<RuleStatement> statements )
		{
			this.statements = statements;
		}

		public IList<RuleStatement> Statements
		{
			get { return this.statements; }
		}
	}
}