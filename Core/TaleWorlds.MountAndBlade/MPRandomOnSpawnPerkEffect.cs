using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using TaleWorlds.MountAndBlade.Network.Gameplay.Perks;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MPRandomOnSpawnPerkEffect : MPOnSpawnPerkEffectBase
	{
		static MPRandomOnSpawnPerkEffect()
		{
			foreach (Type type in from t in PerkAssemblyCollection.GetPerkAssemblyTypes()
				where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MPRandomOnSpawnPerkEffect))
				select t)
			{
				FieldInfo field = type.GetField("StringType", BindingFlags.Static | BindingFlags.NonPublic);
				string text = (string)((field != null) ? field.GetValue(null) : null);
				MPRandomOnSpawnPerkEffect.Registered.Add(text, type);
			}
		}

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

		protected static Dictionary<string, Type> Registered = new Dictionary<string, Type>();
	}
}
