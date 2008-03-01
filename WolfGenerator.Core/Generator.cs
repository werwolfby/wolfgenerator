/*****************************************************
 *
 * Created by: WerWolf
 * Created: 13.05.2007 13:25:52
 *
 * File: Generator.cs
 * Remarks:
 *
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace WolfGenerator.Core
{
	public class Generator : IGenerator
	{
		#region Inner Classes
	    protected class PropertyData
		{
			#region Fields
			private readonly string name;
			private readonly Type type;
			private readonly string setter;
			private readonly string getter;
			#endregion

			#region Constructors
			public PropertyData( string name, Type type, string setter, string getter )
			{
				this.name = name;
				this.type = type;
				this.setter = setter;
				this.getter = getter;
			}
			#endregion

			#region Properties
			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public Type Type
			{
				get
				{
					return this.type;
				}
			}
			public string Setter
			{
				get
				{
					return this.setter;
				}
			}

			public string Getter
			{
				get
				{
					return this.getter;
				}
			}
			#endregion

			public string GenerateCode()
			{
				CodeWriter writer = new CodeWriter();
				writer.AppendLine( "public " + type.FullName + " " + name );
				writer.AppendLine( "{" );
				writer.Indent++;
				if (setter != null)
				{
					writer.AppendLine( "set" );
					writer.AppendLine( "{" );
					writer.Indent++;
					writer.AppendLine( setter );
					writer.Indent--;
					writer.AppendLine( "{" );
				}
				if (getter != null)
				{
					writer.AppendLine( "get" );
					writer.AppendLine( "{" );
					writer.Indent++;
					writer.AppendLine( getter );
					writer.Indent--;
					writer.AppendLine( "{" );
				}
				writer.Indent--;
				writer.AppendLine( "}");

				return writer.ToString();
			}
		}

	    protected class EventData
		{
			#region Fields
			private readonly string name;
			private readonly Type type;
			#endregion

			#region Constructor
			public EventData( string name, Type type )
			{
				this.name = name;
				this.type = type;
			}
			#endregion

			#region Properties
			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public Type Type
			{
				get
				{
					return this.type;
				}
			}
			#endregion

			public string GenerateCode()
			{
				StringBuilder stringBuilder = new StringBuilder();

				stringBuilder.AppendLine( "public event " + type.FullName + " " + name );

				return stringBuilder.ToString();
			}
		}

	    protected class MethodData
		{
			#region Fields
			private readonly string name;
			private readonly List<ParamInfo> paramList;
			private readonly Type returnType;
			private readonly string code;
			#endregion

			#region Constructors
			public MethodData( string name, IEnumerable<ParamInfo> paramList, Type returnType, string code )
			{
				this.name = name;
				if (paramList != null) this.paramList = new List<ParamInfo>( paramList );
				else this.paramList = new List<ParamInfo>();
				this.returnType = returnType;
				this.code = code;
			}
			#endregion

			#region Properties
			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public IList<ParamInfo> ParamList
			{
				get
				{
					return this.paramList;
				}
			}

			public Type ReturnType
			{
				get
				{
					return this.returnType;
				}
			}

			public string Code
			{
				get
				{
					return this.code;
				}
			}
			#endregion

			public string GenerateCode()
			{
				CodeWriter codeWriter = new CodeWriter();

				codeWriter.AppendLine( string.Format( "public {0} {1}( {2} )",
				                                        this.returnType.FullName,
				                                        this.name,
				                                        string.Join( ", ",
				                                                     Array.ConvertAll<ParamInfo, string>( this.paramList.ToArray(),
				                                                                                          delegate( ParamInfo input )
				                                                                                          	{
				                                                                                          		return input.TypeName + " " + input.Name;
				                                                                                          	} ) ) ) );
				codeWriter.AppendLine( "{" );
				codeWriter.Indent++;
				codeWriter.AppendLine( code );
				codeWriter.Indent--;
				codeWriter.AppendLine( "}" );

				return codeWriter.ToString();
			}
		}
		#endregion

		#region Fields
		private readonly string indentSymbol = "\t";
		private readonly Dictionary<string, object> session = new Dictionary<string, object>();

		private readonly List<PropertyData> propertyList = new List<PropertyData>();
		private readonly List<EventData> eventList = new List<EventData>();
		private readonly List<MethodData> methodList = new List<MethodData>();
		private readonly List<string> usingList = new List<string>();
		#endregion

		#region Properties
		public string IndentSymbol
		{
			get
			{
				return this.indentSymbol;
			}
		}

		public IDictionary<string, object> Session
		{
			get
			{
				return this.session;
			}
		}
		#endregion

		public void AddProperty( string name, Type type, string setterCode, string getterCode )
		{
			propertyList.Add( new PropertyData( name, type, setterCode, getterCode ) );
		}

		public void AddEvent( string name, Type eventHadler )
		{
			eventList.Add( new EventData( name, eventHadler ) );
		}

		public void AddMethod( string name, IList<ParamInfo> paramList, Type returnType, string code )
		{
			methodList.Add( new MethodData( name, paramList, returnType, code ) );
		}

		public void AddUsing( string namespaceName )
		{
			usingList.Add( namespaceName );
		}

		public void GenerateCode( string className, CodeWriter codeWriter )
		{
			List<string> regionList = new List<string>();

			if (propertyList.Count > 0)
			{
				List<string> propertyCodeList = new List<string>();

				foreach (PropertyData propertyData in propertyList)
				{
					propertyCodeList.Add( propertyData.GenerateCode() );
				}

				regionList.Add( "#region Properties\n" + string.Join( "\n", propertyCodeList.ToArray() ) + "#endregion\n" );
			}

			if (eventList.Count > 0)
			{
				List<string> eventCodeList = new List<string>();

				foreach (EventData eventData in eventList)
				{
					eventCodeList.Add( eventData.GenerateCode() );
				}

				regionList.Add( "#region Events\n" + string.Join( "", eventCodeList.ToArray() ) + "#endregion\n" );
			}

			if (methodList.Count > 0)
			{
				List<string> methodCodeList = new List<string>();

				foreach (MethodData methodData in methodList)
				{
					methodCodeList.Add( methodData.GenerateCode() );
				}

				regionList.Add( "#region Methods\n" + string.Join( "\n", methodCodeList.ToArray() ) + "#endregion\n" );
			}

			foreach (string usingName in usingList)
			{
				codeWriter.AppendLine( "using " + usingName + ";" );
			}

			codeWriter.AppendLine( "public class " + className );
			codeWriter.AppendLine( "{" );
			codeWriter.Indent++;
			codeWriter.AppendLine( string.Join( "\n", regionList.ToArray() ) );
			codeWriter.Indent--;
			codeWriter.AppendLine( "}" );
		}
	}
}