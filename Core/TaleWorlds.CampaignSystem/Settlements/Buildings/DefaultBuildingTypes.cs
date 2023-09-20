using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Settlements.Buildings
{
	// Token: 0x02000375 RID: 885
	public class DefaultBuildingTypes
	{
		// Token: 0x17000C94 RID: 3220
		// (get) Token: 0x06003387 RID: 13191 RVA: 0x000D4AA1 File Offset: 0x000D2CA1
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

		// Token: 0x17000C95 RID: 3221
		// (get) Token: 0x06003388 RID: 13192 RVA: 0x000D4AAA File Offset: 0x000D2CAA
		private static DefaultBuildingTypes Instance
		{
			get
			{
				return Campaign.Current.DefaultBuildingTypes;
			}
		}

		// Token: 0x17000C96 RID: 3222
		// (get) Token: 0x06003389 RID: 13193 RVA: 0x000D4AB6 File Offset: 0x000D2CB6
		public static BuildingType Fortifications
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingFortifications;
			}
		}

		// Token: 0x17000C97 RID: 3223
		// (get) Token: 0x0600338A RID: 13194 RVA: 0x000D4AC2 File Offset: 0x000D2CC2
		public static BuildingType SettlementGarrisonBarracks
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementGarrisonBarracks;
			}
		}

		// Token: 0x17000C98 RID: 3224
		// (get) Token: 0x0600338B RID: 13195 RVA: 0x000D4ACE File Offset: 0x000D2CCE
		public static BuildingType SettlementTrainingFields
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementTrainingFields;
			}
		}

		// Token: 0x17000C99 RID: 3225
		// (get) Token: 0x0600338C RID: 13196 RVA: 0x000D4ADA File Offset: 0x000D2CDA
		public static BuildingType SettlementFairgrounds
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementFairgrounds;
			}
		}

		// Token: 0x17000C9A RID: 3226
		// (get) Token: 0x0600338D RID: 13197 RVA: 0x000D4AE6 File Offset: 0x000D2CE6
		public static BuildingType SettlementMarketplace
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementMarketplace;
			}
		}

		// Token: 0x17000C9B RID: 3227
		// (get) Token: 0x0600338E RID: 13198 RVA: 0x000D4AF2 File Offset: 0x000D2CF2
		public static BuildingType SettlementAquaducts
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementAquaducts;
			}
		}

		// Token: 0x17000C9C RID: 3228
		// (get) Token: 0x0600338F RID: 13199 RVA: 0x000D4AFE File Offset: 0x000D2CFE
		public static BuildingType SettlementForum
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementForum;
			}
		}

		// Token: 0x17000C9D RID: 3229
		// (get) Token: 0x06003390 RID: 13200 RVA: 0x000D4B0A File Offset: 0x000D2D0A
		public static BuildingType SettlementGranary
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementGranary;
			}
		}

		// Token: 0x17000C9E RID: 3230
		// (get) Token: 0x06003391 RID: 13201 RVA: 0x000D4B16 File Offset: 0x000D2D16
		public static BuildingType SettlementWorkshop
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementOrchard;
			}
		}

		// Token: 0x17000C9F RID: 3231
		// (get) Token: 0x06003392 RID: 13202 RVA: 0x000D4B22 File Offset: 0x000D2D22
		public static BuildingType SettlementMilitiaBarracks
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementMilitiaBarracks;
			}
		}

		// Token: 0x17000CA0 RID: 3232
		// (get) Token: 0x06003393 RID: 13203 RVA: 0x000D4B2E File Offset: 0x000D2D2E
		public static BuildingType SettlementSiegeWorkshop
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementSiegeWorkshop;
			}
		}

		// Token: 0x17000CA1 RID: 3233
		// (get) Token: 0x06003394 RID: 13204 RVA: 0x000D4B3A File Offset: 0x000D2D3A
		public static BuildingType SettlementLimeKilns
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingSettlementLimeKilns;
			}
		}

		// Token: 0x17000CA2 RID: 3234
		// (get) Token: 0x06003395 RID: 13205 RVA: 0x000D4B46 File Offset: 0x000D2D46
		public static BuildingType Wall
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingWall;
			}
		}

		// Token: 0x17000CA3 RID: 3235
		// (get) Token: 0x06003396 RID: 13206 RVA: 0x000D4B52 File Offset: 0x000D2D52
		public static BuildingType CastleBarracks
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleBarracks;
			}
		}

		// Token: 0x17000CA4 RID: 3236
		// (get) Token: 0x06003397 RID: 13207 RVA: 0x000D4B5E File Offset: 0x000D2D5E
		public static BuildingType CastleTrainingFields
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleTrainingFields;
			}
		}

		// Token: 0x17000CA5 RID: 3237
		// (get) Token: 0x06003398 RID: 13208 RVA: 0x000D4B6A File Offset: 0x000D2D6A
		public static BuildingType CastleGranary
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleGranary;
			}
		}

		// Token: 0x17000CA6 RID: 3238
		// (get) Token: 0x06003399 RID: 13209 RVA: 0x000D4B76 File Offset: 0x000D2D76
		public static BuildingType CastleGardens
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleGardens;
			}
		}

		// Token: 0x17000CA7 RID: 3239
		// (get) Token: 0x0600339A RID: 13210 RVA: 0x000D4B82 File Offset: 0x000D2D82
		public static BuildingType CastleCastallansOffice
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleCastallansOffice;
			}
		}

		// Token: 0x17000CA8 RID: 3240
		// (get) Token: 0x0600339B RID: 13211 RVA: 0x000D4B8E File Offset: 0x000D2D8E
		public static BuildingType CastleWorkshop
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleWorkshop;
			}
		}

		// Token: 0x17000CA9 RID: 3241
		// (get) Token: 0x0600339C RID: 13212 RVA: 0x000D4B9A File Offset: 0x000D2D9A
		public static BuildingType CastleFairgrounds
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleFairgrounds;
			}
		}

		// Token: 0x17000CAA RID: 3242
		// (get) Token: 0x0600339D RID: 13213 RVA: 0x000D4BA6 File Offset: 0x000D2DA6
		public static BuildingType CastleSiegeWorkshop
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleSiegeWorkshop;
			}
		}

		// Token: 0x17000CAB RID: 3243
		// (get) Token: 0x0600339E RID: 13214 RVA: 0x000D4BB2 File Offset: 0x000D2DB2
		public static BuildingType CastleMilitiaBarracks
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleMilitiaBarracks;
			}
		}

		// Token: 0x17000CAC RID: 3244
		// (get) Token: 0x0600339F RID: 13215 RVA: 0x000D4BBE File Offset: 0x000D2DBE
		public static BuildingType CastleTollCollector
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingCastleTollCollector;
			}
		}

		// Token: 0x17000CAD RID: 3245
		// (get) Token: 0x060033A0 RID: 13216 RVA: 0x000D4BCA File Offset: 0x000D2DCA
		public static BuildingType BuildHouseDaily
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingDailyBuildHouse;
			}
		}

		// Token: 0x17000CAE RID: 3246
		// (get) Token: 0x060033A1 RID: 13217 RVA: 0x000D4BD6 File Offset: 0x000D2DD6
		public static BuildingType TrainMilitiaDaily
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingDailyTrainMilitia;
			}
		}

		// Token: 0x17000CAF RID: 3247
		// (get) Token: 0x060033A2 RID: 13218 RVA: 0x000D4BE2 File Offset: 0x000D2DE2
		public static BuildingType FestivalsAndGamesDaily
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingDailyFestivalsAndGames;
			}
		}

		// Token: 0x17000CB0 RID: 3248
		// (get) Token: 0x060033A3 RID: 13219 RVA: 0x000D4BEE File Offset: 0x000D2DEE
		public static BuildingType IrrigationDaily
		{
			get
			{
				return DefaultBuildingTypes.Instance._buildingDailyIrrigation;
			}
		}

		// Token: 0x060033A4 RID: 13220 RVA: 0x000D4BFA File Offset: 0x000D2DFA
		public DefaultBuildingTypes()
		{
			this.RegisterAll();
		}

		// Token: 0x060033A5 RID: 13221 RVA: 0x000D4C08 File Offset: 0x000D2E08
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

		// Token: 0x060033A6 RID: 13222 RVA: 0x000D4DE6 File Offset: 0x000D2FE6
		private BuildingType Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<BuildingType>(new BuildingType(stringId));
		}

		// Token: 0x060033A7 RID: 13223 RVA: 0x000D4E00 File Offset: 0x000D3000
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

		// Token: 0x040010DA RID: 4314
		public const int MaxBuildingLevel = 3;

		// Token: 0x040010DB RID: 4315
		private BuildingType _buildingFortifications;

		// Token: 0x040010DC RID: 4316
		private BuildingType _buildingSettlementGarrisonBarracks;

		// Token: 0x040010DD RID: 4317
		private BuildingType _buildingSettlementTrainingFields;

		// Token: 0x040010DE RID: 4318
		private BuildingType _buildingSettlementFairgrounds;

		// Token: 0x040010DF RID: 4319
		private BuildingType _buildingSettlementMarketplace;

		// Token: 0x040010E0 RID: 4320
		private BuildingType _buildingSettlementAquaducts;

		// Token: 0x040010E1 RID: 4321
		private BuildingType _buildingSettlementForum;

		// Token: 0x040010E2 RID: 4322
		private BuildingType _buildingSettlementGranary;

		// Token: 0x040010E3 RID: 4323
		private BuildingType _buildingSettlementOrchard;

		// Token: 0x040010E4 RID: 4324
		private BuildingType _buildingSettlementMilitiaBarracks;

		// Token: 0x040010E5 RID: 4325
		private BuildingType _buildingSettlementSiegeWorkshop;

		// Token: 0x040010E6 RID: 4326
		private BuildingType _buildingSettlementLimeKilns;

		// Token: 0x040010E7 RID: 4327
		private BuildingType _buildingWall;

		// Token: 0x040010E8 RID: 4328
		private BuildingType _buildingCastleBarracks;

		// Token: 0x040010E9 RID: 4329
		private BuildingType _buildingCastleTrainingFields;

		// Token: 0x040010EA RID: 4330
		private BuildingType _buildingCastleGranary;

		// Token: 0x040010EB RID: 4331
		private BuildingType _buildingCastleGardens;

		// Token: 0x040010EC RID: 4332
		private BuildingType _buildingCastleCastallansOffice;

		// Token: 0x040010ED RID: 4333
		private BuildingType _buildingCastleWorkshop;

		// Token: 0x040010EE RID: 4334
		private BuildingType _buildingCastleFairgrounds;

		// Token: 0x040010EF RID: 4335
		private BuildingType _buildingCastleSiegeWorkshop;

		// Token: 0x040010F0 RID: 4336
		private BuildingType _buildingCastleMilitiaBarracks;

		// Token: 0x040010F1 RID: 4337
		private BuildingType _buildingCastleTollCollector;

		// Token: 0x040010F2 RID: 4338
		private BuildingType _buildingDailyBuildHouse;

		// Token: 0x040010F3 RID: 4339
		private BuildingType _buildingDailyTrainMilitia;

		// Token: 0x040010F4 RID: 4340
		private BuildingType _buildingDailyFestivalsAndGames;

		// Token: 0x040010F5 RID: 4341
		private BuildingType _buildingDailyIrrigation;
	}
}
