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

		public ValueStatement( string value )
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

		public override void Generate( Writer.CodeWriter writer, string innerWriter )
		{
			writer.Append( innerWriter );
			writer.Append( ".AppendText( " );
			writer.Append( value );
			writer.AppendLine( " );" );
		}

		public override void GenerateJoin( Writer.CodeWriter writer, string innerWriter )
		{
			writer.AppendLine( "temp = new CodeWriter();" );
			writer.Append( "temp.AppendText( " );
			writer.Append( value );
			writer.AppendLine( " );" );

			writer.AppendLine( "list.Add( temp );" );
		}
	}
}