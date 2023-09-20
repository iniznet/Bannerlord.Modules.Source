using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Settlements
{
	// Token: 0x02000365 RID: 869
	public class DefaultVillageTypes
	{
		// Token: 0x17000C27 RID: 3111
		// (get) Token: 0x0600323D RID: 12861 RVA: 0x000D0B6A File Offset: 0x000CED6A
		private static DefaultVillageTypes Instance
		{
			get
			{
				return Campaign.Current.DefaultVillageTypes;
			}
		}

		// Token: 0x17000C28 RID: 3112
		// (get) Token: 0x0600323E RID: 12862 RVA: 0x000D0B76 File Offset: 0x000CED76
		// (set) Token: 0x0600323F RID: 12863 RVA: 0x000D0B7E File Offset: 0x000CED7E
		public IList<ItemObject> ConsumableRawItems { get; private set; }

		// Token: 0x17000C29 RID: 3113
		// (get) Token: 0x06003240 RID: 12864 RVA: 0x000D0B87 File Offset: 0x000CED87
		public static VillageType EuropeHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeEuropeHorseRanch;
			}
		}

		// Token: 0x17000C2A RID: 3114
		// (get) Token: 0x06003241 RID: 12865 RVA: 0x000D0B93 File Offset: 0x000CED93
		public static VillageType BattanianHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeBattanianHorseRanch;
			}
		}

		// Token: 0x17000C2B RID: 3115
		// (get) Token: 0x06003242 RID: 12866 RVA: 0x000D0B9F File Offset: 0x000CED9F
		public static VillageType SturgianHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSturgianHorseRanch;
			}
		}

		// Token: 0x17000C2C RID: 3116
		// (get) Token: 0x06003243 RID: 12867 RVA: 0x000D0BAB File Offset: 0x000CEDAB
		public static VillageType VlandianHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeVlandianHorseRanch;
			}
		}

		// Token: 0x17000C2D RID: 3117
		// (get) Token: 0x06003244 RID: 12868 RVA: 0x000D0BB7 File Offset: 0x000CEDB7
		public static VillageType SteppeHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSteppeHorseRanch;
			}
		}

		// Token: 0x17000C2E RID: 3118
		// (get) Token: 0x06003245 RID: 12869 RVA: 0x000D0BC3 File Offset: 0x000CEDC3
		public static VillageType DesertHorseRanch
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeDesertHorseRanch;
			}
		}

		// Token: 0x17000C2F RID: 3119
		// (get) Token: 0x06003246 RID: 12870 RVA: 0x000D0BCF File Offset: 0x000CEDCF
		public static VillageType WheatFarm
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeWheatFarm;
			}
		}

		// Token: 0x17000C30 RID: 3120
		// (get) Token: 0x06003247 RID: 12871 RVA: 0x000D0BDB File Offset: 0x000CEDDB
		public static VillageType Lumberjack
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeLumberjack;
			}
		}

		// Token: 0x17000C31 RID: 3121
		// (get) Token: 0x06003248 RID: 12872 RVA: 0x000D0BE7 File Offset: 0x000CEDE7
		public static VillageType ClayMine
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeClayMine;
			}
		}

		// Token: 0x17000C32 RID: 3122
		// (get) Token: 0x06003249 RID: 12873 RVA: 0x000D0BF3 File Offset: 0x000CEDF3
		public static VillageType SaltMine
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSaltMine;
			}
		}

		// Token: 0x17000C33 RID: 3123
		// (get) Token: 0x0600324A RID: 12874 RVA: 0x000D0BFF File Offset: 0x000CEDFF
		public static VillageType IronMine
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeIronMine;
			}
		}

		// Token: 0x17000C34 RID: 3124
		// (get) Token: 0x0600324B RID: 12875 RVA: 0x000D0C0B File Offset: 0x000CEE0B
		public static VillageType Fisherman
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeFisherman;
			}
		}

		// Token: 0x17000C35 RID: 3125
		// (get) Token: 0x0600324C RID: 12876 RVA: 0x000D0C17 File Offset: 0x000CEE17
		public static VillageType CattleRange
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeCattleRange;
			}
		}

		// Token: 0x17000C36 RID: 3126
		// (get) Token: 0x0600324D RID: 12877 RVA: 0x000D0C23 File Offset: 0x000CEE23
		public static VillageType SheepFarm
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSheepFarm;
			}
		}

		// Token: 0x17000C37 RID: 3127
		// (get) Token: 0x0600324E RID: 12878 RVA: 0x000D0C2F File Offset: 0x000CEE2F
		public static VillageType HogFarm
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeHogFarm;
			}
		}

		// Token: 0x17000C38 RID: 3128
		// (get) Token: 0x0600324F RID: 12879 RVA: 0x000D0C3B File Offset: 0x000CEE3B
		public static VillageType VineYard
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeVineYard;
			}
		}

		// Token: 0x17000C39 RID: 3129
		// (get) Token: 0x06003250 RID: 12880 RVA: 0x000D0C47 File Offset: 0x000CEE47
		public static VillageType FlaxPlant
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeFlaxPlant;
			}
		}

		// Token: 0x17000C3A RID: 3130
		// (get) Token: 0x06003251 RID: 12881 RVA: 0x000D0C53 File Offset: 0x000CEE53
		public static VillageType DateFarm
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeDateFarm;
			}
		}

		// Token: 0x17000C3B RID: 3131
		// (get) Token: 0x06003252 RID: 12882 RVA: 0x000D0C5F File Offset: 0x000CEE5F
		public static VillageType OliveTrees
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeOliveTrees;
			}
		}

		// Token: 0x17000C3C RID: 3132
		// (get) Token: 0x06003253 RID: 12883 RVA: 0x000D0C6B File Offset: 0x000CEE6B
		public static VillageType SilkPlant
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSilkPlant;
			}
		}

		// Token: 0x17000C3D RID: 3133
		// (get) Token: 0x06003254 RID: 12884 RVA: 0x000D0C77 File Offset: 0x000CEE77
		public static VillageType SilverMine
		{
			get
			{
				return DefaultVillageTypes.Instance.VillageTypeSilverMine;
			}
		}

		// Token: 0x17000C3E RID: 3134
		// (get) Token: 0x06003255 RID: 12885 RVA: 0x000D0C83 File Offset: 0x000CEE83
		// (set) Token: 0x06003256 RID: 12886 RVA: 0x000D0C8B File Offset: 0x000CEE8B
		internal VillageType VillageTypeEuropeHorseRanch { get; private set; }

		// Token: 0x17000C3F RID: 3135
		// (get) Token: 0x06003257 RID: 12887 RVA: 0x000D0C94 File Offset: 0x000CEE94
		// (set) Token: 0x06003258 RID: 12888 RVA: 0x000D0C9C File Offset: 0x000CEE9C
		internal VillageType VillageTypeBattanianHorseRanch { get; private set; }

		// Token: 0x17000C40 RID: 3136
		// (get) Token: 0x06003259 RID: 12889 RVA: 0x000D0CA5 File Offset: 0x000CEEA5
		// (set) Token: 0x0600325A RID: 12890 RVA: 0x000D0CAD File Offset: 0x000CEEAD
		internal VillageType VillageTypeSturgianHorseRanch { get; private set; }

		// Token: 0x17000C41 RID: 3137
		// (get) Token: 0x0600325B RID: 12891 RVA: 0x000D0CB6 File Offset: 0x000CEEB6
		// (set) Token: 0x0600325C RID: 12892 RVA: 0x000D0CBE File Offset: 0x000CEEBE
		internal VillageType VillageTypeVlandianHorseRanch { get; private set; }

		// Token: 0x17000C42 RID: 3138
		// (get) Token: 0x0600325D RID: 12893 RVA: 0x000D0CC7 File Offset: 0x000CEEC7
		// (set) Token: 0x0600325E RID: 12894 RVA: 0x000D0CCF File Offset: 0x000CEECF
		internal VillageType VillageTypeSteppeHorseRanch { get; private set; }

		// Token: 0x17000C43 RID: 3139
		// (get) Token: 0x0600325F RID: 12895 RVA: 0x000D0CD8 File Offset: 0x000CEED8
		// (set) Token: 0x06003260 RID: 12896 RVA: 0x000D0CE0 File Offset: 0x000CEEE0
		internal VillageType VillageTypeDesertHorseRanch { get; private set; }

		// Token: 0x17000C44 RID: 3140
		// (get) Token: 0x06003261 RID: 12897 RVA: 0x000D0CE9 File Offset: 0x000CEEE9
		// (set) Token: 0x06003262 RID: 12898 RVA: 0x000D0CF1 File Offset: 0x000CEEF1
		internal VillageType VillageTypeWheatFarm { get; private set; }

		// Token: 0x17000C45 RID: 3141
		// (get) Token: 0x06003263 RID: 12899 RVA: 0x000D0CFA File Offset: 0x000CEEFA
		// (set) Token: 0x06003264 RID: 12900 RVA: 0x000D0D02 File Offset: 0x000CEF02
		internal VillageType VillageTypeLumberjack { get; private set; }

		// Token: 0x17000C46 RID: 3142
		// (get) Token: 0x06003265 RID: 12901 RVA: 0x000D0D0B File Offset: 0x000CEF0B
		// (set) Token: 0x06003266 RID: 12902 RVA: 0x000D0D13 File Offset: 0x000CEF13
		internal VillageType VillageTypeClayMine { get; private set; }

		// Token: 0x17000C47 RID: 3143
		// (get) Token: 0x06003267 RID: 12903 RVA: 0x000D0D1C File Offset: 0x000CEF1C
		// (set) Token: 0x06003268 RID: 12904 RVA: 0x000D0D24 File Offset: 0x000CEF24
		internal VillageType VillageTypeSaltMine { get; private set; }

		// Token: 0x17000C48 RID: 3144
		// (get) Token: 0x06003269 RID: 12905 RVA: 0x000D0D2D File Offset: 0x000CEF2D
		// (set) Token: 0x0600326A RID: 12906 RVA: 0x000D0D35 File Offset: 0x000CEF35
		internal VillageType VillageTypeIronMine { get; private set; }

		// Token: 0x17000C49 RID: 3145
		// (get) Token: 0x0600326B RID: 12907 RVA: 0x000D0D3E File Offset: 0x000CEF3E
		// (set) Token: 0x0600326C RID: 12908 RVA: 0x000D0D46 File Offset: 0x000CEF46
		internal VillageType VillageTypeFisherman { get; private set; }

		// Token: 0x17000C4A RID: 3146
		// (get) Token: 0x0600326D RID: 12909 RVA: 0x000D0D4F File Offset: 0x000CEF4F
		// (set) Token: 0x0600326E RID: 12910 RVA: 0x000D0D57 File Offset: 0x000CEF57
		internal VillageType VillageTypeCattleRange { get; private set; }

		// Token: 0x17000C4B RID: 3147
		// (get) Token: 0x0600326F RID: 12911 RVA: 0x000D0D60 File Offset: 0x000CEF60
		// (set) Token: 0x06003270 RID: 12912 RVA: 0x000D0D68 File Offset: 0x000CEF68
		internal VillageType VillageTypeSheepFarm { get; private set; }

		// Token: 0x17000C4C RID: 3148
		// (get) Token: 0x06003271 RID: 12913 RVA: 0x000D0D71 File Offset: 0x000CEF71
		// (set) Token: 0x06003272 RID: 12914 RVA: 0x000D0D79 File Offset: 0x000CEF79
		internal VillageType VillageTypeHogFarm { get; private set; }

		// Token: 0x17000C4D RID: 3149
		// (get) Token: 0x06003273 RID: 12915 RVA: 0x000D0D82 File Offset: 0x000CEF82
		// (set) Token: 0x06003274 RID: 12916 RVA: 0x000D0D8A File Offset: 0x000CEF8A
		internal VillageType VillageTypeTrapper { get; private set; }

		// Token: 0x17000C4E RID: 3150
		// (get) Token: 0x06003275 RID: 12917 RVA: 0x000D0D93 File Offset: 0x000CEF93
		// (set) Token: 0x06003276 RID: 12918 RVA: 0x000D0D9B File Offset: 0x000CEF9B
		internal VillageType VillageTypeVineYard { get; private set; }

		// Token: 0x17000C4F RID: 3151
		// (get) Token: 0x06003277 RID: 12919 RVA: 0x000D0DA4 File Offset: 0x000CEFA4
		// (set) Token: 0x06003278 RID: 12920 RVA: 0x000D0DAC File Offset: 0x000CEFAC
		internal VillageType VillageTypeFlaxPlant { get; private set; }

		// Token: 0x17000C50 RID: 3152
		// (get) Token: 0x06003279 RID: 12921 RVA: 0x000D0DB5 File Offset: 0x000CEFB5
		// (set) Token: 0x0600327A RID: 12922 RVA: 0x000D0DBD File Offset: 0x000CEFBD
		internal VillageType VillageTypeDateFarm { get; private set; }

		// Token: 0x17000C51 RID: 3153
		// (get) Token: 0x0600327B RID: 12923 RVA: 0x000D0DC6 File Offset: 0x000CEFC6
		// (set) Token: 0x0600327C RID: 12924 RVA: 0x000D0DCE File Offset: 0x000CEFCE
		internal VillageType VillageTypeOliveTrees { get; private set; }

		// Token: 0x17000C52 RID: 3154
		// (get) Token: 0x0600327D RID: 12925 RVA: 0x000D0DD7 File Offset: 0x000CEFD7
		// (set) Token: 0x0600327E RID: 12926 RVA: 0x000D0DDF File Offset: 0x000CEFDF
		internal VillageType VillageTypeSilkPlant { get; private set; }

		// Token: 0x17000C53 RID: 3155
		// (get) Token: 0x0600327F RID: 12927 RVA: 0x000D0DE8 File Offset: 0x000CEFE8
		// (set) Token: 0x06003280 RID: 12928 RVA: 0x000D0DF0 File Offset: 0x000CEFF0
		internal VillageType VillageTypeSilverMine { get; private set; }

		// Token: 0x06003281 RID: 12929 RVA: 0x000D0DF9 File Offset: 0x000CEFF9
		public DefaultVillageTypes()
		{
			this.ConsumableRawItems = new List<ItemObject>();
			this.RegisterAll();
			this.AddProductions();
		}

		// Token: 0x06003282 RID: 12930 RVA: 0x000D0E18 File Offset: 0x000CF018
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

		// Token: 0x06003283 RID: 12931 RVA: 0x000D0FA1 File Offset: 0x000CF1A1
		private VillageType Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<VillageType>(new VillageType(stringId));
		}

		// Token: 0x06003284 RID: 12932 RVA: 0x000D0FB8 File Offset: 0x000CF1B8
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

		// Token: 0x06003285 RID: 12933 RVA: 0x000D1574 File Offset: 0x000CF774
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

		// Token: 0x06003286 RID: 12934 RVA: 0x000D1DCA File Offset: 0x000CFFCA
		private void AddProductions(VillageType villageType, ValueTuple<string, float>[] productions)
		{
			villageType.AddProductions(productions.Select((ValueTuple<string, float> p) => new ValueTuple<ItemObject, float>(Game.Current.ObjectManager.GetObject<ItemObject>(p.Item1), p.Item2)));
		}
	}
}
