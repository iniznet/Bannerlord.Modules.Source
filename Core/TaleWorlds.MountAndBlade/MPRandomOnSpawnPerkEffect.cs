using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200031C RID: 796
	public abstract class MPRandomOnSpawnPerkEffect : MPOnSpawnPerkEffectBase
	{
		// Token: 0x06002B00 RID: 11008 RVA: 0x000A89F4 File Offset: 0x000A6BF4
		static MPRandomOnSpawnPerkEffect()
		{
			foreach (Type type in from t in Assembly.GetAssembly(typeof(MPRandomOnSpawnPerkEffect)).GetTypes()
				where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MPRandomOnSpawnPerkEffect))
				select t)
			{
				FieldInfo field = type.GetField("StringType", BindingFlags.Static | BindingFlags.NonPublic);
				string text = (string)((field != null) ? field.GetValue(null) : null);
				MPRandomOnSpawnPerkEffect.Registered.Add(text, type);
			}
		}

		// Token: 0x06002B01 RID: 11009 RVA: 0x000A8A94 File Offset: 0x000A6C94
		public static MPRandomOnSpawnPerkEffect CreateFrom(XmlNode node)
		{
			string text;
			if (node == null)
			{
				text = null;
			}
			else
			{
				XmlAttributeCollection attributes = node.Attributes;
				if (attributes == null)
				{
					text = null;
				}
				else
				{
					XmlAttribute xmlAttribute = attributes["type"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			MPRandomOnSpawnPerkEffect mprandomOnSpawnPerkEffect = (MPRandomOnSpawnPerkEffect)Activator.CreateInstance(MPRandomOnSpawnPerkEffect.Registered[text2], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture);
			mprandomOnSpawnPerkEffect.Deserialize(node);
			return mprandomOnSpawnPerkEffect;
		}

		// Token: 0x0400105D RID: 4189
		protected static Dictionary<string, Type> Registered = new Dictionary<string, Type>();
	}
}
