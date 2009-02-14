/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 14.02.2009 11:50
 *
 * File: TypeUnitTest.cs
 * Remarks:
 * 
 * History:
 *   14.02.2009 11:50 - Create Wireframe
 *   14.02.2009 12:49 - First full type test implementation
 *   14.02.2009 13:17 - Add ToString method test
 *
 *******************************************************/

using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Type=WolfGenerator.Core.AST.Type;
using System.Linq;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class TypeUnitTest
	{
		private readonly Type dictionaryType;
		private readonly Type listType;
		private readonly Type simpleType;
		private readonly Type funcType;

		public TypeUnitTest()
		{
			this.simpleType     = new Type( "int", null );
			this.listType       = new Type( "List", this.simpleType );
			this.dictionaryType = new Type( "Dictionary", new Type( "string", null ), listType );
			this.funcType       = new Type( "Func", listType, dictionaryType, listType, simpleType );
		}

		[TestMethod]
		public void SimpleTypeTest()
		{
			MainTestMethod2( simpleType );
		}

		[TestMethod]
		public void SimpleTypeToStringTest()
		{
			var str = TypeToString( simpleType, ",", "<", ">" );
			Assert.AreEqual( str, ParserHelper.ParseType( str ).ToString() );
		}

		[TestMethod]
		public void ListTypeTest()
		{
			MainTestMethod2( listType );
		}

		[TestMethod]
		public void ListTypeToStringTest()
		{
			var str = TypeToString( listType, ",", "<", ">" );
			Assert.AreEqual( str, ParserHelper.ParseType( str ).ToString() );
		}

		[TestMethod]
		public void DictionaryTypeTest()
		{
			MainTestMethod2( dictionaryType );
		}

		[TestMethod]
		public void DistionaryTypeToStringTest()
		{
			var str = TypeToString( dictionaryType, ",", "<", ">" );
			Assert.AreEqual( str, ParserHelper.ParseType( str ).ToString() );
		}

		[TestMethod]
		public void FuncTypeTest()
		{
			MainTestMethod2( funcType );
		}

		[TestMethod]
		public void FuncTypeToStringTest()
		{
			var str = TypeToString( funcType, ",", "<", ">" );
			Assert.AreEqual( str, ParserHelper.ParseType( str ).ToString() );
		}

		public string TypeToString( Type t, string ag, string sg, string eg )
		{
			var builder = new StringBuilder();
			builder.Append( t.TypeName );
			if (t.GenericParameters.Count > 0)
			{
				builder.Append( sg );
				// ReSharper disable AccessToModifiedClosure
				// ReSharper disable PossibleNullReferenceException
				builder.Append( t.GenericParameters.Select( gt => TypeToString( gt, ag, sg, eg ) ).Aggregate( ( s1, s2 ) => s1 + ag + s2 ) );
				// ReSharper restore PossibleNullReferenceException
				// ReSharper restore AccessToModifiedClosure
				builder.Append( eg );
			}
			return builder.ToString();
		}

		private void MainTestMethod( Type expectedType, string ag, string sg, string eg, Func<Type, Type, int> tf )
		{
			var typeStatementText = this.TypeToString( expectedType, ag, sg, eg );

			var actualType = ParserHelper.ParseType( typeStatementText );

			tf( expectedType, actualType );
		}

		private void MainTestMethod2( Type type )
		{
			Func<string, string, string, int> func = ( ag, sg, eg ) =>
			                                         {
			                                         	this.MainTestMethod( type, ag, sg, eg, AssertHelper.AssertType );
			                                         	return 0;
			                                         };

			func( ", ", "<", ">" );
			func( ",\r\n\t", "<\r\n", "\r\n>" );
		}
	}
}