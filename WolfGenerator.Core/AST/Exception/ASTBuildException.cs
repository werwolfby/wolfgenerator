/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 15.02.2009 13:51
 *
 * File: ASTBuildException.cs
 * Remarks:
 * 
 * History:
 *   15.02.2009 13:51 - Create Wireframe
 *
 *******************************************************/

using System;
using System.Runtime.Serialization;

namespace WolfGenerator.Core.AST.Exception
{
	[Serializable]
	public abstract class ASTBuildException : System.Exception
	{
		public ASTBuildException() {}

		public ASTBuildException( string message ) : base( message ) {}

		public ASTBuildException( string message, System.Exception inner ) : base( message, inner ) {}

		protected ASTBuildException( SerializationInfo info, StreamingContext context ) : base( info, context ) {}
	}
}