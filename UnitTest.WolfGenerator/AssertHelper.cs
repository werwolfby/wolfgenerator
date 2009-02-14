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
			if (expected.GenericParameters.Count > 0)
			{
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
	}
}