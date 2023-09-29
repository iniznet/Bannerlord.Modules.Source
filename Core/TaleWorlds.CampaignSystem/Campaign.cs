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
	public class Campaign : GameType
	{
		public static float MapDiagonal { get; private set; }

		public static float MapDiagonalSquared { get; private set; }

		public static float MaximumDistanceBetweenTwoSettlements { get; private set; }

		public static Vec2 MapMinimumPosition { get; private set; }

		public static Vec2 MapMaximumPosition { get; private set; }

		public static float MapMaximumHeight { get; private set; }

		public static float AverageDistanceBetweenTwoFortifications { get; private set; }

		[CachedData]
		public float AverageWage { get; private set; }

		public string NewGameVersion
		{
			get
			{
				return this._newGameVersion;
			}
		}

		public MBReadOnlyList<string> PreviouslyUsedModules
		{
			get
			{
				return this._previouslyUsedModules;
			}
		}

		public MBReadOnlyList<string> UsedGameVersions
		{
			get
			{
				return this._usedGameVersions;
			}
		}

		[SaveableProperty(83)]
		public bool EnabledCheatsBefore { get; set; }

		[SaveableProperty(82)]
		public string PlatformID { get; private set; }

		internal CampaignEventDispatcher CampaignEventDispatcher { get; private set; }

		[SaveableProperty(80)]
		public string UniqueGameId { get; private set; }

		public SaveHandler SaveHandler { get; private set; }

		public override bool SupportsSaving
		{
			get
			{
				return this.GameMode == CampaignGameMode.Campaign;
			}
		}

		[SaveableProperty(211)]
		public CampaignObjectManager CampaignObjectManager { get; private set; }

		public override bool IsDevelopment
		{
			get
			{
				return this.GameMode == CampaignGameMode.Tutorial;
			}
		}

		[SaveableProperty(3)]
		public bool IsCraftingEnabled { get; set; } = true;

		[SaveableProperty(4)]
		public bool IsBannerEditorEnabled { get; set; } = true;

		[SaveableProperty(5)]
		public bool IsFaceGenEnabled { get; set; } = true;

		public ICampaignBehaviorManager CampaignBehaviorManager
		{
			get
			{
				return this._campaignBehaviorManager;
			}
		}

		[SaveableProperty(8)]
		public QuestManager QuestManager { get; private set; }

		[SaveableProperty(9)]
		public IssueManager IssueManager { get; private set; }

		[SaveableProperty(11)]
		public FactionManager FactionManager { get; private set; }

		[SaveableProperty(12)]
		public CharacterRelationManager CharacterRelationManager { get; private set; }

		[SaveableProperty(14)]
		public Romance Romance { get; private set; }

		[SaveableProperty(16)]
		public PlayerCaptivity PlayerCaptivity { get; private set; }

		[SaveableProperty(17)]
		internal Clan PlayerDefaultFaction { get; set; }

		public CampaignMission.ICampaignMissionManager CampaignMissionManager { get; set; }

		public ISkillLevelingManager SkillLevelingManager { get; set; }

		public IMapSceneCreator MapSceneCreator { get; set; }

		public override bool IsInventoryAccessibleAtMission
		{
			get
			{
				return this.GameMode == CampaignGameMode.Tutorial;
			}
		}

		public GameMenuCallbackManager GameMenuCallbackManager { get; private set; }

		public VisualCreator VisualCreator { get; set; }

		[SaveableProperty(28)]
		public MapStateData MapStateData { get; private set; }

		public DefaultPerks DefaultPerks { get; private set; }

		public DefaultTraits DefaultTraits { get; private set; }

		public DefaultPolicies DefaultPolicies { get; private set; }

		public DefaultBuildingTypes DefaultBuildingTypes { get; private set; }

		public DefaultIssueEffects DefaultIssueEffects { get; private set; }

		public DefaultItems DefaultItems { get; private set; }

		public DefaultSiegeStrategies DefaultSiegeStrategies { get; private set; }

		internal MBReadOnlyList<PerkObject> AllPerks { get; private set; }

		public DefaultSkillEffects DefaultSkillEffects { get; private set; }

		public DefaultVillageTypes DefaultVillageTypes { get; private set; }

		internal MBReadOnlyList<TraitObject> AllTraits { get; private set; }

		internal MBReadOnlyList<MBEquipmentRoster> AllEquipmentRosters { get; private set; }

		public DefaultCulturalFeats DefaultFeats { get; private set; }

		internal MBReadOnlyList<PolicyObject> AllPolicies { get; private set; }

		internal MBReadOnlyList<BuildingType> AllBuildingTypes { get; private set; }

		internal MBReadOnlyList<IssueEffect> AllIssueEffects { get; private set; }

		internal MBReadOnlyList<SiegeStrategy> AllSiegeStrategies { get; private set; }

		internal MBReadOnlyList<VillageType> AllVillageTypes { get; private set; }

		internal MBReadOnlyList<SkillEffect> AllSkillEffects { get; private set; }

		internal MBReadOnlyList<FeatObject> AllFeats { get; private set; }

		internal MBReadOnlyList<SkillObject> AllSkills { get; private set; }

		internal MBReadOnlyList<SiegeEngineType> AllSiegeEngineTypes { get; private set; }

		internal MBReadOnlyList<ItemCategory> AllItemCategories { get; private set; }

		internal MBReadOnlyList<CharacterAttribute> AllCharacterAttributes { get; private set; }

		internal MBReadOnlyList<ItemObject> AllItems { get; private set; }

		[SaveableProperty(100)]
		internal MapTimeTracker MapTimeTracker { get; private set; }

		public bool TimeControlModeLock { get; private set; }

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

		public bool IsMapTooltipLongForm { get; set; }

		public float SpeedUpMultiplier { get; set; } = 4f;

		public float CampaignDt
		{
			get
			{
				return this._dt;
			}
		}

		public bool TrueSight { get; set; }

		public static Campaign Current { get; private set; }

		[SaveableProperty(36)]
		public CampaignTime CampaignStartTime { get; private set; }

		[SaveableProperty(37)]
		public CampaignGameMode GameMode { get; private set; }

		[SaveableProperty(38)]
		public float PlayerProgress { get; private set; }

		public GameMenuManager GameMenuManager { get; private set; }

		public GameModels Models
		{
			get
			{
				return this._gameModels;
			}
		}

		public SandBoxManager SandBoxManager { get; private set; }

		public Campaign.GameLoadingType CampaignGameLoadingType
		{
			get
			{
				return this._gameLoadingType;
			}
		}

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

		[SaveableProperty(40)]
		public SiegeEventManager SiegeEventManager { get; internal set; }

		[SaveableProperty(41)]
		public MapEventManager MapEventManager { get; internal set; }

		internal CampaignEvents CampaignEvents { get; private set; }

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
				GameState activeState = gameStateManager.ActiveState;
				MapState mapState2;
				if (((activeState != null) ? activeState.Predecessor : null) != null && (mapState2 = gameStateManager.ActiveState.Predecessor as MapState) != null)
				{
					return mapState2.MenuContext;
				}
				return null;
			}
		}

		internal List<MBCampaignEvent> CustomPeriodicCampaignEvents { get; private set; }

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

		[SaveableProperty(45)]
		private int _curMapFrame { get; set; }

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

		public IMapScene MapSceneWrapper
		{
			get
			{
				return this._mapSceneWrapper;
			}
		}

		[SaveableProperty(54)]
		public PlayerEncounter PlayerEncounter { get; internal set; }

		[CachedData]
		internal LocationEncounter LocationEncounter { get; set; }

		internal NameGenerator NameGenerator { get; private set; }

		[SaveableProperty(58)]
		public BarterManager BarterManager { get; private set; }

		[SaveableProperty(69)]
		public bool IsMainHeroDisguised { get; set; }

		[SaveableProperty(70)]
		public bool DesertionEnabled { get; set; }

		public Vec2 DefaultStartingPosition
		{
			get
			{
				return new Vec2(685.3f, 410.9f);
			}
		}

		public Equipment DeadBattleEquipment { get; set; }

		public Equipment DeadCivilianEquipment { get; set; }

		public void InitializeMainParty()
		{
			this.InitializeSinglePlayerReferences();
			this.MainParty.InitializeMobilePartyAtPosition(base.CurrentGame.ObjectManager.GetObject<PartyTemplateObject>("main_hero_party_template"), this.DefaultStartingPosition, -1);
			this.MainParty.ActualClan = Clan.PlayerClan;
			this.MainParty.PartyComponent = new LordPartyComponent(Hero.MainHero, Hero.MainHero);
			this.MainParty.ItemRoster.AddToCounts(DefaultItems.Grain, 1);
		}

		[LoadInitializationCallback]
		private void OnLoad(MetaData metaData, ObjectLoadData objectLoadData)
		{
			this._campaignEntitySystem = new EntitySystem<CampaignEntityComponent>();
			this.PlayerFormationPreferences = this._playerFormationPreferences.GetReadOnlyDictionary<CharacterObject, FormationClass>();
			this.SpeedUpMultiplier = 4f;
			if (this.UniqueGameId == null && MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.2", 27066))
			{
				this.UniqueGameId = "oldSave";
			}
		}

		private void InitializeForSavedGame()
		{
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
			this.KingdomManager.OnSessionStart();
			this.CampaignInformationManager.RegisterEvents();
		}

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

		private void GameInitTick()
		{
			foreach (Settlement settlement in Settlement.All)
			{
				settlement.Party.UpdateVisibilityAndInspected(0f);
			}
			foreach (MobileParty mobileParty in this.MobileParties)
			{
				mobileParty.Party.UpdateVisibilityAndInspected(0f);
			}
		}

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

		internal void DailyTick(MBCampaignEvent campaignEvent, object[] delegateParams)
		{
			this.PlayerProgress = (this.PlayerProgress + Campaign.Current.Models.PlayerProgressionModel.GetPlayerProgress()) / 2f;
			Debug.Print("Before Daily Tick: " + CampaignTime.Now.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
			CampaignEventDispatcher.Instance.DailyTick();
			if ((int)this.CampaignStartTime.ElapsedDaysUntilNow % 7 == 0)
			{
				CampaignEventDispatcher.Instance.WeeklyTick();
				this.OnWeeklyTick();
			}
		}

		public void WaitAsyncTasks()
		{
			if (this.CampaignLateAITickTask != null)
			{
				this.CampaignLateAITickTask.Wait();
			}
		}

		private void OnWeeklyTick()
		{
			this.LogEntryHistory.DeleteOutdatedLogs();
		}

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

		private void CheckMainPartyNeedsUpdate()
		{
			MobileParty.MainParty.Ai.CheckPartyNeedsUpdate();
		}

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

		public void OnGameOver()
		{
			if (CampaignOptions.IsIronmanMode)
			{
				this.SaveHandler.QuickSaveCurrentGame();
			}
		}

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

		public static float CurrentTime
		{
			get
			{
				return (float)CampaignTime.Now.ToHours;
			}
		}

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

		public static void LateAITick()
		{
			Campaign.Current.LateAITickAux();
		}

		internal void LateAITickAux()
		{
			if (this._dt > 0f || this._curSessionFrame < 3)
			{
				this.PartiesThink(this._dt);
			}
		}

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

		private void PartiesThink(float dt)
		{
			for (int i = 0; i < this.MobileParties.Count; i++)
			{
				this.MobileParties[i].Ai.Tick(dt);
			}
		}

		public TComponent GetEntityComponent<TComponent>() where TComponent : CampaignEntityComponent
		{
			EntitySystem<CampaignEntityComponent> campaignEntitySystem = this._campaignEntitySystem;
			if (campaignEntitySystem == null)
			{
				return default(TComponent);
			}
			return campaignEntitySystem.GetComponent<TComponent>();
		}

		public TComponent AddEntityComponent<TComponent>() where TComponent : CampaignEntityComponent, new()
		{
			return this._campaignEntitySystem.AddComponent<TComponent>();
		}

		public MBReadOnlyList<CampaignEntityComponent> CampaignEntityComponents
		{
			get
			{
				return this._campaignEntitySystem.Components;
			}
		}

		public T GetCampaignBehavior<T>()
		{
			return this._campaignBehaviorManager.GetBehavior<T>();
		}

		public IEnumerable<T> GetCampaignBehaviors<T>()
		{
			return this._campaignBehaviorManager.GetBehaviors<T>();
		}

		public void AddCampaignBehaviorManager(ICampaignBehaviorManager manager)
		{
			this._campaignBehaviorManager = manager;
		}

		public MBReadOnlyList<Hero> AliveHeroes
		{
			get
			{
				return this.CampaignObjectManager.AliveHeroes;
			}
		}

		public MBReadOnlyList<Hero> DeadOrDisabledHeroes
		{
			get
			{
				return this.CampaignObjectManager.DeadOrDisabledHeroes;
			}
		}

		public MBReadOnlyList<MobileParty> MobileParties
		{
			get
			{
				return this.CampaignObjectManager.MobileParties;
			}
		}

		public MBReadOnlyList<MobileParty> CaravanParties
		{
			get
			{
				return this.CampaignObjectManager.CaravanParties;
			}
		}

		public MBReadOnlyList<MobileParty> VillagerParties
		{
			get
			{
				return this.CampaignObjectManager.VillagerParties;
			}
		}

		public MBReadOnlyList<MobileParty> MilitiaParties
		{
			get
			{
				return this.CampaignObjectManager.MilitiaParties;
			}
		}

		public MBReadOnlyList<MobileParty> GarrisonParties
		{
			get
			{
				return this.CampaignObjectManager.GarrisonParties;
			}
		}

		public MBReadOnlyList<MobileParty> CustomParties
		{
			get
			{
				return this.CampaignObjectManager.CustomParties;
			}
		}

		public MBReadOnlyList<MobileParty> LordParties
		{
			get
			{
				return this.CampaignObjectManager.LordParties;
			}
		}

		public MBReadOnlyList<MobileParty> BanditParties
		{
			get
			{
				return this.CampaignObjectManager.BanditParties;
			}
		}

		public MBReadOnlyList<MobileParty> PartiesWithoutPartyComponent
		{
			get
			{
				return this.CampaignObjectManager.PartiesWithoutPartyComponent;
			}
		}

		public MBReadOnlyList<Settlement> Settlements
		{
			get
			{
				return this.CampaignObjectManager.Settlements;
			}
		}

		public IEnumerable<IFaction> Factions
		{
			get
			{
				return this.CampaignObjectManager.Factions;
			}
		}

		public MBReadOnlyList<Kingdom> Kingdoms
		{
			get
			{
				return this.CampaignObjectManager.Kingdoms;
			}
		}

		public MBReadOnlyList<Clan> Clans
		{
			get
			{
				return this.CampaignObjectManager.Clans;
			}
		}

		public MBReadOnlyList<CharacterObject> Characters
		{
			get
			{
				return this._characters;
			}
		}

		public MBReadOnlyList<WorkshopType> Workshops
		{
			get
			{
				return this._workshops;
			}
		}

		public MBReadOnlyList<ItemModifier> ItemModifiers
		{
			get
			{
				return this._itemModifiers;
			}
		}

		public MBReadOnlyList<ItemModifierGroup> ItemModifierGroups
		{
			get
			{
				return this._itemModifierGroups;
			}
		}

		public MBReadOnlyList<Concept> Concepts
		{
			get
			{
				return this._concepts;
			}
		}

		[SaveableProperty(60)]
		public MobileParty MainParty { get; private set; }

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

		[SaveableProperty(62)]
		public CampaignInformationManager CampaignInformationManager { get; set; }

		[SaveableProperty(63)]
		public VisualTrackerManager VisualTrackerManager { get; set; }

		public LogEntryHistory LogEntryHistory
		{
			get
			{
				return this._logEntryHistory;
			}
		}

		public EncyclopediaManager EncyclopediaManager { get; private set; }

		public InventoryManager InventoryManager { get; private set; }

		public PartyScreenManager PartyScreenManager { get; private set; }

		public ConversationManager ConversationManager { get; private set; }

		public bool IsDay
		{
			get
			{
				return !this.IsNight;
			}
		}

		public bool IsNight
		{
			get
			{
				return CampaignTime.Now.IsNightTime;
			}
		}

		internal int GeneratePartyId(PartyBase party)
		{
			int lastPartyIndex = this._lastPartyIndex;
			this._lastPartyIndex++;
			return lastPartyIndex;
		}

		[SaveableProperty(68)]
		public HeroTraitDeveloper PlayerTraitDeveloper { get; private set; }

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

		public void UpdateMaximumDistanceBetweenTwoSettlements()
		{
			Campaign.MaximumDistanceBetweenTwoSettlements = Campaign.Current.Models.MapDistanceModel.MaximumDistanceBetweenTwoSettlements;
		}

		private void InitializeCachedLists()
		{
			MBObjectManager objectManager = Game.Current.ObjectManager;
			this._characters = objectManager.GetObjectTypeList<CharacterObject>();
			this._workshops = objectManager.GetObjectTypeList<WorkshopType>();
			this._itemModifiers = objectManager.GetObjectTypeList<ItemModifier>();
			this._itemModifierGroups = objectManager.GetObjectTypeList<ItemModifierGroup>();
			this._concepts = objectManager.GetObjectTypeList<Concept>();
		}

		private void InitializeDefaultEquipments()
		{
			this.DeadBattleEquipment = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>("default_battle_equipment_roster_neutral").DefaultEquipment;
			this.DeadCivilianEquipment = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>("default_civilian_equipment_roster_neutral").DefaultEquipment;
		}

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
			MBTextManager.ClearAll();
			GameSceneDataManager.Destroy();
			this.CampaignInformationManager.DeRegisterEvents();
			MBSaveLoad.OnGameDestroy();
			Campaign.Current = null;
		}

		public void InitializeSinglePlayerReferences()
		{
			this.IsInitializedSinglePlayerReferences = true;
			this.InitializeGamePlayReferences();
		}

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

		private void CalculateCachedValues()
		{
			this.CalculateAverageDistanceBetweenTowns();
			this.CalculateAverageWage();
		}

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

		private void InitializeScenes()
		{
			GameSceneDataManager.Instance.LoadSPBattleScenes(ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/sp_battle_scenes.xml");
			GameSceneDataManager.Instance.LoadConversationScenes(ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/conversation_scenes.xml");
			GameSceneDataManager.Instance.LoadMeetingScenes(ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/meeting_scenes.xml");
		}

		public void SetLoadingParameters(Campaign.GameLoadingType gameLoadingType)
		{
			Campaign.Current = this;
			this._gameLoadingType = gameLoadingType;
			if (gameLoadingType == Campaign.GameLoadingType.SavedCampaign)
			{
				Campaign.Current.GameStarted = true;
			}
		}

		public void AddCampaignEventReceiver(CampaignEventReceiver receiver)
		{
			this.CampaignEventDispatcher.AddCampaignEventReceiver(receiver);
		}

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
			this.InitializeDefaultEquipments();
			this.NameGenerator.Initialize();
			base.CurrentGame.OnGameStart();
			base.GameManager.OnGameInitializationFinished(base.CurrentGame);
		}

		private void CalculateCachedStatsOnLoad()
		{
			ItemRoster.CalculateCachedStatsOnLoad();
		}

		private void InitializeBasicObjectXmls()
		{
			base.ObjectManager.LoadXML("SPCultures", false);
			base.ObjectManager.LoadXML("Concepts", false);
		}

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
		}

		private void InitializeManagers()
		{
			this.KingdomManager = new KingdomManager();
			this.CampaignInformationManager = new CampaignInformationManager();
			this.VisualTrackerManager = new VisualTrackerManager();
			this.TournamentManager = new TournamentManager();
		}

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

		private void OnNewCampaignStart()
		{
			Game.Current.PlayerTroop = null;
			this.MapStateData = new MapStateData();
			this.InitializeDefaultCampaignObjects();
			this.MainParty = MBObjectManager.Instance.CreateObject<MobileParty>("player_party");
			this.MainParty.Ai.SetAsMainParty();
			this.InitializeManagers();
		}

		protected override void BeforeRegisterTypes(MBObjectManager objectManager)
		{
			objectManager.RegisterType<FeatObject>("Feat", "Feats", 0U, true, false);
		}

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

		private void CreateManagers()
		{
			this.EncyclopediaManager = new EncyclopediaManager();
			this.InventoryManager = new InventoryManager();
			this.PartyScreenManager = new PartyScreenManager();
			this.ConversationManager = new ConversationManager();
			this.NameGenerator = new NameGenerator();
			this.SkillLevelingManager = new DefaultSkillLevelingManager();
		}

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

		public override void OnMissionIsStarting(string missionName, MissionInitializerRecord rec)
		{
			if (rec.PlayingInCampaignMode)
			{
				CampaignEventDispatcher.Instance.BeforeMissionOpened();
			}
		}

		public override void InitializeParameters()
		{
			ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_campaign_parameters"));
		}

		public void SetTimeControlModeLock(bool isLocked)
		{
			this.TimeControlModeLock = isLocked;
		}

		public override bool IsPartyWindowAccessibleAtMission
		{
			get
			{
				return this.GameMode == CampaignGameMode.Tutorial;
			}
		}

		internal MBReadOnlyList<Town> AllTowns
		{
			get
			{
				return this._towns;
			}
		}

		internal MBReadOnlyList<Town> AllCastles
		{
			get
			{
				return this._castles;
			}
		}

		internal MBReadOnlyList<Village> AllVillages
		{
			get
			{
				return this._villages;
			}
		}

		internal MBReadOnlyList<Hideout> AllHideouts
		{
			get
			{
				return this._hideouts;
			}
		}

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
			PartyBase.MainParty.UpdateVisibilityAndInspected(0f);
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

		public void SetPlayerFormationPreference(CharacterObject character, FormationClass formation)
		{
			if (!this._playerFormationPreferences.ContainsKey(character))
			{
				this._playerFormationPreferences.Add(character, formation);
				return;
			}
			this._playerFormationPreferences[character] = formation;
		}

		public override void OnStateChanged(GameState oldState)
		{
		}

		internal static void AutoGeneratedStaticCollectObjectsCampaign(object o, List<object> collectedObjects)
		{
			((Campaign)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

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

		internal static object AutoGeneratedGetMemberValueEnabledCheatsBefore(object o)
		{
			return ((Campaign)o).EnabledCheatsBefore;
		}

		internal static object AutoGeneratedGetMemberValuePlatformID(object o)
		{
			return ((Campaign)o).PlatformID;
		}

		internal static object AutoGeneratedGetMemberValueUniqueGameId(object o)
		{
			return ((Campaign)o).UniqueGameId;
		}

		internal static object AutoGeneratedGetMemberValueCampaignObjectManager(object o)
		{
			return ((Campaign)o).CampaignObjectManager;
		}

		internal static object AutoGeneratedGetMemberValueIsCraftingEnabled(object o)
		{
			return ((Campaign)o).IsCraftingEnabled;
		}

		internal static object AutoGeneratedGetMemberValueIsBannerEditorEnabled(object o)
		{
			return ((Campaign)o).IsBannerEditorEnabled;
		}

		internal static object AutoGeneratedGetMemberValueIsFaceGenEnabled(object o)
		{
			return ((Campaign)o).IsFaceGenEnabled;
		}

		internal static object AutoGeneratedGetMemberValueQuestManager(object o)
		{
			return ((Campaign)o).QuestManager;
		}

		internal static object AutoGeneratedGetMemberValueIssueManager(object o)
		{
			return ((Campaign)o).IssueManager;
		}

		internal static object AutoGeneratedGetMemberValueFactionManager(object o)
		{
			return ((Campaign)o).FactionManager;
		}

		internal static object AutoGeneratedGetMemberValueCharacterRelationManager(object o)
		{
			return ((Campaign)o).CharacterRelationManager;
		}

		internal static object AutoGeneratedGetMemberValueRomance(object o)
		{
			return ((Campaign)o).Romance;
		}

		internal static object AutoGeneratedGetMemberValuePlayerCaptivity(object o)
		{
			return ((Campaign)o).PlayerCaptivity;
		}

		internal static object AutoGeneratedGetMemberValuePlayerDefaultFaction(object o)
		{
			return ((Campaign)o).PlayerDefaultFaction;
		}

		internal static object AutoGeneratedGetMemberValueMapStateData(object o)
		{
			return ((Campaign)o).MapStateData;
		}

		internal static object AutoGeneratedGetMemberValueMapTimeTracker(object o)
		{
			return ((Campaign)o).MapTimeTracker;
		}

		internal static object AutoGeneratedGetMemberValueCampaignStartTime(object o)
		{
			return ((Campaign)o).CampaignStartTime;
		}

		internal static object AutoGeneratedGetMemberValueGameMode(object o)
		{
			return ((Campaign)o).GameMode;
		}

		internal static object AutoGeneratedGetMemberValuePlayerProgress(object o)
		{
			return ((Campaign)o).PlayerProgress;
		}

		internal static object AutoGeneratedGetMemberValueSiegeEventManager(object o)
		{
			return ((Campaign)o).SiegeEventManager;
		}

		internal static object AutoGeneratedGetMemberValueMapEventManager(object o)
		{
			return ((Campaign)o).MapEventManager;
		}

		internal static object AutoGeneratedGetMemberValue_curMapFrame(object o)
		{
			return ((Campaign)o)._curMapFrame;
		}

		internal static object AutoGeneratedGetMemberValuePlayerEncounter(object o)
		{
			return ((Campaign)o).PlayerEncounter;
		}

		internal static object AutoGeneratedGetMemberValueBarterManager(object o)
		{
			return ((Campaign)o).BarterManager;
		}

		internal static object AutoGeneratedGetMemberValueIsMainHeroDisguised(object o)
		{
			return ((Campaign)o).IsMainHeroDisguised;
		}

		internal static object AutoGeneratedGetMemberValueDesertionEnabled(object o)
		{
			return ((Campaign)o).DesertionEnabled;
		}

		internal static object AutoGeneratedGetMemberValueMainParty(object o)
		{
			return ((Campaign)o).MainParty;
		}

		internal static object AutoGeneratedGetMemberValueCampaignInformationManager(object o)
		{
			return ((Campaign)o).CampaignInformationManager;
		}

		internal static object AutoGeneratedGetMemberValueVisualTrackerManager(object o)
		{
			return ((Campaign)o).VisualTrackerManager;
		}

		internal static object AutoGeneratedGetMemberValuePlayerTraitDeveloper(object o)
		{
			return ((Campaign)o).PlayerTraitDeveloper;
		}

		internal static object AutoGeneratedGetMemberValueOptions(object o)
		{
			return ((Campaign)o).Options;
		}

		internal static object AutoGeneratedGetMemberValueTournamentManager(object o)
		{
			return ((Campaign)o).TournamentManager;
		}

		internal static object AutoGeneratedGetMemberValueIsInitializedSinglePlayerReferences(object o)
		{
			return ((Campaign)o).IsInitializedSinglePlayerReferences;
		}

		internal static object AutoGeneratedGetMemberValueLastTimeControlMode(object o)
		{
			return ((Campaign)o).LastTimeControlMode;
		}

		internal static object AutoGeneratedGetMemberValueautoEnterTown(object o)
		{
			return ((Campaign)o).autoEnterTown;
		}

		internal static object AutoGeneratedGetMemberValueMainHeroIllDays(object o)
		{
			return ((Campaign)o).MainHeroIllDays;
		}

		internal static object AutoGeneratedGetMemberValueKingdomManager(object o)
		{
			return ((Campaign)o).KingdomManager;
		}

		internal static object AutoGeneratedGetMemberValue_campaignPeriodicEventManager(object o)
		{
			return ((Campaign)o)._campaignPeriodicEventManager;
		}

		internal static object AutoGeneratedGetMemberValue_isMainPartyWaiting(object o)
		{
			return ((Campaign)o)._isMainPartyWaiting;
		}

		internal static object AutoGeneratedGetMemberValue_newGameVersion(object o)
		{
			return ((Campaign)o)._newGameVersion;
		}

		internal static object AutoGeneratedGetMemberValue_previouslyUsedModules(object o)
		{
			return ((Campaign)o)._previouslyUsedModules;
		}

		internal static object AutoGeneratedGetMemberValue_usedGameVersions(object o)
		{
			return ((Campaign)o)._usedGameVersions;
		}

		internal static object AutoGeneratedGetMemberValue_playerFormationPreferences(object o)
		{
			return ((Campaign)o)._playerFormationPreferences;
		}

		internal static object AutoGeneratedGetMemberValue_campaignBehaviorManager(object o)
		{
			return ((Campaign)o)._campaignBehaviorManager;
		}

		internal static object AutoGeneratedGetMemberValue_lastPartyIndex(object o)
		{
			return ((Campaign)o)._lastPartyIndex;
		}

		internal static object AutoGeneratedGetMemberValue_cameraFollowParty(object o)
		{
			return ((Campaign)o)._cameraFollowParty;
		}

		internal static object AutoGeneratedGetMemberValue_logEntryHistory(object o)
		{
			return ((Campaign)o)._logEntryHistory;
		}

		public const float ConfigTimeMultiplier = 0.25f;

		private EntitySystem<CampaignEntityComponent> _campaignEntitySystem;

		public ITask CampaignLateAITickTask;

		[SaveableField(210)]
		private CampaignPeriodicEventManager _campaignPeriodicEventManager;

		[SaveableField(53)]
		private bool _isMainPartyWaiting;

		[SaveableField(344)]
		private string _newGameVersion;

		[SaveableField(78)]
		private MBList<string> _previouslyUsedModules;

		[SaveableField(81)]
		private MBList<string> _usedGameVersions;

		[SaveableField(77)]
		private Dictionary<CharacterObject, FormationClass> _playerFormationPreferences;

		[SaveableField(7)]
		private ICampaignBehaviorManager _campaignBehaviorManager;

		private CampaignTickCacheDataStore _tickData;

		[SaveableField(2)]
		public readonly CampaignOptions Options;

		public MBReadOnlyDictionary<CharacterObject, FormationClass> PlayerFormationPreferences;

		[SaveableField(13)]
		public ITournamentManager TournamentManager;

		public float MinSettlementX;

		public float MaxSettlementX;

		public float MinSettlementY;

		public float MaxSettlementY;

		[SaveableField(27)]
		public bool IsInitializedSinglePlayerReferences;

		private LocatorGrid<MobileParty> _mobilePartyLocator;

		private LocatorGrid<Settlement> _settlementLocator;

		private GameModels _gameModels;

		[SaveableField(31)]
		public CampaignTimeControlMode LastTimeControlMode = CampaignTimeControlMode.UnstoppablePlay;

		private IMapScene _mapSceneWrapper;

		public bool GameStarted;

		[SaveableField(44)]
		public PartyBase autoEnterTown;

		private Campaign.GameLoadingType _gameLoadingType;

		public ConversationContext CurrentConversationContext;

		[CachedData]
		private float _dt;

		private CampaignTimeControlMode _timeControlMode;

		private int _curSessionFrame;

		[SaveableField(30)]
		public int MainHeroIllDays = -1;

		private MBCampaignEvent _dailyTickEvent;

		private MBCampaignEvent _hourlyTickEvent;

		[CachedData]
		private int _lastNonZeroDtFrame;

		private MBList<Town> _towns;

		private MBList<Town> _castles;

		private MBList<Village> _villages;

		private MBList<Hideout> _hideouts;

		private MBReadOnlyList<CharacterObject> _characters;

		private MBReadOnlyList<WorkshopType> _workshops;

		private MBReadOnlyList<ItemModifier> _itemModifiers;

		private MBReadOnlyList<Concept> _concepts;

		private MBReadOnlyList<ItemModifierGroup> _itemModifierGroups;

		[SaveableField(79)]
		private int _lastPartyIndex;

		[SaveableField(61)]
		private PartyBase _cameraFollowParty;

		[SaveableField(64)]
		private readonly LogEntryHistory _logEntryHistory = new LogEntryHistory();

		[SaveableField(65)]
		public KingdomManager KingdomManager;

		[Flags]
		public enum PartyRestFlags : uint
		{
			None = 0U,
			SafeMode = 1U
		}

		public enum GameLoadingType
		{
			Tutorial,
			NewCampaign,
			SavedCampaign,
			Editor
		}
	}
}
