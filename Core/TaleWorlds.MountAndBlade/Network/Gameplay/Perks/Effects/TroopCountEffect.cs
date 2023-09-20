using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	public class TroopCountEffect : MPOnSpawnPerkEffect
	{
		protected TroopCountEffect()
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\TroopCountEffect.cs", "Deserialize", 21);
			}
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
					XmlAttribute xmlAttribute2 = attributes2["is_multiplier"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			this._isMultiplier = ((text4 != null) ? text4.ToLower() : null) == "true";
		}

		public override float GetTroopCountMultiplier()
		{
			if (!this._isMultiplier)
			{
				return 0f;
			}
			return this._value;
		}

		public override float GetExtraTroopCount()
		{
			if (!this._isMultiplier)
			{
				return this._value;
			}
			return 0f;
		}

		protected static string StringType = "TroopCountOnSpawn";

		private bool _isMultiplier;

		private float _value;
	}
}
