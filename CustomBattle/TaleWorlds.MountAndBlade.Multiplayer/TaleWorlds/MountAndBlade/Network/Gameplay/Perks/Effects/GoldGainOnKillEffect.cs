using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	public class GoldGainOnKillEffect : MPPerkEffect
	{
		protected GoldGainOnKillEffect()
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
			if (text4 == null || !int.TryParse(text4, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Effects\\GoldGainOnKillEffect.cs", "Deserialize", 31);
			}
			string text5;
			if (node == null)
			{
				text5 = null;
			}
			else
			{
				XmlAttributeCollection attributes3 = node.Attributes;
				if (attributes3 == null)
				{
					text5 = null;
				}
				else
				{
					XmlAttribute xmlAttribute3 = attributes3["enemy_value"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			this._enemyValue = GoldGainOnKillEffect.EnemyValue.Any;
			if (text6 != null && !Enum.TryParse<GoldGainOnKillEffect.EnemyValue>(text6, true, out this._enemyValue))
			{
				this._enemyValue = GoldGainOnKillEffect.EnemyValue.Any;
				Debug.FailedAssert("provided 'enemy_value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Effects\\GoldGainOnKillEffect.cs", "Deserialize", 39);
			}
		}

		public override int GetGoldOnKill(float attackerValue, float victimValue)
		{
			switch (this._enemyValue)
			{
			case GoldGainOnKillEffect.EnemyValue.Any:
				return this._value;
			case GoldGainOnKillEffect.EnemyValue.Higher:
				if (victimValue <= attackerValue)
				{
					return 0;
				}
				return this._value;
			case GoldGainOnKillEffect.EnemyValue.Lower:
				if (victimValue >= attackerValue)
				{
					return 0;
				}
				return this._value;
			default:
				return 0;
			}
		}

		protected static string StringType = "GoldGainOnKill";

		private int _value;

		private GoldGainOnKillEffect.EnemyValue _enemyValue;

		private enum EnemyValue
		{
			Any,
			Higher,
			Lower
		}
	}
}
