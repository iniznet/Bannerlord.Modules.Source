using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003B9 RID: 953
	public class ArmorEffect : MPOnSpawnPerkEffect
	{
		// Token: 0x06003391 RID: 13201 RVA: 0x000D5DB6 File Offset: 0x000D3FB6
		protected ArmorEffect()
		{
		}

		// Token: 0x06003392 RID: 13202 RVA: 0x000D5DC0 File Offset: 0x000D3FC0
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\ArmorEffect.cs", "Deserialize", 21);
			}
		}

		// Token: 0x06003393 RID: 13203 RVA: 0x000D5E28 File Offset: 0x000D4028
		public override float GetDrivenPropertyBonusOnSpawn(bool isPlayer, DrivenProperty drivenProperty, float baseValue)
		{
			if ((drivenProperty == DrivenProperty.ArmorHead || drivenProperty == DrivenProperty.ArmorTorso || drivenProperty == DrivenProperty.ArmorLegs || drivenProperty == DrivenProperty.ArmorArms) && (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Any || (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops))))
			{
				return this._value;
			}
			return 0f;
		}

		// Token: 0x040015E6 RID: 5606
		protected static string StringType = "ArmorOnSpawn";

		// Token: 0x040015E7 RID: 5607
		private float _value;
	}
}
