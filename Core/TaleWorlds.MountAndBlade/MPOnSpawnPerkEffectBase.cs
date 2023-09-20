using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000314 RID: 788
	public abstract class MPOnSpawnPerkEffectBase : MPPerkEffectBase, IOnSpawnPerkEffect
	{
		// Token: 0x06002A7F RID: 10879 RVA: 0x000A5878 File Offset: 0x000A3A78
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
				Debug.FailedAssert("provided 'target' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\MPOnSpawnPerkEffectBase.cs", "Deserialize", 39);
			}
		}

		// Token: 0x06002A80 RID: 10880 RVA: 0x000A592B File Offset: 0x000A3B2B
		public virtual float GetTroopCountMultiplier()
		{
			return 0f;
		}

		// Token: 0x06002A81 RID: 10881 RVA: 0x000A5932 File Offset: 0x000A3B32
		public virtual float GetExtraTroopCount()
		{
			return 0f;
		}

		// Token: 0x06002A82 RID: 10882 RVA: 0x000A5939 File Offset: 0x000A3B39
		public virtual List<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isPlayer, List<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments, bool getAll = false)
		{
			return alternativeEquipments;
		}

		// Token: 0x06002A83 RID: 10883 RVA: 0x000A593C File Offset: 0x000A3B3C
		public virtual float GetDrivenPropertyBonusOnSpawn(bool isPlayer, DrivenProperty drivenProperty, float baseValue)
		{
			return 0f;
		}

		// Token: 0x06002A84 RID: 10884 RVA: 0x000A5943 File Offset: 0x000A3B43
		public virtual float GetHitpoints(bool isPlayer)
		{
			return 0f;
		}

		// Token: 0x04001045 RID: 4165
		protected MPOnSpawnPerkEffectBase.Target EffectTarget;

		// Token: 0x02000630 RID: 1584
		protected enum Target
		{
			// Token: 0x04002014 RID: 8212
			Player,
			// Token: 0x04002015 RID: 8213
			Troops,
			// Token: 0x04002016 RID: 8214
			Any
		}
	}
}
