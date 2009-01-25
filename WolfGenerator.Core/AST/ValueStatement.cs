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
	}
}