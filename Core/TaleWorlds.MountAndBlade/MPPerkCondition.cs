using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000315 RID: 789
	public abstract class MPPerkCondition
	{
		// Token: 0x06002A86 RID: 10886 RVA: 0x000A5954 File Offset: 0x000A3B54
		static MPPerkCondition()
		{
			foreach (Type type in from t in Assembly.GetAssembly(typeof(MPPerkCondition)).GetTypes()
				where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MPPerkCondition))
				select t)
			{
				FieldInfo field = type.GetField("StringType", BindingFlags.Static | BindingFlags.NonPublic);
				string text = (string)((field != null) ? field.GetValue(null) : null);
				MPPerkCondition.Registered.Add(text, type);
			}
		}

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06002A87 RID: 10887 RVA: 0x000A59F4 File Offset: 0x000A3BF4
		public virtual MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.None;
			}
		}

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06002A88 RID: 10888 RVA: 0x000A59F7 File Offset: 0x000A3BF7
		public virtual bool IsPeerCondition
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002A89 RID: 10889
		public abstract bool Check(MissionPeer peer);

		// Token: 0x06002A8A RID: 10890
		public abstract bool Check(Agent agent);

		// Token: 0x06002A8B RID: 10891 RVA: 0x000A59FA File Offset: 0x000A3BFA
		protected virtual bool IsGameModesValid(List<string> gameModes)
		{
			return true;
		}

		// Token: 0x06002A8C RID: 10892
		protected abstract void Deserialize(XmlNode node);

		// Token: 0x06002A8D RID: 10893 RVA: 0x000A5A00 File Offset: 0x000A3C00
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

		// Token: 0x04001046 RID: 4166
		protected static Dictionary<string, Type> Registered = new Dictionary<string, Type>();

		// Token: 0x02000631 RID: 1585
		[Flags]
		public enum PerkEventFlags
		{
			// Token: 0x04002018 RID: 8216
			None = 0,
			// Token: 0x04002019 RID: 8217
			MoraleChange = 1,
			// Token: 0x0400201A RID: 8218
			FlagCapture = 2,
			// Token: 0x0400201B RID: 8219
			FlagRemoval = 4,
			// Token: 0x0400201C RID: 8220
			HealthChange = 8,
			// Token: 0x0400201D RID: 8221
			AliveBotCountChange = 16,
			// Token: 0x0400201E RID: 8222
			PeerControlledAgentChange = 32,
			// Token: 0x0400201F RID: 8223
			BannerPickUp = 64,
			// Token: 0x04002020 RID: 8224
			BannerDrop = 128,
			// Token: 0x04002021 RID: 8225
			SpawnEnd = 256,
			// Token: 0x04002022 RID: 8226
			MountHealthChange = 512,
			// Token: 0x04002023 RID: 8227
			MountChange = 1024,
			// Token: 0x04002024 RID: 8228
			AgentEventsMask = 1576
		}
	}
}
