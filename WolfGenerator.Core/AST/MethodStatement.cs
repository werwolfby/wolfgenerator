/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 27.01.2009 01:50
 *
 * File: MethodStatement.cs
 * Remarks:
 * 
 * History:
 *   27.01.2009 01:50 - Create Wireframe
 *   27.01.2009 01:59 - Inhirit from RuleClassMethodStatement and implement method Generate.
 *
 *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	public class MethodStatement : RuleClassMethodStatement
	{
		private readonly Type returnType;
		private readonly string name;
		private readonly IList<Variable> variables;
		private readonly string code;

		public MethodStatement( Type returnType, string name, IList<Variable> variables, string code )
		{
			this.returnType = returnType;
			this.name = name;
			this.variables = variables;
			this.code = code;
		}

		public Type ReturnType
		{
			get { return this.returnType; }
		}

		public string Name
		{
			get { return this.name; }
		}

		public IList<Variable> Variables
		{
			get { return this.variables; }
		}

		public string Code
		{
			get { return this.code; }
		}

		public override void Generate( CodeWriter writer )
		{
			writer.Append( "public " );
			writer.Append( returnType.ToString() );
			writer.Append( this.name );
			writer.Append( "( " );
			writer.Append( string.Join( ", ", Array.ConvertAll( this.Variables.ToArray(), input => input.ToString() ) ) );
			writer.AppendLine( " )" );

			writer.AppendLine( "{" );
			writer.Indent++;

			writer.AppendText( code );

			writer.Indent--;
			writer.AppendLine( "}" );
		}
	}
}