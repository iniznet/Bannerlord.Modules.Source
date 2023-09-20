using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using TaleWorlds.MountAndBlade.Network.Gameplay.Perks;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MPPerkCondition
	{
		static MPPerkCondition()
		{
			foreach (Type type in from t in PerkAssemblyCollection.GetPerkAssemblyTypes()
				where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MPPerkCondition))
				select t)
			{
				FieldInfo field = type.GetField("StringType", BindingFlags.Static | BindingFlags.NonPublic);
				string text = (string)((field != null) ? field.GetValue(null) : null);
				MPPerkCondition.Registered.Add(text, type);
			}
		}

		public virtual MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.None;
			}
		}

		public virtual bool IsPeerCondition
		{
			get
			{
				return false;
			}
		}

		public abstract bool Check(MissionPeer peer);

		public abstract bool Check(Agent agent);

		protected virtual bool IsGameModesValid(List<string> gameModes)
		{
			return true;
		}

		protected abstract void Deserialize(XmlNode node);

		public static MPPerkCondition CreateFrom(List<string> gameModes, XmlNode node)
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
			MPPerkCondition mpperkCondition = (MPPerkCondition)Activator.CreateInstance(MPPerkCondition.Registered[text2], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture);
			mpperkCondition.Deserialize(node);
			return mpperkCondition;
		}

		protected static Dictionary<string, Type> Registered = new Dictionary<string, Type>();

		[Flags]
		public enum PerkEventFlags
		{
			None = 0,
			MoraleChange = 1,
			FlagCapture = 2,
			FlagRemoval = 4,
			HealthChange = 8,
			AliveBotCountChange = 16,
			PeerControlledAgentChange = 32,
			BannerPickUp = 64,
			BannerDrop = 128,
			SpawnEnd = 256,
			MountHealthChange = 512,
			MountChange = 1024,
			AgentEventsMask = 1576
		}
	}
}
