using System;
using System.IO;
using WolfGenerator.Core;
using WolfGenerator.Core.CodeGenerator;
using WolfGenerator.Core.Parsing;

namespace Example.WolfGenerator
{
	class Program
	{
		static void Main()
		{
			var scanner = new Scanner( @"Rules\Main.txt" );
			var parser = new Parser( scanner );

			parser.Parse();

			var mainClass = new MainClass();

			try
			{
				Console.WriteLine( mainClass.Main( new object[] { "Id", "Name" } ) );
			}
			catch (Exception e)
			{
				Console.WriteLine( e );
			}
			var code = new Generator().Generate( null, parser.ruleClassStatement, "test.rule" ).ToString();

			using (var stream = new StreamWriter( "../../GeneratedClass.cs" ))
			{
				stream.WriteLine( code );
			}
		}
	}
}
