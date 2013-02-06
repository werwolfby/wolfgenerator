using System.Linq;
using WolfGenerator.Core.AST;
using System;
using System.Collections.Generic;
using WolfGenerator.Core.Writer;
using WolfGenerator.Core;

namespace WolfGenerator.Core.CodeGenerator
{
	public partial class Generator : GeneratorBase
	{
		[MatchMethod( "GenerateInvoke", "IsEmpty", "Generator.rule" )]
		private bool Match_GenerateInvoke_IsEmpty( CallStatement call )
		{
			return IsNullOrWhiteSpace( call.Parameters );
		}

		[MatchMethod( "GenerateInvoke", "IsNotEmpty", "Generator.rule" )]
		private bool Match_GenerateInvoke_IsNotEmpty( CallStatement call )
		{
			return !IsNullOrWhiteSpace( call.Parameters );
		}

		[RuleMethod( "GenerateInvoke", "IsEmpty", "Generator.rule" )]
		public CodeWriter GenerateInvoke_IsEmpty( CallStatement call )
		{
			var writer = new CodeWriter();

			writer.Indent = 0;
			writer.Append( "this.Invoke( \"" );
			writer.AppendText( call.Name );
			writer.Indent = 0;
			writer.Append( "\" )" );

			return writer;
		}

		[RuleMethod( "GenerateInvoke", "IsNotEmpty", "Generator.rule" )]
		public CodeWriter GenerateInvoke_IsNotEmpty( CallStatement call )
		{
			var writer = new CodeWriter();

			writer.Indent = 0;
			writer.Append( "this.Invoke( \"" );
			writer.AppendText( call.Name );
			writer.Indent = 0;
			writer.Append( "\", " );
			writer.AppendText( call.Parameters );
			writer.Indent = 0;
			writer.Append( " )" );

			return writer;
		}

		public CodeWriter Generate( string @namespace, RuleClassStatement ruleClassStatement, string fileName )
		{
			var writer = new CodeWriter();

			{
				var list = new List<CodeWriter>();
				CodeWriter temp;

				foreach (var item in ruleClassStatement.UsingStatements.Select( u => u.Namespace ).Concat( defaultNamespaces ).Distinct())
				{
					temp = this.Invoke( "Using", item );
					list.Add( temp );
				}

				writer.AppendType = AppendType.EmptyLastLine;
				for (var listI = 0; listI < list.Count; listI++)
				{
					var codeWriter = list[listI];
					writer.Append( codeWriter );
					if (listI < list.Count - 1)
						writer.AppendText( "\r\n" );
				}
				writer.AppendType = AppendType.EmptyLastLine;
			}
			writer.Indent = 0;
			writer.AppendLine();
			writer.AppendLine();
			if (@namespace != null)
			{
			writer.Indent = 0;
			writer.Append( "namespace " );
			writer.AppendText( @namespace );
			writer.Indent = 0;
			writer.AppendLine();
			writer.AppendLine( "{" );
			writer.Indent = 1;
			writer.Append( this.Invoke( "Class", ruleClassStatement, fileName ) );
			writer.Indent = 0;
			writer.AppendLine();
			writer.AppendLine( "}" );
			}
			else
			{
			writer.Append( this.Invoke( "Class", ruleClassStatement, fileName ) );
			writer.Indent = 0;
			writer.AppendLine();
			}

			return writer;
		}

