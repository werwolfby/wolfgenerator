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
 *
 *******************************************************/

using System.Collections.Generic;
using System.Linq;

namespace WolfGenerator.Core.AST
{
	public class RuleMethodGroup
	{
		public RuleMethodGroup( IEnumerable<RuleMethodStatement> ruleMethodStatements )
		{
			this.MatchStatements = ruleMethodStatements.Where( rms => rms.MatchMethodStatement != null ).ToList().AsReadOnly();
			this.DefaultStatement = ruleMethodStatements.Where( rms => rms.MatchMethodStatement == null ).SingleOrDefault();
		}

		public bool IsMatched 
		{ 
			get { return MatchStatements.Count > 0; }
		}

		public IList<RuleMethodStatement> MatchStatements { get; private set; }

		public RuleMethodStatement DefaultStatement { get; private set; }
	}
}