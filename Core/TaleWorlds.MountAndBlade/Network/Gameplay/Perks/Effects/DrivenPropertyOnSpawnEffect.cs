using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	public class DrivenPropertyOnSpawnEffect : MPOnSpawnPerkEffect
	{
		protected DrivenPropertyOnSpawnEffect()
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
					XmlAttribute xmlAttribute = attributes["is_ratio"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			this._isRatio = ((text2 != null) ? text2.ToLower() : null) == "true";
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
					XmlAttribute xmlAttribute2 = attributes2["driven_property"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			if (!Enum.TryParse<DrivenProperty>(text3, true, out this._drivenProperty))
			{
				Debug.FailedAssert("provided 'driven_property' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\DrivenPropertyOnSpawnEffect.cs", "Deserialize", 26);
			}
			string text4;
			if (node == null)
			{
				text4 = null;
			}
			else
			{
				XmlAttributeCollection attributes3 = node.Attributes;
				if (attributes3 == null)
				{
					text4 = null;
				}
				else
				{
					XmlAttribute xmlAttribute3 = attributes3["value"];
					text4 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text5 = text4;
			if (text5 == null || !float.TryParse(text5, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\DrivenPropertyOnSpawnEffect.cs", "Deserialize", 33);
			}
		}

		public override float GetDrivenPropertyBonusOnSpawn(bool isPlayer, DrivenProperty drivenProperty, float baseValue)
		{
			if (drivenProperty != this._drivenProperty || (this.EffectTarget != MPOnSpawnPerkEffectBase.Target.Any && !(isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops))))
			{
				return 0f;
			}
			if (!this._isRatio)
			{
				return this._value;
			}
			return baseValue * this._value;
		}

		protected static string StringType = "DrivenPropertyOnSpawn";

		private DrivenProperty _drivenProperty;

		private float _value;

		private bool _isRatio;
	}
}
