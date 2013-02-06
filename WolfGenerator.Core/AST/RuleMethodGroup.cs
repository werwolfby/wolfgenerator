/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 11.02.2009 20:16
 *
 * File: RuleMethodGroup.cs
 * Remarks:
 * 
 * History:
 *   11.02.2009 20:16 - Create Wireframe
 *   26.02.2009 22:13 - Rename property from `MatchStatements` to `RuleMethodStatements`
 *
 *******************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfGenerator.Core.AST
{
	public class RuleMethodGroup
	{
		public RuleMethodGroup( IEnumerable<RuleMethodStatement> ruleMethodStatements )
		{
			this.RuleMethodStatements = ruleMethodStatements.Where( rms => rms.MatchMethodStatement != null ).ToList().AsReadOnly();
			this.DefaultStatements = ruleMethodStatements.Where( rms => rms.MatchMethodStatement == null ).ToList().AsReadOnly();
		}

		public bool IsMatched 
		{ 
			get { return this.RuleMethodStatements.Count > 0; }
		}

		public IList<RuleMethodStatement> RuleMethodStatements { get; private set; }

		public IList<RuleMethodStatement> DefaultStatements { get; private set; }

		public override string ToString()
		{
			var firstRuleMethodStatement = this.DefaultStatements.Concat( this.RuleMethodStatements ).First();
			return string.Format( "Match Name={0}({1}), IsMatch={2}", firstRuleMethodStatement.Name,
			                      firstRuleMethodStatement.Variables != null ? firstRuleMethodStatement.Variables.Count : 0,
			                      this.IsMatched );
		}
	}
}