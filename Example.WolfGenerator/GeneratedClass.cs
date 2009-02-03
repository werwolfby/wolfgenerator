using System;
using System.Reflection;
using System.Text;
using System.Collections;
using Example.WolfGenerator;
using WolfGenerator.Core.Writer;
using System.Collections.Generic;

public partial class MainClass
{
	public int Test2( int a, int b )
	{
		return 15;
	}
	public CodeWriter TempRule()
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.Append( "Test text" );

		return writer;
	}
	public CodeWriter SetField( string field )
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

			list.Add( (CodeWriter)this.GetType().InvokeMember( "SetField",
				BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
				Type.DefaultBinder, this, new object[] { "Id" } ) );

			list.Add( (CodeWriter)this.GetType().InvokeMember( "SetField",
				BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
				Type.DefaultBinder, this, new object[] { "Name" } ) );

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
