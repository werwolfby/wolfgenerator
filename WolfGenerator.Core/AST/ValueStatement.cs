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
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
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
			writer.Append( ".Append( " );
			writer.Append( value );
			writer.AppendLine( " );" );
		}
	}
}