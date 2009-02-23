/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 23.02.2009 23:14
 *
 * File: AppendWriterException.cs
 * Remarks:
 * 
 * History:
 *   23.02.2009 23:14 - Create Wireframe
 *
 *******************************************************/

using System;
using System.Runtime.Serialization;

namespace WolfGenerator.Core.Writer.Exception
{
	[Serializable]
	public class UnexpectedAppendTypeWriterException : WriterException
	{
		private readonly AppendType appendType;

		public UnexpectedAppendTypeWriterException( AppendType appendType )
		{
			this.appendType = appendType;
		}

		public UnexpectedAppendTypeWriterException( string message, AppendType appendType ) : base( message )
		{
			this.appendType = appendType;
		}

		public UnexpectedAppendTypeWriterException( string message, System.Exception inner, AppendType appendType )
			: base( message, inner )
		{
			this.appendType = appendType;
		}

		protected UnexpectedAppendTypeWriterException( SerializationInfo info, StreamingContext context, AppendType appendType )
			: base( info, context )
		{
			this.appendType = appendType;
		}

		public AppendType AppendType
		{
			get { return this.appendType; }
		}
	}
}