/*****************************************************
 *
 * Created by: 
 * Created: 11.05.2007 10:46:42
 *
 * File: CommandRuleString.cs
 * Remarks:
 *
 *****************************************************/

namespace WolfGenerator.Core
{
	public class CommandRuleString : RuleString
	{
		public CommandRuleString( string text, string fileName, int lineNumber, int columNumber )
			: base( text, fileName, lineNumber, columNumber )
		{
		}
	}
}