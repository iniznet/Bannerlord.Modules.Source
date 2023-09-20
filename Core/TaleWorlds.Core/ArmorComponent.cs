using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public class ArmorComponent : ItemComponent
	{
		public int HeadArmor { get; private set; }

		public int BodyArmor { get; private set; }

		public int LegArmor { get; private set; }

		public int ArmArmor { get; private set; }

		public int ManeuverBonus { get; private set; }

		public int SpeedBonus { get; private set; }

		public int ChargeBonus { get; private set; }

		public int FamilyType { get; private set; }

		public bool MultiMeshHasGenderVariations { get; private set; }

		public ArmorComponent.ArmorMaterialTypes MaterialType { get; private set; }

		public SkinMask MeshesMask { get; private set; }

		public ArmorComponent.BodyMeshTypes BodyMeshType { get; private set; }

		public ArmorComponent.BodyDeformTypes BodyDeformType { get; private set; }

		public ArmorComponent.HairCoverTypes HairCoverType { get; private set; }

		public ArmorComponent.BeardCoverTypes BeardCoverType { get; private set; }

		public ArmorComponent.HorseHarnessCoverTypes ManeCoverType { get; private set; }

		public ArmorComponent.HorseTailCoverTypes TailCoverType { get; private set; }

		public string ReinsMesh { get; private set; }

		public string ReinsRopeMesh
		{
			get
			{
				return this.ReinsMesh + "_rope";
			}
		}

		public ArmorComponent(ItemObject item)
		{
			base.Item = item;
		}

		public override ItemComponent GetCopy()
		{
			return new ArmorComponent(base.Item)
			{
				HeadArmor = this.HeadArmor,
				BodyArmor = this.BodyArmor,
				LegArmor = this.LegArmor,
				ArmArmor = this.ArmArmor,
				MultiMeshHasGenderVariations = this.MultiMeshHasGenderVariations,
				MaterialType = this.MaterialType,
				MeshesMask = this.MeshesMask,
				BodyMeshType = this.BodyMeshType,
				HairCoverType = this.HairCoverType,
				BeardCoverType = this.BeardCoverType,
				ManeCoverType = this.ManeCoverType,
				TailCoverType = this.TailCoverType,
				BodyDeformType = this.BodyDeformType,
				ManeuverBonus = this.ManeuverBonus,
				SpeedBonus = this.SpeedBonus,
				ChargeBonus = this.ChargeBonus,
				FamilyType = this.FamilyType,
				ReinsMesh = this.ReinsMesh
			};
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			this.HeadArmor = ((node.Attributes["head_armor"] != null) ? int.Parse(node.Attributes["head_armor"].Value) : 0);
			this.BodyArmor = ((node.Attributes["body_armor"] != null) ? int.Parse(node.Attributes["body_armor"].Value) : 0);
			this.LegArmor = ((node.Attributes["leg_armor"] != null) ? int.Parse(node.Attributes["leg_armor"].Value) : 0);
			this.ArmArmor = ((node.Attributes["arm_armor"] != null) ? int.Parse(node.Attributes["arm_armor"].Value) : 0);
			this.FamilyType = ((node.Attributes["family_type"] != null) ? int.Parse(node.Attributes["family_type"].Value) : 0);
			this.ManeuverBonus = ((node.Attributes["maneuver_bonus"] != null) ? int.Parse(node.Attributes["maneuver_bonus"].Value) : 0);
			this.SpeedBonus = ((node.Attributes["speed_bonus"] != null) ? int.Parse(node.Attributes["speed_bonus"].Value) : 0);
			this.ChargeBonus = ((node.Attributes["charge_bonus"] != null) ? int.Parse(node.Attributes["charge_bonus"].Value) : 0);
			this.MaterialType = ((node.Attributes["material_type"] != null) ? ((ArmorComponent.ArmorMaterialTypes)Enum.Parse(typeof(ArmorComponent.ArmorMaterialTypes), node.Attributes["material_type"].Value)) : ArmorComponent.ArmorMaterialTypes.None);
			ArmorComponent.ArmorMaterialTypes materialType = this.MaterialType;
			this.MultiMeshHasGenderVariations = true;
			if (node.Attributes["has_gender_variations"] != null)
			{
				this.MultiMeshHasGenderVariations = Convert.ToBoolean(node.Attributes["has_gender_variations"].Value);
			}
			this.BodyMeshType = ArmorComponent.BodyMeshTypes.Normal;
			if (node.Attributes["body_mesh_type"] != null)
			{
				string value = node.Attributes["body_mesh_type"].Value;
				if (value == "upperbody")
				{
					this.BodyMeshType = ArmorComponent.BodyMeshTypes.Upperbody;
				}
				else if (value == "shoulders")
				{
					this.BodyMeshType = ArmorComponent.BodyMeshTypes.Shoulders;
				}
			}
			this.BodyDeformType = ArmorComponent.BodyDeformTypes.Medium;
			if (node.Attributes["body_deform_type"] != null)
			{
				string value2 = node.Attributes["body_deform_type"].Value;
				if (value2 == "large")
				{
					this.BodyDeformType = ArmorComponent.BodyDeformTypes.Large;
				}
				else if (value2 == "skinny")
				{
					this.BodyDeformType = ArmorComponent.BodyDeformTypes.Skinny;
				}
			}
			this.HairCoverType = ((node.Attributes["hair_cover_type"] != null) ? ((ArmorComponent.HairCoverTypes)Enum.Parse(typeof(ArmorComponent.HairCoverTypes), node.Attributes["hair_cover_type"].Value, true)) : ArmorComponent.HairCoverTypes.None);
			this.BeardCoverType = ((node.Attributes["beard_cover_type"] != null) ? ((ArmorComponent.BeardCoverTypes)Enum.Parse(typeof(ArmorComponent.BeardCoverTypes), node.Attributes["beard_cover_type"].Value, true)) : ArmorComponent.BeardCoverTypes.None);
			this.ManeCoverType = ((node.Attributes["mane_cover_type"] != null) ? ((ArmorComponent.HorseHarnessCoverTypes)Enum.Parse(typeof(ArmorComponent.HorseHarnessCoverTypes), node.Attributes["mane_cover_type"].Value, true)) : ArmorComponent.HorseHarnessCoverTypes.None);
			this.TailCoverType = ((node.Attributes["tail_cover_type"] != null) ? ((ArmorComponent.HorseTailCoverTypes)Enum.Parse(typeof(ArmorComponent.HorseTailCoverTypes), node.Attributes["tail_cover_type"].Value, true)) : ArmorComponent.HorseTailCoverTypes.None);
			this.ReinsMesh = ((node.Attributes["reins_mesh"] != null) ? node.Attributes["reins_mesh"].Value : "");
			bool flag = node.Attributes["covers_head"] != null && Convert.ToBoolean(node.Attributes["covers_head"].Value);
			bool flag2 = node.Attributes["covers_body"] != null && Convert.ToBoolean(node.Attributes["covers_body"].Value);
			bool flag3 = node.Attributes["covers_hands"] != null && Convert.ToBoolean(node.Attributes["covers_hands"].Value);
			bool flag4 = node.Attributes["covers_legs"] != null && Convert.ToBoolean(node.Attributes["covers_legs"].Value);
			if (!flag)
			{
				this.MeshesMask |= SkinMask.HeadVisible;
			}
			if (!flag2)
			{
				this.MeshesMask |= SkinMask.BodyVisible;
			}
			if (!flag3)
			{
				this.MeshesMask |= SkinMask.HandsVisible;
			}
			if (!flag4)
			{
				this.MeshesMask |= SkinMask.LegsVisible;
			}
		}

		internal static void AutoGeneratedStaticCollectObjectsArmorComponent(object o, List<object> collectedObjects)
		{
			((ArmorComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		public enum ArmorMaterialTypes : sbyte
		{
			None,
			Cloth,
			Leather,
			Chainmail,
			Plate
		}

		public enum HairCoverTypes
		{
			None,
			Type1,
			Type2,
			Type3,
			Type4,
			All,
			NumHairCoverTypes
		}

		public enum BeardCoverTypes
		{
			None,
			Type1,
			Type2,
			Type3,
			Type4,
			All,
			NumBeardBoverTypes
		}

		public enum HorseHarnessCoverTypes
		{
			None,
			Type1,
			Type2,
			All,
			HorseHarnessCoverTypes
		}

		public enum HorseTailCoverTypes
		{
			None,
			All
		}

		public enum BodyMeshTypes
		{
			Normal,
			Upperbody,
			Shoulders,
			BodyMeshTypesNum
		}

		public enum BodyDeformTypes
		{
			Medium,
			Large,
			Skinny,
			BodyMeshTypesNum
		}
	}
}
