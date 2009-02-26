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
					temp = this.Invoke( "JoinStatement", item, "temp", "list" );
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
	}
}