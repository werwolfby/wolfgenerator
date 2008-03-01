using System.IO;
using System.Text;

namespace WolfGenerator.Core
{
	public class CodeWriter
	{
		private StringBuilder stringBuilder = new StringBuilder();
		private int indent = 0;
		private static string indentSymbol = "\t";

		private string indentValue = null;
		
		public int Indent
		{
			get
			{
				return this.indent;
			}
			set
			{
				this.indent = value;
			}
		}

		public static string IndentSymbol
		{
			get
			{
				return indentSymbol;
			}
			set
			{
				indentSymbol = value;
			}
		}

		public string IndentString
		{
			get
			{
				return GetIndent( indent );
			}
		}

		public void AppendIndent()
		{
			stringBuilder.Append( IndentString );
		}

		public void Append( string text )
		{
			stringBuilder.Append( text );
			indentValue = text;
		}

		public void AppendLine( string text )
		{
			text = SetIndent( indent, text );
			stringBuilder.Append( text );
			indentValue = null;
		}

		public void AppendFormatLine( string text, params object[] args )
		{
			text = SetIndent( indent, text );
			stringBuilder.AppendFormat( text, args );
			indentValue = null;
		}

		public void AppendLine()
		{
			stringBuilder.AppendLine();
			indentValue = null;
		}

		private string SetIndent( int indentCount, string code )
		{
			//if (indentCount == 0) return code + "\n";

			string indent = GetIndent( indentCount );

			StringReader stringReader = new StringReader( code );
			StringBuilder stringBuilder = new StringBuilder();

			string line = null;

			int count = 0;

			while ((line = stringReader.ReadLine()) != null)
			{
				if (count++ == 0 && indentValue != null) stringBuilder.AppendLine( line );
				else stringBuilder.AppendLine( indent + line );
			}

			return stringBuilder.ToString();
		}

		private string GetIndent( int indentCount )
		{
			if (indentValue != null) return indentValue;

			StringBuilder indentBuilder = new StringBuilder();
			for (int i = 0; i < indentCount; i++)
			{
				indentBuilder.Append( indentSymbol );
			}
			return indentBuilder.ToString();
		}

		public override string ToString()
		{
			return this.stringBuilder.ToString();
		}
	}
}
