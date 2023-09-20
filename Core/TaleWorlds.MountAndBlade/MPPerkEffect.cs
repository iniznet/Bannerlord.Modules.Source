using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000317 RID: 791
	public abstract class MPPerkEffect : MPPerkEffectBase
	{
		// Token: 0x06002A92 RID: 10898 RVA: 0x000A5C3C File Offset: 0x000A3E3C
		static MPPerkEffect()
		{
			foreach (Type type in from t in Assembly.GetAssembly(typeof(MPPerkEffect)).GetTypes()
				where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MPPerkEffect))
				select t)
			{
				FieldInfo field = type.GetField("StringType", BindingFlags.Static | BindingFlags.NonPublic);
				string text = (string)((field != null) ? field.GetValue(null) : null);
				MPPerkEffect.Registered.Add(text, type);
			}
		}

		// Token: 0x06002A93 RID: 10899 RVA: 0x000A5CDC File Offset: 0x000A3EDC
		public static MPPerkEffect CreateFrom(XmlNode node)
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
			MPPerkEffect mpperkEffect = (MPPerkEffect)Activator.CreateInstance(MPPerkEffect.Registered[text2], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture);
			mpperkEffect.Deserialize(node);
			return mpperkEffect;
		}

		// Token: 0x04001047 RID: 4167
		protected static Dictionary<string, Type> Registered = new Dictionary<string, Type>();
	}
}
