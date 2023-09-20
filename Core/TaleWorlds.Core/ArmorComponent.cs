using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x0200000A RID: 10
	public class ArmorComponent : ItemComponent
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00002507 File Offset: 0x00000707
		// (set) Token: 0x06000049 RID: 73 RVA: 0x0000250F File Offset: 0x0000070F
		public int HeadArmor { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002518 File Offset: 0x00000718
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002520 File Offset: 0x00000720
		public int BodyArmor { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00002529 File Offset: 0x00000729
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00002531 File Offset: 0x00000731
		public int LegArmor { get; private set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600004E RID: 78 RVA: 0x0000253A File Offset: 0x0000073A
		// (set) Token: 0x0600004F RID: 79 RVA: 0x00002542 File Offset: 0x00000742
		public int ArmArmor { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000050 RID: 80 RVA: 0x0000254B File Offset: 0x0000074B
		// (set) Token: 0x06000051 RID: 81 RVA: 0x00002553 File Offset: 0x00000753
		public int ManeuverBonus { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000052 RID: 82 RVA: 0x0000255C File Offset: 0x0000075C
		// (set) Token: 0x06000053 RID: 83 RVA: 0x00002564 File Offset: 0x00000764
		public int SpeedBonus { get; private set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000054 RID: 84 RVA: 0x0000256D File Offset: 0x0000076D
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00002575 File Offset: 0x00000775
		public int ChargeBonus { get; private set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000056 RID: 86 RVA: 0x0000257E File Offset: 0x0000077E
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00002586 File Offset: 0x00000786
		public int FamilyType { get; private set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000058 RID: 88 RVA: 0x0000258F File Offset: 0x0000078F
		// (set) Token: 0x06000059 RID: 89 RVA: 0x00002597 File Offset: 0x00000797
		public bool MultiMeshHasGenderVariations { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600005A RID: 90 RVA: 0x000025A0 File Offset: 0x000007A0
		// (set) Token: 0x0600005B RID: 91 RVA: 0x000025A8 File Offset: 0x000007A8
		public ArmorComponent.ArmorMaterialTypes MaterialType { get; private set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600005C RID: 92 RVA: 0x000025B1 File Offset: 0x000007B1
		// (set) Token: 0x0600005D RID: 93 RVA: 0x000025B9 File Offset: 0x000007B9
		public SkinMask MeshesMask { get; private set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600005E RID: 94 RVA: 0x000025C2 File Offset: 0x000007C2
		// (set) Token: 0x0600005F RID: 95 RVA: 0x000025CA File Offset: 0x000007CA
		public ArmorComponent.BodyMeshTypes BodyMeshType { get; private set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000025D3 File Offset: 0x000007D3
		// (set) Token: 0x06000061 RID: 97 RVA: 0x000025DB File Offset: 0x000007DB
		public ArmorComponent.BodyDeformTypes BodyDeformType { get; private set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000025E4 File Offset: 0x000007E4
		// (set) Token: 0x06000063 RID: 99 RVA: 0x000025EC File Offset: 0x000007EC
		public ArmorComponent.HairCoverTypes HairCoverType { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000064 RID: 100 RVA: 0x000025F5 File Offset: 0x000007F5
		// (set) Token: 0x06000065 RID: 101 RVA: 0x000025FD File Offset: 0x000007FD
		public ArmorComponent.BeardCoverTypes BeardCoverType { get; private set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00002606 File Offset: 0x00000806
		// (set) Token: 0x06000067 RID: 103 RVA: 0x0000260E File Offset: 0x0000080E
		public ArmorComponent.HorseHarnessCoverTypes ManeCoverType { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00002617 File Offset: 0x00000817
		// (set) Token: 0x06000069 RID: 105 RVA: 0x0000261F File Offset: 0x0000081F
		public ArmorComponent.HorseTailCoverTypes TailCoverType { get; private set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00002628 File Offset: 0x00000828
		// (set) Token: 0x0600006B RID: 107 RVA: 0x00002630 File Offset: 0x00000830
		public string ReinsMesh { get; private set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00002639 File Offset: 0x00000839
		public string ReinsRopeMesh
		{
			get
			{
				return this.ReinsMesh + "_rope";
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000264B File Offset: 0x0000084B
		public ArmorComponent(ItemObject item)
		{
			base.Item = item;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000265C File Offset: 0x0000085C
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

		// Token: 0x0600006F RID: 111 RVA: 0x00002750 File Offset: 0x00000950
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

		// Token: 0x06000070 RID: 112 RVA: 0x00002C99 File Offset: 0x00000E99
		internal static void AutoGeneratedStaticCollectObjectsArmorComponent(object o, List<object> collectedObjects)
		{
			((ArmorComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002CA7 File Offset: 0x00000EA7
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x020000CC RID: 204
		public enum ArmorMaterialTypes : sbyte
		{
			// Token: 0x040005D8 RID: 1496
			None,
			// Token: 0x040005D9 RID: 1497
			Cloth,
			// Token: 0x040005DA RID: 1498
			Leather,
			// Token: 0x040005DB RID: 1499
			Chainmail,
			// Token: 0x040005DC RID: 1500
			Plate
		}

		// Token: 0x020000CD RID: 205
		public enum HairCoverTypes
		{
			// Token: 0x040005DE RID: 1502
			None,
			// Token: 0x040005DF RID: 1503
			Type1,
			// Token: 0x040005E0 RID: 1504
			Type2,
			// Token: 0x040005E1 RID: 1505
			Type3,
			// Token: 0x040005E2 RID: 1506
			Type4,
			// Token: 0x040005E3 RID: 1507
			All,
			// Token: 0x040005E4 RID: 1508
			NumHairCoverTypes
		}

		// Token: 0x020000CE RID: 206
		public enum BeardCoverTypes
		{
			// Token: 0x040005E6 RID: 1510
			None,
			// Token: 0x040005E7 RID: 1511
			Type1,
			// Token: 0x040005E8 RID: 1512
			Type2,
			// Token: 0x040005E9 RID: 1513
			Type3,
			// Token: 0x040005EA RID: 1514
			Type4,
			// Token: 0x040005EB RID: 1515
			All,
			// Token: 0x040005EC RID: 1516
			NumBeardBoverTypes
		}

		// Token: 0x020000CF RID: 207
		public enum HorseHarnessCoverTypes
		{
			// Token: 0x040005EE RID: 1518
			None,
			// Token: 0x040005EF RID: 1519
			Type1,
			// Token: 0x040005F0 RID: 1520
			Type2,
			// Token: 0x040005F1 RID: 1521
			All,
			// Token: 0x040005F2 RID: 1522
			HorseHarnessCoverTypes
		}

		// Token: 0x020000D0 RID: 208
		public enum HorseTailCoverTypes
		{
			// Token: 0x040005F4 RID: 1524
			None,
			// Token: 0x040005F5 RID: 1525
			All
		}

		// Token: 0x020000D1 RID: 209
		public enum BodyMeshTypes
		{
			// Token: 0x040005F7 RID: 1527
			Normal,
			// Token: 0x040005F8 RID: 1528
			Upperbody,
			// Token: 0x040005F9 RID: 1529
			Shoulders,
			// Token: 0x040005FA RID: 1530
			BodyMeshTypesNum
		}

		// Token: 0x020000D2 RID: 210
		public enum BodyDeformTypes
		{
			// Token: 0x040005FC RID: 1532
			Medium,
			// Token: 0x040005FD RID: 1533
			Large,
			// Token: 0x040005FE RID: 1534
			Skinny,
			// Token: 0x040005FF RID: 1535
			BodyMeshTypesNum
		}
	}
}
