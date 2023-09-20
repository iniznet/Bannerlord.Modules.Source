using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Settlements
{
	public class DefaultVillageTypes
	{
		private static DefaultVillageTypes Instance
		{
			get
			{
				return Campaign.Current.DefaultVillageTypes;
			}
		}

		public IList<ItemObject> ConsumableRawItems { get; private set; }

		public static VillageType EuropeHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeEuropeHorseRanch;
			}
		}

		public static VillageType BattanianHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeBattanianHorseRanch;
			}
		}

		public static VillageType SturgianHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSturgianHorseRanch;
			}
		}

		public static VillageType VlandianHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeVlandianHorseRanch;
			}
		}

		public static VillageType SteppeHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSteppeHorseRanch;
			}
		}

		public static VillageType DesertHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeDesertHorseRanch;
			}
		}

		public static VillageType WheatFarm
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeWheatFarm;
			}
		}

		public static VillageType Lumberjack
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeLumberjack;
			}
		}

		public static VillageType ClayMine
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeClayMine;
			}
		}

		public static VillageType SaltMine
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSaltMine;
			}
		}

		public static VillageType IronMine
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeIronMine;
			}
		}

		public static VillageType Fisherman
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeFisherman;
			}
		}

		public static VillageType CattleRange
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeCattleRange;
			}
		}

		public static VillageType SheepFarm
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSheepFarm;
			}
		}

		public static VillageType HogFarm
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeHogFarm;
			}
		}

		public static VillageType VineYard
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeVineYard;
			}
		}

		public static VillageType FlaxPlant
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeFlaxPlant;
			}
		}

		public static VillageType DateFarm
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeDateFarm;
			}
		}

		public static VillageType OliveTrees
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeOliveTrees;
			}
		}

		public static VillageType SilkPlant
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSilkPlant;
			}
		}

		public static VillageType SilverMine
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSilverMine;
			}
		}

		internal VillageType VillageTypeEuropeHorseRanch { get; private set; }

		internal VillageType VillageTypeBattanianHorseRanch { get; private set; }

		internal VillageType VillageTypeSturgianHorseRanch { get; private set; }

		internal VillageType VillageTypeVlandianHorseRanch { get; private set; }

		internal VillageType VillageTypeSteppeHorseRanch { get; private set; }

		internal VillageType VillageTypeDesertHorseRanch { get; private set; }

		internal VillageType VillageTypeWheatFarm { get; private set; }

		internal VillageType VillageTypeLumberjack { get; private set; }

		internal VillageType VillageTypeClayMine { get; private set; }

		internal VillageType VillageTypeSaltMine { get; private set; }

		internal VillageType VillageTypeIronMine { get; private set; }

		internal VillageType VillageTypeFisherman { get; private set; }

		internal VillageType VillageTypeCattleRange { get; private set; }

		internal VillageType VillageTypeSheepFarm { get; private set; }

		internal VillageType VillageTypeHogFarm { get; private set; }

		internal VillageType VillageTypeTrapper { get; private set; }

		internal VillageType VillageTypeVineYard { get; private set; }

		internal VillageType VillageTypeFlaxPlant { get; private set; }

		internal VillageType VillageTypeDateFarm { get; private set; }

		internal VillageType VillageTypeOliveTrees { get; private set; }

		internal VillageType VillageTypeSilkPlant { get; private set; }

		internal VillageType VillageTypeSilverMine { get; private set; }

		public DefaultVillageTypes()
		{
			this.ConsumableRawItems = new List<ItemObject>();
			this.RegisterAll();
			this.AddProductions();
		}

		private void RegisterAll()
		{
			this.VillageTypeWheatFarm = this.Create("wheat_farm");
			this.VillageTypeEuropeHorseRanch = this.Create("europe_horse_ranch");
			this.VillageTypeSteppeHorseRanch = this.Create("steppe_horse_ranch");
			this.VillageTypeDesertHorseRanch = this.Create("desert_horse_ranch");
			this.VillageTypeBattanianHorseRanch = this.Create("battanian_horse_ranch");
			this.VillageTypeSturgianHorseRanch = this.Create("sturgian_horse_ranch");
			this.VillageTypeVlandianHorseRanch = this.Create("vlandian_horse_ranch");
			this.VillageTypeLumberjack = this.Create("lumberjack");
			this.VillageTypeClayMine = this.Create("clay_mine");
			this.VillageTypeSaltMine = this.Create("salt_mine");
			this.VillageTypeIronMine = this.Create("iron_mine");
			this.VillageTypeFisherman = this.Create("fisherman");
			this.VillageTypeCattleRange = this.Create("cattle_farm");
			this.VillageTypeSheepFarm = this.Create("sheep_farm");
			this.VillageTypeHogFarm = this.Create("swine_farm");
			this.VillageTypeVineYard = this.Create("vineyard");
			this.VillageTypeFlaxPlant = this.Create("flax_plant");
			this.VillageTypeDateFarm = this.Create("date_farm");
			this.VillageTypeOliveTrees = this.Create("olive_trees");
			this.VillageTypeSilkPlant = this.Create("silk_plant");
			this.VillageTypeSilverMine = this.Create("silver_mine");
			this.VillageTypeTrapper = this.Create("trapper");
			this.InitializeAll();
		}

		private VillageType Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<VillageType>(new VillageType(stringId));
		}

		private void InitializeAll()
		{
			this.VillageTypeWheatFarm.Initialize(new TextObject("{=BPPG2XF7}Wheat Farm", null), "wheat_farm", "wheat_farm_ucon", "wheat_farm_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 50f)
			});
			this.VillageTypeEuropeHorseRanch.Initialize(new TextObject("{=eEh752CZ}Horse Farm", null), "europe_horse_ranch", "ranch_ucon", "europe_horse_ranch_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeSteppeHorseRanch.Initialize(new TextObject("{=eEh752CZ}Horse Farm", null), "steppe_horse_ranch", "ranch_ucon", "steppe_horse_ranch_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeDesertHorseRanch.Initialize(new TextObject("{=eEh752CZ}Horse Farm", null), "desert_horse_ranch", "ranch_ucon", "desert_horse_ranch_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeBattanianHorseRanch.Initialize(new TextObject("{=eEh752CZ}Horse Farm", null), "battanian_horse_ranch", "ranch_ucon", "desert_horse_ranch_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeSturgianHorseRanch.Initialize(new TextObject("{=eEh752CZ}Horse Farm", null), "sturgian_horse_ranch", "ranch_ucon", "desert_horse_ranch_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeVlandianHorseRanch.Initialize(new TextObject("{=eEh752CZ}Horse Farm", null), "vlandian_horse_ranch", "ranch_ucon", "desert_horse_ranch_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeLumberjack.Initialize(new TextObject("{=YYl1W2jU}Forester", null), "lumberjack", "lumberjack_ucon", "lumberjack_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeClayMine.Initialize(new TextObject("{=myuzMhOn}Clay Pits", null), "clay_mine", "clay_mine_ucon", "clay_mine_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeSaltMine.Initialize(new TextObject("{=3aOIY6wl}Salt Mine", null), "salt_mine", "salt_mine_ucon", "salt_mine_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeIronMine.Initialize(new TextObject("{=rHcVKSbA}Iron Mine", null), "iron_mine", "iron_mine_ucon", "iron_mine_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeFisherman.Initialize(new TextObject("{=XpREJNHD}Fishers", null), "fisherman", "fisherman_ucon", "fisherman_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeCattleRange.Initialize(new TextObject("{=bW3csuSZ}Cattle Farms", null), "cattle_farm", "ranch_ucon", "cattle_farm_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeSheepFarm.Initialize(new TextObject("{=QbKbGu2h}Sheep Farms", null), "sheep_farm", "ranch_ucon", "sheep_farm_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeHogFarm.Initialize(new TextObject("{=vqSHB7mJ}Swine Farm", null), "swine_farm", "swine_farm_ucon", "swine_farm_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeVineYard.Initialize(new TextObject("{=ZtxWTS9V}Vineyard", null), "vineyard", "vineyard_ucon", "vineyard_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeFlaxPlant.Initialize(new TextObject("{=Z8ntYx0Y}Flax Field", null), "flax_plant", "flax_plant_ucon", "flax_plant_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeDateFarm.Initialize(new TextObject("{=2NR2E663}Palm Orchard", null), "date_farm", "date_farm_ucon", "date_farm_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeOliveTrees.Initialize(new TextObject("{=ewrkbwI9}Olive Trees", null), "date_farm", "date_farm_ucon", "date_farm_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeSilkPlant.Initialize(new TextObject("{=wTyq7LaM}Silkworm Farm", null), "silk_plant", "silk_plant_ucon", "silk_plant_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeSilverMine.Initialize(new TextObject("{=aJLQz9iZ}Silver Mine", null), "silver_mine", "silver_mine_ucon", "silver_mine_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
			this.VillageTypeTrapper.Initialize(new TextObject("{=RREyouKr}Trapper", null), "trapper", "trapper_ucon", "trapper_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(DefaultItems.Grain, 3f)
			});
		}

		private void AddProductions()
		{
			this.AddProductions(this.VillageTypeWheatFarm, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("cow", 0.2f),
				new ValueTuple<string, float>("sheep", 0.4f),
				new ValueTuple<string, float>("hog", 0.8f)
			});
			this.AddProductions(this.VillageTypeEuropeHorseRanch, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("empire_horse", 2.1f),
				new ValueTuple<string, float>("t2_empire_horse", 0.5f),
				new ValueTuple<string, float>("t3_empire_horse", 0.07f),
				new ValueTuple<string, float>("sumpter_horse", 0.5f),
				new ValueTuple<string, float>("mule", 0.5f),
				new ValueTuple<string, float>("saddle_horse", 0.5f),
				new ValueTuple<string, float>("old_horse", 0.5f),
				new ValueTuple<string, float>("hunter", 0.2f),
				new ValueTuple<string, float>("charger", 0.2f)
			});
			this.AddProductions(this.VillageTypeSturgianHorseRanch, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("sturgia_horse", 2.5f),
				new ValueTuple<string, float>("t2_sturgia_horse", 0.7f),
				new ValueTuple<string, float>("t3_sturgia_horse", 0.1f),
				new ValueTuple<string, float>("sumpter_horse", 0.5f),
				new ValueTuple<string, float>("mule", 0.5f),
				new ValueTuple<string, float>("saddle_horse", 0.5f),
				new ValueTuple<string, float>("old_horse", 0.5f),
				new ValueTuple<string, float>("hunter", 0.2f),
				new ValueTuple<string, float>("charger", 0.2f)
			});
			this.AddProductions(this.VillageTypeVlandianHorseRanch, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("vlandia_horse", 2.1f),
				new ValueTuple<string, float>("t2_vlandia_horse", 0.4f),
				new ValueTuple<string, float>("t3_vlandia_horse", 0.08f),
				new ValueTuple<string, float>("sumpter_horse", 0.5f),
				new ValueTuple<string, float>("mule", 0.5f),
				new ValueTuple<string, float>("saddle_horse", 0.5f),
				new ValueTuple<string, float>("old_horse", 0.5f),
				new ValueTuple<string, float>("hunter", 0.2f),
				new ValueTuple<string, float>("charger", 0.2f)
			});
			this.AddProductions(this.VillageTypeBattanianHorseRanch, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("battania_horse", 2.3f),
				new ValueTuple<string, float>("t2_battania_horse", 0.7f),
				new ValueTuple<string, float>("t3_battania_horse", 0.09f),
				new ValueTuple<string, float>("sumpter_horse", 0.5f),
				new ValueTuple<string, float>("mule", 0.5f),
				new ValueTuple<string, float>("saddle_horse", 0.5f),
				new ValueTuple<string, float>("old_horse", 0.5f),
				new ValueTuple<string, float>("hunter", 0.2f),
				new ValueTuple<string, float>("charger", 0.2f)
			});
			this.AddProductions(this.VillageTypeSteppeHorseRanch, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("khuzait_horse", 1.8f),
				new ValueTuple<string, float>("t2_khuzait_horse", 0.4f),
				new ValueTuple<string, float>("t3_khuzait_horse", 0.05f),
				new ValueTuple<string, float>("sumpter_horse", 0.5f),
				new ValueTuple<string, float>("mule", 0.5f)
			});
			this.AddProductions(this.VillageTypeDesertHorseRanch, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("aserai_horse", 1.7f),
				new ValueTuple<string, float>("t2_aserai_horse", 0.3f),
				new ValueTuple<string, float>("t3_aserai_horse", 0.05f),
				new ValueTuple<string, float>("camel", 0.3f),
				new ValueTuple<string, float>("war_camel", 0.08f),
				new ValueTuple<string, float>("pack_camel", 0.3f),
				new ValueTuple<string, float>("sumpter_horse", 0.4f),
				new ValueTuple<string, float>("mule", 0.5f)
			});
			this.AddProductions(this.VillageTypeCattleRange, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("cow", 2f),
				new ValueTuple<string, float>("butter", 4f),
				new ValueTuple<string, float>("cheese", 4f)
			});
			this.AddProductions(this.VillageTypeSheepFarm, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("sheep", 4f),
				new ValueTuple<string, float>("wool", 6f),
				new ValueTuple<string, float>("butter", 2f),
				new ValueTuple<string, float>("cheese", 2f)
			});
			this.AddProductions(this.VillageTypeHogFarm, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("hog", 8f),
				new ValueTuple<string, float>("butter", 2f),
				new ValueTuple<string, float>("cheese", 2f)
			});
			this.AddProductions(this.VillageTypeLumberjack, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("hardwood", 18f)
			});
			this.AddProductions(this.VillageTypeClayMine, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("clay", 10f)
			});
			this.AddProductions(this.VillageTypeSaltMine, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("salt", 15f)
			});
			this.AddProductions(this.VillageTypeIronMine, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("iron", 10f)
			});
			this.AddProductions(this.VillageTypeFisherman, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("fish", 28f)
			});
			this.AddProductions(this.VillageTypeVineYard, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("grape", 11f)
			});
			this.AddProductions(this.VillageTypeFlaxPlant, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("flax", 18f)
			});
			this.AddProductions(this.VillageTypeDateFarm, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("date_fruit", 8f)
			});
			this.AddProductions(this.VillageTypeOliveTrees, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("olives", 12f)
			});
			this.AddProductions(this.VillageTypeSilkPlant, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("cotton", 8f)
			});
			this.AddProductions(this.VillageTypeSilverMine, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("silver", 3f)
			});
			this.AddProductions(this.VillageTypeTrapper, new ValueTuple<string, float>[]
			{
				new ValueTuple<string, float>("fur", 1.4f)
			});
			this.ConsumableRawItems.Add(Game.Current.ObjectManager.GetObject<ItemObject>("grain"));
			this.ConsumableRawItems.Add(Game.Current.ObjectManager.GetObject<ItemObject>("cheese"));
			this.ConsumableRawItems.Add(Game.Current.ObjectManager.GetObject<ItemObject>("butter"));
		}

		private void AddProductions(VillageType villageType, ValueTuple<string, float>[] productions)
		{
			villageType.AddProductions(productions.Select((ValueTuple<string, float> p) => new ValueTuple<ItemObject, float>(Game.Current.ObjectManager.GetObject<ItemObject>(p.Item1), p.Item2)));
		}
	}
}
