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
 *
 *******************************************************/

using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Call = "&lt;&call" ident ( ("([" {ANY} "])") | ("(" {ANY} ")") ) "%&gt;".
	/// </summary>
	public class CallStatement : RuleStatement
	{
		private readonly string name;
		private readonly string parameters;

		public CallStatement( string name, string parameters )
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

		public override void Generate( CodeWriter writer, string innerWriter )
		{
			writer.Append( innerWriter );
			writer.AppendLine( ".Append( this.Invoke( \"" + name + "\", " + parameters + " ) );" );
		}

		public override void GenerateJoin( CodeWriter writer, string innerWriter )
		{
			writer.AppendLine( "list.Add( this.Invoke( \"" + name + "\", " + parameters + " ) );" );
		}
	}
}