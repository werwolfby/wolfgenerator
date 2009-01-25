/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:26
 *
 * File: RuleStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 08:26 - Create Wireframe
 *   26.01.2009 00:25 - Add Generate method.
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	public abstract class RuleStatement
	{
		public abstract void Generate( Writer.CodeWriter writer, string innerWriter );
	}
}