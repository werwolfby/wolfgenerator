using System;
using WolfGenerator.Core;
using CodeWriter=WolfGenerator.Core.Writer.CodeWriter;

namespace Example.WolfGenerator
{
	class Program
	{
		static void Main()
		{
			var scanner = new Scanner( @"Rules\Main.txt" );
			var parser = new Parser( scanner );

			parser.Parse();

			//Console.WriteLine( parser.ruleClassStatement.ToString() );

			var writer = new CodeWriter();

			writer.AppendLine( "using WolfGenerator.Core" );
			writer.AppendLine( "using System.Collection" );

			writer.AppendLine();
			
			writer.AppendLine( "namespace Example" );
			writer.AppendLine( "{" );
			writer.Indent++;
			writer.AppendLine();

			writer.Append( "class " );
			writer.AppendLine( "Test" );
			writer.AppendLine( "{" );
			writer.Indent++;

			writer.AppendLine( "static void Main()" );
			writer.AppendLine( "{" );
			writer.Indent++;

			writer.AppendLine( "System.Console.WriteLine( \"Test string\" );" );

			writer.Indent--;
			writer.AppendLine( "}" );

			writer.Indent--;
			writer.AppendLine( "}" );

			writer.Indent--;
			writer.AppendLine( "}" );

			Console.WriteLine( writer.ToString() );
		}
	}
}
