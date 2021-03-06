/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 15.02.2009 10:30
 *
 * File: ApplyUnitTest.cs
 * Remarks:
 * 
 * History:
 *   15.02.2009 10:30 - Create Wireframe
 *   15.02.2009 10:56 - Finish first implemetation.
 *   15.02.2009 10:56 - Create test ExtendedFromApplyTest and fix it.
 *   21.04.2012 23:19 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using System.Text;
using NUnit.Framework;
using WolfGenerator.Core.AST;

namespace UnitTest.WolfGenerator.ParserTest
{
	[TestFixture]
	public class ApplyUnitTest
	{
		public static readonly ApplyStatement simpleApply       = new ApplyStatement( Helper.EmptyPosition, "SetField", "item.Value, \"Test\"", "items" );
		public static readonly ApplyStatement fromApply         = new ApplyStatement( Helper.EmptyPosition, "SetField", "item.Value, \"Test\"", "frim" );
		public static readonly ApplyStatement extendedApply     = new ApplyStatement( Helper.EmptyPosition, "SetField", "this.Method( item ), \"Test\"", "items" );
		public static readonly ApplyStatement extendedFromApply = new ApplyStatement( Helper.EmptyPosition, "SetField", "this.Method( item ), \"Test\"", "item.Items" );

		[Test]
		public void SimpleApplyTest()
		{
			MainTestMethod2( simpleApply, false );
		}

		[Test]
		public void FromApplyTest()
		{
			MainTestMethod2( fromApply, false );
		}

		[Test]
		public void ExtendedApplyTest()
		{
			MainTestMethod2( extendedApply, true );
		}

		[Test]
		public void ExtendedFromApplyTest()
		{
			MainTestMethod2( extendedFromApply, true );
		}

		public static string ApplyToString( ApplyStatement applyStatement, string spaces, bool isExtended )
		{
			var builder = new StringBuilder();

			builder.Append( "<%apply" );
			builder.Append( spaces );
			builder.Append( applyStatement.ApplyMethod );

			if (isExtended) builder.Append( "([" );
			else builder.Append( "(" );
			builder.Append( spaces );
			builder.Append( applyStatement.Parameters );
			builder.Append( spaces );
			if (isExtended) builder.Append( "])" );
			else builder.Append( ")" );

			builder.Append( spaces );
			builder.Append( "from" );
			builder.Append( spaces );
			builder.Append( applyStatement.From );

			builder.Append( "%>" );

			return builder.ToString();
		}

		private static void MainTestMethod( ApplyStatement apply, string spaces, bool isExtended ) 
		{
			var applyStatementText = ApplyToString( apply, spaces, isExtended );
			var actualApply = ParserHelper.ParseApply( applyStatementText );

			AssertHelper.AssertApply( apply, actualApply );
		}

		private static void MainTestMethod2( ApplyStatement apply, bool isExtended )
		{
			MainTestMethod( apply, " ", isExtended );
			MainTestMethod( apply, "\r\n\t", isExtended );
		}
	}
}