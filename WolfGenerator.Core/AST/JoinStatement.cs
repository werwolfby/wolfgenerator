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
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Join = "&lt;%join" string "%&lt;" { Value | Apply | Call } "&lt;end&gt;".
	/// </summary>
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
			writer.AppendLine("{");
			writer.Indent++;

			writer.AppendLine( "var list = new List<CodeWriter>();" );
			writer.AppendLine( "CodeWriter temp;" );
			writer.AppendLine();

			foreach (var statement in statements)
			{
				statement.GenerateJoin( writer, innerWriter );
				writer.AppendLine();
			}

			writer.AppendLine( "for (var listI = 0; listI < list.Count; listI++)" );
			writer.AppendLine( "{" );
			writer.Indent++;

			writer.AppendLine( "var codeWriter = list[listI];" );
			writer.AppendLine( "writer.Append( codeWriter );" );
			writer.AppendLine( "if (listI < list.Count - 1)" );
			writer.Indent++;
			writer.AppendLine( "writer.AppendText( \"" + this.String + "\" );" );
			writer.Indent--;

			writer.Indent--;
			writer.AppendLine( "}" );

			writer.Indent--;
			writer.AppendLine("}");
		}

		public override void GenerateJoin( Writer.CodeWriter writer, string innerWriter )
		{
			throw new System.NotSupportedException();
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