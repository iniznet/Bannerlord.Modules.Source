﻿using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	public class ThrowingWeaponSpeedEffect : MPPerkEffect
	{
		protected ThrowingWeaponSpeedEffect()
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

		public override float GetThrowingWeaponSpeed(WeaponComponentData attackerWeapon)
		{
			if (attackerWeapon == null || WeaponComponentData.GetItemTypeFromWeaponClass(attackerWeapon.WeaponClass) != ItemObject.ItemTypeEnum.Thrown)
			{
				return 0f;
			}
			return this._value;
		}

		protected static string StringType = "ThrowingWeaponSpeed";

		private float _value;
	}
}
