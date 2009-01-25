/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:27
 *
 * File: TextStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 08:27 - Create Wireframe
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	public class TextStatement : RuleStatement
	{
		private readonly string text;

		public TextStatement( string text )
		{
			this.text = text;
		}

		public string Text
		{
			get { return this.text; }
		}
	}
}