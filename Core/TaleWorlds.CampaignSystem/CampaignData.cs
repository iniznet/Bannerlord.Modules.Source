using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	public static class CampaignData
	{
		public static Clan NeutralFaction
		{
			get
			{
				if (CampaignData._neutralFaction != null)
				{
					return CampaignData._neutralFaction;
				}
				foreach (Clan clan in Clan.All)
				{
					if (clan.StringId == "neutral")
					{
						CampaignData._neutralFaction = clan;
						return clan;
					}
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignData.cs", "NeutralFaction", 216);
				return null;
			}
			private set
			{
				CampaignData._neutralFaction = value;
			}
		}

		public static void OnGameEnd()
		{
			CampaignData.NeutralFaction = null;
		}

		public const string MainHeroTag = "main_hero";

		public const string PlayerTag = "spawnpoint_player";

		public const string PlayerConversationTag = "sp_player_conversation";

		public const string PlayerOutsideTag = "spawnpoint_player_outside";

		public const string PlayerNearTownMainGate = "sp_outside_near_town_main_gate";

		public const string PlayerPrisonBreakTag = "sp_prison_break";

		public const string PlayerAmbushTag = "ambush_spot";

		public const string TournamentMasterTag = "spawnpoint_tournamentmaster";

		public const string PlayerNearArenaMasterTag = "sp_player_near_arena_master";

		public const string ArcherySetTag = "tournament_archery";

		public const string FightSetTag = "tournament_fight";

		public const string JoustingSetTag = "tournament_jousting";

		public const string TavernWenchTag = "sp_tavern_wench";

		public const string TavernKeeperTag = "spawnpoint_tavernkeeper";

		public const string MercenaryTag = "spawnpoint_mercenary";

		public const string MusicianTag = "musician";

		public const string DesertWarHorseSpawnpointTag = "desert_war_horse";

		public const string SteppeChargerSpawnpointTag = "steppe_charger";

		public const string WarHorseSpawnpointTag = "war_horse";

		public const string HorseChargerSpawnpointTag = "charger";

		public const string DesertHorseSpawnpointTag = "desert_horse";

		public const string HunterHorseSpawnpointTag = "hunter";

		public const string SheepSpawnpointTag = "sp_sheep";

		public const string CowSpawnpointTag = "sp_cow";

		public const string HogSpawnpointTag = "sp_hog";

		public const string GooseSpawnpointTag = "sp_goose";

		public const string ChickenSpawnpointTag = "sp_chicken";

		public const string CatSpawnpointTag = "sp_cat";

		public const string DogSpawnpointTag = "sp_dog";

		public const string HorseSpawnpointTag = "sp_horse";

		public const string SpawnpointNPCTag = "sp_npc";

		public const string GuardTag = "sp_guard";

		public const string GuardWithSpearTag = "sp_guard_with_spear";

		public const string GuardPatrolTag = "sp_guard_patrol";

		public const string PrisonGuard = "sp_prison_guard";

		public const string GuardLordsHallGateTag = "sp_guard_castle";

		public const string UnarmedGuardTag = "sp_guard_unarmed";

		public const string TraderTag = "sp_merchant";

		public const string HorseTraderTag = "sp_horse_merchant";

		public const string ArmorerTag = "sp_armorer";

		public const string BarberTag = "sp_barber";

		public const string WeaponSmithTag = "sp_weaponsmith";

		public const string BlacksmithTag = "sp_blacksmith";

		public const string DancerTag = "npc_dancer";

		public const string BeggarTag = "npc_beggar";

		public const string GovernorTag = "sp_governor";

		public const string CleanerTag = "spawnpoint_cleaner";

		public const string WorkshopSellerTag = "sp_shop_seller";

		public const string NpcCommonTag = "npc_common";

		public const string NpcCommonLimitedTag = "npc_common_limited";

		public const string ElderTag = "spawnpoint_elder";

		public const string IdleTag = "npc_idle";

		public const string GamblerTag = "gambler_npc";

		public const string ReservedTag = "reserved";

		public const string HiddenSpawnPointTag = "sp_common_hidden";

		public const string PassageTag = "npc_passage";

		public const string DisableAtNightTag = "disable_at_night";

		public const string EnableAtNightTag = "enable_at_night";

		public const string KingTag = "sp_king";

		public const string ThroneTag = "sp_throne";

		public const string SpawnPointCleanerTag = "spawnpoint_cleaner";

		public const string NpcAmbushTag = "spawnpoint_thug";

		public const string NpcSneakTag = "spawnpoint_npc_sneak";

		public const string NavigationMeshDeactivatorTag = "navigation_mesh_deactivator";

		public const string BattleSetTag = "battle_set";

		public const string AlleyMarker = "alley_marker";

		public const string Alley1Tag = "alley_1";

		public const string Alley2Tag = "alley_2";

		public const string Alley3Tag = "alley_3";

		public const string WorkshopAreaMarkerTag = "workshop_area_marker";

		public const string WorkshopSignTag = "shop_sign";

		public const string PrisonerTag = "sp_prisoner";

		public const string PrisonerGuardTag = "sp_prison_guard";

		public const string NotableTag = "sp_notable";

		public const string NotableGangLeaderTag = "sp_notable_gangleader";

		public const string NotableRuralNotableTag = "sp_notable_rural_notable";

		public const string NotablePreacherTag = "sp_notable_preacher";

		public const string NotableArtisanTag = "sp_notable_artisan";

		public const string NotableMerchantTag = "sp_notable_merchant";

		public const string GangLeaderBodyGuard = "sp_gangleader_bodyguard";

		public const string MerchantNotary = "sp_merchant_notary";

		public const string ArtisanNotary = "sp_artisan_notary";

		public const string PreacherNotary = "sp_preacher_notary";

		public const string RuralNotableNotary = "sp_rural_notable_notary";

		public const string PrisonBreakPrisonerTag = "sp_prison_break_prisoner";

		public const string HermitTag = "sp_hermit";

		public const string Level1Tag = "level_1";

		public const string Level2Tag = "level_2";

		public const string Level3Tag = "level_3";

		public const string CivilianTag = "civilian";

		public const string PrisonBreakLevelTag = "prison_break";

		public const string SiegeTag = "siege";

		public const string RaidTag = "raid";

		public const string BurnedTag = "burned";

		public const string PlayerStealthTag = "sp_player_stealth";

		public const string Shop1Tag = "shop_1";

		public const string Shop2Tag = "shop_2";

		public const string Shop3Tag = "shop_3";

		public const string Shop4Tag = "shop_4";

		public const string CultureEmpire = "empire";

		public const string CultureSturgia = "sturgia";

		public const string CultureAserai = "aserai";

		public const string CultureVlandia = "vlandia";

		public const string CultureBattania = "battania";

		public const string CultureKhuzait = "khuzait";

		public const string CultureNord = "nord";

		public const string CultureDarshi = "darshi";

		public const string CultureVakken = "vakken";

		public const string CultureNeutral = "neutral_culture";

		public const string CultureForestHideout = "forest_bandits";

		public const string CultureSeaHideout = "sea_raiders";

		public const string CultureMountainHideout = "mountain_bandits";

		public const string CultureDesertHideout = "desert_bandits";

		public const string CultureSteppeHideout = "steppe_bandits";

		public const string Looters = "looters";

		public const string LocationCenter = "center";

		public const string LocationArena = "arena";

		public const string LocationPrison = "prison";

		public const string LocationLordsHall = "lordshall";

		public const string LocationTavern = "tavern";

		public const string LocationVillageCenter = "village_center";

		public const string LocationHouse1 = "house_1";

		public const string LocationHouse2 = "house_2";

		public const string LocationHouse3 = "house_3";

		public const string LocationAlley = "alley";

		public const string RetreatSettlement = "retirement_retreat";

		public const string LameHorseModifier = "lame_horse";

		public const int MinFactionNameLength = 1;

		public const int MaxFactionNameLength = 50;

		public const int CampaignStartYear = 1084;

		public const uint NeutralColor1 = 4291609515U;

		public const uint NeutralColor2 = 4291609515U;

		public const uint NeutralAlternativeColor1 = 4291609515U;

		public const uint NeutralAlternativeColor2 = 4291609515U;

		public static readonly uint[] EmpireHeroClothColors = new uint[]
		{
			4291528279U, 4290276438U, 4290269521U, 4284852366U, 4286228910U, 4283542194U, 4286151096U, 4284963729U, 4288833640U, 4288572253U,
			4287586439U, 4285745029U, 4286071139U
		};

		public static readonly uint[] SturgiaHeroClothColors = new uint[]
		{
			4290289001U, 4288652894U, 4287868002U, 4285442652U, 4287080046U, 4284842410U, 4285047948U, 4283010986U, 4282619302U, 4285512133U,
			4290148171U, 4290086464U, 4291270999U
		};

		public static readonly uint[] AseraiHeroClothColors = new uint[]
		{
			4290276438U, 4290148171U, 4291528279U, 4282880383U, 4289758794U, 4281434977U, 4290879574U, 4292654718U, 4291735684U, 4290285174U,
			4290475610U, 4288438863U, 4290472789U, 4283397725U, 4283144585U
		};

		public static readonly uint[] VlandiaHeroClothColors = new uint[]
		{
			4289761132U, 4289958756U, 4290610530U, 4290085254U, 4288438646U, 4288115639U, 4284835242U, 4285571727U, 4288569965U, 4286230451U,
			4290080353U, 4288911473U, 4287930717U, 4284918381U
		};

		public static readonly uint[] BattaniaHeroClothColors = new uint[]
		{
			4285839483U, 4287282028U, 4288129365U, 4290221889U, 4289887020U, 4288977744U, 4289499237U, 4290220376U, 4291855694U, 4288832825U,
			4281894195U, 4286027611U, 4285364809U
		};

		public static readonly uint[] KhuzaitHeroClothColors = new uint[]
		{
			4286362809U, 4285633960U, 4285642139U, 4289629300U, 4290874464U, 4289626441U, 4289100112U, 4290356599U, 4291141740U, 4291539052U,
			4290227326U, 4291016062U
		};

		private static Clan _neutralFaction;

		public static readonly CampaignTime CampaignStartTime = CampaignTime.Years(1084f) + CampaignTime.Weeks(3f) + CampaignTime.Hours(9f);
	}
}
