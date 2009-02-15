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
 *
 *******************************************************/

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestClass]
	public class MatchUnitTest
	{
		public static readonly MatchMethodStatement[] statements = new[]
		                                                           {
		                                                           	new MatchMethodStatement( "IsId", "return field == \"Id\"" )
		                                                           };

		[TestMethod]
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