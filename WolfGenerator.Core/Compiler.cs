/*****************************************************
 *
 * Created by: 
 * Created: 14.05.2007 11:21:46
 *
 * File: Compiler.cs
 * Remarks:
 *
 *****************************************************/

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace WolfGenerator.Core
{
	public class Compiler
	{
		private readonly string ruleCode;
		private List<RuleString> ruleStrings;
		private string programCode;
		private ClassRuleNode classRuleNode;
		private CompilerResults compilerResults;

		private readonly string ruleFileDirectory;
		private readonly string ruleFilePath;

		public Compiler( string ruleFilePath )
		{
			FileInfo fileInfo = new FileInfo( ruleFilePath );

			this.ruleFileDirectory = fileInfo.Directory.FullName;

			TextReader textReader = new StreamReader( ruleFilePath, Encoding.GetEncoding( 1251 ) );
			this.ruleCode = textReader.ReadToEnd();
			textReader.Close();

			this.ruleFilePath = ruleFilePath;
		}

		public Compiler( string code, string fileName )
		{
			this.ruleCode = code;
		
			this.ruleFilePath = fileName;
		}

		public CompilerResults CompilerResults
		{
			get
			{
				return this.compilerResults;
			}
		}

		public Assembly CompiledAssembly
		{
			get
			{
				return this.compilerResults.CompiledAssembly;
			}
		}

		public string ProgramCode
		{
			get
			{
				return this.programCode;
			}
		}

		public void Parse()
		{
			RuleParser parser = new RuleParser( this.ruleCode, ruleFilePath );
			List<RuleString> buildRuleStringList = parser.Build();

			for (int i = 0; i < buildRuleStringList.Count; i++)
			{
				RuleString ruleString = buildRuleStringList[i];

				if (ruleString is CommandRuleString && ruleString.Text.StartsWith( "include" ))
				{
					string includeFileName = ruleString.Text.Substring( "include".Length + 1 ).Trim();

					if (includeFileName[0] != '"' || includeFileName[includeFileName.Length - 1] != '"')
						throw new Exception( "Include file name must wrapped on quot symbols" );

					Compiler compiler = new Compiler( ruleFileDirectory + "\\" + includeFileName.Substring( 1, includeFileName.Length - 2 ) );
					compiler.Parse();

					List<RuleString> includeRuleStringList = compiler.ruleStrings;

					buildRuleStringList.RemoveAt( i );

					buildRuleStringList.InsertRange( i, includeRuleStringList );

					i += includeRuleStringList.Count;
				}
			}

			this.ruleStrings = buildRuleStringList;
		}

		public void Generate()
		{
			int index = 0;

			RuleClassRuleAnalizer ruleAnalizer = new RuleClassRuleAnalizer();
			this.classRuleNode = (ClassRuleNode)ruleAnalizer.Analize( ruleStrings, ref index );

			CodeWriter writer = new CodeWriter();
			this.classRuleNode.AddUsingList( writer );
			writer.AppendLine( "using System.Reflection;" );
			writer.AppendLine( "using System.Collections;" );
			writer.AppendLine();

			this.programCode = this.classRuleNode.Generate( writer );
		}

		public void Compile( params string[] extReferencedAssemblies )
		{
			CodeDomProvider codeDomProvider = new CSharpCodeProvider();

			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.GenerateInMemory = false;

			foreach (ClassRuleNode.ReferenceData reference in this.classRuleNode.ReferenceList)
			{
				Assembly assembly = Assembly.LoadFrom( reference.ReferenceName );
				compilerParameters.ReferencedAssemblies.Add( assembly.Location );
			}

			foreach (AssemblyName assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
			{
				if (assemblyName.CodeBase != null) compilerParameters.ReferencedAssemblies.Add( assemblyName.FullName );
			}

			compilerParameters.ReferencedAssemblies.Add( Assembly.GetExecutingAssembly().Location );

			if (extReferencedAssemblies != null && extReferencedAssemblies.Length > 0)
			{
				foreach (string referencedAssembly in extReferencedAssemblies)
				{
					Assembly assembly = Assembly.Load( referencedAssembly );
					compilerParameters.ReferencedAssemblies.Add( assembly.Location );
				}
			}

			Assembly entryAssembly = Assembly.GetEntryAssembly();

			if (entryAssembly != null)
			{
			    foreach (AssemblyName assemblyName in entryAssembly.GetReferencedAssemblies())
			    {
			        Assembly assembly = Assembly.Load( assemblyName );
			        compilerParameters.ReferencedAssemblies.Add( assembly.Location );
			    }
			}

			this.compilerResults = codeDomProvider.CompileAssemblyFromSource( compilerParameters, programCode );
		}

		public object ExecuteMethod( string methodName, params object[] methodParams )
		{
			Type type = compilerResults.CompiledAssembly.GetType( classRuleNode.Name );
			object generatedMethod = Activator.CreateInstance( type );

			MethodInfo methodInfo = type.GetMethod( methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod );

			return methodInfo.Invoke( generatedMethod, methodParams );
		}
	}
}