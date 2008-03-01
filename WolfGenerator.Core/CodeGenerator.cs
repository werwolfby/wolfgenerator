/*****************************************************
 *
 * Created by: WerWolf
 * Created: 14.05.2007 9:33:38
 *
 * File: CodeGenerator.cs
 * Remarks:
 *
 *****************************************************/

using System.Collections.Generic;

namespace WolfGenerator.Core
{
	public class CodeGenerator
	{
		private Dictionary<string, List<string>> codeList = new Dictionary<string, List<string>>();
		private Dictionary<string, object> session = new Dictionary<string, object>();

		public Dictionary<string, object> Session
		{
			get
			{
				return this.session;
			}
		}

		public void AddCode( string codeRegion, string code )
		{
			List<string> codeDataList;
			codeList.TryGetValue( codeRegion, out codeDataList );
			if (codeDataList == null)
			{
				codeDataList = new List<string>();
				codeList.Add( codeRegion, codeDataList );
			}

			codeDataList.Add( code );
		}

		public void GenerateTo( CodeWriter codeWriter )
		{
			foreach (KeyValuePair<string, List<string>> keyValuePair in codeList)
			{
				codeWriter.AppendLine( "#region " + keyValuePair.Key );
				codeWriter.AppendLine( string.Join( "\r\n\r\n", keyValuePair.Value.ToArray() ) );
				codeWriter.AppendLine( "#endregion " );
			}
		}
	}
}