using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace WolfGenerator.Core
{
    [Guid( "02F8A379-7EBA-420C-AFAD-9B8E98DA8562" )]
    [ComVisible( true )]
    public class CustomToolGenerator : IVsSingleFileGenerator
    {
        #region IVsSingleFileGenerator Members
        public int DefaultExtension( out string pbstrDefaultExtension )
        {
            pbstrDefaultExtension = ".cs";
            return 0;
        }

        public int Generate( string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
                             IntPtr[] rgbOutputFileContents,
                             out uint pcbOutput, IVsGeneratorProgress pGenerateProgress )
        {
        	string programCode;
			try
			{
				FileInfo fileInfo = new FileInfo( wszInputFilePath );
				Compiler compiler = new Compiler( bstrInputFileContents, fileInfo.Name );

				compiler.Parse();
				compiler.Generate();

				programCode = compiler.ProgramCode;
			}
			catch(Exception e)
			{
				programCode = "// Exception: " + e.Message;
			}

        	byte[] bytes = Encoding.GetEncoding( 1251 ).GetBytes( programCode );

        	rgbOutputFileContents[0] = Marshal.AllocCoTaskMem( bytes.Length );
			Marshal.Copy( bytes, 0, rgbOutputFileContents[0], bytes.Length );

        	pcbOutput = (uint)bytes.Length;

        	return 0;
        }
    	#endregion
    }
}