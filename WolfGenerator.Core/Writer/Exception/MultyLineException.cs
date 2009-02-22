/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 18.02.2009 00:32
 *
 * File: MultyLineException.cs
 * Remarks:
 * 
 * History:
 *   18.02.2009 00:32 - Create Wireframe
 *   23.02.2009 00:40 - Rename from `MultiLineException` ti `MultyLineException`.
 *
 *******************************************************/

using System;
using System.Runtime.Serialization;

namespace WolfGenerator.Core.Writer.Exception
{
	[Serializable]
	public class MultyLineException : LineException
	{
		private readonly string value;

		public MultyLineException( string value ) : base( "String contain more than one line" )
		{
			this.value = value;
		}

		public string Value
		{
			get { return this.value; }
		}

		protected MultyLineException( SerializationInfo info, StreamingContext context, string value ) : base( info, context )
		{
			this.value = value;
		}
	}
}