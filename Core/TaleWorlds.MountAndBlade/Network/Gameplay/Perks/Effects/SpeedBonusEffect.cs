using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003D1 RID: 977
	public class SpeedBonusEffect : MPPerkEffect
	{
		// Token: 0x060033FB RID: 13307 RVA: 0x000D79F8 File Offset: 0x000D5BF8
		protected SpeedBonusEffect()
		{
		}

		// Token: 0x060033FC RID: 13308 RVA: 0x000D7A00 File Offset: 0x000D5C00
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\SpeedBonusEffect.cs", "Deserialize", 23);
			}
		}

		// Token: 0x060033FD RID: 13309 RVA: 0x000D7AA4 File Offset: 0x000D5CA4
		public override float GetSpeedBonusEffectiveness(Agent attacker)
		{
			attacker = ((attacker != null && attacker.IsMount) ? attacker.RiderAgent : attacker);
			if (attacker != null)
			{
				return this._value;
			}
			return 0f;
		}

		// Token: 0x04001623 RID: 5667
		protected static string StringType = "SpeedBonus";

		// Token: 0x04001624 RID: 5668
		private float _value;
	}
}
