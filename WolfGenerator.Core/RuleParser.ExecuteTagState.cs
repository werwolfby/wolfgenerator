/*****************************************************
 *
 * Created by: WerWolf
 * Created: 11.05.2007 10:51:01
 *
 * File: RuleParser.ExecuteTagState.cs
 * Remarks:
 *
 *****************************************************/

using System;
using System.Text;

namespace WolfGenerator.Core
{
	public partial class RuleParser
	{
		private class ExecuteRuleBuilder : Builder
		{
			private enum State
			{
				Text,
				String,
				Finish
			}

			private StringBuilder ruleTextBuilder;
			private State state = State.Text;

			public ExecuteRuleBuilder( RuleParser ruleParser )
				: base( ruleParser )
			{
			}

			public override void Init()
			{
				this.ruleTextBuilder = new StringBuilder();
				this.state = State.Text;
			}

			public override RuleString Build()
			{
				ruleParser.GetNextSymbol();
				ruleParser.GetNextSymbol();
				ruleParser.GetNextSymbol();

				while (state != State.Finish)
				{
					char? symbol = ruleParser.GetNextSymbol();
					if (symbol != null)
					{
						switch (state)
						{
							case State.Text:
								BuildText( (char)symbol );
								break;
							case State.String:
								BuildString( (char)symbol );
								break;
						}
						ruleTextBuilder.Append( (char)symbol );
					}
					else
					{
						throw new Exception( "Unexpected end of code" );
					}
				}

				return new ValueRuleString( ruleTextBuilder.ToString( 0, ruleTextBuilder.Length - 2 ).Trim(), this.ruleParser.fileName,
										   this.ruleParser.lineNumber, this.ruleParser.columnNumber );
			}

			private bool escapeSymbol = false;

			private void BuildString( char symbol )
			{
				if (!escapeSymbol && symbol == '"')
				{
					state = State.Text;
				}

				if (symbol == '\\')
				{
					escapeSymbol = true;
				}
				else
				{
					escapeSymbol = false;
				}
			}

			private bool endTagFirstSymbol = false;

			private void BuildText( char symbol )
			{
				if (endTagFirstSymbol && symbol == '>')
				{
					state = State.Finish;
					return;
				}

				if (symbol == '"')
				{
					state = State.String;
				}

				if (symbol == '%') endTagFirstSymbol = true;
				else endTagFirstSymbol = false;
			}
		}
	}
}