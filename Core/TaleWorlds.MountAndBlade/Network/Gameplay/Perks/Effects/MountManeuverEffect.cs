using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003C8 RID: 968
	public class MountManeuverEffect : MPPerkEffect
	{
		// Token: 0x060033D2 RID: 13266 RVA: 0x000D6D02 File Offset: 0x000D4F02
		protected MountManeuverEffect()
		{
		}

		// Token: 0x060033D3 RID: 13267 RVA: 0x000D6D0C File Offset: 0x000D4F0C
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\MountManeuverEffect.cs", "Deserialize", 23);
			}
		}

		// Token: 0x060033D4 RID: 13268 RVA: 0x000D6DB0 File Offset: 0x000D4FB0
		public override void OnUpdate(Agent agent, bool newState)
		{
			agent = ((agent != null && !agent.IsMount) ? agent.MountAgent : agent);
			if (agent != null)
			{
				agent.UpdateAgentProperties();
			}
		}

		// Token: 0x060033D5 RID: 13269 RVA: 0x000D6DD1 File Offset: 0x000D4FD1
		public override float GetMountManeuver()
		{
			return this._value;
		}

		// Token: 0x0400160D RID: 5645
		protected static string StringType = "MountManeuver";

		// Token: 0x0400160E RID: 5646
		private float _value;
	}
}
