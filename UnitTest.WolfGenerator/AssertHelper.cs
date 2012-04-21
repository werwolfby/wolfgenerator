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
 *   28.02.2009 15:43 - Add AssertError.
 *   28.02.2009 16:38 - Add AssertStatementPosition.
 *   22.04.2012 00:05 - [*] Use [Assert] from [NUnit].
 *
 *******************************************************/

using System;
using System.Collections.Generic;
using NUnit.Framework;
using WolfGenerator.Core.AST;
using WolfGenerator.Core.Parsing;
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
			Assert.That( actual.TypeName, Is.EqualTo( expected.TypeName ), "Wrong name of types" );
			Assert.That( expected.GenericParameters, Is.Not.Null, "GenericParameters can't be null" );
			if (expected.GenericParameters.Count > 0)
			{
				Assert.That( actual.GenericParameters, Is.Not.Null.And.Count.EqualTo( expected.GenericParameters.Count ), "GenericParameters is not equal" );
				for (var i = 0; i < expected.GenericParameters.Count; i++)
				{
					var expectedGenericParameter = expected.GenericParameters[i];
					var actualGenericParameter = actual.GenericParameters[i];

					AssertType( expectedGenericParameter, actualGenericParameter );
				}
			}
			else
				Assert.That( expected.GenericParameters, Has.Count.EqualTo( 0 ), "Expected type contains generic parameters" );

			return 0;
		}

		public static int AssertVariable( Variable expected, Variable actual )
		{
			Assert.That( actual.Name, Is.EqualTo( expected.Name ),
			                 String.Format( "expected name ('{0}') don't match actual name ('{1}')", expected.Name, actual.Name ) );
			AssertType( expected.Type, actual.Type );

			return 0;
		}

		public static int AssertValue( ValueStatement expected, ValueStatement actual )
		{
			Assert.That( actual.Value, Is.Not.Null, "Value can't be null" );
			Assert.That( actual.Value.Trim(), Is.EqualTo( expected.Value.Trim() ) );
			return 0;
		}

		public static int AssertApply( ApplyStatement expected, ApplyStatement actual )
		{
			Assert.That( actual.ApplyMethod, Is.EqualTo( expected.ApplyMethod ), "Apply Method are wrong" );
			Assert.That( actual.Parameters, Is.EqualTo( expected.Parameters ), "Apply Parameters are different" );
			Assert.That( actual.From, Is.EqualTo( expected.From ), "Apply from are different" );
			return 0;
		}

		public static int AssertCode( CodeStatement expected, CodeStatement actual, bool expectedIsStart, bool actualIsStart )
		{
			Assert.That( actual.Value.Trim(), Is.EqualTo( expected.Value.Trim() ), "Code value are different" );
			Assert.That( actualIsStart, Is.EqualTo( expectedIsStart ), "IsStart are different" );

			return 0;
		}

		public static int AssertJoin( JoinStatement expected, JoinStatement actual )
		{
			var joinInnerTypes = new JoinInnerStatementData[]
			                     {
			                     	new JoinInnerStatementData<ValueStatement>( AssertValue ),
			                     	new JoinInnerStatementData<ApplyStatement>( AssertApply ),
			                     };

			Assert.That( actual.String, Is.EqualTo( expected.String ), "Join string are different" );
			Assert.That( actual.Statements, Has.Count.EqualTo( expected.Statements.Count ), "Join statements are different" );
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
			Assert.That( actual.Name, Is.EqualTo( expected.Name ), "Match method name" );
			Assert.That( actual.Code.Trim(), Is.EqualTo( expected.Code.Trim() ), "Match code" );

			return 0;
		}

		public static void AssertVariables( Variable[] expectedVariables, IList<Variable> variables )
		{
			Assert.That( expectedVariables.Length, Is.EqualTo( variables.Count ), "Variables count dismatch" );
			for (var i = 0; i < expectedVariables.Length; i++)
				AssertVariable( expectedVariables[i], variables[i] );
		}

		public static void AssertUsing( UsingStatement expected, UsingStatement actual )
		{
			Assert.That( actual.Namespace, Is.EqualTo( expected.Namespace ) );
		}

		private static void AssertJoinInnerStatementHelper<T>( RuleStatement expected, RuleStatement actual,
		                                                       Func<T, T, int> assertFunc )
			where T : RuleStatement
		{
			var expectedValueStatement = (T)expected;
			Assert.That( actual, Is.TypeOf<T>() );
			var actualValueStatement = (T)actual;
			assertFunc( expectedValueStatement, actualValueStatement );
		}

		public static void AssertError( ErrorData errorData, int? line, int? column, string message )
		{
			Assert.That( line, Is.Null.Or.EqualTo( errorData.line ), "Error line wrong" );
			Assert.That( column, Is.Null.Or.EqualTo( errorData.column ), "Error column wrong" );
			Assert.That( message, Is.Null.Or.Empty.Or.EqualTo( errorData.message ), "Error message wrong" );
		}

		public static void AssertStatementPosition( int startPos, int endPos, StatementPosition actualPosition )
		{
			Assert.That( actualPosition.StartPos, Is.EqualTo( startPos ), "StartPos wrong" );
			Assert.That( actualPosition.EndPos, Is.EqualTo( endPos ), "EndPos wrong" );
		}

		public static void AssertStatementPosition( int startLine, int endLine, int startColumn, int endColumn, int startPos,
		                                            int endPos, StatementPosition actualPosition )
		{
			Assert.That( actualPosition.StartLine, Is.EqualTo( startLine ), "StartLine wrong" );
			Assert.That( actualPosition.EndLine, Is.EqualTo( endLine ), "EndLine wrong" );
			Assert.That( actualPosition.StartColumn, Is.EqualTo( startColumn ), "StartColumn wrong" );
			Assert.That( actualPosition.EndColumn, Is.EqualTo( endColumn ), "EndColumn wrong" );

			AssertStatementPosition( startPos, endPos, actualPosition );
		}
	}
}