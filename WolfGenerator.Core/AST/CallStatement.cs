/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 27.01.2009 01:30
 *
 * File: CallStatement.cs
 * Remarks:
 * 
 * History:
 *   27.01.2009 01:30 - Create Wireframe
 *   30.01.2009 20:19 - Add EBNF Comment.
 *   04.02.2009 01:10 - Use `Invoke` method of `GeneratorBase` base class.
 *   26.02.2009 23:10 - Remove Generate & GenerateJoin methods.
 *   28.02.2009 10:26 - Add support inheritance from Statement class.
 *   28.02.2009 12:12 - Override ToString method.
 *   
 *******************************************************/

using System.Text;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Call = "&lt;%call" ident ( ("([" {ANY} "])") | ("(" {ANY} ")") ) "%&gt;".
	/// </summary>
	public class CallStatement : RuleStatement
	{
		private readonly string name;
		private readonly string parameters;

		public CallStatement( StatementPosition position, string name, string parameters ) : base( position )
		{
			this.name = name;
			this.parameters = parameters;
		}

		public string Name
		{
			get { return this.name; }
		}

		public string Parameters
		{
			get { return this.parameters; }
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append( "<%call " );
			builder.Append( Name );
			builder.Append( "(" );
			if (!string.IsNullOrEmpty( Parameters ))
			{
				builder.Append( " " );
				builder.Append( Parameters );
				builder.Append( " " );
			}
			builder.Append( ")" );

			return builder.ToString();
		}
	}
}