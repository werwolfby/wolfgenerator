/*******************************************************
 *
 * Created by: Alexander Puzynia aka WerWolf
 * Created: 21.04.2012 20:25
 *
 * File: MethodInfoExtender.cs
 * Remarks:
 * 
 * History:
 *   21.04.2012 20:25 - Moved to separate file
 *
 *******************************************************/

using System;
using System.Reflection;

namespace WolfGenerator.Core.Invoker
{
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