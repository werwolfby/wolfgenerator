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
 *
 *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: RuleMethod      = RuleMethodStart {ANY | Value } RuleMethodEnd.
	///       RuleMethodStart = <%rule ident ( [ Var { , Var } ] ) %>.
	///       RuleMethodEnd   = <%end%>.
	/// </summary>
	public class RuleMethodStatement : RuleClassMethodStatement
	{
		private readonly string name;
		private readonly IList<Variable> variables;
		private readonly IList<RuleStatement> statements;

		public RuleMethodStatement( string name, IList<Variable> variables, IList<RuleStatement> statements )
		{
			this.name = name;
			this.variables = variables;
			this.statements = statements;
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

		public override void Generate( CodeWriter writer )
		{
			writer.AppendLine( "public CodeWriter " + this.Name +
			                   "( " +
			                   string.Join( ", ", Array.ConvertAll( this.Variables.ToArray(), input => input.ToString() ) ) +
			                   " )" );
			writer.AppendLine( "{" );
			writer.Indent++;

			writer.AppendLine( "var writer = new CodeWriter();" );
			writer.AppendLine();

			foreach (var statement in Statements)
			{
				statement.Generate( writer, "writer" );
			}

			writer.AppendLine();
			writer.AppendLine( "return writer;" );

			writer.Indent--;
			writer.AppendLine( "}" );
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append( "<%rule " );
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