using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003C9 RID: 969
	public class MountSpeedEffect : MPPerkEffect
	{
		// Token: 0x060033D7 RID: 13271 RVA: 0x000D6DE5 File Offset: 0x000D4FE5
		protected MountSpeedEffect()
		{
		}

		// Token: 0x060033D8 RID: 13272 RVA: 0x000D6DF0 File Offset: 0x000D4FF0
		protected override void Deserialize(XmlNode node)
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
					XmlAttribute xmlAttribute = attributes["is_disabled_in_warmup"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			base.IsDisabledInWarmup = ((text2 != null) ? text2.ToLower() : null) == "true";
			string text3;
			if (node == null)
			{
				text3 = null;
			}
			else
			{
				XmlAttributeCollection attributes2 = node.Attributes;
				if (attributes2 == null)
				{
					text3 = null;
				}
				else
				{
					XmlAttribute xmlAttribute2 = attributes2["value"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			if (text4 == null || !float.TryParse(text4, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\MountSpeedEffect.cs", "Deserialize", 23);
			}
		}

		// Token: 0x060033D9 RID: 13273 RVA: 0x000D6E94 File Offset: 0x000D5094
		public override void OnUpdate(Agent agent, bool newState)
		{
			agent = ((agent != null && !agent.IsMount) ? agent.MountAgent : agent);
			if (agent != null)
			{
				agent.UpdateAgentProperties();
			}
		}

		// Token: 0x060033DA RID: 13274 RVA: 0x000D6EB5 File Offset: 0x000D50B5
		public override float GetMountSpeed()
		{
			return this._value;
		}

		// Token: 0x0400160F RID: 5647
		protected static string StringType = "MountSpeed";

		// Token: 0x04001610 RID: 5648
		private float _value;
	}
}
