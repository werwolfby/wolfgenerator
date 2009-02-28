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
 *   26.02.2009 23:12 - Remove Generate methods.
 *   28.02.2009 10:03 - Add inheritance from Statement class.
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	public abstract class RuleClassMethodStatement : Statement
	{
		protected RuleClassMethodStatement( StatementPosition statementPosition ) : base( statementPosition ) {}
	}
}