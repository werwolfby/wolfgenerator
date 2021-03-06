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
 *   11.02.2009 20:25 - Add MatchMethodGroups property.
 *   18.02.2009 00:19 - RuleMethodStatements always return not null collection (instead null return empty array).
 *   28.02.2009 10:00 - Add inheritance from Statement class.
 *   21.04.2012 23:54 - [+] Add [emptyRuleMethodStatementList] field.
 *
 *******************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: RuleClass = RuleClassStart {Using} {RuleMethod | Method} RuleClassEnd.
	///       RuleClassStart = lt;%ruleclass ident %&gt;.
	///       RuleClassEnd = &lt;%end%&gt;.
	/// </summary>
	public class RuleClassStatement : Statement
	{
		private static readonly IList<RuleClassMethodStatement> emptyRuleMethodStatementList =
			new List<RuleClassMethodStatement>( 0 ).AsReadOnly();

		private readonly string name;
		private readonly IList<UsingStatement> usingStatements;
		private readonly IList<RuleClassMethodStatement> ruleMethodStatements;
		private readonly IList<RuleMethodGroup> matchMethodGroups;

		public RuleClassStatement( StatementPosition position, string name, IList<UsingStatement> usingStatements,
		                           IList<RuleClassMethodStatement> ruleMethodStatements ) : base( position )
		{
			this.name = name;
			this.usingStatements = usingStatements;
			this.ruleMethodStatements = ruleMethodStatements ?? emptyRuleMethodStatementList;

			this.matchMethodGroups = (from statement in this.ruleMethodStatements.OfType<RuleMethodStatement>()
			                          group statement by
				                          new
				                          {
					                          statement.Name,
					                          Count = statement.Variables != null ? statement.Variables.Count : 0
				                          }
			                          into g select new RuleMethodGroup( g )).ToList().AsReadOnly();
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

		public IList<RuleMethodGroup> MatchMethodGroups
		{
			get { return this.matchMethodGroups; }
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.AppendLine( "<%ruleclass " + this.name + "%>" );

			if (this.usingStatements != null && this.usingStatements.Count > 0)
			{
				builder.AppendLine();
				foreach (var usingStatement in this.usingStatements)
					builder.AppendLine( usingStatement.ToString() );
			}

			if (this.ruleMethodStatements != null && this.ruleMethodStatements.Count > 0)
			{
				builder.AppendLine();
				foreach (var ruleMethodStatement in this.ruleMethodStatements)
					builder.AppendLine( ruleMethodStatement.ToString() );
			}

			builder.AppendLine();
			builder.Append( "<%end%>" );

			return builder.ToString();
		}
	}
}