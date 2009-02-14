/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 14.02.2009 21:52
 *
 * File: VariableUnitTest.cs
 * Remarks:
 * 
 * History:
 *   14.02.2009 21:52 - Create Wireframe
 *   14.02.2009 22:09 - Finish first implementation.
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class VariableUnitTest
	{
		public static readonly Variable simpleVariable     = new Variable( "simple", TypeUnitTest.simpleType );
		public static readonly Variable listVariable       = new Variable( "simple", TypeUnitTest.listType );
		public static readonly Variable dictionaryVariable = new Variable( "simple", TypeUnitTest.dictionaryType );
		public static readonly Variable funcVariable       = new Variable( "simple", TypeUnitTest.funcType );

		[TestMethod]
		public void SimpleVariableTest()
		{
			MainParseTest( simpleVariable );
		}

		[TestMethod]
		public void ListVariableTest()
		{
			MainParseTest( listVariable );
		}

		[TestMethod]
		public void DictionaryVariableTest()
		{
			MainParseTest( dictionaryVariable );
		}

		[TestMethod]
		public void FuncVariableTest()
		{
			MainParseTest( funcVariable );
		}

		public static string VariableToString( Variable variable, string ag, string sg, string eg )
		{
			return TypeUnitTest.TypeToString( variable.Type, ag, sg, eg ) + ' ' + variable.Name;
		}

		private static void MainParseTest( Variable variable ) 
		{
			MainTestMethod( variable, ",", "<", ">" );
			MainTestMethod( variable, ", ", "< ", " >" );
			MainTestMethod( variable, ",\r\n\t", "<\r\n\t", "\r\n\t>" );
		}

		private static void MainTestMethod( Variable variable, string ag, string sg, string eg )
		{
			var actualVariable = ParserHelper.ParseVariable( VariableToString( variable, ag, sg, eg ) );
			AssertHelper.AssertVariable( variable, actualVariable );
		}
	}
}