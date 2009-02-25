using System;
using System.IO;
using WolfGenerator.Core;

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
			var code = global::WolfGenerator.Core.Writer.Generator.Generate( parser.ruleClassStatement, "test.rule" ).ToString();

			using (var stream = new StreamWriter( "../../GeneratedClass.cs" ))
			{
				stream.WriteLine( code );
			}
		}
	}
}
