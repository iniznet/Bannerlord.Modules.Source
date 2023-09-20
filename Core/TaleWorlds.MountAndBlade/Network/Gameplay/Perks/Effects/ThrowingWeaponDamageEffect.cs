using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003D2 RID: 978
	public class ThrowingWeaponDamageEffect : MPPerkEffect
	{
		// Token: 0x060033FF RID: 13311 RVA: 0x000D7AD7 File Offset: 0x000D5CD7
		protected ThrowingWeaponDamageEffect()
		{
		}

		// Token: 0x06003400 RID: 13312 RVA: 0x000D7AE0 File Offset: 0x000D5CE0
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\ThrowingWeaponDamageEffect.cs", "Deserialize", 24);
			}
		}

		// Token: 0x06003401 RID: 13313 RVA: 0x000D7B84 File Offset: 0x000D5D84
		public override float GetDamage(WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			if (isAlternativeAttack || attackerWeapon == null || WeaponComponentData.GetItemTypeFromWeaponClass(attackerWeapon.WeaponClass) != ItemObject.ItemTypeEnum.Thrown)
			{
				return 0f;
			}
			return this._value;
		}

		// Token: 0x04001625 RID: 5669
		protected static string StringType = "ThrowingWeaponDamage";

		// Token: 0x04001626 RID: 5670
		private float _value;
	}
}
