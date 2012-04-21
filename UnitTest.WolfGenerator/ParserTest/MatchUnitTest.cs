/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 15.02.2009 14:02
 *
 * File: MatchUnitTest.cs
 * Remarks:
 * 
 * History:
 *   15.02.2009 14:02 - Create Wireframe
 *   21.04.2012 23:35 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using System.Text;
using NUnit.Framework;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class MatchUnitTest
	{
		public static readonly MatchMethodStatement[] statements = new[]
		                                                           {
		                                                           	new MatchMethodStatement( Helper.EmptyPosition, "IsId", "return field == \"Id\"" )
		                                                           };

		[Test]
		public void MatchTest()
		{
			MainTest2( statements[0] );
		}

		public static string MatchToString( MatchMethodStatement codeStatement, string spaces )
		{
			var builder = new StringBuilder();

			builder.Append( "<%match" );
			builder.Append( spaces );
			builder.Append( codeStatement.Name );
			builder.Append( "%>" );
			builder.Append( spaces );
			builder.Append( codeStatement.Code );
			builder.Append( spaces );
			builder.Append( "<%end%>" );

			return builder.ToString();
		}

		private static void MainTest2( MatchMethodStatement code )
		{
			MainTest( code, " " );
			MainTest( code, "\r\n\t" );
			MainTest( code, " " );
			MainTest( code, "\r\n\t" );
		}

		private static void MainTest( MatchMethodStatement code, string spaces )
		{
			var codeStatementText = MatchToString( code, spaces );
			var actualCodeStatement = ParserHelper.ParseMatch( codeStatementText );

			AssertHelper.AssertMatch( code, actualCodeStatement );
		}
	}
}