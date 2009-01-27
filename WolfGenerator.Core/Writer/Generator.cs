/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 26.01.2009 00:17
 *
 * File: Generator.cs
 * Remarks:
 * 
 * History:
 *   26.01.2009 00:17 - Create Wireframe
 *
 *******************************************************/

using System;
using WolfGenerator.Core.AST;
using System.Linq;

namespace WolfGenerator.Core.Writer
{
	public static class Generator
	{
		public static CodeWriter Generate( RuleClassStatement ruleClassStatement )
		{
			var writer = new CodeWriter();

			if (ruleClassStatement.UsingStatements != null && ruleClassStatement.UsingStatements.Count > 0)
			{
				foreach (var usingStatement in ruleClassStatement.UsingStatements)
					writer.AppendLine( "using " + usingStatement.Namespace + ";" );
				writer.AppendLine();
			}

			writer.Append( "public partial class " );
			writer.AppendLine( ruleClassStatement.Name );
			writer.AppendLine( "{" );
			writer.Indent++;

			foreach (var ruleMethodStatement in ruleClassStatement.RuleMethodStatements)
			{
				ruleMethodStatement.Generate( writer );
			}

			writer.Indent--;
			writer.AppendLine( "}" );

			return writer;
		}
	}
}