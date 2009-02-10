/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:25
 *
 * File: RuleMethodStatement.cs
 * Remarks:
 * 
 * History:
 *   27.01.2009 01:54 - Extracted as super class from RuleMethodStatement
 *   10.02.2009 20:27 - Add fileName parameter to Generate method
 *
 *******************************************************/

using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	public abstract class RuleClassMethodStatement 
	{
		public abstract void Generate( CodeWriter writer, string fileName );
	}
}