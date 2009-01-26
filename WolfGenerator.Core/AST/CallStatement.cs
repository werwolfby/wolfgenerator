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
 *
 *******************************************************/

using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
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
			writer.AppendLine( ".Append( (CodeWriter)this.GetType().InvokeMember( \"" + name + "\"," );
			writer.Indent++;
			writer.AppendLine( "BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public," );
			writer.AppendLine( "Type.DefaultBinder, this, new object[] { " + parameters + " } ) );" );
			writer.Indent--;
		}

		public override void GenerateJoin( CodeWriter writer, string innerWriter )
		{
			writer.AppendLine( "list.Add( (CodeWriter)this.GetType().InvokeMember( \"" + name + "\"," );
			writer.Indent++;
			writer.AppendLine( "BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public," );
			writer.AppendLine( "Type.DefaultBinder, this, new object[] { " + parameters + " } ) );" );
			writer.Indent--;
		}
	}
}