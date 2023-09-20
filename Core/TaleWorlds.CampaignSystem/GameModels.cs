using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200007F RID: 127
	public sealed class GameModels : GameModelsManager
	{
		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06000F66 RID: 3942 RVA: 0x00048322 File Offset: 0x00046522
		// (set) Token: 0x06000F67 RID: 3943 RVA: 0x0004832A File Offset: 0x0004652A
		public MapVisibilityModel MapVisibilityModel { get; private set; }

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06000F68 RID: 3944 RVA: 0x00048333 File Offset: 0x00046533
		// (set) Token: 0x06000F69 RID: 3945 RVA: 0x0004833B File Offset: 0x0004653B
		public InformationRestrictionModel InformationRestrictionModel { get; private set; }

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06000F6A RID: 3946 RVA: 0x00048344 File Offset: 0x00046544
		// (set) Token: 0x06000F6B RID: 3947 RVA: 0x0004834C File Offset: 0x0004654C
		public PartySpeedModel PartySpeedCalculatingModel { get; private set; }

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06000F6C RID: 3948 RVA: 0x00048355 File Offset: 0x00046555
		// (set) Token: 0x06000F6D RID: 3949 RVA: 0x0004835D File Offset: 0x0004655D
		public PartyHealingModel PartyHealingModel { get; private set; }

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06000F6E RID: 3950 RVA: 0x00048366 File Offset: 0x00046566
		// (set) Token: 0x06000F6F RID: 3951 RVA: 0x0004836E File Offset: 0x0004656E
		public PartyTrainingModel PartyTrainingModel { get; private set; }

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06000F70 RID: 3952 RVA: 0x00048377 File Offset: 0x00046577
		// (set) Token: 0x06000F71 RID: 3953 RVA: 0x0004837F File Offset: 0x0004657F
		public BarterModel BarterModel { get; private set; }

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06000F72 RID: 3954 RVA: 0x00048388 File Offset: 0x00046588
		// (set) Token: 0x06000F73 RID: 3955 RVA: 0x00048390 File Offset: 0x00046590
		public PersuasionModel PersuasionModel { get; private set; }

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06000F74 RID: 3956 RVA: 0x00048399 File Offset: 0x00046599
		// (set) Token: 0x06000F75 RID: 3957 RVA: 0x000483A1 File Offset: 0x000465A1
		public CombatSimulationModel CombatSimulationModel { get; private set; }

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06000F76 RID: 3958 RVA: 0x000483AA File Offset: 0x000465AA
		// (set) Token: 0x06000F77 RID: 3959 RVA: 0x000483B2 File Offset: 0x000465B2
		public CombatXpModel CombatXpModel { get; private set; }

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06000F78 RID: 3960 RVA: 0x000483BB File Offset: 0x000465BB
		// (set) Token: 0x06000F79 RID: 3961 RVA: 0x000483C3 File Offset: 0x000465C3
		public GenericXpModel GenericXpModel { get; private set; }

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06000F7A RID: 3962 RVA: 0x000483CC File Offset: 0x000465CC
		// (set) Token: 0x06000F7B RID: 3963 RVA: 0x000483D4 File Offset: 0x000465D4
		public SmithingModel SmithingModel { get; private set; }

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06000F7C RID: 3964 RVA: 0x000483DD File Offset: 0x000465DD
		// (set) Token: 0x06000F7D RID: 3965 RVA: 0x000483E5 File Offset: 0x000465E5
		public PartyTradeModel PartyTradeModel { get; private set; }

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06000F7E RID: 3966 RVA: 0x000483EE File Offset: 0x000465EE
		// (set) Token: 0x06000F7F RID: 3967 RVA: 0x000483F6 File Offset: 0x000465F6
		public RansomValueCalculationModel RansomValueCalculationModel { get; private set; }

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06000F80 RID: 3968 RVA: 0x000483FF File Offset: 0x000465FF
		// (set) Token: 0x06000F81 RID: 3969 RVA: 0x00048407 File Offset: 0x00046607
		public RaidModel RaidModel { get; private set; }

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06000F82 RID: 3970 RVA: 0x00048410 File Offset: 0x00046610
		// (set) Token: 0x06000F83 RID: 3971 RVA: 0x00048418 File Offset: 0x00046618
		public MobilePartyFoodConsumptionModel MobilePartyFoodConsumptionModel { get; private set; }

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x00048421 File Offset: 0x00046621
		// (set) Token: 0x06000F85 RID: 3973 RVA: 0x00048429 File Offset: 0x00046629
		public PartyFoodBuyingModel PartyFoodBuyingModel { get; private set; }

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x00048432 File Offset: 0x00046632
		// (set) Token: 0x06000F87 RID: 3975 RVA: 0x0004843A File Offset: 0x0004663A
		public PartyImpairmentModel PartyImpairmentModel { get; private set; }

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06000F88 RID: 3976 RVA: 0x00048443 File Offset: 0x00046643
		// (set) Token: 0x06000F89 RID: 3977 RVA: 0x0004844B File Offset: 0x0004664B
		public PartyMoraleModel PartyMoraleModel { get; private set; }

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06000F8A RID: 3978 RVA: 0x00048454 File Offset: 0x00046654
		// (set) Token: 0x06000F8B RID: 3979 RVA: 0x0004845C File Offset: 0x0004665C
		public PartyDesertionModel PartyDesertionModel { get; private set; }

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06000F8C RID: 3980 RVA: 0x00048465 File Offset: 0x00046665
		// (set) Token: 0x06000F8D RID: 3981 RVA: 0x0004846D File Offset: 0x0004666D
		public DiplomacyModel DiplomacyModel { get; private set; }

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06000F8E RID: 3982 RVA: 0x00048476 File Offset: 0x00046676
		// (set) Token: 0x06000F8F RID: 3983 RVA: 0x0004847E File Offset: 0x0004667E
		public MinorFactionsModel MinorFactionsModel { get; private set; }

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06000F90 RID: 3984 RVA: 0x00048487 File Offset: 0x00046687
		// (set) Token: 0x06000F91 RID: 3985 RVA: 0x0004848F File Offset: 0x0004668F
		public KingdomCreationModel KingdomCreationModel { get; private set; }

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06000F92 RID: 3986 RVA: 0x00048498 File Offset: 0x00046698
		// (set) Token: 0x06000F93 RID: 3987 RVA: 0x000484A0 File Offset: 0x000466A0
		public KingdomDecisionPermissionModel KingdomDecisionPermissionModel { get; private set; }

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06000F94 RID: 3988 RVA: 0x000484A9 File Offset: 0x000466A9
		// (set) Token: 0x06000F95 RID: 3989 RVA: 0x000484B1 File Offset: 0x000466B1
		public EmissaryModel EmissaryModel { get; private set; }

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06000F96 RID: 3990 RVA: 0x000484BA File Offset: 0x000466BA
		// (set) Token: 0x06000F97 RID: 3991 RVA: 0x000484C2 File Offset: 0x000466C2
		public CharacterDevelopmentModel CharacterDevelopmentModel { get; private set; }

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06000F98 RID: 3992 RVA: 0x000484CB File Offset: 0x000466CB
		// (set) Token: 0x06000F99 RID: 3993 RVA: 0x000484D3 File Offset: 0x000466D3
		public CharacterStatsModel CharacterStatsModel { get; private set; }

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06000F9A RID: 3994 RVA: 0x000484DC File Offset: 0x000466DC
		// (set) Token: 0x06000F9B RID: 3995 RVA: 0x000484E4 File Offset: 0x000466E4
		public EncounterModel EncounterModel { get; private set; }

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06000F9C RID: 3996 RVA: 0x000484ED File Offset: 0x000466ED
		// (set) Token: 0x06000F9D RID: 3997 RVA: 0x000484F5 File Offset: 0x000466F5
		public ItemDiscardModel ItemDiscardModel { get; private set; }

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06000F9E RID: 3998 RVA: 0x000484FE File Offset: 0x000466FE
		// (set) Token: 0x06000F9F RID: 3999 RVA: 0x00048506 File Offset: 0x00046706
		public ValuationModel ValuationModel { get; private set; }

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x0004850F File Offset: 0x0004670F
		// (set) Token: 0x06000FA1 RID: 4001 RVA: 0x00048517 File Offset: 0x00046717
		public PartySizeLimitModel PartySizeLimitModel { get; private set; }

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06000FA2 RID: 4002 RVA: 0x00048520 File Offset: 0x00046720
		// (set) Token: 0x06000FA3 RID: 4003 RVA: 0x00048528 File Offset: 0x00046728
		public InventoryCapacityModel InventoryCapacityModel { get; private set; }

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06000FA4 RID: 4004 RVA: 0x00048531 File Offset: 0x00046731
		// (set) Token: 0x06000FA5 RID: 4005 RVA: 0x00048539 File Offset: 0x00046739
		public PartyWageModel PartyWageModel { get; private set; }

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06000FA6 RID: 4006 RVA: 0x00048542 File Offset: 0x00046742
		// (set) Token: 0x06000FA7 RID: 4007 RVA: 0x0004854A File Offset: 0x0004674A
		public VillageProductionCalculatorModel VillageProductionCalculatorModel { get; private set; }

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x00048553 File Offset: 0x00046753
		// (set) Token: 0x06000FA9 RID: 4009 RVA: 0x0004855B File Offset: 0x0004675B
		public VolunteerModel VolunteerModel { get; private set; }

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06000FAA RID: 4010 RVA: 0x00048564 File Offset: 0x00046764
		// (set) Token: 0x06000FAB RID: 4011 RVA: 0x0004856C File Offset: 0x0004676C
		public RomanceModel RomanceModel { get; private set; }

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06000FAC RID: 4012 RVA: 0x00048575 File Offset: 0x00046775
		// (set) Token: 0x06000FAD RID: 4013 RVA: 0x0004857D File Offset: 0x0004677D
		public ArmyManagementCalculationModel ArmyManagementCalculationModel { get; private set; }

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06000FAE RID: 4014 RVA: 0x00048586 File Offset: 0x00046786
		// (set) Token: 0x06000FAF RID: 4015 RVA: 0x0004858E File Offset: 0x0004678E
		public BanditDensityModel BanditDensityModel { get; private set; }

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x00048597 File Offset: 0x00046797
		// (set) Token: 0x06000FB1 RID: 4017 RVA: 0x0004859F File Offset: 0x0004679F
		public EncounterGameMenuModel EncounterGameMenuModel { get; private set; }

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06000FB2 RID: 4018 RVA: 0x000485A8 File Offset: 0x000467A8
		// (set) Token: 0x06000FB3 RID: 4019 RVA: 0x000485B0 File Offset: 0x000467B0
		public BattleRewardModel BattleRewardModel { get; private set; }

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06000FB4 RID: 4020 RVA: 0x000485B9 File Offset: 0x000467B9
		// (set) Token: 0x06000FB5 RID: 4021 RVA: 0x000485C1 File Offset: 0x000467C1
		public MapTrackModel MapTrackModel { get; private set; }

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06000FB6 RID: 4022 RVA: 0x000485CA File Offset: 0x000467CA
		// (set) Token: 0x06000FB7 RID: 4023 RVA: 0x000485D2 File Offset: 0x000467D2
		public MapDistanceModel MapDistanceModel { get; private set; }

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06000FB8 RID: 4024 RVA: 0x000485DB File Offset: 0x000467DB
		// (set) Token: 0x06000FB9 RID: 4025 RVA: 0x000485E3 File Offset: 0x000467E3
		public MapWeatherModel MapWeatherModel { get; private set; }

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06000FBA RID: 4026 RVA: 0x000485EC File Offset: 0x000467EC
		// (set) Token: 0x06000FBB RID: 4027 RVA: 0x000485F4 File Offset: 0x000467F4
		public TargetScoreCalculatingModel TargetScoreCalculatingModel { get; private set; }

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06000FBC RID: 4028 RVA: 0x000485FD File Offset: 0x000467FD
		// (set) Token: 0x06000FBD RID: 4029 RVA: 0x00048605 File Offset: 0x00046805
		public TradeItemPriceFactorModel TradeItemPriceFactorModel { get; private set; }

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06000FBE RID: 4030 RVA: 0x0004860E File Offset: 0x0004680E
		// (set) Token: 0x06000FBF RID: 4031 RVA: 0x00048616 File Offset: 0x00046816
		public SettlementEconomyModel SettlementConsumptionModel { get; private set; }

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x0004861F File Offset: 0x0004681F
		// (set) Token: 0x06000FC1 RID: 4033 RVA: 0x00048627 File Offset: 0x00046827
		public SettlementFoodModel SettlementFoodModel { get; private set; }

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06000FC2 RID: 4034 RVA: 0x00048630 File Offset: 0x00046830
		// (set) Token: 0x06000FC3 RID: 4035 RVA: 0x00048638 File Offset: 0x00046838
		public SettlementValueModel SettlementValueModel { get; private set; }

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06000FC4 RID: 4036 RVA: 0x00048641 File Offset: 0x00046841
		// (set) Token: 0x06000FC5 RID: 4037 RVA: 0x00048649 File Offset: 0x00046849
		public SettlementMilitiaModel SettlementMilitiaModel { get; private set; }

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x00048652 File Offset: 0x00046852
		// (set) Token: 0x06000FC7 RID: 4039 RVA: 0x0004865A File Offset: 0x0004685A
		public SettlementLoyaltyModel SettlementLoyaltyModel { get; private set; }

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x00048663 File Offset: 0x00046863
		// (set) Token: 0x06000FC9 RID: 4041 RVA: 0x0004866B File Offset: 0x0004686B
		public SettlementSecurityModel SettlementSecurityModel { get; private set; }

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06000FCA RID: 4042 RVA: 0x00048674 File Offset: 0x00046874
		// (set) Token: 0x06000FCB RID: 4043 RVA: 0x0004867C File Offset: 0x0004687C
		public SettlementProsperityModel SettlementProsperityModel { get; private set; }

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06000FCC RID: 4044 RVA: 0x00048685 File Offset: 0x00046885
		// (set) Token: 0x06000FCD RID: 4045 RVA: 0x0004868D File Offset: 0x0004688D
		public SettlementGarrisonModel SettlementGarrisonModel { get; private set; }

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06000FCE RID: 4046 RVA: 0x00048696 File Offset: 0x00046896
		// (set) Token: 0x06000FCF RID: 4047 RVA: 0x0004869E File Offset: 0x0004689E
		public ClanTierModel ClanTierModel { get; private set; }

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06000FD0 RID: 4048 RVA: 0x000486A7 File Offset: 0x000468A7
		// (set) Token: 0x06000FD1 RID: 4049 RVA: 0x000486AF File Offset: 0x000468AF
		public VassalRewardsModel VassalRewardsModel { get; private set; }

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x000486B8 File Offset: 0x000468B8
		// (set) Token: 0x06000FD3 RID: 4051 RVA: 0x000486C0 File Offset: 0x000468C0
		public ClanPoliticsModel ClanPoliticsModel { get; private set; }

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x000486C9 File Offset: 0x000468C9
		// (set) Token: 0x06000FD5 RID: 4053 RVA: 0x000486D1 File Offset: 0x000468D1
		public ClanFinanceModel ClanFinanceModel { get; private set; }

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06000FD6 RID: 4054 RVA: 0x000486DA File Offset: 0x000468DA
		// (set) Token: 0x06000FD7 RID: 4055 RVA: 0x000486E2 File Offset: 0x000468E2
		public SettlementTaxModel SettlementTaxModel { get; private set; }

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x000486EB File Offset: 0x000468EB
		// (set) Token: 0x06000FD9 RID: 4057 RVA: 0x000486F3 File Offset: 0x000468F3
		public HeirSelectionCalculationModel HeirSelectionCalculationModel { get; private set; }

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x000486FC File Offset: 0x000468FC
		// (set) Token: 0x06000FDB RID: 4059 RVA: 0x00048704 File Offset: 0x00046904
		public HeroDeathProbabilityCalculationModel HeroDeathProbabilityCalculationModel { get; private set; }

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06000FDC RID: 4060 RVA: 0x0004870D File Offset: 0x0004690D
		// (set) Token: 0x06000FDD RID: 4061 RVA: 0x00048715 File Offset: 0x00046915
		public BuildingConstructionModel BuildingConstructionModel { get; private set; }

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06000FDE RID: 4062 RVA: 0x0004871E File Offset: 0x0004691E
		// (set) Token: 0x06000FDF RID: 4063 RVA: 0x00048726 File Offset: 0x00046926
		public BuildingEffectModel BuildingEffectModel { get; private set; }

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06000FE0 RID: 4064 RVA: 0x0004872F File Offset: 0x0004692F
		// (set) Token: 0x06000FE1 RID: 4065 RVA: 0x00048737 File Offset: 0x00046937
		public WallHitPointCalculationModel WallHitPointCalculationModel { get; private set; }

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06000FE2 RID: 4066 RVA: 0x00048740 File Offset: 0x00046940
		// (set) Token: 0x06000FE3 RID: 4067 RVA: 0x00048748 File Offset: 0x00046948
		public MarriageModel MarriageModel { get; private set; }

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06000FE4 RID: 4068 RVA: 0x00048751 File Offset: 0x00046951
		// (set) Token: 0x06000FE5 RID: 4069 RVA: 0x00048759 File Offset: 0x00046959
		public AgeModel AgeModel { get; private set; }

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x00048762 File Offset: 0x00046962
		// (set) Token: 0x06000FE7 RID: 4071 RVA: 0x0004876A File Offset: 0x0004696A
		public PlayerProgressionModel PlayerProgressionModel { get; private set; }

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06000FE8 RID: 4072 RVA: 0x00048773 File Offset: 0x00046973
		// (set) Token: 0x06000FE9 RID: 4073 RVA: 0x0004877B File Offset: 0x0004697B
		public DailyTroopXpBonusModel DailyTroopXpBonusModel { get; private set; }

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x00048784 File Offset: 0x00046984
		// (set) Token: 0x06000FEB RID: 4075 RVA: 0x0004878C File Offset: 0x0004698C
		public PregnancyModel PregnancyModel { get; private set; }

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06000FEC RID: 4076 RVA: 0x00048795 File Offset: 0x00046995
		// (set) Token: 0x06000FED RID: 4077 RVA: 0x0004879D File Offset: 0x0004699D
		public NotablePowerModel NotablePowerModel { get; private set; }

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06000FEE RID: 4078 RVA: 0x000487A6 File Offset: 0x000469A6
		// (set) Token: 0x06000FEF RID: 4079 RVA: 0x000487AE File Offset: 0x000469AE
		public MilitaryPowerModel MilitaryPowerModel { get; private set; }

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x000487B7 File Offset: 0x000469B7
		// (set) Token: 0x06000FF1 RID: 4081 RVA: 0x000487BF File Offset: 0x000469BF
		public PrisonerDonationModel PrisonerDonationModel { get; private set; }

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x000487C8 File Offset: 0x000469C8
		// (set) Token: 0x06000FF3 RID: 4083 RVA: 0x000487D0 File Offset: 0x000469D0
		public NotableSpawnModel NotableSpawnModel { get; private set; }

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x000487D9 File Offset: 0x000469D9
		// (set) Token: 0x06000FF5 RID: 4085 RVA: 0x000487E1 File Offset: 0x000469E1
		public TournamentModel TournamentModel { get; private set; }

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x000487EA File Offset: 0x000469EA
		// (set) Token: 0x06000FF7 RID: 4087 RVA: 0x000487F2 File Offset: 0x000469F2
		public CrimeModel CrimeModel { get; private set; }

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06000FF8 RID: 4088 RVA: 0x000487FB File Offset: 0x000469FB
		// (set) Token: 0x06000FF9 RID: 4089 RVA: 0x00048803 File Offset: 0x00046A03
		public DisguiseDetectionModel DisguiseDetectionModel { get; private set; }

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06000FFA RID: 4090 RVA: 0x0004880C File Offset: 0x00046A0C
		// (set) Token: 0x06000FFB RID: 4091 RVA: 0x00048814 File Offset: 0x00046A14
		public BribeCalculationModel BribeCalculationModel { get; private set; }

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06000FFC RID: 4092 RVA: 0x0004881D File Offset: 0x00046A1D
		// (set) Token: 0x06000FFD RID: 4093 RVA: 0x00048825 File Offset: 0x00046A25
		public TroopSacrificeModel TroopSacrificeModel { get; private set; }

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06000FFE RID: 4094 RVA: 0x0004882E File Offset: 0x00046A2E
		// (set) Token: 0x06000FFF RID: 4095 RVA: 0x00048836 File Offset: 0x00046A36
		public SiegeStrategyActionModel SiegeStrategyActionModel { get; private set; }

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06001000 RID: 4096 RVA: 0x0004883F File Offset: 0x00046A3F
		// (set) Token: 0x06001001 RID: 4097 RVA: 0x00048847 File Offset: 0x00046A47
		public SiegeEventModel SiegeEventModel { get; private set; }

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001002 RID: 4098 RVA: 0x00048850 File Offset: 0x00046A50
		// (set) Token: 0x06001003 RID: 4099 RVA: 0x00048858 File Offset: 0x00046A58
		public SiegeAftermathModel SiegeAftermathModel { get; private set; }

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001004 RID: 4100 RVA: 0x00048861 File Offset: 0x00046A61
		// (set) Token: 0x06001005 RID: 4101 RVA: 0x00048869 File Offset: 0x00046A69
		public SiegeLordsHallFightModel SiegeLordsHallFightModel { get; private set; }

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001006 RID: 4102 RVA: 0x00048872 File Offset: 0x00046A72
		// (set) Token: 0x06001007 RID: 4103 RVA: 0x0004887A File Offset: 0x00046A7A
		public CompanionHiringPriceCalculationModel CompanionHiringPriceCalculationModel { get; private set; }

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001008 RID: 4104 RVA: 0x00048883 File Offset: 0x00046A83
		// (set) Token: 0x06001009 RID: 4105 RVA: 0x0004888B File Offset: 0x00046A8B
		public BuildingScoreCalculationModel BuildingScoreCalculationModel { get; private set; }

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x0600100A RID: 4106 RVA: 0x00048894 File Offset: 0x00046A94
		// (set) Token: 0x0600100B RID: 4107 RVA: 0x0004889C File Offset: 0x00046A9C
		public SettlementAccessModel SettlementAccessModel { get; private set; }

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x0600100C RID: 4108 RVA: 0x000488A5 File Offset: 0x00046AA5
		// (set) Token: 0x0600100D RID: 4109 RVA: 0x000488AD File Offset: 0x00046AAD
		public IssueModel IssueModel { get; private set; }

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x0600100E RID: 4110 RVA: 0x000488B6 File Offset: 0x00046AB6
		// (set) Token: 0x0600100F RID: 4111 RVA: 0x000488BE File Offset: 0x00046ABE
		public PrisonerRecruitmentCalculationModel PrisonerRecruitmentCalculationModel { get; private set; }

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001010 RID: 4112 RVA: 0x000488C7 File Offset: 0x00046AC7
		// (set) Token: 0x06001011 RID: 4113 RVA: 0x000488CF File Offset: 0x00046ACF
		public PartyTroopUpgradeModel PartyTroopUpgradeModel { get; private set; }

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001012 RID: 4114 RVA: 0x000488D8 File Offset: 0x00046AD8
		// (set) Token: 0x06001013 RID: 4115 RVA: 0x000488E0 File Offset: 0x00046AE0
		public TavernMercenaryTroopsModel TavernMercenaryTroopsModel { get; private set; }

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06001014 RID: 4116 RVA: 0x000488E9 File Offset: 0x00046AE9
		// (set) Token: 0x06001015 RID: 4117 RVA: 0x000488F1 File Offset: 0x00046AF1
		public WorkshopModel WorkshopModel { get; private set; }

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06001016 RID: 4118 RVA: 0x000488FA File Offset: 0x00046AFA
		// (set) Token: 0x06001017 RID: 4119 RVA: 0x00048902 File Offset: 0x00046B02
		public DifficultyModel DifficultyModel { get; private set; }

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06001018 RID: 4120 RVA: 0x0004890B File Offset: 0x00046B0B
		// (set) Token: 0x06001019 RID: 4121 RVA: 0x00048913 File Offset: 0x00046B13
		public LocationModel LocationModel { get; private set; }

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x0600101A RID: 4122 RVA: 0x0004891C File Offset: 0x00046B1C
		// (set) Token: 0x0600101B RID: 4123 RVA: 0x00048924 File Offset: 0x00046B24
		public PrisonBreakModel PrisonBreakModel { get; private set; }

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x0600101C RID: 4124 RVA: 0x0004892D File Offset: 0x00046B2D
		// (set) Token: 0x0600101D RID: 4125 RVA: 0x00048935 File Offset: 0x00046B35
		public BattleCaptainModel BattleCaptainModel { get; private set; }

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x0600101E RID: 4126 RVA: 0x0004893E File Offset: 0x00046B3E
		// (set) Token: 0x0600101F RID: 4127 RVA: 0x00048946 File Offset: 0x00046B46
		public ExecutionRelationModel ExecutionRelationModel { get; private set; }

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001020 RID: 4128 RVA: 0x0004894F File Offset: 0x00046B4F
		// (set) Token: 0x06001021 RID: 4129 RVA: 0x00048957 File Offset: 0x00046B57
		public BannerItemModel BannerItemModel { get; private set; }

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001022 RID: 4130 RVA: 0x00048960 File Offset: 0x00046B60
		// (set) Token: 0x06001023 RID: 4131 RVA: 0x00048968 File Offset: 0x00046B68
		public DelayedTeleportationModel DelayedTeleportationModel { get; private set; }

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001024 RID: 4132 RVA: 0x00048971 File Offset: 0x00046B71
		// (set) Token: 0x06001025 RID: 4133 RVA: 0x00048979 File Offset: 0x00046B79
		public TroopSupplierProbabilityModel TroopSupplierProbabilityModel { get; private set; }

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06001026 RID: 4134 RVA: 0x00048982 File Offset: 0x00046B82
		// (set) Token: 0x06001027 RID: 4135 RVA: 0x0004898A File Offset: 0x00046B8A
		public CutsceneSelectionModel CutsceneSelectionModel { get; private set; }

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06001028 RID: 4136 RVA: 0x00048993 File Offset: 0x00046B93
		// (set) Token: 0x06001029 RID: 4137 RVA: 0x0004899B File Offset: 0x00046B9B
		public EquipmentSelectionModel EquipmentSelectionModel { get; private set; }

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x0600102A RID: 4138 RVA: 0x000489A4 File Offset: 0x00046BA4
		// (set) Token: 0x0600102B RID: 4139 RVA: 0x000489AC File Offset: 0x00046BAC
		public AlleyModel AlleyModel { get; private set; }

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x0600102C RID: 4140 RVA: 0x000489B5 File Offset: 0x00046BB5
		// (set) Token: 0x0600102D RID: 4141 RVA: 0x000489BD File Offset: 0x00046BBD
		public VoiceOverModel VoiceOverModel { get; private set; }

		// Token: 0x0600102E RID: 4142 RVA: 0x000489C8 File Offset: 0x00046BC8
		private void GetSpecificGameBehaviors()
		{
			if (Campaign.Current.GameMode == CampaignGameMode.Campaign || Campaign.Current.GameMode == CampaignGameMode.Tutorial)
			{
				this.CharacterDevelopmentModel = base.GetGameModel<CharacterDevelopmentModel>();
				this.CharacterStatsModel = base.GetGameModel<CharacterStatsModel>();
				this.EncounterModel = base.GetGameModel<EncounterModel>();
				this.ItemDiscardModel = base.GetGameModel<ItemDiscardModel>();
				this.ValuationModel = base.GetGameModel<ValuationModel>();
				this.MapVisibilityModel = base.GetGameModel<MapVisibilityModel>();
				this.InformationRestrictionModel = base.GetGameModel<InformationRestrictionModel>();
				this.PartySpeedCalculatingModel = base.GetGameModel<PartySpeedModel>();
				this.PartyHealingModel = base.GetGameModel<PartyHealingModel>();
				this.PartyTrainingModel = base.GetGameModel<PartyTrainingModel>();
				this.PartyTradeModel = base.GetGameModel<PartyTradeModel>();
				this.RansomValueCalculationModel = base.GetGameModel<RansomValueCalculationModel>();
				this.RaidModel = base.GetGameModel<RaidModel>();
				this.CombatSimulationModel = base.GetGameModel<CombatSimulationModel>();
				this.CombatXpModel = base.GetGameModel<CombatXpModel>();
				this.GenericXpModel = base.GetGameModel<GenericXpModel>();
				this.SmithingModel = base.GetGameModel<SmithingModel>();
				this.MobilePartyFoodConsumptionModel = base.GetGameModel<MobilePartyFoodConsumptionModel>();
				this.PartyImpairmentModel = base.GetGameModel<PartyImpairmentModel>();
				this.PartyFoodBuyingModel = base.GetGameModel<PartyFoodBuyingModel>();
				this.PartyMoraleModel = base.GetGameModel<PartyMoraleModel>();
				this.PartyDesertionModel = base.GetGameModel<PartyDesertionModel>();
				this.DiplomacyModel = base.GetGameModel<DiplomacyModel>();
				this.MinorFactionsModel = base.GetGameModel<MinorFactionsModel>();
				this.KingdomCreationModel = base.GetGameModel<KingdomCreationModel>();
				this.EmissaryModel = base.GetGameModel<EmissaryModel>();
				this.KingdomDecisionPermissionModel = base.GetGameModel<KingdomDecisionPermissionModel>();
				this.VillageProductionCalculatorModel = base.GetGameModel<VillageProductionCalculatorModel>();
				this.RomanceModel = base.GetGameModel<RomanceModel>();
				this.VolunteerModel = base.GetGameModel<VolunteerModel>();
				this.ArmyManagementCalculationModel = base.GetGameModel<ArmyManagementCalculationModel>();
				this.BanditDensityModel = base.GetGameModel<BanditDensityModel>();
				this.EncounterGameMenuModel = base.GetGameModel<EncounterGameMenuModel>();
				this.BattleRewardModel = base.GetGameModel<BattleRewardModel>();
				this.MapTrackModel = base.GetGameModel<MapTrackModel>();
				this.MapDistanceModel = base.GetGameModel<MapDistanceModel>();
				this.MapWeatherModel = base.GetGameModel<MapWeatherModel>();
				this.TargetScoreCalculatingModel = base.GetGameModel<TargetScoreCalculatingModel>();
				this.PartySizeLimitModel = base.GetGameModel<PartySizeLimitModel>();
				this.PartyWageModel = base.GetGameModel<PartyWageModel>();
				this.PlayerProgressionModel = base.GetGameModel<PlayerProgressionModel>();
				this.InventoryCapacityModel = base.GetGameModel<InventoryCapacityModel>();
				this.TradeItemPriceFactorModel = base.GetGameModel<TradeItemPriceFactorModel>();
				this.SettlementValueModel = base.GetGameModel<SettlementValueModel>();
				this.SettlementConsumptionModel = base.GetGameModel<SettlementEconomyModel>();
				this.SettlementMilitiaModel = base.GetGameModel<SettlementMilitiaModel>();
				this.SettlementFoodModel = base.GetGameModel<SettlementFoodModel>();
				this.SettlementLoyaltyModel = base.GetGameModel<SettlementLoyaltyModel>();
				this.SettlementSecurityModel = base.GetGameModel<SettlementSecurityModel>();
				this.SettlementProsperityModel = base.GetGameModel<SettlementProsperityModel>();
				this.SettlementGarrisonModel = base.GetGameModel<SettlementGarrisonModel>();
				this.SettlementTaxModel = base.GetGameModel<SettlementTaxModel>();
				this.BarterModel = base.GetGameModel<BarterModel>();
				this.PersuasionModel = base.GetGameModel<PersuasionModel>();
				this.ClanTierModel = base.GetGameModel<ClanTierModel>();
				this.VassalRewardsModel = base.GetGameModel<VassalRewardsModel>();
				this.ClanPoliticsModel = base.GetGameModel<ClanPoliticsModel>();
				this.ClanFinanceModel = base.GetGameModel<ClanFinanceModel>();
				this.HeirSelectionCalculationModel = base.GetGameModel<HeirSelectionCalculationModel>();
				this.HeroDeathProbabilityCalculationModel = base.GetGameModel<HeroDeathProbabilityCalculationModel>();
				this.BuildingConstructionModel = base.GetGameModel<BuildingConstructionModel>();
				this.BuildingEffectModel = base.GetGameModel<BuildingEffectModel>();
				this.WallHitPointCalculationModel = base.GetGameModel<WallHitPointCalculationModel>();
				this.MarriageModel = base.GetGameModel<MarriageModel>();
				this.AgeModel = base.GetGameModel<AgeModel>();
				this.DailyTroopXpBonusModel = base.GetGameModel<DailyTroopXpBonusModel>();
				this.PregnancyModel = base.GetGameModel<PregnancyModel>();
				this.NotablePowerModel = base.GetGameModel<NotablePowerModel>();
				this.NotableSpawnModel = base.GetGameModel<NotableSpawnModel>();
				this.TournamentModel = base.GetGameModel<TournamentModel>();
				this.SiegeStrategyActionModel = base.GetGameModel<SiegeStrategyActionModel>();
				this.SiegeEventModel = base.GetGameModel<SiegeEventModel>();
				this.SiegeAftermathModel = base.GetGameModel<SiegeAftermathModel>();
				this.SiegeLordsHallFightModel = base.GetGameModel<SiegeLordsHallFightModel>();
				this.CrimeModel = base.GetGameModel<CrimeModel>();
				this.DisguiseDetectionModel = base.GetGameModel<DisguiseDetectionModel>();
				this.BribeCalculationModel = base.GetGameModel<BribeCalculationModel>();
				this.CompanionHiringPriceCalculationModel = base.GetGameModel<CompanionHiringPriceCalculationModel>();
				this.TroopSacrificeModel = base.GetGameModel<TroopSacrificeModel>();
				this.BuildingScoreCalculationModel = base.GetGameModel<BuildingScoreCalculationModel>();
				this.SettlementAccessModel = base.GetGameModel<SettlementAccessModel>();
				this.IssueModel = base.GetGameModel<IssueModel>();
				this.PrisonerRecruitmentCalculationModel = base.GetGameModel<PrisonerRecruitmentCalculationModel>();
				this.PartyTroopUpgradeModel = base.GetGameModel<PartyTroopUpgradeModel>();
				this.TavernMercenaryTroopsModel = base.GetGameModel<TavernMercenaryTroopsModel>();
				this.WorkshopModel = base.GetGameModel<WorkshopModel>();
				this.DifficultyModel = base.GetGameModel<DifficultyModel>();
				this.LocationModel = base.GetGameModel<LocationModel>();
				this.MilitaryPowerModel = base.GetGameModel<MilitaryPowerModel>();
				this.PrisonerDonationModel = base.GetGameModel<PrisonerDonationModel>();
				this.PrisonBreakModel = base.GetGameModel<PrisonBreakModel>();
				this.BattleCaptainModel = base.GetGameModel<BattleCaptainModel>();
				this.ExecutionRelationModel = base.GetGameModel<ExecutionRelationModel>();
				this.BannerItemModel = base.GetGameModel<BannerItemModel>();
				this.DelayedTeleportationModel = base.GetGameModel<DelayedTeleportationModel>();
				this.TroopSupplierProbabilityModel = base.GetGameModel<TroopSupplierProbabilityModel>();
				this.CutsceneSelectionModel = base.GetGameModel<CutsceneSelectionModel>();
				this.EquipmentSelectionModel = base.GetGameModel<EquipmentSelectionModel>();
				this.AlleyModel = base.GetGameModel<AlleyModel>();
				this.VoiceOverModel = base.GetGameModel<VoiceOverModel>();
			}
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x00048EA2 File Offset: 0x000470A2
		private void MakeGameComponentBindings()
		{
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x00048EA4 File Offset: 0x000470A4
		public GameModels(IEnumerable<GameModel> inputComponents)
			: base(inputComponents)
		{
			this.GetSpecificGameBehaviors();
			this.MakeGameComponentBindings();
		}
	}
}
