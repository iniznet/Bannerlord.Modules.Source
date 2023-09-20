using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MPOnSpawnPerkEffectBase : MPPerkEffectBase, IOnSpawnPerkEffect
	{
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
					XmlAttribute xmlAttribute2 = attributes2["target"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			this.EffectTarget = MPOnSpawnPerkEffectBase.Target.Any;
			if (text4 != null && !Enum.TryParse<MPOnSpawnPerkEffectBase.Target>(text4, true, out this.EffectTarget))
			{
				this.EffectTarget = MPOnSpawnPerkEffectBase.Target.Any;
				Debug.FailedAssert("provided 'target' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\MPOnSpawnPerkEffectBase.cs", "Deserialize", 38);
			}
		}

		public virtual float GetTroopCountMultiplier()
		{
			return 0f;
		}

		public virtual int GetExtraTroopCount()
		{
			return 0;
		}

		public virtual List<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isPlayer, List<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments, bool getAll = false)
		{
			return alternativeEquipments;
		}

		public virtual float GetDrivenPropertyBonusOnSpawn(bool isPlayer, DrivenProperty drivenProperty, float baseValue)
		{
			return 0f;
		}

		public virtual float GetHitpoints(bool isPlayer)
		{
			return 0f;
		}

		protected MPOnSpawnPerkEffectBase.Target EffectTarget;

		protected enum Target
		{
			Player,
			Troops,
			Any
		}
	}
}
