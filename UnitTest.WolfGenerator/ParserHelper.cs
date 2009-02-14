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
			Type actualType;
			var parser = new Parser_Accessor( statement );

			parser.InitParse();
			parser.Type( out actualType );
			return actualType;
		}
	}
}