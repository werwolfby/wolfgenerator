/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 14.02.2009 11:51
 *
 * File: AssertHelper.cs
 * Remarks:
 * 
 * History:
 *   14.02.2009 11:51 - Create Wireframe
 *   14.02.2009 12:39 - Change type of method AssertType to int, to support Func<> syntax.
 *   14.02.2009 22:05 - Add AssertVariable. Add in AssertType check for null GenericParameters.
 *   15.02.2009 10:47 - Add AssertApply.
 *   15.02.2009 11:29 - Add AssertCode.
 *   15.02.2009 13:29 - Move AssertValue from ValueUnitTest.
 *   15.02.2009 13:33 - Fix: Forget AssertValue make return type to int.
 *   15.02.2009 13:47 - Add AssertJoin method (I hope in functional style).
 *   15.02.2009 14:07 - Fix: again forget return type of AssertJoin method.
 *   15.02.2009 15:56 - Add AssertVariables.
 *
 *******************************************************/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.AST;
using Type=WolfGenerator.Core.AST.Type;

namespace UnitTest.WolfGenerator
{
	public class AssertHelper
	{
		private abstract class JoinInnerStatementData
		{
			public abstract bool Check( RuleStatement expected );

			public abstract void Invoke( RuleStatement expected, RuleStatement actual );
		}

		private class JoinInnerStatementData<T> : JoinInnerStatementData
			where T : RuleStatement
		{
			public JoinInnerStatementData( Func<T, T, int> func )
			{
				this.Func = func;
			}

			public Func<T, T, int> Func { get; private set; }

			public override bool Check( RuleStatement expected )
			{
				return expected is T;
			}

			public override void Invoke( RuleStatement expected, RuleStatement actual )
			{
				AssertJoinInnerStatementHelper( expected, actual, this.Func );
			}
		}

		public static int AssertType( Type expected, Type actual )
		{
			Assert.AreEqual( expected.TypeName, actual.TypeName, "Wrong name of types" );
			Assert.IsNotNull( expected.GenericParameters, "GenericParameters can't be null" );
			if (expected.GenericParameters.Count > 0)
			{
				Assert.IsNotNull( actual.GenericParameters, "GenericParameters can't be null" );
				Assert.AreEqual( expected.GenericParameters.Count, actual.GenericParameters.Count,
				                 "Wrong count of generic parameter" );
				for (var i = 0; i < expected.GenericParameters.Count; i++)
				{
					var expectedGenericParameter = expected.GenericParameters[i];
					var actualGenericParameter = actual.GenericParameters[i];

					AssertType( expectedGenericParameter, actualGenericParameter );
				}
			}
			else
			{
				Assert.AreEqual( 0, expected.GenericParameters.Count, "Expected type contains generic parameters" );
			}

			return 0;
		}

		public static int AssertVariable( Variable expected, Variable actual )
		{
			Assert.AreEqual( expected.Name, actual.Name,
			                 String.Format( "expected name ('{0}') don't match actual name ('{1}')", expected.Name, actual.Name ) );
			AssertType( expected.Type, actual.Type );

			return 0;
		}

		public static int AssertValue( ValueStatement expected, ValueStatement actual ) 
		{
			Assert.IsNotNull( actual.Value, "Value can't be null" );
			Assert.AreEqual( expected.Value.Trim(), actual.Value.Trim() );
			return 0;
		}

		public static int AssertApply( ApplyStatement expected, ApplyStatement actual )
		{
			Assert.AreEqual( expected.ApplyMethod, actual.ApplyMethod, "Apply Method are wrong" );
			Assert.AreEqual( expected.Parameters, actual.Parameters, "Apply Parameters are different" );
			Assert.AreEqual( expected.From, actual.From, "Apply from are different" );
			return 0;
		}

		public static int AssertCode( CodeStatement expected, CodeStatement actual, bool expectedIsStart, bool actualIsStart )
		{
			Assert.AreEqual( expected.Value.Trim(), actual.Value.Trim(), "Code value are different" );
			Assert.AreEqual( expectedIsStart, actualIsStart, "IsStart are different" );

			return 0;
		}

		public static int AssertJoin( JoinStatement expected, JoinStatement actual )
		{
			var joinInnerTypes = new JoinInnerStatementData[]
			                     {
			                     	new JoinInnerStatementData<ValueStatement>( AssertValue ),
			                     	new JoinInnerStatementData<ApplyStatement>( AssertApply ),
			                     };

			Assert.AreEqual( expected.String, actual.String, "Join string are different" );
			Assert.AreEqual( expected.Statements.Count, actual.Statements.Count, "Join statements are different" );
			for (var i = 0; i < expected.Statements.Count; i++)
			{
				var finded = false;
				foreach (var joinInnerType in joinInnerTypes)
				{
					if (!joinInnerType.Check( expected.Statements[i] )) continue;

					joinInnerType.Invoke( expected.Statements[i], actual.Statements[i] );
					finded = true;
				}

				if (!finded) Assert.Fail( "Join cann't containt statement of type: {0}", expected.Statements[i].GetType() );
			}

			return 0;
		}

		public static int AssertMatch( MatchMethodStatement expected, MatchMethodStatement actual )
		{
			Assert.AreEqual( expected.Name, actual.Name, "Match method name" );
			Assert.AreEqual( expected.Code.Trim(), actual.Code.Trim(), "Match code" );

			return 0;
		}

		public static void AssertVariables( Variable[] expectedVariables, IList<Variable> variables ) 
		{
			Assert.AreEqual( expectedVariables.Length, variables.Count, "Variables count dismatch" );
			for (var i = 0; i < expectedVariables.Length; i++)
				AssertVariable( expectedVariables[i], variables[i] );
		}

		private static void AssertJoinInnerStatementHelper<T>( RuleStatement expected, RuleStatement actual, Func<T,T,int> assertFunc )
			where T : RuleStatement
		{
			var expectedValueStatement = (T)expected;
			Assert.IsInstanceOfType( actual, typeof(T) );
			var actualValueStatement = (T)actual;
			assertFunc( expectedValueStatement, actualValueStatement );
		}
	}
}