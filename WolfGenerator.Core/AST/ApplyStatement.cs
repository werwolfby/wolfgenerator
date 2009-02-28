/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 12:08
 *
 * File: ApplyStatement.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 12:08 - Create Wireframe
 *   26.01.2009 00:38 - Add Generate method without implementation.
 *   26.01.2009 10:46 - Implement GenerateJoin method.
 *   26.01.2009 10:56 - Work on GeneateJoin method.
 *   04.02.2009 01:13 - Use `Invoke` method of `GeneratorBase` base class.
 *   26.02.2009 23:10 - Remove Generate & GenerateJoin methods.
 *   28.02.2009 10:24 - Add support inheritance from Statement class.
 *
 *******************************************************/

using System.Text;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Apply = "&lt;%apply" ident (("(["{ ANY }"])"|"("{ ANY }")"))"from" ident "%&gt;".
	/// </summary>
	public class ApplyStatement : RuleStatement
	{
		private readonly string applyMethod;
		private readonly string parameters;
		private readonly string from;

		public ApplyStatement( StatementPosition position, string applyMethod, string parameters, string from )
			: base( position )
		{
			this.applyMethod = applyMethod;
			this.parameters = parameters;
			this.from = from;
		}

		public string ApplyMethod
		{
			get { return this.applyMethod; }
		}

		public string Parameters
		{
			get { return this.parameters; }
		}

		public string From
		{
			get { return this.from; }
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append( "<%apply " );
			builder.Append( this.applyMethod ?? "[NULL-VALUE]" );
			builder.Append( "( " );
			builder.Append( this.parameters ?? "[NULL-VALUE]" );
			builder.Append( " ) " );
			builder.Append( "from " );
			builder.Append( this.from ?? "[NULL-VALUE]" );

			return builder.ToString();
		}
	}
}