/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 18.02.2009 00:30
 *
 * File: LineException.cs
 * Remarks:
 * 
 * History:
 *   18.02.2009 00:30 - Create Wireframe
 *
 *******************************************************/

using System;
using System.Runtime.Serialization;

namespace WolfGenerator.Core.Writer.Exception
{
	[Serializable]
	public abstract class LineException : System.Exception
	{
		protected LineException() {}

		protected LineException( string message ) : base( message ) {}

		protected LineException( string message, System.Exception inner ) : base( message, inner ) {}

		protected LineException( SerializationInfo info, StreamingContext context ) : base( info, context ) {}
	}
}