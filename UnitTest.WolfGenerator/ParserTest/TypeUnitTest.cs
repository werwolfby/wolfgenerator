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
 *   14.02.2009 21:57 - Made inner variable static and public.
 *   14.02.2009 22:01 - Made TypeToString, MainTestMethod, MainTestMethod2 static.
 *   21.04.2012 23:34 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using System;
using System.Text;
using NUnit.Framework;
using Type=WolfGenerator.Core.AST.Type;
using System.Linq;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class TypeUnitTest
	{
		public static readonly Type dictionaryType;
		public static readonly Type listType;
		public static readonly Type simpleType;
		public static readonly Type funcType;

		static TypeUnitTest()
		{
			simpleType     = new Type( Helper.EmptyPosition, "int", null );
			listType       = new Type( Helper.EmptyPosition, "List", simpleType );
			dictionaryType = new Type( Helper.EmptyPosition, "Dictionary", new Type( Helper.EmptyPosition, "string", null ), listType );
			funcType       = new Type( Helper.EmptyPosition, "Func", listType, dictionaryType, listType, simpleType );
		}

		[Test]
		public void SimpleTypeTest()
		{
			MainTestMethod2( simpleType );
		}

		[Test]
		public void SimpleTypeToStringTest()
		{
			var str = TypeToString( simpleType, ",", "<", ">" );
			Assert.That( ParserHelper.ParseType( str ).ToString(), Is.EqualTo( str ) );
		}

		[Test]
		public void ListTypeTest()
		{
			MainTestMethod2( listType );
		}

		[Test]
		public void ListTypeToStringTest()
		{
			var str = TypeToString( listType, ",", "<", ">" );
			Assert.That( ParserHelper.ParseType( str ).ToString(), Is.EqualTo( str ) );
		}

		[Test]
		public void DictionaryTypeTest()
		{
			MainTestMethod2( dictionaryType );
		}

		[Test]
		public void DistionaryTypeToStringTest()
		{
			var str = TypeToString( dictionaryType, ",", "<", ">" );
			Assert.That( ParserHelper.ParseType( str ).ToString(), Is.EqualTo( str ) );
		}

		[Test]
		public void FuncTypeTest()
		{
			MainTestMethod2( funcType );
		}

		[Test]
		public void FuncTypeToStringTest()
		{
			var str = TypeToString( funcType, ",", "<", ">" );
			Assert.That( ParserHelper.ParseType( str ).ToString(), Is.EqualTo( str ) );
		}

		public static string TypeToString( Type t, string ag, string sg, string eg )
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

		private static void MainTestMethod( Type expectedType, string ag, string sg, string eg, Func<Type, Type, int> tf )
		{
			var typeStatementText = TypeToString( expectedType, ag, sg, eg );

			var actualType = ParserHelper.ParseType( typeStatementText );

			tf( expectedType, actualType );
		}

		private static void MainTestMethod2( Type type )
		{
			Func<string, string, string, int> func = ( ag, sg, eg ) =>
			                                         {
			                                         	MainTestMethod( type, ag, sg, eg, AssertHelper.AssertType );
			                                         	return 0;
			                                         };

			func( ", ", "<", ">" );
			func( ",\r\n\t", "<\r\n", "\r\n>" );
		}
	}
}