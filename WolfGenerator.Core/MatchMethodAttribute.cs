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
 *   11.02.2009 21:?? - Add MatchMethodName property.
 *
 *******************************************************/

using System;

namespace WolfGenerator.Core
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MatchMethodAttribute : Attribute
	{
		private readonly string ruleMethodName;
		private readonly string mathcMethodName;
		private readonly string fileName;

		public MatchMethodAttribute( string ruleMethodName, string mathcMethodName, string fileName )
		{
			this.ruleMethodName = ruleMethodName;
			this.mathcMethodName = mathcMethodName;
			this.fileName = fileName;
		}

		public string RuleMethodName
		{
			get { return this.ruleMethodName; }
		}

		public string MathcMethodName
		{
			get { return this.mathcMethodName; }
		}

		public string FileName
		{
			get { return this.fileName; }
		}
	}
}