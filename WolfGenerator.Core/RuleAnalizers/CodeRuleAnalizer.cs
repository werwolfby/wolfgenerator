/*****************************************************
 *
 * Created by: WerWolf
 * Created: 14.05.2007 2:18:52
 *
 * File: CodeRuleAnalizer.cs
 * Remarks:
 *
 *****************************************************/

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WolfGenerator.Core
{
	[RuleAnalizer("%")]
	public class CodeRuleAnalizer : RuleAnalizer
	{
		public override RuleNode Analize( List<RuleString> ruleStrings, ref int index )
		{
			RuleString ruleString = ruleStrings[index++];

			string code = ruleString.Text.Substring( 1 ).Trim();

			if (ruleStrings[index].Text == "\r\n") index++;

			return new CodeRuleNode( ruleString.FileName, ruleString.LineNumber, ruleString.ColumNumber, code );
		}
	}

	public class CodeRuleNode : RuleNode
	{
		private readonly string code;
		private static readonly Regex regex = new Regex( @"((\$incIndent\$)|(\$decIndent\$))", RegexOptions.Compiled );

		public CodeRuleNode( string fileName, int lineNumber, int columnNumber, string code ) : base( fileName, lineNumber, columnNumber )
		{
			this.code = code;
		}

		public override void Workaround( CodeGenerator generator, string prefixName )
		{
		}

		public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
		{
			MatchCollection matchCollection = regex.Matches( this.code );
			int startIndex = 0;

			foreach (Match match in matchCollection)
			{
				string tempCode = code.Substring( startIndex, match.Index );

				startIndex = match.Index + match.Length;

				//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
				if (tempCode != "") codeWriter.AppendLine( tempCode );

				if (match.Value == "$incIndent$") codeWriter.Indent++;
				if (match.Value == "$decIndent$") codeWriter.Indent--;
			}

			if (code.Length - startIndex > 0)
			{
				//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
				codeWriter.AppendLine( code.Substring( startIndex, code.Length - startIndex ) );
			}
		}

		public override string GenerateCallMethod( CodeGenerator generator, string prefixName )
		{
			return null;
		}
	}
}