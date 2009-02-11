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
 *   04.02.2009 01:13 - Add default namespace `WolfGenerator.Core`.
 *   04.02.2009 01:15 - Remove default namespace `System.Reflection`.
 *   10.02.2009 20:31 - Add support fileName of Generated method.
 *   11.02.2009 20:38 - Add new generate code from MatchMethodGroup.
 *   11.02.2009 22:12 - GenerateSingleMethod - don't generate RuleMethod attribute.
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
		                                                     	"WolfGenerator.Core.Writer",
		                                                     	"WolfGenerator.Core",
		                                                     };

		public static CodeWriter Generate( RuleClassStatement ruleClassStatement, string fileName )
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
			writer.Append( ruleClassStatement.Name );
			writer.Append( " : " );
			writer.AppendLine( typeof(GeneratorBase).Name );
			writer.AppendLine( "{" );
			writer.Indent++;

			//if (ruleClassStatement.RuleMethodStatements != null && ruleClassStatement.RuleMethodStatements.Count > 0)
			//{
			//    foreach (var ruleMethodStatement in ruleClassStatement.RuleMethodStatements)
			//    {
			//        ruleMethodStatement.Generate( writer, fileName );
			//    }
			//}

			if (ruleClassStatement.MatchMethodGroups != null && ruleClassStatement.MatchMethodGroups.Count > 0)
			{
				foreach (var matchMethodGroup in ruleClassStatement.MatchMethodGroups)
				{
					if (matchMethodGroup.IsMatched) GenerateMatchMethod( matchMethodGroup, fileName, writer );
					else GenerateSingleMethod( matchMethodGroup, fileName, writer );
				}
			}

			writer.Indent--;
			writer.AppendLine( "}" );

			return writer;
		}

		private static void GenerateMatchMethod( RuleMethodGroup methodGroup, string fileName, CodeWriter writer )
		{
			foreach (var matchStatement in methodGroup.MatchStatements)
			{
				matchStatement.MatchMethodStatement.Generate( writer, fileName );
				matchStatement.Generate( writer, fileName );
			}

			if (methodGroup.DefaultStatement != null) methodGroup.DefaultStatement.Generate( writer, fileName, true, true );
		}

		private static void GenerateSingleMethod( RuleMethodGroup methodGroup, string fileName, CodeWriter writer )
		{
			methodGroup.DefaultStatement.Generate( writer, fileName, false, false );
		}
	}
}