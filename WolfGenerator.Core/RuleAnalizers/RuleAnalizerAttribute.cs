/*****************************************************
 *
 * Created by: 
 * Created: 11.05.2007 13:44:53
 *
 * File: RuleAnalizerAttribute.cs
 * Remarks:
 *
 *****************************************************/

using System;

namespace WolfGenerator.Core
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class RuleAnalizerAttribute : Attribute
	{
		private string name;

		public RuleAnalizerAttribute( string name )
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool Check( RuleString ruleString )
		{
			if (!(ruleString is CommandRuleString)) return false;

			return ruleString.Text.Trim().ToUpper().StartsWith( name.ToUpper() );
		}
	}
}