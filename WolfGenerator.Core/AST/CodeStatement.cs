/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 26.01.2009 23:25
 *
 * File: CodeStatement.cs
 * Remarks:
 * 
 * History:
 *   26.01.2009 23:25 - Create Wireframe
 *
 *******************************************************/

using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	public class CodeStatement : RuleStatement
	{
		private readonly string value;

		public CodeStatement( string value )
		{
			this.value = value;
		}

		public override void Generate( CodeWriter writer, string innerWriter )
		{
			writer.AppendText( value + "\r\n" );
		}

		public override void GenerateJoin( CodeWriter writer, string innerWriter )
		{
			throw new System.NotSupportedException();
		}
	}
}