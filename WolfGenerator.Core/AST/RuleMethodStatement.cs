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
 *   25.01.2009 08:52 - Add EBNF.
 *   25.01.2009 10:14 - Add Name, Variable properties.
 *   25.01.2009 10:39 - Override ToString() method.
 *   25.01.2009 10:57 - Exted ToString method (check if contains any statement).
 *   27.01.2009 01:54 - Add Generate method.
 *   02.02.2009 01:37 - BugFix: If Variables is null or haven't any variable generate parameterless method.
 *   04.02.2009 01:53 - Add matchMethodStatement : MatchMethodStatement field.
 *   04.02.2009 02:11 - Fix: Generate method. Use MatchMethod generation.
 *   10.02.2009 20:28 - Add support fileName of Generate method
 *   11.02.2009 20:14 - Update EBNF.
 *   11.02.2009 20:15 - Add MethodHeader property to simple `group by` using.
 *   11.02.2009 20:33 - Add support Generate default rule method.
 *   11.02.2009 20:41 - Fix: Generate method.
 *   11.02.2009 21:59 - Add RuleMethod attribute generate supporting.
 *   11.02.2009 22:12 - Add to generate method boolean parameter to generate RuleMethod Attribute.
 *   15.02.2009 16:16 - Now Variables & Statements properti instead null return empty collection.
 *   26.02.2009 23:14 - Remove Generate methods.
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: RuleMethod      = [MatchMethod] RuleMethodStart {ANY | Value | Join | Code | Call} RuleMethodEnd.
	///       RuleMethodStart = &lt;%rule ident ( [ Var { , Var } ] ) %&gt;.
	///       RuleMethodEnd   = &lt;%end%&gt;.
	/// </summary>
	public class RuleMethodStatement : RuleClassMethodStatement
	{
		private readonly MatchMethodStatement matchMethodStatement;
		private readonly string name;
		private readonly IList<Variable> variables;
		private readonly IList<RuleStatement> statements;

		public RuleMethodStatement( MatchMethodStatement matchMethodStatement, string name, IList<Variable> variables, IList<RuleStatement> statements )
		{
			this.matchMethodStatement = matchMethodStatement;
			this.name = name;
			this.variables = variables ?? new Variable[0];
			this.statements = statements ?? new RuleStatement[0];

			if (this.matchMethodStatement != null) this.matchMethodStatement.RuleMethod = this;
		}

		public MatchMethodStatement MatchMethodStatement
		{
			get { return this.matchMethodStatement; }
		}

		public string Name
		{
			get { return this.name; }
		}

		public IList<Variable> Variables
		{
			get { return this.variables; }
		}

		public IList<RuleStatement> Statements
		{
			get { return this.statements; }
		}

		public string MethodHeader
		{
			get
			{
				var builder = new StringBuilder();

				builder.Append( name );

				if (variables != null)
				{
					builder.Append( "( " );

					for (int i = 0; i < variables.Count; i++)
					{
						var variable = variables[i];

						builder.Append( variable );
						if (i < variables.Count - 1) builder.Append( ", " );
					}

					builder.Append( " )" );
				}
				else builder.Append( "()" );

				return builder.ToString();
			}
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append( "<%rule " );

			builder.Append( MethodHeader );

			builder.AppendLine( "%>" );

			if (statements != null && statements.Count > 0)
			{
				foreach (var statement in statements)
					builder.Append( statement );
				builder.AppendLine();
			}

			builder.Append( "<%end%>" );

			return builder.ToString();
		}
	}
}