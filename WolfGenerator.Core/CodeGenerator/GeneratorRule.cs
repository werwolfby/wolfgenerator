using System;
using System.Collections.Generic;
using WolfGenerator.Core.Writer;
using WolfGenerator.Core;
using WolfGenerator.Core.AST;

public partial class Generator : GeneratorBase
{
	public CodeWriter Main( string ns, RuleClassStatement ruleClassStatement, string fileName )
	{
		var writer = new CodeWriter();

		if (ns != null)
		{
		writer.Indent = 0;
		writer.Append( "namespace " );
		writer.AppendText( ns );
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

		return writer;
	}
	public CodeWriter Namespace( UsingStatement ns )
	{
		var writer = new CodeWriter();

		writer.Append( this.Invoke( "Namespace", ns.Namespace ) );

		return writer;
	}
	public CodeWriter Namespace( string ns )
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.Append( "using " );
		writer.AppendText( ns );
		writer.Indent = 0;
		writer.Append( ";" );

		return writer;
	}
}