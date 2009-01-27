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
	public CodeWriter Main( IEnumerable value )
	{
		var writer = new CodeWriter();

		{
			var list = new List<CodeWriter>();
			CodeWriter temp;

			list.Add( (CodeWriter)this.GetType().InvokeMember( "Test",
				BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
				Type.DefaultBinder, this, new object[] { 12, "s" } ) );

			list.Add( (CodeWriter)this.GetType().InvokeMember( "Test",
				BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
				Type.DefaultBinder, this, new object[] { "Test", "s" } ) );

			foreach (var item in value)
			{
				temp = (CodeWriter)this.GetType().InvokeMember( "Test",
					BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
					Type.DefaultBinder, this, new object[] { item, "s" } );
				list.Add( temp );
			}

			for (var listI = 0; listI < list.Count; listI++)
			{
				var codeWriter = list[listI];
				writer.Append( codeWriter );
				if (listI < list.Count - 1)
					writer.AppendText( "\r\nGO\r\n" );
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
