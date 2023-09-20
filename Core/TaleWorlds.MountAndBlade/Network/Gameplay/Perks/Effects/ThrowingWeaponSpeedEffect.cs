using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003D3 RID: 979
	public class ThrowingWeaponSpeedEffect : MPPerkEffect
	{
		// Token: 0x06003403 RID: 13315 RVA: 0x000D7BB3 File Offset: 0x000D5DB3
		protected ThrowingWeaponSpeedEffect()
		{
		}

		// Token: 0x06003404 RID: 13316 RVA: 0x000D7BBC File Offset: 0x000D5DBC
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\ThrowingWeaponSpeedEffect.cs", "Deserialize", 24);
			}
		}

		// Token: 0x06003405 RID: 13317 RVA: 0x000D7C60 File Offset: 0x000D5E60
		public override float GetThrowingWeaponSpeed(WeaponComponentData attackerWeapon)
		{
			if (attackerWeapon == null || WeaponComponentData.GetItemTypeFromWeaponClass(attackerWeapon.WeaponClass) != ItemObject.ItemTypeEnum.Thrown)
			{
				return 0f;
			}
			return this._value;
		}

		// Token: 0x04001627 RID: 5671
		protected static string StringType = "ThrowingWeaponSpeed";

		// Token: 0x04001628 RID: 5672
		private float _value;
	}
}
