/*****************************************************
 *
 * Created by: WerWolf
 * Created: 13.05.2007 18:27:02
 *
 * File: CallRuleAnalizer.cs
 * Remarks:
 *
 *****************************************************/

using System.Collections.Generic;

namespace WolfGenerator.Core
{
	[RuleAnalizer( "call" )]
	public class CallRuleAnalizer : RuleAnalizer
	{
		public override RuleNode Analize( List<RuleString> ruleStrings, ref int index )
		{
			RuleString callRuleString = ruleStrings[index++];

			if (ruleStrings[index] is TextRuleString && ruleStrings[index].Text == "\r\n") index++;

			string callText = callRuleString.Text.Substring( "call".Length ).Trim();

			return new CallRuleNode( callRuleString.FileName, callRuleString.LineNumber, callRuleString.ColumNumber, callText );
		}
	}

	public class CallRuleNode : RuleNode
	{
		private readonly string methodName;
		private readonly string methodParams;

		public CallRuleNode( string fileName, int lineNumber, int columnNumber, string callText ) : base( fileName, lineNumber, columnNumber )
		{
			int openBracetIndex = callText.IndexOf( '(' );
			int closeBracetIndex = callText.LastIndexOf( ')' );

			this.methodName = callText.Substring( 0, openBracetIndex );
			this.methodParams = callText.Substring( openBracetIndex + 1, closeBracetIndex - openBracetIndex - 1 );
		}

		public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
		{
			//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
			codeWriter.AppendLine( "{" );
			codeWriter.Indent++;
			codeWriter.AppendFormatLine( "string value = {0};", GenerateCallMethod( generator, prefixName ) );
			codeWriter.AppendFormatLine( @"if (value.EndsWith( ""\r\n"" )) {0}.AppendLine( value );", stringWriterName );
			codeWriter.AppendFormatLine( @"else {0}.Append( value );", stringWriterName );
			codeWriter.Indent--;
			codeWriter.AppendLine( "}" );
		}

		public override void Workaround( CodeGenerator generator, string prefixName )
		{
		}

		public override string GenerateCallMethod( CodeGenerator generator, string prefixName )
		{
			return "this.GetType().InvokeMember( \"" + methodName + "\",\n" +
			       "                             BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,\n" +
			       "                             Type.DefaultBinder, this, new object[] { " + methodParams + " } ).ToString()";
		}
	}
}