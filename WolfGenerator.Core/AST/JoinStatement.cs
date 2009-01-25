/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 11:30
 *
 * File: JoinStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 11:30 - Create Wireframe
 *   25.01.2009 11:33 - Add String property.
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	public class JoinStatement : RuleStatement
	{
		private readonly string @string;

		public JoinStatement( string @string )
		{
			this.@string = @string;
		}

		public string String
		{
			get { return this.@string; }
		}

		public override string ToString()
		{
			return "<%join \"" + @string + "\"%><%end%>";
		}
	}
}