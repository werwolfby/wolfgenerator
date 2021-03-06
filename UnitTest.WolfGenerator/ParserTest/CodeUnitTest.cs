/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 15.02.2009 11:19
 *
 * File: CodeUnitTest.cs
 * Remarks:
 * 
 * History:
 *   15.02.2009 11:19 - Create Wireframe
 *   15.02.2009 11:32 - Finish first implementation.
 *   15.02.2009 11:38 - Make statements field public.
 *   21.04.2012 23:21 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using System.Text;
using NUnit.Framework;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class CodeUnitTest
	{
		public static readonly CodeStatement[] statements = new[]
		                                                     {
		                                                     	new CodeStatement( Helper.EmptyPosition, "if (true) return;" ),
		                                                     };

		[Test]
		public void CodeTest()
		{
			MainTest2( statements[0] );
		}

		public static string CodeToString( CodeStatement codeStatement, string spaces, bool isStart )
		{
			var builder = new StringBuilder();

			builder.Append( "<%$" );
			builder.Append( spaces );
			builder.Append( codeStatement.Value );
			builder.Append( spaces );
			if (isStart) builder.Append( "$-%>" );
			else builder.Append( "$%>" );

			return builder.ToString();
		}

		private static void MainTest2( CodeStatement code )
		{
			MainTest( code, " ", false );
			MainTest( code, "\r\n\t", false );
			MainTest( code, " ", true );
			MainTest( code, "\r\n\t", true );
		}

		private static void MainTest( CodeStatement code, string spaces, bool isStart )
		{
			bool actualIsStart;
			var codeStatementText = CodeToString( code, spaces, isStart );
			var actualCodeStatement = ParserHelper.ParseCode( codeStatementText, out actualIsStart );

			AssertHelper.AssertCode( code, actualCodeStatement, isStart, actualIsStart );
		}
	}
}