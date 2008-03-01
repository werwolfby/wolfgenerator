/*****************************************************
 *
 * Created by: WerWolf
 * Created: 05.05.2007 17:49:44
 *
 * File: RuleParser.cs
 * Remarks:
 * 
 * History:
 *   05.05.2007 17:49 - Create wireframe
 *   05.05.2007 18:19 - Adding basic functionality
 *
 *****************************************************/

using System;
using System.Collections.Generic;

namespace WolfGenerator.Core
{
	public partial class RuleParser
	{
		#region Inner classes
		private abstract class Builder
		{
			protected readonly RuleParser ruleParser;

			protected Builder( RuleParser ruleParser )
			{
				this.ruleParser = ruleParser;
			}

			public abstract void Init();

			public abstract RuleString Build();
		}
		#endregion

		#region Inner constant's
		private string startTagString = "<%";
		private string endTagString = "%>";
		private string startExecuteTagString = "<%=";
		private string endExecuteTagString = "%>";
		#endregion

		#region Fields
		private int codeIndex = 0;
		private string code;

		private string fileName;

		private int lineNumber = 1;
		private int columnNumber = 1;
		#endregion

		#region Builders
		private Builder currentBuilder;

		private TagBuilder tagBuilder;
		private ExecuteRuleBuilder executeBuilder;
		private TextBuilder textBuilder;
		#endregion

		#region Constructors
		public RuleParser( string code, string fileName )
		{
			this.code = code;
			this.fileName = fileName;

			this.tagBuilder     = new TagBuilder( this );
			this.executeBuilder = new ExecuteRuleBuilder( this );
			this.textBuilder    = new TextBuilder( this );

			SelectBuildState();
		}
		#endregion

		private char? CurrentSymbol
		{
			get
			{
				if (codeIndex < code.Length)
				{
					char symbol = this.code[codeIndex];

					if (symbol == '\n')
					{
						lineNumber++;
						columnNumber = 0;
					}

					columnNumber++;

					return symbol;
				}
				else return null;
			}
		}

		#region Methods
		public List<RuleString> Build()
		{
			List<RuleString> ruleNodeList = new List<RuleString>();

			while (CurrentSymbol != null)
			{
				ruleNodeList.Add( this.currentBuilder.Build() );
				SelectBuildState();
			}

			List<RuleString> newRuleStringList = new List<RuleString>();

			foreach (RuleString ruleString in ruleNodeList)
			{
				if (!(ruleString is CommandRuleString) && !(ruleString is ValueRuleString))
				{
					string[] delimiters = new string[] { "\r\n", "\n\r", "\r", "\n" };
					string[] strings = ruleString.Text.Split( delimiters, StringSplitOptions.None );
					int length = strings.Length;
					foreach (string delimiter in delimiters)
					{
						if (ruleString.Text.EndsWith(delimiter))
						{
							length--;
							break;
						}
					}
					for (int i = 0; i < length; i++)
					{
						string line = strings[i];
						if (i == length - 1 && ruleString.Text[ruleString.Text.Length - 1] != '\r' && ruleString.Text[ruleString.Text.Length - 1] != '\n') 
							newRuleStringList.Add( new TextRuleString( line, fileName, lineNumber, columnNumber ) );
						else newRuleStringList.Add( new TextRuleString( line + "\r\n", fileName, lineNumber, columnNumber ) );
					}
				}
				else
				{
					newRuleStringList.Add( ruleString );
				}
			}

			return newRuleStringList;
		}

		private char? GetNextSymbol()
		{
			char? symbol = this.CurrentSymbol;
			codeIndex++;
			return symbol;
		}

		private bool CheckNextSymbols( string text )
		{
			if (codeIndex + text.Length > code.Length) return false;

			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != code[codeIndex + i]) return false;
			}

			return true;
		}

		private void SelectBuildState()
		{
			if (CheckNextSymbols( startExecuteTagString ))
			{
				executeBuilder.Init();
				currentBuilder = executeBuilder;
			}
			else if (CheckNextSymbols( startTagString ))
			{
				tagBuilder.Init();
				currentBuilder = tagBuilder;
			}
			else
			{
				textBuilder.Init();
				currentBuilder = textBuilder;
			}
		}
		#endregion
	}
}