/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 24.04.2012 15:50
 *
 * File: ParserAccessor.cs
 * Remarks:
 * 
 * History:
 *   24.04.2012 15:50 - Create Wireframe
 *
 *******************************************************/

using System.Reflection;
using WolfGenerator.Core.AST;
using WolfGenerator.Core.Parsing;

namespace UnitTest.WolfGenerator.Accessors
{
	public class ParserAccessor
	{
		private readonly Parser parser;
		private readonly System.Type parserType = typeof(Parser);

		public ParserAccessor( string source )
		{
			this.parser = new Parser( source );
			this.parser.InitParse();
		}

		public void Type( out Type type )
		{
			var method = parserType.GetMethod( "Type", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			var args = new object[1];
			method.Invoke( parser, args );
			type = (Type)args[0];
		}
	}
}