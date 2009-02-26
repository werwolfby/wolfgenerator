/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 26.01.2009 23:25
 *
 * File: CodeStatement.cs
 * Remarks:
 * 
 *   TODO: Add is start parser parameter to code implicity
 * 
 * History:
 *   26.01.2009 23:25 - Create Wireframe
 *   30.01.2009 20:17 - Add EBNF comment.
 *   15.02.2009 11:22 - Add Value property.
 *   26.02.2009 23:10 - Remove Generate & GenerateJoin methods.
 *
 *******************************************************/

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
	}
}