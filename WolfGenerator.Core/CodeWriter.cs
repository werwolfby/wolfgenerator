using System.IO;
using System.Text;

namespace WolfGenerator.Core
{
	public class CodeWriter
	{
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private int indent;
		private static string indentSymbol = "\t";

		private string indentValue;
		
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
			text = SetIndent( indent, text );
			if (text.EndsWith( "\r\n" )) text = text.Substring( 0, text.Length - 2 );
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
			var currentIndent = GetIndent( indentCount );

			var stringReader = new StringReader( code );
			var currentStringBuilder = new StringBuilder();

			string line;

			var count = 0;

			while ((line = stringReader.ReadLine()) != null)
			{
				if (count++ == 0 && indentValue != null) currentStringBuilder.AppendLine( line );
				else currentStringBuilder.AppendLine( currentIndent + line );
			}

			return currentStringBuilder.ToString();
		}

		private string GetIndent( int indentCount )
		{
			if (indentValue != null) return indentValue;

			var indentBuilder = new StringBuilder();
			for (var i = 0; i < indentCount; i++)
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
