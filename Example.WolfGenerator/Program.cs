using System;
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

			Console.WriteLine( parser.ruleClassStatement.ToString() );
		}
	}
}
