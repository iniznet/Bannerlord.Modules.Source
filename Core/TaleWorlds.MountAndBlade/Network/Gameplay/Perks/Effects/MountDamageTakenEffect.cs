using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003C6 RID: 966
	public class MountDamageTakenEffect : MPCombatPerkEffect
	{
		// Token: 0x060033C9 RID: 13257 RVA: 0x000D6B01 File Offset: 0x000D4D01
		protected MountDamageTakenEffect()
		{
		}

		// Token: 0x060033CA RID: 13258 RVA: 0x000D6B0C File Offset: 0x000D4D0C
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

		// Token: 0x060033CB RID: 13259 RVA: 0x000D6B71 File Offset: 0x000D4D71
		public override float GetMountDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			if (!base.IsSatisfied(attackerWeapon, damageType))
			{
				return 0f;
			}
			return this._value;
		}

		// Token: 0x04001608 RID: 5640
		protected static string StringType = "MountDamageTaken";

		// Token: 0x04001609 RID: 5641
		private float _value;
	}
}
