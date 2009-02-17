/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 18.02.2009 00:29
 *
 * File: WriterException.cs
 * Remarks:
 * 
 * History:
 *   18.02.2009 00:29 - Create Wireframe
 *
 *******************************************************/

using System;
using System.Runtime.Serialization;

namespace WolfGenerator.Core.Writer.Exception
{
	[Serializable]
	public abstract class WriterException : System.Exception
	{
		protected WriterException() {}

		protected WriterException( string message ) : base( message ) {}

		protected WriterException( string message, System.Exception inner ) : base( message, inner ) {}

		protected WriterException( SerializationInfo info, StreamingContext context ) : base( info, context ) {}
	}
}