/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 21.02.2009 17:40
 *
 * File: CodeWriterUnitTest.cs
 * Remarks:
 * 
 * History:
 *   21.02.2009 17:40 - Create Wireframe
 *   21.02.2009 17:51 - Finish SimpleCodeWriterTest
 *   21.02.2009 17:56 - Finish ComplexCodeWriterTest
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.Writer;

namespace UnitTest.WolfGenerator.Writer
{
	[TestClass]
	public class CodeWriterUnitTest
	{
		private class CodeWriterHelper 
		{
			private readonly int indent;
			private readonly string[] texts;

			public int Indent
			{
				get { return indent; }
			}

			public string[] Texts
			{
				get { return texts; }
			}

			public CodeWriterHelper( int indent, params string[] texts )
			{
				this.indent = indent;
				this.texts = texts;
			}
		}

		[TestMethod]
		public void SimpleCodeWriterTest()
		{
			var lines = new[]
			            {
			            	new CodeWriterHelper( 1, "namespace Test" ),
			            	new CodeWriterHelper( 1, "{" ),
			            	new CodeWriterHelper( 2, "public class A" ),
			            	new CodeWriterHelper( 2, "{" ),
			            	new CodeWriterHelper( 3, "public void MethodA() {}" ),
			            	new CodeWriterHelper( 2, "}" ),
			            	new CodeWriterHelper( 1, "}" ),
			            };
			var expectedText = BuildText( lines );

			var codeWriter = new CodeWriter();

			foreach (var line in lines)
			{
				codeWriter.Indent = line.Indent;
				foreach (var text in line.Texts)
					codeWriter.Append( text );
				codeWriter.AppendLine();
			}

			Assert.AreEqual( expectedText, codeWriter.ToString() );
		}

		[TestMethod]
		public void ÑomplexCodeWriterTest()
		{
			var lines = new[]
			            {
			            	new CodeWriterHelper( 1, "namespace Test" ),
			            	new CodeWriterHelper( 1, "{" ),
			            	new CodeWriterHelper( 2, "public", " class", " A" ),
			            	new CodeWriterHelper( 2, "{" ),
			            	new CodeWriterHelper( 3, "public void MethodA() {}" ),
			            	new CodeWriterHelper( 2, "}" ),
			            	new CodeWriterHelper( 1, "}" ),
			            };
			var expectedText = BuildText( lines );

			var codeWriter = new CodeWriter();

			foreach (var line in lines)
			{
				codeWriter.Indent = line.Indent;
				foreach (var text in line.Texts)
					codeWriter.Append( text );
				codeWriter.AppendLine();
			}

			Assert.AreEqual( expectedText, codeWriter.ToString() );
		}

		private static string BuildText( IList<CodeWriterHelper> lines ) 
		{
			var builder = new StringBuilder();

			for (var i = 0; i < lines.Count; i++)
			{
				var line = lines[i];
				builder.Append( new string( '\t', line.Indent ) );
				foreach (var text in line.Texts)
					builder.Append( text );
				if (i < lines.Count - 1) builder.AppendLine();
			}

			return builder.ToString();
		}
	}
}