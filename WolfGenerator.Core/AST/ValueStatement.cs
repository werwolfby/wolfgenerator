/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:28
 *
 * File: ValueStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 08:28 - Create Wireframe
 *   25.01.2009 10:55 - Override ToString method.
 *   26.01.2009 00:39 - Implement Generate method.
 *   26.01.2009 10:48 - Implement GenerateJoin method. Use AppendText in Generate method instead of Append.
 *   26.01.2009 11:03 - Fix: GenerateJoin.
 *   30.01.2009 20:10 - Add EBNF comment.
 *   24.02.2009 00:02 - Update EBNF comment.
 *   26.02.2009 23:15 - Remove Generate & GenerateJoin methods.
 *   28.02.2009 10:24 - Add support inheritance from Statement class.
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Value = "&lt;%=" {ANY} "%&gt;".
	/// </summary>
	public class ValueStatement : RuleStatement
	{
		private readonly string value;

		public ValueStatement( StatementPosition position, string value ) : base( position )
		{
			this.value = value;
		}

		public string Value
		{
			get { return this.value; }
		}

		public override string ToString()
		{
			return "<%= " + value + " %>";
		}
	}
}