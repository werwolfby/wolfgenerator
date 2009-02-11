using System;
using System.Collections.Generic;
using WolfGenerator.Core.Writer;
using WolfGenerator.Core;
using System.Text;
using System.Collections;
using Example.WolfGenerator;

public partial class MainClass : GeneratorBase
{
	public CodeWriter TempRule()
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.Append( "Test text" );

		return writer;
	}
	[MatchMethod( "SetField", "test.rule" )]
	private bool Match_SetField_IsId( string field )
	{
		return field == "Id";
	}
	public CodeWriter SetField_IsId( string field )
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.Append( "'[Id]' = @" );
		writer.AppendText( field );

		return writer;
	}
	public CodeWriter SetField_Default( string field )
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.Append( "'[" );
		writer.AppendText( field );
		writer.Indent = 0;
		writer.Append( "]' = @" );
		writer.AppendText( field );

		return writer;
	}
	public CodeWriter Main( IEnumerable value )
	{
		var writer = new CodeWriter();

		{
			var list = new List<CodeWriter>();
			CodeWriter temp;

			temp = new CodeWriter();
			temp.AppendText( "Temp" );
			list.Add( temp );

			list.Add( this.Invoke( "SetField", "Id" ) );

			list.Add( this.Invoke( "SetField", "Name" ) );

			for (var listI = 0; listI < list.Count; listI++)
			{
				var codeWriter = list[listI];
				writer.Append( codeWriter );
				if (listI < list.Count - 1)
					writer.AppendText( ", " );
			}
		}

		return writer;
	}
	public CodeWriter Test( int value, object a )
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.Append( "CREATE TABLE [Table_" );
		writer.AppendText( value.ToString() );
		writer.Indent = 0;
		writer.AppendLine( "]" );
		writer.AppendLine( "(" );
		writer.Indent = 1;
		writer.AppendLine( "[Id] INT NOT NULL IDENTITY(1, 1)," );
		writer.AppendLine( "[Name] varchar(30) NOT NULL" );
		writer.Indent = 0;
		writer.AppendLine( ")" );
		writer.AppendLine( "ON" );
		writer.Append( "[PRIMARY]" );

		return writer;
	}
	public CodeWriter Test( string value, object a )
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.Append( "CREATE TABLE [" );
		writer.AppendText( value );
		writer.Indent = 0;
		writer.AppendLine( "]" );
		writer.AppendLine( "(" );
		writer.Indent = 1;
		writer.AppendLine( "[Id] INT NOT NULL IDENTITY(1, 1)," );
		writer.AppendLine( "[Name] varchar(30) NOT NULL" );
		writer.Indent = 0;
		writer.AppendLine( ")" );
		writer.AppendLine( "ON" );
		writer.Append( "[PRIMARY]" );

		return writer;
	}
}
