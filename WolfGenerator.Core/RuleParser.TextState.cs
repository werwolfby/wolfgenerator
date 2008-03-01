/*****************************************************
 *
 * Created by: 
 * Created: 11.05.2007 11:00:23
 *
 * File: RuleParser.TextState.cs
 * Remarks:
 *
 *****************************************************/

using System.Text;

namespace WolfGenerator.Core
{
	public partial class RuleParser
	{
		private class TextBuilder : Builder
		{
			private enum State
			{
				Text,
				Finish
			}

			private StringBuilder ruleTextBuilder;
			private StringBuilder prevString;

			private State state;

			public TextBuilder( RuleParser ruleParser ) : base( ruleParser )
			{
			}

			public override void Init()
			{
				this.ruleTextBuilder = new StringBuilder();
				this.prevString = new StringBuilder();
				this.state = State.Text;
			}

			public override RuleString Build()
			{
				while(state != State.Finish)
				{
					char? symbol = this.ruleParser.GetNextSymbol();
					if (symbol.HasValue)
					{
						char c = (char)symbol;
						switch (c)
						{
							case '<':
								prevString.Append( c );
								break;
							case '%':
								if (prevString.Length == 0)
									goto default;

								if (prevString.ToString().EndsWith( "<<" ))
								{
									if (prevString.Length > 2) ruleTextBuilder.Append( prevString.ToString( 0, prevString.Length - 2 ) );

									ruleTextBuilder.Append( '<' );
									ruleTextBuilder.Append( '%' );

									prevString.Remove( 0, prevString.Length );
								}
								else if (prevString.ToString() == "<")
								{
									ruleParser.codeIndex -= 2;

									state = State.Finish;
								}
								break;
							default:
								if (prevString.Length > 0)
								{
									ruleTextBuilder.Append( prevString.ToString() );
									prevString.Remove( 0, prevString.Length );
								}

								ruleTextBuilder.Append( c );
								break;
						}
					}
					else
					{
						state = State.Finish;
					}
				}

				return new TextRuleString( ruleTextBuilder.ToString(), this.ruleParser.fileName,
										   this.ruleParser.lineNumber, this.ruleParser.columnNumber );
			}
		}
	}
}