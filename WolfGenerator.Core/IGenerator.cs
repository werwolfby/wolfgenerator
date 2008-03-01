/*****************************************************
 *
 * Created by: WerWolf
 * Created: 13.05.2007 13:18:57
 *
 * File: IGenerator.cs
 * Remarks:
 *
 *****************************************************/

using System;
using System.Collections.Generic;

namespace WolfGenerator.Core
{
	public class ParamInfo
	{
		public enum ParamModifier
		{
			IN,
			REF,
			OUT,
		}

		private string name;
		private string typeName;
		private ParamModifier modifier;

		public ParamInfo( string name, Type type ) : this( name, type.FullName, ParamModifier.IN )
		{
		}

		public ParamInfo( string name, string typeName ) : this( name, typeName, ParamModifier.IN )
		{
		}

		public ParamInfo( string name, string typeName, ParamModifier modifier )
		{
			this.name = name;
			this.typeName = typeName;
			this.modifier = modifier;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		public ParamModifier Modifier
		{
			get
			{
				return this.modifier;
			}
		}
	}

	public interface IGenerator
	{
		IDictionary<string, object> Session { get; }

		string IndentSymbol { get; }

		void AddProperty( string name, Type type, string setterCode, string getterCode );

		void AddEvent( string name, Type eventHadler );

		void AddMethod( string name, IList<ParamInfo> paramList, Type returnType, string code );

		void AddUsing( string namespaceName );

		void GenerateCode( string className, CodeWriter codeWriter );
	}
}