using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003CD RID: 973
	public class RewardGoldOnAssistEffect : MPPerkEffect
	{
		// Token: 0x060033E9 RID: 13289 RVA: 0x000D730C File Offset: 0x000D550C
		protected RewardGoldOnAssistEffect()
		{
		}

		// Token: 0x060033EA RID: 13290 RVA: 0x000D7314 File Offset: 0x000D5514
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
			if (text4 == null || !int.TryParse(text4, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\RewardGoldOnAssistEffect.cs", "Deserialize", 22);
			}
		}

		// Token: 0x060033EB RID: 13291 RVA: 0x000D73B8 File Offset: 0x000D55B8
		public override int GetRewardedGoldOnAssist()
		{
			return this._value;
		}

		// Token: 0x04001617 RID: 5655
		protected static string StringType = "RewardGoldOnAssist";

		// Token: 0x04001618 RID: 5656
		private int _value;
	}
}
