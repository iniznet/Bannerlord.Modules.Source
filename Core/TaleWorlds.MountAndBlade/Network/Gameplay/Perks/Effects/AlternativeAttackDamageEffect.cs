using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	public class AlternativeAttackDamageEffect : MPPerkEffect
	{
		protected AlternativeAttackDamageEffect()
		{
		}

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
					XmlAttribute xmlAttribute2 = attributes2["attack_type"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			this._attackType = AlternativeAttackDamageEffect.AttackType.Any;
			if (text4 != null && !Enum.TryParse<AlternativeAttackDamageEffect.AttackType>(text4, true, out this._attackType))
			{
				this._attackType = AlternativeAttackDamageEffect.AttackType.Any;
				Debug.FailedAssert("provided 'attack_type' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\AlternativeAttackDamageEffect.cs", "Deserialize", 34);
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
					XmlAttribute xmlAttribute3 = attributes3["value"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			if (text6 == null || !float.TryParse(text6, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\AlternativeAttackDamageEffect.cs", "Deserialize", 40);
			}
		}

		public override float GetDamage(WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			if (isAlternativeAttack)
			{
				switch (this._attackType)
				{
				case AlternativeAttackDamageEffect.AttackType.Any:
					return this._value;
				case AlternativeAttackDamageEffect.AttackType.Kick:
					if (attackerWeapon != null)
					{
						return 0f;
					}
					return this._value;
				case AlternativeAttackDamageEffect.AttackType.Bash:
					if (attackerWeapon == null)
					{
						return 0f;
					}
					return this._value;
				}
			}
			return 0f;
		}

		protected static string StringType = "AlternativeAttackDamage";

		private AlternativeAttackDamageEffect.AttackType _attackType;

		private float _value;

		private enum AttackType
		{
			Any,
			Kick,
			Bash
		}
	}
}
