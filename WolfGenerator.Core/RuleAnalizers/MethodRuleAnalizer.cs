/*****************************************************
 *
 * Created by: WerWolf
 * Created: 31.05.2007 10:58:13
 *
 * File: MethodRuleAnalizer.cs
 * Remarks:
 *
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core
{
	[RuleAnalizer( "method" )]
	public class MethodRuleAnalizer : RuleAnalizer
	{
		public override RuleNode Analize( List<RuleString> ruleStrings, ref int index )
		{
			RuleString headerRuleString = ruleStrings[index++];
			if (!(headerRuleString is CommandRuleString) && !headerRuleString.Text.StartsWith( "method" ))
				throw new Exception( "Wrong header rule string for `rule` node" );

			string ruleMethodHeader = headerRuleString.Text.Substring( "method".Length ).Trim();

			if (ruleStrings[index].Text == "\r\n") index++;

			StringBuilder builder = new StringBuilder();

			for (; index < ruleStrings.Count; index++)
			{
				RuleString ruleString = ruleStrings[index];

				if (ruleString is TextRuleString) builder.Append( ruleString.Text );
				else if (ruleString is CommandRuleString)
				{
					if (ruleString.Text == "end method") break;

					throw new Exception( "Method rule can contain only text nodes" );
				}
				else if (ruleString is ValueRuleString)
				{
					throw new Exception( "Method rule can contain only text nodes" );
				}
			}

			return
				new MethodRuleNode( headerRuleString.FileName, headerRuleString.LineNumber, headerRuleString.ColumNumber, ruleMethodHeader,
				                    builder.ToString() );
		}
	}

	public class MethodRuleNode : RuleNode
	{
		private readonly string methodHeader;
		private readonly string text;

		public MethodRuleNode( string fileName, int lineNumber, int columnNumber, string methodHeader, string text ) : base( fileName, lineNumber, columnNumber )
		{
			this.methodHeader = methodHeader;
			this.text = text;
		}

		public override void Workaround( CodeGenerator generator, string prefixName )
		{
			//string line = "#line " + LineNumber + " \"" + FileName + "\"";
			generator.AddCode( "Method", "public " + methodHeader + "\r\n" + "{" + text + "}" );
		}

		public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
		{
		}

		public override string GenerateCallMethod( CodeGenerator generator, string prefixName )
		{
			return null;
		}
	}
}