		public CodeWriter Class( RuleClassStatement ruleClassStatement, string fileName )
		{
			var writer = new CodeWriter();

			writer.Indent = 0;
			writer.Append( "public partial class " );
			writer.AppendText( ruleClassStatement.Name );
			writer.Indent = 0;
			writer.AppendLine( " : GeneratorBase" );
			writer.AppendLine( "{" );
			writer.Indent = 1;
			{
				var list = new List<CodeWriter>();
				CodeWriter temp;

				foreach (var item in ruleClassStatement.RuleMethodStatements.OfType<MethodStatement>())
				{
					temp = this.Invoke( "Method", item, fileName );
					list.Add( temp );
				}
				foreach (var item in ruleClassStatement.MatchMethodGroups.Where( mmg => mmg.IsMatched ).SelectMany( mmg => mmg.RuleMethodStatements ).Select( ms => ms.MatchMethodStatement ))
				{
					temp = this.Invoke( "Match", item, fileName );
					list.Add( temp );
				}
				foreach (var item in ruleClassStatement.MatchMethodGroups.Where( mmg => mmg.IsMatched ).SelectMany( mmg => mmg.RuleMethodStatements ))
				{
					temp = this.Invoke( "Rule", item, fileName, false, true );
					list.Add( temp );
				}
				foreach (var item in ruleClassStatement.MatchMethodGroups.Where( mmg => mmg.IsMatched ).SelectMany( mmg => mmg.DefaultStatements ))
				{
					temp = this.Invoke( "Rule", item, fileName, true,  true );
					list.Add( temp );
				}
				foreach (var item in ruleClassStatement.MatchMethodGroups.Where( mmg => !mmg.IsMatched ).SelectMany( mmg => mmg.DefaultStatements ))
				{
					temp = this.Invoke( "Rule", item, fileName, false, false );
					list.Add( temp );
				}

				writer.AppendType = AppendType.EmptyLastLine;
				for (var listI = 0; listI < list.Count; listI++)
				{
					var codeWriter = list[listI];
					writer.Append( codeWriter );
					if (listI < list.Count - 1)
						writer.AppendText( "\r\n\r\n" );
				}
				writer.AppendType = AppendType.EmptyLastLine;
			}
			writer.Indent = 0;
			writer.AppendLine();
			writer.Append( "}" );

			return writer;
		}

		public CodeWriter Match( MatchMethodStatement matchMethodStatement, string fileName )
		{
			var writer = new CodeWriter();

			writer.Indent = 0;
			writer.Append( "[MatchMethod( \"" );
			writer.AppendText( matchMethodStatement.RuleMethod.Name );
			writer.Indent = 0;
			writer.Append( "\", \"" );
			writer.AppendText( matchMethodStatement.Name );
			writer.Indent = 0;
			writer.Append( "\", \"" );
			writer.AppendText( fileName );
			writer.Indent = 0;
			writer.AppendLine( "\" )]" );
			writer.Append( "private bool Match_" );
			writer.AppendText( matchMethodStatement.RuleMethod.Name );
			writer.Indent = 0;
			writer.Append( "_" );
			writer.AppendText( matchMethodStatement.Name );
			writer.Indent = 0;
			writer.Append( "(" );
			writer.Append( this.Invoke( "MethodParamenters", matchMethodStatement.RuleMethod.Variables ) );
			writer.Indent = 0;
			writer.AppendLine( ")" );
			writer.AppendLine( "{" );
			writer.Indent = 1;
			writer.AppendText( matchMethodStatement.Code );
			writer.Indent = 0;
			writer.AppendLine();
			writer.Append( "}" );

			return writer;
		}

