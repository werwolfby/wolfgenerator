/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 04.02.2009 01:05
 *
 * File: GeneratorBase.cs
 * Remarks:
 * 
 * History:
 *   12.02.2009 21:49 - Create Wireframe
 *
 *******************************************************/
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core;
using WolfGenerator.Core.AST;
using System.Collections.Generic;

namespace UnitTest.WolfGenerator
{
	[TestClass]
	public class ParserTest
	{
		/// <summary>
		///A test for Var
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void VarTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			Variable var = null; // TODO: Initialize to an appropriate value
			Variable varExpected = null; // TODO: Initialize to an appropriate value
			target.Var(out var);
			Assert.AreEqual(varExpected, var);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Value
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void ValueTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			ValueStatement valueStatement = null; // TODO: Initialize to an appropriate value
			ValueStatement valueStatementExpected = null; // TODO: Initialize to an appropriate value
			target.Value(out valueStatement);
			Assert.AreEqual(valueStatementExpected, valueStatement);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Using
		///</summary>
		[TestMethod]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void UsingTest()
		{
			var @namespace = "Test.Test";

			var usingStatementText = "<%using " + @namespace + "%>";

			var target = new Parser_Accessor( usingStatementText );
			UsingStatement usingStatement;
			target.Using( out usingStatement );

			Assert.AreEqual( @namespace, usingStatement.Namespace );
		}

		/// <summary>
		///A test for Type
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void TypeTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			Type type = null; // TODO: Initialize to an appropriate value
			Type typeExpected = null; // TODO: Initialize to an appropriate value
			target.Type(out type);
			Assert.AreEqual(typeExpected, type);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for RuleMethodStart
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void RuleMethodStartTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			string name = string.Empty; // TODO: Initialize to an appropriate value
			string nameExpected = string.Empty; // TODO: Initialize to an appropriate value
			IList<Variable> variables = null; // TODO: Initialize to an appropriate value
			IList<Variable> variablesExpected = null; // TODO: Initialize to an appropriate value
			target.RuleMethodStart(out name, out variables);
			Assert.AreEqual(nameExpected, name);
			Assert.AreEqual(variablesExpected, variables);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for RuleMethodEnd
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void RuleMethodEndTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			target.RuleMethodEnd();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for RuleMethod
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void RuleMethodTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			RuleMethodStatement statement = null; // TODO: Initialize to an appropriate value
			RuleMethodStatement statementExpected = null; // TODO: Initialize to an appropriate value
			target.RuleMethod(out statement);
			Assert.AreEqual(statementExpected, statement);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for RuleClassStart
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void RuleClassStartTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			string name = string.Empty; // TODO: Initialize to an appropriate value
			string nameExpected = string.Empty; // TODO: Initialize to an appropriate value
			target.RuleClassStart(out name);
			Assert.AreEqual(nameExpected, name);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Method
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void MethodTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			MethodStatement methodStatement = null; // TODO: Initialize to an appropriate value
			MethodStatement methodStatementExpected = null; // TODO: Initialize to an appropriate value
			target.Method(out methodStatement);
			Assert.AreEqual(methodStatementExpected, methodStatement);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for MatchMethod
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void MatchMethodTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			MatchMethodStatement statement = null; // TODO: Initialize to an appropriate value
			MatchMethodStatement statementExpected = null; // TODO: Initialize to an appropriate value
			target.MatchMethod(out statement);
			Assert.AreEqual(statementExpected, statement);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Join
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void JoinTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			JoinStatement joinStatement = null; // TODO: Initialize to an appropriate value
			JoinStatement joinStatementExpected = null; // TODO: Initialize to an appropriate value
			target.Join(out joinStatement);
			Assert.AreEqual(joinStatementExpected, joinStatement);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Code
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void CodeTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			CodeStatement codeStatement = null; // TODO: Initialize to an appropriate value
			CodeStatement codeStatementExpected = null; // TODO: Initialize to an appropriate value
			bool isStart = false; // TODO: Initialize to an appropriate value
			bool isStartExpected = false; // TODO: Initialize to an appropriate value
			target.Code(out codeStatement, ref isStart);
			Assert.AreEqual(codeStatementExpected, codeStatement);
			Assert.AreEqual(isStartExpected, isStart);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Call
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void CallTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			CallStatement callStatement = null; // TODO: Initialize to an appropriate value
			CallStatement callStatementExpected = null; // TODO: Initialize to an appropriate value
			target.Call(out callStatement);
			Assert.AreEqual(callStatementExpected, callStatement);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Apply
		///</summary>
		[TestMethod()]
		[DeploymentItem("WolfGenerator.Core.dll")]
		public void ApplyTest()
		{
			PrivateObject param0 = null; // TODO: Initialize to an appropriate value
			Parser_Accessor target = new Parser_Accessor(param0); // TODO: Initialize to an appropriate value
			ApplyStatement applyStatement = null; // TODO: Initialize to an appropriate value
			ApplyStatement applyStatementExpected = null; // TODO: Initialize to an appropriate value
			target.Apply(out applyStatement);
			Assert.AreEqual(applyStatementExpected, applyStatement);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}
	}
}
