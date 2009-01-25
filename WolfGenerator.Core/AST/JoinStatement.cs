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
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core.AST
{
	public class JoinStatement : RuleStatement
	{
		private readonly string @string;
		private readonly IList<RuleStatement> statements;

		public JoinStatement( string @string, IList<RuleStatement> statements )
		{
			this.@string = @string;
			this.statements = statements;
		}

		public string String
		{
			get { return this.@string; }
		}

		public IList<RuleStatement> Statements
		{
			get { return this.statements; }
		}

		public override void Generate( Writer.CodeWriter writer, string innerWriter )
		{
			//throw new System.NotImplementedException();
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append( "<%join \"" );
			builder.Append( this.@string );
			builder.Append( "\"%>" );

			if (statements != null && statements.Count > 0)
			{
				builder.AppendLine();

				foreach (var statement in statements)
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