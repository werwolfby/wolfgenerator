using System;
using System.Collections.Generic;
using WolfGenerator.Core.Writer;
using WolfGenerator.Core;
using System.Linq;
using WolfGenerator.Core.AST;

public partial class Generator : GeneratorBase
{
	public CodeWriter JoinStatement( ApplyStatement statement, string tempWriter, string list )
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.Append( "foreach (var item in " );
		writer.AppendText( statement.From );
		writer.Indent = 0;
		writer.AppendLine( ")" );
		writer.AppendLine( "{" );
		writer.Indent = 1;
		writer.AppendText( tempWriter );
		writer.Indent = 0;
		writer.Append( " = this.Invoke( \"" );
		writer.AppendText( statement.ApplyMethod );
		writer.Indent = 0;
		writer.Append( "\", " );
		writer.AppendText( statement.Parameters );
		writer.Indent = 0;
		writer.AppendLine( " );" );
		writer.Indent = 1;
		writer.AppendText( list );
		writer.Indent = 0;
		writer.Append( ".Add( " );
		writer.AppendText( tempWriter );
		writer.Indent = 0;
		writer.AppendLine( " );" );
		writer.Append( "}" );

		return writer;
	}
	public CodeWriter JoinStatement( CallStatement statement, string tempWriter, string list )
	{
		var writer = new CodeWriter();

		writer.AppendText( list );
		writer.Indent = 0;
		writer.Append( ".Append( this.Invoke( \"" );
		writer.AppendText( statement.Name );
		writer.Indent = 0;
		writer.Append( "\", " );
		writer.AppendText( statement.Parameters );
		writer.Indent = 0;
		writer.Append( " ) );" );

		return writer;
	}
	public CodeWriter JoinStatement( ValueStatement statement, string tempWriter, string list )
	{
		var writer = new CodeWriter();

		writer.AppendText( tempWriter );
		writer.Indent = 0;
		writer.AppendLine( " = new CodeWriter();" );
		writer.AppendText( tempWriter );
		writer.Indent = 0;
		writer.Append( ".AppendText( " );
		writer.AppendText( statement.Value );
		writer.Indent = 0;
		writer.AppendLine( " );" );
		writer.AppendText( list );
		writer.Indent = 0;
		writer.Append( ".Add( " );
		writer.AppendText( tempWriter );
		writer.Indent = 0;
		writer.Append( " );" );

		return writer;
	}
}