using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem
{
	public sealed class GameModels : GameModelsManager
	{
		public MapVisibilityModel MapVisibilityModel { get; private set; }

		public InformationRestrictionModel InformationRestrictionModel { get; private set; }

		public PartySpeedModel PartySpeedCalculatingModel { get; private set; }

		public PartyHealingModel PartyHealingModel { get; private set; }

		public PartyTrainingModel PartyTrainingModel { get; private set; }

		public BarterModel BarterModel { get; private set; }

		public PersuasionModel PersuasionModel { get; private set; }

		public CombatSimulationModel CombatSimulationModel { get; private set; }

		public CombatXpModel CombatXpModel { get; private set; }

		public GenericXpModel GenericXpModel { get; private set; }

		public SmithingModel SmithingModel { get; private set; }

		public PartyTradeModel PartyTradeModel { get; private set; }

		public RansomValueCalculationModel RansomValueCalculationModel { get; private set; }

		public RaidModel RaidModel { get; private set; }

		public MobilePartyFoodConsumptionModel MobilePartyFoodConsumptionModel { get; private set; }

		public PartyFoodBuyingModel PartyFoodBuyingModel { get; private set; }

		public PartyImpairmentModel PartyImpairmentModel { get; private set; }

		public PartyMoraleModel PartyMoraleModel { get; private set; }

		public PartyDesertionModel PartyDesertionModel { get; private set; }

		public DiplomacyModel DiplomacyModel { get; private set; }

		public MinorFactionsModel MinorFactionsModel { get; private set; }

		public KingdomCreationModel KingdomCreationModel { get; private set; }

		public KingdomDecisionPermissionModel KingdomDecisionPermissionModel { get; private set; }

		public EmissaryModel EmissaryModel { get; private set; }

		public CharacterDevelopmentModel CharacterDevelopmentModel { get; private set; }

		public CharacterStatsModel CharacterStatsModel { get; private set; }

		public EncounterModel EncounterModel { get; private set; }

		public ItemDiscardModel ItemDiscardModel { get; private set; }

		public ValuationModel ValuationModel { get; private set; }

		public PartySizeLimitModel PartySizeLimitModel { get; private set; }

		public InventoryCapacityModel InventoryCapacityModel { get; private set; }

		public PartyWageModel PartyWageModel { get; private set; }

		public VillageProductionCalculatorModel VillageProductionCalculatorModel { get; private set; }

		public VolunteerModel VolunteerModel { get; private set; }

		public RomanceModel RomanceModel { get; private set; }

		public ArmyManagementCalculationModel ArmyManagementCalculationModel { get; private set; }

		public BanditDensityModel BanditDensityModel { get; private set; }

		public EncounterGameMenuModel EncounterGameMenuModel { get; private set; }

		public BattleRewardModel BattleRewardModel { get; private set; }

		public MapTrackModel MapTrackModel { get; private set; }

		public MapDistanceModel MapDistanceModel { get; private set; }

		public MapWeatherModel MapWeatherModel { get; private set; }

		public TargetScoreCalculatingModel TargetScoreCalculatingModel { get; private set; }

		public TradeItemPriceFactorModel TradeItemPriceFactorModel { get; private set; }

		public SettlementEconomyModel SettlementConsumptionModel { get; private set; }

		public SettlementFoodModel SettlementFoodModel { get; private set; }

		public SettlementValueModel SettlementValueModel { get; private set; }

		public SettlementMilitiaModel SettlementMilitiaModel { get; private set; }

		public SettlementLoyaltyModel SettlementLoyaltyModel { get; private set; }

		public SettlementSecurityModel SettlementSecurityModel { get; private set; }

		public SettlementProsperityModel SettlementProsperityModel { get; private set; }

		public SettlementGarrisonModel SettlementGarrisonModel { get; private set; }

		public ClanTierModel ClanTierModel { get; private set; }

		public VassalRewardsModel VassalRewardsModel { get; private set; }

		public ClanPoliticsModel ClanPoliticsModel { get; private set; }

		public ClanFinanceModel ClanFinanceModel { get; private set; }

		public SettlementTaxModel SettlementTaxModel { get; private set; }

		public HeirSelectionCalculationModel HeirSelectionCalculationModel { get; private set; }

		public HeroDeathProbabilityCalculationModel HeroDeathProbabilityCalculationModel { get; private set; }

		public BuildingConstructionModel BuildingConstructionModel { get; private set; }

		public BuildingEffectModel BuildingEffectModel { get; private set; }

		public WallHitPointCalculationModel WallHitPointCalculationModel { get; private set; }

		public MarriageModel MarriageModel { get; private set; }

		public AgeModel AgeModel { get; private set; }

		public PlayerProgressionModel PlayerProgressionModel { get; private set; }

		public DailyTroopXpBonusModel DailyTroopXpBonusModel { get; private set; }

		public PregnancyModel PregnancyModel { get; private set; }

		public NotablePowerModel NotablePowerModel { get; private set; }

		public MilitaryPowerModel MilitaryPowerModel { get; private set; }

		public PrisonerDonationModel PrisonerDonationModel { get; private set; }

		public NotableSpawnModel NotableSpawnModel { get; private set; }

		public TournamentModel TournamentModel { get; private set; }

		public CrimeModel CrimeModel { get; private set; }

		public DisguiseDetectionModel DisguiseDetectionModel { get; private set; }

		public BribeCalculationModel BribeCalculationModel { get; private set; }

		public TroopSacrificeModel TroopSacrificeModel { get; private set; }

		public SiegeStrategyActionModel SiegeStrategyActionModel { get; private set; }

		public SiegeEventModel SiegeEventModel { get; private set; }

		public SiegeAftermathModel SiegeAftermathModel { get; private set; }

		public SiegeLordsHallFightModel SiegeLordsHallFightModel { get; private set; }

		public CompanionHiringPriceCalculationModel CompanionHiringPriceCalculationModel { get; private set; }

		public BuildingScoreCalculationModel BuildingScoreCalculationModel { get; private set; }

		public SettlementAccessModel SettlementAccessModel { get; private set; }

		public IssueModel IssueModel { get; private set; }

		public PrisonerRecruitmentCalculationModel PrisonerRecruitmentCalculationModel { get; private set; }

		public PartyTroopUpgradeModel PartyTroopUpgradeModel { get; private set; }

		public TavernMercenaryTroopsModel TavernMercenaryTroopsModel { get; private set; }

		public WorkshopModel WorkshopModel { get; private set; }

		public DifficultyModel DifficultyModel { get; private set; }

		public LocationModel LocationModel { get; private set; }

		public PrisonBreakModel PrisonBreakModel { get; private set; }

		public BattleCaptainModel BattleCaptainModel { get; private set; }

		public ExecutionRelationModel ExecutionRelationModel { get; private set; }

		public BannerItemModel BannerItemModel { get; private set; }

		public DelayedTeleportationModel DelayedTeleportationModel { get; private set; }

		public TroopSupplierProbabilityModel TroopSupplierProbabilityModel { get; private set; }

		public CutsceneSelectionModel CutsceneSelectionModel { get; private set; }

		public EquipmentSelectionModel EquipmentSelectionModel { get; private set; }

		public AlleyModel AlleyModel { get; private set; }

		public VoiceOverModel VoiceOverModel { get; private set; }

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

		private void MakeGameComponentBindings()
		{
		}

		public GameModels(IEnumerable<GameModel> inputComponents)
			: base(inputComponents)
		{
			this.GetSpecificGameBehaviors();
			this.MakeGameComponentBindings();
		}
	}
}
