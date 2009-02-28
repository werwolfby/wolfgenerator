/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 11:30
 *
 * File: JoinStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 11:30 - Create Wireframe
 *   25.01.2009 11:33 - Add String property.
 *   25.01.2009 11:51 - Add Statements property & extend ToString method.
 *   26.01.2009 00:38 - Add Generate method without implementation.
 *   30.01.2009 20:15 - Add EBNF Comment.
 *   15.02.2009 13:53 - Statements property now newer return null (insted it return empty collection).
 *   15.02.2009 13:56 - Add check for type of inner statements (and throw exception in such way).
 *   21.02.2009 18:17 - Forget add check for CallStatement.
 *   23.02.2009 23:56 - Update EBNF & Add AppendType implementation.
 *   26.02.2009 23:11 - Remove Generate & GenerateJoin methods.
 *   28.02.2009 10:28 - Add support inheritance from Statement class.
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;
using WolfGenerator.Core.AST.Exception;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Join = "&lt;%join" string ["with" ("Space" | "Clone")] "%&lt;" { Value | Apply | Call } "&lt;end&gt;".
	/// </summary>
	public class JoinStatement : RuleStatement
	{
		private readonly string @string;
		private readonly AppendType appendType;
		private readonly IList<RuleStatement> statements;

		public JoinStatement( StatementPosition position, string @string, AppendType appendType,
		                      IList<RuleStatement> statements ) : base( position )
		{
			this.@string = @string;
			this.appendType = appendType;
			this.statements = statements ?? new RuleStatement[0];

			foreach (var statement in this.statements)
			{
				if (statement is ValueStatement) continue;
				if (statement is ApplyStatement) continue;
				if (statement is CallStatement) continue;
				throw new JoinBuildException( "Can't contain statement of type: " + statement.GetType() );
			}
		}

		public string String
		{
			get { return this.@string; }
		}

		public AppendType AppendType
		{
			get { return this.appendType; }
		}

		public IList<RuleStatement> Statements
		{
			get { return this.statements; }
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append( "<%join \"" );
			builder.Append( this.@string );
			builder.Append( "\"%>" );

			if (this.statements != null && this.statements.Count > 0)
			{
				builder.AppendLine();

				foreach (var statement in this.statements)
				{
					builder.Append( '\t' );
					builder.AppendLine( statement.ToString() );
				}
			}

			builder.Append( "<%end%>" );

			return builder.ToString();
		}
	}
}