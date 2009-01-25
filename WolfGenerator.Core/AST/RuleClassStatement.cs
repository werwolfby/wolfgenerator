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
 *   25.01.2009 08:33 - Add EBNF of ruleclass.
 *   25.01.2009 08:45 - Add Name property.
 *   25.01.2009 08:48 - Override ToString method.
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: RuleClass = RuleClassStart {Using} {RuleMethod} RuleClassEnd.
	///       RuleClassStart = lt;%ruleclass ident %&gt;.
	///       RuleClassEnd = &lt;%end%&gt;.
	/// </summary>
	public class RuleClassStatement
	{
		private readonly string name;
		private readonly IList<UsingStatement> usingStatements;
		private readonly IList<RuleMethodStatement> ruleMethodStatements;

		public RuleClassStatement( string name, IList<UsingStatement> usingStatements, IList<RuleMethodStatement> ruleMethodStatements )
		{
			this.name = name;
			this.usingStatements = usingStatements;
			this.ruleMethodStatements = ruleMethodStatements;
		}

		public string Name
		{
			get { return this.name; }
		}

		public IList<UsingStatement> UsingStatements
		{
			get { return this.usingStatements; }
		}

		public IList<RuleMethodStatement> RuleMethodStatements
		{
			get { return this.ruleMethodStatements; }
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.AppendLine( "<%ruleclass " + name + "%>" );

			if (usingStatements != null)
			{
				foreach (var usingStatement in usingStatements)
				{
					builder.AppendLine( usingStatement.ToString() );
				}
			}

			if (ruleMethodStatements != null)
			{
				foreach (var ruleMethodStatement in ruleMethodStatements)
				{
					builder.AppendLine( ruleMethodStatement.ToString() );
				}
			}

			return builder.ToString();
		}
	}
}