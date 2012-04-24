/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 15.02.2009 16:25
 *
 * File: UsingUnitTest.cs
 * Remarks:
 * 
 * History:
 *   15.02.2009 16:25 - Create Wireframe
 *   21.04.2012 23:37 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using NUnit.Framework;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class UsingUnitTest
	{
		public static readonly UsingStatement[] statements = new[]
		                                                     {
		                                                     	new UsingStatement( Helper.EmptyPosition, "System" ),
		                                                     	new UsingStatement( Helper.EmptyPosition, "System.Collection" ),
		                                                     	new UsingStatement( Helper.EmptyPosition, "System.Collection.Generic" ),
		                                                     };

		[Test]
		public void SystemTest()
		{
			foreach (var statement in statements)
				MainTest2( statement );
		}

		public static string UsingToString( UsingStatement statement, string spaces )
		{
			return "<%using" + spaces + statement.Namespace + spaces + "%>";
		}

		private static void MainTest2( UsingStatement statement )
		{
			MainTest( statement, " " );
			MainTest( statement, "\r\n" );
			MainTest( statement, "\t" );
		}

		private static void MainTest( UsingStatement statement, string spaces ) 
		{
			var usingStatementText = UsingToString( statement, spaces );
			var usingStatement = ParserHelper.ParseUsing( usingStatementText );
			AssertHelper.AssertUsing( statement, usingStatement );
		}
	}
}