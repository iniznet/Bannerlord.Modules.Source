using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	public class MountDamageTakenEffect : MPCombatPerkEffect
	{
		protected MountDamageTakenEffect()
		{
		}

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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\MountDamageTakenEffect.cs", "Deserialize", 22);
			}
		}

		public override float GetMountDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			if (!base.IsSatisfied(attackerWeapon, damageType))
			{
				return 0f;
			}
			return this._value;
		}

		protected static string StringType = "MountDamageTaken";

		private float _value;
	}
}
