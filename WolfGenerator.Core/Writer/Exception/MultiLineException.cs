/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 18.02.2009 00:32
 *
 * File: MultiLineException.cs
 * Remarks:
 * 
 * History:
 *   18.02.2009 00:32 - Create Wireframe
 *
 *******************************************************/

using System;
using System.Runtime.Serialization;

namespace WolfGenerator.Core.Writer.Exception
{
	[Serializable]
	public class MultiLineException : LineException
	{
		private readonly string value;

		public MultiLineException( string value ) : base( "String contain more than one line" )
		{
			this.value = value;
		}

		public string Value
		{
			get { return this.value; }
		}

		protected MultiLineException( SerializationInfo info, StreamingContext context, string value ) : base( info, context )
		{
			this.value = value;
		}
	}
}