using System;
using System.Runtime.InteropServices;
using System.Text;
using CustomToolGenerator;
using EnvDTE;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core
{
	[Guid( "02F8A379-7EBA-420C-AFAD-9B8E98DA8562" )]
	[ComVisible( true )]
	public class WolfGeneratorCustomToolGenerator : BaseCodeGeneratorWithSite
	{
		public enum ErrorLevel
		{
			Info,
			Warning,
			Error
		}

		private string fileName;
		private OutputWindowPane generatorPane;
		private const string wolfGeneratorPaneName = "WolfGenerator";

		#region IVsSingleFileGenerator Members
		public override int DefaultExtension( out string pbstrDefaultExtension )
		{
			pbstrDefaultExtension = ".cs";
			return 0;
		}
		#endregion

		protected override byte[] GenerateCode( string inputFileName, string inputFileContent )
		{
			this.fileName = inputFileName;

			var serviseObject = this.GetService<ProjectItem>();
			Project containintProject;

			if (serviseObject != null)
			{
				containintProject = serviseObject.ContainingProject;
				var dte = containintProject.DTE;
				var outputWindow = dte.Windows.Item( Constants.vsWindowKindOutput );
				var output = (OutputWindow)outputWindow.Object;
				foreach (OutputWindowPane pane in output.OutputWindowPanes)
				{
					if (pane.Name != wolfGeneratorPaneName) continue;

					this.generatorPane = pane;
					this.generatorPane.Clear();
					break;
				}
				if (this.generatorPane == null)
					this.generatorPane = output.OutputWindowPanes.Add( wolfGeneratorPaneName );
			}

			string programCode;
			try
			{
				var scanner = new GeneratorScanner( inputFileName, this );
				var parser = new Parser( scanner );

				parser.Parse();

				if (parser.errors.count == 0)
					programCode = new Generator().Generate( FileNameSpace, parser.ruleClassStatement, inputFileName ).ToString();
				else
					programCode = "parser errors: " + parser.errors.count;
			}
			catch (Exception e)
			{
				programCode = "// Exception See OutputWindow/WolfGenerator for detaile";
				WriteError( 0, 0, ErrorLevel.Error, e.Message );
			}

			return Encoding.UTF8.GetBytes( programCode );
		}

		public void Write( string str )
		{
			this.generatorPane.OutputString( str );
		}

		public void WriteLine( string str )
		{
			this.generatorPane.OutputString( str );
			this.generatorPane.OutputString( "\r\n" );
		}

		public void WriteError( int line, int column, ErrorLevel errorLevel, string message )
		{
			this.generatorPane.OutputString( string.Format( "{0}({1},{2}): {3} {4}\n",
			                                                fileName, line, column, errorLevel, message ) );
			this.CodeGeneratorProgress.GeneratorError( 0, 1, message, (uint)line, (uint)column );
		}
	}
}