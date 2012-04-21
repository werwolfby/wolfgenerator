/*
 * 
 *  Code base on http://www.amazedsaint.com/2010/05/accessprivatewrapper-c-40-dynamic.html
 * 
 * */

using System;
using System.Dynamic;
using System.Reflection;
using System.Linq;

namespace UnitTest.WolfGenerator
{
	public class AccessPrivateWrapper : DynamicObject
	{
		/// <summary>
		/// The object we are going to wrap
		/// </summary>
		private readonly object instance;

		private readonly Type instanceType;

		/// <summary>
		/// Specify the flags for accessing members
		/// </summary>
		private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance
		                                   | BindingFlags.Static | BindingFlags.Public;

		/// <summary>
		/// Create a simple private wrapper
		/// </summary>
		public AccessPrivateWrapper(object instance)
		{
			this.instance = instance;
			this.instanceType = instance.GetType();
		}

		public AccessPrivateWrapper(Type instanceType)
		{
			this.instance = null;
			this.instanceType = instanceType;
		}

		/// <summary>
		/// Try invoking a method
		/// </summary>
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var methodInfo = this.instanceType.GetMethod(binder.Name, Flags);

			if (methodInfo != null)
			{
				result = methodInfo.Invoke(this.instance, args);
				if (methodInfo.ReturnType != typeof(void)) result = CastValue(result, methodInfo.ReturnType);
				return true;
			}

			return base.TryInvokeMember(binder, args, out result);
		}

		/// <summary>
		/// Tries to get a property or field with the given name
		/// </summary>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			//Try getting a property of that name
			var propertyInfo = this.instanceType.GetProperty(binder.Name, Flags);
			if (propertyInfo != null)
			{
				result = CastValue(propertyInfo.GetValue(this.instance, null), propertyInfo.PropertyType);
				return true;
			}

			//Try getting a field of that name
			var fieldInfo = this.instanceType.GetField(binder.Name, Flags);
			if (fieldInfo != null)
			{
				result = CastValue(fieldInfo.GetValue(this.instance), fieldInfo.FieldType);
				return true;
			}

			return base.TryGetMember(binder, out result);
		}

		/// <summary>
		/// Tries to set a property or field with the given name
		/// </summary>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			var propertyInfo = this.instanceType.GetProperty(binder.Name, Flags);
			if (propertyInfo != null)
			{
				propertyInfo.SetValue(this.instance, value, null);
				return true;
			}

			var fieldInfo = this.instanceType.GetField(binder.Name, Flags);
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(this.instance, value);
				return true;
			}

			return base.TrySetMember(binder, value);
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			var testType = binder.Type;
			if (IsNullableType(testType))
				testType = Nullable.GetUnderlyingType(testType);

			if (this.instanceType == testType || this.instanceType.IsSubclassOf(testType))
			{
				result = this.instance;
				return true;
			}
			if (testType.IsInterface && this.instanceType.GetInterfaces().Contains(testType))
			{
				result = this.instance;
				return true;
			}

			return base.TryConvert(binder, out result);
		}

		private static object CastValue(object result, Type type)
		{
			if (result != null && !type.IsPrimitive)
				result = new AccessPrivateWrapper(result);
			return result;
		}

		public static bool IsNullableType( Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
		}
	}
}