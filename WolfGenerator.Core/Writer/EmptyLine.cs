/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 25.01.2009 22:14
 *
 * File: EmptyLine.cs
 * Remarks:
 * 
 * History:
 *   25.01.2009 22:14 - Create Wireframe
 *
 *******************************************************/

namespace WolfGenerator.Core.Writer
{
	public sealed class EmptyLine : Line
	{
		public static readonly EmptyLine Instance = new EmptyLine();

		private EmptyLine() : base( 0 ) {}

		public override string GetText()
		{
			return string.Empty;
		}

		public override Line Clone( int newIndent )
		{
			return Instance;
		}
	}
}