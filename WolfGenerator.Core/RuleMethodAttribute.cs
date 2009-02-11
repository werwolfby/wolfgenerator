/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 11.02.2009 20:53
 *
 * File: RuleMethodAttribute.cs
 * Remarks:
 * 
 * History:
 *   11.02.2009 20:53 - Create Wireframe
 *   11.02.2009 21:10 - Add MatchName method.
 *
 *******************************************************/

using System;

namespace WolfGenerator.Core
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class RuleMethodAttribute : Attribute
	{
		private readonly string name;
		private readonly string matchName;
		private readonly string fileName;

		public RuleMethodAttribute( string name, string matchName, string fileName )
		{
			this.name = name;
			this.matchName = matchName;
			this.fileName = fileName;
		}

		public string Name
		{
			get { return this.name; }
		}

		public string MatchName
		{
			get { return this.matchName; }
		}

		public string FileName
		{
			get { return this.fileName; }
		}
	}
}