using System;
using System.Reflection;
using System.Text;
using System.Collections;
using Example.WolfGenerator;
using WolfGenerator.Core.Writer;
using System.Collections.Generic;

public partial class MainClass
{
	public CodeWriter Main( IEnumerable value)
	{
		var writer = new CodeWriter();

		writer.Indent = 1;
		{
			var list = new List<CodeWriter>();
			CodeWriter temp;

			temp = new CodeWriter();
			temp.Append( "Test" );

			list.Add( temp );

			temp = new CodeWriter();
			temp.Append( "Test" );

			list.Add( temp );

			foreach (var item in value)
			{
				temp = (CodeWriter)this.GetType().InvokeMember( "Test",
				                                                BindingFlags.Instance | BindingFlags.InvokeMethod |
				                                                BindingFlags.Public,
				                                                Type.DefaultBinder, this, new object[] { item, 1 } );
				list.Add( temp );
			}

			for (var listI = 0; listI < list.Count; listI++)
			{
				var codeWriter = list[listI];
				writer.Append( codeWriter );
				if (listI < list.Count - 1)
					writer.AppendText( "\r\n" );
			}
		}

		return writer;
	}
	public CodeWriter Test( int value, object a)
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.AppendLine( "Extend value" );
		writer.AppendLine( "Int : " );
		writer.Append( value.ToString() );
		writer.Indent = 0;
		writer.AppendLine();
		writer.Append( "New line" );

		return writer;
	}
	public CodeWriter Test( string value, object a)
	{
		var writer = new CodeWriter();

		writer.Indent = 0;
		writer.Append( "string : " );
		writer.Append( value.ToString() );

		return writer;
	}
}
