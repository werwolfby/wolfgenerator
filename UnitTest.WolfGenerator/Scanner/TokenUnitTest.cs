/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.02.2009 21:38
 *
 * File: TokenUnitTest.cs
 * Remarks:
 * 
 * History:
 *   25.02.2009 21:38 - Create Wireframe
 *
 *******************************************************/

using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core;
using WolfGenerator.Core.Parsing;
using System.Linq;

namespace UnitTest.WolfGenerator.Scanner
{
	[TestClass]
	public class TokenUnitTest
	{
		private class Token
		{
			public readonly int? type;
			public readonly string value;

			public Token( int? type, string value )
			{
				this.type = type;
				this.value = value;
			}

			public static implicit operator Token( string str )
			{
				return new Token( null, str );
			}
		}

		[TestMethod]
		public void TextTokenTest()
		{
			var tokens = new[]
			             {
			             	new Token( Parser_Accessor._rule, "<%rule" ), 
							null, 
							new Token( Parser_Accessor._ident, "Test" ),
			             	new Token( Parser_Accessor._endStatement, "%>" ), 
							new Token( Parser_Accessor._end, "<%end%>" )
			             };
			var text = string.Join( "", tokens.Select( t => t != null ? t.value : " " ).ToArray() );
			var scanner = new Scanner_Accessor( new MemoryStream( Encoding.UTF8.GetBytes( text ) ) );

			foreach (var expectedTokenValue in tokens.Where( s => s != null ))
			{
				var actualTokenValue = scanner.NextToken();
				Assert.AreEqual( expectedTokenValue.value, actualTokenValue.val );
				if (expectedTokenValue.type.HasValue)
					Assert.AreEqual( expectedTokenValue.type.Value, actualTokenValue.kind,
					                 "Wrong kind of parsing token" );
			}
		}
	}
}