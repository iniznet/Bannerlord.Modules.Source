using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Settlements.Buildings
{
	public class DefaultBuildingTypes
	{
		public static IEnumerable<BuildingType> MilitaryBuildings
		{
			get
			{
				yield return DefaultBuildingTypes.Fortifications;
				yield return DefaultBuildingTypes.SettlementGarrisonBarracks;
				yield return DefaultBuildingTypes.SettlementTrainingFields;
				yield return DefaultBuildingTypes.SettlementWorkshop;
				yield return DefaultBuildingTypes.SettlementMilitiaBarracks;
				yield return DefaultBuildingTypes.SettlementSiegeWorkshop;
				yield return DefaultBuildingTypes.Wall;
				yield return DefaultBuildingTypes.CastleBarracks;
				yield return DefaultBuildingTypes.CastleTrainingFields;
				yield return DefaultBuildingTypes.CastleCastallansOffice;
				yield return DefaultBuildingTypes.CastleWorkshop;
				yield return DefaultBuildingTypes.CastleSiegeWorkshop;
				yield return DefaultBuildingTypes.CastleMilitiaBarracks;
				yield return DefaultBuildingTypes.TrainMilitiaDaily;
				yield break;
			}
		}

		private static DefaultBuildingTypes Instance
		{
			get
			{
				return Campaign.Current.DefaultBuildingTypes;
			}
		}

		public static BuildingType Fortifications
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingFortifications;
			}
		}

		public static BuildingType SettlementGarrisonBarracks
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementGarrisonBarracks;
			}
		}

		public static BuildingType SettlementTrainingFields
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementTrainingFields;
			}
		}

		public static BuildingType SettlementFairgrounds
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementFairgrounds;
			}
		}

		public static BuildingType SettlementMarketplace
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementMarketplace;
			}
		}

		public static BuildingType SettlementAquaducts
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementAquaducts;
			}
		}

		public static BuildingType SettlementForum
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementForum;
			}
		}

		public static BuildingType SettlementGranary
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementGranary;
			}
		}

		public static BuildingType SettlementWorkshop
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementOrchard;
			}
		}

		public static BuildingType SettlementMilitiaBarracks
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementMilitiaBarracks;
			}
		}

		public static BuildingType SettlementSiegeWorkshop
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementSiegeWorkshop;
			}
		}

		public static BuildingType SettlementLimeKilns
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementLimeKilns;
			}
		}

		public static BuildingType Wall
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingWall;
			}
		}

		public static BuildingType CastleBarracks
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleBarracks;
			}
		}

		public static BuildingType CastleTrainingFields
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleTrainingFields;
			}
		}

		public static BuildingType CastleGranary
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleGranary;
			}
		}

		public static BuildingType CastleGardens
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleGardens;
			}
		}

		public static BuildingType CastleCastallansOffice
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleCastallansOffice;
			}
		}

		public static BuildingType CastleWorkshop
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleWorkshop;
			}
		}

		public static BuildingType CastleFairgrounds
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleFairgrounds;
			}
		}

		public static BuildingType CastleSiegeWorkshop
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleSiegeWorkshop;
			}
		}

		public static BuildingType CastleMilitiaBarracks
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleMilitiaBarracks;
			}
		}

		public static BuildingType CastleTollCollector
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleTollCollector;
			}
		}

		public static BuildingType BuildHouseDaily
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingDailyBuildHouse;
			}
		}

		public static BuildingType TrainMilitiaDaily
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingDailyTrainMilitia;
			}
		}

		public static BuildingType FestivalsAndGamesDaily
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingDailyFestivalsAndGames;
			}
		}

		public static BuildingType IrrigationDaily
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingDailyIrrigation;
			}
		}

		public DefaultBuildingTypes()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			this._buildingFortifications = this.Create("building_fortifications");
			this._buildingSettlementGarrisonBarracks = this.Create("building_settlement_garrison_barracks");
			this._buildingSettlementTrainingFields = this.Create("building_settlement_training_fields");
			this._buildingSettlementFairgrounds = this.Create("building_settlement_fairgrounds");
			this._buildingSettlementMarketplace = this.Create("building_settlement_marketplace");
			this._buildingSettlementAquaducts = this.Create("building_settlement_aquaducts");
			this._buildingSettlementForum = this.Create("building_settlement_forum");
			this._buildingSettlementGranary = this.Create("building_settlement_granary");
			this._buildingSettlementOrchard = this.Create("building_settlement_lime_kilns");
			this._buildingSettlementMilitiaBarracks = this.Create("building_settlement_militia_barracks");
			this._buildingSettlementSiegeWorkshop = this.Create("building_siege_workshop");
			this._buildingSettlementLimeKilns = this.Create("building_settlement_workshop");
			this._buildingWall = this.Create("building_wall");
			this._buildingCastleBarracks = this.Create("building_castle_barracks");
			this._buildingCastleTrainingFields = this.Create("building_castle_training_fields");
			this._buildingCastleGranary = this.Create("building_castle_granary");
			this._buildingCastleGardens = this.Create("building_castle_gardens");
			this._buildingCastleCastallansOffice = this.Create("building_castle_castallans_office");
			this._buildingCastleWorkshop = this.Create("building_castle_workshops");
			this._buildingCastleFairgrounds = this.Create("building_castle_fairgrounds");
			this._buildingCastleSiegeWorkshop = this.Create("building_castle_siege_workshop");
			this._buildingCastleMilitiaBarracks = this.Create("building_castle_militia_barracks");
			this._buildingCastleTollCollector = this.Create("building_castle_lime_kilns");
			this._buildingDailyBuildHouse = this.Create("building_daily_build_house");
			this._buildingDailyTrainMilitia = this.Create("building_daily_train_militia");
			this._buildingDailyFestivalsAndGames = this.Create("building_festivals_and_games");
			this._buildingDailyIrrigation = this.Create("building_irrigation");
			this.InitializeAll();
		}

		private BuildingType Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<BuildingType>(new BuildingType(stringId));
		}

		private void InitializeAll()
		{
			this._buildingFortifications.Initialize(new TextObject("{=CVdK1ax1}Fortifications", null), new TextObject("{=dIM6xa2O}Better fortifications and higher walls around town, also increases the max garrison limit since it provides more space for the resident troops.", null), new int[] { 0, 8000, 16000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.GarrisonCapacity, 25f, 50f, 100f)
			}, 1);
			this._buildingSettlementGarrisonBarracks.Initialize(new TextObject("{=54vkRuHo}Garrison Barracks", null), new TextObject("{=DHm1MBsj}Lodging for the garrisoned troops. Each level increases the garrison capacity of the stronghold.", null), new int[] { 2000, 3000, 4000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.GarrisonCapacity, 30f, 60f, 100f)
			}, 0);
			this._buildingSettlementTrainingFields.Initialize(new TextObject("{=BkTiRPT4}Training Fields", null), new TextObject("{=otWlERkc}A field for military drills that increases the daily experience gain of all garrisoned units.", null), new int[] { 2000, 3000, 4000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Experience, 1f, 2f, 3f)
			}, 0);
			this._buildingSettlementFairgrounds.Initialize(new TextObject("{=ixHqTrX5}Fairgrounds", null), new TextObject("{=0B91pZ2R}A permanent space that hosts fairs. Citizens can gather, drink dance and socialize,  increasing the daily morale of the settlement.", null), new int[] { 2000, 3000, 4000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Loyalty, 0.5f, 1f, 1.5f)
			}, 0);
			this._buildingSettlementMarketplace.Initialize(new TextObject("{=zLdXCpne}Marketplace", null), new TextObject("{=Z9LWA6A3}Scheduled market days lure folks from surrounding villages to the settlement and of course the local ruler takes a handsome cut of any sales. Increases the wealth and tax yield of the settlement.", null), new int[] { 2000, 3000, 4000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Tax, 5f, 10f, 15f)
			}, 0);
			this._buildingSettlementAquaducts.Initialize(new TextObject("{=f5jHMbOq}Aqueducts", null), new TextObject("{=UojHRjdG}Access to clean water provides room for growth with healthy citizens and a clean infrastructure. Increases daily Prosperity change.", null), new int[] { 2000, 3000, 4000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Prosperity, 0.3f, 0.6f, 1f)
			}, 0);
			this._buildingSettlementForum.Initialize(new TextObject("{=paelEWj1}Forum", null), new TextObject("{=wTBtu1t5}An open square in the settlement where people can meet, spend time, and share their ideas. Increases influence of the settlement owner.", null), new int[] { 2000, 3000, 4000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Influence, 0.5f, 1f, 1.5f)
			}, 0);
			this._buildingSettlementGranary.Initialize(new TextObject("{=PstO2f5I}Granary", null), new TextObject("{=aK23T43P}Keeps stockpiles of food so that the settlement has more food supply. Each level increases the local food supply.", null), new int[] { 1000, 1500, 2000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Foodstock, 200f, 400f, 600f)
			}, 0);
			this._buildingSettlementLimeKilns.Initialize(new TextObject("{=NbgeKwVr}Workshops", null), new TextObject("{=qR9bEE6g}A building which provides the means required for the manufacture or repair of buildings. Improves project development speed. Also stonemasons reinforce the walls.", null), new int[] { 2000, 3000, 4000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Construction, 3f, 6f, 9f)
			}, 0);
			this._buildingSettlementMilitiaBarracks.Initialize(new TextObject("{=l91xAgmU}Militia Grounds", null), new TextObject("{=RliyRJKl}Provides weapons training for citizens. Increases daily militia recruitment.", null), new int[] { 2000, 3000, 4000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Militia, 0.5f, 1f, 1.5f)
			}, 0);
			this._buildingSettlementSiegeWorkshop.Initialize(new TextObject("{=9Bnwttn6}Siege Workshop", null), new TextObject("{=MharAceZ}A workshop dedicated to sieges. Contains tools and materials to repair walls, build and repair siege engines.", null), new int[] { 1000, 1500, 2000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.WallRepairSpeed, 50f, 50f, 50f),
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.SiegeEngineSpeed, 30f, 60f, 100f)
			}, 0);
			this._buildingSettlementOrchard.Initialize(new TextObject("{=AkbiPIij}Orchards", null), new TextObject("{=ZCLVOXgM}Fruit trees and vegetable gardens outside the walls provide food as long as there is no siege.", null), new int[] { 2000, 3000, 4000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.FoodProduction, 6f, 12f, 18f)
			}, 0);
			this._buildingWall.Initialize(new TextObject("{=6pNrNj93}Wall", null), new TextObject("{=oS5Nesmi}Better fortifications and higher walls around the keep, also increases the max garrison limit since it provides more space for the resident troops.", null), new int[] { 0, 2500, 5000 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.GarrisonCapacity, 25f, 50f, 100f)
			}, 1);
			this._buildingCastleBarracks.Initialize(new TextObject("{=x2B0OjhI}Barracks", null), new TextObject("{=HJ1is924}Lodgings for the garrisoned troops. Increases the garrison capacity of the stronghold.", null), new int[] { 500, 1000, 1500 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.GarrisonCapacity, 30f, 60f, 100f)
			}, 0);
			this._buildingCastleTrainingFields.Initialize(new TextObject("{=BkTiRPT4}Training Fields", null), new TextObject("{=otWlERkc}A field for military drills that increases the daily experience gain of all garrisoned units.", null), new int[] { 500, 1000, 1500 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Experience, 1f, 2f, 3f)
			}, 0);
			this._buildingCastleGranary.Initialize(new TextObject("{=PstO2f5I}Granary", null), new TextObject("{=iazij7fO}Keeps stockpiles of food so that the settlement has more food supply. Increases the local food supply.", null), new int[] { 500, 1000, 1500 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Foodstock, 100f, 150f, 200f)
			}, 0);
			this._buildingCastleGardens.Initialize(new TextObject("{=yT6XN4Mr}Gardens", null), new TextObject("{=ZCLVOXgM}Fruit trees and vegetable gardens outside the walls provide food as long as there is no siege.", null), new int[] { 500, 750, 1000 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.FoodProduction, 3f, 6f, 9f)
			}, 0);
			this._buildingCastleCastallansOffice.Initialize(new TextObject("{=kLNnFMR9}Castellan's Office", null), new TextObject("{=GDsI6daq}Provides a warden for the castle who maintains discipline and upholds the law.", null), new int[] { 500, 750, 1000 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.GarrisonWageReduce, 10f, 20f, 30f)
			}, 0);
			this._buildingCastleWorkshop.Initialize(new TextObject("{=NbgeKwVr}Workshops", null), new TextObject("{=qR9bEE6g}A building which provides the means required for the manufacture or repair of buildings. Improves project development speed. Also stonemasons reinforce the walls.", null), new int[] { 500, 750, 1000 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Construction, 1f, 2f, 3f)
			}, 0);
			this._buildingCastleFairgrounds.Initialize(new TextObject("{=ixHqTrX5}Fairgrounds", null), new TextObject("{=QHZeCDJy}A permanent space that hosts fairs. Citizens can gather, drink dance and socialize, increasing the daily morale of the settlement.", null), new int[] { 500, 750, 1000 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Loyalty, 0.5f, 1f, 1.5f)
			}, 0);
			this._buildingCastleSiegeWorkshop.Initialize(new TextObject("{=9Bnwttn6}Siege Workshop", null), new TextObject("{=MharAceZ}A workshop dedicated to sieges. Contains tools and materials to repair walls, build and repair siege engines.", null), new int[] { 500, 750, 1000 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.WallRepairSpeed, 50f, 50f, 50f),
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.SiegeEngineSpeed, 30f, 60f, 100f)
			}, 0);
			this._buildingCastleMilitiaBarracks.Initialize(new TextObject("{=l91xAgmU}Militia Grounds", null), new TextObject("{=YRrx8bAK}Provides weapons training for citizens. Each level increases daily militia recruitment.", null), new int[] { 500, 750, 1000 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Militia, 1f, 2f, 3f)
			}, 0);
			this._buildingCastleTollCollector.Initialize(new TextObject("{=VawDQKLl}Toll Collector", null), new TextObject("{=ac8PkfhG}Increases tax income from the region", null), new int[] { 500, 750, 1000 }, BuildingLocation.Castle, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Tax, 10f, 20f, 30f)
			}, 0);
			this._buildingDailyBuildHouse.Initialize(new TextObject("{=F4V7oaVx}Housing", null), new TextObject("{=yWXtcxqb}Construct housing so that more folks can settle, increasing population.", null), new int[3], BuildingLocation.Daily, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.ProsperityDaily, 1f, 1f, 1f)
			}, 0);
			this._buildingDailyTrainMilitia.Initialize(new TextObject("{=p1Y3EU5O}Train Militia", null), new TextObject("{=61J1wa6k}Schedule drills for commoners, increasing militia recruitment.", null), new int[3], BuildingLocation.Daily, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.MilitiaDaily, 3f, 3f, 3f)
			}, 0);
			this._buildingDailyFestivalsAndGames.Initialize(new TextObject("{=aEmYZadz}Festival and Games", null), new TextObject("{=ovDbQIo9}Organize festivals and games in the settlement, increasing morale.", null), new int[3], BuildingLocation.Daily, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.LoyaltyDaily, 3f, 3f, 3f)
			}, 0);
			this._buildingDailyIrrigation.Initialize(new TextObject("{=O4cknzhW}Irrigation", null), new TextObject("{=CU9g49fo}Provide irrigation, increasing growth in bound villages.", null), new int[3], BuildingLocation.Daily, new Tuple<BuildingEffectEnum, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.VillageDevelopmentDaily, 1f, 1f, 1f)
			}, 0);
		}

		public const int MaxBuildingLevel = 3;

		private BuildingType _buildingFortifications;

		private BuildingType _buildingSettlementGarrisonBarracks;

		private BuildingType _buildingSettlementTrainingFields;

		private BuildingType _buildingSettlementFairgrounds;

		private BuildingType _buildingSettlementMarketplace;

		private BuildingType _buildingSettlementAquaducts;

		private BuildingType _buildingSettlementForum;

		private BuildingType _buildingSettlementGranary;

		private BuildingType _buildingSettlementOrchard;

		private BuildingType _buildingSettlementMilitiaBarracks;

		private BuildingType _buildingSettlementSiegeWorkshop;

		private BuildingType _buildingSettlementLimeKilns;

		private BuildingType _buildingWall;

		private BuildingType _buildingCastleBarracks;

		private BuildingType _buildingCastleTrainingFields;

		private BuildingType _buildingCastleGranary;

		private BuildingType _buildingCastleGardens;

		private BuildingType _buildingCastleCastallansOffice;

		private BuildingType _buildingCastleWorkshop;

		private BuildingType _buildingCastleFairgrounds;

		private BuildingType _buildingCastleSiegeWorkshop;

		private BuildingType _buildingCastleMilitiaBarracks;

		private BuildingType _buildingCastleTollCollector;

		private BuildingType _buildingDailyBuildHouse;

		private BuildingType _buildingDailyTrainMilitia;

		private BuildingType _buildingDailyFestivalsAndGames;

		private BuildingType _buildingDailyIrrigation;
	}
}
