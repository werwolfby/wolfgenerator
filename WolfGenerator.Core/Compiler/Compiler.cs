﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using WolfGenerator.Core.CodeGenerator;
using WolfGenerator.Core.Parsing;

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
			this.AddReference("WolfGenerator.Core.dll");
		}

		public void AddReference(string assemblyName)
		{
			_compilerParams.ReferencedAssemblies.Add(assemblyName);
		}

		public Assembly Compile(string generatedCodeFileName = null)
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

			if (generatedCodeFileName != null)
			{
				using (var stream = new StreamWriter(generatedCodeFileName, false, Encoding.UTF8))
				{
					stream.Write(generatedCode);
				}
			}

			var result = codeProvider.CompileAssemblyFromSource(this._compilerParams, generatedCode.ToString());

			return result.CompiledAssembly;
		}
	}
}