/*****************************************************
 *
 * Created by: 
 * Created: 13.05.2007 12:53:13
 *
 * File: ApplyRuleAnalizer.cs
 * Remarks:
 * 
 *   Command Syntax: <%apply {MethodName}( {param0}, ..., item, ..., {paramN} ) {P0Type} {p0Name}, ..., {PNType} {pNName} from {Enumerator}%>
 *
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WolfGenerator.Core
{
	[RuleAnalizer( "apply" )]
	public class ApplyRuleAnalizer : RuleAnalizer
	{
		public override RuleNode Analize( List<RuleString> ruleStrings, ref int index )
		{
			RuleString applyRuleString = ruleStrings[index++];

			string applyParams = applyRuleString.Text.Substring( "apply".Length + 1 ).Trim();

			if (ruleStrings[index].Text == "\r\n") index++;

			int applyMethodIndex = 0;

			int countOpenBraces = 0;
			for (; applyMethodIndex < applyParams.Length; applyMethodIndex++ )
			{
				if (applyParams[applyMethodIndex] == '(')
				{
					countOpenBraces++;
					break;
				}
			}
			applyMethodIndex++;
			// Find open braces "("
			while (applyMethodIndex++ < applyParams.Length && countOpenBraces > 0)
			{
				if (applyParams[applyMethodIndex] == '(')
				{
					countOpenBraces++;
				}
				if (applyParams[applyMethodIndex] == ')')
				{
					countOpenBraces--;
				}
			}

			if (countOpenBraces != 0) throw new Exception( "Error in apply params string : " + applyRuleString );

			string applyMethod = applyParams.Substring( 0, applyMethodIndex );

			Regex regex = new Regex( @"^(?'params'.*)\s+from\s+(?'fromdata'.*)$" );

			string applyData = applyParams.Substring( applyMethodIndex ).TrimEnd( null );

			Match match = regex.Match( applyData );

			if (!match.Success) throw new Exception( "Can't parse apply value: "+ applyData );

			//if (!fromData.StartsWith( "from" )) throw new Exception( "Can't find `from` word in apply string : " + applyRuleString );

			return
				new ApplyRuleNode( applyRuleString.FileName, applyRuleString.LineNumber, applyRuleString.ColumNumber, applyMethod,
				                   match.Groups["params"].Value, match.Groups["fromdata"].Value );
		}
	}

	public class ApplyRuleNode : RuleNode, IStringArray
	{
		private const string applyRuleSessionName = "ApplyRule";

		private class ApplyRuleNodeSession
		{
			private readonly Dictionary<string, string> applyMethods = new Dictionary<string, string>();

			public void Add( string applyMethod, string generateMethodName )
			{
				this.applyMethods.Add( applyMethod, generateMethodName );
			}

			public bool Contain( string applyMethod )
			{
				return this.applyMethods.ContainsKey( applyMethod );
			}

			public int Count
			{
				get
				{
					return this.applyMethods.Count;
				}
			}

			public string this[string applyMethod]
			{
				get
				{
					return this.applyMethods[applyMethod];
				}
			}
		}

		private static readonly Regex indexRegex = new Regex( @"(\s*|,)index(\s*|,)", RegexOptions.Compiled );

		private readonly string applyMethod;
		private readonly string paramList;
		private readonly string fromPath;

		public ApplyRuleNode( string fileName, int lineNumber, int columnNumber, string applyMethod, string paramList, string fromPath ) : base( fileName, lineNumber, columnNumber )
		{
			this.applyMethod = applyMethod;
			this.paramList = paramList;
			this.fromPath = fromPath;
		}

		public override void Workaround( CodeGenerator generator, string prefixName )
		{
			ApplyRuleNodeSession applyRuleData;
			object applyRuleDataObject;
			if (!generator.Session.TryGetValue( applyRuleSessionName, out applyRuleDataObject ))
			{
				applyRuleData = new ApplyRuleNodeSession();
				generator.Session[applyRuleSessionName] = applyRuleData;
			}
			else
			{
				applyRuleData = (ApplyRuleNodeSession)applyRuleDataObject;
			}

			CodeWriter codeWriter = new CodeWriter();

			if (applyRuleData.Contain( applyMethod )) return;

			int firstParamListIndex = applyMethod.IndexOf( '(' );
			int lastParamListIndex = applyMethod.LastIndexOf( ')' );

			string methodName = applyMethod.Substring( 0, firstParamListIndex );
			string methodParams = applyMethod.Substring( firstParamListIndex + 1, lastParamListIndex - firstParamListIndex - 1 );

			bool containIndex = indexRegex.Match( methodParams ).Success;

			string innerApplyMethodName = prefixName + "ApplyMethod" + applyRuleData.Count;

			if (!string.IsNullOrEmpty(paramList))
			{
				//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
				codeWriter.AppendLine( "public List<string> " + innerApplyMethodName + "( " + paramList + ", IEnumerable enumList )" );
			}
			else
			{
				//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
				codeWriter.AppendLine( "public List<string> " + innerApplyMethodName + "( IEnumerable enumList )" );
			}
			codeWriter.AppendLine( "{" );
			codeWriter.Indent++;
			codeWriter.AppendLine( "List<string> strings = new List<string>();" );
			if (containIndex) codeWriter.AppendLine( "int index = 0;" );
			codeWriter.AppendLine( "foreach( object item in enumList )" );
			codeWriter.AppendLine( "{" );
			codeWriter.Indent++;

			//codeWriter.AppendLine( string.Format( "strings.Add( {0} );", applyMethod ) );
			string applyMethodCall = "this.GetType().InvokeMember( \"" + methodName + "\",\n" +
			                         "                             BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,\n" +
			                         "                             Type.DefaultBinder, this, new object[] { " + methodParams + " } ).ToString()";

			codeWriter.AppendLine( string.Format( "string value = {0};", applyMethodCall ) );
			codeWriter.AppendLine( string.Format( "if (!string.IsNullOrEmpty( value )) strings.Add( value );" ) );
			if (containIndex) codeWriter.AppendLine( "index++;" );
			codeWriter.Indent--;
			codeWriter.AppendLine( "}" );
			codeWriter.AppendLine( "return strings;" );
			codeWriter.Indent--;
			codeWriter.AppendLine( "}" );

			applyRuleData.Add( applyMethod, innerApplyMethodName );

			generator.AddCode( "Apply Methods", codeWriter.ToString() );
		}

		public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
		{
			ApplyRuleNodeSession applyRuleData;
			object applyRuleDataObject;
			if (!generator.Session.TryGetValue( applyRuleSessionName, out applyRuleDataObject ))
			{
				applyRuleData = new ApplyRuleNodeSession();
				generator.Session[applyRuleSessionName] = applyRuleData;
			}
			else
			{
				applyRuleData = (ApplyRuleNodeSession)applyRuleDataObject;
			}

			string applyInnerMethod = applyRuleData[this.applyMethod];

			//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
			codeWriter.AppendLine( "{" );
			codeWriter.Indent++;
			codeWriter.AppendLine( string.Format( @"string value = {0};", this.GenerateCallMethod( generator, prefixName ) ) );
			codeWriter.AppendLine( string.Format( @"if (value.EndsWith( ""\r\n"" )) {0}.AppendLine( value );", stringWriterName ) );
			codeWriter.AppendLine( string.Format( @"else {0}.Append( value );", stringWriterName ) );
			//codeWriter.AppendLine( string.Format( "{0}.AppendLine( " + GenerateCallMethod( generator, prefixName ) + " );", 
			//    stringWriterName, applyInnerMethod ) );
			codeWriter.Indent--;
			codeWriter.AppendLine( "}" );
		}

		#region IStringArray Members
		public string GetStringArrayMethodName( CodeGenerator generator )
		{
			ApplyRuleNodeSession applyRuleData;
			object applyRuleDataObject;
			if (!generator.Session.TryGetValue( applyRuleSessionName, out applyRuleDataObject )) throw new Exception( "Can't get need session" );
			applyRuleData = (ApplyRuleNodeSession)applyRuleDataObject;

			string[] joinParamArray = this.paramList.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
			string[] paramNameArray = new string[joinParamArray.Length + 1];

			for (int i = 0; i < joinParamArray.Length; i++)
			{
				string param = joinParamArray[i].Trim();

				string[] paramValue = param.Split( ' ' );

				paramNameArray[i] = paramValue[1].Trim();
			}

			paramNameArray[paramNameArray.Length - 1] = fromPath.Replace( "{", "{{" ).Replace( "}", "}}" );

			return applyRuleData[applyMethod] + "( " + string.Join( ", ", paramNameArray ) + " )";
		}
		#endregion

		public override string GenerateCallMethod( CodeGenerator generator, string prefixName )
		{
			return "string.Join( \"\", " + GetStringArrayMethodName( generator ) + ".ToArray() )";
		}
	}
}