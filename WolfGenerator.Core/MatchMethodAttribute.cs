/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 04.02.2009 01:35
 *
 * File: MatchMethodAttribute.cs
 * Remarks:
 * 
 * History:
 *   04.02.2009 01:37 - Create Wireframe
 *
 *******************************************************/

using System;

namespace WolfGenerator.Core
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MatchMethodAttribute : Attribute
	{
		private readonly string ruleMethodName;
		private readonly string fileName;

		public MatchMethodAttribute( string ruleMethodName, string fileName )
		{
			this.ruleMethodName = ruleMethodName;
			this.fileName = fileName;
		}

		public string RuleMethodName
		{
			get { return this.ruleMethodName; }
		}

		public string FileName
		{
			get { return this.fileName; }
		}
	}
}