/*****************************************************
 *
 * Created by: WerWolf
 * Created: 12.05.2007 0:37:11
 *
 * File: RuleRuleAnalizer.cs
 * Remarks:
 *
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WolfGenerator.Core
{
	[RuleAnalizer( "rule" )]
	public class RuleRuleAnalizer : RuleAnalizer
	{
		public override RuleNode Analize( List<RuleString> ruleStrings, ref int index )
		{
			RuleString headerRuleString = ruleStrings[index++];
			if (!(headerRuleString is CommandRuleString) && !headerRuleString.Text.StartsWith( "rule" )) 
				throw new Exception( "Wrong header rule string for `rule` node" );

			string ruleMethodHeader = headerRuleString.Text.Substring( 4 ).Trim();

			if (ruleStrings[index].Text == "\r\n") index++;

			List<RuleRuleNode.RuleItem> ruleItemList = new List<RuleRuleNode.RuleItem>();

			bool lastEnd = false;

			for(; index < ruleStrings.Count; index++)
			{
				RuleString ruleString = ruleStrings[index];

				if (ruleString is TextRuleString)
				{
					string text = ruleString.Text;
					if (lastEnd && text.EndsWith( "\r\n" ))
					{
						text = text.Substring( 0, text.Length - 2 );
						lastEnd = false;
					}
					if (text != "") ruleItemList.Add( new RuleRuleNode.TextRuleItem( text ) );
				}
				else if (ruleString is CommandRuleString)
				{
					if (ruleString.Text == "end rule") break;
					if (ruleString.Text == "end")
					{
						lastEnd = true;
						continue;
					}

					RuleAnalizer analizer = FindAnAnalizer( ruleString );
					RuleNode ruleNode = analizer.Analize( ruleStrings, ref index );
					ruleItemList.Add( new RuleRuleNode.NodeRuleItem( ruleNode ) );

					index--;
				}
				else if (ruleString is ValueRuleString)
				{
					ruleItemList.Add( new RuleRuleNode.ValueRuleItem( ruleString.Text ) );
				}
			}

			return
				new RuleRuleNode( headerRuleString.FileName, headerRuleString.LineNumber, headerRuleString.ColumNumber, ruleMethodHeader, ruleItemList );
		}
	}

	public class RuleRuleNode : RuleNode
	{
		#region Inner classes
		public abstract class RuleItem
		{
			public abstract void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName );

			public abstract void Workaround( CodeGenerator generator, string prefixName );
		}

		public class TextRuleItem : RuleItem
		{
			private static readonly string lineFormat = "{0}.AppendLine( \"{1}\" );";
			private static readonly string format = "{0}.Append( \"{1}\" );";
			private static readonly string emptyFormat = "{0}.AppendLine();";

			private readonly string text;

			public TextRuleItem( string text )
			{
				this.text = text;
			}

			public string Text
			{
				get
				{
					return this.text;
				}
			}

			public override void Generate( string writerName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
			{
				if (text == "\r\n")
				{
					codeWriter.AppendLine( string.Format( emptyFormat, writerName ) );
					return;
				}

				string convertText = this.text.Replace( "\"", "\\\"" );

				if (text.EndsWith( "\r\n" ))
					codeWriter.AppendLine( string.Format( lineFormat, writerName, convertText.Substring( 0, convertText.Length - 2 ) ) );
				else codeWriter.AppendLine( string.Format( format, writerName, convertText ) );
			}

			public override void Workaround( CodeGenerator generator, string prefixName )
			{
			}
		}

		public class NodeRuleItem : RuleItem
		{
			private readonly RuleNode node;

			public NodeRuleItem( RuleNode node )
			{
				this.node = node;
			}

			public RuleNode Node
			{
				get
				{
					return this.node;
				}
			}

			public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
			{
				this.node.Generate( stringWriterName, generator, codeWriter, prefixName );
			}

			public override void Workaround( CodeGenerator generator, string prefixName )
			{
				this.node.Workaround( generator, prefixName );
			}
		}

		public class ValueRuleItem : RuleItem
		{
			private readonly string text;

			public ValueRuleItem( string text )
			{
				this.text = text;
			}

			public string Text
			{
				get
				{
					return this.text;
				}
			}

			public override void Generate( string writerName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
			{
				codeWriter.AppendLine( writerName + ".Append( " + text + " );" );
			}

			public override void Workaround( CodeGenerator generator, string prefixName )
			{
			}
		}
		#endregion

		#region Fields
		private readonly string ruleMethodHeader;
		private readonly List<RuleItem> ruleItemList;
		#endregion

		#region Constructors
		public RuleRuleNode( string fileName, int lineNumber, int columnNumber, string ruleMethodHeader, List<RuleItem> ruleItemList ) : base( fileName, lineNumber, columnNumber )
		{
			this.ruleMethodHeader = ruleMethodHeader;
			this.ruleItemList = ruleItemList;
		}
		#endregion

		#region Properties
		public string RuleMethodHeader
		{
			get
			{
				return this.ruleMethodHeader;
			}
		}
		#endregion

		public override void Workaround( CodeGenerator generator, string prefixName )
		{
			foreach (RuleItem ruleItem in ruleItemList)
			{
				ruleItem.Workaround( generator, prefixName );
			}
		}

		public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
		{
			CodeWriter innerCodeWriter = new CodeWriter();

			//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
			innerCodeWriter.AppendLine( "public string " + ruleMethodHeader );
			innerCodeWriter.AppendLine( "{" );
			innerCodeWriter.Indent++;
			innerCodeWriter.AppendLine( "CodeWriter " + stringWriterName + " = new CodeWriter();" );
			foreach (RuleItem ruleItem in ruleItemList)
			{
				ruleItem.Generate( stringWriterName, generator, innerCodeWriter, prefixName );
			}
			innerCodeWriter.AppendLine( "return " + stringWriterName + ".ToString();" );
			innerCodeWriter.Indent--;
			innerCodeWriter.AppendLine( "}" );

			codeWriter.AppendLine( innerCodeWriter.ToString() );
		}

		public override string GenerateCallMethod( CodeGenerator generator, string prefixName )
		{
			Regex regex = new Regex( @"^(?'method'\w+)\((?'params'.*)\)$" );
			Match match = regex.Match( this.ruleMethodHeader );

			if (!match.Success) throw new Exception( "Wrong header data: " + ruleMethodHeader );

			string name = match.Groups["method"].Value;
			string[] paramsArray = match.Groups["params"].Value.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

			List<ParamInfo> paramList = new List<ParamInfo>();
			foreach (string paramData in paramsArray)
			{
				string[] splitParamData = paramData.Split( new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries );
				if (splitParamData.Length != 2) throw new Exception( "Wrong param data: " + paramData );

				string paramType = splitParamData[0];
				string paramName = splitParamData[1];

				paramList.Add( new ParamInfo( paramName, paramType ) );
			}

			return name + "( " + string.Join( ", ", Array.ConvertAll<ParamInfo, string>( paramList.ToArray(), delegate( ParamInfo input )
			                                                                                                  	{
			                                                                                                  		return input.Name;
			                                                                                                  	} ) ) + " )";
		}
	}
}