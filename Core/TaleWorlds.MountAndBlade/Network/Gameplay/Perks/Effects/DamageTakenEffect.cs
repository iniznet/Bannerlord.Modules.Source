using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003BC RID: 956
	public class DamageTakenEffect : MPCombatPerkEffect
	{
		// Token: 0x0600339D RID: 13213 RVA: 0x000D5FD8 File Offset: 0x000D41D8
		protected DamageTakenEffect()
		{
		}

		// Token: 0x0600339E RID: 13214 RVA: 0x000D5FE0 File Offset: 0x000D41E0
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\DamageTakenEffect.cs", "Deserialize", 22);
			}
		}

		// Token: 0x0600339F RID: 13215 RVA: 0x000D6045 File Offset: 0x000D4245
		public override float GetDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType)
		{
			if (!base.IsSatisfied(attackerWeapon, damageType))
			{
				return 0f;
			}
			return this._value;
		}

		// Token: 0x040015EC RID: 5612
		protected static string StringType = "DamageTaken";

		// Token: 0x040015ED RID: 5613
		private float _value;
	}
}
