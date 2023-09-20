using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000EA RID: 234
	public class AgentBuildData
	{
		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000AAF RID: 2735 RVA: 0x0001543C File Offset: 0x0001363C
		// (set) Token: 0x06000AB0 RID: 2736 RVA: 0x00015444 File Offset: 0x00013644
		public AgentData AgentData { get; private set; }

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000AB1 RID: 2737 RVA: 0x0001544D File Offset: 0x0001364D
		public BasicCharacterObject AgentCharacter
		{
			get
			{
				return this.AgentData.AgentCharacter;
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000AB2 RID: 2738 RVA: 0x0001545A File Offset: 0x0001365A
		public Monster AgentMonster
		{
			get
			{
				return this.AgentData.AgentMonster;
			}
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000AB3 RID: 2739 RVA: 0x00015467 File Offset: 0x00013667
		public Equipment AgentOverridenSpawnEquipment
		{
			get
			{
				return this.AgentData.AgentOverridenEquipment;
			}
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000AB4 RID: 2740 RVA: 0x00015474 File Offset: 0x00013674
		// (set) Token: 0x06000AB5 RID: 2741 RVA: 0x0001547C File Offset: 0x0001367C
		public MissionEquipment AgentOverridenSpawnMissionEquipment { get; private set; }

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000AB6 RID: 2742 RVA: 0x00015485 File Offset: 0x00013685
		public int AgentEquipmentSeed
		{
			get
			{
				return this.AgentData.AgentEquipmentSeed;
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000AB7 RID: 2743 RVA: 0x00015492 File Offset: 0x00013692
		public bool AgentNoHorses
		{
			get
			{
				return this.AgentData.AgentNoHorses;
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000AB8 RID: 2744 RVA: 0x0001549F File Offset: 0x0001369F
		public string AgentMountKey
		{
			get
			{
				return this.AgentData.AgentMountKey;
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000AB9 RID: 2745 RVA: 0x000154AC File Offset: 0x000136AC
		public bool AgentNoWeapons
		{
			get
			{
				return this.AgentData.AgentNoWeapons;
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000ABA RID: 2746 RVA: 0x000154B9 File Offset: 0x000136B9
		public bool AgentNoArmor
		{
			get
			{
				return this.AgentData.AgentNoArmor;
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000ABB RID: 2747 RVA: 0x000154C6 File Offset: 0x000136C6
		public bool AgentFixedEquipment
		{
			get
			{
				return this.AgentData.AgentFixedEquipment;
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000ABC RID: 2748 RVA: 0x000154D3 File Offset: 0x000136D3
		public bool AgentCivilianEquipment
		{
			get
			{
				return this.AgentData.AgentCivilianEquipment;
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000ABD RID: 2749 RVA: 0x000154E0 File Offset: 0x000136E0
		public uint AgentClothingColor1
		{
			get
			{
				return this.AgentData.AgentClothingColor1;
			}
		}

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000ABE RID: 2750 RVA: 0x000154ED File Offset: 0x000136ED
		public uint AgentClothingColor2
		{
			get
			{
				return this.AgentData.AgentClothingColor2;
			}
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000ABF RID: 2751 RVA: 0x000154FA File Offset: 0x000136FA
		public bool BodyPropertiesOverriden
		{
			get
			{
				return this.AgentData.BodyPropertiesOverriden;
			}
		}

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000AC0 RID: 2752 RVA: 0x00015507 File Offset: 0x00013707
		public BodyProperties AgentBodyProperties
		{
			get
			{
				return this.AgentData.AgentBodyProperties;
			}
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000AC1 RID: 2753 RVA: 0x00015514 File Offset: 0x00013714
		public bool AgeOverriden
		{
			get
			{
				return this.AgentData.AgeOverriden;
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000AC2 RID: 2754 RVA: 0x00015521 File Offset: 0x00013721
		public int AgentAge
		{
			get
			{
				return this.AgentData.AgentAge;
			}
		}

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06000AC3 RID: 2755 RVA: 0x0001552E File Offset: 0x0001372E
		public bool GenderOverriden
		{
			get
			{
				return this.AgentData.GenderOverriden;
			}
		}

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000AC4 RID: 2756 RVA: 0x0001553B File Offset: 0x0001373B
		public bool AgentIsFemale
		{
			get
			{
				return this.AgentData.AgentIsFemale;
			}
		}

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000AC5 RID: 2757 RVA: 0x00015548 File Offset: 0x00013748
		public int AgentRace
		{
			get
			{
				return this.AgentData.AgentRace;
			}
		}

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000AC6 RID: 2758 RVA: 0x00015555 File Offset: 0x00013755
		public IAgentOriginBase AgentOrigin
		{
			get
			{
				return this.AgentData.AgentOrigin;
			}
		}

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000AC7 RID: 2759 RVA: 0x00015562 File Offset: 0x00013762
		// (set) Token: 0x06000AC8 RID: 2760 RVA: 0x0001556A File Offset: 0x0001376A
		public Agent.ControllerType AgentController { get; private set; }

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000AC9 RID: 2761 RVA: 0x00015573 File Offset: 0x00013773
		// (set) Token: 0x06000ACA RID: 2762 RVA: 0x0001557B File Offset: 0x0001377B
		public Team AgentTeam { get; private set; }

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000ACB RID: 2763 RVA: 0x00015584 File Offset: 0x00013784
		// (set) Token: 0x06000ACC RID: 2764 RVA: 0x0001558C File Offset: 0x0001378C
		public bool AgentIsReinforcement { get; private set; }

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000ACD RID: 2765 RVA: 0x00015595 File Offset: 0x00013795
		// (set) Token: 0x06000ACE RID: 2766 RVA: 0x0001559D File Offset: 0x0001379D
		public bool AgentSpawnsIntoOwnFormation { get; private set; }

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000ACF RID: 2767 RVA: 0x000155A6 File Offset: 0x000137A6
		// (set) Token: 0x06000AD0 RID: 2768 RVA: 0x000155AE File Offset: 0x000137AE
		public bool AgentSpawnsUsingOwnTroopClass { get; private set; }

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06000AD1 RID: 2769 RVA: 0x000155B7 File Offset: 0x000137B7
		// (set) Token: 0x06000AD2 RID: 2770 RVA: 0x000155BF File Offset: 0x000137BF
		public float MakeUnitStandOutDistance { get; private set; }

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06000AD3 RID: 2771 RVA: 0x000155C8 File Offset: 0x000137C8
		// (set) Token: 0x06000AD4 RID: 2772 RVA: 0x000155D0 File Offset: 0x000137D0
		public Vec3? AgentInitialPosition { get; private set; }

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x000155D9 File Offset: 0x000137D9
		// (set) Token: 0x06000AD6 RID: 2774 RVA: 0x000155E1 File Offset: 0x000137E1
		public Vec2? AgentInitialDirection { get; private set; }

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000AD7 RID: 2775 RVA: 0x000155EA File Offset: 0x000137EA
		// (set) Token: 0x06000AD8 RID: 2776 RVA: 0x000155F2 File Offset: 0x000137F2
		public Formation AgentFormation { get; private set; }

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000AD9 RID: 2777 RVA: 0x000155FB File Offset: 0x000137FB
		// (set) Token: 0x06000ADA RID: 2778 RVA: 0x00015603 File Offset: 0x00013803
		public int AgentFormationTroopSpawnCount { get; private set; }

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06000ADB RID: 2779 RVA: 0x0001560C File Offset: 0x0001380C
		// (set) Token: 0x06000ADC RID: 2780 RVA: 0x00015614 File Offset: 0x00013814
		public int AgentFormationTroopSpawnIndex { get; private set; }

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000ADD RID: 2781 RVA: 0x0001561D File Offset: 0x0001381D
		// (set) Token: 0x06000ADE RID: 2782 RVA: 0x00015625 File Offset: 0x00013825
		public MissionPeer AgentMissionPeer { get; private set; }

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000ADF RID: 2783 RVA: 0x0001562E File Offset: 0x0001382E
		// (set) Token: 0x06000AE0 RID: 2784 RVA: 0x00015636 File Offset: 0x00013836
		public MissionPeer OwningAgentMissionPeer { get; private set; }

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x0001563F File Offset: 0x0001383F
		// (set) Token: 0x06000AE2 RID: 2786 RVA: 0x00015647 File Offset: 0x00013847
		public bool AgentIndexOverriden { get; private set; }

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000AE3 RID: 2787 RVA: 0x00015650 File Offset: 0x00013850
		// (set) Token: 0x06000AE4 RID: 2788 RVA: 0x00015658 File Offset: 0x00013858
		public int AgentIndex { get; private set; }

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x00015661 File Offset: 0x00013861
		// (set) Token: 0x06000AE6 RID: 2790 RVA: 0x00015669 File Offset: 0x00013869
		public bool AgentMountIndexOverriden { get; private set; }

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x00015672 File Offset: 0x00013872
		// (set) Token: 0x06000AE8 RID: 2792 RVA: 0x0001567A File Offset: 0x0001387A
		public int AgentMountIndex { get; private set; }

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x00015683 File Offset: 0x00013883
		// (set) Token: 0x06000AEA RID: 2794 RVA: 0x0001568B File Offset: 0x0001388B
		public int AgentVisualsIndex { get; private set; }

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000AEB RID: 2795 RVA: 0x00015694 File Offset: 0x00013894
		// (set) Token: 0x06000AEC RID: 2796 RVA: 0x0001569C File Offset: 0x0001389C
		public Banner AgentBanner { get; private set; }

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06000AED RID: 2797 RVA: 0x000156A5 File Offset: 0x000138A5
		// (set) Token: 0x06000AEE RID: 2798 RVA: 0x000156AD File Offset: 0x000138AD
		public ItemObject AgentBannerItem { get; private set; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06000AEF RID: 2799 RVA: 0x000156B6 File Offset: 0x000138B6
		// (set) Token: 0x06000AF0 RID: 2800 RVA: 0x000156BE File Offset: 0x000138BE
		public ItemObject AgentBannerReplacementWeaponItem { get; private set; }

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06000AF1 RID: 2801 RVA: 0x000156C7 File Offset: 0x000138C7
		// (set) Token: 0x06000AF2 RID: 2802 RVA: 0x000156CF File Offset: 0x000138CF
		public bool AgentCanSpawnOutsideOfMissionBoundary { get; private set; }

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06000AF3 RID: 2803 RVA: 0x000156D8 File Offset: 0x000138D8
		public bool RandomizeColors
		{
			get
			{
				return this.AgentCharacter != null && !this.AgentCharacter.IsHero && this.AgentMissionPeer == null;
			}
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x000156FA File Offset: 0x000138FA
		private AgentBuildData()
		{
			this.AgentController = Agent.ControllerType.AI;
			this.AgentTeam = TaleWorlds.MountAndBlade.Team.Invalid;
			this.AgentFormation = null;
			this.AgentMissionPeer = null;
			this.AgentFormationTroopSpawnIndex = -1;
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00015729 File Offset: 0x00013929
		public AgentBuildData(AgentData agentData)
			: this()
		{
			this.AgentData = agentData;
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00015738 File Offset: 0x00013938
		public AgentBuildData(IAgentOriginBase agentOrigin)
			: this()
		{
			this.AgentData = new AgentData(agentOrigin);
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x0001574C File Offset: 0x0001394C
		public AgentBuildData(BasicCharacterObject characterObject)
			: this()
		{
			this.AgentData = new AgentData(characterObject);
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x00015760 File Offset: 0x00013960
		public AgentBuildData Character(BasicCharacterObject characterObject)
		{
			this.AgentData.Character(characterObject);
			return this;
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x00015770 File Offset: 0x00013970
		public AgentBuildData Controller(Agent.ControllerType controller)
		{
			this.AgentController = controller;
			return this;
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x0001577A File Offset: 0x0001397A
		public AgentBuildData Team(Team team)
		{
			this.AgentTeam = team;
			return this;
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x00015784 File Offset: 0x00013984
		public AgentBuildData IsReinforcement(bool isReinforcement)
		{
			this.AgentIsReinforcement = isReinforcement;
			return this;
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x0001578E File Offset: 0x0001398E
		public AgentBuildData SpawnsIntoOwnFormation(bool spawnIntoOwnFormation)
		{
			this.AgentSpawnsIntoOwnFormation = spawnIntoOwnFormation;
			return this;
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x00015798 File Offset: 0x00013998
		public AgentBuildData SpawnsUsingOwnTroopClass(bool spawnUsingOwnTroopClass)
		{
			this.AgentSpawnsUsingOwnTroopClass = spawnUsingOwnTroopClass;
			return this;
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x000157A2 File Offset: 0x000139A2
		public AgentBuildData MakeUnitStandOutOfFormationDistance(float makeUnitStandOutDistance)
		{
			this.MakeUnitStandOutDistance = makeUnitStandOutDistance;
			return this;
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x000157AC File Offset: 0x000139AC
		public AgentBuildData InitialPosition(in Vec3 position)
		{
			this.AgentInitialPosition = new Vec3?(position);
			return this;
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x000157C0 File Offset: 0x000139C0
		public AgentBuildData InitialDirection(in Vec2 direction)
		{
			this.AgentInitialDirection = new Vec2?(direction);
			return this;
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x000157D4 File Offset: 0x000139D4
		public AgentBuildData InitialFrameFromSpawnPointEntity(GameEntity entity)
		{
			MatrixFrame globalFrame = entity.GetGlobalFrame();
			this.AgentInitialPosition = new Vec3?(globalFrame.origin);
			this.AgentInitialDirection = new Vec2?(globalFrame.rotation.f.AsVec2.Normalized());
			return this;
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x0001581E File Offset: 0x00013A1E
		public AgentBuildData Formation(Formation formation)
		{
			this.AgentFormation = formation;
			return this;
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00015828 File Offset: 0x00013A28
		public AgentBuildData Monster(Monster monster)
		{
			this.AgentData.Monster(monster);
			return this;
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x00015838 File Offset: 0x00013A38
		public AgentBuildData VisualsIndex(int index)
		{
			this.AgentVisualsIndex = index;
			return this;
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00015842 File Offset: 0x00013A42
		public AgentBuildData Equipment(Equipment equipment)
		{
			this.AgentData.Equipment(equipment);
			return this;
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00015852 File Offset: 0x00013A52
		public AgentBuildData MissionEquipment(MissionEquipment missionEquipment)
		{
			this.AgentOverridenSpawnMissionEquipment = missionEquipment;
			return this;
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x0001585C File Offset: 0x00013A5C
		public AgentBuildData EquipmentSeed(int seed)
		{
			this.AgentData.EquipmentSeed(seed);
			return this;
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x0001586C File Offset: 0x00013A6C
		public AgentBuildData NoHorses(bool noHorses)
		{
			this.AgentData.NoHorses(noHorses);
			return this;
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x0001587C File Offset: 0x00013A7C
		public AgentBuildData NoWeapons(bool noWeapons)
		{
			this.AgentData.NoWeapons(noWeapons);
			return this;
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x0001588C File Offset: 0x00013A8C
		public AgentBuildData NoArmor(bool noArmor)
		{
			this.AgentData.NoArmor(noArmor);
			return this;
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x0001589C File Offset: 0x00013A9C
		public AgentBuildData FixedEquipment(bool fixedEquipment)
		{
			this.AgentData.FixedEquipment(fixedEquipment);
			return this;
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x000158AC File Offset: 0x00013AAC
		public AgentBuildData CivilianEquipment(bool civilianEquipment)
		{
			this.AgentData.CivilianEquipment(civilianEquipment);
			return this;
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x000158BC File Offset: 0x00013ABC
		public AgentBuildData ClothingColor1(uint color)
		{
			this.AgentData.ClothingColor1(color);
			return this;
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x000158CC File Offset: 0x00013ACC
		public AgentBuildData ClothingColor2(uint color)
		{
			this.AgentData.ClothingColor2(color);
			return this;
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x000158DC File Offset: 0x00013ADC
		public AgentBuildData MissionPeer(MissionPeer missionPeer)
		{
			this.AgentMissionPeer = missionPeer;
			return this;
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x000158E6 File Offset: 0x00013AE6
		public AgentBuildData OwningMissionPeer(MissionPeer missionPeer)
		{
			this.OwningAgentMissionPeer = missionPeer;
			return this;
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x000158F0 File Offset: 0x00013AF0
		public AgentBuildData BodyProperties(BodyProperties bodyProperties)
		{
			this.AgentData.BodyProperties(bodyProperties);
			return this;
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x00015900 File Offset: 0x00013B00
		public AgentBuildData Age(int age)
		{
			this.AgentData.Age(age);
			return this;
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x00015910 File Offset: 0x00013B10
		public AgentBuildData TroopOrigin(IAgentOriginBase troopOrigin)
		{
			this.AgentData.TroopOrigin(troopOrigin);
			return this;
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x00015920 File Offset: 0x00013B20
		public AgentBuildData IsFemale(bool isFemale)
		{
			this.AgentData.IsFemale(isFemale);
			return this;
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x00015930 File Offset: 0x00013B30
		public AgentBuildData Race(int race)
		{
			this.AgentData.Race(race);
			return this;
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x00015940 File Offset: 0x00013B40
		public AgentBuildData MountKey(string mountKey)
		{
			this.AgentData.MountKey(mountKey);
			return this;
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x00015950 File Offset: 0x00013B50
		public AgentBuildData Index(int index)
		{
			this.AgentIndex = index;
			this.AgentIndexOverriden = true;
			return this;
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x00015961 File Offset: 0x00013B61
		public AgentBuildData MountIndex(int mountIndex)
		{
			this.AgentMountIndex = mountIndex;
			this.AgentMountIndexOverriden = true;
			return this;
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x00015972 File Offset: 0x00013B72
		public AgentBuildData Banner(Banner banner)
		{
			this.AgentBanner = banner;
			return this;
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0001597C File Offset: 0x00013B7C
		public AgentBuildData BannerItem(ItemObject bannerItem)
		{
			this.AgentBannerItem = bannerItem;
			return this;
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x00015986 File Offset: 0x00013B86
		public AgentBuildData BannerReplacementWeaponItem(ItemObject weaponItem)
		{
			this.AgentBannerReplacementWeaponItem = weaponItem;
			return this;
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x00015990 File Offset: 0x00013B90
		public AgentBuildData FormationTroopSpawnCount(int formationTroopCount)
		{
			this.AgentFormationTroopSpawnCount = formationTroopCount;
			return this;
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x0001599A File Offset: 0x00013B9A
		public AgentBuildData FormationTroopSpawnIndex(int formationTroopIndex)
		{
			this.AgentFormationTroopSpawnIndex = formationTroopIndex;
			return this;
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x000159A4 File Offset: 0x00013BA4
		public AgentBuildData CanSpawnOutsideOfMissionBoundary(bool canSpawn)
		{
			this.AgentCanSpawnOutsideOfMissionBoundary = canSpawn;
			return this;
		}
	}
}
