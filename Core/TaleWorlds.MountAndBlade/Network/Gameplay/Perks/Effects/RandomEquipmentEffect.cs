using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	public class RandomEquipmentEffect : MPRandomOnSpawnPerkEffect
	{
		protected RandomEquipmentEffect()
		{
		}

		protected override void Deserialize(XmlNode node)
		{
			base.Deserialize(node);
			this._groups = new MBList<List<ValueTuple<EquipmentIndex, EquipmentElement>>>();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType != XmlNodeType.Comment && xmlNode.NodeType != XmlNodeType.SignificantWhitespace && xmlNode.Name == "Group")
				{
					List<ValueTuple<EquipmentIndex, EquipmentElement>> list = new List<ValueTuple<EquipmentIndex, EquipmentElement>>();
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.NodeType != XmlNodeType.Comment && xmlNode2.NodeType != XmlNodeType.SignificantWhitespace)
						{
							EquipmentElement equipmentElement = default(EquipmentElement);
							EquipmentIndex equipmentIndex = EquipmentIndex.None;
							XmlAttributeCollection attributes = xmlNode2.Attributes;
							XmlAttribute xmlAttribute = ((attributes != null) ? attributes["item"] : null);
							if (xmlAttribute != null)
							{
								ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(xmlAttribute.Value);
								equipmentElement = new EquipmentElement(@object, null, null, false);
							}
							XmlAttributeCollection attributes2 = xmlNode2.Attributes;
							XmlAttribute xmlAttribute2 = ((attributes2 != null) ? attributes2["slot"] : null);
							if (xmlAttribute2 != null)
							{
								equipmentIndex = Equipment.GetEquipmentIndexFromOldEquipmentIndexName(xmlAttribute2.Value);
							}
							list.Add(new ValueTuple<EquipmentIndex, EquipmentElement>(equipmentIndex, equipmentElement));
						}
					}
					if (list.Count > 0)
					{
						this._groups.Add(list);
					}
				}
			}
		}

		public override List<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isPlayer, List<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments, bool getAll)
		{
			if (getAll)
			{
				using (List<List<ValueTuple<EquipmentIndex, EquipmentElement>>>.Enumerator enumerator = this._groups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						List<ValueTuple<EquipmentIndex, EquipmentElement>> list = enumerator.Current;
						if (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Any || (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops)))
						{
							if (alternativeEquipments == null)
							{
								alternativeEquipments = new List<ValueTuple<EquipmentIndex, EquipmentElement>>(list);
							}
							else
							{
								alternativeEquipments.AddRange(list);
							}
						}
					}
					return alternativeEquipments;
				}
			}
			if (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Any || (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops)))
			{
				if (alternativeEquipments == null)
				{
					alternativeEquipments = new List<ValueTuple<EquipmentIndex, EquipmentElement>>(this._groups.GetRandomElement<List<ValueTuple<EquipmentIndex, EquipmentElement>>>());
				}
				else
				{
					alternativeEquipments.AddRange(this._groups.GetRandomElement<List<ValueTuple<EquipmentIndex, EquipmentElement>>>());
				}
			}
			return alternativeEquipments;
		}

		protected static string StringType = "RandomEquipmentOnSpawn";

		private MBList<List<ValueTuple<EquipmentIndex, EquipmentElement>>> _groups;
	}
}
