/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 08:59
 *
 * File: Variable.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 08:59 - Create Wireframe
 *   25.01.2009 09:02 - Override ToStringMethod.
 *   25.01.2009 09:06 - Fix: ToString - return <Type> <Name>
 *   25.02.2009 20:17 - Update EBNF
 *   28.02.2009 10:07 - Add inheritance from Statement class.
 *
 *******************************************************/

namespace WolfGenerator.Core.AST
{
	/// <summary>
	/// Var = Type ident.
	/// ident = ['@'] letter { letter | digit }.
	/// </summary>
	public class Variable : Statement
	{
		private readonly string name;
		private readonly Type type;

		public Variable( StatementPosition position, string name, Type type ) : base( position )
		{
			this.name = name;
			this.type = type;
		}

		public string Name
		{
			get { return this.name; }
		}

		public Type Type
		{
			get { return this.type; }
		}

		public override string ToString()
		{
			return this.type + " " + this.name;
		}
	}
}