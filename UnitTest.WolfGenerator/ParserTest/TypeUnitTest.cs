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
 *
 *******************************************************/

using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core;
using Type=WolfGenerator.Core.AST.Type;
using System.Linq;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class TypeUnitTest
	{
		[TestMethod]
		public void TypeTest()
		{
			var expectedType = new Type( "List", new[]
			                                     {
			                                     	new Type( "int", null ),
			                                     } );
			Type actualType;

			Func<Type, string> func = null;
			func = t =>
			       {
			       	var builder = new StringBuilder();
			       	builder.Append( t.TypeName );
			       	if (t.GenericParameters.Count > 0)
			       	{
			       		builder.Append( '<' );
						// ReSharper disable AccessToModifiedClosure
			       		builder.Append( t.GenericParameters.Select( gt => func( gt ) ).Aggregate( ( s1, s2 ) => s1 + ", " + s2 ) );
						// ReSharper restore AccessToModifiedClosure
			       		builder.Append( '>' );
			       	}
			       	return builder.ToString();
			       };

			var typeStatementText = func( expectedType );

			var parser = new Parser_Accessor( typeStatementText );
			parser.InitParse();
			parser.Type( out actualType );

			AssertHelper.AssertType( expectedType, actualType );
		}
	}
}