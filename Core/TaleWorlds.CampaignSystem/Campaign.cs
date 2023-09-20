using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000028 RID: 40
	public class Campaign : GameType
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600016D RID: 365 RVA: 0x0000F0B8 File Offset: 0x0000D2B8
		// (set) Token: 0x0600016E RID: 366 RVA: 0x0000F0BF File Offset: 0x0000D2BF
		public static float MapDiagonal { get; private set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600016F RID: 367 RVA: 0x0000F0C7 File Offset: 0x0000D2C7
		// (set) Token: 0x06000170 RID: 368 RVA: 0x0000F0CE File Offset: 0x0000D2CE
		public static float MapDiagonalSquared { get; private set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000171 RID: 369 RVA: 0x0000F0D6 File Offset: 0x0000D2D6
		// (set) Token: 0x06000172 RID: 370 RVA: 0x0000F0DD File Offset: 0x0000D2DD
		public static float MaximumDistanceBetweenTwoSettlements { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000173 RID: 371 RVA: 0x0000F0E5 File Offset: 0x0000D2E5
		// (set) Token: 0x06000174 RID: 372 RVA: 0x0000F0EC File Offset: 0x0000D2EC
		public static Vec2 MapMinimumPosition { get; private set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000175 RID: 373 RVA: 0x0000F0F4 File Offset: 0x0000D2F4
		// (set) Token: 0x06000176 RID: 374 RVA: 0x0000F0FB File Offset: 0x0000D2FB
		public static Vec2 MapMaximumPosition { get; private set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000F103 File Offset: 0x0000D303
		// (set) Token: 0x06000178 RID: 376 RVA: 0x0000F10A File Offset: 0x0000D30A
		public static float MapMaximumHeight { get; private set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000179 RID: 377 RVA: 0x0000F112 File Offset: 0x0000D312
		// (set) Token: 0x0600017A RID: 378 RVA: 0x0000F119 File Offset: 0x0000D319
		public static float AverageDistanceBetweenTwoFortifications { get; private set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600017B RID: 379 RVA: 0x0000F121 File Offset: 0x0000D321
		// (set) Token: 0x0600017C RID: 380 RVA: 0x0000F129 File Offset: 0x0000D329
		[CachedData]
		public float AverageWage { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600017D RID: 381 RVA: 0x0000F132 File Offset: 0x0000D332
		public string NewGameVersion
		{
			get
			{
				return this._newGameVersion;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600017E RID: 382 RVA: 0x0000F13A File Offset: 0x0000D33A
		public MBReadOnlyList<string> PreviouslyUsedModules
		{
			get
			{
				return this._previouslyUsedModules;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600017F RID: 383 RVA: 0x0000F142 File Offset: 0x0000D342
		public MBReadOnlyList<string> UsedGameVersions
		{
			get
			{
				return this._usedGameVersions;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000180 RID: 384 RVA: 0x0000F14A File Offset: 0x0000D34A
		// (set) Token: 0x06000181 RID: 385 RVA: 0x0000F152 File Offset: 0x0000D352
		[SaveableProperty(82)]
		public string PlatformID { get; private set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000182 RID: 386 RVA: 0x0000F15B File Offset: 0x0000D35B
		// (set) Token: 0x06000183 RID: 387 RVA: 0x0000F163 File Offset: 0x0000D363
		internal CampaignEventDispatcher CampaignEventDispatcher { get; private set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000184 RID: 388 RVA: 0x0000F16C File Offset: 0x0000D36C
		// (set) Token: 0x06000185 RID: 389 RVA: 0x0000F174 File Offset: 0x0000D374
		[SaveableProperty(80)]
		public string UniqueGameId { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000186 RID: 390 RVA: 0x0000F17D File Offset: 0x0000D37D
		// (set) Token: 0x06000187 RID: 391 RVA: 0x0000F185 File Offset: 0x0000D385
		public SaveHandler SaveHandler { get; private set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000188 RID: 392 RVA: 0x0000F18E File Offset: 0x0000D38E
		public override bool SupportsSaving
		{
			get
			{
				return this.GameMode == CampaignGameMode.Campaign;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000189 RID: 393 RVA: 0x0000F199 File Offset: 0x0000D399
		// (set) Token: 0x0600018A RID: 394 RVA: 0x0000F1A1 File Offset: 0x0000D3A1
		[SaveableProperty(211)]
		public CampaignObjectManager CampaignObjectManager { get; private set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600018B RID: 395 RVA: 0x0000F1AA File Offset: 0x0000D3AA
		public override bool IsDevelopment
		{
			get
			{
				return this.GameMode == CampaignGameMode.Tutorial;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600018C RID: 396 RVA: 0x0000F1B5 File Offset: 0x0000D3B5
		// (set) Token: 0x0600018D RID: 397 RVA: 0x0000F1BD File Offset: 0x0000D3BD
		[SaveableProperty(3)]
		public bool IsCraftingEnabled { get; set; } = true;

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600018E RID: 398 RVA: 0x0000F1C6 File Offset: 0x0000D3C6
		// (set) Token: 0x0600018F RID: 399 RVA: 0x0000F1CE File Offset: 0x0000D3CE
		[SaveableProperty(4)]
		public bool IsBannerEditorEnabled { get; set; } = true;

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000190 RID: 400 RVA: 0x0000F1D7 File Offset: 0x0000D3D7
		// (set) Token: 0x06000191 RID: 401 RVA: 0x0000F1DF File Offset: 0x0000D3DF
		[SaveableProperty(5)]
		public bool IsFaceGenEnabled { get; set; } = true;

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000192 RID: 402 RVA: 0x0000F1E8 File Offset: 0x0000D3E8
		public ICampaignBehaviorManager CampaignBehaviorManager
		{
			get
			{
				return this._campaignBehaviorManager;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000193 RID: 403 RVA: 0x0000F1F0 File Offset: 0x0000D3F0
		// (set) Token: 0x06000194 RID: 404 RVA: 0x0000F1F8 File Offset: 0x0000D3F8
		[SaveableProperty(8)]
		public QuestManager QuestManager { get; private set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000195 RID: 405 RVA: 0x0000F201 File Offset: 0x0000D401
		// (set) Token: 0x06000196 RID: 406 RVA: 0x0000F209 File Offset: 0x0000D409
		[SaveableProperty(9)]
		public IssueManager IssueManager { get; private set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000F212 File Offset: 0x0000D412
		// (set) Token: 0x06000198 RID: 408 RVA: 0x0000F21A File Offset: 0x0000D41A
		[SaveableProperty(11)]
		public FactionManager FactionManager { get; private set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000199 RID: 409 RVA: 0x0000F223 File Offset: 0x0000D423
		// (set) Token: 0x0600019A RID: 410 RVA: 0x0000F22B File Offset: 0x0000D42B
		[SaveableProperty(12)]
		public CharacterRelationManager CharacterRelationManager { get; private set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000F234 File Offset: 0x0000D434
		// (set) Token: 0x0600019C RID: 412 RVA: 0x0000F23C File Offset: 0x0000D43C
		[SaveableProperty(14)]
		public Romance Romance { get; private set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000F245 File Offset: 0x0000D445
		// (set) Token: 0x0600019E RID: 414 RVA: 0x0000F24D File Offset: 0x0000D44D
		[SaveableProperty(16)]
		public PlayerCaptivity PlayerCaptivity { get; private set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600019F RID: 415 RVA: 0x0000F256 File Offset: 0x0000D456
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x0000F25E File Offset: 0x0000D45E
		[SaveableProperty(17)]
		internal Clan PlayerDefaultFaction { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000F267 File Offset: 0x0000D467
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x0000F26F File Offset: 0x0000D46F
		public CampaignMission.ICampaignMissionManager CampaignMissionManager { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000F278 File Offset: 0x0000D478
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x0000F280 File Offset: 0x0000D480
		public ISkillLevelingManager SkillLevelingManager { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000F289 File Offset: 0x0000D489
		// (set) Token: 0x060001A6 RID: 422 RVA: 0x0000F291 File Offset: 0x0000D491
		public IMapSceneCreator MapSceneCreator { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000F29A File Offset: 0x0000D49A
		public override bool IsInventoryAccessibleAtMission
		{
			get
			{
				return this.GameMode == CampaignGameMode.Tutorial;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000F2A5 File Offset: 0x0000D4A5
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x0000F2AD File Offset: 0x0000D4AD
		public GameMenuCallbackManager GameMenuCallbackManager { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0000F2B6 File Offset: 0x0000D4B6
		// (set) Token: 0x060001AB RID: 427 RVA: 0x0000F2BE File Offset: 0x0000D4BE
		public VisualCreator VisualCreator { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001AC RID: 428 RVA: 0x0000F2C7 File Offset: 0x0000D4C7
		// (set) Token: 0x060001AD RID: 429 RVA: 0x0000F2CF File Offset: 0x0000D4CF
		[SaveableProperty(28)]
		public MapStateData MapStateData { get; private set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001AE RID: 430 RVA: 0x0000F2D8 File Offset: 0x0000D4D8
		// (set) Token: 0x060001AF RID: 431 RVA: 0x0000F2E0 File Offset: 0x0000D4E0
		public DefaultPerks DefaultPerks { get; private set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000F2E9 File Offset: 0x0000D4E9
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x0000F2F1 File Offset: 0x0000D4F1
		public DefaultTraits DefaultTraits { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x0000F2FA File Offset: 0x0000D4FA
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x0000F302 File Offset: 0x0000D502
		public DefaultPolicies DefaultPolicies { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000F30B File Offset: 0x0000D50B
		// (set) Token: 0x060001B5 RID: 437 RVA: 0x0000F313 File Offset: 0x0000D513
		public DefaultBuildingTypes DefaultBuildingTypes { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x0000F31C File Offset: 0x0000D51C
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x0000F324 File Offset: 0x0000D524
		public DefaultIssueEffects DefaultIssueEffects { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x0000F32D File Offset: 0x0000D52D
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x0000F335 File Offset: 0x0000D535
		public DefaultItems DefaultItems { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001BA RID: 442 RVA: 0x0000F33E File Offset: 0x0000D53E
		// (set) Token: 0x060001BB RID: 443 RVA: 0x0000F346 File Offset: 0x0000D546
		public DefaultSiegeStrategies DefaultSiegeStrategies { get; private set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001BC RID: 444 RVA: 0x0000F34F File Offset: 0x0000D54F
		// (set) Token: 0x060001BD RID: 445 RVA: 0x0000F357 File Offset: 0x0000D557
		internal MBReadOnlyList<PerkObject> AllPerks { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001BE RID: 446 RVA: 0x0000F360 File Offset: 0x0000D560
		// (set) Token: 0x060001BF RID: 447 RVA: 0x0000F368 File Offset: 0x0000D568
		public PlayerUpdateTracker PlayerUpdateTracker { get; private set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000F371 File Offset: 0x0000D571
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x0000F379 File Offset: 0x0000D579
		public DefaultSkillEffects DefaultSkillEffects { get; private set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x0000F382 File Offset: 0x0000D582
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x0000F38A File Offset: 0x0000D58A
		public DefaultVillageTypes DefaultVillageTypes { get; private set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x0000F393 File Offset: 0x0000D593
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x0000F39B File Offset: 0x0000D59B
		internal MBReadOnlyList<TraitObject> AllTraits { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x0000F3A4 File Offset: 0x0000D5A4
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x0000F3AC File Offset: 0x0000D5AC
		internal MBReadOnlyList<MBEquipmentRoster> AllEquipmentRosters { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000F3B5 File Offset: 0x0000D5B5
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x0000F3BD File Offset: 0x0000D5BD
		public DefaultCulturalFeats DefaultFeats { get; private set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001CA RID: 458 RVA: 0x0000F3C6 File Offset: 0x0000D5C6
		// (set) Token: 0x060001CB RID: 459 RVA: 0x0000F3CE File Offset: 0x0000D5CE
		internal MBReadOnlyList<PolicyObject> AllPolicies { get; private set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001CC RID: 460 RVA: 0x0000F3D7 File Offset: 0x0000D5D7
		// (set) Token: 0x060001CD RID: 461 RVA: 0x0000F3DF File Offset: 0x0000D5DF
		internal MBReadOnlyList<BuildingType> AllBuildingTypes { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0000F3E8 File Offset: 0x0000D5E8
		// (set) Token: 0x060001CF RID: 463 RVA: 0x0000F3F0 File Offset: 0x0000D5F0
		internal MBReadOnlyList<IssueEffect> AllIssueEffects { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000F3F9 File Offset: 0x0000D5F9
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x0000F401 File Offset: 0x0000D601
		internal MBReadOnlyList<SiegeStrategy> AllSiegeStrategies { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x0000F40A File Offset: 0x0000D60A
		// (set) Token: 0x060001D3 RID: 467 RVA: 0x0000F412 File Offset: 0x0000D612
		internal MBReadOnlyList<VillageType> AllVillageTypes { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000F41B File Offset: 0x0000D61B
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x0000F423 File Offset: 0x0000D623
		internal MBReadOnlyList<SkillEffect> AllSkillEffects { get; private set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000F42C File Offset: 0x0000D62C
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x0000F434 File Offset: 0x0000D634
		internal MBReadOnlyList<FeatObject> AllFeats { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000F43D File Offset: 0x0000D63D
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000F445 File Offset: 0x0000D645
		internal MBReadOnlyList<SkillObject> AllSkills { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000F44E File Offset: 0x0000D64E
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000F456 File Offset: 0x0000D656
		internal MBReadOnlyList<SiegeEngineType> AllSiegeEngineTypes { get; private set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001DC RID: 476 RVA: 0x0000F45F File Offset: 0x0000D65F
		// (set) Token: 0x060001DD RID: 477 RVA: 0x0000F467 File Offset: 0x0000D667
		internal MBReadOnlyList<ItemCategory> AllItemCategories { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000F470 File Offset: 0x0000D670
		// (set) Token: 0x060001DF RID: 479 RVA: 0x0000F478 File Offset: 0x0000D678
		internal MBReadOnlyList<CharacterAttribute> AllCharacterAttributes { get; private set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000F481 File Offset: 0x0000D681
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x0000F489 File Offset: 0x0000D689
		internal MBReadOnlyList<ItemObject> AllItems { get; private set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000F492 File Offset: 0x0000D692
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x0000F49A File Offset: 0x0000D69A
		[SaveableProperty(100)]
		internal MapTimeTracker MapTimeTracker { get; private set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000F4A3 File Offset: 0x0000D6A3
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x0000F4AB File Offset: 0x0000D6AB
		public bool TimeControlModeLock { get; private set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000F4B4 File Offset: 0x0000D6B4
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000F4BC File Offset: 0x0000D6BC
		public CampaignTimeControlMode TimeControlMode
		{
			get
			{
				return this._timeControlMode;
			}
			set
			{
				if (!this.TimeControlModeLock && value != this._timeControlMode)
				{
					this._timeControlMode = value;
				}
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000F4D6 File Offset: 0x0000D6D6
		// (set) Token: 0x060001E9 RID: 489 RVA: 0x0000F4DE File Offset: 0x0000D6DE
		public bool IsMapTooltipLongForm { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001EA RID: 490 RVA: 0x0000F4E7 File Offset: 0x0000D6E7
		// (set) Token: 0x060001EB RID: 491 RVA: 0x0000F4EF File Offset: 0x0000D6EF
		public float SpeedUpMultiplier { get; set; } = 4f;

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001EC RID: 492 RVA: 0x0000F4F8 File Offset: 0x0000D6F8
		public float CampaignDt
		{
			get
			{
				return this._dt;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001ED RID: 493 RVA: 0x0000F500 File Offset: 0x0000D700
		// (set) Token: 0x060001EE RID: 494 RVA: 0x0000F508 File Offset: 0x0000D708
		public bool TrueSight { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000F511 File Offset: 0x0000D711
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x0000F518 File Offset: 0x0000D718
		public static Campaign Current { get; private set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000F520 File Offset: 0x0000D720
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x0000F528 File Offset: 0x0000D728
		[SaveableProperty(36)]
		public CampaignTime CampaignStartTime { get; private set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x0000F531 File Offset: 0x0000D731
		// (set) Token: 0x060001F4 RID: 500 RVA: 0x0000F539 File Offset: 0x0000D739
		[SaveableProperty(37)]
		public CampaignGameMode GameMode { get; private set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x0000F542 File Offset: 0x0000D742
		// (set) Token: 0x060001F6 RID: 502 RVA: 0x0000F54A File Offset: 0x0000D74A
		[SaveableProperty(38)]
		public float PlayerProgress { get; private set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x0000F553 File Offset: 0x0000D753
		// (set) Token: 0x060001F8 RID: 504 RVA: 0x0000F55B File Offset: 0x0000D75B
		public GameMenuManager GameMenuManager { get; private set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x0000F564 File Offset: 0x0000D764
		public GameModels Models
		{
			get
			{
				return this._gameModels;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000F56C File Offset: 0x0000D76C
		// (set) Token: 0x060001FB RID: 507 RVA: 0x0000F574 File Offset: 0x0000D774
		public SandBoxManager SandBoxManager { get; private set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001FC RID: 508 RVA: 0x0000F57D File Offset: 0x0000D77D
		public Campaign.GameLoadingType CampaignGameLoadingType
		{
			get
			{
				return this._gameLoadingType;
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000F588 File Offset: 0x0000D788
		public Campaign(CampaignGameMode gameMode)
		{
			this.GameMode = gameMode;
			this.Options = new CampaignOptions();
			this.MapTimeTracker = new MapTimeTracker(CampaignData.CampaignStartTime);
			this.CampaignStartTime = this.MapTimeTracker.Now;
			this.CampaignObjectManager = new CampaignObjectManager();
			this.CurrentConversationContext = ConversationContext.Default;
			this.QuestManager = new QuestManager();
			this.IssueManager = new IssueManager();
			this.FactionManager = new FactionManager();
			this.CharacterRelationManager = new CharacterRelationManager();
			this.Romance = new Romance();
			this.PlayerCaptivity = new PlayerCaptivity();
			this.BarterManager = new BarterManager();
			this.GameMenuCallbackManager = new GameMenuCallbackManager();
			this._campaignPeriodicEventManager = new CampaignPeriodicEventManager();
			this._tickData = new CampaignTickCacheDataStore();
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001FE RID: 510 RVA: 0x0000F687 File Offset: 0x0000D887
		// (set) Token: 0x060001FF RID: 511 RVA: 0x0000F68F File Offset: 0x0000D88F
		[SaveableProperty(40)]
		public SiegeEventManager SiegeEventManager { get; internal set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000F698 File Offset: 0x0000D898
		// (set) Token: 0x06000201 RID: 513 RVA: 0x0000F6A0 File Offset: 0x0000D8A0
		[SaveableProperty(41)]
		public MapEventManager MapEventManager { get; internal set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000F6A9 File Offset: 0x0000D8A9
		// (set) Token: 0x06000203 RID: 515 RVA: 0x0000F6B1 File Offset: 0x0000D8B1
		internal CampaignEvents CampaignEvents { get; private set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000204 RID: 516 RVA: 0x0000F6BC File Offset: 0x0000D8BC
		public MenuContext CurrentMenuContext
		{
			get
			{
				GameStateManager gameStateManager = base.CurrentGame.GameStateManager;
				TutorialState tutorialState = gameStateManager.ActiveState as TutorialState;
				if (tutorialState != null)
				{
					return tutorialState.MenuContext;
				}
				MapState mapState = gameStateManager.ActiveState as MapState;
				if (mapState != null)
				{
					return mapState.MenuContext;
				}
				MapState mapState2;
				if (gameStateManager.ActiveState.Predecessor != null && (mapState2 = gameStateManager.ActiveState.Predecessor as MapState) != null)
				{
					return mapState2.MenuContext;
				}
				return null;
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000205 RID: 517 RVA: 0x0000F72A File Offset: 0x0000D92A
		// (set) Token: 0x06000206 RID: 518 RVA: 0x0000F732 File Offset: 0x0000D932
		internal List<MBCampaignEvent> CustomPeriodicCampaignEvents { get; private set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000207 RID: 519 RVA: 0x0000F73B File Offset: 0x0000D93B
		// (set) Token: 0x06000208 RID: 520 RVA: 0x0000F743 File Offset: 0x0000D943
		public bool IsMainPartyWaiting
		{
			get
			{
				return this._isMainPartyWaiting;
			}
			private set
			{
				this._isMainPartyWaiting = value;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000209 RID: 521 RVA: 0x0000F74C File Offset: 0x0000D94C
		// (set) Token: 0x0600020A RID: 522 RVA: 0x0000F754 File Offset: 0x0000D954
		[SaveableProperty(45)]
		private int _curMapFrame { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600020B RID: 523 RVA: 0x0000F760 File Offset: 0x0000D960
		internal LocatorGrid<Settlement> SettlementLocator
		{
			get
			{
				LocatorGrid<Settlement> locatorGrid;
				if ((locatorGrid = this._settlementLocator) == null)
				{
					locatorGrid = (this._settlementLocator = new LocatorGrid<Settlement>(5f, 32, 32));
				}
				return locatorGrid;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0000F790 File Offset: 0x0000D990
		internal LocatorGrid<MobileParty> MobilePartyLocator
		{
			get
			{
				LocatorGrid<MobileParty> locatorGrid;
				if ((locatorGrid = this._mobilePartyLocator) == null)
				{
					locatorGrid = (this._mobilePartyLocator = new LocatorGrid<MobileParty>(5f, 32, 32));
				}
				return locatorGrid;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600020D RID: 525 RVA: 0x0000F7BE File Offset: 0x0000D9BE
		public IMapScene MapSceneWrapper
		{
			get
			{
				return this._mapSceneWrapper;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0000F7C6 File Offset: 0x0000D9C6
		// (set) Token: 0x0600020F RID: 527 RVA: 0x0000F7CE File Offset: 0x0000D9CE
		[SaveableProperty(54)]
		public PlayerEncounter PlayerEncounter { get; internal set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0000F7D7 File Offset: 0x0000D9D7
		// (set) Token: 0x06000211 RID: 529 RVA: 0x0000F7DF File Offset: 0x0000D9DF
		[CachedData]
		internal LocationEncounter LocationEncounter { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000F7E8 File Offset: 0x0000D9E8
		// (set) Token: 0x06000213 RID: 531 RVA: 0x0000F7F0 File Offset: 0x0000D9F0
		internal NameGenerator NameGenerator { get; private set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0000F7F9 File Offset: 0x0000D9F9
		// (set) Token: 0x06000215 RID: 533 RVA: 0x0000F801 File Offset: 0x0000DA01
		[SaveableProperty(58)]
		public BarterManager BarterManager { get; private set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000216 RID: 534 RVA: 0x0000F80A File Offset: 0x0000DA0A
		// (set) Token: 0x06000217 RID: 535 RVA: 0x0000F812 File Offset: 0x0000DA12
		[SaveableProperty(69)]
		public bool IsMainHeroDisguised { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000218 RID: 536 RVA: 0x0000F81B File Offset: 0x0000DA1B
		// (set) Token: 0x06000219 RID: 537 RVA: 0x0000F823 File Offset: 0x0000DA23
		[SaveableProperty(70)]
		public bool DesertionEnabled { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600021A RID: 538 RVA: 0x0000F82C File Offset: 0x0000DA2C
		public Vec2 DefaultStartingPosition
		{
			get
			{
				return new Vec2(685.3f, 410.9f);
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000F840 File Offset: 0x0000DA40
		public void InitializeMainParty()
		{
			this.InitializeSinglePlayerReferences();
			this.MainParty.InitializeMobilePartyAtPosition(base.CurrentGame.ObjectManager.GetObject<PartyTemplateObject>("main_hero_party_template"), this.DefaultStartingPosition, -1);
			this.MainParty.ActualClan = Clan.PlayerClan;
			this.MainParty.PartyComponent = new LordPartyComponent(Hero.MainHero, Hero.MainHero);
			this.MainParty.ItemRoster.AddToCounts(DefaultItems.Grain, 1);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000F8BB File Offset: 0x0000DABB
		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData, ObjectLoadData objectLoadData)
		{
			this._campaignEntitySystem = new EntitySystem<CampaignEntityComponent>();
			this.PlayerFormationPreferences = this._playerFormationPreferences.GetReadOnlyDictionary<CharacterObject, FormationClass>();
			this.SpeedUpMultiplier = 4f;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000F8E4 File Offset: 0x0000DAE4
		private void InitializeForSavedGame()
		{
			foreach (CampaignEntityComponent campaignEntityComponent in this._campaignEntitySystem.GetComponents())
			{
				campaignEntityComponent.OnLoadSavedGame();
			}
			foreach (Settlement settlement in Settlement.All)
			{
				settlement.Party.OnFinishLoadState();
			}
			foreach (MobileParty mobileParty in this.MobileParties.ToList<MobileParty>())
			{
				mobileParty.Party.OnFinishLoadState();
			}
			foreach (Settlement settlement2 in Settlement.All)
			{
				settlement2.OnFinishLoadState();
			}
			this.GameMenuCallbackManager = new GameMenuCallbackManager();
			this.GameMenuCallbackManager.OnGameLoad();
			this.IssueManager.InitializeForSavedGame();
			this.MinSettlementX = 1000f;
			this.MinSettlementY = 1000f;
			foreach (Settlement settlement3 in Settlement.All)
			{
				if (settlement3.Position2D.x < this.MinSettlementX)
				{
					this.MinSettlementX = settlement3.Position2D.x;
				}
				if (settlement3.Position2D.y < this.MinSettlementY)
				{
					this.MinSettlementY = settlement3.Position2D.y;
				}
				if (settlement3.Position2D.x > this.MaxSettlementX)
				{
					this.MaxSettlementX = settlement3.Position2D.x;
				}
				if (settlement3.Position2D.y > this.MaxSettlementY)
				{
					this.MaxSettlementY = settlement3.Position2D.y;
				}
			}
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000FB0C File Offset: 0x0000DD0C
		private void OnGameLoaded(CampaignGameStarter starter)
		{
			this._tickData = new CampaignTickCacheDataStore();
			base.ObjectManager.PreAfterLoad();
			this.CampaignObjectManager.PreAfterLoad();
			base.ObjectManager.AfterLoad();
			this.CampaignObjectManager.AfterLoad();
			this.CharacterRelationManager.AfterLoad();
			CampaignEventDispatcher.Instance.OnGameEarlyLoaded(starter);
			TroopRoster.CalculateCachedStatsOnLoad();
			CampaignEventDispatcher.Instance.OnGameLoaded(starter);
			this.InitializeForSavedGame();
			this._tickData.InitializeDataCache();
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000FB88 File Offset: 0x0000DD88
		private void OnDataLoadFinished(CampaignGameStarter starter)
		{
			this._towns = (from x in Settlement.All
				where x.IsTown
				select x.Town).ToMBList<Town>();
			this._castles = (from x in Settlement.All
				where x.IsCastle
				select x.Town).ToMBList<Town>();
			this._villages = (from x in Settlement.All
				where x.Village != null
				select x.Village).ToMBList<Village>();
			this._hideouts = (from x in Settlement.All
				where x.IsHideout
				select x.Hideout).ToMBList<Hideout>();
			this._campaignPeriodicEventManager.InitializeTickers();
			this.CreateCampaignEvents();
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000FD08 File Offset: 0x0000DF08
		private void OnSessionStart(CampaignGameStarter starter)
		{
			CampaignEventDispatcher.Instance.OnSessionStart(starter);
			CampaignEventDispatcher.Instance.OnAfterSessionStart(starter);
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
			this.ConversationManager.Build();
			foreach (Settlement settlement in this.Settlements)
			{
				settlement.OnSessionStart();
			}
			this.IsCraftingEnabled = true;
			this.IsBannerEditorEnabled = true;
			this.IsFaceGenEnabled = true;
			this.MapEventManager.OnAfterLoad();
			this.KingdomManager.RegisterEvents();
			this.KingdomManager.OnNewGameCreated();
			this.CampaignInformationManager.RegisterEvents();
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000FDD4 File Offset: 0x0000DFD4
		private void DailyTickSettlement(Settlement settlement)
		{
			if (settlement.IsVillage)
			{
				settlement.Village.DailyTick();
				return;
			}
			if (settlement.Town != null)
			{
				settlement.Town.DailyTick();
			}
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000FE00 File Offset: 0x0000E000
		private void GameInitTick()
		{
			foreach (Settlement settlement in Settlement.All)
			{
				settlement.Party.UpdateVisibilityAndInspected(0f, false);
			}
			foreach (MobileParty mobileParty in this.MobileParties)
			{
				mobileParty.Party.UpdateVisibilityAndInspected(0f, false);
			}
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000FEA4 File Offset: 0x0000E0A4
		internal void HourlyTick(MBCampaignEvent campaignEvent, object[] delegateParams)
		{
			CampaignEventDispatcher.Instance.HourlyTick();
			MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
			if (mapState == null)
			{
				return;
			}
			mapState.OnHourlyTick();
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000FED0 File Offset: 0x0000E0D0
		internal void DailyTick(MBCampaignEvent campaignEvent, object[] delegateParams)
		{
			this.PlayerProgress = (this.PlayerProgress + Campaign.Current.Models.PlayerProgressionModel.GetPlayerProgress()) / 2f;
			CampaignEventDispatcher.Instance.DailyTick();
			if ((int)this.CampaignStartTime.ElapsedDaysUntilNow % 7 == 0)
			{
				CampaignEventDispatcher.Instance.WeeklyTick();
				this.OnWeeklyTick();
			}
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000FF31 File Offset: 0x0000E131
		public void WaitAsyncTasks()
		{
			if (this.CampaignLateAITickTask != null)
			{
				this.CampaignLateAITickTask.Wait();
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000FF46 File Offset: 0x0000E146
		private void OnWeeklyTick()
		{
			this.LogEntryHistory.DeleteOutdatedLogs();
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000FF54 File Offset: 0x0000E154
		public CampaignTimeControlMode GetSimplifiedTimeControlMode()
		{
			switch (this.TimeControlMode)
			{
			case CampaignTimeControlMode.Stop:
				return CampaignTimeControlMode.Stop;
			case CampaignTimeControlMode.UnstoppablePlay:
				return CampaignTimeControlMode.UnstoppablePlay;
			case CampaignTimeControlMode.UnstoppableFastForward:
			case CampaignTimeControlMode.UnstoppableFastForwardForPartyWaitTime:
				return CampaignTimeControlMode.UnstoppableFastForward;
			case CampaignTimeControlMode.StoppablePlay:
				if (!this.IsMainPartyWaiting)
				{
					return CampaignTimeControlMode.StoppablePlay;
				}
				return CampaignTimeControlMode.Stop;
			case CampaignTimeControlMode.StoppableFastForward:
				if (!this.IsMainPartyWaiting)
				{
					return CampaignTimeControlMode.StoppableFastForward;
				}
				return CampaignTimeControlMode.Stop;
			default:
				return CampaignTimeControlMode.Stop;
			}
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000FFA7 File Offset: 0x0000E1A7
		private void CheckMainPartyNeedsUpdate()
		{
			MobileParty.MainParty.Ai.CheckPartyNeedsUpdate();
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000FFB8 File Offset: 0x0000E1B8
		private void TickMapTime(float realDt)
		{
			float num = 0f;
			float speedUpMultiplier = this.SpeedUpMultiplier;
			float num2 = 0.25f * realDt;
			this.IsMainPartyWaiting = MobileParty.MainParty.ComputeIsWaiting();
			switch (this.TimeControlMode)
			{
			case CampaignTimeControlMode.Stop:
			case CampaignTimeControlMode.FastForwardStop:
				break;
			case CampaignTimeControlMode.UnstoppablePlay:
				num = num2;
				break;
			case CampaignTimeControlMode.UnstoppableFastForward:
			case CampaignTimeControlMode.UnstoppableFastForwardForPartyWaitTime:
				num = num2 * speedUpMultiplier;
				break;
			case CampaignTimeControlMode.StoppablePlay:
				if (!this.IsMainPartyWaiting)
				{
					num = num2;
				}
				break;
			case CampaignTimeControlMode.StoppableFastForward:
				if (!this.IsMainPartyWaiting)
				{
					num = num2 * speedUpMultiplier;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this._dt = num;
			this.MapTimeTracker.Tick(4320f * num);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00010058 File Offset: 0x0000E258
		public void OnGameOver()
		{
			if (CampaignOptions.IsIronmanMode)
			{
				this.SaveHandler.QuickSaveCurrentGame();
			}
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0001006C File Offset: 0x0000E26C
		internal void RealTick(float realDt)
		{
			this.WaitAsyncTasks();
			this.CheckMainPartyNeedsUpdate();
			this.TickMapTime(realDt);
			foreach (CampaignEntityComponent campaignEntityComponent in this._campaignEntitySystem.GetComponents())
			{
				campaignEntityComponent.OnTick(realDt, this._dt);
			}
			if (!this.GameStarted)
			{
				this.GameStarted = true;
				this._tickData.InitializeDataCache();
				this.SiegeEventManager.Tick(this._dt);
			}
			this._tickData.RealTick(this._dt, realDt);
			this.SiegeEventManager.Tick(this._dt);
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600022C RID: 556 RVA: 0x0001012C File Offset: 0x0000E32C
		public static float CurrentTime
		{
			get
			{
				return (float)CampaignTime.Now.ToHours;
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00010148 File Offset: 0x0000E348
		public void SetTimeSpeed(int speed)
		{
			switch (speed)
			{
			case 0:
				if (this.TimeControlMode == CampaignTimeControlMode.UnstoppableFastForward || this.TimeControlMode == CampaignTimeControlMode.StoppableFastForward)
				{
					this.TimeControlMode = CampaignTimeControlMode.FastForwardStop;
					return;
				}
				if (this.TimeControlMode != CampaignTimeControlMode.FastForwardStop && this.TimeControlMode != CampaignTimeControlMode.Stop)
				{
					this.TimeControlMode = CampaignTimeControlMode.Stop;
					return;
				}
				break;
			case 1:
				if (((this.TimeControlMode == CampaignTimeControlMode.Stop || this.TimeControlMode == CampaignTimeControlMode.FastForwardStop) && this.MainParty.DefaultBehavior == AiBehavior.Hold) || this.IsMainPartyWaiting || (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty))
				{
					this.TimeControlMode = CampaignTimeControlMode.UnstoppablePlay;
					return;
				}
				this.TimeControlMode = CampaignTimeControlMode.StoppablePlay;
				return;
			case 2:
				if (((this.TimeControlMode == CampaignTimeControlMode.Stop || this.TimeControlMode == CampaignTimeControlMode.FastForwardStop) && this.MainParty.DefaultBehavior == AiBehavior.Hold) || this.IsMainPartyWaiting || (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty))
				{
					this.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForward;
					return;
				}
				this.TimeControlMode = CampaignTimeControlMode.StoppableFastForward;
				break;
			default:
				return;
			}
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00010250 File Offset: 0x0000E450
		public static void LateAITick()
		{
			Campaign.Current.LateAITickAux();
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0001025C File Offset: 0x0000E45C
		internal void LateAITickAux()
		{
			if (this._dt > 0f || this._curSessionFrame < 3)
			{
				this.PartiesThink(this._dt);
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x00010280 File Offset: 0x0000E480
		public void RegisterFadingVisual(IPartyVisual visual)
		{
			this._tickData.RegisterFadingVisual(visual);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00010290 File Offset: 0x0000E490
		internal void Tick()
		{
			int curMapFrame = this._curMapFrame;
			this._curMapFrame = curMapFrame + 1;
			this._curSessionFrame++;
			if (this._dt > 0f || this._curSessionFrame < 3)
			{
				CampaignEventDispatcher.Instance.Tick(this._dt);
				this._campaignPeriodicEventManager.OnTick(this._dt);
				this.MapEventManager.Tick();
				this._lastNonZeroDtFrame = this._curMapFrame;
				this._campaignPeriodicEventManager.MobilePartyHourlyTick();
			}
			if (this._dt > 0f)
			{
				this._campaignPeriodicEventManager.TickPeriodicEvents();
			}
			this._tickData.Tick();
			Campaign.Current.PlayerCaptivity.Update(this._dt);
			if (this._dt > 0f || (MobileParty.MainParty.MapEvent == null && this._curMapFrame == this._lastNonZeroDtFrame + 1))
			{
				EncounterManager.Tick(this._dt);
				MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
				if (mapState != null && mapState.AtMenu && !mapState.MenuContext.GameMenu.IsWaitActive)
				{
					this._dt = 0f;
				}
			}
			if (this._dt > 0f || this._curSessionFrame < 3)
			{
				this._campaignPeriodicEventManager.TickPartialHourlyAi();
			}
			MapState mapState2;
			if ((mapState2 = Game.Current.GameStateManager.ActiveState as MapState) != null && !mapState2.AtMenu)
			{
				string genericStateMenu = this.Models.EncounterGameMenuModel.GetGenericStateMenu();
				if (!string.IsNullOrEmpty(genericStateMenu))
				{
					GameMenu.ActivateGameMenu(genericStateMenu);
				}
			}
			this.CampaignLateAITickTask.Invoke();
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00010428 File Offset: 0x0000E628
		private void CreateCampaignEvents()
		{
			long numTicks = (CampaignTime.Now - CampaignData.CampaignStartTime).NumTicks;
			CampaignTime campaignTime = CampaignTime.Days(1f);
			if (numTicks % 864000000L != 0L)
			{
				campaignTime = CampaignTime.Days((float)(numTicks % 864000000L) / 864000000f);
			}
			this._dailyTickEvent = CampaignPeriodicEventManager.CreatePeriodicEvent(CampaignTime.Days(1f), campaignTime);
			this._dailyTickEvent.AddHandler(new MBCampaignEvent.CampaignEventDelegate(this.DailyTick));
			CampaignTime campaignTime2 = CampaignTime.Hours(0.5f);
			if (numTicks % 36000000L != 0L)
			{
				campaignTime2 = CampaignTime.Hours((float)(numTicks % 36000000L) / 36000000f);
			}
			this._hourlyTickEvent = CampaignPeriodicEventManager.CreatePeriodicEvent(CampaignTime.Hours(1f), campaignTime2);
			this._hourlyTickEvent.AddHandler(new MBCampaignEvent.CampaignEventDelegate(this.HourlyTick));
		}

		// Token: 0x06000233 RID: 563 RVA: 0x000104FB File Offset: 0x0000E6FB
		public void UnregisterFadingVisual(IPartyVisual visual)
		{
			this._tickData.UnregisterFadingVisual(visual);
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0001050C File Offset: 0x0000E70C
		private void PartiesThink(float dt)
		{
			for (int i = 0; i < this.MobileParties.Count; i++)
			{
				this.MobileParties[i].Ai.Tick(dt);
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00010546 File Offset: 0x0000E746
		public TComponent GetEntityComponent<TComponent>() where TComponent : CampaignEntityComponent
		{
			return this._campaignEntitySystem.GetComponent<TComponent>();
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00010553 File Offset: 0x0000E753
		public TComponent AddEntityComponent<TComponent>() where TComponent : CampaignEntityComponent, new()
		{
			return this._campaignEntitySystem.AddComponent<TComponent>();
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000237 RID: 567 RVA: 0x00010560 File Offset: 0x0000E760
		public MBReadOnlyList<CampaignEntityComponent> CampaignEntityComponents
		{
			get
			{
				return this._campaignEntitySystem.Components;
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0001056D File Offset: 0x0000E76D
		public T GetCampaignBehavior<T>()
		{
			return this._campaignBehaviorManager.GetBehavior<T>();
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0001057A File Offset: 0x0000E77A
		public IEnumerable<T> GetCampaignBehaviors<T>()
		{
			return this._campaignBehaviorManager.GetBehaviors<T>();
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00010587 File Offset: 0x0000E787
		public void AddCampaignBehaviorManager(ICampaignBehaviorManager manager)
		{
			this._campaignBehaviorManager = manager;
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600023B RID: 571 RVA: 0x00010590 File Offset: 0x0000E790
		public MBReadOnlyList<Hero> AliveHeroes
		{
			get
			{
				return this.CampaignObjectManager.AliveHeroes;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600023C RID: 572 RVA: 0x0001059D File Offset: 0x0000E79D
		public MBReadOnlyList<Hero> DeadOrDisabledHeroes
		{
			get
			{
				return this.CampaignObjectManager.DeadOrDisabledHeroes;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600023D RID: 573 RVA: 0x000105AA File Offset: 0x0000E7AA
		public MBReadOnlyList<MobileParty> MobileParties
		{
			get
			{
				return this.CampaignObjectManager.MobileParties;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600023E RID: 574 RVA: 0x000105B7 File Offset: 0x0000E7B7
		public MBReadOnlyList<MobileParty> CaravanParties
		{
			get
			{
				return this.CampaignObjectManager.CaravanParties;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600023F RID: 575 RVA: 0x000105C4 File Offset: 0x0000E7C4
		public MBReadOnlyList<MobileParty> VillagerParties
		{
			get
			{
				return this.CampaignObjectManager.VillagerParties;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000240 RID: 576 RVA: 0x000105D1 File Offset: 0x0000E7D1
		public MBReadOnlyList<MobileParty> MilitiaParties
		{
			get
			{
				return this.CampaignObjectManager.MilitiaParties;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000241 RID: 577 RVA: 0x000105DE File Offset: 0x0000E7DE
		public MBReadOnlyList<MobileParty> GarrisonParties
		{
			get
			{
				return this.CampaignObjectManager.GarrisonParties;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000242 RID: 578 RVA: 0x000105EB File Offset: 0x0000E7EB
		public MBReadOnlyList<MobileParty> CustomParties
		{
			get
			{
				return this.CampaignObjectManager.CustomParties;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000243 RID: 579 RVA: 0x000105F8 File Offset: 0x0000E7F8
		public MBReadOnlyList<MobileParty> LordParties
		{
			get
			{
				return this.CampaignObjectManager.LordParties;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000244 RID: 580 RVA: 0x00010605 File Offset: 0x0000E805
		public MBReadOnlyList<MobileParty> BanditParties
		{
			get
			{
				return this.CampaignObjectManager.BanditParties;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000245 RID: 581 RVA: 0x00010612 File Offset: 0x0000E812
		public MBReadOnlyList<MobileParty> PartiesWithoutPartyComponent
		{
			get
			{
				return this.CampaignObjectManager.PartiesWithoutPartyComponent;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000246 RID: 582 RVA: 0x0001061F File Offset: 0x0000E81F
		public MBReadOnlyList<Settlement> Settlements
		{
			get
			{
				return this.CampaignObjectManager.Settlements;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000247 RID: 583 RVA: 0x0001062C File Offset: 0x0000E82C
		public IEnumerable<IFaction> Factions
		{
			get
			{
				return this.CampaignObjectManager.Factions;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000248 RID: 584 RVA: 0x00010639 File Offset: 0x0000E839
		public MBReadOnlyList<Kingdom> Kingdoms
		{
			get
			{
				return this.CampaignObjectManager.Kingdoms;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000249 RID: 585 RVA: 0x00010646 File Offset: 0x0000E846
		public MBReadOnlyList<Clan> Clans
		{
			get
			{
				return this.CampaignObjectManager.Clans;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600024A RID: 586 RVA: 0x00010653 File Offset: 0x0000E853
		public MBReadOnlyList<CharacterObject> Characters
		{
			get
			{
				return this._characters;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600024B RID: 587 RVA: 0x0001065B File Offset: 0x0000E85B
		public MBReadOnlyList<WorkshopType> Workshops
		{
			get
			{
				return this._workshops;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600024C RID: 588 RVA: 0x00010663 File Offset: 0x0000E863
		public MBReadOnlyList<ItemModifier> ItemModifiers
		{
			get
			{
				return this._itemModifiers;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600024D RID: 589 RVA: 0x0001066B File Offset: 0x0000E86B
		public MBReadOnlyList<ItemModifierGroup> ItemModifierGroups
		{
			get
			{
				return this._itemModifierGroups;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600024E RID: 590 RVA: 0x00010673 File Offset: 0x0000E873
		public MBReadOnlyList<Concept> Concepts
		{
			get
			{
				return this._concepts;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600024F RID: 591 RVA: 0x0001067B File Offset: 0x0000E87B
		// (set) Token: 0x06000250 RID: 592 RVA: 0x00010683 File Offset: 0x0000E883
		[SaveableProperty(60)]
		public MobileParty MainParty { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000251 RID: 593 RVA: 0x0001068C File Offset: 0x0000E88C
		// (set) Token: 0x06000252 RID: 594 RVA: 0x00010694 File Offset: 0x0000E894
		public PartyBase CameraFollowParty
		{
			get
			{
				return this._cameraFollowParty;
			}
			set
			{
				this._cameraFollowParty = value;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000253 RID: 595 RVA: 0x0001069D File Offset: 0x0000E89D
		// (set) Token: 0x06000254 RID: 596 RVA: 0x000106A5 File Offset: 0x0000E8A5
		[SaveableProperty(62)]
		public CampaignInformationManager CampaignInformationManager { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000255 RID: 597 RVA: 0x000106AE File Offset: 0x0000E8AE
		// (set) Token: 0x06000256 RID: 598 RVA: 0x000106B6 File Offset: 0x0000E8B6
		[SaveableProperty(63)]
		public VisualTrackerManager VisualTrackerManager { get; set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000257 RID: 599 RVA: 0x000106BF File Offset: 0x0000E8BF
		public LogEntryHistory LogEntryHistory
		{
			get
			{
				return this._logEntryHistory;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000258 RID: 600 RVA: 0x000106C7 File Offset: 0x0000E8C7
		// (set) Token: 0x06000259 RID: 601 RVA: 0x000106CF File Offset: 0x0000E8CF
		public EncyclopediaManager EncyclopediaManager { get; private set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600025A RID: 602 RVA: 0x000106D8 File Offset: 0x0000E8D8
		// (set) Token: 0x0600025B RID: 603 RVA: 0x000106E0 File Offset: 0x0000E8E0
		public InventoryManager InventoryManager { get; private set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600025C RID: 604 RVA: 0x000106E9 File Offset: 0x0000E8E9
		// (set) Token: 0x0600025D RID: 605 RVA: 0x000106F1 File Offset: 0x0000E8F1
		public PartyScreenManager PartyScreenManager { get; private set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600025E RID: 606 RVA: 0x000106FA File Offset: 0x0000E8FA
		// (set) Token: 0x0600025F RID: 607 RVA: 0x00010702 File Offset: 0x0000E902
		public ConversationManager ConversationManager { get; private set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000260 RID: 608 RVA: 0x0001070B File Offset: 0x0000E90B
		public bool IsDay
		{
			get
			{
				return !this.IsNight;
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000261 RID: 609 RVA: 0x00010718 File Offset: 0x0000E918
		public bool IsNight
		{
			get
			{
				return CampaignTime.Now.IsNightTime;
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x00010732 File Offset: 0x0000E932
		internal int GeneratePartyId(PartyBase party)
		{
			int lastPartyIndex = this._lastPartyIndex;
			this._lastPartyIndex++;
			return lastPartyIndex;
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000263 RID: 611 RVA: 0x00010748 File Offset: 0x0000E948
		// (set) Token: 0x06000264 RID: 612 RVA: 0x00010750 File Offset: 0x0000E950
		[SaveableProperty(68)]
		public HeroTraitDeveloper PlayerTraitDeveloper { get; private set; }

		// Token: 0x06000265 RID: 613 RVA: 0x0001075C File Offset: 0x0000E95C
		private void LoadMapScene()
		{
			this._mapSceneWrapper = this.MapSceneCreator.CreateMapScene();
			this._mapSceneWrapper.SetSceneLevels(new List<string> { "level_1", "level_2", "level_3", "siege", "raid", "burned" });
			this._mapSceneWrapper.Load();
			Vec2 vec;
			Vec2 vec2;
			float num;
			this._mapSceneWrapper.GetMapBorders(out vec, out vec2, out num);
			Campaign.MapMinimumPosition = vec;
			Campaign.MapMaximumPosition = vec2;
			Campaign.MapMaximumHeight = num;
			Campaign.MapDiagonal = Campaign.MapMinimumPosition.Distance(Campaign.MapMaximumPosition);
			Campaign.MapDiagonalSquared = Campaign.MapDiagonal * Campaign.MapDiagonal;
			this.UpdateMaximumDistanceBetweenTwoSettlements();
		}

		// Token: 0x06000266 RID: 614 RVA: 0x00010827 File Offset: 0x0000EA27
		public void UpdateMaximumDistanceBetweenTwoSettlements()
		{
			Campaign.MaximumDistanceBetweenTwoSettlements = Campaign.Current.Models.MapDistanceModel.MaximumDistanceBetweenTwoSettlements;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00010844 File Offset: 0x0000EA44
		private void InitializeCachedLists()
		{
			MBObjectManager objectManager = Game.Current.ObjectManager;
			this._characters = objectManager.GetObjectTypeList<CharacterObject>();
			this._workshops = objectManager.GetObjectTypeList<WorkshopType>();
			this._itemModifiers = objectManager.GetObjectTypeList<ItemModifier>();
			this._itemModifierGroups = objectManager.GetObjectTypeList<ItemModifierGroup>();
			this._concepts = objectManager.GetObjectTypeList<Concept>();
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00010898 File Offset: 0x0000EA98
		public override void OnDestroy()
		{
			this.WaitAsyncTasks();
			GameTexts.ClearInstance();
			IMapScene mapSceneWrapper = this._mapSceneWrapper;
			if (mapSceneWrapper != null)
			{
				mapSceneWrapper.Destroy();
			}
			ConversationManager.Clear();
			CampaignData.OnGameEnd();
			MBTextManager.ClearAll();
			CampaignSiegeTestStatic.Destruct();
			GameSceneDataManager.Destroy();
			this.CampaignInformationManager.DeRegisterEvents();
			MBSaveLoad.OnGameDestroy();
			Campaign.Current = null;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x000108F0 File Offset: 0x0000EAF0
		public void InitializeSinglePlayerReferences()
		{
			this.IsInitializedSinglePlayerReferences = true;
			this.InitializeGamePlayReferences();
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00010900 File Offset: 0x0000EB00
		private void CreateLists()
		{
			this.AllPerks = MBObjectManager.Instance.GetObjectTypeList<PerkObject>();
			this.AllTraits = MBObjectManager.Instance.GetObjectTypeList<TraitObject>();
			this.AllEquipmentRosters = MBObjectManager.Instance.GetObjectTypeList<MBEquipmentRoster>();
			this.AllPolicies = MBObjectManager.Instance.GetObjectTypeList<PolicyObject>();
			this.AllBuildingTypes = MBObjectManager.Instance.GetObjectTypeList<BuildingType>();
			this.AllIssueEffects = MBObjectManager.Instance.GetObjectTypeList<IssueEffect>();
			this.AllSiegeStrategies = MBObjectManager.Instance.GetObjectTypeList<SiegeStrategy>();
			this.AllVillageTypes = MBObjectManager.Instance.GetObjectTypeList<VillageType>();
			this.AllSkillEffects = MBObjectManager.Instance.GetObjectTypeList<SkillEffect>();
			this.AllFeats = MBObjectManager.Instance.GetObjectTypeList<FeatObject>();
			this.AllSkills = MBObjectManager.Instance.GetObjectTypeList<SkillObject>();
			this.AllSiegeEngineTypes = MBObjectManager.Instance.GetObjectTypeList<SiegeEngineType>();
			this.AllItemCategories = MBObjectManager.Instance.GetObjectTypeList<ItemCategory>();
			this.AllCharacterAttributes = MBObjectManager.Instance.GetObjectTypeList<CharacterAttribute>();
			this.AllItems = MBObjectManager.Instance.GetObjectTypeList<ItemObject>();
		}

		// Token: 0x0600026B RID: 619 RVA: 0x000109FD File Offset: 0x0000EBFD
		private void CalculateCachedValues()
		{
			this.CalculateAverageDistanceBetweenTowns();
			this.CalculateAverageWage();
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00010A0C File Offset: 0x0000EC0C
		private void CalculateAverageWage()
		{
			float num = 0f;
			float num2 = 0f;
			foreach (CultureObject cultureObject in MBObjectManager.Instance.GetObjectTypeList<CultureObject>())
			{
				if (cultureObject.IsMainCulture)
				{
					foreach (PartyTemplateStack partyTemplateStack in cultureObject.DefaultPartyTemplate.Stacks)
					{
						int troopWage = partyTemplateStack.Character.TroopWage;
						float num3 = (float)(partyTemplateStack.MaxValue + partyTemplateStack.MinValue) * 0.5f;
						num += (float)troopWage * num3;
						num2 += num3;
					}
				}
			}
			if (num2 > 0f)
			{
				this.AverageWage = num / num2;
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00010AFC File Offset: 0x0000ECFC
		private void CalculateAverageDistanceBetweenTowns()
		{
			if (this.GameMode != CampaignGameMode.Tutorial)
			{
				float num = 0f;
				int num2 = 0;
				foreach (Town town in this.AllTowns)
				{
					float num3 = 5000f;
					foreach (Town town2 in this.AllTowns)
					{
						if (town != town2)
						{
							float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(town.Settlement, town2.Settlement);
							if (distance < num3)
							{
								num3 = distance;
							}
						}
					}
					num += num3;
					num2++;
				}
				Campaign.AverageDistanceBetweenTwoFortifications = num / (float)num2;
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00010BE8 File Offset: 0x0000EDE8
		public void InitializeGamePlayReferences()
		{
			base.CurrentGame.PlayerTroop = base.CurrentGame.ObjectManager.GetObject<CharacterObject>("main_hero");
			if (Hero.MainHero.Mother != null)
			{
				Hero.MainHero.Mother.SetHasMet();
			}
			if (Hero.MainHero.Father != null)
			{
				Hero.MainHero.Father.SetHasMet();
			}
			this.PlayerDefaultFaction = this.CampaignObjectManager.Find<Clan>("player_faction");
			GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, 1000, true);
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00010C74 File Offset: 0x0000EE74
		private void InitializeScenes()
		{
			GameSceneDataManager.Instance.LoadSPBattleScenes(ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/sp_battle_scenes.xml");
			GameSceneDataManager.Instance.LoadConversationScenes(ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/conversation_scenes.xml");
			GameSceneDataManager.Instance.LoadMeetingScenes(ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/meeting_scenes.xml");
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00010CDB File Offset: 0x0000EEDB
		public void SetLoadingParameters(Campaign.GameLoadingType gameLoadingType)
		{
			Campaign.Current = this;
			this._gameLoadingType = gameLoadingType;
			if (gameLoadingType == Campaign.GameLoadingType.SavedCampaign)
			{
				Campaign.Current.GameStarted = true;
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00010CF9 File Offset: 0x0000EEF9
		public void AddCampaignEventReceiver(CampaignEventReceiver receiver)
		{
			this.CampaignEventDispatcher.AddCampaignEventReceiver(receiver);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00010D08 File Offset: 0x0000EF08
		protected override void OnInitialize()
		{
			this.CampaignEvents = new CampaignEvents();
			this.CustomPeriodicCampaignEvents = new List<MBCampaignEvent>();
			this.CampaignEventDispatcher = new CampaignEventDispatcher(new CampaignEventReceiver[] { this.CampaignEvents, this.IssueManager, this.QuestManager });
			this.SandBoxManager = Game.Current.AddGameHandler<SandBoxManager>();
			this.SaveHandler = new SaveHandler();
			this.VisualCreator = new VisualCreator();
			this.GameMenuManager = new GameMenuManager();
			if (this._gameLoadingType != Campaign.GameLoadingType.Editor)
			{
				this.CreateManagers();
			}
			CampaignGameStarter campaignGameStarter = new CampaignGameStarter(this.GameMenuManager, this.ConversationManager, base.CurrentGame.GameTextManager);
			this.SandBoxManager.Initialize(campaignGameStarter);
			base.GameManager.InitializeGameStarter(base.CurrentGame, campaignGameStarter);
			GameSceneDataManager.Initialize();
			if (this._gameLoadingType == Campaign.GameLoadingType.NewCampaign || this._gameLoadingType == Campaign.GameLoadingType.SavedCampaign)
			{
				this.InitializeScenes();
			}
			base.GameManager.OnGameStart(base.CurrentGame, campaignGameStarter);
			base.CurrentGame.SetBasicModels(campaignGameStarter.Models);
			this._gameModels = base.CurrentGame.AddGameModelsManager<GameModels>(campaignGameStarter.Models);
			base.CurrentGame.CreateGameManager();
			if (this._gameLoadingType == Campaign.GameLoadingType.SavedCampaign)
			{
				this.InitializeDefaultCampaignObjects();
			}
			base.GameManager.BeginGameStart(base.CurrentGame);
			if (this._gameLoadingType != Campaign.GameLoadingType.SavedCampaign)
			{
				this.OnNewCampaignStart();
			}
			this.CreateLists();
			this.InitializeBasicObjectXmls();
			if (this._gameLoadingType != Campaign.GameLoadingType.SavedCampaign)
			{
				base.GameManager.OnNewCampaignStart(base.CurrentGame, campaignGameStarter);
			}
			this.SandBoxManager.OnCampaignStart(campaignGameStarter, base.GameManager, this._gameLoadingType == Campaign.GameLoadingType.SavedCampaign);
			if (this._gameLoadingType != Campaign.GameLoadingType.SavedCampaign)
			{
				this.AddCampaignBehaviorManager(new CampaignBehaviorManager(campaignGameStarter.CampaignBehaviors));
				base.GameManager.OnAfterCampaignStart(base.CurrentGame);
			}
			else
			{
				base.GameManager.OnGameLoaded(base.CurrentGame, campaignGameStarter);
				this._campaignBehaviorManager.InitializeCampaignBehaviors(campaignGameStarter.CampaignBehaviors);
				this._campaignBehaviorManager.LoadBehaviorData();
				this._campaignBehaviorManager.RegisterEvents();
			}
			ICraftingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			if (campaignBehavior != null)
			{
				campaignBehavior.InitializeCraftingElements();
			}
			campaignGameStarter.UnregisterNonReadyObjects();
			if (this._gameLoadingType == Campaign.GameLoadingType.SavedCampaign)
			{
				this.InitializeCampaignObjectsOnAfterLoad();
			}
			else if (this._gameLoadingType == Campaign.GameLoadingType.NewCampaign || this._gameLoadingType == Campaign.GameLoadingType.Tutorial)
			{
				this.CampaignObjectManager.InitializeOnNewGame();
			}
			this.InitializeCachedLists();
			this.NameGenerator.Initialize();
			base.CurrentGame.OnGameStart();
			base.GameManager.OnGameInitializationFinished(base.CurrentGame);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00010F80 File Offset: 0x0000F180
		private void CalculateCachedStatsOnLoad()
		{
			ItemRoster.CalculateCachedStatsOnLoad();
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00010F87 File Offset: 0x0000F187
		private void InitializeBasicObjectXmls()
		{
			base.ObjectManager.LoadXML("SPCultures", false);
			base.ObjectManager.LoadXML("Concepts", false);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00010FAC File Offset: 0x0000F1AC
		private void InitializeDefaultCampaignObjects()
		{
			base.CurrentGame.InitializeDefaultGameObjects();
			this.DefaultItems = new DefaultItems();
			base.CurrentGame.LoadBasicFiles();
			base.ObjectManager.LoadXML("Items", false);
			base.ObjectManager.LoadXML("EquipmentRosters", false);
			base.ObjectManager.LoadXML("partyTemplates", false);
			WeaponDescription @object = MBObjectManager.Instance.GetObject<WeaponDescription>("OneHandedBastardSwordAlternative");
			if (@object != null)
			{
				@object.IsHiddenFromUI = true;
			}
			WeaponDescription object2 = MBObjectManager.Instance.GetObject<WeaponDescription>("OneHandedBastardAxeAlternative");
			if (object2 != null)
			{
				object2.IsHiddenFromUI = true;
			}
			this.DefaultIssueEffects = new DefaultIssueEffects();
			this.DefaultTraits = new DefaultTraits();
			this.DefaultPolicies = new DefaultPolicies();
			this.DefaultPerks = new DefaultPerks();
			this.DefaultBuildingTypes = new DefaultBuildingTypes();
			this.DefaultVillageTypes = new DefaultVillageTypes();
			this.DefaultSiegeStrategies = new DefaultSiegeStrategies();
			this.DefaultSkillEffects = new DefaultSkillEffects();
			this.DefaultFeats = new DefaultCulturalFeats();
			this.PlayerUpdateTracker = new PlayerUpdateTracker();
		}

		// Token: 0x06000276 RID: 630 RVA: 0x000110AF File Offset: 0x0000F2AF
		private void InitializeManagers()
		{
			this.KingdomManager = new KingdomManager();
			this.CampaignInformationManager = new CampaignInformationManager();
			this.VisualTrackerManager = new VisualTrackerManager();
			this.TournamentManager = new TournamentManager();
		}

		// Token: 0x06000277 RID: 631 RVA: 0x000110E0 File Offset: 0x0000F2E0
		private void InitializeCampaignObjectsOnAfterLoad()
		{
			this.CampaignObjectManager.InitializeOnLoad();
			this.FactionManager.AfterLoad();
			List<PerkObject> list = this.AllPerks.Where((PerkObject x) => !x.IsTrash).ToList<PerkObject>();
			this.AllPerks = new MBReadOnlyList<PerkObject>(list);
			this.LogEntryHistory.OnAfterLoad();
			foreach (Kingdom kingdom in this.Kingdoms)
			{
				foreach (Army army in kingdom.Armies)
				{
					army.OnAfterLoad();
				}
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x000111C8 File Offset: 0x0000F3C8
		private void OnNewCampaignStart()
		{
			Game.Current.PlayerTroop = null;
			this.MapStateData = new MapStateData();
			this.InitializeDefaultCampaignObjects();
			this.MainParty = MBObjectManager.Instance.CreateObject<MobileParty>("player_party");
			this.MainParty.Ai.SetAsMainParty();
			this.InitializeManagers();
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0001121C File Offset: 0x0000F41C
		protected override void BeforeRegisterTypes(MBObjectManager objectManager)
		{
			objectManager.RegisterType<FeatObject>("Feat", "Feats", 0U, true, false);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00011234 File Offset: 0x0000F434
		protected override void OnRegisterTypes(MBObjectManager objectManager)
		{
			objectManager.RegisterType<MobileParty>("MobileParty", "MobileParties", 14U, true, true);
			objectManager.RegisterType<CharacterObject>("NPCCharacter", "NPCCharacters", 16U, true, false);
			if (this.GameMode == CampaignGameMode.Tutorial)
			{
				objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "MPCharacters", 43U, true, false);
			}
			objectManager.RegisterType<CultureObject>("Culture", "SPCultures", 17U, true, false);
			objectManager.RegisterType<Clan>("Faction", "Factions", 18U, true, true);
			objectManager.RegisterType<PerkObject>("Perk", "Perks", 19U, true, false);
			objectManager.RegisterType<Kingdom>("Kingdom", "Kingdoms", 20U, true, true);
			objectManager.RegisterType<TraitObject>("Trait", "Traits", 21U, true, false);
			objectManager.RegisterType<VillageType>("VillageType", "VillageTypes", 22U, true, false);
			objectManager.RegisterType<BuildingType>("BuildingType", "BuildingTypes", 23U, true, false);
			objectManager.RegisterType<PartyTemplateObject>("PartyTemplate", "partyTemplates", 24U, true, false);
			objectManager.RegisterType<Settlement>("Settlement", "Settlements", 25U, true, false);
			objectManager.RegisterType<WorkshopType>("WorkshopType", "WorkshopTypes", 26U, true, false);
			objectManager.RegisterType<Village>("Village", "Components", 27U, true, false);
			objectManager.RegisterType<Hideout>("Hideout", "Components", 30U, true, false);
			objectManager.RegisterType<Town>("Town", "Components", 31U, true, false);
			objectManager.RegisterType<Hero>("Hero", "Heroes", 32U, true, true);
			objectManager.RegisterType<MenuContext>("MenuContext", "MenuContexts", 35U, true, false);
			objectManager.RegisterType<PolicyObject>("Policy", "Policies", 36U, true, false);
			objectManager.RegisterType<Concept>("Concept", "Concepts", 37U, true, false);
			objectManager.RegisterType<IssueEffect>("IssueEffect", "IssueEffects", 39U, true, false);
			objectManager.RegisterType<SiegeStrategy>("SiegeStrategy", "SiegeStrategies", 40U, true, false);
			objectManager.RegisterType<SkillEffect>("SkillEffect", "SkillEffects", 53U, true, false);
			objectManager.RegisterType<LocationComplexTemplate>("LocationComplexTemplate", "LocationComplexTemplates", 42U, true, false);
			objectManager.RegisterType<RetirementSettlementComponent>("RetirementSettlementComponent", "Components", 56U, true, false);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00011440 File Offset: 0x0000F640
		private void CreateManagers()
		{
			this.EncyclopediaManager = new EncyclopediaManager();
			this.InventoryManager = new InventoryManager();
			this.PartyScreenManager = new PartyScreenManager();
			this.ConversationManager = new ConversationManager();
			this.NameGenerator = new NameGenerator();
			this.SkillLevelingManager = new DefaultSkillLevelingManager();
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0001148F File Offset: 0x0000F68F
		private void OnNewGameCreated(CampaignGameStarter gameStarter)
		{
			this.OnNewGameCreatedInternal();
			GameManagerBase gameManager = base.GameManager;
			if (gameManager != null)
			{
				gameManager.OnNewGameCreated(base.CurrentGame, gameStarter);
			}
			CampaignEventDispatcher.Instance.OnNewGameCreated(gameStarter);
			this.OnAfterNewGameCreatedInternal();
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000114C0 File Offset: 0x0000F6C0
		private void OnNewGameCreatedInternal()
		{
			this.UniqueGameId = MiscHelper.GenerateCampaignId(12);
			this._newGameVersion = ApplicationVersion.FromParametersFile(null).ToString();
			this.PlatformID = ApplicationPlatform.CurrentPlatform.ToString();
			this.PlayerTraitDeveloper = new HeroTraitDeveloper(Hero.MainHero);
			this.TimeControlMode = CampaignTimeControlMode.Stop;
			this._campaignEntitySystem = new EntitySystem<CampaignEntityComponent>();
			this.SiegeEventManager = new SiegeEventManager();
			this.MapEventManager = new MapEventManager();
			this.autoEnterTown = null;
			this.MinSettlementX = 1000f;
			this.MinSettlementY = 1000f;
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.Position2D.x < this.MinSettlementX)
				{
					this.MinSettlementX = settlement.Position2D.x;
				}
				if (settlement.Position2D.y < this.MinSettlementY)
				{
					this.MinSettlementY = settlement.Position2D.y;
				}
				if (settlement.Position2D.x > this.MaxSettlementX)
				{
					this.MaxSettlementX = settlement.Position2D.x;
				}
				if (settlement.Position2D.y > this.MaxSettlementY)
				{
					this.MaxSettlementY = settlement.Position2D.y;
				}
			}
			this.CampaignBehaviorManager.RegisterEvents();
			this.CameraFollowParty = this.MainParty.Party;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00011654 File Offset: 0x0000F854
		private void OnAfterNewGameCreatedInternal()
		{
			Hero.MainHero.Gold = 1000;
			ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -Clan.PlayerClan.Influence);
			Hero.MainHero.ChangeState(Hero.CharacterStates.Active);
			this.GameInitTick();
			this._playerFormationPreferences = new Dictionary<CharacterObject, FormationClass>();
			this.PlayerFormationPreferences = this._playerFormationPreferences.GetReadOnlyDictionary<CharacterObject, FormationClass>();
			Campaign.Current.DesertionEnabled = true;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x000116C0 File Offset: 0x0000F8C0
		protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
		{
			nextState = GameTypeLoadingStates.None;
			switch (gameTypeLoadingState)
			{
			case GameTypeLoadingStates.InitializeFirstStep:
				base.CurrentGame.Initialize();
				nextState = GameTypeLoadingStates.WaitSecondStep;
				return;
			case GameTypeLoadingStates.WaitSecondStep:
				nextState = GameTypeLoadingStates.LoadVisualsThirdState;
				return;
			case GameTypeLoadingStates.LoadVisualsThirdState:
				if (this.GameMode == CampaignGameMode.Campaign)
				{
					this.LoadMapScene();
				}
				nextState = GameTypeLoadingStates.PostInitializeFourthState;
				return;
			case GameTypeLoadingStates.PostInitializeFourthState:
			{
				CampaignGameStarter gameStarter = this.SandBoxManager.GameStarter;
				if (this._gameLoadingType == Campaign.GameLoadingType.SavedCampaign)
				{
					this.OnDataLoadFinished(gameStarter);
					this.CalculateCachedValues();
					this.DeterminedSavedStats(this._gameLoadingType);
					foreach (Settlement settlement in Settlement.All)
					{
						settlement.Party.OnGameInitialized();
					}
					foreach (MobileParty mobileParty in this.MobileParties.ToList<MobileParty>())
					{
						mobileParty.Party.OnGameInitialized();
					}
					this.CalculateCachedStatsOnLoad();
					this.OnGameLoaded(gameStarter);
					this.OnSessionStart(gameStarter);
					foreach (Hero hero in Hero.AllAliveHeroes)
					{
						hero.CheckInvalidEquipmentsAndReplaceIfNeeded();
					}
					using (List<Hero>.Enumerator enumerator3 = Hero.DeadOrDisabledHeroes.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							Hero hero2 = enumerator3.Current;
							hero2.CheckInvalidEquipmentsAndReplaceIfNeeded();
						}
						goto IL_297;
					}
				}
				if (this._gameLoadingType == Campaign.GameLoadingType.NewCampaign)
				{
					this.OnDataLoadFinished(gameStarter);
					this.CalculateCachedValues();
					MBSaveLoad.OnNewGame();
					this.InitializeMainParty();
					this.DeterminedSavedStats(this._gameLoadingType);
					foreach (Settlement settlement2 in Settlement.All)
					{
						settlement2.Party.OnGameInitialized();
					}
					foreach (MobileParty mobileParty2 in this.MobileParties.ToList<MobileParty>())
					{
						mobileParty2.Party.OnGameInitialized();
					}
					foreach (Settlement settlement3 in Settlement.All)
					{
						settlement3.OnGameCreated();
					}
					foreach (Clan clan in Clan.All)
					{
						clan.OnGameCreated();
					}
					MBObjectManager.Instance.RemoveTemporaryTypes();
					this.OnNewGameCreated(gameStarter);
					this.OnSessionStart(gameStarter);
					Debug.Print("Finished starting a new game.", 0, Debug.DebugColor.White, 17592186044416UL);
				}
				IL_297:
				base.GameManager.OnAfterGameInitializationFinished(base.CurrentGame, gameStarter);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x000119DC File Offset: 0x0000FBDC
		private void DeterminedSavedStats(Campaign.GameLoadingType gameLoadingType)
		{
			if (this._previouslyUsedModules == null)
			{
				this._previouslyUsedModules = new MBList<string>();
			}
			foreach (string text in SandBoxManager.Instance.ModuleManager.ModuleNames)
			{
				if (!this._previouslyUsedModules.Contains(text))
				{
					this._previouslyUsedModules.Add(text);
				}
			}
			if (this._usedGameVersions == null)
			{
				this._usedGameVersions = new MBList<string>();
			}
			string text2 = MBSaveLoad.LastLoadedGameVersion.ToString();
			if (this._usedGameVersions.Count <= 0 || !this._usedGameVersions[this._usedGameVersions.Count - 1].Equals(text2))
			{
				this._usedGameVersions.Add(text2);
			}
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00011A9A File Offset: 0x0000FC9A
		public override void OnMissionIsStarting(string missionName, MissionInitializerRecord rec)
		{
			if (rec.PlayingInCampaignMode)
			{
				CampaignEventDispatcher.Instance.BeforeMissionOpened();
			}
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00011AAE File Offset: 0x0000FCAE
		public override void InitializeParameters()
		{
			ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_campaign_parameters"));
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00011AC9 File Offset: 0x0000FCC9
		public void SetTimeControlModeLock(bool isLocked)
		{
			this.TimeControlModeLock = isLocked;
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000284 RID: 644 RVA: 0x00011AD2 File Offset: 0x0000FCD2
		public override bool IsPartyWindowAccessibleAtMission
		{
			get
			{
				return this.GameMode == CampaignGameMode.Tutorial;
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000285 RID: 645 RVA: 0x00011ADD File Offset: 0x0000FCDD
		internal MBReadOnlyList<Town> AllTowns
		{
			get
			{
				return this._towns;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000286 RID: 646 RVA: 0x00011AE5 File Offset: 0x0000FCE5
		internal MBReadOnlyList<Town> AllCastles
		{
			get
			{
				return this._castles;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000287 RID: 647 RVA: 0x00011AED File Offset: 0x0000FCED
		internal MBReadOnlyList<Village> AllVillages
		{
			get
			{
				return this._villages;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000288 RID: 648 RVA: 0x00011AF5 File Offset: 0x0000FCF5
		internal MBReadOnlyList<Hideout> AllHideouts
		{
			get
			{
				return this._hideouts;
			}
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00011B00 File Offset: 0x0000FD00
		public void OnPlayerCharacterChanged(out bool isMainPartyChanged)
		{
			isMainPartyChanged = false;
			if (MobileParty.MainParty != Hero.MainHero.PartyBelongedTo)
			{
				isMainPartyChanged = true;
			}
			this.MainParty = Hero.MainHero.PartyBelongedTo;
			if (Hero.MainHero.CurrentSettlement != null && !Hero.MainHero.IsPrisoner)
			{
				if (this.MainParty == null)
				{
					LeaveSettlementAction.ApplyForCharacterOnly(Hero.MainHero);
				}
				else
				{
					LeaveSettlementAction.ApplyForParty(this.MainParty);
				}
			}
			if (Hero.MainHero.IsFugitive)
			{
				Hero.MainHero.ChangeState(Hero.CharacterStates.Active);
			}
			this.PlayerTraitDeveloper = new HeroTraitDeveloper(Hero.MainHero);
			if (this.MainParty == null)
			{
				this.MainParty = MobileParty.CreateParty("player_party_" + Hero.MainHero.StringId, new LordPartyComponent(Hero.MainHero, Hero.MainHero), delegate(MobileParty mobileParty)
				{
					mobileParty.ActualClan = Clan.PlayerClan;
				});
				isMainPartyChanged = true;
				if (Hero.MainHero.IsPrisoner)
				{
					this.MainParty.InitializeMobilePartyAtPosition(base.CurrentGame.ObjectManager.GetObject<PartyTemplateObject>("main_hero_party_template"), Hero.MainHero.GetPosition().AsVec2, 0);
					this.MainParty.IsActive = false;
				}
				else
				{
					Vec2 vec;
					if (!(Hero.MainHero.GetPosition().AsVec2 != Vec2.Zero))
					{
						vec = SettlementHelper.FindRandomSettlement((Settlement s) => s.OwnerClan != null && !s.OwnerClan.IsAtWarWith(Clan.PlayerClan)).GetPosition2D;
					}
					else
					{
						vec = Hero.MainHero.GetPosition().AsVec2;
					}
					Vec2 vec2 = vec;
					this.MainParty.InitializeMobilePartyAtPosition(base.CurrentGame.ObjectManager.GetObject<PartyTemplateObject>("main_hero_party_template"), vec2, 0);
					this.MainParty.IsActive = true;
					this.MainParty.MemberRoster.AddToCounts(Hero.MainHero.CharacterObject, 1, true, 0, 0, true, -1);
				}
			}
			Campaign.Current.MainParty.Ai.SetAsMainParty();
			PartyBase.MainParty.ItemRoster.UpdateVersion();
			PartyBase.MainParty.MemberRoster.UpdateVersion();
			if (MobileParty.MainParty.IsActive)
			{
				PartyBase.MainParty.SetAsCameraFollowParty();
			}
			PartyBase.MainParty.UpdateVisibilityAndInspected(0f, true);
			if (Hero.MainHero.Mother != null)
			{
				Hero.MainHero.Mother.SetHasMet();
			}
			if (Hero.MainHero.Father != null)
			{
				Hero.MainHero.Father.SetHasMet();
			}
			this.MainParty.SetWagePaymentLimit(Campaign.Current.Models.PartyWageModel.MaxWage);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00011D95 File Offset: 0x0000FF95
		public void SetPlayerFormationPreference(CharacterObject character, FormationClass formation)
		{
			if (!this._playerFormationPreferences.ContainsKey(character))
			{
				this._playerFormationPreferences.Add(character, formation);
				return;
			}
			this._playerFormationPreferences[character] = formation;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00011DC0 File Offset: 0x0000FFC0
		public override void OnStateChanged(GameState oldState)
		{
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00011DC2 File Offset: 0x0000FFC2
		internal static void AutoGeneratedStaticCollectObjectsCampaign(object o, List<object> collectedObjects)
		{
			((Campaign)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00011DD0 File Offset: 0x0000FFD0
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Options);
			collectedObjects.Add(this.TournamentManager);
			collectedObjects.Add(this.autoEnterTown);
			collectedObjects.Add(this.KingdomManager);
			collectedObjects.Add(this._campaignPeriodicEventManager);
			collectedObjects.Add(this._previouslyUsedModules);
			collectedObjects.Add(this._usedGameVersions);
			collectedObjects.Add(this._playerFormationPreferences);
			collectedObjects.Add(this._campaignBehaviorManager);
			collectedObjects.Add(this._cameraFollowParty);
			collectedObjects.Add(this._logEntryHistory);
			collectedObjects.Add(this.CampaignObjectManager);
			collectedObjects.Add(this.QuestManager);
			collectedObjects.Add(this.IssueManager);
			collectedObjects.Add(this.FactionManager);
			collectedObjects.Add(this.CharacterRelationManager);
			collectedObjects.Add(this.Romance);
			collectedObjects.Add(this.PlayerCaptivity);
			collectedObjects.Add(this.PlayerDefaultFaction);
			collectedObjects.Add(this.MapStateData);
			collectedObjects.Add(this.MapTimeTracker);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.CampaignStartTime, collectedObjects);
			collectedObjects.Add(this.SiegeEventManager);
			collectedObjects.Add(this.MapEventManager);
			collectedObjects.Add(this.PlayerEncounter);
			collectedObjects.Add(this.BarterManager);
			collectedObjects.Add(this.MainParty);
			collectedObjects.Add(this.CampaignInformationManager);
			collectedObjects.Add(this.VisualTrackerManager);
			collectedObjects.Add(this.PlayerTraitDeveloper);
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00011F51 File Offset: 0x00010151
		internal static object AutoGeneratedGetMemberValuePlatformID(object o)
		{
			return ((Campaign)o).PlatformID;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00011F5E File Offset: 0x0001015E
		internal static object AutoGeneratedGetMemberValueUniqueGameId(object o)
		{
			return ((Campaign)o).UniqueGameId;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00011F6B File Offset: 0x0001016B
		internal static object AutoGeneratedGetMemberValueCampaignObjectManager(object o)
		{
			return ((Campaign)o).CampaignObjectManager;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00011F78 File Offset: 0x00010178
		internal static object AutoGeneratedGetMemberValueIsCraftingEnabled(object o)
		{
			return ((Campaign)o).IsCraftingEnabled;
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00011F8A File Offset: 0x0001018A
		internal static object AutoGeneratedGetMemberValueIsBannerEditorEnabled(object o)
		{
			return ((Campaign)o).IsBannerEditorEnabled;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00011F9C File Offset: 0x0001019C
		internal static object AutoGeneratedGetMemberValueIsFaceGenEnabled(object o)
		{
			return ((Campaign)o).IsFaceGenEnabled;
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00011FAE File Offset: 0x000101AE
		internal static object AutoGeneratedGetMemberValueQuestManager(object o)
		{
			return ((Campaign)o).QuestManager;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00011FBB File Offset: 0x000101BB
		internal static object AutoGeneratedGetMemberValueIssueManager(object o)
		{
			return ((Campaign)o).IssueManager;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00011FC8 File Offset: 0x000101C8
		internal static object AutoGeneratedGetMemberValueFactionManager(object o)
		{
			return ((Campaign)o).FactionManager;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00011FD5 File Offset: 0x000101D5
		internal static object AutoGeneratedGetMemberValueCharacterRelationManager(object o)
		{
			return ((Campaign)o).CharacterRelationManager;
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00011FE2 File Offset: 0x000101E2
		internal static object AutoGeneratedGetMemberValueRomance(object o)
		{
			return ((Campaign)o).Romance;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00011FEF File Offset: 0x000101EF
		internal static object AutoGeneratedGetMemberValuePlayerCaptivity(object o)
		{
			return ((Campaign)o).PlayerCaptivity;
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00011FFC File Offset: 0x000101FC
		internal static object AutoGeneratedGetMemberValuePlayerDefaultFaction(object o)
		{
			return ((Campaign)o).PlayerDefaultFaction;
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00012009 File Offset: 0x00010209
		internal static object AutoGeneratedGetMemberValueMapStateData(object o)
		{
			return ((Campaign)o).MapStateData;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00012016 File Offset: 0x00010216
		internal static object AutoGeneratedGetMemberValueMapTimeTracker(object o)
		{
			return ((Campaign)o).MapTimeTracker;
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00012023 File Offset: 0x00010223
		internal static object AutoGeneratedGetMemberValueCampaignStartTime(object o)
		{
			return ((Campaign)o).CampaignStartTime;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00012035 File Offset: 0x00010235
		internal static object AutoGeneratedGetMemberValueGameMode(object o)
		{
			return ((Campaign)o).GameMode;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00012047 File Offset: 0x00010247
		internal static object AutoGeneratedGetMemberValuePlayerProgress(object o)
		{
			return ((Campaign)o).PlayerProgress;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00012059 File Offset: 0x00010259
		internal static object AutoGeneratedGetMemberValueSiegeEventManager(object o)
		{
			return ((Campaign)o).SiegeEventManager;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00012066 File Offset: 0x00010266
		internal static object AutoGeneratedGetMemberValueMapEventManager(object o)
		{
			return ((Campaign)o).MapEventManager;
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00012073 File Offset: 0x00010273
		internal static object AutoGeneratedGetMemberValue_curMapFrame(object o)
		{
			return ((Campaign)o)._curMapFrame;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00012085 File Offset: 0x00010285
		internal static object AutoGeneratedGetMemberValuePlayerEncounter(object o)
		{
			return ((Campaign)o).PlayerEncounter;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00012092 File Offset: 0x00010292
		internal static object AutoGeneratedGetMemberValueBarterManager(object o)
		{
			return ((Campaign)o).BarterManager;
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0001209F File Offset: 0x0001029F
		internal static object AutoGeneratedGetMemberValueIsMainHeroDisguised(object o)
		{
			return ((Campaign)o).IsMainHeroDisguised;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x000120B1 File Offset: 0x000102B1
		internal static object AutoGeneratedGetMemberValueDesertionEnabled(object o)
		{
			return ((Campaign)o).DesertionEnabled;
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x000120C3 File Offset: 0x000102C3
		internal static object AutoGeneratedGetMemberValueMainParty(object o)
		{
			return ((Campaign)o).MainParty;
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x000120D0 File Offset: 0x000102D0
		internal static object AutoGeneratedGetMemberValueCampaignInformationManager(object o)
		{
			return ((Campaign)o).CampaignInformationManager;
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x000120DD File Offset: 0x000102DD
		internal static object AutoGeneratedGetMemberValueVisualTrackerManager(object o)
		{
			return ((Campaign)o).VisualTrackerManager;
		}

		// Token: 0x060002AA RID: 682 RVA: 0x000120EA File Offset: 0x000102EA
		internal static object AutoGeneratedGetMemberValuePlayerTraitDeveloper(object o)
		{
			return ((Campaign)o).PlayerTraitDeveloper;
		}

		// Token: 0x060002AB RID: 683 RVA: 0x000120F7 File Offset: 0x000102F7
		internal static object AutoGeneratedGetMemberValueOptions(object o)
		{
			return ((Campaign)o).Options;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00012104 File Offset: 0x00010304
		internal static object AutoGeneratedGetMemberValueTournamentManager(object o)
		{
			return ((Campaign)o).TournamentManager;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00012111 File Offset: 0x00010311
		internal static object AutoGeneratedGetMemberValueIsInitializedSinglePlayerReferences(object o)
		{
			return ((Campaign)o).IsInitializedSinglePlayerReferences;
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00012123 File Offset: 0x00010323
		internal static object AutoGeneratedGetMemberValueLastTimeControlMode(object o)
		{
			return ((Campaign)o).LastTimeControlMode;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00012135 File Offset: 0x00010335
		internal static object AutoGeneratedGetMemberValueautoEnterTown(object o)
		{
			return ((Campaign)o).autoEnterTown;
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00012142 File Offset: 0x00010342
		internal static object AutoGeneratedGetMemberValueMainHeroIllDays(object o)
		{
			return ((Campaign)o).MainHeroIllDays;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00012154 File Offset: 0x00010354
		internal static object AutoGeneratedGetMemberValueKingdomManager(object o)
		{
			return ((Campaign)o).KingdomManager;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00012161 File Offset: 0x00010361
		internal static object AutoGeneratedGetMemberValue_campaignPeriodicEventManager(object o)
		{
			return ((Campaign)o)._campaignPeriodicEventManager;
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0001216E File Offset: 0x0001036E
		internal static object AutoGeneratedGetMemberValue_isMainPartyWaiting(object o)
		{
			return ((Campaign)o)._isMainPartyWaiting;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00012180 File Offset: 0x00010380
		internal static object AutoGeneratedGetMemberValue_newGameVersion(object o)
		{
			return ((Campaign)o)._newGameVersion;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0001218D File Offset: 0x0001038D
		internal static object AutoGeneratedGetMemberValue_previouslyUsedModules(object o)
		{
			return ((Campaign)o)._previouslyUsedModules;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0001219A File Offset: 0x0001039A
		internal static object AutoGeneratedGetMemberValue_usedGameVersions(object o)
		{
			return ((Campaign)o)._usedGameVersions;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x000121A7 File Offset: 0x000103A7
		internal static object AutoGeneratedGetMemberValue_playerFormationPreferences(object o)
		{
			return ((Campaign)o)._playerFormationPreferences;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x000121B4 File Offset: 0x000103B4
		internal static object AutoGeneratedGetMemberValue_campaignBehaviorManager(object o)
		{
			return ((Campaign)o)._campaignBehaviorManager;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x000121C1 File Offset: 0x000103C1
		internal static object AutoGeneratedGetMemberValue_lastPartyIndex(object o)
		{
			return ((Campaign)o)._lastPartyIndex;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x000121D3 File Offset: 0x000103D3
		internal static object AutoGeneratedGetMemberValue_cameraFollowParty(object o)
		{
			return ((Campaign)o)._cameraFollowParty;
		}

		// Token: 0x060002BB RID: 699 RVA: 0x000121E0 File Offset: 0x000103E0
		internal static object AutoGeneratedGetMemberValue_logEntryHistory(object o)
		{
			return ((Campaign)o)._logEntryHistory;
		}

		// Token: 0x0400005A RID: 90
		public const float ConfigTimeMultiplier = 0.25f;

		// Token: 0x0400005B RID: 91
		private EntitySystem<CampaignEntityComponent> _campaignEntitySystem;

		// Token: 0x04000062 RID: 98
		public ITask CampaignLateAITickTask;

		// Token: 0x04000063 RID: 99
		[SaveableField(210)]
		private CampaignPeriodicEventManager _campaignPeriodicEventManager;

		// Token: 0x04000066 RID: 102
		[SaveableField(53)]
		private bool _isMainPartyWaiting;

		// Token: 0x04000067 RID: 103
		[SaveableField(344)]
		private string _newGameVersion;

		// Token: 0x04000068 RID: 104
		[SaveableField(78)]
		private MBList<string> _previouslyUsedModules;

		// Token: 0x04000069 RID: 105
		[SaveableField(81)]
		private MBList<string> _usedGameVersions;

		// Token: 0x0400006E RID: 110
		[SaveableField(77)]
		private Dictionary<CharacterObject, FormationClass> _playerFormationPreferences;

		// Token: 0x0400006F RID: 111
		[SaveableField(7)]
		private ICampaignBehaviorManager _campaignBehaviorManager;

		// Token: 0x04000071 RID: 113
		private CampaignTickCacheDataStore _tickData;

		// Token: 0x04000072 RID: 114
		[SaveableField(2)]
		public readonly CampaignOptions Options;

		// Token: 0x04000073 RID: 115
		public MBReadOnlyDictionary<CharacterObject, FormationClass> PlayerFormationPreferences;

		// Token: 0x04000074 RID: 116
		[SaveableField(13)]
		public ITournamentManager TournamentManager;

		// Token: 0x04000075 RID: 117
		public float MinSettlementX;

		// Token: 0x04000076 RID: 118
		public float MaxSettlementX;

		// Token: 0x04000077 RID: 119
		public float MinSettlementY;

		// Token: 0x04000078 RID: 120
		public float MaxSettlementY;

		// Token: 0x04000079 RID: 121
		[SaveableField(27)]
		public bool IsInitializedSinglePlayerReferences;

		// Token: 0x0400007A RID: 122
		private LocatorGrid<MobileParty> _mobilePartyLocator;

		// Token: 0x0400007B RID: 123
		private LocatorGrid<Settlement> _settlementLocator;

		// Token: 0x0400007C RID: 124
		private GameModels _gameModels;

		// Token: 0x0400007F RID: 127
		[SaveableField(31)]
		public CampaignTimeControlMode LastTimeControlMode = CampaignTimeControlMode.UnstoppablePlay;

		// Token: 0x04000080 RID: 128
		private IMapScene _mapSceneWrapper;

		// Token: 0x04000081 RID: 129
		public bool GameStarted;

		// Token: 0x04000083 RID: 131
		[SaveableField(44)]
		public PartyBase autoEnterTown;

		// Token: 0x04000084 RID: 132
		private Campaign.GameLoadingType _gameLoadingType;

		// Token: 0x04000085 RID: 133
		public ConversationContext CurrentConversationContext;

		// Token: 0x04000086 RID: 134
		[CachedData]
		private float _dt;

		// Token: 0x04000089 RID: 137
		private CampaignTimeControlMode _timeControlMode;

		// Token: 0x0400008A RID: 138
		private int _curSessionFrame;

		// Token: 0x040000B0 RID: 176
		[SaveableField(30)]
		public int MainHeroIllDays = -1;

		// Token: 0x040000C0 RID: 192
		private MBCampaignEvent _dailyTickEvent;

		// Token: 0x040000C1 RID: 193
		private MBCampaignEvent _hourlyTickEvent;

		// Token: 0x040000C3 RID: 195
		[CachedData]
		private int _lastNonZeroDtFrame;

		// Token: 0x040000CA RID: 202
		private MBList<Town> _towns;

		// Token: 0x040000CB RID: 203
		private MBList<Town> _castles;

		// Token: 0x040000CC RID: 204
		private MBList<Village> _villages;

		// Token: 0x040000CD RID: 205
		private MBList<Hideout> _hideouts;

		// Token: 0x040000CE RID: 206
		private MBReadOnlyList<CharacterObject> _characters;

		// Token: 0x040000CF RID: 207
		private MBReadOnlyList<WorkshopType> _workshops;

		// Token: 0x040000D0 RID: 208
		private MBReadOnlyList<ItemModifier> _itemModifiers;

		// Token: 0x040000D1 RID: 209
		private MBReadOnlyList<Concept> _concepts;

		// Token: 0x040000D2 RID: 210
		private MBReadOnlyList<ItemModifierGroup> _itemModifierGroups;

		// Token: 0x040000D3 RID: 211
		[SaveableField(79)]
		private int _lastPartyIndex;

		// Token: 0x040000D5 RID: 213
		[SaveableField(61)]
		private PartyBase _cameraFollowParty;

		// Token: 0x040000D8 RID: 216
		[SaveableField(64)]
		private readonly LogEntryHistory _logEntryHistory = new LogEntryHistory();

		// Token: 0x040000DD RID: 221
		[SaveableField(65)]
		public KingdomManager KingdomManager;

		// Token: 0x0200047B RID: 1147
		[Flags]
		public enum PartyRestFlags : uint
		{
			// Token: 0x04001379 RID: 4985
			None = 0U,
			// Token: 0x0400137A RID: 4986
			SafeMode = 1U
		}

		// Token: 0x0200047C RID: 1148
		public enum GameLoadingType
		{
			// Token: 0x0400137C RID: 4988
			Tutorial,
			// Token: 0x0400137D RID: 4989
			NewCampaign,
			// Token: 0x0400137E RID: 4990
			SavedCampaign,
			// Token: 0x0400137F RID: 4991
			Editor
		}
	}
}
