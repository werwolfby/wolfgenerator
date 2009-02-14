/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 14.02.2009 12:52
 *
 * File: ParserHelper.cs
 * Remarks:
 * 
 * History:
 *   14.02.2009 12:52 - Create Wireframe
 *   14.02.2009 21:53 - Add ParseVariable method.
 *
 *******************************************************/

using WolfGenerator.Core;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator
{
	public class ParserHelper
	{
		public static Type ParseType( string statement )
		{
			Type type;
			var parser = new Parser_Accessor( statement );

			parser.InitParse();
			parser.Type( out type );
			return type;
		}

		public static Variable ParseVariable( string statement )
		{
			Variable var;
			var parser = new Parser_Accessor( statement );

			parser.InitParse();
			parser.Var( out var );

			return var;
		}
	}
}