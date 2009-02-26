/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 04.02.2009 01:39
 *
 * File: MatchMethodStatement.cs
 * Remarks:
 * 
 * History:
 *   04.02.2009 01:39 - Create Wireframe
 *   04.02.2009 01:45 - Implement Generate method.
 *   04.02.2009 02:12 - Fix: Generate method.
 *   10.02.2009 20:30 - Add support fileName of Generated method
 *   11.02.2009 21:54 - New MatchMethodAttribute code generating.
 *   26.02.2009 23:12 - Remove Generate methods.
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	public class MatchMethodStatement : RuleClassMethodStatement
	{
		private readonly string name;
		private readonly string code;

		public MatchMethodStatement( string name, string code )
		{
			this.name = name;
			this.code = code;
		}

		public string Name
		{
			get { return this.name; }
		}

		public string Code
		{
			get { return this.code; }
		}

		internal RuleMethodStatement RuleMethod { get; set; }
	}
}