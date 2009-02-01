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
 *   25.01.2009 10:38 - Extend ToString() method.
 *   25.01.2009 10:57 - Exted ToString method (check if contains any using 
 *                      statements and rule method statements).
 *   27.01.2009 02:00 - change collection type from RuleMethodStatements to RuleClassMethodStatement
 *   25.01.2009 08:33 - Update EBNF.
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: RuleClass = RuleClassStart {Using} {RuleMethod | Method} RuleClassEnd.
	///       RuleClassStart = lt;%ruleclass ident %&gt;.
	///       RuleClassEnd = &lt;%end%&gt;.
	/// </summary>
	public class RuleClassStatement
	{
		private readonly string name;
		private readonly IList<UsingStatement> usingStatements;
		private readonly IList<RuleClassMethodStatement> ruleMethodStatements;

		public RuleClassStatement( string name, IList<UsingStatement> usingStatements, IList<RuleClassMethodStatement> ruleMethodStatements )
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

		public IList<RuleClassMethodStatement> RuleMethodStatements
		{
			get { return this.ruleMethodStatements; }
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.AppendLine( "<%ruleclass " + name + "%>" );
            
			if (usingStatements != null && usingStatements.Count > 0)
			{
				builder.AppendLine();
				foreach (var usingStatement in usingStatements)
				{
					builder.AppendLine( usingStatement.ToString() );
				}
			}

			if (ruleMethodStatements != null && ruleMethodStatements.Count > 0)
			{
				builder.AppendLine();
				foreach (var ruleMethodStatement in ruleMethodStatements)
				{
					builder.AppendLine( ruleMethodStatement.ToString() );
				}
			}

			builder.AppendLine();
			builder.Append( "<%end%>" );

			return builder.ToString();
		}
	}
}