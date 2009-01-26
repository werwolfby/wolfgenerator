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

		public override void Generate( Writer.CodeWriter writer, string innerWriter )
		{
			//throw new System.NotImplementedException();
		}

		public override void GenerateJoin( Writer.CodeWriter writer, string innerWriter )
		{
			writer.AppendLine( "foreach (var item in " + from + ")" );
			writer.AppendLine("{");

			writer.Indent++;

			writer.AppendLine( "temp = (CodeWriter)this.GetType().InvokeMember( \"" + applyMethod + "\"," );
			writer.Indent++;
			writer.AppendLine( "BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public," );
			writer.AppendLine( "Type.DefaultBinder, this, new object[] { " + parameters + " } );" );
			writer.Indent--;

			writer.AppendLine( "list.Add( temp );" );

			writer.Indent--;

			writer.AppendLine("}");
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