/*****************************************************
 *
 * Created by: 
 * Created: 18.05.2007 11:36:12
 *
 * File: JoinRuleAnalizer.cs
 * Remarks:
 * 
 * <%join "<string>"%>
 * <%end join%>
 *
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core
{
	[RuleAnalizer( "join" )]
	public class JoinRuleAnalizer : RuleAnalizer
	{
		public override RuleNode Analize( List<RuleString> ruleStrings, ref int index )
		{
			RuleString joinRuleString = ruleStrings[index++];

			int nextIndex = "join".Length;

			bool emptyLine = false;

			if (joinRuleString.Text[nextIndex] == '+')
			{
				nextIndex++;
				emptyLine = true;
			}

			string joinParams = joinRuleString.Text.Substring( nextIndex + 1 ).Trim();

			StringBuilder joinString = new StringBuilder();
			joinString.Append( '"' );
			int i = 1;
			for (; i < joinParams.Length; i++)
			{
				char c = joinParams[i];

				if (c == '"') break;

				joinString.Append( c );

				if (c == '\\')
				{
					//joinString.Append( c );
					joinString.Append( joinParams[++i] );
				}
			}
			joinString.Append( '"' );

			string joinMethodParams = joinParams.Substring( i + 1 );

			List<JoinRuleNode.RuleItem> innerRuleNodeList = new List<JoinRuleNode.RuleItem>();

			for (; index < ruleStrings.Count; index++)
			{
				RuleString ruleString = ruleStrings[index];

				if (ruleString is CommandRuleString)
				{
					if (ruleString.Text.StartsWith( "end join" )) break;
					else
					{
						RuleAnalizer ruleAnalizer = FindAnAnalizer( ruleString );
						RuleNode ruleNode = ruleAnalizer.Analize( ruleStrings, ref index );

						index--;

						if (ruleNode is IStringArray) innerRuleNodeList.Add( new JoinRuleNode.StringArrayRuleItem( ruleNode ) );
						else innerRuleNodeList.Add( new JoinRuleNode.CommandRuleItem( ruleNode ) );
					}
				}
				else if (ruleString is ValueRuleString)
				{
					innerRuleNodeList.Add( new JoinRuleNode.ValueRuleItem( ruleString.Text ) );
				}
			}

			index++;

			if (ruleStrings[index] is TextRuleString && ruleStrings[index].Text == "\r\n") index++;

			return
				new JoinRuleNode( joinRuleString.FileName, joinRuleString.LineNumber, joinRuleString.ColumNumber, joinString.ToString(), joinMethodParams,
				                  innerRuleNodeList, emptyLine );
		}
	}

	public class JoinRuleNode : RuleNode
	{
		private const string joinRuleSessionName = "JoinRule";

		#region Inner Classes
		private class JoinRuleNodeSession
		{
			private readonly Dictionary<JoinRuleNode, string> applyMethods = new Dictionary<JoinRuleNode, string>();

			public void Add( JoinRuleNode joinRuleNode, string generateMethodName )
			{
				this.applyMethods.Add( joinRuleNode, generateMethodName );
			}

			public int Count
			{
				get
				{
					return this.applyMethods.Count;
				}
			}

			public string this[JoinRuleNode joinRuleNode]
			{
				get
				{
					return this.applyMethods[joinRuleNode];
				}
			}
		}

		public abstract class RuleItem
		{
			public abstract void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName );

			public abstract void Workaround( CodeGenerator generator, string prefixName );
		}

		public class StringArrayRuleItem : RuleItem
		{
			private readonly RuleNode ruleNode;
			private readonly IStringArray stringArray;

			public StringArrayRuleItem( RuleNode ruleNode )
			{
				this.stringArray = (IStringArray)ruleNode;
				this.ruleNode = ruleNode;
			}

			public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
			{
				codeWriter.AppendLine( string.Format( "{0}.AddRange( " + stringArray.GetStringArrayMethodName( generator ) + " );",
				                                      stringWriterName ) );
			}

			public override void Workaround( CodeGenerator generator, string prefixName )
			{
				ruleNode.Workaround( generator, prefixName );
			}
		}

		public class CommandRuleItem : RuleItem
		{
			private readonly RuleNode ruleNode;

			public CommandRuleItem( RuleNode ruleNode )
			{
				this.ruleNode = ruleNode;
			}

			public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
			{
				codeWriter.AppendLine( "{" );
				codeWriter.Indent++;
				codeWriter.AppendLine( string.Format( "string value = {0};", this.ruleNode.GenerateCallMethod( generator, prefixName ) ) );
				codeWriter.AppendLine( string.Format( "if (!string.IsNullOrEmpty( value )) {0}.Add( value );", stringWriterName ) );
				codeWriter.Indent--;
				codeWriter.AppendLine( "}" );
			}

			public override void Workaround( CodeGenerator generator, string prefixName )
			{
				ruleNode.Workaround( generator, prefixName );
			}
		}

		public class ValueRuleItem : RuleItem
		{
			private readonly string text;

			public ValueRuleItem( string text )
			{
				this.text = text;
			}

			public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
			{
				codeWriter.AppendLine( "{" );
				codeWriter.Indent++;
				codeWriter.AppendLine( string.Format( "string value = {0};", text ) );
				codeWriter.AppendLine( string.Format( "if (!string.IsNullOrEmpty( value )) {0}.Add( value );", stringWriterName ) );
				codeWriter.Indent--;
				codeWriter.AppendLine( "}" );
			}

			public override void Workaround( CodeGenerator generator, string prefixName )
			{
			}
		}
		#endregion

		#region Fields
		private string joinString;
		private string joinParams;
		private List<RuleItem> ruleItemList;
		private bool emptyLine;
		#endregion

		#region Constructors
		public JoinRuleNode( string fileName, int lineNumber, int columnNumber, string joinString, string joinParams, List<RuleItem> ruleItemList, bool emptyLine ) : base( fileName, lineNumber, columnNumber )
		{
			this.joinString = joinString;
			this.joinParams = joinParams;
			this.ruleItemList = ruleItemList;
			this.emptyLine = emptyLine;
		}
		#endregion

		#region Methods
		public override void Workaround( CodeGenerator generator, string prefixName )
		{
			foreach (RuleItem ruleItem in ruleItemList)
			{
				ruleItem.Workaround( generator, prefixName );
			}

			JoinRuleNodeSession joinRuleNodeSession;
			object joinRuleNodeObject;
			if (!generator.Session.TryGetValue( joinRuleSessionName, out joinRuleNodeObject ))
			{
				joinRuleNodeSession = new JoinRuleNodeSession();
				generator.Session[joinRuleSessionName] = joinRuleNodeSession;
			}
			else
			{
				joinRuleNodeSession = (JoinRuleNodeSession)joinRuleNodeObject;
			}

			CodeWriter codeWriter = new CodeWriter();

			string innerJoinMethodName = prefixName + "JoinMethod" + joinRuleNodeSession.Count;

			//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
			codeWriter.AppendLine( "public List<string> " + innerJoinMethodName + "( " + joinParams + " )" );
			codeWriter.AppendLine( "{" );
			codeWriter.Indent++;
			codeWriter.AppendLine( "List<string> strings = new List<string>();" );
			foreach (RuleItem ruleItem in ruleItemList)
			{
				ruleItem.Generate( "strings", generator, codeWriter, prefixName );
			}
			codeWriter.AppendLine( "return strings;" );
			codeWriter.Indent--;
			codeWriter.AppendLine( "}" );

			generator.AddCode( "Join Methods", codeWriter.ToString() );

			joinRuleNodeSession.Add( this, innerJoinMethodName );
		}

		public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
		{
			codeWriter.AppendLine( "{" );
			codeWriter.Indent++;
			if (emptyLine)
			{
				//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
				codeWriter.AppendLine( string.Format( "{0}.AppendLine( {1} );", stringWriterName, GenerateCallMethod( generator, prefixName ) ) );
			}
			else
			{
				//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
				codeWriter.AppendLine( string.Format( "string value = {0};", GenerateCallMethod( generator, prefixName ) ) );
				codeWriter.AppendLine( string.Format( @"if (value.EndsWith( ""\r\n"" )) {0}.AppendLine( value );", stringWriterName ) );
				codeWriter.AppendLine( string.Format( @"else {0}.Append( value );", stringWriterName ) );
			}
			//codeWriter.AppendLine( "" );
			//codeWriter.AppendLine( string.Format( "{0}.AppendLine( string.Join( {1}, {2}.ToArray() ) );", stringWriterName, joinString,
			//    GetStringArrayMethodName( generator ) ) );
			codeWriter.Indent--;
			codeWriter.AppendLine( "}" );
		}

		public string GetStringArrayMethodName( CodeGenerator generator )
		{
			JoinRuleNodeSession joinRuleNodeSession;
			object joinRuleNodeSessionObject;
			if (!generator.Session.TryGetValue( joinRuleSessionName, out joinRuleNodeSessionObject )) throw new Exception( "Can't get need session" );
			joinRuleNodeSession = (JoinRuleNodeSession)joinRuleNodeSessionObject;

			string[] joinParamArray = this.joinParams.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
			string[] paramNameArray = new string[joinParamArray.Length];

			for (int i = 0; i < joinParamArray.Length; i++)
			{
				string param = joinParamArray[i].Trim();

				string[] paramValue = param.Split( ' ' );

				paramNameArray[i] = paramValue[1].Trim();
			}

			return joinRuleNodeSession[this] + "( " + string.Join( ", ", paramNameArray ) + " )";
		}

		public override string GenerateCallMethod( CodeGenerator generator, string prefixName )
		{
			return "string.Join( " + joinString + ", " + GetStringArrayMethodName( generator ) + ".ToArray() )";
		}
		#endregion
	}
}