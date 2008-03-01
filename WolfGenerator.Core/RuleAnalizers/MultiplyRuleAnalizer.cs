/*
 * Created by: 
 * Created: 12 июля 2007 г.
 */

using System;
using System.Collections.Generic;

namespace WolfGenerator.Core
{
	[RuleAnalizer("*")]
	public class MultiplyRuleAnalizer : RuleAnalizer
	{
		public override RuleNode Analize( List<RuleString> ruleStrings, ref int index )
		{
			RuleString ruleString = ruleStrings[index++];

			string command = ruleString.Text.Substring(1);

			int delimiterIndex = command.IndexOf( '|' );
			string count = command.Substring( 0, delimiterIndex ).Trim();
			string str = command.Substring( delimiterIndex + 1, command.Length - delimiterIndex - 1 ).Trim();

			return new MultiplyRuleNode( ruleString.FileName, ruleString.LineNumber, ruleString.ColumNumber,
			                             count, str );
		}
	}

	public class MultiplyRuleNode : RuleNode
	{
		private readonly string count;
		private readonly string str;

		public MultiplyRuleNode( string fileName, int lineNumber, int columnNumber, string count, string str )
			: base( fileName, lineNumber, columnNumber )
		{
			this.str = str;
			this.count = count;
		}

		public override void Workaround( CodeGenerator generator, string prefixName )
		{
		}

		public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
		{
			codeWriter.AppendLine( "{" );

			codeWriter.Indent++;
			codeWriter.AppendLine( "StringBuilder str = new StringBuilder();" );

			codeWriter.AppendLine( "for (int i = 0; i < " + count + "; i++)" );
			codeWriter.AppendLine( "{" );

			codeWriter.Indent++;
			codeWriter.AppendLine( "str.Append( " + str + " );" );
			codeWriter.Indent--;

			codeWriter.AppendLine( "}" );

			codeWriter.AppendLine( stringWriterName + ".Append( str.ToString() );" );
			codeWriter.Indent--;

			codeWriter.AppendLine( "}" );
		}

		public override string GenerateCallMethod( CodeGenerator generator, string prefixName )
		{
			throw new NotImplementedException();
		}
	}
}