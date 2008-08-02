using System;
using System.CodeDom.Compiler;
using WolfGenerator.Core;

namespace Example.WolfGenerator
{
	class Program
	{
		static void Main()
		{
			var compiler = new Compiler( @"Rules\Main.txt" );

			compiler.Parse();
			compiler.Generate();
			compiler.Compile();
			if (compiler.CompilerResults.Errors.HasErrors)
			{
				foreach (CompilerError error in compiler.CompilerResults.Errors)
				{
					Console.WriteLine( error.ErrorText );
				}

				Console.WriteLine( compiler.ProgramCode );
			}
			else
			{
				Console.WriteLine( compiler.ExecuteMethod( "Main", (object)(new object[] { "string", 10, 12, "Test" }) ) );
			}
		}
	}
}
