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

		public MatchMethodAttribute( string ruleMethodName )
		{
			this.ruleMethodName = ruleMethodName;
		}

		public string RuleMethodName
		{
			get { return this.ruleMethodName; }
		}
	}
}