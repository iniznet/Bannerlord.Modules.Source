using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003CC RID: 972
	public class RangedHeadShotDamageEffect : MPPerkEffect
	{
		// Token: 0x060033E5 RID: 13285 RVA: 0x000D7249 File Offset: 0x000D5449
		protected RangedHeadShotDamageEffect()
		{
		}

		// Token: 0x060033E6 RID: 13286 RVA: 0x000D7254 File Offset: 0x000D5454
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\RangedHeadShotDamageEffect.cs", "Deserialize", 22);
			}
		}

		// Token: 0x060033E7 RID: 13287 RVA: 0x000D72F8 File Offset: 0x000D54F8
		public override float GetRangedHeadShotDamage()
		{
			return this._value;
		}

		// Token: 0x04001615 RID: 5653
		protected static string StringType = "RangedHeadShotDamage";

		// Token: 0x04001616 RID: 5654
		private float _value;
	}
}
