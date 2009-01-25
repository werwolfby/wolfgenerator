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
 *
 *******************************************************/

using System.Text;

namespace WolfGenerator.Core.AST
{
	public class ApplyStatement : RuleStatement
	{
		private readonly string applyMethod;
		private readonly string parameters;
		private readonly string from;

		public ApplyStatement( string applyMethod, string parameters, string from )
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
			builder.Append( applyMethod ?? "[NULL-VALUE]" );
			builder.Append( "( " );
			builder.Append( parameters ?? "[NULL-VALUE]" );
			builder.Append( " ) " );
			builder.Append( "from " );
			builder.Append( from ?? "[NULL-VALUE]" );

			return builder.ToString();
		}
	}
}