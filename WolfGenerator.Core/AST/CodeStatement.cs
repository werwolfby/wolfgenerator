/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 26.01.2009 23:25
 *
 * File: CodeStatement.cs
 * Remarks:
 * 
 *   TODO: Add is start parse parameter to code implicity
 * 
 * History:
 *   26.01.2009 23:25 - Create Wireframe
 *   30.01.2009 20:17 - Add EBNF comment.
 *   15.02.2009 11:22 - Add Value property.
 *
 *******************************************************/

using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Code = "&lt;%$" {ANY} ("$%&gt;" | "$-%&gt;").
	/// </summary>
	public class CodeStatement : RuleStatement
	{
		private readonly string value;

		public CodeStatement( string value )
		{
			this.value = value;
		}

		public string Value
		{
			get { return this.value; }
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