using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003C3 RID: 963
	public class HealthRecoveryEffect : MPPerkEffect
	{
		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x060033BC RID: 13244 RVA: 0x000D6853 File Offset: 0x000D4A53
		public override bool IsTickRequired
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060033BD RID: 13245 RVA: 0x000D6856 File Offset: 0x000D4A56
		protected HealthRecoveryEffect()
		{
		}

		// Token: 0x060033BE RID: 13246 RVA: 0x000D6860 File Offset: 0x000D4A60
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\HealthRecoveryEffect.cs", "Deserialize", 29);
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
				Debug.FailedAssert("provided 'period' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\HealthRecoveryEffect.cs", "Deserialize", 35);
			}
		}

		// Token: 0x060033BF RID: 13247 RVA: 0x000D6960 File Offset: 0x000D4B60
		public override void OnTick(Agent agent, int tickCount)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (tickCount % this._period == 0 && agent != null && agent.IsActive())
			{
				agent.Health = MathF.Min(agent.HealthLimit, agent.Health + this._value);
			}
		}

		// Token: 0x04001601 RID: 5633
		protected static string StringType = "HealthRecovery";

		// Token: 0x04001602 RID: 5634
		private float _value;

		// Token: 0x04001603 RID: 5635
		private int _period;
	}
}
