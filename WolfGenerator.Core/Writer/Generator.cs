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
 *   28.01.2009 16:35 - Check if ruleClassStatement.RuleMethodStatements contains any element.
 *   04.02.2009 01:01 - Add default namespaces.
 *
 *******************************************************/

using WolfGenerator.Core.AST;
using System.Linq;

namespace WolfGenerator.Core.Writer
{
	public static class Generator
	{
		private static readonly string[] defaultNamespaces = new[]
		                                                     {
		                                                     	"System",
		                                                     	"System.Collections.Generic",
		                                                     	"System.Reflection",
		                                                     	"WolfGenerator.Core.Writer"
		                                                     };

		public static CodeWriter Generate( RuleClassStatement ruleClassStatement )
		{
			var writer = new CodeWriter();

			foreach (var defaultNamespace in defaultNamespaces)
			{
				var checkNamespace = defaultNamespace;
				if (ruleClassStatement.UsingStatements != null &&
				    ruleClassStatement.UsingStatements.SingleOrDefault( statement => statement.Namespace == checkNamespace ) != null)
					continue;
				writer.Append( "using " );
				writer.Append( defaultNamespace );
				writer.AppendLine( ";" );
			}

			if (ruleClassStatement.UsingStatements != null && ruleClassStatement.UsingStatements.Count > 0)
			{
				foreach (var usingStatement in ruleClassStatement.UsingStatements)
					writer.AppendLine( "using " + usingStatement.Namespace + ";" );
			}
			writer.AppendLine();

			writer.Append( "public partial class " );
			writer.AppendLine( ruleClassStatement.Name );
			writer.AppendLine( "{" );
			writer.Indent++;

			if (ruleClassStatement.RuleMethodStatements != null && ruleClassStatement.RuleMethodStatements.Count > 0)
			{
				foreach (var ruleMethodStatement in ruleClassStatement.RuleMethodStatements)
				{
					ruleMethodStatement.Generate( writer );
				}
			}

			writer.Indent--;
			writer.AppendLine( "}" );

			return writer;
		}
	}
}