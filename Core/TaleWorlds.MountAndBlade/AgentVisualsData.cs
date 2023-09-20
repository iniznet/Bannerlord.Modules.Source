using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002B3 RID: 691
	public class AgentVisualsData
	{
		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06002636 RID: 9782 RVA: 0x00090B7A File Offset: 0x0008ED7A
		// (set) Token: 0x06002637 RID: 9783 RVA: 0x00090B82 File Offset: 0x0008ED82
		public MBActionSet ActionSetData { get; private set; }

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06002638 RID: 9784 RVA: 0x00090B8B File Offset: 0x0008ED8B
		// (set) Token: 0x06002639 RID: 9785 RVA: 0x00090B93 File Offset: 0x0008ED93
		public MatrixFrame FrameData { get; private set; }

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x0600263A RID: 9786 RVA: 0x00090B9C File Offset: 0x0008ED9C
		// (set) Token: 0x0600263B RID: 9787 RVA: 0x00090BA4 File Offset: 0x0008EDA4
		public BodyProperties BodyPropertiesData { get; private set; }

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x0600263C RID: 9788 RVA: 0x00090BAD File Offset: 0x0008EDAD
		// (set) Token: 0x0600263D RID: 9789 RVA: 0x00090BB5 File Offset: 0x0008EDB5
		public Equipment EquipmentData { get; private set; }

		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x0600263E RID: 9790 RVA: 0x00090BBE File Offset: 0x0008EDBE
		// (set) Token: 0x0600263F RID: 9791 RVA: 0x00090BC6 File Offset: 0x0008EDC6
		public int RightWieldedItemIndexData { get; private set; }

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x06002640 RID: 9792 RVA: 0x00090BCF File Offset: 0x0008EDCF
		// (set) Token: 0x06002641 RID: 9793 RVA: 0x00090BD7 File Offset: 0x0008EDD7
		public int LeftWieldedItemIndexData { get; private set; }

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06002642 RID: 9794 RVA: 0x00090BE0 File Offset: 0x0008EDE0
		// (set) Token: 0x06002643 RID: 9795 RVA: 0x00090BE8 File Offset: 0x0008EDE8
		public SkeletonType SkeletonTypeData { get; private set; }

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06002644 RID: 9796 RVA: 0x00090BF1 File Offset: 0x0008EDF1
		// (set) Token: 0x06002645 RID: 9797 RVA: 0x00090BF9 File Offset: 0x0008EDF9
		public Banner BannerData { get; private set; }

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06002646 RID: 9798 RVA: 0x00090C02 File Offset: 0x0008EE02
		// (set) Token: 0x06002647 RID: 9799 RVA: 0x00090C0A File Offset: 0x0008EE0A
		public GameEntity CachedWeaponSlot0Entity { get; private set; }

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06002648 RID: 9800 RVA: 0x00090C13 File Offset: 0x0008EE13
		// (set) Token: 0x06002649 RID: 9801 RVA: 0x00090C1B File Offset: 0x0008EE1B
		public GameEntity CachedWeaponSlot1Entity { get; private set; }

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x0600264A RID: 9802 RVA: 0x00090C24 File Offset: 0x0008EE24
		// (set) Token: 0x0600264B RID: 9803 RVA: 0x00090C2C File Offset: 0x0008EE2C
		public GameEntity CachedWeaponSlot2Entity { get; private set; }

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x0600264C RID: 9804 RVA: 0x00090C35 File Offset: 0x0008EE35
		// (set) Token: 0x0600264D RID: 9805 RVA: 0x00090C3D File Offset: 0x0008EE3D
		public GameEntity CachedWeaponSlot3Entity { get; private set; }

		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x0600264E RID: 9806 RVA: 0x00090C46 File Offset: 0x0008EE46
		// (set) Token: 0x0600264F RID: 9807 RVA: 0x00090C4E File Offset: 0x0008EE4E
		public GameEntity CachedWeaponSlot4Entity { get; private set; }

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x06002650 RID: 9808 RVA: 0x00090C57 File Offset: 0x0008EE57
		// (set) Token: 0x06002651 RID: 9809 RVA: 0x00090C5F File Offset: 0x0008EE5F
		public Scene SceneData { get; private set; }

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x06002652 RID: 9810 RVA: 0x00090C68 File Offset: 0x0008EE68
		// (set) Token: 0x06002653 RID: 9811 RVA: 0x00090C70 File Offset: 0x0008EE70
		public Monster MonsterData { get; private set; }

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x06002654 RID: 9812 RVA: 0x00090C79 File Offset: 0x0008EE79
		// (set) Token: 0x06002655 RID: 9813 RVA: 0x00090C81 File Offset: 0x0008EE81
		public bool PrepareImmediatelyData { get; private set; }

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x06002656 RID: 9814 RVA: 0x00090C8A File Offset: 0x0008EE8A
		// (set) Token: 0x06002657 RID: 9815 RVA: 0x00090C92 File Offset: 0x0008EE92
		public bool UseScaledWeaponsData { get; private set; }

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x06002658 RID: 9816 RVA: 0x00090C9B File Offset: 0x0008EE9B
		// (set) Token: 0x06002659 RID: 9817 RVA: 0x00090CA3 File Offset: 0x0008EEA3
		public bool UseTranslucencyData { get; private set; }

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x0600265A RID: 9818 RVA: 0x00090CAC File Offset: 0x0008EEAC
		// (set) Token: 0x0600265B RID: 9819 RVA: 0x00090CB4 File Offset: 0x0008EEB4
		public bool UseTesselationData { get; private set; }

		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x0600265C RID: 9820 RVA: 0x00090CBD File Offset: 0x0008EEBD
		// (set) Token: 0x0600265D RID: 9821 RVA: 0x00090CC5 File Offset: 0x0008EEC5
		public bool UseMorphAnimsData { get; private set; }

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x0600265E RID: 9822 RVA: 0x00090CCE File Offset: 0x0008EECE
		// (set) Token: 0x0600265F RID: 9823 RVA: 0x00090CD6 File Offset: 0x0008EED6
		public uint ClothColor1Data { get; private set; }

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x06002660 RID: 9824 RVA: 0x00090CDF File Offset: 0x0008EEDF
		// (set) Token: 0x06002661 RID: 9825 RVA: 0x00090CE7 File Offset: 0x0008EEE7
		public uint ClothColor2Data { get; private set; }

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x06002662 RID: 9826 RVA: 0x00090CF0 File Offset: 0x0008EEF0
		// (set) Token: 0x06002663 RID: 9827 RVA: 0x00090CF8 File Offset: 0x0008EEF8
		public float ScaleData { get; private set; }

		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06002664 RID: 9828 RVA: 0x00090D01 File Offset: 0x0008EF01
		// (set) Token: 0x06002665 RID: 9829 RVA: 0x00090D09 File Offset: 0x0008EF09
		public string CharacterObjectStringIdData { get; private set; }

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06002666 RID: 9830 RVA: 0x00090D12 File Offset: 0x0008EF12
		// (set) Token: 0x06002667 RID: 9831 RVA: 0x00090D1A File Offset: 0x0008EF1A
		public ActionIndexCache ActionCodeData { get; private set; } = ActionIndexCache.act_none;

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x06002668 RID: 9832 RVA: 0x00090D23 File Offset: 0x0008EF23
		// (set) Token: 0x06002669 RID: 9833 RVA: 0x00090D2B File Offset: 0x0008EF2B
		public GameEntity EntityData { get; private set; }

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x0600266A RID: 9834 RVA: 0x00090D34 File Offset: 0x0008EF34
		// (set) Token: 0x0600266B RID: 9835 RVA: 0x00090D3C File Offset: 0x0008EF3C
		public bool HasClippingPlaneData { get; private set; }

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x0600266C RID: 9836 RVA: 0x00090D45 File Offset: 0x0008EF45
		// (set) Token: 0x0600266D RID: 9837 RVA: 0x00090D4D File Offset: 0x0008EF4D
		public string MountCreationKeyData { get; private set; }

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x0600266E RID: 9838 RVA: 0x00090D56 File Offset: 0x0008EF56
		// (set) Token: 0x0600266F RID: 9839 RVA: 0x00090D5E File Offset: 0x0008EF5E
		public bool AddColorRandomnessData { get; private set; }

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x06002670 RID: 9840 RVA: 0x00090D67 File Offset: 0x0008EF67
		// (set) Token: 0x06002671 RID: 9841 RVA: 0x00090D6F File Offset: 0x0008EF6F
		public int RaceData { get; private set; }

		// Token: 0x06002672 RID: 9842 RVA: 0x00090D78 File Offset: 0x0008EF78
		public AgentVisualsData(AgentVisualsData agentVisualsData)
		{
			this.AgentVisuals = agentVisualsData.AgentVisuals;
			this.ActionSetData = agentVisualsData.ActionSetData;
			this.FrameData = agentVisualsData.FrameData;
			this.BodyPropertiesData = agentVisualsData.BodyPropertiesData;
			this.EquipmentData = agentVisualsData.EquipmentData;
			this.RightWieldedItemIndexData = agentVisualsData.RightWieldedItemIndexData;
			this.LeftWieldedItemIndexData = agentVisualsData.LeftWieldedItemIndexData;
			this.SkeletonTypeData = agentVisualsData.SkeletonTypeData;
			this.BannerData = agentVisualsData.BannerData;
			this.CachedWeaponSlot0Entity = agentVisualsData.CachedWeaponSlot0Entity;
			this.CachedWeaponSlot1Entity = agentVisualsData.CachedWeaponSlot1Entity;
			this.CachedWeaponSlot2Entity = agentVisualsData.CachedWeaponSlot2Entity;
			this.CachedWeaponSlot3Entity = agentVisualsData.CachedWeaponSlot3Entity;
			this.CachedWeaponSlot4Entity = agentVisualsData.CachedWeaponSlot4Entity;
			this.SceneData = agentVisualsData.SceneData;
			this.MonsterData = agentVisualsData.MonsterData;
			this.PrepareImmediatelyData = agentVisualsData.PrepareImmediatelyData;
			this.UseScaledWeaponsData = agentVisualsData.UseScaledWeaponsData;
			this.UseTranslucencyData = agentVisualsData.UseTranslucencyData;
			this.UseTesselationData = agentVisualsData.UseTesselationData;
			this.UseMorphAnimsData = agentVisualsData.UseMorphAnimsData;
			this.ClothColor1Data = agentVisualsData.ClothColor1Data;
			this.ClothColor2Data = agentVisualsData.ClothColor2Data;
			this.ScaleData = agentVisualsData.ScaleData;
			this.ActionCodeData = agentVisualsData.ActionCodeData;
			this.EntityData = agentVisualsData.EntityData;
			this.CharacterObjectStringIdData = agentVisualsData.CharacterObjectStringIdData;
			this.HasClippingPlaneData = agentVisualsData.HasClippingPlaneData;
			this.MountCreationKeyData = agentVisualsData.MountCreationKeyData;
			this.AddColorRandomnessData = agentVisualsData.AddColorRandomnessData;
			this.RaceData = agentVisualsData.RaceData;
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x00090F0A File Offset: 0x0008F10A
		public AgentVisualsData()
		{
			this.ClothColor1Data = uint.MaxValue;
			this.ClothColor2Data = uint.MaxValue;
			this.RightWieldedItemIndexData = -1;
			this.LeftWieldedItemIndexData = -1;
			this.ScaleData = 0f;
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x00090F44 File Offset: 0x0008F144
		public AgentVisualsData Equipment(Equipment equipment)
		{
			this.EquipmentData = equipment;
			return this;
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x00090F4E File Offset: 0x0008F14E
		public AgentVisualsData BodyProperties(BodyProperties bodyProperties)
		{
			this.BodyPropertiesData = bodyProperties;
			return this;
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x00090F58 File Offset: 0x0008F158
		public AgentVisualsData Frame(MatrixFrame frame)
		{
			this.FrameData = frame;
			return this;
		}

		// Token: 0x06002677 RID: 9847 RVA: 0x00090F62 File Offset: 0x0008F162
		public AgentVisualsData ActionSet(MBActionSet actionSet)
		{
			this.ActionSetData = actionSet;
			return this;
		}

		// Token: 0x06002678 RID: 9848 RVA: 0x00090F6C File Offset: 0x0008F16C
		public AgentVisualsData Scene(Scene scene)
		{
			this.SceneData = scene;
			return this;
		}

		// Token: 0x06002679 RID: 9849 RVA: 0x00090F76 File Offset: 0x0008F176
		public AgentVisualsData Monster(Monster monster)
		{
			this.MonsterData = monster;
			return this;
		}

		// Token: 0x0600267A RID: 9850 RVA: 0x00090F80 File Offset: 0x0008F180
		public AgentVisualsData PrepareImmediately(bool prepareImmediately)
		{
			this.PrepareImmediatelyData = prepareImmediately;
			return this;
		}

		// Token: 0x0600267B RID: 9851 RVA: 0x00090F8A File Offset: 0x0008F18A
		public AgentVisualsData UseScaledWeapons(bool useScaledWeapons)
		{
			this.UseScaledWeaponsData = useScaledWeapons;
			return this;
		}

		// Token: 0x0600267C RID: 9852 RVA: 0x00090F94 File Offset: 0x0008F194
		public AgentVisualsData SkeletonType(SkeletonType skeletonType)
		{
			this.SkeletonTypeData = skeletonType;
			return this;
		}

		// Token: 0x0600267D RID: 9853 RVA: 0x00090F9E File Offset: 0x0008F19E
		public AgentVisualsData UseMorphAnims(bool useMorphAnims)
		{
			this.UseMorphAnimsData = useMorphAnims;
			return this;
		}

		// Token: 0x0600267E RID: 9854 RVA: 0x00090FA8 File Offset: 0x0008F1A8
		public AgentVisualsData ClothColor1(uint clothColor1)
		{
			this.ClothColor1Data = clothColor1;
			return this;
		}

		// Token: 0x0600267F RID: 9855 RVA: 0x00090FB2 File Offset: 0x0008F1B2
		public AgentVisualsData ClothColor2(uint clothColor2)
		{
			this.ClothColor2Data = clothColor2;
			return this;
		}

		// Token: 0x06002680 RID: 9856 RVA: 0x00090FBC File Offset: 0x0008F1BC
		public AgentVisualsData Banner(Banner banner)
		{
			this.BannerData = banner;
			return this;
		}

		// Token: 0x06002681 RID: 9857 RVA: 0x00090FC6 File Offset: 0x0008F1C6
		public AgentVisualsData Race(int race)
		{
			this.RaceData = race;
			return this;
		}

		// Token: 0x06002682 RID: 9858 RVA: 0x00090FD0 File Offset: 0x0008F1D0
		public GameEntity GetCachedWeaponEntity(EquipmentIndex slotIndex)
		{
			switch (slotIndex)
			{
			case EquipmentIndex.WeaponItemBeginSlot:
				return this.CachedWeaponSlot0Entity;
			case EquipmentIndex.Weapon1:
				return this.CachedWeaponSlot1Entity;
			case EquipmentIndex.Weapon2:
				return this.CachedWeaponSlot2Entity;
			case EquipmentIndex.Weapon3:
				return this.CachedWeaponSlot3Entity;
			case EquipmentIndex.ExtraWeaponSlot:
				return this.CachedWeaponSlot4Entity;
			default:
				return null;
			}
		}

		// Token: 0x06002683 RID: 9859 RVA: 0x00091020 File Offset: 0x0008F220
		public AgentVisualsData CachedWeaponEntity(EquipmentIndex slotIndex, GameEntity cachedWeaponEntity)
		{
			switch (slotIndex)
			{
			case EquipmentIndex.WeaponItemBeginSlot:
				this.CachedWeaponSlot0Entity = cachedWeaponEntity;
				break;
			case EquipmentIndex.Weapon1:
				this.CachedWeaponSlot1Entity = cachedWeaponEntity;
				break;
			case EquipmentIndex.Weapon2:
				this.CachedWeaponSlot2Entity = cachedWeaponEntity;
				break;
			case EquipmentIndex.Weapon3:
				this.CachedWeaponSlot3Entity = cachedWeaponEntity;
				break;
			case EquipmentIndex.ExtraWeaponSlot:
				this.CachedWeaponSlot4Entity = cachedWeaponEntity;
				break;
			}
			return this;
		}

		// Token: 0x06002684 RID: 9860 RVA: 0x00091075 File Offset: 0x0008F275
		public AgentVisualsData Entity(GameEntity entity)
		{
			this.EntityData = entity;
			return this;
		}

		// Token: 0x06002685 RID: 9861 RVA: 0x0009107F File Offset: 0x0008F27F
		public AgentVisualsData UseTranslucency(bool useTranslucency)
		{
			this.UseTranslucencyData = useTranslucency;
			return this;
		}

		// Token: 0x06002686 RID: 9862 RVA: 0x00091089 File Offset: 0x0008F289
		public AgentVisualsData UseTesselation(bool useTesselation)
		{
			this.UseTesselationData = useTesselation;
			return this;
		}

		// Token: 0x06002687 RID: 9863 RVA: 0x00091093 File Offset: 0x0008F293
		public AgentVisualsData ActionCode(ActionIndexCache actionCode)
		{
			this.ActionCodeData = actionCode;
			return this;
		}

		// Token: 0x06002688 RID: 9864 RVA: 0x0009109D File Offset: 0x0008F29D
		public AgentVisualsData RightWieldedItemIndex(int rightWieldedItemIndex)
		{
			this.RightWieldedItemIndexData = rightWieldedItemIndex;
			return this;
		}

		// Token: 0x06002689 RID: 9865 RVA: 0x000910A7 File Offset: 0x0008F2A7
		public AgentVisualsData LeftWieldedItemIndex(int leftWieldedItemIndex)
		{
			this.LeftWieldedItemIndexData = leftWieldedItemIndex;
			return this;
		}

		// Token: 0x0600268A RID: 9866 RVA: 0x000910B1 File Offset: 0x0008F2B1
		public AgentVisualsData Scale(float scale)
		{
			this.ScaleData = scale;
			return this;
		}

		// Token: 0x0600268B RID: 9867 RVA: 0x000910BB File Offset: 0x0008F2BB
		public AgentVisualsData CharacterObjectStringId(string characterObjectStringId)
		{
			this.CharacterObjectStringIdData = characterObjectStringId;
			return this;
		}

		// Token: 0x0600268C RID: 9868 RVA: 0x000910C5 File Offset: 0x0008F2C5
		public AgentVisualsData HasClippingPlane(bool hasClippingPlane)
		{
			this.HasClippingPlaneData = hasClippingPlane;
			return this;
		}

		// Token: 0x0600268D RID: 9869 RVA: 0x000910CF File Offset: 0x0008F2CF
		public AgentVisualsData MountCreationKey(string mountCreationKey)
		{
			this.MountCreationKeyData = mountCreationKey;
			return this;
		}

		// Token: 0x0600268E RID: 9870 RVA: 0x000910D9 File Offset: 0x0008F2D9
		public AgentVisualsData AddColorRandomness(bool addColorRandomness)
		{
			this.AddColorRandomnessData = addColorRandomness;
			return this;
		}

		// Token: 0x04000E30 RID: 3632
		public MBAgentVisuals AgentVisuals;
	}
}
