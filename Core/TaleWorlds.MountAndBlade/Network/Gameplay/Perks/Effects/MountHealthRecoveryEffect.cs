using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003C7 RID: 967
	public class MountHealthRecoveryEffect : MPPerkEffect
	{
		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x060033CD RID: 13261 RVA: 0x000D6B95 File Offset: 0x000D4D95
		public override bool IsTickRequired
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060033CE RID: 13262 RVA: 0x000D6B98 File Offset: 0x000D4D98
		protected MountHealthRecoveryEffect()
		{
		}

		// Token: 0x060033CF RID: 13263 RVA: 0x000D6BA0 File Offset: 0x000D4DA0
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\MountHealthRecoveryEffect.cs", "Deserialize", 29);
			}
			string text5;
			if (node == null)
			{
				text5 = null;
			}
			else
			{
				XmlAttributeCollection attributes3 = node.Attributes;
				if (attributes3 == null)
				{
					text5 = null;
				}
				else
				{
					XmlAttribute xmlAttribute3 = attributes3["period"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			if (text6 == null || !int.TryParse(text6, out this._period) || this._period < 1)
			{
				Debug.FailedAssert("provided 'period' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\MountHealthRecoveryEffect.cs", "Deserialize", 35);
			}
		}

		// Token: 0x060033D0 RID: 13264 RVA: 0x000D6CA0 File Offset: 0x000D4EA0
		public override void OnTick(Agent agent, int tickCount)
		{
			agent = ((agent != null && !agent.IsMount) ? agent.MountAgent : agent);
			if (tickCount % this._period == 0 && agent != null && agent.IsActive())
			{
				agent.Health = MathF.Min(agent.HealthLimit, agent.Health + this._value);
			}
		}

		// Token: 0x0400160A RID: 5642
		protected static string StringType = "MountHealthRecovery";

		// Token: 0x0400160B RID: 5643
		private float _value;

		// Token: 0x0400160C RID: 5644
		private int _period;
	}
}
