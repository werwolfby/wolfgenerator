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
		writer.AppendText( ruleMethodStatement.MatchMethodStatement.Name );
		}else{
		writer.Indent = 0;
		writer.Append( "null" );
		}
		writer.Indent = 0;
		writer.Append( "\", \"" );
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
		writer.AppendText( ruleMethodStatement.Name );
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
	public CodeWriter Statement( CallStatement call, string writerName )
	{
		var writer = new CodeWriter();

		writer.AppendText( writerName );
		writer.Indent = 0;
		writer.Append( ".Append( this.Invoke( \"" );
		writer.AppendText( call.Name );
		writer.Indent = 0;
		writer.Append( "\", " );
		writer.AppendText( call.Parameters );
		writer.Indent = 0;
		writer.Append( " ) );" );

		return writer;
	}
	public CodeWriter Statement( CodeStatement code, string writerName )
	{
		var writer = new CodeWriter();

		writer.AppendText( code.Value );

		return writer;
	}
	public CodeWriter Statement( JoinStatement join, string writerName )
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.AppendLine( "{" );
		writer.Indent = 1;
		writer.AppendLine( "var list = new List<CodeWriter>();" );
		writer.AppendLine( "CodeWriter temp;" );
		writer.Indent = 0;
		writer.AppendLine();
		writer.Indent = 1;
		{
			var list = new List<CodeWriter>();
			CodeWriter temp;

			foreach (var item in join.Statements)
			{
				temp = this.Invoke( "JoinStatement", item, "temp" );
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
		writer.Append( "writer.AppendType = AppendType." );
		writer.AppendText( join.AppendType.ToString() );
		writer.Indent = 0;
		writer.AppendLine( ";" );
		writer.Indent = 1;
		writer.AppendLine( "for (var listI = 0; listI < list.Count; listI++)" );
		writer.AppendLine( "{" );
		writer.Indent = 2;
		writer.AppendLine( "var codeWriter = list[listI];" );
		writer.AppendText( writerName );
		writer.Indent = 0;
		writer.AppendLine( ".Append( codeWriter );" );
		writer.Indent = 2;
		writer.AppendLine( "if (listI < list.Count - 1)" );
		writer.Indent = 3;
		writer.AppendText( writerName );
		writer.Indent = 0;
		writer.Append( ".AppendText( \"" );
		writer.AppendText( join.String );
		writer.Indent = 0;
		writer.AppendLine( "\" );" );
		writer.Indent = 1;
		writer.AppendLine( "}" );
		writer.AppendLine( "writer.AppendType = AppendType.EmptyLastLine;" );
		writer.Indent = 0;
		writer.Append( "}" );

		return writer;
	}
	public CodeWriter Statement( TextStatement text, string writerName )
	{
		var writer = new CodeWriter();

		if (text.Lines.Count == 0) return new CodeWriter(); 

		var oldIndent = text.Lines[0].Indent;
		writer.AppendText( writerName );
		writer.Indent = 0;
		writer.Append( ".Indent = " );
		writer.AppendText( oldIndent.ToString() );
		writer.Indent = 0;
		writer.AppendLine( ";" );
		for (var i = 0; i < text.Lines.Count; i++)
		{
			var line = text.Lines[i];

			if (oldIndent != line.Indent)
			{
		writer.AppendText( writerName );
		writer.Indent = 0;
		writer.Append( ".Indent = " );
		writer.AppendText( line.Indent.ToString() );
		writer.Indent = 0;
		writer.AppendLine( ";" );
		oldIndent = line.Indent;
			}

			var generatedText = line.GetText().Replace( "\"", "\\\"" );
			string append;

			if (i == text.Lines.Count - 1 && !text.CropLastLine)
			{
				if (generatedText == "") continue;
				append = "Append";
			}
			else append = "AppendLine";

			string result;	
			if (generatedText == "") result = "";
			else                     result = " \"" + generatedText + "\" ";
		writer.AppendText( writerName );
		writer.Indent = 0;
		writer.Append( "." );
		writer.AppendText( append );
		writer.Indent = 0;
		writer.Append( "(" );
		writer.AppendText( result );
		writer.Indent = 0;
		writer.AppendLine( ");" );
		}

		return writer;
	}
	public CodeWriter Statement( ValueStatement value, string writerName )
	{
		var writer = new CodeWriter();

		writer.AppendText( writerName );
		writer.Indent = 0;
		writer.Append( ".AppendText( " );
		writer.AppendText( value.Value );
		writer.Indent = 0;
		writer.Append( " );" );

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