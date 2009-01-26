using System;
using System.IO;
using WolfGenerator.Core;
using CodeWriter=WolfGenerator.Core.Writer.CodeWriter;
using Generator=WolfGenerator.Core.Writer.Generator;

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

			Console.WriteLine( mainClass.Main( new object[] { 1, "String" } ) );

			var code = Generator.Generate( parser.ruleClassStatement ).ToString();

			using (var stream = new StreamWriter( "../../GeneratedClass.cs" ))
			{
				stream.WriteLine( code );
			}
		}
	}
}
