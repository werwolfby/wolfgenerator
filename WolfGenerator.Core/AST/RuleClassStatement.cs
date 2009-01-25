/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:23
 *
 * File: RuleClassStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 08:23 - Create Wireframe
 *   25.01.2009 08:24 - Add using statements.
 *   25.01.2009 08:25 - Add rule method statements. Add constructor.
 *   25.01.2009 08:33 - Add EBNF of ruleclass
 *
 *******************************************************/

using System.Collections.Generic;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: RuleClass = RuleClassStart {Using} {RuleMethod} RuleClassEnd.
	///       RuleClassStart = <%ruleclass ident %>.
	///       RuleClassEnd = <%end%>.
	/// </summary>
	public class RuleClassStatement
	{
		private readonly IList<UsingStatement> usingStatements;
		private readonly IList<RuleMethodStatement> ruleMethodStatements;

		public RuleClassStatement( IList<UsingStatement> usingStatements, IList<RuleMethodStatement> ruleMethodStatements )
		{
			this.usingStatements = usingStatements;
			this.ruleMethodStatements = ruleMethodStatements;
		}

		public IList<UsingStatement> UsingStatements
		{
			get { return this.usingStatements; }
		}

		public IList<RuleMethodStatement> RuleMethodStatements
		{
			get { return this.ruleMethodStatements; }
		}
	}
}