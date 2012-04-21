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
 *   12.02.2009 20:55 - Change field type of some inner classes from MethodInfo to string, 
 *                      because we use only name of method.
 *   21.04.2021 08:26 - [+] Add [CheckParams] method, for check [MatchMethod] parameters before call the match method.
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
			public string matchMethodName;

			public string ruleMethodName;
		}

		private class MatchMethodDataCollection
		{
			public string methodName;

			public MatchMethodData[] methodDatas;

			public string defaultMethodName;
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

		public class RuleMethodWithAttribute
		{
			public string MethodName { get; set; }

			public RuleMethodAttribute RuleMethodAttribute { get; set; }
		}

		public class MatchMethodWithAttribute
		{
			public string MethodName { get; set; }

			public MatchMethodAttribute MatchMethodAttribute { get; set; }
		}

		private readonly MatchMethodCollection matchMethodCollection;

		private const BindingFlags INVOKE_MEMBER_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic;

		protected GeneratorBase()
		{
			const BindingFlags ruleMethodBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
			const BindingFlags matchMethodBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			var ruleMethods = (from m in this.GetType().GetMethods( ruleMethodBindingFlags )
				 where m.GetAttribute<RuleMethodAttribute>() != null
				 select new RuleMethodWithAttribute
				        {
				        	MethodName = m.Name,
				        	RuleMethodAttribute = m.GetAttribute<RuleMethodAttribute>()
				        }).ToArray();

			var ruleMethodNames = (ruleMethods.Select( p => p.RuleMethodAttribute.Name ).Distinct()).ToArray();

			var matchMethods = (from m in this.GetType().GetMethods( matchMethodBindingFlags )
			                    where m.GetAttribute<MatchMethodAttribute>() != null
			                    select new MatchMethodWithAttribute
			                           {
			                           	MethodName = m.Name,
			                           	MatchMethodAttribute = m.GetAttribute<MatchMethodAttribute>()
			                           }).ToArray();

		    var matchData = (from methodName in ruleMethodNames
		                     select new MatchMethodDataCollection
		                            {
		                                methodName = methodName,
		                                methodDatas = (from rm in ruleMethods
		                                               from mm in matchMethods
		                                               where
		                                                   rm.RuleMethodAttribute.Name ==
		                                                   mm.MatchMethodAttribute.RuleMethodName &&
		                                                   rm.RuleMethodAttribute.MatchName ==
		                                                   mm.MatchMethodAttribute.MathcMethodName
		                                               select new MatchMethodData
		                                                      {
		                                                          ruleMethodName = rm.MethodName,
		                                                          matchMethodName = mm.MethodName
		                                                      }).ToArray(),
		                                defaultMethodName =
		                                    ruleMethods.SingleOrDefault( p => p.RuleMethodAttribute.MatchName == null &&
		                                                                      p.RuleMethodAttribute.Name == methodName )
		                                    .NullOrProperty( e => e.MethodName )

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
					if (!CheckParams(data.matchMethodName, parameters) || !this.InnerInvoke<bool>( data.matchMethodName, parameters )) continue;

					name = data.ruleMethodName;
					isFinded = true;
					break;
				}

                if (!isFinded)
                {
                	if (methodCollection.defaultMethodName == null)
                		throw new Exception( "Can't find right method " + name );

                	name = methodCollection.defaultMethodName;
                }
			}

			return InnerInvoke<CodeWriter>( name, parameters );
		}

		private bool CheckParams( string name, object[] parameters )
		{
			var method = this.GetType().GetMethod( name, INVOKE_MEMBER_BINDING_FLAGS );
			if (method == null) throw new Exception( "Can't find method: " + name );
			var methodParameters = method.GetParameters();
			// TODO: Check parameters count depends on default parameters of method
			if (methodParameters.Length != parameters.Length) return false;
			for (var i = 0; i < methodParameters.Length; i++)
			{
				var parameterValue = parameters[i];
				var parameter = methodParameters[i];
				if (parameterValue == null)
				{
					// Null can be assigned to value type
					if (parameter.ParameterType.IsValueType) return false;
				}
				else
				{
					var parameterType = parameterValue.GetType();
					if (!(parameter.ParameterType.IsAssignableFrom(parameterType) ||
						parameter.ParameterType == parameterType)) return false;
				}
			}
			return true;
		}

		private T InnerInvoke<T>( string name, params object[] parameters )
		{
            return (T)this.GetType().InvokeMember( name, INVOKE_MEMBER_BINDING_FLAGS,
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

        public static TItem NullOrProperty<T, TItem>( this T item, Func<T, TItem> func )
            where T : class 
            where TItem : class
        {
            return item == null ? null : func( item );
        }
	}
}