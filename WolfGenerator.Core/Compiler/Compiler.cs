using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;
using WolfGenerator.Core.CodeGenerator;

namespace WolfGenerator.Core.Compiler
{
	public class Compiler
	{
		private readonly string fileName;
		private readonly CompilerParameters _compilerParams = new CompilerParameters
		                                                      {
		                                                      	GenerateExecutable = false,
		                                                      	GenerateInMemory = true,
		                                                      	OutputAssembly = "Generated.dll"
		                                                      };

		public Compiler(string fileName)
		{
			this.fileName = fileName;
		}

		public void AddReference(string assemblyName)
		{
			_compilerParams.ReferencedAssemblies.Add(assemblyName);
		}

		public Assembly Compile()
		{
			var options = new Dictionary<string, string>
			              {
			              	{"CompilerVersion", "v3.5"}
			              };
			var codeProvider = new CSharpCodeProvider(options);

			var scanner = new Scanner(this.fileName);
			var parser = new Parser(scanner);

			parser.Parse();

			var generatator = new Generator();
			var generatedCode = generatator.Generate(null, parser.ruleClassStatement, "");

			var result = codeProvider.CompileAssemblyFromSource(this._compilerParams, generatedCode.ToString());

			return result.CompiledAssembly;
		}
	}
}