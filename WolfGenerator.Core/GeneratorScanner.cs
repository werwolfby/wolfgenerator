/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 26.01.2009 19:13
 *
 * File: GeneratorScanner.cs
 * Remarks:
 * 
 * History:
 *   26.01.2009 19:13 - Create Wireframe
 *
 *******************************************************/

using System.IO;
using WolfGenerator.Core.Parsing;

namespace WolfGenerator.Core
{
	public class GeneratorScanner : Scanner
	{
		private readonly WolfGeneratorCustomToolGenerator customToolGenerator;

		public GeneratorScanner( string fileName, WolfGeneratorCustomToolGenerator customToolGenerator ) : base( fileName )
		{
			this.customToolGenerator = customToolGenerator;
		}

		public GeneratorScanner( Stream s, WolfGeneratorCustomToolGenerator customToolGenerator ) : base( s )
		{
			this.customToolGenerator = customToolGenerator;
		}

		public override void WriteLine( string s )
		{
			customToolGenerator.WriteLine( s );
		}

		public override void Write( string s )
		{
			customToolGenerator.Write( s );
		}

		public override void WriteError( string fmt, string file, int lin, int col, string err )
		{
			customToolGenerator.WriteError( lin, col, WolfGeneratorCustomToolGenerator.ErrorLevel.Error, err );
		}
	}
}