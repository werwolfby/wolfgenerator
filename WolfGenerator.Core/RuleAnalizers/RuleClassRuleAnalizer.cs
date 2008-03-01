/*****************************************************
 *
 * Created by: WerWolf
 * Created: 11.05.2007 13:44:24
 *
 * File: RuleClassRuleAnalizer.cs
 * Remarks:
 *
 *****************************************************/

using System;
using System.Collections.Generic;

namespace WolfGenerator.Core
{
	[RuleAnalizer( "ruleclass" )]
	public class RuleClassRuleAnalizer : RuleAnalizer
	{
		public override RuleNode Analize( List<RuleString> ruleStrings, ref int index )
		{
			RuleString ruleClassString = ruleStrings[index++];
			if (!(ruleClassString is CommandRuleString) && !ruleClassString.Text.StartsWith( "ruleclass" ))
				throw new Exception( "wrong first rule string" );

			string name = ruleClassString.Text.Substring( "ruleclass".Length ).Trim();

			List<ClassRuleNode.UsingData> usingList = new List<ClassRuleNode.UsingData>();
			List<ClassRuleNode.ReferenceData> referenceList = new List<ClassRuleNode.ReferenceData>();
			List<RuleNode> innerRuleNodeList = new List<RuleNode>();

			string innerMethodName = "";

			for (; index < ruleStrings.Count; index++)
			{
				RuleString ruleString = ruleStrings[index];

				if (ruleString is CommandRuleString)
				{
					if (ruleString.Text.StartsWith( "using" ))
					{
						usingList.Add( new ClassRuleNode.UsingData( ruleString.Text.Substring( "using".Length ).Trim(), ruleString.FileName,
						                                            ruleString.LineNumber, ruleString.ColumNumber ) );
					}
					else if (ruleString.Text.StartsWith( "reference" ))
					{
						referenceList.Add( new ClassRuleNode.ReferenceData( ruleString.Text.Substring( "reference".Length ).Trim(),
						                                                    ruleString.FileName, ruleString.LineNumber, ruleString.ColumNumber ) );
					}
					else if (ruleString.Text.StartsWith( "inner_method_name" ))
					{
						innerMethodName = ruleString.Text.Substring( "inner_method_name".Length + 1 ).Trim();
					}
					else if (ruleString.Text.StartsWith( "end class" )) break;
					else
					{
						RuleAnalizer ruleAnalizer = FindAnAnalizer( ruleString );
						innerRuleNodeList.Add( ruleAnalizer.Analize( ruleStrings, ref index ) );
					}
				}
				else if (ruleString is ValueRuleString) 
					throw new Exception( "Rule class can't contain value rule nodes" );
				else if (ruleString is TextRuleString && ruleString.Text != "\r\n")
				{
					//throw new Exception( "Rule class can't contain only \\n\\r text nodes" );
				}
			}

			return new ClassRuleNode( ruleClassString.FileName, ruleClassString.LineNumber, ruleClassString.ColumNumber,
			                          name, usingList, referenceList, innerRuleNodeList, innerMethodName );
		}
	}

	public class ClassRuleNode : RuleNode
	{
		public class UsingData
		{
			private readonly string usingName;
			private readonly string fileName;
			private readonly int lineNumber;
			private readonly int columnNumber;

			public UsingData( string usingName, string fileName, int lineNumber, int columnNumber )
			{
				this.fileName = fileName;
				this.usingName = usingName;
				this.lineNumber = lineNumber;
				this.columnNumber = columnNumber;
			}

			public string UsingName
			{
				get
				{
					return this.usingName;
				}
			}

			public string FileName
			{
				get
				{
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			public int ColumnNumber
			{
				get
				{
					return this.columnNumber;
				}
			}

			public string Line()
			{
				//return "#line " + lineNumber + " \"" + fileName + "\"";
				return "";
			}
		}

		public class ReferenceData
		{
			private readonly string referenceName;
			private readonly string fileName;
			private readonly int lineNumber;
			private readonly int columnNumber;

			public ReferenceData( string referenceName, string fileName, int lineNumber, int columnNumber )
			{
				this.referenceName = referenceName;
				this.fileName = fileName;
				this.lineNumber = lineNumber;
				this.columnNumber = columnNumber;
			}

			public string ReferenceName
			{
				get
				{
					return this.referenceName;
				}
			}

			public string FileName
			{
				get
				{
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			public int ColumnNumber
			{
				get
				{
					return this.columnNumber;
				}
			}

			public string Line()
			{
				//return "#line " + lineNumber + " \"" + fileName + "\"";
				return "";
			}
		}

		#region Fields
		private readonly string name;
		private readonly List<UsingData> usingList;
		private readonly List<ReferenceData> referenceList;
		private readonly List<RuleNode> innerRuleNode;
	
		private readonly string innerMethodName;
		#endregion

		#region Constructors
		public ClassRuleNode( string fileName, int lineNumber, int columnNumber, string name, List<UsingData> usingList, List<ReferenceData> referenceList, List<RuleNode> innerRuleNode, string innerMethodName )
			: base( fileName, lineNumber, columnNumber )
		{
			this.name = name;
			this.innerMethodName = innerMethodName;
			this.usingList = usingList;
			this.referenceList = referenceList;
			this.innerRuleNode = innerRuleNode;
		}
		#endregion

		#region Properties
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public List<ReferenceData> ReferenceList
		{
			get
			{
				return this.referenceList;
			}
		}

		public string InnerMethodName
		{
			get { return this.innerMethodName; }
		}
		#endregion

		#region Methods
		public void AddUsingList( CodeWriter codeWriter )
		{
			foreach (UsingData usingData in usingList)
			{
				//codeWriter.AppendLine( usingData.Line() );
				codeWriter.AppendLine( "using " + usingData.UsingName + ";" );
			}
		}

		public string Generate( CodeWriter codeWriter )
		{
			CodeGenerator codeGenerator = new CodeGenerator();
			//codeWriter.AppendLine( "#line " + LineNumber + " \"" + FileName + "\"" );
			codeWriter.AppendLine( "public partial class " + name );
			codeWriter.AppendLine( "{" );
			codeWriter.Indent++;

			Workaround( codeGenerator, innerMethodName );

			Generate( "codeWriter", codeGenerator, codeWriter, innerMethodName );

			codeGenerator.GenerateTo( codeWriter );

			codeWriter.Indent--;
			codeWriter.AppendLine( "}" );

			return codeWriter.ToString();
		}

		public override void Workaround( CodeGenerator generator, string prefixName )
		{
			foreach (RuleNode ruleNode in innerRuleNode)
			{
				ruleNode.Workaround( generator, prefixName );
			}
		}

		public override void Generate( string stringWriterName, CodeGenerator generator, CodeWriter codeWriter, string prefixName )
		{
			foreach (RuleNode ruleNode in innerRuleNode)
			{
				ruleNode.Generate( stringWriterName, generator, codeWriter, prefixName );
			}
		}
		#endregion

		public override string GenerateCallMethod( CodeGenerator generator, string prefixName )
		{
			throw new NotSupportedException();
		}
	}
}