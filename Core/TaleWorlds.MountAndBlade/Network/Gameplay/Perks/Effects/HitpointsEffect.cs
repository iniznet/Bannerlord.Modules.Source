using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	public class HitpointsEffect : MPOnSpawnPerkEffect
	{
		protected HitpointsEffect()
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
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\HitpointsEffect.cs", "Deserialize", 20);
			}
		}

		public override float GetHitpoints(bool isPlayer)
		{
			if (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Any || (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops)))
			{
				return this._value;
			}
			return 0f;
		}

		protected static string StringType = "HitpointsOnSpawn";

		private float _value;
	}
}
