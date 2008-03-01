/*****************************************************
 *
 * Created by: WerWolf
 * Created: 11.05.2007 13:44:24
 *
 * File: RuleAnalizer.cs
 * Remarks:
 *
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace WolfGenerator.Core
{
	public abstract class RuleAnalizer
	{
		protected class RuleAnalizerData
		{
			private RuleAnalizerAttribute analizerAttribute;
			private Type type;

			public RuleAnalizerData( RuleAnalizerAttribute analizerAttribute, Type type )
			{
				this.analizerAttribute = analizerAttribute;
				this.type = type;
			}

			public RuleAnalizerAttribute AnalizerAttribute
			{
				get
				{
					return this.analizerAttribute;
				}
			}

			public Type Type
			{
				get
				{
					return this.type;
				}
			}
		}

		protected readonly static List<RuleAnalizerData> analizerDataList = new List<RuleAnalizerData>();

		static RuleAnalizer()
		{
			foreach (Type type in Assembly.GetCallingAssembly().GetExportedTypes())
			{
				if (type.IsSubclassOf( typeof(RuleAnalizer) ))
				{
					object[] attributes = type.GetCustomAttributes( typeof(RuleAnalizerAttribute), false );

					foreach (RuleAnalizerAttribute analizerAttribute in attributes)
					{
						analizerDataList.Add( new RuleAnalizerData( analizerAttribute, type ) );
					}
				}
			}
		}

		public abstract RuleNode Analize( List<RuleString> ruleStrings, ref int index );

		public static RuleAnalizer FindAnAnalizer( RuleString ruleString )
		{
			Type analizerType = null;

			foreach (RuleAnalizerData analizerData in analizerDataList)
			{
				if (analizerData.AnalizerAttribute.Check( ruleString ))
				{
					analizerType = analizerData.Type;
					break;
				}
			}

			if (analizerType == null) throw new Exception( "Can't find RuleAnalizer for rulestring: " + ruleString.Text );

			return (RuleAnalizer)Activator.CreateInstance( analizerType );
		}
	}
}
