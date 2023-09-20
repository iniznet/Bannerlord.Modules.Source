using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003D4 RID: 980
	public class TroopCountEffect : MPOnSpawnPerkEffect
	{
		// Token: 0x06003407 RID: 13319 RVA: 0x000D7C8C File Offset: 0x000D5E8C
		protected TroopCountEffect()
		{
		}

		// Token: 0x06003408 RID: 13320 RVA: 0x000D7C94 File Offset: 0x000D5E94
		protected override void Deserialize(XmlNode node)
		{
			base.Deserialize(node);
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
					XmlAttribute xmlAttribute = attributes["value"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			if (text2 == null || !float.TryParse(text2, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\TroopCountEffect.cs", "Deserialize", 21);
			}
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
					XmlAttribute xmlAttribute2 = attributes2["is_multiplier"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			this._isMultiplier = ((text4 != null) ? text4.ToLower() : null) == "true";
		}

		// Token: 0x06003409 RID: 13321 RVA: 0x000D7D3F File Offset: 0x000D5F3F
		public override float GetTroopCountMultiplier()
		{
			if (!this._isMultiplier)
			{
				return 0f;
			}
			return this._value;
		}

		// Token: 0x0600340A RID: 13322 RVA: 0x000D7D55 File Offset: 0x000D5F55
		public override float GetExtraTroopCount()
		{
			if (!this._isMultiplier)
			{
				return this._value;
			}
			return 0f;
		}

		// Token: 0x04001629 RID: 5673
		protected static string StringType = "TroopCountOnSpawn";

		// Token: 0x0400162A RID: 5674
		private bool _isMultiplier;

		// Token: 0x0400162B RID: 5675
		private float _value;
	}
}
