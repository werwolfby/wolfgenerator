/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 14.02.2009 11:51
 *
 * File: AssertHelper.cs
 * Remarks:
 * 
 * History:
 *   14.02.2009 11:51 - Create Wireframe
 *   14.02.2009 12:39 - Change type of method AssertType to int, to support Func<> syntax.
 *   14.02.2009 22:05 - Add AssertVariable. Add in AssertType check for null GenericParameters.
 *
 *******************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator
{
	public class AssertHelper
	{
		public static int AssertType( Type expected, Type actual )
		{
			Assert.AreEqual( expected.TypeName, actual.TypeName, "Wrong name of types" );
			Assert.IsNotNull( expected.GenericParameters, "GenericParameters can't be null" );
			if (expected.GenericParameters.Count > 0)
			{
				Assert.IsNotNull( actual.GenericParameters, "GenericParameters can't be null" );
				Assert.AreEqual( expected.GenericParameters.Count, actual.GenericParameters.Count,
				                 "Wrong count of generic parameter" );
				for (var i = 0; i < expected.GenericParameters.Count; i++)
				{
					var expectedGenericParameter = expected.GenericParameters[i];
					var actualGenericParameter = actual.GenericParameters[i];

					AssertType( expectedGenericParameter, actualGenericParameter );
				}
			}
			else
			{
				Assert.AreEqual( 0, expected.GenericParameters.Count, "Expected type contains generic parameters" );
			}

			return 0;
		}

		public static int AssertVariable( Variable expected, Variable actual )
		{
			Assert.AreEqual( expected.Name, actual.Name,
			                 string.Format( "expected name ('{0}') don't match actual name ('{1}')", expected.Name, actual.Name ) );
			AssertType( expected.Type, actual.Type );

			return 0;
		}
	}
}