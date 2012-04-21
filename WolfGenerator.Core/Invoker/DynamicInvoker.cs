﻿/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 21.04.2012 20:25
 *
 * File: DynamicInvoker.cs
 * Remarks:
 * 
 * History:
 *   21.04.2012 20:25 - Create Wireframe
 *   21.04.2012 20:30 - [*] Move code from [GeneratorBase].
 *   21.04.2012 20:32 - [!] Use [instance] field for invoking.
 *   21.04.2012 20:41 - [*] Remove direct use of [RuleMethodAttribute] attribute in [RuleMethodWithAttribute] class.
 *   21.04.2012 20:46 - [*] Remove direct use of [MatchMethodAttribute] attribute in [MatchMethodWithAttribute] class.
 *   21.04.2012 20:54 - [+] Add [instanceType] field.
 *
 *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WolfGenerator.Core.Invoker
{
	public class DynamicInvoker
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

		private class RuleMethodWithAttribute
		{
			public string MethodName { get; set; }

			public string RuleName { get; set; }

			public string MatchName { get; set; }
		}

		private class MatchMethodWithAttribute
		{
			public string MethodName { get; set; }

			public string RuleName { get; set; }

			public string MatchName { get; set; }
		}

		private readonly MatchMethodCollection matchMethodCollection;

		private const BindingFlags INVOKE_MEMBER_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic;

		private readonly object instance;
		private readonly Type instanceType;

		public DynamicInvoker( object instance )
		{
			this.instance = instance;
			this.instanceType = this.instance.GetType();

			const BindingFlags ruleMethodBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
			const BindingFlags matchMethodBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			var ruleMethods = instanceType
				.GetMethods( ruleMethodBindingFlags )
				.Select( m => new
				              {
				              	Method = m,
				              	RuleMethodAttribute = m.GetAttribute<RuleMethodAttribute>(),
				              } )
				.Where( e => e.RuleMethodAttribute != null )
				.Select( m => new RuleMethodWithAttribute
				              {
				              	MethodName = m.Method.Name,
				              	RuleName = m.RuleMethodAttribute.Name,
				              	MatchName = m.RuleMethodAttribute.MatchName,
				              } )
				.ToArray();

			var ruleMethodNames = (ruleMethods.Select( p => p.RuleName ).Distinct()).ToArray();

			var matchMethods = instanceType
				.GetMethods( matchMethodBindingFlags )
				.Select( m => new
				              {
				              	Method = m,
				              	MatchMethodAttribute = m.GetAttribute<MatchMethodAttribute>(),
				              } )
				.Where( e => e.MatchMethodAttribute != null )
				.Select( m => new MatchMethodWithAttribute
				              {
				              	MethodName = m.Method.Name,
				              	RuleName = m.MatchMethodAttribute.RuleMethodName,
				              	MatchName = m.MatchMethodAttribute.MathcMethodName,
				              } )
				.ToArray();

			var matchData = (from methodName in ruleMethodNames
			                 select new MatchMethodDataCollection
			                        {
			                        	methodName = methodName,
			                        	methodDatas = GetMethodDatas( ruleMethods, matchMethods, methodName ),
			                        }).ToArray();

			this.matchMethodCollection = new MatchMethodCollection
			                             {
			                             	matchMethods = matchData.ToDictionary( collection => collection.methodName )
			                             };
		}

		private static MatchMethodData[] GetMethodDatas( IEnumerable<RuleMethodWithAttribute> ruleMethods, IEnumerable<MatchMethodWithAttribute> matchMethods, string methodName )
		{
			return (from rm in ruleMethods
			        from mm in matchMethods
			        where
			        	rm.RuleName == methodName &&
			        	rm.RuleName == mm.RuleName &&
			        	rm.MatchName == mm.MatchName
			        select new MatchMethodData
			               {
			               	ruleMethodName = rm.MethodName,
			               	matchMethodName = mm.MethodName
			               }).ToArray();
		}

		public T Invoke<T>( string name, params object[] parameters )
		{
			if (matchMethodCollection[name] != null)
			{
				var methodCollection = this.matchMethodCollection[name];
				var matches = methodCollection.methodDatas;

				foreach (var data in matches)
				{
					if (!CheckParams(data.matchMethodName, parameters) || !this.InnerInvoke<bool>( data.matchMethodName, parameters )) continue;

					name = data.ruleMethodName;
					break;
				}
			}

			return InnerInvoke<T>( name, parameters );
		}

		private bool CheckParams( string name, object[] parameters )
		{
			var method = this.instanceType.GetMethod( name, INVOKE_MEMBER_BINDING_FLAGS );
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
			return (T)this.instanceType.InvokeMember( name, INVOKE_MEMBER_BINDING_FLAGS,
			                                           Type.DefaultBinder, instance, parameters );
		}
	}
}