		public CodeWriter Rule( RuleMethodStatement ruleMethodStatement, string fileName, bool isDefault, bool generateAttribute )
		{
			var writer = new CodeWriter();

			if (generateAttribute)
			{
			writer.Indent = 0;
			writer.Append( "[RuleMethod( \"" );
			writer.AppendText( ruleMethodStatement.Name );
			writer.Indent = 0;
			writer.Append( "\", " );
			if (ruleMethodStatement.MatchMethodStatement != null){
			writer.Indent = 0;
			writer.Append( "\"" );
			writer.AppendText( ruleMethodStatement.MatchMethodStatement.Name );
			writer.Indent = 0;
			writer.Append( "\"" );
			}else{
			writer.Indent = 0;
			writer.Append( "null" );
			}
			writer.Indent = 0;
			writer.Append( ", \"" );
			writer.AppendText( fileName );
			writer.Indent = 0;
			writer.AppendLine( "\" )]" );
			}
			writer.Indent = 0;
			writer.Append( "public CodeWriter " );
			writer.AppendText( ruleMethodStatement.Name );
			if (ruleMethodStatement.MatchMethodStatement != null){
			writer.Indent = 0;
			writer.Append( "_" );
			writer.AppendText( ruleMethodStatement.MatchMethodStatement.Name );
			}else if (isDefault){
			writer.Indent = 0;
			writer.Append( "_Default" );
			}
			writer.Indent = 0;
			writer.Append( "(" );
			writer.Append( this.Invoke( "MethodParamenters", ruleMethodStatement.Variables ) );
			writer.Indent = 0;
			writer.AppendLine( ")" );
			writer.AppendLine( "{" );
			writer.Indent = 1;
			writer.AppendLine( "var writer = new CodeWriter();" );
			writer.Indent = 0;
			writer.AppendLine();
			writer.Indent = 1;
			{
				var list = new List<CodeWriter>();
				CodeWriter temp;

				foreach (var item in ruleMethodStatement.Statements)
				{
					temp = this.Invoke( "Statement", item, "writer" );
					list.Add( temp );
				}

				writer.AppendType = AppendType.EmptyLastLine;
				for (var listI = 0; listI < list.Count; listI++)
				{
					var codeWriter = list[listI];
					writer.Append( codeWriter );
					if (listI < list.Count - 1)
						writer.AppendText( "\r\n" );
				}
				writer.AppendType = AppendType.EmptyLastLine;
			}
			writer.Indent = 0;
			writer.AppendLine();
			writer.AppendLine();
			writer.Indent = 1;
			writer.AppendLine( "return writer;" );
			writer.Indent = 0;
			writer.Append( "}" );

			return writer;
		}

		public CodeWriter Method( MethodStatement method, string fileName )
		{
			var writer = new CodeWriter();

			writer.Indent = 0;
			writer.Append( "public " );
			writer.AppendText( method.ReturnType.ToString() );
			writer.Indent = 0;
			writer.Append( " " );
			writer.AppendText( method.Name );
			writer.Indent = 0;
			writer.Append( "(" );
			writer.Append( this.Invoke( "MethodParamenters", method.Variables ) );
			writer.Indent = 0;
			writer.AppendLine( ")" );
			writer.AppendLine( "{" );
			writer.Indent = 1;
			writer.AppendText( method.Code );
			writer.Indent = 0;
			writer.AppendLine();
			writer.Append( "}" );

			return writer;
		}

		public CodeWriter Using( UsingStatement usingStatement )
		{
			var writer = new CodeWriter();

			writer.Append( this.Invoke( "Namespace", usingStatement.Namespace ) );

			return writer;
		}

		public CodeWriter Using( string @namespace )
		{
			var writer = new CodeWriter();

			writer.Indent = 0;
			writer.Append( "using " );
			writer.AppendText( @namespace );
			writer.Indent = 0;
			writer.Append( ";" );

			return writer;
		}

		public CodeWriter MethodParamenters( IEnumerable<Variable> variables )
		{
			var writer = new CodeWriter();

			writer.Indent = 0;
			writer.Append( " " );
			{
				var list = new List<CodeWriter>();
				CodeWriter temp;

				foreach (var item in variables)
				{
					temp = this.Invoke( "Parameter", item );
					list.Add( temp );
				}

				writer.AppendType = AppendType.EmptyLastLine;
				for (var listI = 0; listI < list.Count; listI++)
				{
					var codeWriter = list[listI];
					writer.Append( codeWriter );
					if (listI < list.Count - 1)
						writer.AppendText( ", " );
				}
				writer.AppendType = AppendType.EmptyLastLine;
			}
			writer.Indent = 0;
			writer.Append( " " );

			return writer;
		}

		public CodeWriter Parameter( Variable var )
		{
			var writer = new CodeWriter();

			writer.AppendText( var.ToString() );

			return writer;
		}
	}
}