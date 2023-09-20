using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003C5 RID: 965
	public class MountDamageEffect : MPCombatPerkEffect
	{
		// Token: 0x060033C5 RID: 13253 RVA: 0x000D6A6D File Offset: 0x000D4C6D
		protected MountDamageEffect()
		{
		}

		// Token: 0x060033C6 RID: 13254 RVA: 0x000D6A78 File Offset: 0x000D4C78
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\MountDamageEffect.cs", "Deserialize", 22);
			}
		}

		// Token: 0x060033C7 RID: 13255 RVA: 0x000D6ADD File Offset: 0x000D4CDD
		public override float GetMountDamage(WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			if (!base.IsSatisfied(attackerWeapon, damageType))
			{
				return 0f;
			}
			return this._value;
		}

		// Token: 0x04001606 RID: 5638
		protected static string StringType = "MountDamage";

		// Token: 0x04001607 RID: 5639
		private float _value;
	}
}
