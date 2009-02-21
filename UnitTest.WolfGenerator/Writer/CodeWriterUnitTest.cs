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
 *   21.02.2009 18:40 - Finish InnerAppendCodeWriterTest
 *
 *******************************************************/

using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WolfGenerator.Core.Writer;
using System.Linq;

namespace UnitTest.WolfGenerator.Writer
{
	[TestClass]
	public class CodeWriterUnitTest
	{
		private class CodeWriterHelper 
		{
			public readonly int indent;
			public readonly string[] texts;

			public CodeWriterHelper( int indent, params string[] texts )
			{
				this.indent = indent;
				this.texts = texts;
			}
		}

		private class CodeWriterHelperHierarchy
		{
			public readonly IEnumerable<CodeWriterHelper> startHelpers;
			public readonly IEnumerable<CodeWriterHelperHierarchy> innerItems;
			public readonly IEnumerable<CodeWriterHelper> endHelpers;

			public CodeWriterHelperHierarchy( IEnumerable<CodeWriterHelper> startHelpers, IEnumerable<CodeWriterHelper> endHelpers, params CodeWriterHelperHierarchy[] innerItems )
			{
				this.startHelpers = startHelpers ?? new CodeWriterHelper[0];
				this.innerItems = innerItems ?? new CodeWriterHelperHierarchy[0];
				this.endHelpers = endHelpers ?? new CodeWriterHelper[0];
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
            var codeWriter = GetCodeWriter( lines );

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
			var codeWriter = GetCodeWriter( lines );

			Assert.AreEqual( expectedText, codeWriter.ToString() );
		}

		[TestMethod]
		public void InnerAppendCodeWriterTest()
		{
			var linesStart = new[]
			                 {
			                 	new CodeWriterHelper( 0, "namespace Test" ),
			                 	new CodeWriterHelper( 0, "{" ),
			                 	new CodeWriterHelper( 1, "public class Main" ),
			                 	new CodeWriterHelper( 1, "{" ),
			                 };
			var linesEnd = new[]
			               {
			               	new CodeWriterHelper( 1, "}" ),
			               	new CodeWriterHelper( 0, "}" ),
			               };

			var lines1 = new[]
			             {
			             	new CodeWriterHelper( 1, "public void Method1()" ),
			             	new CodeWriterHelper( 1, "{" ),
			             	new CodeWriterHelper( 1, "}" ),
			             };
			var lines2 = new[]
			             {
			             	new CodeWriterHelper( 1, "public void Method2( int value )" ),
			             	new CodeWriterHelper( 1, "{" ),
			             	new CodeWriterHelper( 1, "}" ),
			             };

			var mainCodeWriter = GetCodeWriter( linesStart );
			mainCodeWriter.Append( GetCodeWriter( lines1 ) );
			mainCodeWriter.AppendLine();
			mainCodeWriter.Append( GetCodeWriter( lines2 ) );
			mainCodeWriter.AppendLine();
            AppendCodeWriter( mainCodeWriter, linesEnd );

			var hierarchy = new CodeWriterHelperHierarchy( linesStart, linesEnd, new CodeWriterHelperHierarchy( lines1, lines2 ) );

			var expectedText = BuildText( hierarchy );
            var actualText = mainCodeWriter.ToString();

			Assert.AreEqual( expectedText, actualText );
		}

		private static string BuildText( IEnumerable<CodeWriterHelper> lines ) 
		{
			var builder = new StringBuilder();
            var count = lines.Count();
			var i = 0;

			foreach (var line in lines)
			{
				builder.Append( new string( '\t', line.indent ) );
				foreach (var text in line.texts)
					builder.Append( text );
				if (i < count - 1) builder.AppendLine();
				i++;
			}

			return builder.ToString();
		}
		
		private static string BuildText( CodeWriterHelperHierarchy hierarchy )
		{
			var list = new List<CodeWriterHelper>();

			AppendHierarchy( 0, list, hierarchy );

			return BuildText( list );
		}

		private static void AppendHierarchy( int startIndent, ICollection<CodeWriterHelper> list, CodeWriterHelperHierarchy hierarchy )
		{
			var lastIndent = 0;

			foreach (var helper in hierarchy.startHelpers)
			{
				list.Add( new CodeWriterHelper( startIndent + helper.indent, helper.texts ) );
				lastIndent = helper.indent;
			}

			foreach (var item in hierarchy.innerItems)
				AppendHierarchy( lastIndent, list, item );

			foreach (var helper in hierarchy.endHelpers)
				list.Add( new CodeWriterHelper( startIndent + helper.indent, helper.texts ) );
		}

		private static CodeWriter GetCodeWriter( IEnumerable<CodeWriterHelper> lines ) 
		{
			var codeWriter = new CodeWriter();
			AppendCodeWriter( codeWriter, lines );
			return codeWriter;
		}

		private static void AppendCodeWriter( CodeWriter codeWriter, IEnumerable<CodeWriterHelper> lines )
		{
			foreach (var line in lines)
			{
				codeWriter.Indent = line.indent;
				foreach (var text in line.texts)
					codeWriter.Append( text );
				codeWriter.AppendLine();
			}
		}
	}
}