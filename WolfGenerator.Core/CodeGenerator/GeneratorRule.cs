using System;
using System.Collections.Generic;
using WolfGenerator.Core.Writer;
using WolfGenerator.Core;
using System.Linq;
using WolfGenerator.Core.AST;

public partial class Generator : GeneratorBase
{
	public CodeWriter Generate( string @namespace, RuleClassStatement ruleClassStatement, string fileName )
	{
		var writer = new CodeWriter();

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
		writer.Append( "public partial class " );
		writer.AppendText( ruleClassStatement.Name );
		writer.Indent = 0;
		writer.AppendLine( " : GeneratorBase" );
		writer.AppendLine( "{" );
		writer.Indent = 1;
		{
			var list = new List<CodeWriter>();
			CodeWriter temp;

			foreach (var item in ruleClassStatement.MatchMethodGroups.Where( mmg => mmg.IsMatched ).SelectMany( mmg => mmg.MatchStatements ))
			{
				temp = this.Invoke( "Match", item, fileName );
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
		writer.AppendLine( "\" ]" );
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
		writer.Append( "\", \"" );
		if (ruleMethodStatement.MatchMethodStatement != null){
		writer.Indent = 0;
		writer.Append( " " );
		writer.AppendText( ruleMethodStatement.MatchMethodStatement.Name );
		writer.Indent = 0;
		writer.Append( " " );
		}else{
		writer.Indent = 0;
		writer.Append( "null" );
		}
		writer.Indent = 0;
		writer.AppendLine( "\" )]" );
		}

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