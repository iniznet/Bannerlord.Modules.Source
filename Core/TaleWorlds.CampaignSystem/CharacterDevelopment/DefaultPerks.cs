using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	public class DefaultPerks
	{
		private static DefaultPerks Instance
		{
			get
			{
				return Campaign.Current.DefaultPerks;
			}
		}

		public DefaultPerks()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			this._oneHandedWrappedHandles = this.Create("OneHandedWrappedHandles");
			this._oneHandedBasher = this.Create("OneHandedBasher");
			this._oneHandedToBeBlunt = this.Create("OneHandedToBeBlunt");
			this._oneHandedSwiftStrike = this.Create("OneHandedSwiftStrike");
			this._oneHandedCavalry = this.Create("OneHandedCavalry");
			this._oneHandedShieldBearer = this.Create("OneHandedShieldBearer");
			this._oneHandedTrainer = this.Create("OneHandedTrainer");
			this._oneHandedDuelist = this.Create("OneHandedDuelist");
			this._oneHandedShieldWall = this.Create("OneHandedShieldWall");
			this._oneHandedArrowCatcher = this.Create("OneHandedArrowCatcher");
			this._oneHandedMilitaryTradition = this.Create("OneHandedMilitaryTradition");
			this._oneHandedCorpsACorps = this.Create("OneHandedCorpsACorps");
			this._oneHandedStandUnited = this.Create("OneHandedStandUnited");
			this._oneHandedLeadByExample = this.Create("OneHandedLeadByExample");
			this._oneHandedSteelCoreShields = this.Create("OneHandedSteelCoreShields");
			this._oneHandedFleetOfFoot = this.Create("OneHandedFleetOfFoot");
			this._oneHandedDeadlyPurpose = this.Create("OneHandedDeadlyPurpose");
			this._oneHandedUnwaveringDefense = this.Create("OneHandedUnwaveringDefense");
			this._oneHandedPrestige = this.Create("OneHandedPrestige");
			this._oneHandedChinkInTheArmor = this.Create("OneHandedChinkInTheArmor");
			this._oneHandedWayOfTheSword = this.Create("OneHandedWayOfTheSword");
			this._twoHandedStrongGrip = this.Create("TwoHandedStrongGrip");
			this._twoHandedWoodChopper = this.Create("TwoHandedWoodChopper");
			this._twoHandedOnTheEdge = this.Create("TwoHandedOnTheEdge");
			this._twoHandedHeadBasher = this.Create("TwoHandedHeadBasher");
			this._twoHandedShowOfStrength = this.Create("TwoHandedShowOfStrength");
			this._twoHandedBaptisedInBlood = this.Create("TwoHandedBaptisedInBlood");
			this._twoHandedBeastSlayer = this.Create("TwoHandedBeastSlayer");
			this._twoHandedShieldBreaker = this.Create("TwoHandedShieldBreaker");
			this._twoHandedBerserker = this.Create("TwoHandedBerserker");
			this._twoHandedConfidence = this.Create("TwoHandedConfidence");
			this._twoHandedProjectileDeflection = this.Create("TwoHandedProjectileDeflection");
			this._twoHandedTerror = this.Create("TwoHandedTerror");
			this._twoHandedHope = this.Create("TwoHandedHope");
			this._twoHandedRecklessCharge = this.Create("TwoHandedRecklessCharge");
			this._twoHandedThickHides = this.Create("TwoHandedThickHides");
			this._twoHandedBladeMaster = this.Create("TwoHandedBladeMaster");
			this._twoHandedVandal = this.Create("TwoHandedVandal");
			this._twoHandedWayOfTheGreatAxe = this.Create("TwoHandedWayOfTheGreatAxe");
			this._polearmPikeman = this.Create("PolearmPikeman");
			this._polearmCavalry = this.Create("PolearmCavalry");
			this._polearmBraced = this.Create("PolearmBraced");
			this._polearmKeepAtBay = this.Create("PolearmKeepAtBay");
			this._polearmSwiftSwing = this.Create("PolearmSwiftSwing");
			this._polearmCleanThrust = this.Create("PolearmCleanThrust");
			this._polearmFootwork = this.Create("PolearmFootwork");
			this._polearmHardKnock = this.Create("PolearmHardKnock");
			this._polearmSteedKiller = this.Create("PolearmSteadKiller");
			this._polearmLancer = this.Create("PolearmLancer");
			this._polearmSkewer = this.Create("PolearmSkewer");
			this._polearmGuards = this.Create("PolearmGuards");
			this._polearmStandardBearer = this.Create("PolearmStandardBearer");
			this._polearmPhalanx = this.Create("PolearmPhalanx");
			this._polearmHardyFrontline = this.Create("PolearmHardyFrontline");
			this._polearmDrills = this.Create("PolearmDrills");
			this._polearmSureFooted = this.Create("PolearmSureFooted");
			this._polearmUnstoppableForce = this.Create("PolearmUnstoppableForce");
			this._polearmCounterweight = this.Create("PolearmCounterweight");
			this._polearmSharpenTheTip = this.Create("PolearmSharpenTheTip");
			this._polearmWayOfTheSpear = this.Create("PolearmWayOfTheSpear");
			this._bowBowControl = this.Create("BowBowControl");
			this._bowDeadAim = this.Create("BowDeadAim");
			this._bowBodkin = this.Create("BowBodkin");
			this._bowRangersSwiftness = this.Create("BowRangersSwiftness");
			this._bowRapidFire = this.Create("BowRapidFire");
			this._bowQuickAdjustments = this.Create("BowQuickAdjustments");
			this._bowMerryMen = this.Create("BowMerryMen");
			this._bowMountedArchery = this.Create("BowMountedArchery");
			this._bowTrainer = this.Create("BowTrainer");
			this._bowStrongBows = this.Create("BowStrongBows");
			this._bowDiscipline = this.Create("BowDiscipline");
			this._bowHunterClan = this.Create("BowHunterClan");
			this._bowSkirmishPhaseMaster = this.Create("BowSkirmishPhaseMaster");
			this._bowEagleEye = this.Create("BowEagleEye");
			this._bowBullsEye = this.Create("BowBullsEye");
			this._bowRenownedArcher = this.Create("BowRenownedArcher");
			this._bowHorseMaster = this.Create("BowHorseMaster");
			this._bowDeepQuivers = this.Create("BowDeepQuivers");
			this._bowQuickDraw = this.Create("BowQuickDraw");
			this._bowNockingPoint = this.Create("BowNockingPoint");
			this._bowDeadshot = this.Create("BowDeadshot");
			this._crossbowPiercer = this.Create("CrossbowPiercer");
			this._crossbowMarksmen = this.Create("CrossbowMarksmen");
			this._crossbowUnhorser = this.Create("CrossbowUnhorser");
			this._crossbowWindWinder = this.Create("CrossbowWindWinder");
			this._crossbowDonkeysSwiftness = this.Create("CrossbowDonkeysSwiftness");
			this._crossbowSheriff = this.Create("CrossbowSheriff");
			this._crossbowPeasantLeader = this.Create("CrossbowPeasantLeader");
			this._crossbowRenownMarksmen = this.Create("CrossbowRenownMarksmen");
			this._crossbowFletcher = this.Create("CrossbowFletcher");
			this._crossbowPuncture = this.Create("CrossbowPuncture");
			this._crossbowLooseAndMove = this.Create("CrossbowLooseAndMove");
			this._crossbowDeftHands = this.Create("CrossbowDeftHands");
			this._crossbowCounterFire = this.Create("CrossbowCounterFire");
			this._crossbowMountedCrossbowman = this.Create("CrossbowMountedCrossbowman");
			this._crossbowSteady = this.Create("CrossbowSteady");
			this._crossbowLongShots = this.Create("CrossbowLongShots");
			this._crossbowHammerBolts = this.Create("CrossbowHammerBolts");
			this._crossbowPavise = this.Create("CrossbowPavise");
			this._crossbowTerror = this.Create("CrossbowTerror");
			this._crossbowPickedShots = this.Create("CrossbowBoltenGuard");
			this._crossbowMightyPull = this.Create("CrossbowMightyPull");
			this._throwingQuickDraw = this.Create("ThrowingQuickDraw");
			this._throwingShieldBreaker = this.Create("ThrowingShieldBreaker");
			this._throwingHunter = this.Create("ThrowingHunter");
			this._throwingFlexibleFighter = this.Create("ThrowingFlexibleFighter");
			this._throwingMountedSkirmisher = this.Create("ThrowingMountedSkirmisher");
			this._throwingPerfectTechnique = this.Create("ThrowingPerfectTechnique");
			this._throwingRunningThrow = this.Create("ThrowingRunningThrow");
			this._throwingKnockOff = this.Create("ThrowingKnockOff");
			this._throwingSkirmisher = this.Create("ThrowingSkirmisher");
			this._throwingWellPrepared = this.Create("ThrowingWellPrepared");
			this._throwingFocus = this.Create("ThrowingFocus");
			this._throwingLastHit = this.Create("ThrowingLastHit");
			this._throwingHeadHunter = this.Create("ThrowingHeadHunter");
			this._throwingThrowingCompetitions = this.Create("ThrowingThrowingCompetitions");
			this._throwingSaddlebags = this.Create("ThrowingSaddlebags");
			this._throwingSplinters = this.Create("ThrowingSplinters");
			this._throwingResourceful = this.Create("ThrowingResourceful");
			this._throwingLongReach = this.Create("ThrowingLongReach");
			this._throwingWeakSpot = this.Create("ThrowingWeakSpot");
			this._throwingImpale = this.Create("ThrowingImpale");
			this._throwingUnstoppableForce = this.Create("ThrowingUnstoppableForce");
			this._ridingFullSpeed = this.Create("RidingFullSpeed");
			this._ridingNimbleSteed = this.Create("RidingNimbleStead");
			this._ridingWellStraped = this.Create("RidingWellStraped");
			this._ridingVeterinary = this.Create("RidingVeterinary");
			this._ridingNomadicTraditions = this.Create("RidingNomadicTraditions");
			this._ridingDeeperSacks = this.Create("RidingDeeperSacks");
			this._ridingSagittarius = this.Create("RidingSagittarius");
			this._ridingSweepingWind = this.Create("RidingSweepingWind");
			this._ridingReliefForce = this.Create("RidingReliefForce");
			this._ridingMountedWarrior = this.Create("RidingMountedWarrior");
			this._ridingHorseArcher = this.Create("RidingHorseArcher");
			this._ridingShepherd = this.Create("RidingShepherd");
			this._ridingBreeder = this.Create("RidingBreeder");
			this._ridingThunderousCharge = this.Create("RidingThunderousCharge");
			this._ridingAnnoyingBuzz = this.Create("RidingAnnoyingBuzz");
			this._ridingMountedPatrols = this.Create("RidingMountedPatrols");
			this._ridingCavalryTactics = this.Create("RidingCavalryTactics");
			this._ridingDauntlessSteed = this.Create("RidingDauntlessSteed");
			this._ridingToughSteed = this.Create("RidingToughSteed");
			this._ridingTheWayOfTheSaddle = this.Create("RidingTheWayOfTheSaddle");
			this._athleticsMorningExercise = this.Create("AthleticsMorningExercise");
			this._athleticsWellBuilt = this.Create("AthleticsWellBuilt");
			this._athleticsFury = this.Create("AthleticsFury");
			this._athleticsFormFittingArmor = this.Create("AthleticsFormFittingArmor");
			this._athleticsImposingStature = this.Create("AthleticsImposingStature");
			this._athleticsStamina = this.Create("AthleticsStamina");
			this._athleticsSprint = this.Create("AthleticsSprint");
			this._athleticsPowerful = this.Create("AthleticsPowerful");
			this._athleticsSurgingBlow = this.Create("AthleticsSurgingBlow");
			this._athleticsBraced = this.Create("AthleticsBraced");
			this._athleticsWalkItOff = this.Create("AthleticsWalkItOff");
			this._athleticsAGoodDaysRest = this.Create("AthleticsAGoodDaysRest");
			this._athleticsDurable = this.Create("AthleticsDurable");
			this._athleticsEnergetic = this.Create("AthleticsEnergetic");
			this._athleticsSteady = this.Create("AthleticsSteady");
			this._athleticsStrong = this.Create("AthleticsStrong");
			this._athleticsStrongLegs = this.Create("AthleticsStrongLegs");
			this._athleticsStrongArms = this.Create("AthleticsStrongArms");
			this._athleticsSpartan = this.Create("AthleticsSpartan");
			this._athleticsIgnorePain = this.Create("AthleticsIgnorePain");
			this._athleticsMightyBlow = this.Create("AthleticsMightyBlow");
			this._craftingSharpenedEdge = this.Create("CraftingSharpenedEdge");
			this._craftingSharpenedTip = this.Create("CraftingSharpenedTip");
			this._craftingIronMaker = this.Create("IronYield");
			this._craftingCharcoalMaker = this.Create("CharcoalYield");
			this._craftingSteelMaker = this.Create("SteelMaker");
			this._craftingSteelMaker2 = this.Create("SteelMaker2");
			this._craftingSteelMaker3 = this.Create("SteelMaker3");
			this._craftingCuriousSmelter = this.Create("CuriousSmelter");
			this._craftingCuriousSmith = this.Create("CuriousSmith");
			this._craftingPracticalRefiner = this.Create("PracticalRefiner");
			this._craftingPracticalSmelter = this.Create("PracticalSmelter");
			this._craftingPracticalSmith = this.Create("PracticalSmith");
			this._craftingArtisanSmith = this.Create("ArtisanSmith");
			this._craftingExperiencedSmith = this.Create("ExperiencedSmith");
			this._craftingMasterSmith = this.Create("MasterSmith");
			this._craftingLegendarySmith = this.Create("LegendarySmith");
			this._craftingVigorousSmith = this.Create("VigorousSmith");
			this._craftingStrongSmith = this.Create("StrongSmith");
			this._craftingEnduringSmith = this.Create("EnduringSmith");
			this._craftingFencerSmith = this.Create("WeaponMasterSmith");
			this._tacticsTightFormations = this.Create("TacticsTightFormations");
			this._tacticsLooseFormations = this.Create("TacticsLooseFormations");
			this._tacticsExtendedSkirmish = this.Create("TacticsExtendedSkirmish");
			this._tacticsDecisiveBattle = this.Create("TacticsDecisiveBattle");
			this._tacticsSmallUnitTactics = this.Create("TacticsSmallUnitTactics");
			this._tacticsHordeLeader = this.Create("TacticsHordeLeader");
			this._tacticsLawKeeper = this.Create("TacticsLawkeeper");
			this._tacticsCoaching = this.Create("TacticsCoaching");
			this._tacticsSwiftRegroup = this.Create("TacticsSwiftRegroup");
			this._tacticsImproviser = this.Create("TacticsImproviser");
			this._tacticsOnTheMarch = this.Create("TacticsOnTheMarch");
			this._tacticsCallToArms = this.Create("TacticsCallToArms");
			this._tacticsPickThemOfTheWalls = this.Create("TacticsPickThemOfTheWalls");
			this._tacticsMakeThemPay = this.Create("TacticsMakeThemPay");
			this._tacticsEliteReserves = this.Create("TacticsEliteReserves");
			this._tacticsEncirclement = this.Create("TacticsEncirclement");
			this._tacticsPreBattleManeuvers = this.Create("TacticsPreBattleManeuvers");
			this._tacticsBesieged = this.Create("TacticsBesieged");
			this._tacticsCounteroffensive = this.Create("TacticsCounteroffensive");
			this._tacticsGensdarmes = this.Create("TacticsGensdarmes");
			this._tacticsTacticalMastery = this.Create("TacticsTacticalMastery");
			this._scoutingDayTraveler = this.Create("ScoutingDayTraveler");
			this._scoutingNightRunner = this.Create("ScoutingNightRunner");
			this._scoutingPathfinder = this.Create("ScoutingPathfinder");
			this._scoutingWaterDiviner = this.Create("ScoutingWaterDiviner");
			this._scoutingForestKin = this.Create("ScoutingForestKin");
			this._scoutingDesertBorn = this.Create("ScoutingDesertBorn");
			this._scoutingForcedMarch = this.Create("ScoutingForcedMarch");
			this._scoutingUnburdened = this.Create("ScoutingUnburdened");
			this._scoutingTracker = this.Create("ScoutingTracker");
			this._scoutingRanger = this.Create("ScoutingRanger");
			this._scoutingMountedScouts = this.Create("ScoutingMountedScouts");
			this._scoutingPatrols = this.Create("ScoutingPatrols");
			this._scoutingForagers = this.Create("ScoutingForagers");
			this._scoutingBeastWhisperer = this.Create("ScoutingBeastWhisperer");
			this._scoutingVillageNetwork = this.Create("ScoutingVillageNetwork");
			this._scoutingRumourNetwork = this.Create("ScoutingRumourNetwork");
			this._scoutingVantagePoint = this.Create("ScoutingVantagePoint");
			this._scoutingKeenSight = this.Create("ScoutingKeenSight");
			this._scoutingVanguard = this.Create("ScoutingVanguard");
			this._scoutingRearguard = this.Create("ScoutingRearguard");
			this._scoutingUncannyInsight = this.Create("ScoutingUncannyInsight");
			this._rogueryNoRestForTheWicked = this.Create("RogueryNoRestForTheWicked");
			this._roguerySweetTalker = this.Create("RoguerySweetTalker");
			this._rogueryTwoFaced = this.Create("RogueryTwoFaced");
			this._rogueryDeepPockets = this.Create("RogueryDeepPockets");
			this._rogueryInBestLight = this.Create("RogueryInBestLight");
			this._rogueryKnowHow = this.Create("RogueryKnowHow");
			this._rogueryPromises = this.Create("RogueryPromises");
			this._rogueryManhunter = this.Create("RogueryManhunter");
			this._rogueryScarface = this.Create("RogueryScarface");
			this._rogueryWhiteLies = this.Create("RogueryWhiteLies");
			this._roguerySmugglerConnections = this.Create("RoguerySmugglerConnections");
			this._rogueryPartnersInCrime = this.Create("RogueryPartnersInCrime");
			this._rogueryOneOfTheFamily = this.Create("RogueryOneOfTheFamily");
			this._roguerySaltTheEarth = this.Create("RoguerySaltTheEarth");
			this._rogueryCarver = this.Create("RogueryCarver");
			this._rogueryRansomBroker = this.Create("RogueryRansomBroker");
			this._rogueryArmsDealer = this.Create("RogueryArmsDealer");
			this._rogueryDirtyFighting = this.Create("RogueryDirtyFighting");
			this._rogueryDashAndSlash = this.Create("RogueryDashAndSlash");
			this._rogueryFleetFooted = this.Create("RogueryFleetFooted");
			this._rogueryRogueExtraordinaire = this.Create("RogueryRogueExtraordinaire");
			this._leadershipCombatTips = this.Create("LeadershipCombatTips");
			this._leadershipRaiseTheMeek = this.Create("LeadershipRaiseTheMeek");
			this._leadershipFerventAttacker = this.Create("LeadershipFerventAttacker");
			this._leadershipStoutDefender = this.Create("LeadershipStoutDefender");
			this._leadershipAuthority = this.Create("LeadershipAuthority");
			this._leadershipHeroicLeader = this.Create("LeadershipHeroicLeader");
			this._leadershipLoyaltyAndHonor = this.Create("LeadershipLoyaltyAndHonor");
			this._leadershipFamousCommander = this.Create("LeadershipFamousCommander");
			this._leadershipPresence = this.Create("LeadershipPresence");
			this._leadershipLeaderOfTheMasses = this.Create("LeadershipLeaderOfMasses");
			this._leadershipVeteransRespect = this.Create("LeadershipVeteransRespect");
			this._leadershipCitizenMilitia = this.Create("LeadershipCitizenMilitia");
			this._leadershipInspiringLeader = this.Create("LeadershipInspiringLeader");
			this._leadershipUpliftingSpirit = this.Create("LeadershipUpliftingSpirit");
			this._leadershipMakeADifference = this.Create("LeadershipMakeADifference");
			this._leadershipLeadByExample = this.Create("LeadershipLeadByExample");
			this._leadershipTrustedCommander = this.Create("LeadershipTrustedCommander");
			this._leadershipGreatLeader = this.Create("LeadershipGreatLeader");
			this._leadershipWePledgeOurSwords = this.Create("LeadershipWePledgeOurSwords");
			this._leadershipTalentMagnet = this.Create("LeadershipTalentMagnet");
			this._leadershipUltimateLeader = this.Create("LeadershipUltimateLeader");
			this._charmVirile = this.Create("CharmVirile");
			this._charmSelfPromoter = this.Create("CharmSelfPromoter");
			this._charmOratory = this.Create("CharmOratory");
			this._charmWarlord = this.Create("CharmWarlord");
			this._charmForgivableGrievances = this.Create("CharmForgivableGrievances");
			this._charmMeaningfulFavors = this.Create("CharmMeaningfulFavors");
			this._charmInBloom = this.Create("CharmInBloom");
			this._charmYoungAndRespectful = this.Create("CharmYoungAndRespectful");
			this._charmFirebrand = this.Create("CharmFirebrand");
			this._charmFlexibleEthics = this.Create("CharmFlexibleEthics");
			this._charmEffortForThePeople = this.Create("CharmEffortForThePeople");
			this._charmSlickNegotiator = this.Create("CharmSlickNegotiator");
			this._charmGoodNatured = this.Create("CharmGoodNatured");
			this._charmTribute = this.Create("CharmTribute");
			this._charmMoralLeader = this.Create("CharmMoralLeader");
			this._charmNaturalLeader = this.Create("CharmNaturalLeader");
			this._charmPublicSpeaker = this.Create("CharmPublicSpeaker");
			this._charmParade = this.Create("CharmParade");
			this._charmCamaraderie = this.Create("CharmCamaraderie");
			this._charmImmortalCharm = this.Create("CharmImmortalCharm");
			this._tradeAppraiser = this.Create("TradeAppraiser");
			this._tradeWholeSeller = this.Create("TradeWholeSeller");
			this._tradeCaravanMaster = this.Create("TradeCaravanMaster");
			this._tradeMarketDealer = this.Create("TradeMarketDealer");
			this._tradeTravelingRumors = this.Create("TradeTravelingRumors");
			this._tradeLocalConnection = this.Create("TradeLocalConnection");
			this._tradeDistributedGoods = this.Create("TradeDistributedGoods");
			this._tradeTollgates = this.Create("TradeTollgates");
			this._tradeArtisanCommunity = this.Create("TradeArtisanCommunity");
			this._tradeGreatInvestor = this.Create("TradeGreatInvestor");
			this._tradeMercenaryConnections = this.Create("TradeMercenaryConnections");
			this._tradeContentTrades = this.Create("TradeContentTrades");
			this._tradeInsurancePlans = this.Create("TradeInsurancePlans");
			this._tradeRapidDevelopment = this.Create("TradeRapidDevelopment");
			this._tradeGranaryAccountant = this.Create("TradeGranaryAccountant");
			this._tradeTradeyardForeman = this.Create("TradeTradeyardForeman");
			this._tradeSwordForBarter = this.Create("TradeSwordForBarter");
			this._tradeSelfMadeMan = this.Create("TradeSelfMadeMan");
			this._tradeSilverTongue = this.Create("TradeSilverTongue");
			this._tradeSpringOfGold = this.Create("TradeSpringOfGold");
			this._tradeManOfMeans = this.Create("TradeManOfMeans");
			this._tradeTrickleDown = this.Create("TradeTrickleDown");
			this._tradeEverythingHasAPrice = this.Create("TradeEverythingHasAPrice");
			this._stewardWarriorsDiet = this.Create("StewardWarriorsDiet");
			this._stewardFrugal = this.Create("StewardFrugal");
			this._stewardSevenVeterans = this.Create("StewardSevenVeterans");
			this._stewardDrillSergant = this.Create("StewardDrillSergant");
			this._stewardSweatshops = this.Create("StewardSweatshops");
			this._stewardStiffUpperLip = this.Create("StewardStiffUpperLip");
			this._stewardPaidInPromise = this.Create("StewardPaidInPromise");
			this._stewardEfficientCampaigner = this.Create("StewardEfficientCampaigner");
			this._stewardGivingHands = this.Create("StewardForeseeableFuture");
			this._stewardLogistician = this.Create("StewardLogistician");
			this._stewardRelocation = this.Create("StewardRelocation");
			this._stewardAidCorps = this.Create("StewardAidCorps");
			this._stewardGourmet = this.Create("StewardGourmet");
			this._stewardSoundReserves = this.Create("StewardSoundReserves");
			this._stewardForcedLabor = this.Create("StewardForcedLabor");
			this._stewardContractors = this.Create("StewardContractors");
			this._stewardArenicosMules = this.Create("StewardArenicosMules");
			this._stewardArenicosHorses = this.Create("StewardArenicosHorses");
			this._stewardMasterOfPlanning = this.Create("StewardMasterOfPlanning");
			this._stewardMasterOfWarcraft = this.Create("StewardMasterOfWarcraft");
			this._stewardPriceOfLoyalty = this.Create("StewardPriceOfLoyalty");
			this._medicineSelfMedication = this.Create("MedicineSelfMedication");
			this._medicinePreventiveMedicine = this.Create("MedicinePreventiveMedicine");
			this._medicineTriageTent = this.Create("MedicineTriageTent");
			this._medicineWalkItOff = this.Create("MedicineWalkItOff");
			this._medicineSledges = this.Create("MedicineSledges");
			this._medicineDoctorsOath = this.Create("MedicineDoctorsOath");
			this._medicineBestMedicine = this.Create("MedicineBestMedicine");
			this._medicineGoodLodging = this.Create("MedicineGoodLodging");
			this._medicineSiegeMedic = this.Create("MedicineSiegeMedic");
			this._medicineVeterinarian = this.Create("MedicineVeterinarian");
			this._medicinePristineStreets = this.Create("MedicinePristineStreets");
			this._medicineBushDoctor = this.Create("MedicineBushDoctor");
			this._medicinePerfectHealth = this.Create("MedicinePerfectHealth");
			this._medicineHealthAdvise = this.Create("MedicineHealthAdvise");
			this._medicinePhysicianOfPeople = this.Create("MedicinePhysicianOfPeople");
			this._medicineCleanInfrastructure = this.Create("MedicineCleanInfrastructure");
			this._medicineCheatDeath = this.Create("MedicineCheatDeath");
			this._medicineFortitudeTonic = this.Create("MedicineFortitudeTonic");
			this._medicineHelpingHands = this.Create("MedicineHelpingHands");
			this._medicineBattleHardened = this.Create("MedicineBattleHardened");
			this._medicineMinisterOfHealth = this.Create("MedicineMinisterOfHealth");
			this._engineeringScaffolds = this.Create("EngineeringScaffolds");
			this._engineeringTorsionEngines = this.Create("EngineeringTorsionEngines");
			this._engineeringSiegeWorks = this.Create("EngineeringSiegeWorks");
			this._engineeringDungeonArchitect = this.Create("EngineeringDungeonArchitect");
			this._engineeringCarpenters = this.Create("EngineeringCarpenters");
			this._engineeringMilitaryPlanner = this.Create("EngineeringMilitaryPlanner");
			this._engineeringWallBreaker = this.Create("EngineeringWallBreaker");
			this._engineeringDreadfulSieger = this.Create("EngineeringDreadfulSieger");
			this._engineeringSalvager = this.Create("EngineeringSalvager");
			this._engineeringForeman = this.Create("EngineeringForeman");
			this._engineeringStonecutters = this.Create("EngineeringStonecutters");
			this._engineeringSiegeEngineer = this.Create("EngineeringSiegeEngineer");
			this._engineeringCampBuilding = this.Create("EngineeringCampBuilding");
			this._engineeringBattlements = this.Create("EngineeringBattlements");
			this._engineeringEngineeringGuilds = this.Create("EngineeringEngineeringGuilds");
			this._engineeringApprenticeship = this.Create("EngineeringApprenticeship");
			this._engineeringMetallurgy = this.Create("EngineeringMetallurgy");
			this._engineeringImprovedTools = this.Create("EngineeringImprovedTools");
			this._engineeringClockwork = this.Create("EngineeringClockwork");
			this._engineeringArchitecturalCommisions = this.Create("EngineeringArchitecturalCommissions");
			this._engineeringMasterwork = this.Create("EngineeringMasterwork");
			this.InitializeAll();
		}

		private void InitializeAll()
		{
			this._oneHandedWrappedHandles.Initialize("{=looKU9Gl}Wrapped Handles", DefaultSkills.OneHanded, this.GetTierCost(1), this._oneHandedBasher, "{=dY3GOmTN}{VALUE}% handling to one handed weapons.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=0mBHB7mA}{VALUE} one handed skill to infantry troops in your formation.", SkillEffect.PerkRole.Captain, 30f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.OneHandedUser);
			this._oneHandedBasher.Initialize("{=6yEeYNRu}Basher", DefaultSkills.OneHanded, this.GetTierCost(1), this._oneHandedWrappedHandles, "{=fFNNeqxu}{VALUE}% damage and longer stun duration with shield bashes.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=goOE8oiI}{VALUE}% damage taken by infantry while in shield wall formation.", SkillEffect.PerkRole.Captain, -0.04f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._oneHandedToBeBlunt.Initialize("{=SJ69EYuI}To Be Blunt", DefaultSkills.OneHanded, this.GetTierCost(2), this._oneHandedSwiftStrike, "{=mzUa3duw}{VALUE}% damage with one handed axes and maces.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=Ib9RYpMO}{VALUE} daily security to governed settlement.", SkillEffect.PerkRole.Governor, 0.5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedSwiftStrike.Initialize("{=ciELES5v}Swift Strike", DefaultSkills.OneHanded, this.GetTierCost(2), this._oneHandedToBeBlunt, "{=bW7DT97A}{VALUE}% swing speed with one handed weapons.", SkillEffect.PerkRole.Personal, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "{=xwA6Om0Y}{VALUE} daily militia recruitment in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedCavalry.Initialize("{=YVGtcLHF}Cavalry", DefaultSkills.OneHanded, this.GetTierCost(3), this._oneHandedShieldBearer, "{=D3k7UbmZ}{VALUE}% damage with one handed weapons while mounted.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=aj2R3vnb}{VALUE}% melee damage by cavalry troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.Melee);
			this._oneHandedShieldBearer.Initialize("{=vnG1q18y}Shield Bearer", DefaultSkills.OneHanded, this.GetTierCost(3), this._oneHandedCavalry, "{=hMJVRJdw}Removed movement speed penalty of wielding shields.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Invalid, "{=1QsZq9UW}{VALUE}% movement speed to infantry in your formation.", SkillEffect.PerkRole.Captain, 0.03f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._oneHandedTrainer.Initialize("{=3xuwVbfs}Trainer", DefaultSkills.OneHanded, this.GetTierCost(4), this._oneHandedDuelist, "{=Ti9auMiO}{VALUE} hit points.", SkillEffect.PerkRole.Personal, 2f, SkillEffect.EffectIncrementType.Add, "{=rXb91Rwi}{VALUE}% experience to melee troops in your party after every battle.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedDuelist.Initialize("{=XphY9cNV}Duelist", DefaultSkills.OneHanded, this.GetTierCost(4), this._oneHandedTrainer, "{=uRZgz63l}{VALUE}% damage while wielding a one handed weapon without a shield.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=uKTgBX4S}Double the amount of renown gained from tournaments.", SkillEffect.PerkRole.Personal, 2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedShieldWall.Initialize("{=nSwkI97I}Shieldwall", DefaultSkills.OneHanded, this.GetTierCost(5), this._oneHandedArrowCatcher, "{=DiFIyniQ}{VALUE}% damage to your shield while blocking in wrong direction.", SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=EdDRYFoL}Larger shield protection area against projectiles to troops in your formation while in shield wall formation.", SkillEffect.PerkRole.Captain, 0.01f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.ShieldUser);
			this._oneHandedArrowCatcher.Initialize("{=a94mkNNk}Arrow Catcher", DefaultSkills.OneHanded, this.GetTierCost(5), this._oneHandedShieldWall, "{=dcsschkC}Larger shield protection area against projectiles.", SkillEffect.PerkRole.Personal, 0.01f, SkillEffect.EffectIncrementType.Add, "{=uz7KxUlP}Larger shield protection area against projectiles for troops in your formation.", SkillEffect.PerkRole.Captain, 0.01f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.ShieldUser);
			this._oneHandedMilitaryTradition.Initialize("{=Fc7OsyZ8}Military Tradition", DefaultSkills.OneHanded, this.GetTierCost(6), this._oneHandedCorpsACorps, "{=0A6BUASZ}{VALUE} daily experience to infantry in your party.", SkillEffect.PerkRole.PartyLeader, 2f, SkillEffect.EffectIncrementType.Add, "{=B2msxAju}{VALUE}% garrison wages in the governed settlement.", SkillEffect.PerkRole.Governor, -0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedCorpsACorps.Initialize("{=M3aNEkBJ}Corps-a-corps", DefaultSkills.OneHanded, this.GetTierCost(6), this._oneHandedMilitaryTradition, "{=8jHJeh8z}{VALUE}% of the total experience gained as a bonus to infantry after battles.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=wBgpln4f}{VALUE} garrison limit in the governed settlement.", SkillEffect.PerkRole.Governor, 30f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedStandUnited.Initialize("{=d8qjwKza}Stand United", DefaultSkills.OneHanded, this.GetTierCost(7), this._oneHandedLeadByExample, "{=JZ8ihtoa}{VALUE} starting battle morale to troops in your party if you are outnumbered.", SkillEffect.PerkRole.PartyLeader, 8f, SkillEffect.EffectIncrementType.Add, "{=5aVPqukr}{VALUE}% security provided by troops in the garrison of the governed settlement.", SkillEffect.PerkRole.Governor, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedLeadByExample.Initialize("{=bOhbWapX}Lead by example", DefaultSkills.OneHanded, this.GetTierCost(7), this._oneHandedStandUnited, "{=V97vqbIb}{VALUE}% experience to troops in your party after battle.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=g5nnybjz}{VALUE} starting battle morale to troops in your party.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedSteelCoreShields.Initialize("{=rSATpMpq}Steel Core Shields", DefaultSkills.OneHanded, this.GetTierCost(8), this._oneHandedFleetOfFoot, "{=q2gybZYy}{VALUE}% damage to your shields.", SkillEffect.PerkRole.Personal, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=Bb4L971j}{VALUE}% damage to shields of infantry troops in your formation.", SkillEffect.PerkRole.Captain, -0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.ShieldUser);
			this._oneHandedFleetOfFoot.Initialize("{=OtdkOGur}Fleet of Foot", DefaultSkills.OneHanded, this.GetTierCost(8), this._oneHandedSteelCoreShields, "{=V53EYEXx}{VALUE}% combat movement speed.", SkillEffect.PerkRole.Personal, 0.04f, SkillEffect.EffectIncrementType.AddFactor, "{=1QsZq9UW}{VALUE}% movement speed to infantry in your formation.", SkillEffect.PerkRole.Captain, 0.04f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._oneHandedDeadlyPurpose.Initialize("{=xpGoduJq}Deadly Purpose", DefaultSkills.OneHanded, this.GetTierCost(9), this._oneHandedUnwaveringDefense, "{=CTqO5MBf}{VALUE}% damage with one handed weapons.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=fcmt2U5u}{VALUE}% melee weapon damage by infantry in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot | TroopUsageFlags.Melee);
			this._oneHandedUnwaveringDefense.Initialize("{=yFbEDUyb}Unwavering Defense", DefaultSkills.OneHanded, this.GetTierCost(9), this._oneHandedDeadlyPurpose, "{=Ti9auMiO}{VALUE} hit points.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, "{=aeNhsyD7}{VALUE} hit points to infantry in your party.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedPrestige.Initialize("{=DSKtsYPi}Prestige", DefaultSkills.OneHanded, this.GetTierCost(10), this._oneHandedChinkInTheArmor, "{=LjeYTgX7}{VALUE}% damage against shields with one handed weapons.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=qxbBsB1a}{VALUE} party limit.", SkillEffect.PerkRole.PartyLeader, 15f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedChinkInTheArmor.Initialize("{=bBa0LB1D}Chink in the Armor", DefaultSkills.OneHanded, this.GetTierCost(10), this._oneHandedPrestige, "{=KKsCor3D}{VALUE}% armor penetration with melee attacks.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=3a6tmImq}{VALUE}% recruitment cost of infantry.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._oneHandedWayOfTheSword.Initialize("{=nThmB3yB}Way of the Sword", DefaultSkills.OneHanded, this.GetTierCost(11), null, "{=jan55Git}{VALUE}% attack speed with one handed weapons for every skill point above 250.", SkillEffect.PerkRole.Personal, 0.002f, SkillEffect.EffectIncrementType.AddFactor, "{=hr0TfX6o}{VALUE}% damage with one handed weapons for every skill point above 250.", SkillEffect.PerkRole.Personal, 0.005f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._twoHandedStrongGrip.Initialize("{=xDQTgPf0}Strong Grip", DefaultSkills.TwoHanded, this.GetTierCost(1), this._twoHandedWoodChopper, "{=OpVRgL9n}{VALUE}% handling to two handed weapons.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=TmYKKarv}{VALUE} two handed skill to infantry troops in your formation.", SkillEffect.PerkRole.Captain, 30f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot | TroopUsageFlags.TwoHandedUser);
			this._twoHandedWoodChopper.Initialize("{=J7oh7Vin}Wood Chopper", DefaultSkills.TwoHanded, this.GetTierCost(1), this._twoHandedStrongGrip, "{=impj5bAM}{VALUE}% damage to shields with two handed weapons.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=4u69jBeE}{VALUE}% damage against shields by troops in your formation.", SkillEffect.PerkRole.Captain, 0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._twoHandedOnTheEdge.Initialize("{=rkuAgPSA}On the Edge", DefaultSkills.TwoHanded, this.GetTierCost(2), this._twoHandedHeadBasher, "{=R8Lnif8l}{VALUE}% swing speed with two handed weapons.", SkillEffect.PerkRole.Personal, 0.03f, SkillEffect.EffectIncrementType.AddFactor, "{=z5DZXHF7}{VALUE}% swing speed to infantry in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot | TroopUsageFlags.Melee);
			this._twoHandedHeadBasher.Initialize("{=E5bgLJcs}Head Basher", DefaultSkills.TwoHanded, this.GetTierCost(2), this._twoHandedOnTheEdge, "{=qJBhadGi}{VALUE}% damage with two handed axes and maces.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=c86V0dch}{VALUE}% damage by infantry in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._twoHandedShowOfStrength.Initialize("{=RlMqzqbS}Show of Strength", DefaultSkills.TwoHanded, this.GetTierCost(3), this._twoHandedBaptisedInBlood, "{=7Nudvd8S}{VALUE}% chance of knocking the enemy down with a heavy hit.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=3a6tmImq}{VALUE}% recruitment cost of infantry.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._twoHandedBaptisedInBlood.Initialize("{=miMZavW3}Baptised in Blood", DefaultSkills.TwoHanded, this.GetTierCost(3), this._twoHandedShowOfStrength, "{=TR4ORD1T}{VALUE} experience to infantry in your party for each enemy you kill with a two handed weapon.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, "{=rXb91Rwi}{VALUE}% experience to melee troops in your party after every battle.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._twoHandedBeastSlayer.Initialize("{=NDtlE6PY}Beast Slayer", DefaultSkills.TwoHanded, this.GetTierCost(4), this._twoHandedShieldBreaker, "{=fxTRlxBD}{VALUE}% damage to mounts with two handed weapons.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=lekpGqEA}{VALUE}% damage to mounts by troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._twoHandedShieldBreaker.Initialize("{=bM9VX881}Shield breaker", DefaultSkills.TwoHanded, this.GetTierCost(4), this._twoHandedBeastSlayer, "{=impj5bAM}{VALUE}% damage to shields with two handed weapons.", SkillEffect.PerkRole.Personal, 0.4f, SkillEffect.EffectIncrementType.AddFactor, "{=ciCQnAj6}{VALUE}% damage against shields by troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._twoHandedBerserker.Initialize("{=RssJTUpL}Berserker", DefaultSkills.TwoHanded, this.GetTierCost(5), this._twoHandedConfidence, "{=D5RqqHIm}{VALUE}% damage with two handed weapons while you have less than half of your hit points.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=B2msxAju}{VALUE}% garrison wages in the governed settlement.", SkillEffect.PerkRole.Governor, -0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._twoHandedConfidence.Initialize("{=2jnsxsv5}Confidence", DefaultSkills.TwoHanded, this.GetTierCost(5), this._twoHandedBerserker, "{=QUXbhsxb}{VALUE}% damage with two handed weapons while you have more than 90% of your hit points.", SkillEffect.PerkRole.Personal, 0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=FX0GjiNa}{VALUE}% build speed to military projects in the governed settlement.", SkillEffect.PerkRole.Governor, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._twoHandedProjectileDeflection.Initialize("{=rCCG4mSJ}Projectile Deflection", DefaultSkills.TwoHanded, this.GetTierCost(6), null, "{=YP8tN7nl}You can deflect projectiles with two handed swords by blocking.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Invalid, "{=FdSPC05Q}{VALUE}% experience to garrison troops in the governed settlement.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._twoHandedTerror.Initialize("{=nAlCj2m0}Terror", DefaultSkills.TwoHanded, this.GetTierCost(7), this._twoHandedHope, "{=thGHcZMB}{VALUE}% battle morale effect to enemy troops with your two handed kills.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=POp8DAZD}{VALUE} prisoner limit.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._twoHandedHope.Initialize("{=lPuk6bao}Hope", DefaultSkills.TwoHanded, this.GetTierCost(7), this._twoHandedTerror, "{=2zNrVsDj}{VALUE}% battle morale effect to friendly troops with your two handed kills.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=qxbBsB1a}{VALUE} party limit.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._twoHandedRecklessCharge.Initialize("{=ZGovl01w}Reckless Charge", DefaultSkills.TwoHanded, this.GetTierCost(8), this._twoHandedThickHides, "{=1PC4D2fx}{VALUE}% damage bonus from speed with two handed weapons while on foot.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=b5l18lo7}{VALUE}% damage and movement speed to infantry in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._twoHandedThickHides.Initialize("{=j9OIuxxY}Thick Hides", DefaultSkills.TwoHanded, this.GetTierCost(8), this._twoHandedRecklessCharge, "{=Ti9auMiO}{VALUE} hit points.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, "{=ucHrYWaz}{VALUE} hit points to troops in your party.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._twoHandedBladeMaster.Initialize("{=TtwAoHfw}Blade Master", DefaultSkills.TwoHanded, this.GetTierCost(9), this._twoHandedVandal, "{=Pq0bhBUL}{VALUE}% damage with two handed weapons.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=l0ZGaUuI}{VALUE}% attack speed to infantry in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._twoHandedVandal.Initialize("{=czCRxHZy}Vandal", DefaultSkills.TwoHanded, this.GetTierCost(9), this._twoHandedBladeMaster, "{=u57OuUZR}{VALUE}% armor penetration with your attacks.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=8q4vzfbH}{VALUE}% damage against destructible objects by troops in your formation.", SkillEffect.PerkRole.Captain, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._twoHandedWayOfTheGreatAxe.Initialize("{=dbEb7iak}Way Of The Great Axe", DefaultSkills.TwoHanded, this.GetTierCost(10), null, "{=yRvF2Li4}{VALUE}% attack speed with two handed weapons for every skill point above 250.", SkillEffect.PerkRole.Personal, 0.002f, SkillEffect.EffectIncrementType.AddFactor, "{=IM05Jahy}{VALUE}% damage with two handed weapons for every skill point above 250.", SkillEffect.PerkRole.Personal, 0.005f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._polearmPikeman.Initialize("{=IFqtwpb0}Pikeman", DefaultSkills.Polearm, this.GetTierCost(1), this._polearmCavalry, "{=NtzmIh0g}{VALUE}% damage with polearms on foot.", SkillEffect.PerkRole.Personal, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "{=Yu5hTuIN}{VALUE}% damage by infantry troops in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._polearmCavalry.Initialize("{=YVGtcLHF}Cavalry", DefaultSkills.Polearm, this.GetTierCost(1), this._polearmPikeman, "{=IaBTfvfc}{VALUE}% damage with polearms while mounted.", SkillEffect.PerkRole.Personal, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "{=ywc8frAo}{VALUE}% damage by cavalry troops in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted);
			this._polearmBraced.Initialize("{=dU7haWkI}Braced", DefaultSkills.Polearm, this.GetTierCost(2), this._polearmKeepAtBay, "{=mmFrvHRt}{VALUE}% chance of dismounting enemy cavalry with a heavy hit.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=OWXECmbt}{VALUE}% damage by infantry in your formation against cavalry.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._polearmKeepAtBay.Initialize("{=TaucWWCB}Keep at Bay", DefaultSkills.Polearm, this.GetTierCost(2), this._polearmBraced, "{=ObFtbGZl}{VALUE}% chance of knocking enemies back with thrust attacks made with polearms.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=g9gTYB8u}{VALUE} militia recruitment in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._polearmSwiftSwing.Initialize("{=xM5aawCj}Swift Swing", DefaultSkills.Polearm, this.GetTierCost(3), this._polearmCleanThrust, "{=7tdcIxYN}{VALUE}% swing speed with polearms.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=Tq0E9sSW}{VALUE}% swing speed to infantry in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot | TroopUsageFlags.Melee);
			this._polearmCleanThrust.Initialize("{=PeaiNrSu}Clean Thrust", DefaultSkills.Polearm, this.GetTierCost(3), this._polearmSwiftSwing, "{=xEp10rIa}{VALUE}% thrust damage with polearms.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=apgpk6j1}{VALUE} polearm skill to infantry in your formation.", SkillEffect.PerkRole.Captain, 30f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot | TroopUsageFlags.PolearmUser);
			this._polearmFootwork.Initialize("{=Yvk8a2tb}Footwork", DefaultSkills.Polearm, this.GetTierCost(4), this._polearmHardKnock, "{=eDzl7r8u}{VALUE}% combat movement speed with polearms.", SkillEffect.PerkRole.Personal, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "{=1QsZq9UW}{VALUE}% movement speed to infantry in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._polearmHardKnock.Initialize("{=8DTKXbKw}Hard Knock", DefaultSkills.Polearm, this.GetTierCost(4), this._polearmFootwork, "{=7Nudvd8S}{VALUE}% chance of knocking the enemy down with a heavy hit.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=aeNhsyD7}{VALUE} hit points to infantry in your party.", SkillEffect.PerkRole.PartyLeader, 3f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._polearmSteedKiller.Initialize("{=8POWjrr6}Steed Killer", DefaultSkills.Polearm, this.GetTierCost(5), this._polearmLancer, "{=5aE8sEnj}{VALUE}% damage to mounts with polearms.", SkillEffect.PerkRole.Personal, 0.7f, SkillEffect.EffectIncrementType.AddFactor, "{=JGGdnIRO}{VALUE}% damage to mounts with polearms by infantry in your formation.", SkillEffect.PerkRole.Captain, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot | TroopUsageFlags.PolearmUser);
			this._polearmLancer.Initialize("{=hchDYAKL}Lancer", DefaultSkills.Polearm, this.GetTierCost(5), this._polearmSteedKiller, "{=I0hqrQuD}{VALUE}% damage bonus from speed with polearms while mounted.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=00mulBcs}{VALUE}% damage bonus from speed with polearms by troops in your formation.", SkillEffect.PerkRole.Captain, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.PolearmUser);
			this._polearmSkewer.Initialize("{=o57z0zB9}Skewer", DefaultSkills.Polearm, this.GetTierCost(6), this._polearmGuards, "{=dcVuMs9r}{VALUE}% chance of your lance staying couched after a kill.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=buFin46y}{VALUE} daily security in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._polearmGuards.Initialize("{=vquApOWo}Guards", DefaultSkills.Polearm, this.GetTierCost(6), this._polearmSkewer, "{=VB0GJijE}{VALUE}% damage when you hit an enemy in the head with a polearm.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=Ux90sIph}{VALUE}% experience gain to garrisoned cavalry in the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._polearmStandardBearer.Initialize("{=Vv81gkWN}Standard Bearer", DefaultSkills.Polearm, this.GetTierCost(7), this._polearmPhalanx, "{=RbDAfDsF}{VALUE}% battle morale loss to troops in your formation.", SkillEffect.PerkRole.Captain, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=V2v4ZMDT}{VALUE}% wages to garrisoned infantry in the governed settlement.", SkillEffect.PerkRole.Governor, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.None, TroopUsageFlags.Undefined);
			this._polearmPhalanx.Initialize("{=5vs3qlQ8}Phalanx", DefaultSkills.Polearm, this.GetTierCost(7), this._polearmStandardBearer, "{=3zzWzQcO}{VALUE} melee weapon skills to troops in your party while in shield wall formation.", SkillEffect.PerkRole.PartyLeader, 30f, SkillEffect.EffectIncrementType.Add, "{=yank0KD9}{VALUE}% damage with polearms by troops in your formation.", SkillEffect.PerkRole.Captain, 0.03f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.PolearmUser);
			this._polearmHardyFrontline.Initialize("{=NtMEk0lA}Hardy Frontline", DefaultSkills.Polearm, this.GetTierCost(8), this._polearmDrills, "{=ucHrYWaz}{VALUE} hit points to troops in your party.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, "{=3a6tmImq}{VALUE}% recruitment cost of infantry.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._polearmDrills.Initialize("{=JpiQagYa}Drills", DefaultSkills.Polearm, this.GetTierCost(8), this._polearmHardyFrontline, "{=g9gTYB8u}{VALUE} militia recruitment in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, "{=x3SJTtDj}{VALUE} bonus daily experience to troops in your party.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._polearmSureFooted.Initialize("{=bdzt4TcN}Sure Footed", DefaultSkills.Polearm, this.GetTierCost(9), this._polearmUnstoppableForce, "{=QqVLsf0N}{VALUE}% charge damage taken.", SkillEffect.PerkRole.Personal, -0.4f, SkillEffect.EffectIncrementType.AddFactor, "{=Dilnx8Es}{VALUE}% charge damage taken by troops in your formation.", SkillEffect.PerkRole.Captain, -0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._polearmUnstoppableForce.Initialize("{=Jat5GFDi}Unstoppable Force", DefaultSkills.Polearm, this.GetTierCost(9), this._polearmSureFooted, "{=cUaUTwx5}Triple couch lance damage against shields.", SkillEffect.PerkRole.Personal, 3f, SkillEffect.EffectIncrementType.AddFactor, "{=jaLOtaRh}{VALUE}% damage bonus from speed with polearms to cavalry in your formation.", SkillEffect.PerkRole.Captain, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.PolearmUser);
			this._polearmCounterweight.Initialize("{=BazrgEOj}Counterweight", DefaultSkills.Polearm, this.GetTierCost(10), this._polearmSharpenTheTip, "{=02IdNzbt}{VALUE}% handling of swingable polearms.", SkillEffect.PerkRole.Personal, 0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=uZPweUQg}{VALUE} polearm skill to troops in your formation.", SkillEffect.PerkRole.Captain, 20f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.PolearmUser);
			this._polearmSharpenTheTip.Initialize("{=ljduhdzj}Sharpen the Tip", DefaultSkills.Polearm, this.GetTierCost(10), this._polearmCounterweight, "{=sbkrplXi}{VALUE}% damage with thrust attacks made with polearms.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=wLrF0mzr}{VALUE}% damage with thrust attacks by infantry troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot | TroopUsageFlags.Melee);
			this._polearmWayOfTheSpear.Initialize("{=YnimoRlg}Way of the Spear", DefaultSkills.Polearm, this.GetTierCost(11), null, "{=x1T8wWNU}{VALUE}% attack speed with polearms for every skill point above 250.", SkillEffect.PerkRole.Personal, 0.002f, SkillEffect.EffectIncrementType.AddFactor, "{=UB67Ye3r}{VALUE}% damage with polearms for every skill point above 250.", SkillEffect.PerkRole.Personal, 0.005f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowBowControl.Initialize("{=1zteHA7R}Bow Control", DefaultSkills.Bow, this.GetTierCost(1), this._bowDeadAim, "{=4PdKPMNc}{VALUE}% accuracy penalty while moving.", SkillEffect.PerkRole.Personal, -0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=0DaxFvnw}{VALUE}% damage with bows by troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.BowUser);
			this._bowDeadAim.Initialize("{=FVLymWqW}Dead Aim", DefaultSkills.Bow, this.GetTierCost(1), this._bowBowControl, "{=hmbeQW4v}{VALUE}% headshot damage with bows.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=QbWK6sWo}{VALUE} Bow skill to troops in your formation.", SkillEffect.PerkRole.Captain, 20f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.BowUser);
			this._bowBodkin.Initialize("{=PDM8MzCA}Bodkin", DefaultSkills.Bow, this.GetTierCost(2), this._bowNockingPoint, "{=EU3No7XM}{VALUE}% armor penetration with bows.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=KfLZ8Hbw}{VALUE}% armor penetration with bows by troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.BowUser);
			this._bowNockingPoint.Initialize("{=bS8alS24}Nocking Point", DefaultSkills.Bow, this.GetTierCost(2), this._bowBodkin, "{=zady0hI7}{VALUE}% movement speed penalty while reloading.", SkillEffect.PerkRole.Personal, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=kaJ6SJeI}{VALUE}% movement speed to archers in your formation.", SkillEffect.PerkRole.Captain, 0.03f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.BowUser);
			this._bowRapidFire.Initialize("{=Kc9oatmM}Rapid Fire", DefaultSkills.Bow, this.GetTierCost(3), this._bowQuickAdjustments, "{=0vqPUXfr}{VALUE}% reload speed with bows.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=KOlw0Na1}{VALUE}% reload speed to troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Ranged);
			this._bowQuickAdjustments.Initialize("{=nOZerIfl}Quick Adjustments", DefaultSkills.Bow, this.GetTierCost(3), this._bowRapidFire, "{=LAxaYQzv}{VALUE}% accuracy penalty while rotating.", SkillEffect.PerkRole.Personal, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=qC298I3g}{VALUE}% accuracy penalty to archers in your formation.", SkillEffect.PerkRole.Captain, -0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.BowUser);
			this._bowMerryMen.Initialize("{=ssljPTUr}Merry Men", DefaultSkills.Bow, this.GetTierCost(4), this._bowMountedArchery, "{=NouDSrXE}{VALUE} party size.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, "{=g9gTYB8u}{VALUE} militia recruitment in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowMountedArchery.Initialize("{=WEUSMkCp}Mounted Archery", DefaultSkills.Bow, this.GetTierCost(4), this._bowMerryMen, "{=XITAY0KG}{VALUE}% accuracy penalty using bows while mounted.", SkillEffect.PerkRole.Personal, -0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=6XDcZUsb}{VALUE}% security provided by archers in the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowTrainer.Initialize("{=UE2X5rAy}Trainer", DefaultSkills.Bow, this.GetTierCost(5), this._bowStrongBows, "{=xoVR3Xr3}Daily Bow skill experience bonus to the party member with the lowest bow skill.", SkillEffect.PerkRole.PartyLeader, 6f, SkillEffect.EffectIncrementType.Add, "{=TcMme3Da}{VALUE} daily experience to archers in your party.", SkillEffect.PerkRole.PartyLeader, 3f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowStrongBows.Initialize("{=dqbbT5DK}Strong bows", DefaultSkills.Bow, this.GetTierCost(5), this._bowTrainer, "{=FXWn934b}{VALUE}% damage with bows.", SkillEffect.PerkRole.Personal, 0.08f, SkillEffect.EffectIncrementType.AddFactor, "{=yppPCR1z}{VALUE}% damage with bows by tier 3+ troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.BowUser);
			this._bowDiscipline.Initialize("{=D867vF71}Discipline", DefaultSkills.Bow, this.GetTierCost(6), this._bowHunterClan, "{=sHiIwnOb}{VALUE}% aiming duration without losing accuracy.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=F7bbkYx4}{VALUE} loyalty per day in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowHunterClan.Initialize("{=AAy1oX7z}Hunter Clan", DefaultSkills.Bow, this.GetTierCost(6), this._bowDiscipline, "{=kLVpYR0z}{VALUE}% damage with bows to mounts.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=1FPpHasQ}{VALUE}% garrison wages in the governed castle.", SkillEffect.PerkRole.Governor, -0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowSkirmishPhaseMaster.Initialize("{=oVdoauUE}Skirmish Phase Master", DefaultSkills.Bow, this.GetTierCost(7), this._bowEagleEye, "{=R93r6aV7}{VALUE}% damage taken from projectiles.", SkillEffect.PerkRole.Personal, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=pHdIgnYu}{VALUE}% damage taken from projectiles by ranged troops in your formation.", SkillEffect.PerkRole.Captain, -0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Ranged);
			this._bowEagleEye.Initialize("{=lq67KjSY}Eagle Eye", DefaultSkills.Bow, this.GetTierCost(7), this._bowSkirmishPhaseMaster, "{=xTDnna2J}{VALUE}% zoom with bows.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=1Z8oWbo7}{VALUE}% visual range on the campaign map.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowBullsEye.Initialize("{=QH77Weyq}Bulls Eye", DefaultSkills.Bow, this.GetTierCost(8), this._bowRenownedArcher, "{=OFMYfDPZ}{VALUE}% bonus experience to ranged troops in your party after every battle.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=mmH70R4H}{VALUE} daily experience to garrison troops in the governed settlement.", SkillEffect.PerkRole.Governor, 3f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Ranged, TroopUsageFlags.Undefined);
			this._bowRenownedArcher.Initialize("{=aIKVpGvE}Renowned Archer", DefaultSkills.Bow, this.GetTierCost(8), this._bowBullsEye, "{=kmdxvkEV}{VALUE}% starting battle morale to ranged troops in your party.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=bnnWpLbk}{VALUE}% recruitment and upgrade cost to ranged troops.", SkillEffect.PerkRole.PartyLeader, -0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowHorseMaster.Initialize("{=dbUybDTG}Horse Master", DefaultSkills.Bow, this.GetTierCost(9), this._bowDeepQuivers, "{=LiaRnWJZ}You can now use all bows on horseback.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Invalid, "{=0J9dgA7j}{VALUE} bow skill to horse archers in your formation", SkillEffect.PerkRole.Captain, 30f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.BowUser);
			this._bowDeepQuivers.Initialize("{=h83ZU95t}Deep Quivers", DefaultSkills.Bow, this.GetTierCost(9), this._bowHorseMaster, "{=YOQQR1bJ}{VALUE} extra arrows per quiver.", SkillEffect.PerkRole.Personal, 3f, SkillEffect.EffectIncrementType.Add, "{=CBVfPRRe}{VALUE} extra arrow per quiver to troops in your party.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowQuickDraw.Initialize("{=vnJndBgT}Quick Draw", DefaultSkills.Bow, this.GetTierCost(10), this._bowRangersSwiftness, "{=jU084S5S}{VALUE}% aiming speed with bows.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=tsh4RXNa}{VALUE}% tax gain in the governed settlement.", SkillEffect.PerkRole.Governor, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowRangersSwiftness.Initialize("{=p12tSfCC}Ranger's Swiftness", DefaultSkills.Bow, this.GetTierCost(10), this._bowQuickDraw, "{=RQd005zy}Equipped bows do not slow you down.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Invalid, "{=6XDcZUsb}{VALUE}% security provided by archers in the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._bowDeadshot.Initialize("{=rsKhbZpJ}Deadshot", DefaultSkills.Bow, this.GetTierCost(11), null, "{=HCqd0IOt}{VALUE}% reload speed with bows for every skill point above 200.", SkillEffect.PerkRole.Personal, 0.002f, SkillEffect.EffectIncrementType.AddFactor, "{=hiFadSiC}{VALUE}% damage with bows for every skill point above 200.", SkillEffect.PerkRole.Personal, 0.005f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowPiercer.Initialize("{=v8RwzwqD}Piercer", DefaultSkills.Crossbow, this.GetTierCost(1), this._crossbowMarksmen, "{=j3J0Hbax}Your crossbow attacks ignore armors below 20.", SkillEffect.PerkRole.Personal, 20f, SkillEffect.EffectIncrementType.Add, "{=CLBXxPdh}{VALUE}% recruitment cost of ranged troops.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowMarksmen.Initialize("{=IUGVdu64}Marksmen", DefaultSkills.Crossbow, this.GetTierCost(1), this._crossbowPiercer, "{=LCsu8rXa}{VALUE}% faster aiming with crossbows.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=kmdxvkEV}{VALUE}% starting battle morale to ranged troops in your party.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowUnhorser.Initialize("{=75vJc53f}Unhorser", DefaultSkills.Crossbow, this.GetTierCost(2), this._crossbowWindWinder, "{=97nYJcKO}{VALUE}% crossbow damage to mounts.", SkillEffect.PerkRole.Personal, 0.4f, SkillEffect.EffectIncrementType.AddFactor, "{=laWxqjBP}{VALUE}% damage against mounts to crossbow troops in your formation.", SkillEffect.PerkRole.Captain, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.CrossbowUser);
			this._crossbowWindWinder.Initialize("{=1bw2uw8H}Wind Winder", DefaultSkills.Crossbow, this.GetTierCost(2), this._crossbowUnhorser, "{=3cBX5x0x}{VALUE}% reload speed with crossbows.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=YHjdf1uO}{VALUE}% crossbow reload speed to troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.CrossbowUser);
			this._crossbowDonkeysSwiftness.Initialize("{=uANbQUxg}Donkey's Swiftness", DefaultSkills.Crossbow, this.GetTierCost(3), this._crossbowSheriff, "{=Af7zOV2l}{VALUE}% accuracy loss while moving.", SkillEffect.PerkRole.Personal, -0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=aIyRxlCf}{VALUE} crossbow skill to troops in your formation.", SkillEffect.PerkRole.Captain, 30f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.CrossbowUser);
			this._crossbowSheriff.Initialize("{=leaowE4D}Sheriff", DefaultSkills.Crossbow, this.GetTierCost(3), this._crossbowDonkeysSwiftness, "{=W7PaF7Lr}{VALUE}% headshot damage with crossbows.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=HB2wwuj6}{VALUE}% crossbow damage to infantry by troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._crossbowPeasantLeader.Initialize("{=2rPMYl7Y}Peasant Leader", DefaultSkills.Crossbow, this.GetTierCost(4), this._crossbowRenownMarksmen, "{=4CSaYB8H}{VALUE}% battle morale to tier 1 to 3 troops", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=xuUbaa9f}{VALUE}% garrisoned ranged troop wages in the governed settlement.", SkillEffect.PerkRole.Governor, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowRenownMarksmen.Initialize("{=ICkvftaM}Renowned Marksmen", DefaultSkills.Crossbow, this.GetTierCost(4), this._crossbowPeasantLeader, "{=uj52xbdr}{VALUE} daily experience to ranged troops in your party.", SkillEffect.PerkRole.PartyLeader, 2f, SkillEffect.EffectIncrementType.Add, "{=i4GboakR}{VALUE}% security provided by ranged troops in the garrison of the governed settlement.", SkillEffect.PerkRole.Governor, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowFletcher.Initialize("{=FA5QlTvm}Fletcher", DefaultSkills.Crossbow, this.GetTierCost(5), this._crossbowPuncture, "{=wvQbeHpp}{VALUE} bolts per quiver.", SkillEffect.PerkRole.Personal, 4f, SkillEffect.EffectIncrementType.Add, "{=j3Hcp9RQ}{VALUE} bolts per quiver to troops in your party.", SkillEffect.PerkRole.PartyLeader, 2f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowPuncture.Initialize("{=jjkJyKoy}Puncture", DefaultSkills.Crossbow, this.GetTierCost(5), this._crossbowFletcher, "{=bVUQyN8t}{VALUE}% armor penetration with crossbows.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=KhCU9FZU}{VALUE}% armor penetration with crossbows by troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.CrossbowUser);
			this._crossbowLooseAndMove.Initialize("{=SKUPHeAw}Loose and Move", DefaultSkills.Crossbow, this.GetTierCost(6), this._crossbowDeftHands, "{=BbaidhT4}Equipped crossbows do not slow you down.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "{=loVfqss6}{VALUE}% movement speed to ranged troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Ranged);
			this._crossbowDeftHands.Initialize("{=NYHeygaj}Deft Hands", DefaultSkills.Crossbow, this.GetTierCost(6), this._crossbowLooseAndMove, "{=VY7WQSgu}{VALUE}% resistance to getting staggered while reloading your crossbow.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=wUov3khT}{VALUE}% resistance to getting staggered while reloading crossbows to troops in your formation.", SkillEffect.PerkRole.Captain, 0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.CrossbowUser);
			this._crossbowCounterFire.Initialize("{=grgnisK4}Counter Fire", DefaultSkills.Crossbow, this.GetTierCost(7), this._crossbowMountedCrossbowman, "{=8ieLwTyG}{VALUE}% projectile damage taken while equipped with a crossbow.", SkillEffect.PerkRole.Personal, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=zJHhHRBw}{VALUE}% damage taken from projectiles by your troops.", SkillEffect.PerkRole.Captain, -0.03f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._crossbowMountedCrossbowman.Initialize("{=UZHvbYTr}Mounted Crossbowman", DefaultSkills.Crossbow, this.GetTierCost(7), this._crossbowCounterFire, "{=ZTt5Es7q}You can reload any crossbow on horseback.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "{=i36Gg6mW}{VALUE}% experience gained to ranged troops in your party.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowSteady.Initialize("{=Ye9lbBr3}Steady", DefaultSkills.Crossbow, this.GetTierCost(8), this._crossbowLongShots, "{=wFWdhNCN}{VALUE}% accuracy penalty with crossbows while mounted.", SkillEffect.PerkRole.Personal, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=q5IMLou4}{VALUE}% tariff gain in the governed settlement.", SkillEffect.PerkRole.Governor, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowLongShots.Initialize("{=Y5KXWHJY}Long Shots", DefaultSkills.Crossbow, this.GetTierCost(8), this._crossbowSteady, "{=Stykk4VR}{VALUE}% more zoom with crossbows.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.AddFactor, "{=xwA6Om0Y}{VALUE} daily militia recruitment in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowHammerBolts.Initialize("{=FjMS9Mbz}Hammer Bolts", DefaultSkills.Crossbow, this.GetTierCost(9), this._crossbowPavise, "{=K8VhWN85}{VALUE}% chance of dismounting enemy cavalry with a heavy hit from your crossbow.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=yz8xogMc}{VALUE}% damage with crossbows by troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.CrossbowUser);
			this._crossbowPavise.Initialize("{=2667CwaK}Pavise", DefaultSkills.Crossbow, this.GetTierCost(9), this._crossbowHammerBolts, "{=pr5vaFNc}{VALUE}% chance of blocking projectiles from behind with a shield on your back.", SkillEffect.PerkRole.Personal, 0.75f, SkillEffect.EffectIncrementType.AddFactor, "{=Q8LSfvIO}{VALUE}% accuracy to ballistas in the governed settlement.", SkillEffect.PerkRole.Governor, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowTerror.Initialize("{=nAlCj2m0}Terror", DefaultSkills.Crossbow, this.GetTierCost(10), this._crossbowPickedShots, "{=NGFbn4Qx}{VALUE}% chance of increasing the siege bombardment casualties per hit by 1.", SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=sUFw8cDt}{VALUE}% morale loss to enemy due to crossbow kills by troops in your formation.", SkillEffect.PerkRole.Captain, 0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.CrossbowUser);
			this._crossbowPickedShots.Initialize("{=nGWmyZCs}Picked Shots", DefaultSkills.Crossbow, this.GetTierCost(10), this._crossbowTerror, "{=RAmqFXLm}{VALUE}% wages of tier 4+ troops.", SkillEffect.PerkRole.PartyLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=Yxchzh3a}{VALUE} hit points to ranged troops in your party.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._crossbowMightyPull.Initialize("{=ZFtyxzT5}Mighty Pull", DefaultSkills.Crossbow, this.GetTierCost(11), null, "{=Jtx8Czql}{VALUE}% reload speed with crossbows for every skill point above 200.", SkillEffect.PerkRole.Personal, 0.002f, SkillEffect.EffectIncrementType.AddFactor, "{=WUaSub02}{VALUE}% damage with crossbows for every skill point above 200.", SkillEffect.PerkRole.Personal, 0.005f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._throwingQuickDraw.Initialize("{=vnJndBgT}Quick Draw", DefaultSkills.Throwing, this.GetTierCost(1), this._throwingShieldBreaker, "{=Fnbf4ShY}{VALUE}% draw speed with throwing weapons.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=UkADS8nQ}{VALUE}% draw speed with throwing weapons to troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.ThrownUser);
			this._throwingShieldBreaker.Initialize("{=DeWp2GjP}Shield Breaker", DefaultSkills.Throwing, this.GetTierCost(1), this._throwingQuickDraw, "{=wPwbyBra}{VALUE}% damage to shields with throwing weapons.", SkillEffect.PerkRole.Personal, 0.4f, SkillEffect.EffectIncrementType.AddFactor, "{=inFSdSiC}{VALUE}% damage to shields with throwing weapons by troops in your formation.", SkillEffect.PerkRole.Captain, 0.08f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.ThrownUser);
			this._throwingHunter.Initialize("{=xnDWqYKW}Hunter", DefaultSkills.Throwing, this.GetTierCost(2), this._throwingFlexibleFighter, "{=FPdjh976}{VALUE}% damage to mounts with throwing weapons.", SkillEffect.PerkRole.Personal, 0.4f, SkillEffect.EffectIncrementType.AddFactor, "{=ZgvRAR0u}{VALUE}% damage to mounts with throwing weapons by troops in your formation.", SkillEffect.PerkRole.Captain, 0.08f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.ThrownUser);
			this._throwingFlexibleFighter.Initialize("{=mPPWRjCZ}Flexible Fighter", DefaultSkills.Throwing, this.GetTierCost(2), this._throwingHunter, "{=6rEsV6SZ}{VALUE}% damage while using throwing weapons as melee.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=SSm1kkaB}{VALUE} Control skills of infantry, {VALUE} Vigor skills of archers in your formation.", SkillEffect.PerkRole.Captain, 15f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._throwingMountedSkirmisher.Initialize("{=l1I748Fn}Mounted Skirmisher", DefaultSkills.Throwing, this.GetTierCost(3), this._throwingWellPrepared, "{=JsdkJbDL}{VALUE}% accuracy penalty with throwing weapons while mounted.", SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=0L96iq1b}{VALUE}% damage with throwing weapons by mounted troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.ThrownUser);
			this._throwingWellPrepared.Initialize("{=bloxcikL}Well Prepared", DefaultSkills.Throwing, this.GetTierCost(3), this._throwingMountedSkirmisher, "{=nKw4eb22}{VALUE} ammunition for throwing weapons.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "{=1lEckrPh}{VALUE} ammunition for throwing weapons to troops in your party.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._throwingRunningThrow.Initialize("{=OcaW12fJ}Running Throw", DefaultSkills.Throwing, this.GetTierCost(4), this._throwingKnockOff, "{=Z4maWcyl}{VALUE}% damage bonus from speed with throwing weapons.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.Add, "{=a5CWbHsd}{VALUE} throwing skill to troops in your formation.", SkillEffect.PerkRole.Captain, 30f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.ThrownUser);
			this._throwingKnockOff.Initialize("{=Gn3KBN8L}Knock Off", DefaultSkills.Throwing, this.GetTierCost(4), this._throwingRunningThrow, "{=JqGLpG2L}{VALUE}% chance of dismounting enemy cavalry with a heavy hit with your throwing weapons.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=cJEbenVQ}{VALUE}% throwing weapon damage to cavalry by troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.ThrownUser);
			this._throwingSkirmisher.Initialize("{=s9oED1IR}Skirmisher", DefaultSkills.Throwing, this.GetTierCost(5), this._throwingSaddlebags, "{=O6UPQskm}{VALUE}% damage taken by ranged attacks while holding a throwing weapon.", SkillEffect.PerkRole.Personal, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=ZUYOXMFo}{VALUE}% damage taken by ranged attacks to troops in your formation.", SkillEffect.PerkRole.Captain, -0.03f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._throwingSaddlebags.Initialize("{=VUxFbEiW}Saddlebags", DefaultSkills.Throwing, this.GetTierCost(5), this._throwingSkirmisher, "{=bFNFpd2d}{VALUE} ammunition for throwing weapons when you start a battle mounted.", SkillEffect.PerkRole.Personal, 2f, SkillEffect.EffectIncrementType.Add, "{=0jbhAPub}{VALUE} daily experience to infantry troops in your party.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._throwingFocus.Initialize("{=throwingskillfocus}Focus", DefaultSkills.Throwing, this.GetTierCost(6), this._throwingLastHit, "{=hJdHb0G7}{VALUE}% zoom with throwing weapons.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=buFin46y}{VALUE} daily security in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._throwingLastHit.Initialize("{=IsHyjvSq}Last Hit", DefaultSkills.Throwing, this.GetTierCost(6), this._throwingFocus, "{=PleZrXuO}{VALUE}% damage to enemies with less than half of their hit points left.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=g5nnybjz}{VALUE} starting battle morale to troops in your party.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._throwingHeadHunter.Initialize("{=iARYMyuq}Head Hunter", DefaultSkills.Throwing, this.GetTierCost(7), this._throwingThrowingCompetitions, "{=UsD0y74h}{VALUE}% headshot damage with thrown weapons.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=j7inOz04}{VALUE}% recruitment cost of tier 2+ troops.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._throwingThrowingCompetitions.Initialize("{=cC8iTtg5}Throwing Competitions", DefaultSkills.Throwing, this.GetTierCost(7), this._throwingHeadHunter, "{=W0cfTJQv}{VALUE}% upgrade cost of infantry troops.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=g9gTYB8u}{VALUE} militia recruitment in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._throwingResourceful.Initialize("{=w53LSPJ1}Resourceful", DefaultSkills.Throwing, this.GetTierCost(8), this._throwingSplinters, "{=nKw4eb22}{VALUE} ammunition for throwing weapons.", SkillEffect.PerkRole.Personal, 2f, SkillEffect.EffectIncrementType.Add, "{=P0iCmQAf}{VALUE}% experience from battles to troops in your party equipped with throwing weapons.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._throwingSplinters.Initialize("{=b6W74uyR}Splinters", DefaultSkills.Throwing, this.GetTierCost(8), this._throwingResourceful, "{=ymKzbcfB}Triple damage against shields with throwing axes.", SkillEffect.PerkRole.Personal, 3f, SkillEffect.EffectIncrementType.AddFactor, "{=inFSdSiC}{VALUE}% damage to shields with throwing weapons by troops in your formation.", SkillEffect.PerkRole.Captain, 0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.ThrownUser);
			this._throwingPerfectTechnique.Initialize("{=BCoQgZvG}Perfect Technique", DefaultSkills.Throwing, this.GetTierCost(9), this._throwingLongReach, "{=cr1AipGT}{VALUE}% travel speed to your throwing weapons.", SkillEffect.PerkRole.Personal, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=rkHnKmSK}{VALUE}% travel speed to throwing weapons of troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.ThrownUser);
			this._throwingLongReach.Initialize("{=9iLyu1kp}Long Reach", DefaultSkills.Throwing, this.GetTierCost(9), this._throwingPerfectTechnique, "{=lEi1hIIt}You can pick up items from the ground while mounted.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.AddFactor, "{=VgkFpMxF}{VALUE}% morale and renown gained from battles won.", SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._throwingWeakSpot.Initialize("{=cPPLAz8l}Weak Spot", DefaultSkills.Throwing, this.GetTierCost(10), this._throwingImpale, "{=z4zrwc9K}{VALUE}% armor penetration with throwing weapons.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=b97khG1u}{VALUE}% armor penetration with throwing weapons by troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.ThrownUser);
			this._throwingImpale.Initialize("{=tYAYIRjr}Impale", DefaultSkills.Throwing, this.GetTierCost(10), this._throwingWeakSpot, "{=D9coiXFt}Javelins you throw can penetrate shields.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.AddFactor, "{=xlddWniu}{VALUE}% damage with throwing weapons by troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.ThrownUser);
			this._throwingUnstoppableForce.Initialize("{=Jat5GFDi}Unstoppable Force", DefaultSkills.Throwing, this.GetTierCost(11), null, "{=4MPzgKqE}{VALUE}% travel speed to your throwing weapons for every skill point above 200.", SkillEffect.PerkRole.Personal, 0.002f, SkillEffect.EffectIncrementType.AddFactor, "{=pDvv90Th}{VALUE}% damage with throwing weapons for every skill point above 200.", SkillEffect.PerkRole.Personal, 0.005f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingFullSpeed.Initialize("{=kzy9Iduz}Full Speed", DefaultSkills.Riding, this.GetTierCost(1), this._ridingNimbleSteed, "{=wKSA8Qob}{VALUE}% charge damage dealt.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=DS8fM8Op}{VALUE}% charge damage dealt by troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted);
			this._ridingNimbleSteed.Initialize("{=cXlnH1Jp}Nimble Steed", DefaultSkills.Riding, this.GetTierCost(1), this._ridingFullSpeed, "{=f8R0Hkxa}{VALUE}% maneuvering.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=zjctOvv8}{VALUE} riding skill to troops in your formation.", SkillEffect.PerkRole.Captain, 30f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted);
			this._ridingWellStraped.Initialize("{=3lfS4iCZ}Well Strapped", DefaultSkills.Riding, this.GetTierCost(2), this._ridingVeterinary, "{=oKWft2IH}{VALUE}% chance of your mount dying or becoming lame after it falls in battle.", SkillEffect.PerkRole.Personal, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=IkhQPr3Z}{VALUE} daily loyalty to the governed settlement.", SkillEffect.PerkRole.Governor, 0.5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingVeterinary.Initialize("{=ZaSmz64G}Veterinary", DefaultSkills.Riding, this.GetTierCost(2), this._ridingWellStraped, "{=tvRYz5lr}{VALUE}% hit points to your mount.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=b0w3Fruf}{VALUE}% hit points to mounts of troops in your party.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingNomadicTraditions.Initialize("{=PB5iowxh}Nomadic Traditions", DefaultSkills.Riding, this.GetTierCost(3), this._ridingDeeperSacks, "{=Wrmqdoz8}{VALUE}% party speed bonus from footmen on horses.", SkillEffect.PerkRole.PartyLeader, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=fPB1WdEy}{VALUE}% melee damage bonus from speed to mounted troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.Melee);
			this._ridingDeeperSacks.Initialize("{=VWYrJCje}Deeper Sacks", DefaultSkills.Riding, this.GetTierCost(3), this._ridingNomadicTraditions, "{=Yp4zv2ib}{VALUE}% carrying capacity for pack animals in your party.", SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=UC6JdOXk}{VALUE}% trade penalty for mounts.", SkillEffect.PerkRole.PartyLeader, -0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingSagittarius.Initialize("{=jbPZTSP4}Sagittarius", DefaultSkills.Riding, this.GetTierCost(4), this._ridingSweepingWind, "{=nc3carw2}{VALUE}% accuracy penalty while mounted.", SkillEffect.PerkRole.Personal, -0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=r0epmJJJ}{VALUE}% accuracy penalty to mounted troops in your formation.", SkillEffect.PerkRole.Captain, -0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.Ranged);
			this._ridingSweepingWind.Initialize("{=gL7Ltjpc}Sweeping Wind", DefaultSkills.Riding, this.GetTierCost(4), this._ridingSagittarius, "{=lTafHBwZ}{VALUE}% top speed to your mount.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=Q74nUiFJ}{VALUE}% party speed.", SkillEffect.PerkRole.PartyLeader, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingReliefForce.Initialize("{=g5I4qyjw}Relief Force", DefaultSkills.Riding, this.GetTierCost(5), null, "{=tx37EgiO}{VALUE} starting battle morale when you join an ongoing battle of your allies.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, "{=RVNPXS46}{VALUE}% security provided by mounted troops in the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingMountedWarrior.Initialize("{=ixqTFMVA}Mounted Warrior", DefaultSkills.Riding, this.GetTierCost(6), this._ridingHorseArcher, "{=1GwI0hcG}{VALUE}% mounted melee damage.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=f6sgEuS0}{VALUE}% mounted melee damage by troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.Melee);
			this._ridingHorseArcher.Initialize("{=ugJfuabA}Horse Archer", DefaultSkills.Riding, this.GetTierCost(6), this._ridingMountedWarrior, "{=G4xCqSNG}{VALUE}% ranged damage while mounted.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=DFMsbFrB}{VALUE}% damage by mounted archers in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.BowUser);
			this._ridingShepherd.Initialize("{=I5LyCJzj}Shepherd", DefaultSkills.Riding, this.GetTierCost(7), this._ridingBreeder, "{=aiIPozp6}{VALUE}% herding speed penalty.", SkillEffect.PerkRole.PartyLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=YhZj58ut}{VALUE}% chance of producing tier 2 horses in villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingBreeder.Initialize("{=4Pbfs4rV}Breeder", DefaultSkills.Riding, this.GetTierCost(7), this._ridingShepherd, "{=Cpaw42pv}{VALUE}% daily chance of animals in your party reproducing.", SkillEffect.PerkRole.PartyLeader, 0.01f, SkillEffect.EffectIncrementType.AddFactor, "{=665JbYIC}{VALUE}% production rate to villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingThunderousCharge.Initialize("{=3MLtqFPt}Thunderous Charge", DefaultSkills.Riding, this.GetTierCost(8), this._ridingAnnoyingBuzz, "{=qvjCYY61}{VALUE}% battle morale penalty to enemies with mounted melee kills.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=fK9GFdM8}{VALUE}% battle morale penalty to enemies with mounted melee kills by troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.Melee);
			this._ridingAnnoyingBuzz.Initialize("{=Okibjv5n}Annoying Buzz", DefaultSkills.Riding, this.GetTierCost(8), this._ridingThunderousCharge, "{=nbbQfbli}{VALUE}% battle morale penalty to enemies with mounted ranged kills.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=wtdwqO8i}{VALUE}% battle morale penalty to enemies with mounted ranged kills by troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted | TroopUsageFlags.Ranged);
			this._ridingMountedPatrols.Initialize("{=1z3oRPQu}Mounted Patrols", DefaultSkills.Riding, this.GetTierCost(9), this._ridingCavalryTactics, "{=pAkHwm3k}{VALUE}% escape chance to prisoners in your party.", SkillEffect.PerkRole.PartyLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=mNbAR4uk}{VALUE}% escape chance to prisoners in the governed settlement.", SkillEffect.PerkRole.Governor, -0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingCavalryTactics.Initialize("{=ZMxAGDKU}Cavalry Tactics", DefaultSkills.Riding, this.GetTierCost(9), this._ridingMountedPatrols, "{=oboqflX9}{VALUE}% volunteering rate of cavalry troops in the settlements governed by your clan.", SkillEffect.PerkRole.ClanLeader, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=mXGozqqL}{VALUE}% wages of mounted troops in the governed settlement.", SkillEffect.PerkRole.Governor, -0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._ridingDauntlessSteed.Initialize("{=eYzTvFEH}Dauntless Steed", DefaultSkills.Riding, this.GetTierCost(10), this._ridingToughSteed, "{=7uhottjU}{VALUE}% resistance to getting staggered while mounted.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=MEr2aoeC}{VALUE} armor to all equipped armor pieces of mounted troops in your formation.", SkillEffect.PerkRole.Captain, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted);
			this._ridingToughSteed.Initialize("{=vDNbHDfq}Tough Steed", DefaultSkills.Riding, this.GetTierCost(10), this._ridingDauntlessSteed, "{=svkQsokb}{VALUE}% armor to your mount.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=Ful5cXFa}{VALUE} armor to mounts of troops in your formation.", SkillEffect.PerkRole.Captain, 10f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Mounted);
			this._ridingTheWayOfTheSaddle.Initialize("{=HvYgMtXO}The Way Of The Saddle", DefaultSkills.Riding, this.GetTierCost(11), null, "{=nXZktHa6}{VALUE} charge damage and maneuvering for every skill point above 250.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsMorningExercise.Initialize("{=ipwU1JT3}Morning Exercise", DefaultSkills.Athletics, this.GetTierCost(1), this._athleticsWellBuilt, "{=V53EYEXx}{VALUE}% combat movement speed.", SkillEffect.PerkRole.Personal, 0.03f, SkillEffect.EffectIncrementType.AddFactor, "{=nRvR1Rpc}{VALUE}% combat movement speed to troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._athleticsWellBuilt.Initialize("{=bigS7KHi}Well Built", DefaultSkills.Athletics, this.GetTierCost(1), this._athleticsMorningExercise, "{=Ti9auMiO}{VALUE} hit points.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, "{=V4zyUiai}{VALUE} hit points to foot troops in your party.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsFury.Initialize("{=b0ak3yGV}Fury", DefaultSkills.Athletics, this.GetTierCost(2), this._athleticsFormFittingArmor, "{=Epwmv89M}{VALUE}% weapon handling while on foot.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=LGFsDic7}{VALUE}% weapon handling to foot troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._athleticsFormFittingArmor.Initialize("{=tp3p7R8E}Form Fitting Armor", DefaultSkills.Athletics, this.GetTierCost(2), this._athleticsFury, "{=86R9Ttgx}{VALUE}% armor weight.", SkillEffect.PerkRole.Personal, -0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=WpCx75Pc}{VALUE}% combat movement speed to tier 3+ foot troops in your formation.", SkillEffect.PerkRole.Captain, 0.04f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._athleticsImposingStature.Initialize("{=3hffzsoK}Imposing Stature", DefaultSkills.Athletics, this.GetTierCost(3), this._athleticsStamina, "{=qCaIau4o}{VALUE}% persuasion chance.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=NouDSrXE}{VALUE} party size.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsStamina.Initialize("{=2lCLp5eo}Stamina", DefaultSkills.Athletics, this.GetTierCost(3), this._athleticsImposingStature, "{=Lrm17UNm}{VALUE}% crafting stamina recovery rate.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=PNB9bHJd}{VALUE} prisoner limit and -10% escape chance to your prisoners.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsSprint.Initialize("{=864bKdc6}Sprint", DefaultSkills.Athletics, this.GetTierCost(4), this._athleticsPowerful, "{=mWezTaa1}{VALUE}% combat movement speed when you have no shields and no ranged weapons equipped.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=zoNKoZDZ}{VALUE}% combat movement speed to infantry troops in your formation.", SkillEffect.PerkRole.Captain, 0.03f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._athleticsPowerful.Initialize("{=UCpyo9hw}Powerful", DefaultSkills.Athletics, this.GetTierCost(4), this._athleticsSprint, "{=CglYgfiY}{VALUE}% damage with melee weapons.", SkillEffect.PerkRole.Personal, 0.04f, SkillEffect.EffectIncrementType.AddFactor, "{=eBmaa49a}{VALUE}% melee damage by troops in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Melee);
			this._athleticsSurgingBlow.Initialize("{=zrYFYDfj}Surging Blow", DefaultSkills.Athletics, this.GetTierCost(5), this._athleticsBraced, "{=QiZfTNWJ}{VALUE}% damage bonus from speed while on foot.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=m6RcG1bD}{VALUE}% damage bonus from speed to troops in your formation.", SkillEffect.PerkRole.Captain, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._athleticsBraced.Initialize("{=dU7haWkI}Braced", DefaultSkills.Athletics, this.GetTierCost(5), this._athleticsSurgingBlow, "{=QqVLsf0N}{VALUE}% charge damage taken.", SkillEffect.PerkRole.Personal, -0.4f, SkillEffect.EffectIncrementType.AddFactor, "{=Dilnx8Es}{VALUE}% charge damage taken by troops in your formation.", SkillEffect.PerkRole.Captain, -0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._athleticsWalkItOff.Initialize("{=0pyLfrGZ}Walk It Off", DefaultSkills.Athletics, this.GetTierCost(6), this._athleticsAGoodDaysRest, "{=65b6daHW}{VALUE}% hit point regeneration while traveling.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=9Hv0q2lg}{VALUE} daily experience to foot troops while traveling.", SkillEffect.PerkRole.PartyLeader, 3f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsAGoodDaysRest.Initialize("{=B7HwvV6L}A Good Days Rest", DefaultSkills.Athletics, this.GetTierCost(6), this._athleticsWalkItOff, "{=cCXt1jce}{VALUE}% hit point regeneration while waiting in settlements.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=fyibGRUQ}{VALUE} daily experience to foot troops while waiting in settlements.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsDurable.Initialize("{=8AKmJwv7}Durable", DefaultSkills.Athletics, this.GetTierCost(7), this._athleticsEnergetic, "{=4uqDestM}{VALUE} Endurance attribute.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "{=m993aVvX}{VALUE} daily loyalty in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsEnergetic.Initialize("{=1YxFYg3s}Energetic", DefaultSkills.Athletics, this.GetTierCost(7), this._athleticsDurable, "{=qPpN2wW8}{VALUE}% overburdened speed penalty.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=ULY7byYc}{VALUE}% hearth growth in villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsSteady.Initialize("{=Ye9lbBr3}Steady", DefaultSkills.Athletics, this.GetTierCost(8), this._athleticsStrong, "{=C8LhGtUJ}{VALUE} Control attribute.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "{=rkQptw1O}{VALUE}% production in farms, mines, lumber camps and clay pits bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsStrong.Initialize("{=d5aK6Sv0}Strong", DefaultSkills.Athletics, this.GetTierCost(8), this._athleticsSteady, "{=gtlygHIk}{VALUE} Vigor attribute.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "{=yXaozMwY}{VALUE}% party speed by foot troops in your party.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsStrongLegs.Initialize("{=guZWnzaV}Strong Legs", DefaultSkills.Athletics, this.GetTierCost(9), this._athleticsStrongArms, "{=QIDr1cTd}{VALUE}% fall damage taken and +100% kick damage dealt.", SkillEffect.PerkRole.Personal, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=O3sh2iws}{VALUE}% food consumption in the governed settlement while under siege.", SkillEffect.PerkRole.Governor, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsStrongArms.Initialize("{=qBKmIyYx}Strong Arms", DefaultSkills.Athletics, this.GetTierCost(9), this._athleticsStrongLegs, "{=Ztezot02}{VALUE}% damage with throwing weapons.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=a5CWbHsd}{VALUE} throwing skill to troops in your formation.", SkillEffect.PerkRole.Captain, 20f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.ThrownUser);
			this._athleticsSpartan.Initialize("{=PX0Xufmr}Spartan", DefaultSkills.Athletics, this.GetTierCost(10), this._athleticsIgnorePain, "{=NmGcIg3j}{VALUE}% resistance to getting staggered while on foot.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=6NHvsrrx}{VALUE}% food consumption in your party.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._athleticsIgnorePain.Initialize("{=AHtFRv5T}Ignore Pain", DefaultSkills.Athletics, this.GetTierCost(10), this._athleticsSpartan, "{=1be7OEQB}{VALUE}% armor while on foot.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=F2H2lZJ4}{VALUE} armor to all equipped armor pieces of foot troops in your formation.", SkillEffect.PerkRole.Captain, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.OnFoot);
			this._athleticsMightyBlow.Initialize("{=lbGa4ihC}Mighty Blow ", DefaultSkills.Athletics, this.GetTierCost(11), null, "{=cqUXbafi}You stun your enemies longer after they block your attack.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=LItNgwiF}{VALUE} hit points for every skill point above 250.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingIronMaker.Initialize("{=i3eT3Zjb}Efficient Iron Maker", DefaultSkills.Crafting, this.GetTierCost(1), this._craftingCharcoalMaker, "{=6btajdpT}You can produce crude iron more efficiently by obtaining three units of crude iron from one unit of iron ore.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingCharcoalMaker.Initialize("{=u5zNNZKa}Efficient Charcoal Maker", DefaultSkills.Crafting, this.GetTierCost(1), this._craftingIronMaker, "{=wbwoVfSq}You can use a more efficient method of charcoal production that produces three units of charcoal from two units of hardwood.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingSteelMaker.Initialize("{=pKquYFTX}Steel Maker", DefaultSkills.Crafting, this.GetTierCost(2), this._craftingCuriousSmelter, "{=qZpIdBib}You can refine two units of iron into one unit of steel, and one unit of crude iron as by-product.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingCuriousSmelter.Initialize("{=Tu1Sd2qg}Curious Smelter", DefaultSkills.Crafting, this.GetTierCost(2), this._craftingSteelMaker, "{=1dS5OjLQ}{VALUE}% learning rate of new part designs when smelting.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingSteelMaker2.Initialize("{=EerNV0aM}Steel Maker 2", DefaultSkills.Crafting, this.GetTierCost(3), this._craftingCuriousSmith, "{=mm5ZzOOV}You can refine two units of steel into one unit of fine steel, and one unit of crude iron as by-product.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingCuriousSmith.Initialize("{=J1GSW0yk}Curious Smith", DefaultSkills.Crafting, this.GetTierCost(3), this._craftingSteelMaker2, "{=vWt9bvOz}{VALUE}% learning rate of new part designs when smithing.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingExperiencedSmith.Initialize("{=dwtIc9AG}Experienced Smith", DefaultSkills.Crafting, this.GetTierCost(4), this._craftingSteelMaker3, "{=w1K8XDls}{VALUE}% greater chance of creating Fine weapons.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=qPJJnxM1}Successful crafting orders of notables increase your relation by {VALUE} with them.", SkillEffect.PerkRole.Personal, 2f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingSteelMaker3.Initialize("{=c5GOJIhU}Steel Maker 3", DefaultSkills.Crafting, this.GetTierCost(4), this._craftingExperiencedSmith, "{=fxGdAlI2}You can refine two units of fine steel into one unit of Thamaskene steel,{newline}and one unit of crude iron as by-product.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "{=3b4sjuMu}{VALUE} relationships with lords and ladies for successful crafting orders.", SkillEffect.PerkRole.Personal, 4f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingPracticalRefiner.Initialize("{=OrcSQyOb}Practical Refiner", DefaultSkills.Crafting, this.GetTierCost(5), this._craftingPracticalSmelter, "{=hmrUcvwz}{VALUE}% stamina spent while refining.", SkillEffect.PerkRole.Personal, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingPracticalSmelter.Initialize("{=KFpnAWwr}Practical Smelter", DefaultSkills.Crafting, this.GetTierCost(5), this._craftingPracticalRefiner, "{=NzlwbSIj}{VALUE}% stamina spent while smelting.", SkillEffect.PerkRole.Personal, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingVigorousSmith.Initialize("{=8hhS659w}Vigorous Smith", DefaultSkills.Crafting, this.GetTierCost(6), this._craftingStrongSmith, "{=gtlygHIk}{VALUE} Vigor attribute.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingStrongSmith.Initialize("{=83iwPPVH}Controlled Smith", DefaultSkills.Crafting, this.GetTierCost(6), this._craftingVigorousSmith, "{=C8LhGtUJ}{VALUE} Control attribute.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingPracticalSmith.Initialize("{=rR8iTDPI}Practical Smith", DefaultSkills.Crafting, this.GetTierCost(7), this._craftingArtisanSmith, "{=FqmS9wcP}{VALUE}% stamina spent while smithing.", SkillEffect.PerkRole.Personal, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingArtisanSmith.Initialize("{=bnVCX24q}Artisan Smith", DefaultSkills.Crafting, this.GetTierCost(7), this._craftingPracticalSmith, "{=W9pOfMAE}{VALUE}% trade penalty when selling smithing weapons.", SkillEffect.PerkRole.PartyLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingMasterSmith.Initialize("{=ivH8RWyb}Master Smith", DefaultSkills.Crafting, this.GetTierCost(8), null, "{=*}{VALUE}% greater chance of creating masterwork weapons.", SkillEffect.PerkRole.Personal, 0.075f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingFencerSmith.Initialize("{=SSdYsV4R}Fencer Smith", DefaultSkills.Crafting, this.GetTierCost(9), this._craftingEnduringSmith, "{=j3QNVqP5}{VALUE} Focus Point to One Handed and Two Handed.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingEnduringSmith.Initialize("{=RWMACSag}Enduring Smith", DefaultSkills.Crafting, this.GetTierCost(9), this._craftingFencerSmith, "{=4uqDestM}{VALUE} Endurance attribute.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingSharpenedEdge.Initialize("{=knWgaYdk}Sharpened Edge", DefaultSkills.Crafting, this.GetTierCost(10), this._craftingSharpenedTip, "{=S7BMf2Wa}{VALUE}% swing damage of crafted weapons.", SkillEffect.PerkRole.Personal, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingSharpenedTip.Initialize("{=aO2JSbSq}Sharpened Tip", DefaultSkills.Crafting, this.GetTierCost(10), this._craftingSharpenedEdge, "{=KabSHyf0}{VALUE}% thrust damage of crafted weapons.", SkillEffect.PerkRole.Personal, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._craftingLegendarySmith.Initialize("{=f4lnEplc}Legendary Smith", DefaultSkills.Crafting, this.GetTierCost(11), null, "{=*}{VALUE}% greater chance of creating Legendary weapons, chance increases by 1% for every 5 skill points above 275.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingDayTraveler.Initialize("{=6PSgX2BP}Day Traveler", DefaultSkills.Scouting, this.GetTierCost(1), this._scoutingNightRunner, "{=86nHAJs9}{VALUE}% travel speed during daytime.", SkillEffect.PerkRole.Scout, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "{=RUEfCZBG}{VALUE}% sight range during daytime in campaign map.", SkillEffect.PerkRole.Scout, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingNightRunner.Initialize("{=B8Gq2ylh}Night Runner", DefaultSkills.Scouting, this.GetTierCost(1), this._scoutingDayTraveler, "{=QmaIRD7P}{VALUE}% travel speed during nighttime", SkillEffect.PerkRole.Scout, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=6CteaPKm}{VALUE}% sight range during nighttime in campaign map.", SkillEffect.PerkRole.Scout, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingPathfinder.Initialize("{=d2qGHXyx}Pathfinder", DefaultSkills.Scouting, this.GetTierCost(2), this._scoutingWaterDiviner, "{=ETiOGIvT}{VALUE}% travel speed on steppes and plains.", SkillEffect.PerkRole.Scout, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "{=sAv1co78}{VALUE}% daily chance to increase relation with a notable by 1 when you enter a town.", SkillEffect.PerkRole.PartyLeader, 0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingWaterDiviner.Initialize("{=gsz9DMNq}Water Diviner", DefaultSkills.Scouting, this.GetTierCost(2), this._scoutingPathfinder, "{=8EtK0F1K}{VALUE}% sight range while traveling on steppes and plains.", SkillEffect.PerkRole.Scout, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=aW1qO2dN}{VALUE}% daily chance to increase relation with a notable by 1 when you enter a village.", SkillEffect.PerkRole.PartyLeader, 0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingForestKin.Initialize("{=0XuFh3cX}Forest Kin", DefaultSkills.Scouting, this.GetTierCost(3), this._scoutingDesertBorn, "{=cpbKNtlZ}{VALUE}% travel speed penalty from forests if your party is composed of 75% or more infantry units.", SkillEffect.PerkRole.Scout, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=xq9wJPKI}{VALUE}% tax income from villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingDesertBorn.Initialize("{=TbBmjK8M}Desert Born", DefaultSkills.Scouting, this.GetTierCost(3), this._scoutingForestKin, "{=k9WaJ396}{VALUE}% travel speed on deserts and dunes.", SkillEffect.PerkRole.Scout, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=nUJfb5VX}{VALUE}% tax income from the governed settlement.", SkillEffect.PerkRole.Governor, 0.025f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingForcedMarch.Initialize("{=jhZe9Mfo}Forced March", DefaultSkills.Scouting, this.GetTierCost(4), this._scoutingUnburdened, "{=zky6r5Ax}{VALUE}% travel speed when the party morale is higher than 75.", SkillEffect.PerkRole.Scout, 0.025f, SkillEffect.EffectIncrementType.AddFactor, "{=hLbn3SBE}{VALUE} experience per day to all troops while traveling with party morale higher than 75.", SkillEffect.PerkRole.PartyLeader, 2f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingUnburdened.Initialize("{=sA2OrT6l}Unburdened", DefaultSkills.Scouting, this.GetTierCost(4), this._scoutingForcedMarch, "{=N5jFSdGR}{VALUE}% overburden penalty.", SkillEffect.PerkRole.Scout, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=OJ9QCJh8}{VALUE} experience per day to all troops when traveling while overburdened.", SkillEffect.PerkRole.PartyLeader, 2f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingTracker.Initialize("{=AoaabumE}Tracker", DefaultSkills.Scouting, this.GetTierCost(5), this._scoutingRanger, "{=mTHliJuT}{VALUE}% track visibility duration.", SkillEffect.PerkRole.Scout, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=pnAq0a40}{VALUE}% travel speed while following a hostile party.", SkillEffect.PerkRole.Scout, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingRanger.Initialize("{=09gOOa0h}Ranger", DefaultSkills.Scouting, this.GetTierCost(5), this._scoutingTracker, "{=boXP9vkF}{VALUE}% track spotting distance.", SkillEffect.PerkRole.Scout, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=aeK3ykbL}{VALUE}% track detection chance.", SkillEffect.PerkRole.Scout, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingMountedScouts.Initialize("{=K9Nb117q}Mounted Scouts", DefaultSkills.Scouting, this.GetTierCost(6), this._scoutingPatrols, "{=DHZxUm6I}{VALUE}% sight range when your party is composed of more than %50 cavalry troops.", SkillEffect.PerkRole.Scout, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=rLs30aPf}{VALUE} party size limit.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingPatrols.Initialize("{=uKc4le8Q}Patrols", DefaultSkills.Scouting, this.GetTierCost(6), this._scoutingMountedScouts, "{=2ljMER8Z}{VALUE} battle morale against bandit parties.", SkillEffect.PerkRole.Scout, 5f, SkillEffect.EffectIncrementType.Add, "{=7K0BqbFG}{VALUE}% advantage against bandits when troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingForagers.Initialize("{=LPxEDIk7}Foragers", DefaultSkills.Scouting, this.GetTierCost(7), this._scoutingBeastWhisperer, "{=FepLiMeY}{VALUE}% food consumption while traveling through steppes and forests.", SkillEffect.PerkRole.Scout, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=kjn1D5Td}{VALUE}% disorganized state duration.", SkillEffect.PerkRole.PartyLeader, -0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingBeastWhisperer.Initialize("{=mrtDAhtL}Beast Whisperer", DefaultSkills.Scouting, this.GetTierCost(7), this._scoutingForagers, "{=jGAe89hM}{VALUE}% chance to find a mount when traveling through steppes and plains.", SkillEffect.PerkRole.Scout, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=Yp4zv2ib}{VALUE}% carrying capacity for pack animals in your party.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingVillageNetwork.Initialize("{=lYQAuYaH}Village Network", DefaultSkills.Scouting, this.GetTierCost(8), this._scoutingRumourNetwork, "{=zj4Sz28B}{VALUE}% trade penalty with villages of your own culture.", SkillEffect.PerkRole.PartyLeader, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=j9KDLaDo}{VALUE}% villager party size of villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingRumourNetwork.Initialize("{=LwWyc6ou}Rumor Network", DefaultSkills.Scouting, this.GetTierCost(8), this._scoutingVillageNetwork, "{=c7V0ayuX}{VALUE}% trade penalty within cities of your own kingdom.", SkillEffect.PerkRole.PartyLeader, -0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=JrTtFFfe}{VALUE}% hideout detection range.", SkillEffect.PerkRole.Scout, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingVantagePoint.Initialize("{=EC2n5DBl}Vantage Point", DefaultSkills.Scouting, this.GetTierCost(9), this._scoutingKeenSight, "{=Y1lC59hw}{VALUE}% sight range when stationary for at least an hour.", SkillEffect.PerkRole.Scout, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=POp8DAZD}{VALUE} prisoner limit.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingKeenSight.Initialize("{=3yVPrhXt}Keen Sight", DefaultSkills.Scouting, this.GetTierCost(9), this._scoutingVantagePoint, "{=dt1xXqbD}{VALUE}% sight penalty for traveling in forests.", SkillEffect.PerkRole.Scout, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=Lr7TZOFL}{VALUE}% chance of prisoner lords escaping from your party.", SkillEffect.PerkRole.PartyLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingVanguard.Initialize("{=Cp7dI87a}Vanguard", DefaultSkills.Scouting, this.GetTierCost(10), this._scoutingRearguard, "{=9yN8Fpv6}{VALUE}% damage by your troops when they are sent as attackers.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=Bzoxobzn}{VALUE}% damage by your troops when they are sent to sally out.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingRearguard.Initialize("{=e4QAc5A6}Rearguard", DefaultSkills.Scouting, this.GetTierCost(10), this._scoutingVanguard, "{=WlAAsJNK}{VALUE}% wounded troop recovery speed while in an army.", SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=dlnOcIyj}{VALUE}% damage by your troops when defending at your siege camp.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._scoutingUncannyInsight.Initialize("{=M9vC9mio}Uncanny Insight", DefaultSkills.Scouting, this.GetTierCost(11), null, "{=4Onw6Gxa}{VALUE}% party speed for every skill point above 200 scouting skill.", SkillEffect.PerkRole.Scout, 0.001f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsTightFormations.Initialize("{=EX5cZDLH}Tight Formations", DefaultSkills.Tactics, this.GetTierCost(1), this._tacticsLooseFormations, "{=eJ8AN9Au}{VALUE}% damage by your infantry to cavalry when troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=gJJ2F3iL}{VALUE}% morale penalty when troops in your formation use shield wall, square, skein, column formations.", SkillEffect.PerkRole.Captain, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._tacticsLooseFormations.Initialize("{=9y3X0MQg}Loose Formations", DefaultSkills.Tactics, this.GetTierCost(1), this._tacticsTightFormations, "{=Xykn90RV}{VALUE}% damage to your infantry from ranged troops when troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=jZzVlDlf}{VALUE}% morale penalty when troops in your formation use line, loose, circle or scatter formations.", SkillEffect.PerkRole.Captain, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._tacticsExtendedSkirmish.Initialize("{=EsYYcvcA}Extended Skirmish", DefaultSkills.Tactics, this.GetTierCost(2), this._tacticsDecisiveBattle, "{=Jm0GA3ak}{VALUE}% damage in snowy and forest terrains when troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=U3B7zaQb}{VALUE}% movement speed to troops in your formation in snowy and forest terrains.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._tacticsDecisiveBattle.Initialize("{=4ElA7gRS}Decisive Battle", DefaultSkills.Tactics, this.GetTierCost(2), this._tacticsExtendedSkirmish, "{=CcggbEVk}{VALUE}% damage in plains, steppes and deserts when your troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=7yOCFsG5}{VALUE}% movement speed to troops in your formation in plains, steppes and deserts.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._tacticsSmallUnitTactics.Initialize("{=30hNRt3x}Small Unit Tactics", DefaultSkills.Tactics, this.GetTierCost(3), this._tacticsHordeLeader, "{=3mJMAX0Y}{VALUE} troop for the hideout crew", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, "{=GDSQyMaG}{VALUE}% movement speed to troops in your formation when there are less than 15 soldiers.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._tacticsHordeLeader.Initialize("{=Vp8Pwou8}Horde Leader", DefaultSkills.Tactics, this.GetTierCost(3), this._tacticsSmallUnitTactics, "{=NouDSrXE}{VALUE} party size.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, "{=y52Zz7U9}{VALUE}% army cohesion loss to commanded armies.", SkillEffect.PerkRole.ArmyCommander, -0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsLawKeeper.Initialize("{=zUK9JOlb}Law Keeper", DefaultSkills.Tactics, this.GetTierCost(4), this._tacticsCoaching, "{=QOMr1QS7}{VALUE}% damage against bandits when your troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=yfRAX2Qv}{VALUE}% damage against bandits by troops in your formation.", SkillEffect.PerkRole.Captain, 0.04f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._tacticsCoaching.Initialize("{=afaCdojS}Coaching", DefaultSkills.Tactics, this.GetTierCost(4), this._tacticsLawKeeper, "{=KSWdxKPJ}{VALUE}% damage when your troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, 0.03f, SkillEffect.EffectIncrementType.AddFactor, "{=9CjoaJwe}{VALUE}% damage by troops in your formation.", SkillEffect.PerkRole.Captain, 0.01f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._tacticsSwiftRegroup.Initialize("{=nmJe4wN1}Swift Regroup", DefaultSkills.Tactics, this.GetTierCost(5), this._tacticsImproviser, "{=9f16mDn0}{VALUE}% disorganized state duration when a raid or siege is broken.", SkillEffect.PerkRole.PartyMember, -0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=0pW4fcjt}{VALUE}% troops left behind when escaping from battles.", SkillEffect.PerkRole.PartyLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsImproviser.Initialize("{=qAn93jVN}Improviser", DefaultSkills.Tactics, this.GetTierCost(5), this._tacticsSwiftRegroup, "{=pFSWDNaF}No morale penalty for disorganized state in battles, in sally out or when being attacked.", SkillEffect.PerkRole.PartyMember, 0f, SkillEffect.EffectIncrementType.Add, "{=4V8CS018}{VALUE}% loss of troops when breaking into or out of a settlement under siege.", SkillEffect.PerkRole.PartyLeader, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsOnTheMarch.Initialize("{=kolBffjD}On The March", DefaultSkills.Tactics, this.GetTierCost(6), this._tacticsCallToArms, "{=C6rYWvrb}{VALUE}% fortification bonus to enemies when troops are sent to confront the enemy.", SkillEffect.PerkRole.ArmyCommander, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=09npcQY0}{VALUE}% fortification bonus to the governed settlement", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsCallToArms.Initialize("{=mUubYb7v}Call To Arms", DefaultSkills.Tactics, this.GetTierCost(6), this._tacticsOnTheMarch, "{=3UB3qhjk}{VALUE}% movement speed to parties called to your army.", SkillEffect.PerkRole.ArmyCommander, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=mAKqS7Rk}{VALUE}% influence required to call parties to your army", SkillEffect.PerkRole.ArmyCommander, -0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsPickThemOfTheWalls.Initialize("{=XQkY7jkL}Pick Them Off The Walls", DefaultSkills.Tactics, this.GetTierCost(7), this._tacticsMakeThemPay, "{=mmRmG5AY}{VALUE}% chance for dealing double damage to siege defender troops in siege bombardment", SkillEffect.PerkRole.Engineer, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=bBRA2jJp}{VALUE}% chance for dealing double damage to besieging troops in siege bombardment of the governed settlement.", SkillEffect.PerkRole.Governor, 0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsMakeThemPay.Initialize("{=8xxeNK0o}Make Them Pay", DefaultSkills.Tactics, this.GetTierCost(7), this._tacticsPickThemOfTheWalls, "{=e2N77Ufi}{VALUE}% damage to defender siege engines.", SkillEffect.PerkRole.Engineer, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=hDvZTHbq}{VALUE}% damage to besieging siege engines.", SkillEffect.PerkRole.Governor, 0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsEliteReserves.Initialize("{=luDtfSN7}Elite Reserves", DefaultSkills.Tactics, this.GetTierCost(8), this._tacticsEncirclement, "{=zVEvl8WQ}{VALUE}% less damage to tier 3+ units when troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=ldD4sVOi}{VALUE}% damage taken by troops in your formation.", SkillEffect.PerkRole.Captain, -0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._tacticsEncirclement.Initialize("{=EhaMPtRX}Encirclement", DefaultSkills.Tactics, this.GetTierCost(8), this._tacticsEliteReserves, "{=seiduCHq}{VALUE}% damage to outnumbered enemies when troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=mtB1tUIb}{VALUE}% influence cost to boost army cohesion.", SkillEffect.PerkRole.ArmyCommander, -0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsPreBattleManeuvers.Initialize("{=cHgLxbbc}Pre Battle Maneuvers", DefaultSkills.Tactics, this.GetTierCost(9), this._tacticsBesieged, "{=dPo5goLo}{VALUE}% influence gain from winning battles.", SkillEffect.PerkRole.PartyMember, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=70PZ5mFx}{VALUE}% damage per 100 skill difference with the enemy when troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, 0.01f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsBesieged.Initialize("{=ALC3Kzv9}Besieged", DefaultSkills.Tactics, this.GetTierCost(9), this._tacticsPreBattleManeuvers, "{=gjkWXuwC}{VALUE}% damage while besieged when troops are sent to confront the enemy.", SkillEffect.PerkRole.PartyMember, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=SIMwGiJF}{VALUE}% influence gain from winning sieges.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsCounteroffensive.Initialize("{=mn5tQhyp}Counter Offensive", DefaultSkills.Tactics, this.GetTierCost(10), this._tacticsGensdarmes, "{=FQppujVl}{VALUE}% damage when troops are sent to confront the attacking enemy in a field battle.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=4Xb1xtbF}{VALUE}% damage when troops are sent to confront the enemy while outnumbered.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tacticsGensdarmes.Initialize("{=CTEuBfU0}Gens d'armes", DefaultSkills.Tactics, this.GetTierCost(10), this._tacticsCounteroffensive, "{=cPvszBhr}{VALUE}% damage to infantry by cavalry troops in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "{=buFin46y}{VALUE} daily security in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Mounted, TroopUsageFlags.Undefined);
			this._tacticsTacticalMastery.Initialize("{=8rvpcb4k}Tactical Mastery", DefaultSkills.Tactics, this.GetTierCost(11), null, "{=ClrLzkvx}{VALUE}% damage for every skill point above 200 tactics skill when troops are sent to confront the enemy.", SkillEffect.PerkRole.ArmyCommander, 0.005f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryNoRestForTheWicked.Initialize("{=RyfFWmDs}No Rest for the Wicked", DefaultSkills.Roguery, this.GetTierCost(1), this._roguerySweetTalker, "{=yZarNiMq}{VALUE}% experience gain for bandits in your party.", SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=IqRNTls2}{VALUE}% raid speed.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._roguerySweetTalker.Initialize("{=570wiYEe}Sweet Talker", DefaultSkills.Roguery, this.GetTierCost(1), this._rogueryNoRestForTheWicked, "{=P3d4nn88}{VALUE}% chance for convincing bandits to leave in peace with barter.", SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=icyzOJZf}{VALUE}% prisoner escape chance in the governed settlement.", SkillEffect.PerkRole.Governor, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryTwoFaced.Initialize("{=kg4Mx9j4}Two Faced", DefaultSkills.Roguery, this.GetTierCost(2), this._rogueryDeepPockets, "{=uDRb7FmU}{VALUE}% increased chance for sneaking into towns", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=PznnUlI3}No morale loss from converting bandit prisoners.", SkillEffect.PerkRole.PartyLeader, 0f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryDeepPockets.Initialize("{=by1b61pn}Deep Pockets", DefaultSkills.Roguery, this.GetTierCost(2), this._rogueryTwoFaced, "{=ixiL39S4}Double the amount of betting allowed in tournaments.", SkillEffect.PerkRole.Personal, 2f, SkillEffect.EffectIncrementType.AddFactor, "{=xSKhyecU}{VALUE}% bandit troop wages.", SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryInBestLight.Initialize("{=xoARIHde}In Best Light", DefaultSkills.Roguery, this.GetTierCost(3), this._rogueryKnowHow, "{=fcraUMzb}{VALUE} extra troop from village notables when successfully forced for volunteers.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, "{=UYYntLzb}{VALUE}% faster recovery from raids for your villages.", SkillEffect.PerkRole.ClanLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryKnowHow.Initialize("{=tvoN5ynt}Know-How", DefaultSkills.Roguery, this.GetTierCost(3), this._rogueryInBestLight, "{=swgcsLOA}{VALUE}% more loot from defeated villagers and caravans.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=XwmYu5rH}{VALUE} security per day in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryPromises.Initialize("{=XZOtTuxA}Promises", DefaultSkills.Roguery, this.GetTierCost(4), this._rogueryManhunter, "{=jKUmtH7z}{VALUE}% food consumption for bandit units in your party.", SkillEffect.PerkRole.PartyLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=zHQuHeIg}{VALUE}% recruitment rate for bandit prisoners in your party.", SkillEffect.PerkRole.PartyLeader, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryManhunter.Initialize("{=GeB42ygg}Manhunter", DefaultSkills.Roguery, this.GetTierCost(4), this._rogueryPromises, "{=pcys1RSF}{VALUE}% better deals with ransom broker for regular troops.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=POp8DAZD}{VALUE} prisoner limit.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryScarface.Initialize("{=XqSn5Uo0}Scarface", DefaultSkills.Roguery, this.GetTierCost(5), this._rogueryWhiteLies, "{=FaGc9xR4}{VALUE}% chance for bandits, villagers and caravans to surrender.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=1IYP1wHc}{VALUE}% chance per day to increase relation with a notable by 1 in the governed settlement.", SkillEffect.PerkRole.Governor, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryWhiteLies.Initialize("{=F51HfzZj}White Lies", DefaultSkills.Roguery, this.GetTierCost(5), this._rogueryScarface, "{=mseUsbjg}{VALUE}% crime rating decrease rate.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=R8vLC6j0}{VALUE}% chance to get 1 relation per day with a random notable in the governed settlement.", SkillEffect.PerkRole.Governor, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._roguerySmugglerConnections.Initialize("{=E8a2joMO}Smuggler Connections", DefaultSkills.Roguery, this.GetTierCost(6), this._rogueryPartnersInCrime, "{=XZe7YPLJ}{VALUE} armor points to equipped civilian body armors.", SkillEffect.PerkRole.Personal, 10f, SkillEffect.EffectIncrementType.Add, "{=gXmzmJbg}{VALUE}% trade penalty when you have crime rating.", SkillEffect.PerkRole.PartyLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryPartnersInCrime.Initialize("{=2PVm7NON}Partners in Crime", DefaultSkills.Roguery, this.GetTierCost(6), this._roguerySmugglerConnections, "{=zFfkR2uK}Surrendering bandit parties always offer to join you.", SkillEffect.PerkRole.PartyLeader, 0f, SkillEffect.EffectIncrementType.AddFactor, "{=mNleBavO}{VALUE}% damage by bandit troops in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._rogueryOneOfTheFamily.Initialize("{=oumTabhS}One of the Family", DefaultSkills.Roguery, this.GetTierCost(7), this._roguerySaltTheEarth, "{=w0LOgr9e}{VALUE} bonus Vigor and Control skills to bandit units in your party", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, "{=Dn0yNCn8}{VALUE} recruitment slot when recruiting from gang leaders.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._roguerySaltTheEarth.Initialize("{=tuv1O7ig}Salt the Earth", DefaultSkills.Roguery, this.GetTierCost(7), this._rogueryOneOfTheFamily, "{=MesU0nGW}{VALUE}% more loot when villagers comply to your hostile actions.", SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=YbioVwyr}{VALUE}% tariff revenue in the governed settlement.", SkillEffect.PerkRole.Governor, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryCarver.Initialize("{=7gZo2SY4}Carver", DefaultSkills.Roguery, this.GetTierCost(8), this._rogueryRansomBroker, "{=g2Zy1Bso}{VALUE}% damage with civilian weapons.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=hiH9dVhH}{VALUE}% one handed damage by troops under your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.OneHandedUser);
			this._rogueryRansomBroker.Initialize("{=W2WXkiAh}Ransom Broker", DefaultSkills.Roguery, this.GetTierCost(8), this._rogueryCarver, "{=7gabTf4P}{VALUE}% better deals for heroes from ransom brokers.", SkillEffect.PerkRole.PartyLeader, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=8aajPkKG}{VALUE}% escape chance for hero prisoners.", SkillEffect.PerkRole.PartyLeader, -0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryArmsDealer.Initialize("{=5bmlZ26b}Arms Dealer", DefaultSkills.Roguery, this.GetTierCost(9), this._rogueryDirtyFighting, "{=o5rp0ViP}{VALUE}% sell price penalty for weapons.", SkillEffect.PerkRole.PartyLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=nmTai1Vw}{VALUE}% militia per day in the besieged governed settlement.", SkillEffect.PerkRole.Governor, 2f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryDirtyFighting.Initialize("{=bb1hS9I4}Dirty Fighting", DefaultSkills.Roguery, this.GetTierCost(9), this._rogueryArmsDealer, "{=bm3eSbBD}{VALUE}% stun duration for kicking.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=iuCYTaMJ}{VALUE} random food item will be smuggled to the besieged governed settlement.", SkillEffect.PerkRole.Governor, 2f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryDashAndSlash.Initialize("{=w1B71sNj}Dash and Slash", DefaultSkills.Roguery, this.GetTierCost(10), this._rogueryFleetFooted, "{=QiZfTNWJ}{VALUE}% damage bonus from speed while on foot.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=hRCvgbQ5}{VALUE}% two handed weapon damage by troops in your formation.", SkillEffect.PerkRole.Captain, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.TwoHandedUser);
			this._rogueryFleetFooted.Initialize("{=yY5iDvAb}Fleet Footed", DefaultSkills.Roguery, this.GetTierCost(10), this._rogueryDashAndSlash, "{=93Z7G161}{VALUE}% combat movement speed while no weapons or shields are equipped.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=lSebD5Fa}{VALUE}% escape chance when imprisoned by mobile parties.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._rogueryRogueExtraordinaire.Initialize("{=U3cgqyUE}Rogue Extraordinaire", DefaultSkills.Roguery, this.GetTierCost(11), null, "{=ClrwacPi}{VALUE}% loot amount for every skill point above 200.", SkillEffect.PerkRole.Personal, 0.01f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmVirile.Initialize("{=mbqoZ4WH}Virile", DefaultSkills.Charm, this.GetTierCost(1), this._charmSelfPromoter, "{=pdQbJrr4}{VALUE}% more likely to have children.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=29R5VkXa}{VALUE}% daily chance to get +1 relation with a random notable in the governed settlement while a continuous project is active.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmSelfPromoter.Initialize("{=hkG9ATZy}Self Promoter", DefaultSkills.Charm, this.GetTierCost(1), this._charmVirile, "{=qARDRFqO}{VALUE} renown when a tournament is won.", SkillEffect.PerkRole.Personal, 3f, SkillEffect.EffectIncrementType.Add, "{=PSvarWWW}{VALUE} morale while defending in a besieged settlement.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmOratory.Initialize("{=OZXEMb2C}Oratory", DefaultSkills.Charm, this.GetTierCost(2), this._charmWarlord, "{=qRoQuHe4}{VALUE} renown and influence for each issue resolved", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "{=YBmzuIbm}{VALUE} relationship with a random notable of your kingdom when an enemy lord is defeated.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmWarlord.Initialize("{=jiWr5Rlz}Warlord", DefaultSkills.Charm, this.GetTierCost(2), this._charmOratory, "{=IbQlvyY5}{VALUE}% influence gain from battles.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=231BaeH9}{VALUE} relationship with a random lord of your kingdom when an enemy lord is defeated.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmForgivableGrievances.Initialize("{=l863hIyN}Forgivable Grievances", DefaultSkills.Charm, this.GetTierCost(3), this._charmMeaningfulFavors, "{=BCB08mNZ}{VALUE}% chance of avoiding critical failure on persuasion.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=bFMDTiLE}{VALUE}% daily chance to increase relations with a random lord or notable with negative relations with you when you are in a settlement.", SkillEffect.PerkRole.Personal, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmMeaningfulFavors.Initialize("{=4hUEryJ6}Meaningful Favors", DefaultSkills.Charm, this.GetTierCost(3), this._charmForgivableGrievances, "{=T1N2w4uK}{VALUE}% chance for double persuasion success.", SkillEffect.PerkRole.Personal, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=6WP4OkKt}{VALUE}% daily chance to increase relations with powerful notables in the governed settlement.", SkillEffect.PerkRole.Governor, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmInBloom.Initialize("{=ZlXSlx0p}In Bloom", DefaultSkills.Charm, this.GetTierCost(4), this._charmYoungAndRespectful, "{=aVWb6aoQ}{VALUE}% relationship gain with the opposing gender.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=SimMOKbW}{VALUE}% daily chance to increase relations with a random notable of opposed sex in the governed settlement.", SkillEffect.PerkRole.Governor, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmYoungAndRespectful.Initialize("{=TpzZgFsA}Young And Respectful", DefaultSkills.Charm, this.GetTierCost(4), this._charmInBloom, "{=3MOJjS7A}{VALUE}% relationship gain with the same gender.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=7e397ieb}{VALUE}% daily chance to increase relations with a random notable of same sex in the governed settlement.", SkillEffect.PerkRole.Governor, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmFirebrand.Initialize("{=EbKP7Xx5}Firebrand", DefaultSkills.Charm, this.GetTierCost(5), this._charmFlexibleEthics, "{=vYj0z0zr}{VALUE}% influence cost to initiate kingdom decisions.", SkillEffect.PerkRole.ClanLeader, -0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=4ajo4jvp}{VALUE} recruitment slot from rural notables.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmFlexibleEthics.Initialize("{=58Imsasy}Flexible Ethics", DefaultSkills.Charm, this.GetTierCost(5), this._charmFirebrand, "{=HkOatwqw}{VALUE}% influence cost when voting for kingdom proposals made by others.", SkillEffect.PerkRole.Personal, -0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=FbAGhzbI}{VALUE} recruitment slot from urban notables.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmEffortForThePeople.Initialize("{=RIiVDdi0}Effort For The People", DefaultSkills.Charm, this.GetTierCost(6), this._charmSlickNegotiator, "{=P2eOw2sQ}{VALUE} relation with the nearest settlement owner clan when you clear a hideout. +1 town loyalty if it is your clan.", SkillEffect.PerkRole.Personal, 3f, SkillEffect.EffectIncrementType.Add, "{=FpleMw35}{VALUE}% barter penalty with lords of same culture.", SkillEffect.PerkRole.Personal, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmSlickNegotiator.Initialize("{=WOqxWM67}Slick Negotiator", DefaultSkills.Charm, this.GetTierCost(6), this._charmEffortForThePeople, "{=AqpEXxNy}{VALUE}% hiring costs of mercenary troops.", SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=6ex96ekx}{VALUE}% barter penalty with lords of different cultures.", SkillEffect.PerkRole.Personal, -0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmGoodNatured.Initialize("{=2y7gahYi}Good Natured", DefaultSkills.Charm, this.GetTierCost(7), this._charmTribute, "{=aitgGIog}{VALUE}% influence return when a supported proposal fails to pass.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.AddFactor, "{=fpaeONmG}{VALUE} extra relationship when you increase relationship with merciful lords.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmTribute.Initialize("{=dSBbSHkM}Tribute", DefaultSkills.Charm, this.GetTierCost(7), this._charmGoodNatured, "{=nJu03DL9}{VALUE}% relationship bonus when you pay more than minimum amount in barters.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=iqJQd4D8}{VALUE} extra relationship when you increase relationship with cruel lords.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmMoralLeader.Initialize("{=zUXUrGWa}Moral Leader", DefaultSkills.Charm, this.GetTierCost(8), this._charmNaturalLeader, "{=9mlBbzLx}{VALUE} persuasion success required against characters of your own culture.", SkillEffect.PerkRole.Personal, -1f, SkillEffect.EffectIncrementType.Add, "{=Cm0OcbsV}{VALUE} relation with settlement notables when a project is completed in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmNaturalLeader.Initialize("{=qaZDUknZ}Natural Leader", DefaultSkills.Charm, this.GetTierCost(8), this._charmMoralLeader, "{=dyVvsBMs}{VALUE} persuasion success required against characters of different cultures.", SkillEffect.PerkRole.Personal, -1f, SkillEffect.EffectIncrementType.Add, "{=30eSZeZd}{VALUE}% experience gain for companions.", SkillEffect.PerkRole.ClanLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmPublicSpeaker.Initialize("{=16fxd9fN}Public Speaker", DefaultSkills.Charm, this.GetTierCost(9), this._charmParade, "{=z4naITkR}{VALUE}% renown gain from battles.", SkillEffect.PerkRole.PartyLeader, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=J7JaXZm8}{VALUE}% effect from forums, marketplaces and festivals.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmParade.Initialize("{=DTnaWgAv}Parade", DefaultSkills.Charm, this.GetTierCost(9), this._charmPublicSpeaker, "{=yA2P7w9N}{VALUE} loyalty bonus to settlement while waiting in the settlement.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, "{=rHtwp8ag}{VALUE}% daily chance to gain +1 relationship with a random lord in the same army.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmCamaraderie.Initialize("{=p2zZGkZw}Camaraderie", DefaultSkills.Charm, this.GetTierCost(10), null, "{=l2ZKUJQY}Double the relation gain for helping lords in battle.", SkillEffect.PerkRole.Personal, 2f, SkillEffect.EffectIncrementType.AddFactor, "{=XmwIHIMN}{VALUE} companion limit", SkillEffect.PerkRole.ClanLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._charmImmortalCharm.Initialize("{=9XWiXokY}Immortal Charm", DefaultSkills.Charm, this.GetTierCost(11), null, "{=BjLYCHMD}{VALUE} influence per day.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipCombatTips.Initialize("{=Cb5s9HlD}Combat Tips", DefaultSkills.Leadership, this.GetTierCost(1), this._leadershipRaiseTheMeek, "{=76TOkicW}{VALUE} experience per day to all troops in party.", SkillEffect.PerkRole.PartyLeader, 2f, SkillEffect.EffectIncrementType.Add, "{=z3OU7vrn}{VALUE} to troop tiers when recruiting from same culture.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipRaiseTheMeek.Initialize("{=JGCtv8om}Raise The Meek", DefaultSkills.Leadership, this.GetTierCost(1), this._leadershipCombatTips, "{=Ra2poaEh}{VALUE} experience per day to tier 1 and 2 troops.", SkillEffect.PerkRole.PartyLeader, 4f, SkillEffect.EffectIncrementType.Add, "{=CjLuIEgh}{VALUE} experience per day to each troop in garrison in the governed settlement.", SkillEffect.PerkRole.Governor, 3f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipFerventAttacker.Initialize("{=MhRF64eR}Fervent Attacker", DefaultSkills.Leadership, this.GetTierCost(2), this._leadershipStoutDefender, "{=o7xn0ybm}{VALUE} starting battle morale when attacking.", SkillEffect.PerkRole.PartyLeader, 4f, SkillEffect.EffectIncrementType.Add, "{=AbulTQm9}{VALUE}% recruitment rate of tier 1, 2 and 3 prisoners.", SkillEffect.PerkRole.PartyLeader, 0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipStoutDefender.Initialize("{=YogcurDJ}Stout Defender", DefaultSkills.Leadership, this.GetTierCost(2), this._leadershipFerventAttacker, "{=RIMXqF1d}{VALUE} battle morale at the beginning of a battle when defending.", SkillEffect.PerkRole.PartyLeader, 8f, SkillEffect.EffectIncrementType.Add, "{=qItLTWR2}{VALUE}% recruitment rate of tier 4+ prisoners.", SkillEffect.PerkRole.PartyLeader, 0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipAuthority.Initialize("{=CeCAMvkX}Authority", DefaultSkills.Leadership, this.GetTierCost(3), this._leadershipHeroicLeader, "{=bezXAM92}{VALUE}% security bonus from the town garrison in the governing settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=rLs30aPf}{VALUE} party size limit.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipHeroicLeader.Initialize("{=7aX2eh5x}Heroic Leader", DefaultSkills.Leadership, this.GetTierCost(3), this._leadershipAuthority, "{=m993aVvX}{VALUE} daily loyalty in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, "{=yvyVugUN}{VALUE}% battle morale penalty to enemies when troops in your formation kill an enemy.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._leadershipLoyaltyAndHonor.Initialize("{=UJYaonYM}Loyalty and Honor", DefaultSkills.Leadership, this.GetTierCost(4), this._leadershipFamousCommander, "{=wBURlfzR}Tier 3+ troops in your party no longer retreat due to low morale", SkillEffect.PerkRole.PartyLeader, 3f, SkillEffect.EffectIncrementType.Add, "{=kuu7M6aQ}{VALUE}% faster non-bandit prisoner recruitment.", SkillEffect.PerkRole.PartyLeader, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipFamousCommander.Initialize("{=FQkHkMhw}Famous Commander", DefaultSkills.Leadership, this.GetTierCost(4), this._leadershipLoyaltyAndHonor, "{=z4naITkR}{VALUE}% renown gain from battles.", SkillEffect.PerkRole.Personal, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=CkJarFvq}{VALUE} experience to troops on recruitment.", SkillEffect.PerkRole.Personal, 200f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipPresence.Initialize("{=6RckjM4S}Presence", DefaultSkills.Leadership, this.GetTierCost(5), this._leadershipLeaderOfTheMasses, "{=UgRGBWhn}{VALUE} security per day while waiting in a town.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, "{=9JN5bc7f}No morale penalty for recruiting prisoners of your faction.", SkillEffect.PerkRole.PartyLeader, 0f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipLeaderOfTheMasses.Initialize("{=T5rM9XgO}Leader of the Masses", DefaultSkills.Leadership, this.GetTierCost(5), this._leadershipPresence, "{=VUma8oHz}{VALUE} party size for each town you control.", SkillEffect.PerkRole.ClanLeader, 5f, SkillEffect.EffectIncrementType.Add, "{=ptmYmT6B}{VALUE}% experience from battles shared with the troops in your party.", SkillEffect.PerkRole.PartyLeader, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipVeteransRespect.Initialize("{=vWGQNcu5}Veteran's Respect", DefaultSkills.Leadership, this.GetTierCost(6), this._leadershipCitizenMilitia, "{=wSLO8VgG}{VALUE} garrison size in the governed settlement.", SkillEffect.PerkRole.Governor, 20f, SkillEffect.EffectIncrementType.Add, "{=lsnrQCB8}Bandits can be converted into regular troops.", SkillEffect.PerkRole.PartyLeader, 0f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipCitizenMilitia.Initialize("{=vZtLm43v}Citizen Militia", DefaultSkills.Leadership, this.GetTierCost(6), this._leadershipVeteransRespect, "{=kVd6nlXo}{VALUE}% chance of militias to spawn as elite troops in the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=QPfj9Dbj}{VALUE}% morale from victories.", SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipInspiringLeader.Initialize("{=kaEzJUTW}Inspiring Leader", DefaultSkills.Leadership, this.GetTierCost(7), this._leadershipUpliftingSpirit, "{=M04V0cmt}{VALUE}% influence cost for calling parties to an army.", SkillEffect.PerkRole.ArmyCommander, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=je77ZaN7}{VALUE}% experience to troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._leadershipUpliftingSpirit.Initialize("{=EbROfVJJ}Uplifting Spirit", DefaultSkills.Leadership, this.GetTierCost(7), this._leadershipInspiringLeader, "{=FZ06ALO6}{VALUE} battle morale in siege battles.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, "{=rLs30aPf}{VALUE} party size limit.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipTrustedCommander.Initialize("{=6ETg3maz}Trusted Commander", DefaultSkills.Leadership, this.GetTierCost(8), this._leadershipLeadByExample, "{=dAS81esi}{VALUE}% recruitment rate for ranged prisoners.", SkillEffect.PerkRole.PartyLeader, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=lutYwHwt}{VALUE}% experience for troops, when they are sent to confront the enemy.", SkillEffect.PerkRole.PartyLeader, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipLeadByExample.Initialize("{=WFFlp3Qi}Lead by Example", DefaultSkills.Leadership, this.GetTierCost(8), this._leadershipTrustedCommander, "{=tEsgNQOZ}{VALUE}% recruitment rate for infantry prisoners.", SkillEffect.PerkRole.PartyLeader, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=aeceOwWb}{VALUE}% shared experience for cavalry troops.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipMakeADifference.Initialize("{=5uW9zKTN}Make a Difference", DefaultSkills.Leadership, this.GetTierCost(9), this._leadershipGreatLeader, "{=YaPOTaMJ}{VALUE}% battle morale to troops when you kill an enemy in battle.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.AddFactor, "{=MMMuSlOW}{VALUE}% shared experience for archers.", SkillEffect.PerkRole.PartyLeader, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipGreatLeader.Initialize("{=3hzSmrMw}Great Leader", DefaultSkills.Leadership, this.GetTierCost(9), this._leadershipMakeADifference, "{=p8pviWlQ}{VALUE} battle morale to troops at the beginning of a battle.", SkillEffect.PerkRole.ArmyCommander, 5f, SkillEffect.EffectIncrementType.Add, "{=LGH67bOj}{VALUE} battle morale to troops that are of same culture as you.", SkillEffect.PerkRole.PartyLeader, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipWePledgeOurSwords.Initialize("{=3GHIb7YX}We Pledge our Swords", DefaultSkills.Leadership, this.GetTierCost(10), this._leadershipTalentMagnet, "{=0AUYrhGw}{VALUE} companion limit.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "{=FkkVHjBP}{VALUE} battle morale at the beginning of the battle for each tier 6 troop in the party up to 10 morale.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipTalentMagnet.Initialize("{=pFfqWRnf}Talent Magnet", DefaultSkills.Leadership, this.GetTierCost(10), this._leadershipWePledgeOurSwords, "{=rLs30aPf}{VALUE} party size limit.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, "{=gqboke7l}{VALUE} clan party limit.", SkillEffect.PerkRole.ClanLeader, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._leadershipUltimateLeader.Initialize("{=FK3W0SKk}Ultimate Leader", DefaultSkills.Leadership, this.GetTierCost(11), null, "{=Q72PJYtf}{VALUE} party size for each leadership point above 250.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeAppraiser.Initialize("{=b3PsxeiB}Appraiser", DefaultSkills.Trade, this.GetTierCost(1), this._tradeWholeSeller, "{=wki8aFec}{VALUE}% price penalty while selling equipment.", SkillEffect.PerkRole.PartyLeader, -0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=gHUQfWlg}Your profits are marked.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeWholeSeller.Initialize("{=lTNpxGoh}Whole Seller", DefaultSkills.Trade, this.GetTierCost(1), this._tradeAppraiser, "{=9Y4rMcYj}{VALUE}% price penalty while selling trade goods.", SkillEffect.PerkRole.PartyLeader, -0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=gHUQfWlg}Your profits are marked.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeCaravanMaster.Initialize("{=5acLha5Q}Caravan Master", DefaultSkills.Trade, this.GetTierCost(2), this._tradeMarketDealer, "{=SPs04fam}{VALUE}% carrying capacity for your party.", SkillEffect.PerkRole.Quartermaster, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=QUYwIYEi}Item prices are marked relative to the average price.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeMarketDealer.Initialize("{=InLGoUbB}Market Dealer", DefaultSkills.Trade, this.GetTierCost(2), this._tradeCaravanMaster, "{=Si3QiLW4}{VALUE}% cost of bartering for safe passage.", SkillEffect.PerkRole.ClanLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=QUYwIYEi}Item prices are marked relative to the average price.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeDistributedGoods.Initialize("{=nxkNY4YG}Distributed Goods", DefaultSkills.Trade, this.GetTierCost(3), this._tradeLocalConnection, "{=we6jYdRD}Double the relationship gain by resolved issues with artisans.", SkillEffect.PerkRole.Personal, 2f, SkillEffect.EffectIncrementType.AddFactor, "{=RYkPTHv1}{VALUE}% price penalty while buying from villages.", SkillEffect.PerkRole.Quartermaster, -0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeLocalConnection.Initialize("{=mznjEwjC}Local Connection", DefaultSkills.Trade, this.GetTierCost(3), this._tradeDistributedGoods, "{=ORencCvQ}Double the relationship gain by resolved issues with merchants.", SkillEffect.PerkRole.Personal, 2f, SkillEffect.EffectIncrementType.Add, "{=AAYplFKi}{VALUE}% price penalty while selling animals.", SkillEffect.PerkRole.Quartermaster, -0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeTravelingRumors.Initialize("{=3j6Ec63l}Traveling Rumors", DefaultSkills.Trade, this.GetTierCost(4), this._tradeTollgates, "{=DV2kW53e}Your caravans gather trade rumors.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "{=D2nbscmg}{VALUE} gold for each villager party visiting the governed settlement.", SkillEffect.PerkRole.Governor, 20f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeTollgates.Initialize("{=JnSh4Fmz}Toll Gates", DefaultSkills.Trade, this.GetTierCost(4), this._tradeTravelingRumors, "{=SOHgkGKy}Your workshops gather trade rumors.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "{=bteVVFh0}{VALUE} gold for each caravan visiting the governed settlement.", SkillEffect.PerkRole.Governor, 30f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeArtisanCommunity.Initialize("{=8f8UGq46}Artisan Community", DefaultSkills.Trade, this.GetTierCost(5), this._tradeGreatInvestor, "{=CBSDuOmp}{VALUE} daily renown from every profiting workshop.", SkillEffect.PerkRole.ClanLeader, 1f, SkillEffect.EffectIncrementType.Add, "{=amA9OfPU}{VALUE} recruitment slot when recruiting from merchant notables. ", SkillEffect.PerkRole.Quartermaster, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeGreatInvestor.Initialize("{=g9qLrEb4}Great Investor", DefaultSkills.Trade, this.GetTierCost(5), this._tradeArtisanCommunity, "{=aYpbyTfA}{VALUE} daily renown from every profiting caravan.", SkillEffect.PerkRole.ClanLeader, 1f, SkillEffect.EffectIncrementType.Add, "{=m41r7FPw}{VALUE}% companion recruitment cost.", SkillEffect.PerkRole.Quartermaster, -0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeMercenaryConnections.Initialize("{=vivNLdHp}Mercenary Connections", DefaultSkills.Trade, this.GetTierCost(6), this._tradeContentTrades, "{=HrTFr1ox}{VALUE}% workshop production rate.", SkillEffect.PerkRole.Governor, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=GNtTFR0j}{VALUE}% mercenary troop wages in your party.", SkillEffect.PerkRole.PartyLeader, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeContentTrades.Initialize("{=FV4SWLQx}Content Trades", DefaultSkills.Trade, this.GetTierCost(6), this._tradeMercenaryConnections, "{=Eo958e7R}{VALUE}% tariff income in the governed settlement.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=Oq1K7oDW}{VALUE}% wages paid while waiting in settlements.", SkillEffect.PerkRole.PartyLeader, -0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeInsurancePlans.Initialize("{=aYQybo4E}Insurance Plans", DefaultSkills.Trade, this.GetTierCost(7), this._tradeRapidDevelopment, "{=NMnpGic4}{VALUE} denar return when one of your caravans is destroyed.", SkillEffect.PerkRole.ClanLeader, 5000f, SkillEffect.EffectIncrementType.Add, "{=xe0dX5QQ}{VALUE}% price penalty while buying food items.", SkillEffect.PerkRole.Quartermaster, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeRapidDevelopment.Initialize("{=u9oONz9o}Rapid Development", DefaultSkills.Trade, this.GetTierCost(7), this._tradeInsurancePlans, "{=EdCkK2c4}{VALUE} denar return for each workshop when workshop's town is captured by an enemy.", SkillEffect.PerkRole.ClanLeader, 5000f, SkillEffect.EffectIncrementType.Add, "{=4ORpHfu2}{VALUE}% price penalty while buying clay, iron, cotton and silver.", SkillEffect.PerkRole.Quartermaster, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeGranaryAccountant.Initialize("{=TFy2VYtM}Granary Accountant", DefaultSkills.Trade, this.GetTierCost(8), this._tradeTradeyardForeman, "{=SyxQF0tM}{VALUE}% price penalty while selling food items.", SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=JnQcDyAz}{VALUE}% production rate to grain, olives, fish, date in villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeTradeyardForeman.Initialize("{=QqKNxmeF}Tradeyard Foreman", DefaultSkills.Trade, this.GetTierCost(8), this._tradeGranaryAccountant, "{=KgrnmR73}{VALUE}% price penalty while selling pottery, tools, cotton and jewelry.", SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=mN3fLgtx}{VALUE}% production rate to clay, iron, cotton and silver in villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeSwordForBarter.Initialize("{=AIsDxCeG}Sword For Barter", DefaultSkills.Trade, this.GetTierCost(9), this._tradeSelfMadeMan, "{=AqpEXxNy}{VALUE}% hiring costs of mercenary troops.", SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=68ye0NpS}{VALUE}% caravan guard wages.", SkillEffect.PerkRole.Quartermaster, -0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeSelfMadeMan.Initialize("{=uHJltZ5D}Self-made Man", DefaultSkills.Trade, this.GetTierCost(9), this._tradeSwordForBarter, "{=rTbVn6sJ}{VALUE}% barter penalty for items.", SkillEffect.PerkRole.Personal, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=Q9VCvUTg}{VALUE}% build speed for marketplace, kiln and aqueduct projects.", SkillEffect.PerkRole.Governor, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeSilverTongue.Initialize("{=5rDdJpJo}Silver Tongue", DefaultSkills.Trade, this.GetTierCost(10), this._tradeSpringOfGold, "{=UzKyyfbF}{VALUE}% gold required while persuading lords to defect to your faction.", SkillEffect.PerkRole.Personal, -0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=Kb9uC4gQ}{VALUE}% better trade deals from caravans and villagers", SkillEffect.PerkRole.Quartermaster, 0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeSpringOfGold.Initialize("{=K0SRwH6E}Spring of Gold", DefaultSkills.Trade, this.GetTierCost(10), this._tradeSilverTongue, "{=gu7EN92A}{VALUE}% denars of interest income per day based on your current denars up to 1000 denars.", SkillEffect.PerkRole.ClanLeader, 0.001f, SkillEffect.EffectIncrementType.AddFactor, "{=XmqJb7RN}{VALUE}% effect from boosting projects in the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeManOfMeans.Initialize("{=Jy2ap8L1}Man of Means", DefaultSkills.Trade, this.GetTierCost(11), this._tradeTrickleDown, "{=7QadTbWs}{VALUE}% costs of recruiting minor faction clans into your clan.", SkillEffect.PerkRole.ClanLeader, -0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=lA0eEkGP}{VALUE}% ransom cost for your freedom.", SkillEffect.PerkRole.Personal, -0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeTrickleDown.Initialize("{=L4fz3Jdr}Trickle Down", DefaultSkills.Trade, this.GetTierCost(11), this._tradeManOfMeans, "{=ANhbaAhL}{VALUE} relationship with merchants if 10.000 or more denars are spent on a single deal.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, "{=REZyGJGH}{VALUE} daily prosperity while building a project in the governed settlement.", SkillEffect.PerkRole.Governor, 2f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._tradeEverythingHasAPrice.Initialize("{=cRwNeSzb}Everything Has a Price", DefaultSkills.Trade, this.GetTierCost(12), null, "{=HeefccTC}You can now trade settlements in barter.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Invalid, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardWarriorsDiet.Initialize("{=mIDsxe1O}Warrior's Diet", DefaultSkills.Steward, this.GetTierCost(1), this._stewardFrugal, "{=6NHvsrrx}{VALUE}% food consumption in your party.", SkillEffect.PerkRole.Quartermaster, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=mSvfxXVW}No morale penalty from having single type of food.", SkillEffect.PerkRole.PartyLeader, 0f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardFrugal.Initialize("{=eJIbMa8P}Frugal", DefaultSkills.Steward, this.GetTierCost(1), this._stewardWarriorsDiet, "{=CJB5HCsI}{VALUE}% wages in your party.", SkillEffect.PerkRole.Quartermaster, -0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=OTyYJ2Bt}{VALUE}% recruitment costs.", SkillEffect.PerkRole.PartyLeader, -0.15f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardSevenVeterans.Initialize("{=2ryLuN2i}Seven Veterans", DefaultSkills.Steward, this.GetTierCost(2), this._stewardDrillSergant, "{=gX0edfpK}{VALUE} daily experience for tier 4+ troops in your party.", SkillEffect.PerkRole.Quartermaster, 4f, SkillEffect.EffectIncrementType.Add, "{=g9gTYB8u}{VALUE} militia recruitment in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardDrillSergant.Initialize("{=L9k4bovO}Drill Sergeant", DefaultSkills.Steward, this.GetTierCost(2), this._stewardSevenVeterans, "{=UYhJZya5}{VALUE} daily experience to troops in your party.", SkillEffect.PerkRole.Quartermaster, 2f, SkillEffect.EffectIncrementType.Add, "{=B2msxAju}{VALUE}% garrison wages in the governed settlement.", SkillEffect.PerkRole.Governor, -0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardSweatshops.Initialize("{=jbAtOsIy}Sweatshops", DefaultSkills.Steward, this.GetTierCost(3), this._stewardStiffUpperLip, "{=6wqJA77K}{VALUE}% production rate to owned workshops.", SkillEffect.PerkRole.Personal, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=rA9nzrAr}{VALUE}% siege engine build rate in your party.", SkillEffect.PerkRole.Quartermaster, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardStiffUpperLip.Initialize("{=QUeJ4gc3}Stiff Upper Lip", DefaultSkills.Steward, this.GetTierCost(3), this._stewardSweatshops, "{=y9AsEMnV}{VALUE}% food consumption in your party while it is part of an army.", SkillEffect.PerkRole.Quartermaster, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=1FPpHasQ}{VALUE}% garrison wages in the governed castle.", SkillEffect.PerkRole.Governor, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardPaidInPromise.Initialize("{=CPxbG7Zp}Paid in Promise", DefaultSkills.Steward, this.GetTierCost(4), this._stewardEfficientCampaigner, "{=H9tQfeBr}{VALUE}% companion wages and recruitment fees.", SkillEffect.PerkRole.PartyLeader, -0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=1eKRHLur}Discarded armors are donated to troops for increased experience.", SkillEffect.PerkRole.Quartermaster, 0f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardEfficientCampaigner.Initialize("{=sC53NYcA}Efficient Campaigner", DefaultSkills.Steward, this.GetTierCost(4), this._stewardPaidInPromise, "{=5t6cveXT}{VALUE} extra food for each food taken during village raids for your party.", SkillEffect.PerkRole.PartyLeader, 1f, SkillEffect.EffectIncrementType.Add, "{=JhFCoWbE}{VALUE}% troop wages in your party while it is part of an army.", SkillEffect.PerkRole.Quartermaster, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardGivingHands.Initialize("{=VsqyzWYY}Giving Hands", DefaultSkills.Steward, this.GetTierCost(5), this._stewardLogistician, "{=WaGKvsfc}Discarded weapons are donated to troops for increased experience.", SkillEffect.PerkRole.Quartermaster, 0f, SkillEffect.EffectIncrementType.AddFactor, "{=Eo958e7R}{VALUE}% tariff income in the governed settlement.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardLogistician.Initialize("{=U2buPiec}Logistician", DefaultSkills.Steward, this.GetTierCost(5), this._stewardGivingHands, "{=sG9WGOeN}{VALUE} party morale when number of mounts is greater than number of foot troops in your party.", SkillEffect.PerkRole.Quartermaster, 4f, SkillEffect.EffectIncrementType.Add, "{=Z1n0w5Kc}{VALUE}% tax income.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardRelocation.Initialize("{=R6dnhblo}Relocation", DefaultSkills.Steward, this.GetTierCost(6), this._stewardAidCorps, "{=urSSNtUD}{VALUE}% influence gain from donating troops.", SkillEffect.PerkRole.Quartermaster, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=XmqJb7RN}{VALUE}% effect from boosting projects in the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardAidCorps.Initialize("{=4FdtVyj1}Aid Corps", DefaultSkills.Steward, this.GetTierCost(6), this._stewardRelocation, "{=ZLbCqt23}Wounded troops in your party are no longer paid wages.", SkillEffect.PerkRole.Quartermaster, 0f, SkillEffect.EffectIncrementType.AddFactor, "{=ULY7byYc}{VALUE}% hearth growth in villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardGourmet.Initialize("{=63lHFDSG}Gourmet", DefaultSkills.Steward, this.GetTierCost(7), this._stewardSoundReserves, "{=KDtcsKUs}Double the morale bonus from having diverse food in your party.", SkillEffect.PerkRole.Quartermaster, 2f, SkillEffect.EffectIncrementType.AddFactor, "{=q2ZDAm2v}{VALUE}% garrison food consumption during sieges in the governed settlement.", SkillEffect.PerkRole.Governor, -0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardSoundReserves.Initialize("{=O5dgeoss}Sound Reserves", DefaultSkills.Steward, this.GetTierCost(7), this._stewardGourmet, "{=RkYL5eaP}{VALUE}% troop upgrade costs.", SkillEffect.PerkRole.Quartermaster, -0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=P10E5o9l}{VALUE}% food consumption during sieges in your party.", SkillEffect.PerkRole.Quartermaster, -0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardForcedLabor.Initialize("{=cWyqiNrf}Forced Labor", DefaultSkills.Steward, this.GetTierCost(8), this._stewardContractors, "{=HrOTTjgo}Prisoners in your party provide carry capacity as if they are standard troops.", SkillEffect.PerkRole.Quartermaster, 0f, SkillEffect.EffectIncrementType.AddFactor, "{=T9Viygs8}{VALUE}% construction speed per every 3 prisoners.", SkillEffect.PerkRole.Governor, 0.01f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardContractors.Initialize("{=Pg5enC8c}Contractors", DefaultSkills.Steward, this.GetTierCost(8), this._stewardForcedLabor, "{=4220dQ4j}{VALUE}% wages and upgrade costs of the mercenary troops in your party.", SkillEffect.PerkRole.Quartermaster, -0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=xiTD2qUv}{VALUE}% town project effects in the governed settlement.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardArenicosMules.Initialize("{=qBx8UbUt}Arenicos' Mules", DefaultSkills.Steward, this.GetTierCost(9), this._stewardArenicosHorses, "{=Yp4zv2ib}{VALUE}% carrying capacity for pack animals in your party.", SkillEffect.PerkRole.Quartermaster, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=fswrp38u}{VALUE}% trade penalty for trading pack animals.", SkillEffect.PerkRole.Quartermaster, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardArenicosHorses.Initialize("{=tbQ5bUzD}Arenicos' Horses", DefaultSkills.Steward, this.GetTierCost(9), this._stewardArenicosMules, "{=G9OTNRs4}{VALUE}% carrying capacity for troops in your party.", SkillEffect.PerkRole.Quartermaster, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=xm4eEbQY}{VALUE}% trade penalty for trading mounts.", SkillEffect.PerkRole.Personal, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardMasterOfPlanning.Initialize("{=n5aT1Y7s}Master of Planning", DefaultSkills.Steward, this.GetTierCost(10), this._stewardMasterOfWarcraft, "{=KMmAG5bk}{VALUE}% food consumption while your party is in a siege camp.", SkillEffect.PerkRole.Quartermaster, -0.4f, SkillEffect.EffectIncrementType.AddFactor, "{=P5OjioRl}{VALUE}% effectiveness to continuous projects in the governed settlement. ", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardMasterOfWarcraft.Initialize("{=MM0ARhGh}Master of Warcraft", DefaultSkills.Steward, this.GetTierCost(10), this._stewardMasterOfPlanning, "{=StzVsQ2P}{VALUE}% troop wages while your party is in a siege camp.", SkillEffect.PerkRole.Quartermaster, -0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=ya7alenH}{VALUE}% food consumption of town population in the governed settlement.", SkillEffect.PerkRole.Governor, -0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._stewardPriceOfLoyalty.Initialize("{=eVTnUmSB}Price of Loyalty", DefaultSkills.Steward, this.GetTierCost(11), null, "{=sYrG8rNy}{VALUE}% to food consumption, wages and combat related morale loss for each steward point above 250 in your party.", SkillEffect.PerkRole.Quartermaster, -0.005f, SkillEffect.EffectIncrementType.AddFactor, "{=lwp50FuF}{VALUE}% tax income for each skill point above 200 in the governed settlement", SkillEffect.PerkRole.Governor, 0.005f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineSelfMedication.Initialize("{=TLGvIdJB}Self Medication", DefaultSkills.Medicine, this.GetTierCost(1), this._medicinePreventiveMedicine, "{=bLAw2di4}{VALUE}% healing rate.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=V53EYEXx}{VALUE}% combat movement speed.", SkillEffect.PerkRole.Personal, 0.02f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicinePreventiveMedicine.Initialize("{=wI393cla}Preventive Medicine", DefaultSkills.Medicine, this.GetTierCost(1), this._medicineSelfMedication, "{=Ti9auMiO}{VALUE} hit points.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, "{=10cVZTTm}{VALUE}% recovery of lost hit points after each battle.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineTriageTent.Initialize("{=EU4JjLqV}Triage Tent", DefaultSkills.Medicine, this.GetTierCost(2), this._medicineWalkItOff, "{=ZMPhsLdx}{VALUE}% healing rate when stationary on the campaign map.", SkillEffect.PerkRole.Surgeon, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=Mn714dPH}{VALUE}% food consumption for besieged governed settlement.", SkillEffect.PerkRole.Governor, -0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineWalkItOff.Initialize("{=0pyLfrGZ}Walk It Off", DefaultSkills.Medicine, this.GetTierCost(2), this._medicineTriageTent, "{=NtCBRiLH}{VALUE}% healing rate when moving on the campaign map.", SkillEffect.PerkRole.Surgeon, 0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=4YNqWPEu}{VALUE} hit points recovery after each offensive battle.", SkillEffect.PerkRole.Personal, 10f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineSledges.Initialize("{=TyB6y5bh}Sledges", DefaultSkills.Medicine, this.GetTierCost(3), this._medicineDoctorsOath, "{=bFOfZmwC}{VALUE}% party speed penalty from the wounded.", SkillEffect.PerkRole.Surgeon, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=dfULyKsz}{VALUE} hit points to mounts in your party.", SkillEffect.PerkRole.PartyLeader, 15f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineDoctorsOath.Initialize("{=PAwDV08b}Doctor's Oath", DefaultSkills.Medicine, this.GetTierCost(3), this._medicineSledges, "{=XPB1iBkh}Your medicine skill also applies to enemy casualties, increasing potential prisoners.", SkillEffect.PerkRole.Surgeon, 0f, SkillEffect.EffectIncrementType.AddFactor, "{=Ti9auMiO}{VALUE} hit points.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineBestMedicine.Initialize("{=ei1JSeco}Best Medicine", DefaultSkills.Medicine, this.GetTierCost(4), this._medicineGoodLodging, "{=L3kTYA2p}{VALUE}% healing rate while party morale is above 70.", SkillEffect.PerkRole.Surgeon, 0.15f, SkillEffect.EffectIncrementType.AddFactor, "{=At6b9vHF}{VALUE} relationship per day with a random notable over age 40 when party is in a town.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineGoodLodging.Initialize("{=RXo3edjn}Good Lodging", DefaultSkills.Medicine, this.GetTierCost(4), this._medicineBestMedicine, "{=NjMR2ypH}{VALUE}% healing rate while resting in settlements.", SkillEffect.PerkRole.Surgeon, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=ZH3U43xW}{VALUE} relationship per day with a random noble over age 40 when party is in a town.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineSiegeMedic.Initialize("{=ObwbbEqE}Siege Medic", DefaultSkills.Medicine, this.GetTierCost(5), this._medicineVeterinarian, "{=Gyy4rwnD}{VALUE}% chance of troops getting wounded instead of getting killed during siege bombardment.", SkillEffect.PerkRole.Surgeon, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=Nxh6aX2E}{VALUE}% chance to recover from lethal wounds during siege bombardment.", SkillEffect.PerkRole.Surgeon, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineVeterinarian.Initialize("{=DNPbZZPQ}Veterinarian", DefaultSkills.Medicine, this.GetTierCost(5), this._medicineSiegeMedic, "{=PZb8JrMH}{VALUE}% daily chance to recover a lame horse.", SkillEffect.PerkRole.Surgeon, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=GJRcFc0V}{VALUE}% chance to recover mounts of dead cavalry troops in battles.", SkillEffect.PerkRole.Surgeon, 0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicinePristineStreets.Initialize("{=72tbUfrz}Pristine Streets", DefaultSkills.Medicine, this.GetTierCost(6), this._medicineBushDoctor, "{=JMMVcpA0}{VALUE} settlement prosperity every day in governed settlements.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, "{=R9O0Y64L}{VALUE}% party healing rate while waiting in towns.", SkillEffect.PerkRole.Surgeon, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineBushDoctor.Initialize("{=HGrsb7k2}Bush Doctor", DefaultSkills.Medicine, this.GetTierCost(6), this._medicinePristineStreets, "{=ULY7byYc}{VALUE}% hearth growth in villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=UaKTuz1l}{VALUE}% party healing rate while waiting in villages.", SkillEffect.PerkRole.Surgeon, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicinePerfectHealth.Initialize("{=cGuPMx4p}Perfect Health", DefaultSkills.Medicine, this.GetTierCost(7), this._medicineHealthAdvise, "{=1yqMERf2}{VALUE}% recovery rate for each type of food in party inventory.", SkillEffect.PerkRole.Surgeon, 0.05f, SkillEffect.EffectIncrementType.AddFactor, "{=QsMEML5E}{VALUE}% animal production rate in villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineHealthAdvise.Initialize("{=NxcvQlAk}Health Advice", DefaultSkills.Medicine, this.GetTierCost(7), this._medicinePerfectHealth, "{=uRvym4tq}Chance of recovery from death due to old age for every clan member.", SkillEffect.PerkRole.ClanLeader, 0f, SkillEffect.EffectIncrementType.AddFactor, "{=ioYR1Grc}Wounded troops do not decrease morale in battles.", SkillEffect.PerkRole.Surgeon, 0f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicinePhysicianOfPeople.Initialize("{=5o6pSbCx}Physician of People", DefaultSkills.Medicine, this.GetTierCost(8), this._medicineCleanInfrastructure, "{=F7bbkYx4}{VALUE} loyalty per day in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, "{=bNsaUb42}{VALUE}% chance to recover from lethal wounds for tier 1 and 2 troops", SkillEffect.PerkRole.Surgeon, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineCleanInfrastructure.Initialize("{=CZ4y5NAf}Clean Infrastructure", DefaultSkills.Medicine, this.GetTierCost(8), this._medicinePhysicianOfPeople, "{=S9XsuYap}{VALUE} prosperity bonus from civilian projects in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, "{=dYyFWmGB}{VALUE}% recovery rate from raids in villages bound to the governed settlement.", SkillEffect.PerkRole.Governor, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineCheatDeath.Initialize("{=cpg0oHZJ}Cheat Death", DefaultSkills.Medicine, this.GetTierCost(9), this._medicineFortitudeTonic, "{=n2xL3okw}Cheat death due to old age once.", SkillEffect.PerkRole.Personal, 0f, SkillEffect.EffectIncrementType.Add, "{=b1IKTI8t}{VALUE}% chance to die when you fall unconscious in battle.", SkillEffect.PerkRole.Surgeon, -0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineFortitudeTonic.Initialize("{=ib2SMG9b}Fortitude Tonic", DefaultSkills.Medicine, this.GetTierCost(9), this._medicineCheatDeath, "{=v9NohO6l}{VALUE} hit points to other heroes in your party.", SkillEffect.PerkRole.PartyLeader, 10f, SkillEffect.EffectIncrementType.Add, "{=Ti9auMiO}{VALUE} hit points.", SkillEffect.PerkRole.Personal, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineHelpingHands.Initialize("{=KavZKNaa}Helping Hands", DefaultSkills.Medicine, this.GetTierCost(10), this._medicineBattleHardened, "{=6NOzUcGN}{VALUE}% troop recovery rate for every 10 troop in your party.", SkillEffect.PerkRole.Surgeon, 0.02f, SkillEffect.EffectIncrementType.AddFactor, "{=iHuzmdm2}{VALUE}% prosperity loss from starvation.", SkillEffect.PerkRole.Governor, -0.5f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineBattleHardened.Initialize("{=oSbRD72H}Battle Hardened", DefaultSkills.Medicine, this.GetTierCost(10), this._medicineHelpingHands, "{=qWpabhp6}{VALUE} experience to wounded units at the end of the battle.", SkillEffect.PerkRole.Surgeon, 25f, SkillEffect.EffectIncrementType.Add, "{=3tLU4AG7}{VALUE}% siege attrition loss in the governed settlement.", SkillEffect.PerkRole.Governor, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._medicineMinisterOfHealth.Initialize("{=rtTjuJTc}Minister of Health", DefaultSkills.Medicine, this.GetTierCost(11), null, "{=cwFyqrfv}{VALUE} hit point to troops for every skill point above 250.", SkillEffect.PerkRole.Personal, 1f, SkillEffect.EffectIncrementType.Add, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringScaffolds.Initialize("{=ekavTnTp}Scaffolds", DefaultSkills.Engineering, this.GetTierCost(1), this._engineeringTorsionEngines, "{=2WC42D5D}{VALUE}% build speed to non-ranged siege engines.", SkillEffect.PerkRole.Engineer, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=F1CJo2wX}{VALUE}% shield hitpoints.", SkillEffect.PerkRole.Personal, 0.3f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringTorsionEngines.Initialize("{=57TDG2Ta}Torsion Engines", DefaultSkills.Engineering, this.GetTierCost(1), this._engineeringScaffolds, "{=hv18SprX}{VALUE}% build speed to ranged siege engines.", SkillEffect.PerkRole.Engineer, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=aA8T7AsY}{VALUE} damage to equipped crossbows.", SkillEffect.PerkRole.Personal, 3f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringSiegeWorks.Initialize("{=Nr1GPYSr}Siegeworks", DefaultSkills.Engineering, this.GetTierCost(2), this._engineeringDungeonArchitect, "{=oOZH3v9Y}{VALUE}% hit points to ranged siege engines.", SkillEffect.PerkRole.Engineer, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=pIFOcikU}{VALUE} prebuilt catapult to the settlement when a siege starts in the governed settlement.", SkillEffect.PerkRole.Governor, 1f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringDungeonArchitect.Initialize("{=aPbpBJq5}Dungeon Architect", DefaultSkills.Engineering, this.GetTierCost(2), this._engineeringSiegeWorks, "{=KK3DAGej}{VALUE}% chance of ranged siege engines getting hit while under bombardment.", SkillEffect.PerkRole.Engineer, -0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=ako4Xbvk}{VALUE}% escape chance to prisoners in dungeons of governed settlements.", SkillEffect.PerkRole.Governor, -0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringCarpenters.Initialize("{=YwhAlz5n}Carpenters", DefaultSkills.Engineering, this.GetTierCost(3), this._engineeringMilitaryPlanner, "{=cXCbpPqS}{VALUE}% hit points to rams and siege-towers.", SkillEffect.PerkRole.Engineer, 0.33f, SkillEffect.EffectIncrementType.AddFactor, "{=lVp2bwR9}{VALUE}% build speed for projects in the governed town.", SkillEffect.PerkRole.Governor, 0.12f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringMilitaryPlanner.Initialize("{=mzDsT7lV}Military Planner", DefaultSkills.Engineering, this.GetTierCost(3), this._engineeringCarpenters, "{=zU6gKebE}{VALUE}% ammunition to ranged troops when besieging.", SkillEffect.PerkRole.Engineer, 0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=xZqVL9wN}{VALUE}% build speed for projects in the governed castle.", SkillEffect.PerkRole.Governor, 0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringWallBreaker.Initialize("{=0wlWgIeL}Wall Breaker", DefaultSkills.Engineering, this.GetTierCost(4), this._engineeringDreadfulSieger, "{=JBa4DO2u}{VALUE}% damage dealt to walls during siege bombardment.", SkillEffect.PerkRole.Engineer, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=g3SKNcMV}{VALUE}% damage dealt to shields by troops in your formation.", SkillEffect.PerkRole.Captain, 0.1f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._engineeringDreadfulSieger.Initialize("{=bIS4kqmf}Dreadful Besieger", DefaultSkills.Engineering, this.GetTierCost(4), this._engineeringWallBreaker, "{=zUzfRYzf}{VALUE}% accuracy to your siege engines during siege bombardments in the governed settlement.", SkillEffect.PerkRole.Governor, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=cD8a5zbZ}{VALUE}% crossbow damage by troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.CrossbowUser);
			this._engineeringSalvager.Initialize("{=AgJAfEEZ}Salvager", DefaultSkills.Engineering, this.GetTierCost(5), this._engineeringForeman, "{=mtb8vJ4o}{VALUE}% accuracy to ballistas during siege bombardment.", SkillEffect.PerkRole.Engineer, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=qfjgKCty}{VALUE}% siege engine build speed increase for each militia.", SkillEffect.PerkRole.Governor, 0.001f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringForeman.Initialize("{=3ML4EkWY}Foreman", DefaultSkills.Engineering, this.GetTierCost(5), this._engineeringSalvager, "{=M4IaRQJy}{VALUE}% mangonel and trebuchet accuracy during siege bombardment.", SkillEffect.PerkRole.Engineer, 0.1f, SkillEffect.EffectIncrementType.AddFactor, "{=ivrmsCFC}{VALUE} prosperity when a project is finished in the governed settlement.", SkillEffect.PerkRole.Governor, 100f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringStonecutters.Initialize("{=auIRGa2V}Stonecutters", DefaultSkills.Engineering, this.GetTierCost(6), this._engineeringSiegeEngineer, "{=uohYIaSw}{VALUE}% build speed for fortifications, aqueducts and barrack projects in the governed settlement.", SkillEffect.PerkRole.Governor, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=uakMSJY6}Fire versions of siege engines can be constructed.", SkillEffect.PerkRole.Engineer, 0f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringSiegeEngineer.Initialize("{=pFGhJxyN}Siege Engineer", DefaultSkills.Engineering, this.GetTierCost(6), this._engineeringStonecutters, "{=cRfa2IaT}{VALUE}% hit points to defensive siege engines in the governed settlement.", SkillEffect.PerkRole.Governor, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=uakMSJY6}Fire versions of siege engines can be constructed.", SkillEffect.PerkRole.Engineer, 0f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringCampBuilding.Initialize("{=Lv2pbg8c}Camp Building", DefaultSkills.Engineering, this.GetTierCost(7), this._engineeringBattlements, "{=fDSyE0eE}{VALUE}% cohesion loss of armies when besieging.", SkillEffect.PerkRole.ArmyCommander, -0.5f, SkillEffect.EffectIncrementType.AddFactor, "{=0T7AKmVS}{VALUE}% casualty chance from siege bombardments.", SkillEffect.PerkRole.Engineer, -0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringBattlements.Initialize("{=hHHEW1HN}Battlements", DefaultSkills.Engineering, this.GetTierCost(7), this._engineeringCampBuilding, "{=Ix98dg08}{VALUE} prebuilt ballista when you set up a siege camp.", SkillEffect.PerkRole.Engineer, 1f, SkillEffect.EffectIncrementType.Add, "{=hXqSlJM7}{VALUE} maximum granary capacity in the governed settlement.", SkillEffect.PerkRole.Governor, 100f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringEngineeringGuilds.Initialize("{=elKQc0O6}Engineering Guilds", DefaultSkills.Engineering, this.GetTierCost(8), this._engineeringApprenticeship, "{=KAozuVLa}{VALUE} recruitment slot when recruiting from artisan notables.", SkillEffect.PerkRole.Engineer, 1f, SkillEffect.EffectIncrementType.Add, "{=EIkzYco9}{VALUE}% wall hit points in the governed settlement.", SkillEffect.PerkRole.Governor, 0.25f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringApprenticeship.Initialize("{=yzybG5rl}Apprenticeship", DefaultSkills.Engineering, this.GetTierCost(8), this._engineeringEngineeringGuilds, "{=3m2tQF9F}{VALUE} experience to troops when a siege engine is built.", SkillEffect.PerkRole.Engineer, 5f, SkillEffect.EffectIncrementType.Add, "{=AeTSNsRu}{VALUE}% prosperity gain for each unique project in the governed settlement.", SkillEffect.PerkRole.Governor, 0.01f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringMetallurgy.Initialize("{=qjvDsu8u}Metallurgy", DefaultSkills.Engineering, this.GetTierCost(9), this._engineeringImprovedTools, "{=ZMVo5TTq}{VALUE}% chance to remove negative modifiers on looted items.", SkillEffect.PerkRole.Engineer, 0.3f, SkillEffect.EffectIncrementType.AddFactor, "{=XWPcZgM9}{VALUE} armor to all equipped armor pieces of troops in your formation.", SkillEffect.PerkRole.Captain, 5f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.None);
			this._engineeringImprovedTools.Initialize("{=XixNAaD5}Improved Tools", DefaultSkills.Engineering, this.GetTierCost(9), this._engineeringMetallurgy, "{=5ATpHJag}{VALUE}% siege camp preparation speed.", SkillEffect.PerkRole.Engineer, 0.2f, SkillEffect.EffectIncrementType.AddFactor, "{=eBmaa49a}{VALUE}% melee damage by troops in your formation.", SkillEffect.PerkRole.Captain, 0.05f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Melee);
			this._engineeringClockwork.Initialize("{=Z9Rey6LC}Clockwork", DefaultSkills.Engineering, this.GetTierCost(10), this._engineeringArchitecturalCommisions, "{=yn9GhVK4}{VALUE}% reload speed to ballistas during siege bombardment.", SkillEffect.PerkRole.Engineer, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=Jlmtufb3}{VALUE}% effect from boosting projects in the governed town.", SkillEffect.PerkRole.Governor, 0.2f, SkillEffect.EffectIncrementType.AddFactor, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringArchitecturalCommisions.Initialize("{=KODafKT7}Architectural Commissions", DefaultSkills.Engineering, this.GetTierCost(10), this._engineeringClockwork, "{=0aMHHQL4}{VALUE}% reload speed to mangonels and trebuchets in siege bombardment.", SkillEffect.PerkRole.Engineer, 0.25f, SkillEffect.EffectIncrementType.AddFactor, "{=e3ykBSpR}{VALUE} gold per day for continuous projects in the governed settlement.", SkillEffect.PerkRole.Governor, 20f, SkillEffect.EffectIncrementType.Add, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
			this._engineeringMasterwork.Initialize("{=SNsAlN4R}Masterwork", DefaultSkills.Engineering, this.GetTierCost(11), null, "{=RP2Jn3J4}{VALUE}% damage for each engineering skill point over 250 for siege engines in siege bombardment.", SkillEffect.PerkRole.Engineer, 0.01f, SkillEffect.EffectIncrementType.AddFactor, "", SkillEffect.PerkRole.None, 0f, SkillEffect.EffectIncrementType.Invalid, TroopUsageFlags.Undefined, TroopUsageFlags.Undefined);
		}

		private int GetTierCost(int tierIndex)
		{
			return DefaultPerks.TierSkillRequirements[tierIndex - 1];
		}

		private PerkObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<PerkObject>(new PerkObject(stringId));
		}

		private static readonly int[] TierSkillRequirements = new int[]
		{
			25, 50, 75, 100, 125, 150, 175, 200, 225, 250,
			275, 300
		};

		private PerkObject _oneHandedBasher;

		private PerkObject _oneHandedToBeBlunt;

		private PerkObject _oneHandedSteelCoreShields;

		private PerkObject _oneHandedFleetOfFoot;

		private PerkObject _oneHandedDeadlyPurpose;

		private PerkObject _oneHandedUnwaveringDefense;

		private PerkObject _oneHandedWrappedHandles;

		private PerkObject _oneHandedWayOfTheSword;

		private PerkObject _oneHandedPrestige;

		private PerkObject _oneHandedChinkInTheArmor;

		private PerkObject _oneHandedStandUnited;

		private PerkObject _oneHandedLeadByExample;

		private PerkObject _oneHandedMilitaryTradition;

		private PerkObject _oneHandedCorpsACorps;

		private PerkObject _oneHandedShieldWall;

		private PerkObject _oneHandedArrowCatcher;

		private PerkObject _oneHandedShieldBearer;

		private PerkObject _oneHandedTrainer;

		private PerkObject _oneHandedDuelist;

		private PerkObject _oneHandedSwiftStrike;

		private PerkObject _oneHandedCavalry;

		private PerkObject _twoHandedWoodChopper;

		private PerkObject _twoHandedWayOfTheGreatAxe;

		private PerkObject _twoHandedStrongGrip;

		private PerkObject _twoHandedOnTheEdge;

		private PerkObject _twoHandedHeadBasher;

		private PerkObject _twoHandedShowOfStrength;

		private PerkObject _twoHandedBeastSlayer;

		private PerkObject _twoHandedBaptisedInBlood;

		private PerkObject _twoHandedShieldBreaker;

		private PerkObject _twoHandedConfidence;

		private PerkObject _twoHandedBerserker;

		private PerkObject _twoHandedProjectileDeflection;

		private PerkObject _twoHandedTerror;

		private PerkObject _twoHandedHope;

		private PerkObject _twoHandedThickHides;

		private PerkObject _twoHandedRecklessCharge;

		private PerkObject _twoHandedBladeMaster;

		private PerkObject _twoHandedVandal;

		public PerkObject _polearmPikeman;

		public PerkObject _polearmCavalry;

		public PerkObject _polearmBraced;

		public PerkObject _polearmKeepAtBay;

		public PerkObject _polearmSwiftSwing;

		public PerkObject _polearmCleanThrust;

		public PerkObject _polearmFootwork;

		public PerkObject _polearmHardKnock;

		public PerkObject _polearmSteedKiller;

		public PerkObject _polearmLancer;

		public PerkObject _polearmGuards;

		public PerkObject _polearmSkewer;

		public PerkObject _polearmStandardBearer;

		public PerkObject _polearmPhalanx;

		public PerkObject _polearmHardyFrontline;

		public PerkObject _polearmDrills;

		public PerkObject _polearmSureFooted;

		public PerkObject _polearmUnstoppableForce;

		public PerkObject _polearmCounterweight;

		public PerkObject _polearmWayOfTheSpear;

		public PerkObject _polearmSharpenTheTip;

		public PerkObject _bowDeadAim;

		public PerkObject _bowBodkin;

		public PerkObject _bowRangersSwiftness;

		public PerkObject _bowRapidFire;

		public PerkObject _bowQuickAdjustments;

		public PerkObject _bowMerryMen;

		public PerkObject _bowMountedArchery;

		public PerkObject _bowTrainer;

		public PerkObject _bowStrongBows;

		public PerkObject _bowDiscipline;

		public PerkObject _bowHunterClan;

		public PerkObject _bowSkirmishPhaseMaster;

		public PerkObject _bowEagleEye;

		public PerkObject _bowBullsEye;

		public PerkObject _bowRenownedArcher;

		public PerkObject _bowHorseMaster;

		public PerkObject _bowDeepQuivers;

		public PerkObject _bowQuickDraw;

		public PerkObject _bowNockingPoint;

		public PerkObject _bowBowControl;

		public PerkObject _bowDeadshot;

		public PerkObject _crossbowMarksmen;

		public PerkObject _crossbowUnhorser;

		public PerkObject _crossbowWindWinder;

		public PerkObject _crossbowDonkeysSwiftness;

		public PerkObject _crossbowSheriff;

		public PerkObject _crossbowPeasantLeader;

		public PerkObject _crossbowRenownMarksmen;

		public PerkObject _crossbowFletcher;

		public PerkObject _crossbowPuncture;

		public PerkObject _crossbowLooseAndMove;

		public PerkObject _crossbowDeftHands;

		public PerkObject _crossbowCounterFire;

		public PerkObject _crossbowMountedCrossbowman;

		public PerkObject _crossbowSteady;

		public PerkObject _crossbowLongShots;

		public PerkObject _crossbowHammerBolts;

		public PerkObject _crossbowPavise;

		public PerkObject _crossbowTerror;

		public PerkObject _crossbowPickedShots;

		public PerkObject _crossbowPiercer;

		public PerkObject _crossbowMightyPull;

		private PerkObject _throwingShieldBreaker;

		private PerkObject _throwingHunter;

		private PerkObject _throwingFlexibleFighter;

		private PerkObject _throwingMountedSkirmisher;

		private PerkObject _throwingPerfectTechnique;

		private PerkObject _throwingRunningThrow;

		private PerkObject _throwingKnockOff;

		private PerkObject _throwingWellPrepared;

		private PerkObject _throwingSkirmisher;

		private PerkObject _throwingFocus;

		private PerkObject _throwingLastHit;

		private PerkObject _throwingHeadHunter;

		private PerkObject _throwingThrowingCompetitions;

		private PerkObject _throwingSaddlebags;

		private PerkObject _throwingSplinters;

		private PerkObject _throwingResourceful;

		private PerkObject _throwingLongReach;

		private PerkObject _throwingWeakSpot;

		private PerkObject _throwingQuickDraw;

		private PerkObject _throwingImpale;

		private PerkObject _throwingUnstoppableForce;

		private PerkObject _ridingNimbleSteed;

		private PerkObject _ridingWellStraped;

		private PerkObject _ridingVeterinary;

		private PerkObject _ridingNomadicTraditions;

		private PerkObject _ridingDeeperSacks;

		private PerkObject _ridingSagittarius;

		private PerkObject _ridingSweepingWind;

		private PerkObject _ridingReliefForce;

		private PerkObject _ridingMountedWarrior;

		private PerkObject _ridingHorseArcher;

		private PerkObject _ridingShepherd;

		private PerkObject _ridingBreeder;

		private PerkObject _ridingThunderousCharge;

		private PerkObject _ridingAnnoyingBuzz;

		private PerkObject _ridingMountedPatrols;

		private PerkObject _ridingCavalryTactics;

		private PerkObject _ridingDauntlessSteed;

		private PerkObject _ridingToughSteed;

		private PerkObject _ridingFullSpeed;

		private PerkObject _ridingTheWayOfTheSaddle;

		private PerkObject _athleticsFormFittingArmor;

		private PerkObject _athleticsImposingStature;

		private PerkObject _athleticsStamina;

		private PerkObject _athleticsSprint;

		private PerkObject _athleticsPowerful;

		private PerkObject _athleticsSurgingBlow;

		private PerkObject _athleticsWellBuilt;

		private PerkObject _athleticsFury;

		private PerkObject _athleticsBraced;

		private PerkObject _athleticsAGoodDaysRest;

		private PerkObject _athleticsDurable;

		private PerkObject _athleticsEnergetic;

		private PerkObject _athleticsSteady;

		private PerkObject _athleticsStrong;

		private PerkObject _athleticsStrongLegs;

		private PerkObject _athleticsStrongArms;

		private PerkObject _athleticsSpartan;

		private PerkObject _athleticsMorningExercise;

		private PerkObject _athleticsIgnorePain;

		private PerkObject _athleticsWalkItOff;

		private PerkObject _athleticsMightyBlow;

		private PerkObject _craftingSteelMaker2;

		private PerkObject _craftingSteelMaker3;

		private PerkObject _craftingCharcoalMaker;

		private PerkObject _craftingSteelMaker;

		private PerkObject _craftingCuriousSmelter;

		private PerkObject _craftingCuriousSmith;

		private PerkObject _craftingPracticalSmelter;

		private PerkObject _craftingPracticalRefiner;

		private PerkObject _craftingPracticalSmith;

		private PerkObject _craftingArtisanSmith;

		private PerkObject _craftingExperiencedSmith;

		private PerkObject _craftingMasterSmith;

		private PerkObject _craftingLegendarySmith;

		private PerkObject _craftingVigorousSmith;

		private PerkObject _craftingStrongSmith;

		private PerkObject _craftingEnduringSmith;

		private PerkObject _craftingIronMaker;

		private PerkObject _craftingFencerSmith;

		private PerkObject _craftingSharpenedEdge;

		private PerkObject _craftingSharpenedTip;

		private PerkObject _tacticsSmallUnitTactics;

		private PerkObject _tacticsHordeLeader;

		private PerkObject _tacticsLawKeeper;

		private PerkObject _tacticsLooseFormations;

		private PerkObject _tacticsSwiftRegroup;

		private PerkObject _tacticsExtendedSkirmish;

		private PerkObject _tacticsDecisiveBattle;

		private PerkObject _tacticsCoaching;

		private PerkObject _tacticsImproviser;

		private PerkObject _tacticsOnTheMarch;

		private PerkObject _tacticsCallToArms;

		private PerkObject _tacticsPickThemOfTheWalls;

		private PerkObject _tacticsMakeThemPay;

		private PerkObject _tacticsEliteReserves;

		private PerkObject _tacticsEncirclement;

		private PerkObject _tacticsPreBattleManeuvers;

		private PerkObject _tacticsBesieged;

		private PerkObject _tacticsCounteroffensive;

		private PerkObject _tacticsGensdarmes;

		private PerkObject _tacticsTightFormations;

		private PerkObject _tacticsTacticalMastery;

		private PerkObject _scoutingNightRunner;

		private PerkObject _scoutingWaterDiviner;

		private PerkObject _scoutingForestKin;

		private PerkObject _scoutingForcedMarch;

		private PerkObject _scoutingDesertBorn;

		private PerkObject _scoutingPathfinder;

		private PerkObject _scoutingUnburdened;

		private PerkObject _scoutingTracker;

		private PerkObject _scoutingRanger;

		private PerkObject _scoutingMountedScouts;

		private PerkObject _scoutingPatrols;

		private PerkObject _scoutingForagers;

		private PerkObject _scoutingBeastWhisperer;

		private PerkObject _scoutingVillageNetwork;

		private PerkObject _scoutingRumourNetwork;

		private PerkObject _scoutingVantagePoint;

		private PerkObject _scoutingKeenSight;

		private PerkObject _scoutingVanguard;

		private PerkObject _scoutingRearguard;

		private PerkObject _scoutingDayTraveler;

		private PerkObject _scoutingUncannyInsight;

		private PerkObject _rogueryTwoFaced;

		private PerkObject _rogueryDeepPockets;

		private PerkObject _rogueryInBestLight;

		private PerkObject _roguerySweetTalker;

		private PerkObject _rogueryKnowHow;

		private PerkObject _rogueryManhunter;

		private PerkObject _rogueryPromises;

		private PerkObject _rogueryScarface;

		private PerkObject _rogueryWhiteLies;

		private PerkObject _roguerySmugglerConnections;

		private PerkObject _rogueryPartnersInCrime;

		private PerkObject _rogueryOneOfTheFamily;

		private PerkObject _roguerySaltTheEarth;

		private PerkObject _rogueryCarver;

		private PerkObject _rogueryRansomBroker;

		private PerkObject _rogueryArmsDealer;

		private PerkObject _rogueryDirtyFighting;

		private PerkObject _rogueryDashAndSlash;

		private PerkObject _rogueryFleetFooted;

		private PerkObject _rogueryNoRestForTheWicked;

		private PerkObject _rogueryRogueExtraordinaire;

		private PerkObject _leadershipFerventAttacker;

		private PerkObject _leadershipStoutDefender;

		private PerkObject _leadershipAuthority;

		private PerkObject _leadershipHeroicLeader;

		private PerkObject _leadershipLoyaltyAndHonor;

		private PerkObject _leadershipFamousCommander;

		private PerkObject _leadershipRaiseTheMeek;

		private PerkObject _leadershipPresence;

		private PerkObject _leadershipVeteransRespect;

		private PerkObject _leadershipLeaderOfTheMasses;

		private PerkObject _leadershipInspiringLeader;

		private PerkObject _leadershipUpliftingSpirit;

		private PerkObject _leadershipMakeADifference;

		private PerkObject _leadershipLeadByExample;

		private PerkObject _leadershipTrustedCommander;

		private PerkObject _leadershipGreatLeader;

		private PerkObject _leadershipWePledgeOurSwords;

		private PerkObject _leadershipUltimateLeader;

		private PerkObject _leadershipTalentMagnet;

		private PerkObject _leadershipCitizenMilitia;

		private PerkObject _leadershipCombatTips;

		private PerkObject _charmVirile;

		private PerkObject _charmSelfPromoter;

		private PerkObject _charmOratory;

		private PerkObject _charmWarlord;

		private PerkObject _charmForgivableGrievances;

		private PerkObject _charmMeaningfulFavors;

		private PerkObject _charmInBloom;

		private PerkObject _charmYoungAndRespectful;

		private PerkObject _charmFirebrand;

		private PerkObject _charmFlexibleEthics;

		private PerkObject _charmEffortForThePeople;

		private PerkObject _charmSlickNegotiator;

		private PerkObject _charmGoodNatured;

		private PerkObject _charmTribute;

		private PerkObject _charmMoralLeader;

		private PerkObject _charmNaturalLeader;

		private PerkObject _charmPublicSpeaker;

		private PerkObject _charmParade;

		private PerkObject _charmCamaraderie;

		private PerkObject _charmImmortalCharm;

		private PerkObject _tradeTravelingRumors;

		private PerkObject _tradeLocalConnection;

		private PerkObject _tradeDistributedGoods;

		private PerkObject _tradeTollgates;

		private PerkObject _tradeArtisanCommunity;

		private PerkObject _tradeGreatInvestor;

		private PerkObject _tradeMercenaryConnections;

		private PerkObject _tradeContentTrades;

		private PerkObject _tradeInsurancePlans;

		private PerkObject _tradeRapidDevelopment;

		private PerkObject _tradeGranaryAccountant;

		private PerkObject _tradeTradeyardForeman;

		private PerkObject _tradeWholeSeller;

		private PerkObject _tradeCaravanMaster;

		private PerkObject _tradeMarketDealer;

		private PerkObject _tradeSwordForBarter;

		private PerkObject _tradeTrickleDown;

		private PerkObject _tradeManOfMeans;

		private PerkObject _tradeSpringOfGold;

		private PerkObject _tradeSilverTongue;

		private PerkObject _tradeSelfMadeMan;

		private PerkObject _tradeAppraiser;

		private PerkObject _tradeEverythingHasAPrice;

		private PerkObject _medicinePreventiveMedicine;

		private PerkObject _medicineTriageTent;

		private PerkObject _medicineWalkItOff;

		private PerkObject _medicineSledges;

		private PerkObject _medicineDoctorsOath;

		private PerkObject _medicineBestMedicine;

		private PerkObject _medicineGoodLodging;

		private PerkObject _medicineSiegeMedic;

		private PerkObject _medicineVeterinarian;

		private PerkObject _medicinePristineStreets;

		private PerkObject _medicineBushDoctor;

		private PerkObject _medicinePerfectHealth;

		private PerkObject _medicineHealthAdvise;

		private PerkObject _medicinePhysicianOfPeople;

		private PerkObject _medicineCleanInfrastructure;

		private PerkObject _medicineCheatDeath;

		private PerkObject _medicineHelpingHands;

		private PerkObject _medicineFortitudeTonic;

		private PerkObject _medicineBattleHardened;

		private PerkObject _medicineMinisterOfHealth;

		private PerkObject _medicineSelfMedication;

		private PerkObject _stewardFrugal;

		private PerkObject _stewardSevenVeterans;

		private PerkObject _stewardDrillSergant;

		private PerkObject _stewardSweatshops;

		private PerkObject _stewardEfficientCampaigner;

		private PerkObject _stewardGivingHands;

		private PerkObject _stewardLogistician;

		private PerkObject _stewardStiffUpperLip;

		private PerkObject _stewardPaidInPromise;

		private PerkObject _stewardRelocation;

		private PerkObject _stewardAidCorps;

		private PerkObject _stewardGourmet;

		private PerkObject _stewardSoundReserves;

		private PerkObject _stewardArenicosMules;

		private PerkObject _stewardForcedLabor;

		private PerkObject _stewardPriceOfLoyalty;

		private PerkObject _stewardContractors;

		private PerkObject _stewardMasterOfWarcraft;

		private PerkObject _stewardMasterOfPlanning;

		private PerkObject _stewardWarriorsDiet;

		private PerkObject _stewardArenicosHorses;

		private PerkObject _engineeringSiegeWorks;

		private PerkObject _engineeringCarpenters;

		private PerkObject _engineeringDungeonArchitect;

		private PerkObject _engineeringMilitaryPlanner;

		private PerkObject _engineeringDreadfulSieger;

		private PerkObject _engineeringTorsionEngines;

		private PerkObject _engineeringSalvager;

		private PerkObject _engineeringForeman;

		private PerkObject _engineeringWallBreaker;

		private PerkObject _engineeringStonecutters;

		private PerkObject _engineeringSiegeEngineer;

		private PerkObject _engineeringCampBuilding;

		private PerkObject _engineeringBattlements;

		private PerkObject _engineeringEngineeringGuilds;

		private PerkObject _engineeringApprenticeship;

		private PerkObject _engineeringMetallurgy;

		private PerkObject _engineeringImprovedTools;

		private PerkObject _engineeringClockwork;

		private PerkObject _engineeringArchitecturalCommisions;

		private PerkObject _engineeringScaffolds;

		private PerkObject _engineeringMasterwork;

		public static class OneHanded
		{
			public static PerkObject WrappedHandles
			{
				get
				{
					return DefaultPerks.Instance._oneHandedWrappedHandles;
				}
			}

			public static PerkObject Basher
			{
				get
				{
					return DefaultPerks.Instance._oneHandedBasher;
				}
			}

			public static PerkObject ToBeBlunt
			{
				get
				{
					return DefaultPerks.Instance._oneHandedToBeBlunt;
				}
			}

			public static PerkObject SwiftStrike
			{
				get
				{
					return DefaultPerks.Instance._oneHandedSwiftStrike;
				}
			}

			public static PerkObject Cavalry
			{
				get
				{
					return DefaultPerks.Instance._oneHandedCavalry;
				}
			}

			public static PerkObject ShieldBearer
			{
				get
				{
					return DefaultPerks.Instance._oneHandedShieldBearer;
				}
			}

			public static PerkObject Trainer
			{
				get
				{
					return DefaultPerks.Instance._oneHandedTrainer;
				}
			}

			public static PerkObject Duelist
			{
				get
				{
					return DefaultPerks.Instance._oneHandedDuelist;
				}
			}

			public static PerkObject ShieldWall
			{
				get
				{
					return DefaultPerks.Instance._oneHandedShieldWall;
				}
			}

			public static PerkObject ArrowCatcher
			{
				get
				{
					return DefaultPerks.Instance._oneHandedArrowCatcher;
				}
			}

			public static PerkObject MilitaryTradition
			{
				get
				{
					return DefaultPerks.Instance._oneHandedMilitaryTradition;
				}
			}

			public static PerkObject CorpsACorps
			{
				get
				{
					return DefaultPerks.Instance._oneHandedCorpsACorps;
				}
			}

			public static PerkObject StandUnited
			{
				get
				{
					return DefaultPerks.Instance._oneHandedStandUnited;
				}
			}

			public static PerkObject LeadByExample
			{
				get
				{
					return DefaultPerks.Instance._oneHandedLeadByExample;
				}
			}

			public static PerkObject SteelCoreShields
			{
				get
				{
					return DefaultPerks.Instance._oneHandedSteelCoreShields;
				}
			}

			public static PerkObject FleetOfFoot
			{
				get
				{
					return DefaultPerks.Instance._oneHandedFleetOfFoot;
				}
			}

			public static PerkObject DeadlyPurpose
			{
				get
				{
					return DefaultPerks.Instance._oneHandedDeadlyPurpose;
				}
			}

			public static PerkObject UnwaveringDefense
			{
				get
				{
					return DefaultPerks.Instance._oneHandedUnwaveringDefense;
				}
			}

			public static PerkObject Prestige
			{
				get
				{
					return DefaultPerks.Instance._oneHandedPrestige;
				}
			}

			public static PerkObject WayOfTheSword
			{
				get
				{
					return DefaultPerks.Instance._oneHandedWayOfTheSword;
				}
			}

			public static PerkObject ChinkInTheArmor
			{
				get
				{
					return DefaultPerks.Instance._oneHandedChinkInTheArmor;
				}
			}
		}

		public static class TwoHanded
		{
			public static PerkObject StrongGrip
			{
				get
				{
					return DefaultPerks.Instance._twoHandedStrongGrip;
				}
			}

			public static PerkObject WoodChopper
			{
				get
				{
					return DefaultPerks.Instance._twoHandedWoodChopper;
				}
			}

			public static PerkObject OnTheEdge
			{
				get
				{
					return DefaultPerks.Instance._twoHandedOnTheEdge;
				}
			}

			public static PerkObject HeadBasher
			{
				get
				{
					return DefaultPerks.Instance._twoHandedHeadBasher;
				}
			}

			public static PerkObject ShowOfStrength
			{
				get
				{
					return DefaultPerks.Instance._twoHandedShowOfStrength;
				}
			}

			public static PerkObject BaptisedInBlood
			{
				get
				{
					return DefaultPerks.Instance._twoHandedBaptisedInBlood;
				}
			}

			public static PerkObject BeastSlayer
			{
				get
				{
					return DefaultPerks.Instance._twoHandedBeastSlayer;
				}
			}

			public static PerkObject ShieldBreaker
			{
				get
				{
					return DefaultPerks.Instance._twoHandedShieldBreaker;
				}
			}

			public static PerkObject Confidence
			{
				get
				{
					return DefaultPerks.Instance._twoHandedConfidence;
				}
			}

			public static PerkObject Berserker
			{
				get
				{
					return DefaultPerks.Instance._twoHandedBerserker;
				}
			}

			public static PerkObject ProjectileDeflection
			{
				get
				{
					return DefaultPerks.Instance._twoHandedProjectileDeflection;
				}
			}

			public static PerkObject Terror
			{
				get
				{
					return DefaultPerks.Instance._twoHandedTerror;
				}
			}

			public static PerkObject Hope
			{
				get
				{
					return DefaultPerks.Instance._twoHandedHope;
				}
			}

			public static PerkObject RecklessCharge
			{
				get
				{
					return DefaultPerks.Instance._twoHandedRecklessCharge;
				}
			}

			public static PerkObject ThickHides
			{
				get
				{
					return DefaultPerks.Instance._twoHandedThickHides;
				}
			}

			public static PerkObject BladeMaster
			{
				get
				{
					return DefaultPerks.Instance._twoHandedBladeMaster;
				}
			}

			public static PerkObject Vandal
			{
				get
				{
					return DefaultPerks.Instance._twoHandedVandal;
				}
			}

			public static PerkObject WayOfTheGreatAxe
			{
				get
				{
					return DefaultPerks.Instance._twoHandedWayOfTheGreatAxe;
				}
			}
		}

		public static class Polearm
		{
			public static PerkObject Pikeman
			{
				get
				{
					return DefaultPerks.Instance._polearmPikeman;
				}
			}

			public static PerkObject Cavalry
			{
				get
				{
					return DefaultPerks.Instance._polearmCavalry;
				}
			}

			public static PerkObject Braced
			{
				get
				{
					return DefaultPerks.Instance._polearmBraced;
				}
			}

			public static PerkObject KeepAtBay
			{
				get
				{
					return DefaultPerks.Instance._polearmKeepAtBay;
				}
			}

			public static PerkObject SwiftSwing
			{
				get
				{
					return DefaultPerks.Instance._polearmSwiftSwing;
				}
			}

			public static PerkObject CleanThrust
			{
				get
				{
					return DefaultPerks.Instance._polearmCleanThrust;
				}
			}

			public static PerkObject Footwork
			{
				get
				{
					return DefaultPerks.Instance._polearmFootwork;
				}
			}

			public static PerkObject HardKnock
			{
				get
				{
					return DefaultPerks.Instance._polearmHardKnock;
				}
			}

			public static PerkObject SteedKiller
			{
				get
				{
					return DefaultPerks.Instance._polearmSteedKiller;
				}
			}

			public static PerkObject Lancer
			{
				get
				{
					return DefaultPerks.Instance._polearmLancer;
				}
			}

			public static PerkObject Skewer
			{
				get
				{
					return DefaultPerks.Instance._polearmSkewer;
				}
			}

			public static PerkObject Guards
			{
				get
				{
					return DefaultPerks.Instance._polearmGuards;
				}
			}

			public static PerkObject StandardBearer
			{
				get
				{
					return DefaultPerks.Instance._polearmStandardBearer;
				}
			}

			public static PerkObject Phalanx
			{
				get
				{
					return DefaultPerks.Instance._polearmPhalanx;
				}
			}

			public static PerkObject HardyFrontline
			{
				get
				{
					return DefaultPerks.Instance._polearmHardyFrontline;
				}
			}

			public static PerkObject Drills
			{
				get
				{
					return DefaultPerks.Instance._polearmDrills;
				}
			}

			public static PerkObject SureFooted
			{
				get
				{
					return DefaultPerks.Instance._polearmSureFooted;
				}
			}

			public static PerkObject UnstoppableForce
			{
				get
				{
					return DefaultPerks.Instance._polearmUnstoppableForce;
				}
			}

			public static PerkObject CounterWeight
			{
				get
				{
					return DefaultPerks.Instance._polearmCounterweight;
				}
			}

			public static PerkObject SharpenTheTip
			{
				get
				{
					return DefaultPerks.Instance._polearmSharpenTheTip;
				}
			}

			public static PerkObject WayOfTheSpear
			{
				get
				{
					return DefaultPerks.Instance._polearmWayOfTheSpear;
				}
			}
		}

		public static class Bow
		{
			public static PerkObject BowControl
			{
				get
				{
					return DefaultPerks.Instance._bowBowControl;
				}
			}

			public static PerkObject DeadAim
			{
				get
				{
					return DefaultPerks.Instance._bowDeadAim;
				}
			}

			public static PerkObject Bodkin
			{
				get
				{
					return DefaultPerks.Instance._bowBodkin;
				}
			}

			public static PerkObject RangersSwiftness
			{
				get
				{
					return DefaultPerks.Instance._bowRangersSwiftness;
				}
			}

			public static PerkObject RapidFire
			{
				get
				{
					return DefaultPerks.Instance._bowRapidFire;
				}
			}

			public static PerkObject QuickAdjustments
			{
				get
				{
					return DefaultPerks.Instance._bowQuickAdjustments;
				}
			}

			public static PerkObject MerryMen
			{
				get
				{
					return DefaultPerks.Instance._bowMerryMen;
				}
			}

			public static PerkObject MountedArchery
			{
				get
				{
					return DefaultPerks.Instance._bowMountedArchery;
				}
			}

			public static PerkObject Trainer
			{
				get
				{
					return DefaultPerks.Instance._bowTrainer;
				}
			}

			public static PerkObject StrongBows
			{
				get
				{
					return DefaultPerks.Instance._bowStrongBows;
				}
			}

			public static PerkObject Discipline
			{
				get
				{
					return DefaultPerks.Instance._bowDiscipline;
				}
			}

			public static PerkObject HunterClan
			{
				get
				{
					return DefaultPerks.Instance._bowHunterClan;
				}
			}

			public static PerkObject SkirmishPhaseMaster
			{
				get
				{
					return DefaultPerks.Instance._bowSkirmishPhaseMaster;
				}
			}

			public static PerkObject EagleEye
			{
				get
				{
					return DefaultPerks.Instance._bowEagleEye;
				}
			}

			public static PerkObject BullsEye
			{
				get
				{
					return DefaultPerks.Instance._bowBullsEye;
				}
			}

			public static PerkObject RenownedArcher
			{
				get
				{
					return DefaultPerks.Instance._bowRenownedArcher;
				}
			}

			public static PerkObject HorseMaster
			{
				get
				{
					return DefaultPerks.Instance._bowHorseMaster;
				}
			}

			public static PerkObject DeepQuivers
			{
				get
				{
					return DefaultPerks.Instance._bowDeepQuivers;
				}
			}

			public static PerkObject QuickDraw
			{
				get
				{
					return DefaultPerks.Instance._bowQuickDraw;
				}
			}

			public static PerkObject NockingPoint
			{
				get
				{
					return DefaultPerks.Instance._bowNockingPoint;
				}
			}

			public static PerkObject Deadshot
			{
				get
				{
					return DefaultPerks.Instance._bowDeadshot;
				}
			}
		}

		public static class Crossbow
		{
			public static PerkObject Piercer
			{
				get
				{
					return DefaultPerks.Instance._crossbowPiercer;
				}
			}

			public static PerkObject Marksmen
			{
				get
				{
					return DefaultPerks.Instance._crossbowMarksmen;
				}
			}

			public static PerkObject Unhorser
			{
				get
				{
					return DefaultPerks.Instance._crossbowUnhorser;
				}
			}

			public static PerkObject WindWinder
			{
				get
				{
					return DefaultPerks.Instance._crossbowWindWinder;
				}
			}

			public static PerkObject DonkeysSwiftness
			{
				get
				{
					return DefaultPerks.Instance._crossbowDonkeysSwiftness;
				}
			}

			public static PerkObject Sheriff
			{
				get
				{
					return DefaultPerks.Instance._crossbowSheriff;
				}
			}

			public static PerkObject PeasantLeader
			{
				get
				{
					return DefaultPerks.Instance._crossbowPeasantLeader;
				}
			}

			public static PerkObject RenownMarksmen
			{
				get
				{
					return DefaultPerks.Instance._crossbowRenownMarksmen;
				}
			}

			public static PerkObject Fletcher
			{
				get
				{
					return DefaultPerks.Instance._crossbowFletcher;
				}
			}

			public static PerkObject Puncture
			{
				get
				{
					return DefaultPerks.Instance._crossbowPuncture;
				}
			}

			public static PerkObject LooseAndMove
			{
				get
				{
					return DefaultPerks.Instance._crossbowLooseAndMove;
				}
			}

			public static PerkObject DeftHands
			{
				get
				{
					return DefaultPerks.Instance._crossbowDeftHands;
				}
			}

			public static PerkObject CounterFire
			{
				get
				{
					return DefaultPerks.Instance._crossbowCounterFire;
				}
			}

			public static PerkObject MountedCrossbowman
			{
				get
				{
					return DefaultPerks.Instance._crossbowMountedCrossbowman;
				}
			}

			public static PerkObject Steady
			{
				get
				{
					return DefaultPerks.Instance._crossbowSteady;
				}
			}

			public static PerkObject LongShots
			{
				get
				{
					return DefaultPerks.Instance._crossbowLongShots;
				}
			}

			public static PerkObject HammerBolts
			{
				get
				{
					return DefaultPerks.Instance._crossbowHammerBolts;
				}
			}

			public static PerkObject Pavise
			{
				get
				{
					return DefaultPerks.Instance._crossbowPavise;
				}
			}

			public static PerkObject Terror
			{
				get
				{
					return DefaultPerks.Instance._crossbowTerror;
				}
			}

			public static PerkObject PickedShots
			{
				get
				{
					return DefaultPerks.Instance._crossbowPickedShots;
				}
			}

			public static PerkObject MightyPull
			{
				get
				{
					return DefaultPerks.Instance._crossbowMightyPull;
				}
			}
		}

		public static class Throwing
		{
			public static PerkObject QuickDraw
			{
				get
				{
					return DefaultPerks.Instance._throwingQuickDraw;
				}
			}

			public static PerkObject ShieldBreaker
			{
				get
				{
					return DefaultPerks.Instance._throwingShieldBreaker;
				}
			}

			public static PerkObject Hunter
			{
				get
				{
					return DefaultPerks.Instance._throwingHunter;
				}
			}

			public static PerkObject FlexibleFighter
			{
				get
				{
					return DefaultPerks.Instance._throwingFlexibleFighter;
				}
			}

			public static PerkObject MountedSkirmisher
			{
				get
				{
					return DefaultPerks.Instance._throwingMountedSkirmisher;
				}
			}

			public static PerkObject PerfectTechnique
			{
				get
				{
					return DefaultPerks.Instance._throwingPerfectTechnique;
				}
			}

			public static PerkObject RunningThrow
			{
				get
				{
					return DefaultPerks.Instance._throwingRunningThrow;
				}
			}

			public static PerkObject KnockOff
			{
				get
				{
					return DefaultPerks.Instance._throwingKnockOff;
				}
			}

			public static PerkObject WellPrepared
			{
				get
				{
					return DefaultPerks.Instance._throwingWellPrepared;
				}
			}

			public static PerkObject Skirmisher
			{
				get
				{
					return DefaultPerks.Instance._throwingSkirmisher;
				}
			}

			public static PerkObject Focus
			{
				get
				{
					return DefaultPerks.Instance._throwingFocus;
				}
			}

			public static PerkObject LastHit
			{
				get
				{
					return DefaultPerks.Instance._throwingLastHit;
				}
			}

			public static PerkObject HeadHunter
			{
				get
				{
					return DefaultPerks.Instance._throwingHeadHunter;
				}
			}

			public static PerkObject ThrowingCompetitions
			{
				get
				{
					return DefaultPerks.Instance._throwingThrowingCompetitions;
				}
			}

			public static PerkObject Saddlebags
			{
				get
				{
					return DefaultPerks.Instance._throwingSaddlebags;
				}
			}

			public static PerkObject Splinters
			{
				get
				{
					return DefaultPerks.Instance._throwingSplinters;
				}
			}

			public static PerkObject Resourceful
			{
				get
				{
					return DefaultPerks.Instance._throwingResourceful;
				}
			}

			public static PerkObject LongReach
			{
				get
				{
					return DefaultPerks.Instance._throwingLongReach;
				}
			}

			public static PerkObject WeakSpot
			{
				get
				{
					return DefaultPerks.Instance._throwingWeakSpot;
				}
			}

			public static PerkObject Impale
			{
				get
				{
					return DefaultPerks.Instance._throwingImpale;
				}
			}

			public static PerkObject UnstoppableForce
			{
				get
				{
					return DefaultPerks.Instance._throwingUnstoppableForce;
				}
			}
		}

		public static class Riding
		{
			public static PerkObject FullSpeed
			{
				get
				{
					return DefaultPerks.Instance._ridingFullSpeed;
				}
			}

			public static PerkObject NimbleSteed
			{
				get
				{
					return DefaultPerks.Instance._ridingNimbleSteed;
				}
			}

			public static PerkObject WellStraped
			{
				get
				{
					return DefaultPerks.Instance._ridingWellStraped;
				}
			}

			public static PerkObject Veterinary
			{
				get
				{
					return DefaultPerks.Instance._ridingVeterinary;
				}
			}

			public static PerkObject NomadicTraditions
			{
				get
				{
					return DefaultPerks.Instance._ridingNomadicTraditions;
				}
			}

			public static PerkObject DeeperSacks
			{
				get
				{
					return DefaultPerks.Instance._ridingDeeperSacks;
				}
			}

			public static PerkObject Sagittarius
			{
				get
				{
					return DefaultPerks.Instance._ridingSagittarius;
				}
			}

			public static PerkObject SweepingWind
			{
				get
				{
					return DefaultPerks.Instance._ridingSweepingWind;
				}
			}

			public static PerkObject ReliefForce
			{
				get
				{
					return DefaultPerks.Instance._ridingReliefForce;
				}
			}

			public static PerkObject MountedWarrior
			{
				get
				{
					return DefaultPerks.Instance._ridingMountedWarrior;
				}
			}

			public static PerkObject HorseArcher
			{
				get
				{
					return DefaultPerks.Instance._ridingHorseArcher;
				}
			}

			public static PerkObject Shepherd
			{
				get
				{
					return DefaultPerks.Instance._ridingShepherd;
				}
			}

			public static PerkObject Breeder
			{
				get
				{
					return DefaultPerks.Instance._ridingBreeder;
				}
			}

			public static PerkObject ThunderousCharge
			{
				get
				{
					return DefaultPerks.Instance._ridingThunderousCharge;
				}
			}

			public static PerkObject AnnoyingBuzz
			{
				get
				{
					return DefaultPerks.Instance._ridingAnnoyingBuzz;
				}
			}

			public static PerkObject MountedPatrols
			{
				get
				{
					return DefaultPerks.Instance._ridingMountedPatrols;
				}
			}

			public static PerkObject CavalryTactics
			{
				get
				{
					return DefaultPerks.Instance._ridingCavalryTactics;
				}
			}

			public static PerkObject DauntlessSteed
			{
				get
				{
					return DefaultPerks.Instance._ridingDauntlessSteed;
				}
			}

			public static PerkObject ToughSteed
			{
				get
				{
					return DefaultPerks.Instance._ridingToughSteed;
				}
			}

			public static PerkObject TheWayOfTheSaddle
			{
				get
				{
					return DefaultPerks.Instance._ridingTheWayOfTheSaddle;
				}
			}
		}

		public static class Athletics
		{
			public static PerkObject MorningExercise
			{
				get
				{
					return DefaultPerks.Instance._athleticsMorningExercise;
				}
			}

			public static PerkObject WellBuilt
			{
				get
				{
					return DefaultPerks.Instance._athleticsWellBuilt;
				}
			}

			public static PerkObject Fury
			{
				get
				{
					return DefaultPerks.Instance._athleticsFury;
				}
			}

			public static PerkObject FormFittingArmor
			{
				get
				{
					return DefaultPerks.Instance._athleticsFormFittingArmor;
				}
			}

			public static PerkObject ImposingStature
			{
				get
				{
					return DefaultPerks.Instance._athleticsImposingStature;
				}
			}

			public static PerkObject Stamina
			{
				get
				{
					return DefaultPerks.Instance._athleticsStamina;
				}
			}

			public static PerkObject Sprint
			{
				get
				{
					return DefaultPerks.Instance._athleticsSprint;
				}
			}

			public static PerkObject Powerful
			{
				get
				{
					return DefaultPerks.Instance._athleticsPowerful;
				}
			}

			public static PerkObject SurgingBlow
			{
				get
				{
					return DefaultPerks.Instance._athleticsSurgingBlow;
				}
			}

			public static PerkObject Braced
			{
				get
				{
					return DefaultPerks.Instance._athleticsBraced;
				}
			}

			public static PerkObject WalkItOff
			{
				get
				{
					return DefaultPerks.Instance._athleticsWalkItOff;
				}
			}

			public static PerkObject AGoodDaysRest
			{
				get
				{
					return DefaultPerks.Instance._athleticsAGoodDaysRest;
				}
			}

			public static PerkObject Durable
			{
				get
				{
					return DefaultPerks.Instance._athleticsDurable;
				}
			}

			public static PerkObject Energetic
			{
				get
				{
					return DefaultPerks.Instance._athleticsEnergetic;
				}
			}

			public static PerkObject Steady
			{
				get
				{
					return DefaultPerks.Instance._athleticsSteady;
				}
			}

			public static PerkObject Strong
			{
				get
				{
					return DefaultPerks.Instance._athleticsStrong;
				}
			}

			public static PerkObject StrongLegs
			{
				get
				{
					return DefaultPerks.Instance._athleticsStrongLegs;
				}
			}

			public static PerkObject StrongArms
			{
				get
				{
					return DefaultPerks.Instance._athleticsStrongArms;
				}
			}

			public static PerkObject Spartan
			{
				get
				{
					return DefaultPerks.Instance._athleticsSpartan;
				}
			}

			public static PerkObject IgnorePain
			{
				get
				{
					return DefaultPerks.Instance._athleticsIgnorePain;
				}
			}

			public static PerkObject MightyBlow
			{
				get
				{
					return DefaultPerks.Instance._athleticsMightyBlow;
				}
			}
		}

		public static class Crafting
		{
			public static PerkObject IronMaker
			{
				get
				{
					return DefaultPerks.Instance._craftingIronMaker;
				}
			}

			public static PerkObject CharcoalMaker
			{
				get
				{
					return DefaultPerks.Instance._craftingCharcoalMaker;
				}
			}

			public static PerkObject SteelMaker
			{
				get
				{
					return DefaultPerks.Instance._craftingSteelMaker;
				}
			}

			public static PerkObject SteelMaker2
			{
				get
				{
					return DefaultPerks.Instance._craftingSteelMaker2;
				}
			}

			public static PerkObject SteelMaker3
			{
				get
				{
					return DefaultPerks.Instance._craftingSteelMaker3;
				}
			}

			public static PerkObject CuriousSmelter
			{
				get
				{
					return DefaultPerks.Instance._craftingCuriousSmelter;
				}
			}

			public static PerkObject CuriousSmith
			{
				get
				{
					return DefaultPerks.Instance._craftingCuriousSmith;
				}
			}

			public static PerkObject PracticalRefiner
			{
				get
				{
					return DefaultPerks.Instance._craftingPracticalRefiner;
				}
			}

			public static PerkObject PracticalSmelter
			{
				get
				{
					return DefaultPerks.Instance._craftingPracticalSmelter;
				}
			}

			public static PerkObject PracticalSmith
			{
				get
				{
					return DefaultPerks.Instance._craftingPracticalSmith;
				}
			}

			public static PerkObject ArtisanSmith
			{
				get
				{
					return DefaultPerks.Instance._craftingArtisanSmith;
				}
			}

			public static PerkObject ExperiencedSmith
			{
				get
				{
					return DefaultPerks.Instance._craftingExperiencedSmith;
				}
			}

			public static PerkObject MasterSmith
			{
				get
				{
					return DefaultPerks.Instance._craftingMasterSmith;
				}
			}

			public static PerkObject LegendarySmith
			{
				get
				{
					return DefaultPerks.Instance._craftingLegendarySmith;
				}
			}

			public static PerkObject VigorousSmith
			{
				get
				{
					return DefaultPerks.Instance._craftingVigorousSmith;
				}
			}

			public static PerkObject StrongSmith
			{
				get
				{
					return DefaultPerks.Instance._craftingStrongSmith;
				}
			}

			public static PerkObject EnduringSmith
			{
				get
				{
					return DefaultPerks.Instance._craftingEnduringSmith;
				}
			}

			public static PerkObject WeaponMasterSmith
			{
				get
				{
					return DefaultPerks.Instance._craftingFencerSmith;
				}
			}

			public static PerkObject SharpenedEdge
			{
				get
				{
					return DefaultPerks.Instance._craftingSharpenedEdge;
				}
			}

			public static PerkObject SharpenedTip
			{
				get
				{
					return DefaultPerks.Instance._craftingSharpenedTip;
				}
			}
		}

		public static class Scouting
		{
			public static PerkObject DayTraveler
			{
				get
				{
					return DefaultPerks.Instance._scoutingDayTraveler;
				}
			}

			public static PerkObject Pathfinder
			{
				get
				{
					return DefaultPerks.Instance._scoutingPathfinder;
				}
			}

			public static PerkObject NightRunner
			{
				get
				{
					return DefaultPerks.Instance._scoutingNightRunner;
				}
			}

			public static PerkObject WaterDiviner
			{
				get
				{
					return DefaultPerks.Instance._scoutingWaterDiviner;
				}
			}

			public static PerkObject ForestKin
			{
				get
				{
					return DefaultPerks.Instance._scoutingForestKin;
				}
			}

			public static PerkObject DesertBorn
			{
				get
				{
					return DefaultPerks.Instance._scoutingDesertBorn;
				}
			}

			public static PerkObject ForcedMarch
			{
				get
				{
					return DefaultPerks.Instance._scoutingForcedMarch;
				}
			}

			public static PerkObject Unburdened
			{
				get
				{
					return DefaultPerks.Instance._scoutingUnburdened;
				}
			}

			public static PerkObject Tracker
			{
				get
				{
					return DefaultPerks.Instance._scoutingTracker;
				}
			}

			public static PerkObject Ranger
			{
				get
				{
					return DefaultPerks.Instance._scoutingRanger;
				}
			}

			public static PerkObject MountedScouts
			{
				get
				{
					return DefaultPerks.Instance._scoutingMountedScouts;
				}
			}

			public static PerkObject Patrols
			{
				get
				{
					return DefaultPerks.Instance._scoutingPatrols;
				}
			}

			public static PerkObject Foragers
			{
				get
				{
					return DefaultPerks.Instance._scoutingForagers;
				}
			}

			public static PerkObject BeastWhisperer
			{
				get
				{
					return DefaultPerks.Instance._scoutingBeastWhisperer;
				}
			}

			public static PerkObject VillageNetwork
			{
				get
				{
					return DefaultPerks.Instance._scoutingVillageNetwork;
				}
			}

			public static PerkObject RumourNetwork
			{
				get
				{
					return DefaultPerks.Instance._scoutingRumourNetwork;
				}
			}

			public static PerkObject VantagePoint
			{
				get
				{
					return DefaultPerks.Instance._scoutingVantagePoint;
				}
			}

			public static PerkObject KeenSight
			{
				get
				{
					return DefaultPerks.Instance._scoutingKeenSight;
				}
			}

			public static PerkObject Vanguard
			{
				get
				{
					return DefaultPerks.Instance._scoutingVanguard;
				}
			}

			public static PerkObject Rearguard
			{
				get
				{
					return DefaultPerks.Instance._scoutingRearguard;
				}
			}

			public static PerkObject UncannyInsight
			{
				get
				{
					return DefaultPerks.Instance._scoutingUncannyInsight;
				}
			}
		}

		public static class Tactics
		{
			public static PerkObject TightFormations
			{
				get
				{
					return DefaultPerks.Instance._tacticsTightFormations;
				}
			}

			public static PerkObject LooseFormations
			{
				get
				{
					return DefaultPerks.Instance._tacticsLooseFormations;
				}
			}

			public static PerkObject ExtendedSkirmish
			{
				get
				{
					return DefaultPerks.Instance._tacticsExtendedSkirmish;
				}
			}

			public static PerkObject DecisiveBattle
			{
				get
				{
					return DefaultPerks.Instance._tacticsDecisiveBattle;
				}
			}

			public static PerkObject SmallUnitTactics
			{
				get
				{
					return DefaultPerks.Instance._tacticsSmallUnitTactics;
				}
			}

			public static PerkObject HordeLeader
			{
				get
				{
					return DefaultPerks.Instance._tacticsHordeLeader;
				}
			}

			public static PerkObject LawKeeper
			{
				get
				{
					return DefaultPerks.Instance._tacticsLawKeeper;
				}
			}

			public static PerkObject Coaching
			{
				get
				{
					return DefaultPerks.Instance._tacticsCoaching;
				}
			}

			public static PerkObject SwiftRegroup
			{
				get
				{
					return DefaultPerks.Instance._tacticsSwiftRegroup;
				}
			}

			public static PerkObject Improviser
			{
				get
				{
					return DefaultPerks.Instance._tacticsImproviser;
				}
			}

			public static PerkObject OnTheMarch
			{
				get
				{
					return DefaultPerks.Instance._tacticsOnTheMarch;
				}
			}

			public static PerkObject CallToArms
			{
				get
				{
					return DefaultPerks.Instance._tacticsCallToArms;
				}
			}

			public static PerkObject PickThemOfTheWalls
			{
				get
				{
					return DefaultPerks.Instance._tacticsPickThemOfTheWalls;
				}
			}

			public static PerkObject MakeThemPay
			{
				get
				{
					return DefaultPerks.Instance._tacticsMakeThemPay;
				}
			}

			public static PerkObject EliteReserves
			{
				get
				{
					return DefaultPerks.Instance._tacticsEliteReserves;
				}
			}

			public static PerkObject Encirclement
			{
				get
				{
					return DefaultPerks.Instance._tacticsEncirclement;
				}
			}

			public static PerkObject PreBattleManeuvers
			{
				get
				{
					return DefaultPerks.Instance._tacticsPreBattleManeuvers;
				}
			}

			public static PerkObject Besieged
			{
				get
				{
					return DefaultPerks.Instance._tacticsBesieged;
				}
			}

			public static PerkObject Counteroffensive
			{
				get
				{
					return DefaultPerks.Instance._tacticsCounteroffensive;
				}
			}

			public static PerkObject Gensdarmes
			{
				get
				{
					return DefaultPerks.Instance._tacticsGensdarmes;
				}
			}

			public static PerkObject TacticalMastery
			{
				get
				{
					return DefaultPerks.Instance._tacticsTacticalMastery;
				}
			}
		}

		public static class Roguery
		{
			public static PerkObject NoRestForTheWicked
			{
				get
				{
					return DefaultPerks.Instance._rogueryNoRestForTheWicked;
				}
			}

			public static PerkObject SweetTalker
			{
				get
				{
					return DefaultPerks.Instance._roguerySweetTalker;
				}
			}

			public static PerkObject TwoFaced
			{
				get
				{
					return DefaultPerks.Instance._rogueryTwoFaced;
				}
			}

			public static PerkObject DeepPockets
			{
				get
				{
					return DefaultPerks.Instance._rogueryDeepPockets;
				}
			}

			public static PerkObject InBestLight
			{
				get
				{
					return DefaultPerks.Instance._rogueryInBestLight;
				}
			}

			public static PerkObject KnowHow
			{
				get
				{
					return DefaultPerks.Instance._rogueryKnowHow;
				}
			}

			public static PerkObject Promises
			{
				get
				{
					return DefaultPerks.Instance._rogueryPromises;
				}
			}

			public static PerkObject Manhunter
			{
				get
				{
					return DefaultPerks.Instance._rogueryManhunter;
				}
			}

			public static PerkObject Scarface
			{
				get
				{
					return DefaultPerks.Instance._rogueryScarface;
				}
			}

			public static PerkObject WhiteLies
			{
				get
				{
					return DefaultPerks.Instance._rogueryWhiteLies;
				}
			}

			public static PerkObject SmugglerConnections
			{
				get
				{
					return DefaultPerks.Instance._roguerySmugglerConnections;
				}
			}

			public static PerkObject PartnersInCrime
			{
				get
				{
					return DefaultPerks.Instance._rogueryPartnersInCrime;
				}
			}

			public static PerkObject OneOfTheFamily
			{
				get
				{
					return DefaultPerks.Instance._rogueryOneOfTheFamily;
				}
			}

			public static PerkObject SaltTheEarth
			{
				get
				{
					return DefaultPerks.Instance._roguerySaltTheEarth;
				}
			}

			public static PerkObject Carver
			{
				get
				{
					return DefaultPerks.Instance._rogueryCarver;
				}
			}

			public static PerkObject RansomBroker
			{
				get
				{
					return DefaultPerks.Instance._rogueryRansomBroker;
				}
			}

			public static PerkObject ArmsDealer
			{
				get
				{
					return DefaultPerks.Instance._rogueryArmsDealer;
				}
			}

			public static PerkObject DirtyFighting
			{
				get
				{
					return DefaultPerks.Instance._rogueryDirtyFighting;
				}
			}

			public static PerkObject DashAndSlash
			{
				get
				{
					return DefaultPerks.Instance._rogueryDashAndSlash;
				}
			}

			public static PerkObject FleetFooted
			{
				get
				{
					return DefaultPerks.Instance._rogueryFleetFooted;
				}
			}

			public static PerkObject RogueExtraordinaire
			{
				get
				{
					return DefaultPerks.Instance._rogueryRogueExtraordinaire;
				}
			}
		}

		public static class Charm
		{
			public static PerkObject Virile
			{
				get
				{
					return DefaultPerks.Instance._charmVirile;
				}
			}

			public static PerkObject SelfPromoter
			{
				get
				{
					return DefaultPerks.Instance._charmSelfPromoter;
				}
			}

			public static PerkObject Oratory
			{
				get
				{
					return DefaultPerks.Instance._charmOratory;
				}
			}

			public static PerkObject Warlord
			{
				get
				{
					return DefaultPerks.Instance._charmWarlord;
				}
			}

			public static PerkObject ForgivableGrievances
			{
				get
				{
					return DefaultPerks.Instance._charmForgivableGrievances;
				}
			}

			public static PerkObject MeaningfulFavors
			{
				get
				{
					return DefaultPerks.Instance._charmMeaningfulFavors;
				}
			}

			public static PerkObject InBloom
			{
				get
				{
					return DefaultPerks.Instance._charmInBloom;
				}
			}

			public static PerkObject YoungAndRespectful
			{
				get
				{
					return DefaultPerks.Instance._charmYoungAndRespectful;
				}
			}

			public static PerkObject Firebrand
			{
				get
				{
					return DefaultPerks.Instance._charmFirebrand;
				}
			}

			public static PerkObject FlexibleEthics
			{
				get
				{
					return DefaultPerks.Instance._charmFlexibleEthics;
				}
			}

			public static PerkObject EffortForThePeople
			{
				get
				{
					return DefaultPerks.Instance._charmEffortForThePeople;
				}
			}

			public static PerkObject SlickNegotiator
			{
				get
				{
					return DefaultPerks.Instance._charmSlickNegotiator;
				}
			}

			public static PerkObject GoodNatured
			{
				get
				{
					return DefaultPerks.Instance._charmGoodNatured;
				}
			}

			public static PerkObject Tribute
			{
				get
				{
					return DefaultPerks.Instance._charmTribute;
				}
			}

			public static PerkObject MoralLeader
			{
				get
				{
					return DefaultPerks.Instance._charmMoralLeader;
				}
			}

			public static PerkObject NaturalLeader
			{
				get
				{
					return DefaultPerks.Instance._charmNaturalLeader;
				}
			}

			public static PerkObject PublicSpeaker
			{
				get
				{
					return DefaultPerks.Instance._charmPublicSpeaker;
				}
			}

			public static PerkObject Parade
			{
				get
				{
					return DefaultPerks.Instance._charmParade;
				}
			}

			public static PerkObject Camaraderie
			{
				get
				{
					return DefaultPerks.Instance._charmCamaraderie;
				}
			}

			public static PerkObject ImmortalCharm
			{
				get
				{
					return DefaultPerks.Instance._charmImmortalCharm;
				}
			}
		}

		public static class Leadership
		{
			public static PerkObject CombatTips
			{
				get
				{
					return DefaultPerks.Instance._leadershipCombatTips;
				}
			}

			public static PerkObject RaiseTheMeek
			{
				get
				{
					return DefaultPerks.Instance._leadershipRaiseTheMeek;
				}
			}

			public static PerkObject FerventAttacker
			{
				get
				{
					return DefaultPerks.Instance._leadershipFerventAttacker;
				}
			}

			public static PerkObject StoutDefender
			{
				get
				{
					return DefaultPerks.Instance._leadershipStoutDefender;
				}
			}

			public static PerkObject Authority
			{
				get
				{
					return DefaultPerks.Instance._leadershipAuthority;
				}
			}

			public static PerkObject HeroicLeader
			{
				get
				{
					return DefaultPerks.Instance._leadershipHeroicLeader;
				}
			}

			public static PerkObject LoyaltyAndHonor
			{
				get
				{
					return DefaultPerks.Instance._leadershipLoyaltyAndHonor;
				}
			}

			public static PerkObject Presence
			{
				get
				{
					return DefaultPerks.Instance._leadershipPresence;
				}
			}

			public static PerkObject FamousCommander
			{
				get
				{
					return DefaultPerks.Instance._leadershipFamousCommander;
				}
			}

			public static PerkObject LeaderOfMasses
			{
				get
				{
					return DefaultPerks.Instance._leadershipLeaderOfTheMasses;
				}
			}

			public static PerkObject VeteransRespect
			{
				get
				{
					return DefaultPerks.Instance._leadershipVeteransRespect;
				}
			}

			public static PerkObject CitizenMilitia
			{
				get
				{
					return DefaultPerks.Instance._leadershipCitizenMilitia;
				}
			}

			public static PerkObject InspiringLeader
			{
				get
				{
					return DefaultPerks.Instance._leadershipInspiringLeader;
				}
			}

			public static PerkObject UpliftingSpirit
			{
				get
				{
					return DefaultPerks.Instance._leadershipUpliftingSpirit;
				}
			}

			public static PerkObject MakeADifference
			{
				get
				{
					return DefaultPerks.Instance._leadershipMakeADifference;
				}
			}

			public static PerkObject LeadByExample
			{
				get
				{
					return DefaultPerks.Instance._leadershipLeadByExample;
				}
			}

			public static PerkObject TrustedCommander
			{
				get
				{
					return DefaultPerks.Instance._leadershipTrustedCommander;
				}
			}

			public static PerkObject GreatLeader
			{
				get
				{
					return DefaultPerks.Instance._leadershipGreatLeader;
				}
			}

			public static PerkObject WePledgeOurSwords
			{
				get
				{
					return DefaultPerks.Instance._leadershipWePledgeOurSwords;
				}
			}

			public static PerkObject TalentMagnet
			{
				get
				{
					return DefaultPerks.Instance._leadershipTalentMagnet;
				}
			}

			public static PerkObject UltimateLeader
			{
				get
				{
					return DefaultPerks.Instance._leadershipUltimateLeader;
				}
			}
		}

		public static class Trade
		{
			public static PerkObject Appraiser
			{
				get
				{
					return DefaultPerks.Instance._tradeAppraiser;
				}
			}

			public static PerkObject WholeSeller
			{
				get
				{
					return DefaultPerks.Instance._tradeWholeSeller;
				}
			}

			public static PerkObject CaravanMaster
			{
				get
				{
					return DefaultPerks.Instance._tradeCaravanMaster;
				}
			}

			public static PerkObject MarketDealer
			{
				get
				{
					return DefaultPerks.Instance._tradeMarketDealer;
				}
			}

			public static PerkObject TravelingRumors
			{
				get
				{
					return DefaultPerks.Instance._tradeTravelingRumors;
				}
			}

			public static PerkObject LocalConnection
			{
				get
				{
					return DefaultPerks.Instance._tradeLocalConnection;
				}
			}

			public static PerkObject DistributedGoods
			{
				get
				{
					return DefaultPerks.Instance._tradeDistributedGoods;
				}
			}

			public static PerkObject Tollgates
			{
				get
				{
					return DefaultPerks.Instance._tradeTollgates;
				}
			}

			public static PerkObject ArtisanCommunity
			{
				get
				{
					return DefaultPerks.Instance._tradeArtisanCommunity;
				}
			}

			public static PerkObject GreatInvestor
			{
				get
				{
					return DefaultPerks.Instance._tradeGreatInvestor;
				}
			}

			public static PerkObject MercenaryConnections
			{
				get
				{
					return DefaultPerks.Instance._tradeMercenaryConnections;
				}
			}

			public static PerkObject ContentTrades
			{
				get
				{
					return DefaultPerks.Instance._tradeContentTrades;
				}
			}

			public static PerkObject InsurancePlans
			{
				get
				{
					return DefaultPerks.Instance._tradeInsurancePlans;
				}
			}

			public static PerkObject RapidDevelopment
			{
				get
				{
					return DefaultPerks.Instance._tradeRapidDevelopment;
				}
			}

			public static PerkObject GranaryAccountant
			{
				get
				{
					return DefaultPerks.Instance._tradeGranaryAccountant;
				}
			}

			public static PerkObject TradeyardForeman
			{
				get
				{
					return DefaultPerks.Instance._tradeTradeyardForeman;
				}
			}

			public static PerkObject SwordForBarter
			{
				get
				{
					return DefaultPerks.Instance._tradeSwordForBarter;
				}
			}

			public static PerkObject SelfMadeMan
			{
				get
				{
					return DefaultPerks.Instance._tradeSelfMadeMan;
				}
			}

			public static PerkObject SilverTongue
			{
				get
				{
					return DefaultPerks.Instance._tradeSilverTongue;
				}
			}

			public static PerkObject SpringOfGold
			{
				get
				{
					return DefaultPerks.Instance._tradeSpringOfGold;
				}
			}

			public static PerkObject ManOfMeans
			{
				get
				{
					return DefaultPerks.Instance._tradeManOfMeans;
				}
			}

			public static PerkObject TrickleDown
			{
				get
				{
					return DefaultPerks.Instance._tradeTrickleDown;
				}
			}

			public static PerkObject EverythingHasAPrice
			{
				get
				{
					return DefaultPerks.Instance._tradeEverythingHasAPrice;
				}
			}
		}

		public static class Steward
		{
			public static PerkObject WarriorsDiet
			{
				get
				{
					return DefaultPerks.Instance._stewardWarriorsDiet;
				}
			}

			public static PerkObject Frugal
			{
				get
				{
					return DefaultPerks.Instance._stewardFrugal;
				}
			}

			public static PerkObject SevenVeterans
			{
				get
				{
					return DefaultPerks.Instance._stewardSevenVeterans;
				}
			}

			public static PerkObject DrillSergant
			{
				get
				{
					return DefaultPerks.Instance._stewardDrillSergant;
				}
			}

			public static PerkObject Sweatshops
			{
				get
				{
					return DefaultPerks.Instance._stewardSweatshops;
				}
			}

			public static PerkObject StiffUpperLip
			{
				get
				{
					return DefaultPerks.Instance._stewardStiffUpperLip;
				}
			}

			public static PerkObject PaidInPromise
			{
				get
				{
					return DefaultPerks.Instance._stewardPaidInPromise;
				}
			}

			public static PerkObject EfficientCampaigner
			{
				get
				{
					return DefaultPerks.Instance._stewardEfficientCampaigner;
				}
			}

			public static PerkObject GivingHands
			{
				get
				{
					return DefaultPerks.Instance._stewardGivingHands;
				}
			}

			public static PerkObject Logistician
			{
				get
				{
					return DefaultPerks.Instance._stewardLogistician;
				}
			}

			public static PerkObject Relocation
			{
				get
				{
					return DefaultPerks.Instance._stewardRelocation;
				}
			}

			public static PerkObject AidCorps
			{
				get
				{
					return DefaultPerks.Instance._stewardAidCorps;
				}
			}

			public static PerkObject Gourmet
			{
				get
				{
					return DefaultPerks.Instance._stewardGourmet;
				}
			}

			public static PerkObject SoundReserves
			{
				get
				{
					return DefaultPerks.Instance._stewardSoundReserves;
				}
			}

			public static PerkObject ForcedLabor
			{
				get
				{
					return DefaultPerks.Instance._stewardForcedLabor;
				}
			}

			public static PerkObject Contractors
			{
				get
				{
					return DefaultPerks.Instance._stewardContractors;
				}
			}

			public static PerkObject ArenicosMules
			{
				get
				{
					return DefaultPerks.Instance._stewardArenicosMules;
				}
			}

			public static PerkObject ArenicosHorses
			{
				get
				{
					return DefaultPerks.Instance._stewardArenicosHorses;
				}
			}

			public static PerkObject MasterOfPlanning
			{
				get
				{
					return DefaultPerks.Instance._stewardMasterOfPlanning;
				}
			}

			public static PerkObject MasterOfWarcraft
			{
				get
				{
					return DefaultPerks.Instance._stewardMasterOfWarcraft;
				}
			}

			public static PerkObject PriceOfLoyalty
			{
				get
				{
					return DefaultPerks.Instance._stewardPriceOfLoyalty;
				}
			}
		}

		public static class Medicine
		{
			public static PerkObject SelfMedication
			{
				get
				{
					return DefaultPerks.Instance._medicineSelfMedication;
				}
			}

			public static PerkObject PreventiveMedicine
			{
				get
				{
					return DefaultPerks.Instance._medicinePreventiveMedicine;
				}
			}

			public static PerkObject TriageTent
			{
				get
				{
					return DefaultPerks.Instance._medicineTriageTent;
				}
			}

			public static PerkObject WalkItOff
			{
				get
				{
					return DefaultPerks.Instance._medicineWalkItOff;
				}
			}

			public static PerkObject Sledges
			{
				get
				{
					return DefaultPerks.Instance._medicineSledges;
				}
			}

			public static PerkObject DoctorsOath
			{
				get
				{
					return DefaultPerks.Instance._medicineDoctorsOath;
				}
			}

			public static PerkObject BestMedicine
			{
				get
				{
					return DefaultPerks.Instance._medicineBestMedicine;
				}
			}

			public static PerkObject GoodLogdings
			{
				get
				{
					return DefaultPerks.Instance._medicineGoodLodging;
				}
			}

			public static PerkObject SiegeMedic
			{
				get
				{
					return DefaultPerks.Instance._medicineSiegeMedic;
				}
			}

			public static PerkObject Veterinarian
			{
				get
				{
					return DefaultPerks.Instance._medicineVeterinarian;
				}
			}

			public static PerkObject PristineStreets
			{
				get
				{
					return DefaultPerks.Instance._medicinePristineStreets;
				}
			}

			public static PerkObject BushDoctor
			{
				get
				{
					return DefaultPerks.Instance._medicineBushDoctor;
				}
			}

			public static PerkObject PerfectHealth
			{
				get
				{
					return DefaultPerks.Instance._medicinePerfectHealth;
				}
			}

			public static PerkObject HealthAdvise
			{
				get
				{
					return DefaultPerks.Instance._medicineHealthAdvise;
				}
			}

			public static PerkObject PhysicianOfPeople
			{
				get
				{
					return DefaultPerks.Instance._medicinePhysicianOfPeople;
				}
			}

			public static PerkObject CleanInfrastructure
			{
				get
				{
					return DefaultPerks.Instance._medicineCleanInfrastructure;
				}
			}

			public static PerkObject CheatDeath
			{
				get
				{
					return DefaultPerks.Instance._medicineCheatDeath;
				}
			}

			public static PerkObject FortitudeTonic
			{
				get
				{
					return DefaultPerks.Instance._medicineFortitudeTonic;
				}
			}

			public static PerkObject HelpingHands
			{
				get
				{
					return DefaultPerks.Instance._medicineHelpingHands;
				}
			}

			public static PerkObject BattleHardened
			{
				get
				{
					return DefaultPerks.Instance._medicineBattleHardened;
				}
			}

			public static PerkObject MinisterOfHealth
			{
				get
				{
					return DefaultPerks.Instance._medicineMinisterOfHealth;
				}
			}
		}

		public static class Engineering
		{
			public static PerkObject Scaffolds
			{
				get
				{
					return DefaultPerks.Instance._engineeringScaffolds;
				}
			}

			public static PerkObject TorsionEngines
			{
				get
				{
					return DefaultPerks.Instance._engineeringTorsionEngines;
				}
			}

			public static PerkObject SiegeWorks
			{
				get
				{
					return DefaultPerks.Instance._engineeringSiegeWorks;
				}
			}

			public static PerkObject DungeonArchitect
			{
				get
				{
					return DefaultPerks.Instance._engineeringDungeonArchitect;
				}
			}

			public static PerkObject Carpenters
			{
				get
				{
					return DefaultPerks.Instance._engineeringCarpenters;
				}
			}

			public static PerkObject MilitaryPlanner
			{
				get
				{
					return DefaultPerks.Instance._engineeringMilitaryPlanner;
				}
			}

			public static PerkObject WallBreaker
			{
				get
				{
					return DefaultPerks.Instance._engineeringWallBreaker;
				}
			}

			public static PerkObject DreadfulSieger
			{
				get
				{
					return DefaultPerks.Instance._engineeringDreadfulSieger;
				}
			}

			public static PerkObject Salvager
			{
				get
				{
					return DefaultPerks.Instance._engineeringSalvager;
				}
			}

			public static PerkObject Foreman
			{
				get
				{
					return DefaultPerks.Instance._engineeringForeman;
				}
			}

			public static PerkObject Stonecutters
			{
				get
				{
					return DefaultPerks.Instance._engineeringStonecutters;
				}
			}

			public static PerkObject SiegeEngineer
			{
				get
				{
					return DefaultPerks.Instance._engineeringSiegeEngineer;
				}
			}

			public static PerkObject CampBuilding
			{
				get
				{
					return DefaultPerks.Instance._engineeringCampBuilding;
				}
			}

			public static PerkObject Battlements
			{
				get
				{
					return DefaultPerks.Instance._engineeringBattlements;
				}
			}

			public static PerkObject EngineeringGuilds
			{
				get
				{
					return DefaultPerks.Instance._engineeringEngineeringGuilds;
				}
			}

			public static PerkObject Apprenticeship
			{
				get
				{
					return DefaultPerks.Instance._engineeringApprenticeship;
				}
			}

			public static PerkObject Metallurgy
			{
				get
				{
					return DefaultPerks.Instance._engineeringMetallurgy;
				}
			}

			public static PerkObject ImprovedTools
			{
				get
				{
					return DefaultPerks.Instance._engineeringImprovedTools;
				}
			}

			public static PerkObject Clockwork
			{
				get
				{
					return DefaultPerks.Instance._engineeringClockwork;
				}
			}

			public static PerkObject ArchitecturalCommisions
			{
				get
				{
					return DefaultPerks.Instance._engineeringArchitecturalCommisions;
				}
			}

			public static PerkObject Masterwork
			{
				get
				{
					return DefaultPerks.Instance._engineeringMasterwork;
				}
			}
		}
	}
}
