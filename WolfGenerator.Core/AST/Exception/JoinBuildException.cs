/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 15.02.2009 13:54
 *
 * File: JoinBuildException.cs
 * Remarks:
 * 
 * History:
 *   15.02.2009 13:54 - Create Wireframe
 *
 *******************************************************/

using System;
using System.Runtime.Serialization;

namespace WolfGenerator.Core.AST.Exception
{
	[Serializable]
	public class JoinBuildException : ASTBuildException
	{
		public JoinBuildException() {}

		public JoinBuildException( string message ) : base( message ) {}

		public JoinBuildException( string message, System.Exception inner ) : base( message, inner ) {}

		protected JoinBuildException(
			SerializationInfo info,
			StreamingContext context ) : base( info, context ) {}
	}
}