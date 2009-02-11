/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 04.02.2009 01:05
 *
 * File: GeneratorBase.cs
 * Remarks:
 * 
 * History:
 *   04.02.2009 01:05 - Create Wireframe
 *   04.02.2009 01:15 - Make constructor protected.
 *   11.02.2009 20:51 - Add InnerInvoke method.
 *   11.02.2009 21:41 - Finish first implementation of match methods.
 *
 *******************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using WolfGenerator.Core.Writer;
using System.Linq;

namespace WolfGenerator.Core
{
	public class GeneratorBase
	{
		private class MatchMethodData
		{
			public MethodInfo matchMethodInfo;

			public MethodInfo ruleMethodInfo;
		}

		private class MatchMethodDataCollection
		{
			public string methodName;

			public MatchMethodData[] methodDatas;

			public MethodInfo defaultMethodInfo;
		}

		private class MatchMethodCollection
		{
			public Dictionary<string, MatchMethodDataCollection> matchMethods;

			public MatchMethodDataCollection this[string methodName]
			{
				get
				{
					return this.matchMethods.ContainsKey( methodName ) ? this.matchMethods[methodName] : null;
				}
			}
		}

		private MatchMethodCollection matchMethodCollection;

		protected GeneratorBase()
		{
			var ruleMethods = (from m in this.GetType().GetMethods( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly )
			                   where m.GetAttribute<RuleMethodAttribute>() != null
			                   select new
			                          {
			                          	Method = m,
			                          	RuleMethodAttribute = m.GetAttribute<RuleMethodAttribute>()
			                          }).ToArray();

			var ruleMethodNames = (ruleMethods.Select( p => p.RuleMethodAttribute.Name ).Distinct()).ToArray();

			var matchMethods = (from m in this.GetType().GetMethods( BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly )
			                    where m.GetAttribute<MatchMethodAttribute>() != null
			                    select new
			                           {
			                           	Method = m,
			                           	MatchMethodAttribute = m.GetAttribute<MatchMethodAttribute>()
			                           }).ToArray();

			var matchData = (from methodName in ruleMethodNames
			                 select new MatchMethodDataCollection
			                        {
			                        	methodName = methodName,
			                        	methodDatas = (from rm in ruleMethods
			                        	               from mm in matchMethods
			                        	               where
			                        	               	rm.RuleMethodAttribute.Name == mm.MatchMethodAttribute.RuleMethodName &&
			                        	               	rm.RuleMethodAttribute.MatchName ==
			                        	               	mm.MatchMethodAttribute.MathcMethodName
			                        	               select new MatchMethodData
			                        	                      {
			                        	                      	ruleMethodInfo = rm.Method,
			                        	                      	matchMethodInfo = mm.Method
			                        	                      }).ToArray(),
			                        	defaultMethodInfo =
			                        		ruleMethods.SingleOrDefault( p => p.RuleMethodAttribute.MatchName == null &&
			                        		                                  p.RuleMethodAttribute.Name == methodName ).Method

			                        }).ToArray();

			this.matchMethodCollection = new MatchMethodCollection
			                             {
			                             	matchMethods = matchData.ToDictionary( collection => collection.methodName )
			                             };
		}

		public CodeWriter Invoke( string name, params object[] parameters )
		{
			if (matchMethodCollection[name] != null)
			{
				var methodCollection = this.matchMethodCollection[name];
				var matches = methodCollection.methodDatas;

				var isFinded = false;

				foreach (var data in matches)
				{
					if (!this.InnerInvoke<bool>( data.matchMethodInfo.Name, parameters )) continue;

					name = data.ruleMethodInfo.Name;
					isFinded = true;
					break;
				}

                if (!isFinded)
                {
                	if (methodCollection.defaultMethodInfo == null)
                		throw new Exception( "Can't find right method " + name );

                	name = methodCollection.defaultMethodInfo.Name;
                }
			}

			return InnerInvoke<CodeWriter>( name, parameters );
		}

		private T InnerInvoke<T>( string name, params object[] parameters )
		{
            return (T)this.GetType().InvokeMember( name, BindingFlags.Instance | BindingFlags.InvokeMethod |
			                                             BindingFlags.Public | BindingFlags.NonPublic,
			                                       Type.DefaultBinder, this, parameters );
		}
	}

	internal static class MethodInfoExtender
	{
		public static T GetAttribute<T>( this MethodInfo methodInfo )
			where T : Attribute
		{
			var attributes = methodInfo.GetCustomAttributes( typeof(T), false );

			if (attributes.Length == 0) return null;
			if (attributes.Length == 1) return (T)attributes[0];

			throw new ArgumentException( "Method " + methodInfo.Name + " has more than one attribute of type " + typeof(T), "methodInfo" );
		}

		public static string GetMethodHeader( this MethodInfo methodInfo )
		{
			return null;
		}
	}
}