using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003C0 RID: 960
	public class GoldGainOnAssistEffect : MPPerkEffect
	{
		// Token: 0x060033AF RID: 13231 RVA: 0x000D649A File Offset: 0x000D469A
		protected GoldGainOnAssistEffect()
		{
		}

		// Token: 0x060033B0 RID: 13232 RVA: 0x000D64A4 File Offset: 0x000D46A4
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\GoldGainOnAssistEffect.cs", "Deserialize", 22);
			}
		}

		// Token: 0x060033B1 RID: 13233 RVA: 0x000D6548 File Offset: 0x000D4748
		public override int GetGoldOnAssist()
		{
			return this._value;
		}

		// Token: 0x040015F9 RID: 5625
		protected static string StringType = "GoldGainOnAssist";

		// Token: 0x040015FA RID: 5626
		private int _value;
	}
}
