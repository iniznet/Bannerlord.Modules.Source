using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	public class ArmorEffect : MPOnSpawnPerkEffect
	{
		protected ArmorEffect()
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Effects\\ArmorEffect.cs", "Deserialize", 21);
			}
		}

		public override float GetDrivenPropertyBonusOnSpawn(bool isPlayer, DrivenProperty drivenProperty, float baseValue)
		{
			if ((drivenProperty == 50 || drivenProperty == 51 || drivenProperty == 52 || drivenProperty == 53) && (this.EffectTarget == 2 || (isPlayer ? (this.EffectTarget == 0) : (this.EffectTarget == 1))))
			{
				return this._value;
			}
			return 0f;
		}

		protected static string StringType = "ArmorOnSpawn";

		private float _value;
	}
}
