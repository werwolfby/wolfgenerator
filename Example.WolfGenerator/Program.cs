using System;
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

			Console.WriteLine( Generator.Generate( parser.ruleClassStatement ).ToString() );
		}
	}
}
