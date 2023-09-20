using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003CB RID: 971
	public class RangedAccuracyEffect : MPPerkEffect
	{
		// Token: 0x060033E0 RID: 13280 RVA: 0x000D7168 File Offset: 0x000D5368
		protected RangedAccuracyEffect()
		{
		}

		// Token: 0x060033E1 RID: 13281 RVA: 0x000D7170 File Offset: 0x000D5370
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\RangedAccuracyEffect.cs", "Deserialize", 23);
			}
		}

		// Token: 0x060033E2 RID: 13282 RVA: 0x000D7214 File Offset: 0x000D5414
		public override void OnUpdate(Agent agent, bool newState)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (agent != null)
			{
				agent.UpdateAgentProperties();
			}
		}

		// Token: 0x060033E3 RID: 13283 RVA: 0x000D7235 File Offset: 0x000D5435
		public override float GetRangedAccuracy()
		{
			return this._value;
		}

		// Token: 0x04001613 RID: 5651
		protected static string StringType = "RangedAccuracy";

		// Token: 0x04001614 RID: 5652
		private float _value;
	}
}
