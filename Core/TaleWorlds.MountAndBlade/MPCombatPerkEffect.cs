using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000310 RID: 784
	public abstract class MPCombatPerkEffect : MPPerkEffect
	{
		// Token: 0x06002A68 RID: 10856 RVA: 0x000A4BD0 File Offset: 0x000A2DD0
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
					XmlAttribute xmlAttribute2 = attributes2["hit_type"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			this.EffectHitType = MPCombatPerkEffect.HitType.Any;
			if (text4 != null && !Enum.TryParse<MPCombatPerkEffect.HitType>(text4, true, out this.EffectHitType))
			{
				this.EffectHitType = MPCombatPerkEffect.HitType.Any;
				Debug.FailedAssert("provided 'hit_type' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\MPCombatPerkEffect.cs", "Deserialize", 31);
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
					XmlAttribute xmlAttribute3 = attributes3["damage_type"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			DamageTypes damageTypes;
			if (text6 == null || text6.ToLower() == "any")
			{
				this.DamageType = null;
			}
			else if (Enum.TryParse<DamageTypes>(text6, true, out damageTypes))
			{
				this.DamageType = new DamageTypes?(damageTypes);
			}
			else
			{
				Debug.FailedAssert("provided 'damage_type' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\MPCombatPerkEffect.cs", "Deserialize", 47);
				this.DamageType = null;
			}
			string text7;
			if (node == null)
			{
				text7 = null;
			}
			else
			{
				XmlAttributeCollection attributes4 = node.Attributes;
				if (attributes4 == null)
				{
					text7 = null;
				}
				else
				{
					XmlAttribute xmlAttribute4 = attributes4["weapon_class"];
					text7 = ((xmlAttribute4 != null) ? xmlAttribute4.Value : null);
				}
			}
			string text8 = text7;
			if (text8 == null || text8.ToLower() == "any")
			{
				this.WeaponClass = null;
				return;
			}
			WeaponClass weaponClass;
			if (Enum.TryParse<WeaponClass>(text8, true, out weaponClass))
			{
				this.WeaponClass = new WeaponClass?(weaponClass);
				return;
			}
			Debug.FailedAssert("provided 'weapon_class' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\MPCombatPerkEffect.cs", "Deserialize", 65);
			this.WeaponClass = null;
		}

		// Token: 0x06002A69 RID: 10857 RVA: 0x000A4D94 File Offset: 0x000A2F94
		protected bool IsSatisfied(WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			if (this.DamageType == null || this.DamageType.Value == damageType)
			{
				if (this.WeaponClass != null)
				{
					WeaponClass value = this.WeaponClass.Value;
					WeaponClass? weaponClass = ((attackerWeapon != null) ? new WeaponClass?(attackerWeapon.WeaponClass) : null);
					if (!((value == weaponClass.GetValueOrDefault()) & (weaponClass != null)))
					{
						return false;
					}
				}
				switch (this.EffectHitType)
				{
				case MPCombatPerkEffect.HitType.Any:
					return true;
				case MPCombatPerkEffect.HitType.Melee:
					return !this.IsWeaponRanged(attackerWeapon);
				case MPCombatPerkEffect.HitType.Ranged:
					return this.IsWeaponRanged(attackerWeapon);
				}
			}
			return false;
		}

		// Token: 0x06002A6A RID: 10858 RVA: 0x000A4E32 File Offset: 0x000A3032
		protected bool IsWeaponRanged(WeaponComponentData attackerWeapon)
		{
			return attackerWeapon != null && (attackerWeapon.IsConsumable || attackerWeapon.IsRangedWeapon);
		}

		// Token: 0x0400103F RID: 4159
		protected MPCombatPerkEffect.HitType EffectHitType;

		// Token: 0x04001040 RID: 4160
		protected DamageTypes? DamageType;

		// Token: 0x04001041 RID: 4161
		protected WeaponClass? WeaponClass;

		// Token: 0x0200062D RID: 1581
		protected enum HitType
		{
			// Token: 0x0400200E RID: 8206
			Any,
			// Token: 0x0400200F RID: 8207
			Melee,
			// Token: 0x04002010 RID: 8208
			Ranged
		}
	}
}
