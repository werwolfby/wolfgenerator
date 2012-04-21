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
 *   21.02.2009 18:44 - Add some code optimization
 *   23.02.2009 00:17 - Finish MultyLineTest
 *   23.02.2009 00:44 - Finish CodeWriterExceptionTest.
 *   23.02.2009 23:26 - Finish SpaceAppendTypeTest & CloneAppendTypeTest.
 *   21.04.2012 23:16 - [*] Migrate to [NUnit].
 *
 *******************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WolfGenerator.Core.Writer;
using System.Linq;
using WolfGenerator.Core.Writer.Exception;

namespace UnitTest.WolfGenerator.Writer
{
	[TestFixture]
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

		[Test]
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

		[Test]
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

			Assert.That( codeWriter.ToString(), Is.EqualTo( expectedText ) );
		}

		[Test]
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

			Assert.That( actualText, Is.EqualTo( expectedText ) );
		}

		[Test]
		public void MultyLineTest()
		{
			var lines = new[]
			            {
			            	new CodeWriterHelper( 0, "namespace Test" ),
			            	new CodeWriterHelper( 0, "{" ),
			            	new CodeWriterHelper( 1, "public class Main\r\n{\r\n}" ),
			            	new CodeWriterHelper( 0, "}" ),
			            };

			var codeWriter = GetCodeWriter( lines );

			var expectedText = BuildText( lines );
			var actualText = codeWriter.ToString();

			Assert.That( actualText, Is.EqualTo( expectedText ) );
		}

		[Test]
		public void PrivateTest()
		{
			var codeWriter = new CodeWriter_Accessor();

			Assert.IsNull( codeWriter.lastLine, "After initialize CodeWriter lastLine must be null" );
			Assert.IsNotNull( codeWriter.Lines, "Lines must be not null" );
			Assert.AreEqual( 0, codeWriter.Lines.Count, "Lines must be empty" );

			codeWriter.Append( "Test" );
			Assert.That( codeWriter.lastLine, Is.Not.Null, "After Append text lastLine must be not null" );
			Assert.That( codeWriter.Lines, Is.Not.Null.And.Count.EqualTo( 1 ),
			             "After first Append text code write must consist from one line" );

			codeWriter.AppendLine( "Text" );
			Assert.That( codeWriter.lastLine, Is.Null, "After initialize CodeWriter lastLine must be null" );
			Assert.That( codeWriter.Lines, Is.Not.Null.And.Count.EqualTo( 1 ),
			             "After first Append text code write must consist from one line" );

			codeWriter.InnerAppend( "Text", false );
			Assert.That( codeWriter.lastLine, Is.Not.Null, "After Append text lastLine must be not null" );
			Assert.That( codeWriter.Lines, Is.Not.Null.And.Count.EqualTo( 2 ), "CodeWrite must consist from two line" );

			codeWriter.InnerAppend( "Text", true );
			Assert.That( codeWriter.lastLine, Is.Null, "After Append text lastLine must be null" );
			Assert.That( codeWriter.Lines, Is.Not.Null.And.Count.EqualTo( 2 ), "CodeWrite must consist from two line" );

			codeWriter.AppendText( "new\r\nline" );
			Assert.That( codeWriter.lastLine, Is.Not.Null, "Appended text not finish last line, but CodeWriter finish it" );
			Assert.That( codeWriter.Lines, Is.Not.Null.And.Count.EqualTo( 4 ), "CodeWrite must consist from four line" );

			codeWriter.AppendText( "new\r\nlines\r\n" );
			Assert.That( codeWriter.lastLine, Is.Null, "Appended text finish last line, but CodeWriter doesn't" );
			Assert.That( codeWriter.Lines, Is.Not.Null.And.Count.EqualTo( 5 ), "CodeWrite must consist from five line" );
		}

		[Test]
		[ExpectedException(typeof(MultyLineException))]
		public void CodeWriterExceptionTest()
		{
			var codeWriter = new CodeWriter();
			codeWriter.Append( "new\r\nline" );
		}

		[Test]
		public void CloneAppendTypeTest()
		{
			var codeWriter = new CodeWriter();

			var lines = new[]
			            {
			            	"private int val;",
			            	"private string val;"
			            };

			var commentText = "// ";
			codeWriter.Append( commentText );
			codeWriter.AppendType = AppendType.CloneLastLine;
			foreach (var line in lines)
			{
				codeWriter.AppendLine( line );
			}
			codeWriter.AppendType = AppendType.EmptyLastLine;

			var expectedText = string.Join( "\r\n", lines.Select( s => commentText + s ).ToArray() );
			var actualText = codeWriter.ToString();

			Assert.AreEqual( expectedText, actualText );
		}

		[Test]
		public void SpaceAppendTypeTest()
		{
			var codeWriter = new CodeWriter();

			var lines = new[]
			            {
			            	"int val",
			            	"string val1",
			            	"object val2",
			            	"decimal val3",
			            };

			var startLineText = "Constructor( ";
			var endLineText = " )";
			codeWriter.Append( startLineText );
			codeWriter.AppendType = AppendType.SpaceLastLine;
			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				codeWriter.Append( line );
				if (i < lines.Length - 1) codeWriter.AppendLine( "," );
			}
			codeWriter.AppendLine( endLineText );
			codeWriter.AppendType = AppendType.EmptyLastLine;

			var expectedText = startLineText +
			                   string.Join( ",\r\n", lines.Select(
			                                         	( s, i ) =>
			                                         	(i > 0
			                                         	 	? new string( ' ', startLineText.Length )
			                                         	 	: "") + s ).ToArray() ) +
			                   endLineText;
			var actualText = codeWriter.ToString();

			Assert.AreEqual( expectedText, actualText );
		}

		private static string BuildText( IEnumerable<CodeWriterHelper> lines ) 
		{
			var builder = new StringBuilder();
			var count = lines.Count();
			var i = 0;

			foreach (var line in lines)
			{
				var indentLine = new string( '\t', line.indent );
				builder.Append( indentLine );
				foreach (var text in line.texts)
				{
					if (!text.Contains( "\r\n" ))
					{
						builder.Append( text );
					}
					else
					{
						var textItems = text.Split( new[] { "\r\n" }, StringSplitOptions.None );
						for (var j = 0; j < textItems.Length; j++)
						{
							var s = textItems[j];
							if (j < textItems.Length - 1)
							{
								builder.AppendLine( s );
								builder.Append( indentLine );
							}
							else
							{
								builder.Append( s );
							}
						}
					}
				}
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
					codeWriter.AppendText( text );
				codeWriter.AppendLine();
			}
		}
	}
}
