using System;
using System.Runtime.InteropServices;
using System.Text;
using CustomToolGenerator;
using WolfGenerator.Core.Writer;

namespace WolfGenerator.Core
{
    [Guid( "02F8A379-7EBA-420C-AFAD-9B8E98DA8562" )]
    [ComVisible( true )]
    public class WolfGeneratorCustomToolGenerator : BaseCodeGeneratorWithSite
    {
        #region IVsSingleFileGenerator Members
        public int DefaultExtension( out string pbstrDefaultExtension )
        {
            pbstrDefaultExtension = ".cs";
            return 0;
        }
    	#endregion

    	protected override byte[] GenerateCode( string inputFileName, string inputFileContent )
    	{
        	string programCode;
			try
			{
				var scanner = new Scanner( inputFileName );
				var parser = new Parser( scanner );

				parser.Parse();

				if (parser.errors.count == 0)
				{
					programCode = Generator.Generate( parser.ruleClassStatement ).ToString();
				}
				else
				{
					programCode = "parser errors: " + parser.errors.count;
				}
			}
			catch(Exception e)
			{
				programCode = "// Exception: " + e.Message;
			}

    		return Encoding.UTF8.GetBytes( programCode );
    	}
    }
}