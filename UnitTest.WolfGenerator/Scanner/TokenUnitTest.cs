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
 *   21.04.2012 23:06 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using System.IO;
using System.Text;
using NUnit.Framework;
using WolfGenerator.Core.Parsing;
using System.Linq;

namespace UnitTest.WolfGenerator.Scanner
{
	[TestFixture]
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

		[Test]
		public void TextTokenTest()
		{
			var tokens = new[]
			             {
			             	new Token( Parser._rule, "<%rule" ),
			             	null,
			             	new Token( Parser._ident, "Test" ),
			             	new Token( Parser._endStatement, "%>" ),
			             	new Token( Parser._end, "<%end%>" )
			             };
			var text = string.Join( "", tokens.Select( t => t != null ? t.value : " " ).ToArray() );
			var scanner = new global::WolfGenerator.Core.Parsing.Scanner( new MemoryStream( Encoding.UTF8.GetBytes( text ) ) );

			foreach (var expectedTokenValue in tokens.Where( s => s != null ))
			{
				var actualTokenValue = scanner.Scan();
				Assert.That( actualTokenValue.val, Is.EqualTo( expectedTokenValue.value ) );
				if (expectedTokenValue.type.HasValue)
					Assert.That( actualTokenValue.kind, Is.EqualTo( expectedTokenValue.type.Value ),
					             "Wrong kind of parsing token" );
			}
		}
	}
}