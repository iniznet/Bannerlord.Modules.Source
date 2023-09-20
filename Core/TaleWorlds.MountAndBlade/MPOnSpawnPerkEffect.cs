using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000312 RID: 786
	public abstract class MPOnSpawnPerkEffect : MPOnSpawnPerkEffectBase
	{
		// Token: 0x06002A77 RID: 10871 RVA: 0x000A5774 File Offset: 0x000A3974
		static MPOnSpawnPerkEffect()
		{
			foreach (Type type in from t in Assembly.GetAssembly(typeof(MPOnSpawnPerkEffect)).GetTypes()
				where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MPOnSpawnPerkEffect))
				select t)
			{
				FieldInfo field = type.GetField("StringType", BindingFlags.Static | BindingFlags.NonPublic);
				string text = (string)((field != null) ? field.GetValue(null) : null);
				MPOnSpawnPerkEffect.Registered.Add(text, type);
			}
		}

		// Token: 0x06002A78 RID: 10872 RVA: 0x000A5814 File Offset: 0x000A3A14
		public static MPOnSpawnPerkEffect CreateFrom(XmlNode node)
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
			MPOnSpawnPerkEffect mponSpawnPerkEffect = (MPOnSpawnPerkEffect)Activator.CreateInstance(MPOnSpawnPerkEffect.Registered[text2], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture);
			mponSpawnPerkEffect.Deserialize(node);
			return mponSpawnPerkEffect;
		}

		// Token: 0x04001044 RID: 4164
		protected static Dictionary<string, Type> Registered = new Dictionary<string, Type>();
	}
}
