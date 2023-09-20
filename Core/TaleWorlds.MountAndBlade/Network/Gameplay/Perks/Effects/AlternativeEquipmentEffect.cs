using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003B8 RID: 952
	public class AlternativeEquipmentEffect : MPOnSpawnPerkEffect
	{
		// Token: 0x0600338D RID: 13197 RVA: 0x000D5C9B File Offset: 0x000D3E9B
		protected AlternativeEquipmentEffect()
		{
		}

		// Token: 0x0600338E RID: 13198 RVA: 0x000D5CA4 File Offset: 0x000D3EA4
		protected override void Deserialize(XmlNode node)
		{
			base.Deserialize(node);
			this._item = default(EquipmentElement);
			this._index = EquipmentIndex.None;
			XmlNode xmlNode;
			if (node == null)
			{
				xmlNode = null;
			}
			else
			{
				XmlAttributeCollection attributes = node.Attributes;
				xmlNode = ((attributes != null) ? attributes["item"] : null);
			}
			XmlNode xmlNode2 = xmlNode;
			if (xmlNode2 != null)
			{
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(xmlNode2.Value);
				this._item = new EquipmentElement(@object, null, null, false);
			}
			XmlNode xmlNode3;
			if (node == null)
			{
				xmlNode3 = null;
			}
			else
			{
				XmlAttributeCollection attributes2 = node.Attributes;
				xmlNode3 = ((attributes2 != null) ? attributes2["slot"] : null);
			}
			XmlNode xmlNode4 = xmlNode3;
			if (xmlNode4 != null)
			{
				this._index = Equipment.GetEquipmentIndexFromOldEquipmentIndexName(xmlNode4.Value);
			}
		}

		// Token: 0x0600338F RID: 13199 RVA: 0x000D5D40 File Offset: 0x000D3F40
		public override List<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isPlayer, List<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments, bool getAll)
		{
			if (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Any || (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops)))
			{
				if (alternativeEquipments == null)
				{
					alternativeEquipments = new List<ValueTuple<EquipmentIndex, EquipmentElement>>
					{
						new ValueTuple<EquipmentIndex, EquipmentElement>(this._index, this._item)
					};
				}
				else
				{
					alternativeEquipments.Add(new ValueTuple<EquipmentIndex, EquipmentElement>(this._index, this._item));
				}
			}
			return alternativeEquipments;
		}

		// Token: 0x040015E3 RID: 5603
		protected static string StringType = "AlternativeEquipmentOnSpawn";

		// Token: 0x040015E4 RID: 5604
		private EquipmentElement _item;

		// Token: 0x040015E5 RID: 5605
		private EquipmentIndex _index;
	}
}
