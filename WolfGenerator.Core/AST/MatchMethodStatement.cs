/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 04.02.2009 01:39
 *
 * File: MatchMethodStatement.cs
 * Remarks:
 * 
 * History:
 *   04.02.2009 01:39 - Create Wireframe
 *   04.02.2009 01:45 - Implement Generate method.
 *   04.02.2009 02:12 - Fix: Generate method.
 *   10.02.2009 20:30 - Add support fileName of Generated method
 *   11.02.2009 21:54 - New MatchMethodAttribute code generating.
 *
 *******************************************************/

using System;
using System.Linq;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	public class MatchMethodStatement : RuleClassMethodStatement
	{
		private readonly string name;
		private readonly string code;

		public MatchMethodStatement( string name, string code )
		{
			this.name = name;
			this.code = code;
		}

		public string Name
		{
			get { return this.name; }
		}

		public string Code
		{
			get { return this.code; }
		}

		internal RuleMethodStatement RuleMethod { get; set; }

		public override void Generate( CodeWriter writer, string fileName )
		{
			writer.Append( "[MatchMethod( \"" );
			writer.Append( RuleMethod.Name );
			writer.Append( "\", \"" );
			writer.Append( this.name );
			writer.Append( "\", \"" );
			writer.Append( fileName );
			writer.AppendLine( "\" )]" );
			writer.Append( "private bool Match_" );
			writer.Append( RuleMethod.Name );
			writer.Append( "_" );
			writer.Append( name );
			if (RuleMethod.Variables == null || RuleMethod.Variables.Count == 0)
			{
				writer.AppendLine( "()" );
			}
			else
			{
				writer.Append( "( " );
				writer.Append( string.Join( ", ", Array.ConvertAll( RuleMethod.Variables.ToArray(), input => input.ToString() ) ) );
				writer.AppendLine( " )" );
			}
			writer.AppendLine( "{" );
			writer.Indent++;
			writer.AppendText( Code );
			writer.AppendLine();
			writer.Indent--;
			writer.AppendLine( "}" );
		}
	}
}