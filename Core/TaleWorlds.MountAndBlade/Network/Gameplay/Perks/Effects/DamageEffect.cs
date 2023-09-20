using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003BA RID: 954
	public class DamageEffect : MPCombatPerkEffect
	{
		// Token: 0x06003395 RID: 13205 RVA: 0x000D5E83 File Offset: 0x000D4083
		protected DamageEffect()
		{
		}

		// Token: 0x06003396 RID: 13206 RVA: 0x000D5E8C File Offset: 0x000D408C
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\DamageEffect.cs", "Deserialize", 22);
			}
		}

		// Token: 0x06003397 RID: 13207 RVA: 0x000D5EF1 File Offset: 0x000D40F1
		public override float GetDamage(WeaponComponentData attackerWeapon, DamageTypes damageType, bool isAlternativeAttack)
		{
			if (!base.IsSatisfied(attackerWeapon, damageType))
			{
				return 0f;
			}
			return this._value;
		}

		// Token: 0x040015E8 RID: 5608
		protected static string StringType = "DamageDealt";

		// Token: 0x040015E9 RID: 5609
		private float _value;
	}
}
