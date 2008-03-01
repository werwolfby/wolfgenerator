/*****************************************************
 *
 * Created by: WerWolf
 * Created: 12.05.2007 2:44:08
 *
 * File: GeneratedClass.cs
 * Remarks:
 *
 *****************************************************/

using System.Collections.Generic;

namespace Example.WolfGenerator
{
	public class GeneratedClass
	{
		private string name;
		private List<string> attributeList = new List<string>();
		private List<string> entityList = new List<string>();

		public GeneratedClass( string name, IEnumerable<string> attributeEnum, IEnumerable<string> entityEnum )
		{
			this.name = name;

			this.attributeList = new List<string>( attributeEnum );
			this.entityList = new List<string>( entityEnum );
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public List<string> AttributeList
		{
			get
			{
				return this.attributeList;
			}
		}

		public List<string> EntityList
		{
			get
			{
				return this.entityList;
			}
		}
	}
}