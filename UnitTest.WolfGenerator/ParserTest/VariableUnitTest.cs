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
 *   25.02.2009 20:14 - Finish ReserveVariableTest test and fix for it success.
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class VariableUnitTest
	{
		public static readonly Variable simpleVariable     = new Variable( Helper.EmptyPosition, "simple", TypeUnitTest.simpleType );
		public static readonly Variable listVariable       = new Variable( Helper.EmptyPosition, "simple", TypeUnitTest.listType );
		public static readonly Variable dictionaryVariable = new Variable( Helper.EmptyPosition, "simple", TypeUnitTest.dictionaryType );
		public static readonly Variable funcVariable       = new Variable( Helper.EmptyPosition, "simple", TypeUnitTest.funcType );
		public static readonly Variable reservedVariable   = new Variable( Helper.EmptyPosition, "@class", TypeUnitTest.funcType );

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

		[TestMethod]
		public void ReserveVariableTest()
		{
			MainParseTest( reservedVariable );
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
			var expectedString = VariableToString( variable, ag, sg, eg );
			var actualVariable = ParserHelper.ParseVariable( expectedString );
			AssertHelper.AssertVariable( variable, actualVariable );
		}
	}
}