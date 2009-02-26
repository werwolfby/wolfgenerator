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
 *   27.01.2009 02:15 - Fix: Generate - add space between return type and name.
 *   30.01.2009 20:14 - Add EBNF comment.
 *   26.02.2009 23:12 - Remove Generate methods.
 *
 *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// EBNF: Method = "&lt;%method" Type ident "(" [Var {"," Var}] ")" "%&gt;" {ANY} "&lt;end&gt;".
	/// </summary>
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
	}
}