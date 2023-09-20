using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.SaveSystem.Save;

namespace TaleWorlds.Core
{
	// Token: 0x02000062 RID: 98
	[SaveableRootClass(5000)]
	public sealed class Game : IGameStateManagerOwner
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600065E RID: 1630 RVA: 0x000171E0 File Offset: 0x000153E0
		// (remove) Token: 0x0600065F RID: 1631 RVA: 0x00017214 File Offset: 0x00015414
		public static event Action OnGameCreated;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000660 RID: 1632 RVA: 0x00017248 File Offset: 0x00015448
		// (remove) Token: 0x06000661 RID: 1633 RVA: 0x00017280 File Offset: 0x00015480
		public event Action<ItemObject> OnItemDeserializedEvent;

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x000172B5 File Offset: 0x000154B5
		// (set) Token: 0x06000663 RID: 1635 RVA: 0x000172BD File Offset: 0x000154BD
		public Game.State CurrentState { get; private set; }

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000664 RID: 1636 RVA: 0x000172C6 File Offset: 0x000154C6
		// (set) Token: 0x06000665 RID: 1637 RVA: 0x000172CE File Offset: 0x000154CE
		public IMonsterMissionDataCreator MonsterMissionDataCreator { get; set; }

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000666 RID: 1638 RVA: 0x000172D8 File Offset: 0x000154D8
		public Monster DefaultMonster
		{
			get
			{
				Monster monster;
				if ((monster = this._defaultMonster) == null)
				{
					monster = (this._defaultMonster = this.ObjectManager.GetFirstObject<Monster>());
				}
				return monster;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000667 RID: 1639 RVA: 0x00017303 File Offset: 0x00015503
		// (set) Token: 0x06000668 RID: 1640 RVA: 0x0001730B File Offset: 0x0001550B
		[SaveableProperty(3)]
		public GameType GameType { get; private set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000669 RID: 1641 RVA: 0x00017314 File Offset: 0x00015514
		// (set) Token: 0x0600066A RID: 1642 RVA: 0x0001731C File Offset: 0x0001551C
		public DefaultSiegeEngineTypes DefaultSiegeEngineTypes { get; private set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x0600066B RID: 1643 RVA: 0x00017325 File Offset: 0x00015525
		// (set) Token: 0x0600066C RID: 1644 RVA: 0x0001732D File Offset: 0x0001552D
		public MBObjectManager ObjectManager { get; private set; }

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x0600066D RID: 1645 RVA: 0x00017336 File Offset: 0x00015536
		// (set) Token: 0x0600066E RID: 1646 RVA: 0x0001733E File Offset: 0x0001553E
		[SaveableProperty(8)]
		public BasicCharacterObject PlayerTroop { get; set; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x0600066F RID: 1647 RVA: 0x00017347 File Offset: 0x00015547
		// (set) Token: 0x06000670 RID: 1648 RVA: 0x0001734F File Offset: 0x0001554F
		[SaveableProperty(12)]
		internal MBFastRandom RandomGenerator { get; private set; }

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000671 RID: 1649 RVA: 0x00017358 File Offset: 0x00015558
		// (set) Token: 0x06000672 RID: 1650 RVA: 0x00017360 File Offset: 0x00015560
		public BasicGameModels BasicModels { get; private set; }

		// Token: 0x06000673 RID: 1651 RVA: 0x0001736C File Offset: 0x0001556C
		public T AddGameModelsManager<T>(IEnumerable<GameModel> inputComponents) where T : GameModelsManager
		{
			T t = (T)((object)Activator.CreateInstance(typeof(T), new object[] { inputComponents }));
			this._gameModelManagers.Add(typeof(T), t);
			return t;
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000674 RID: 1652 RVA: 0x000173B4 File Offset: 0x000155B4
		// (set) Token: 0x06000675 RID: 1653 RVA: 0x000173BC File Offset: 0x000155BC
		public GameManagerBase GameManager { get; private set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x000173C5 File Offset: 0x000155C5
		// (set) Token: 0x06000677 RID: 1655 RVA: 0x000173CD File Offset: 0x000155CD
		public GameTextManager GameTextManager { get; private set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x000173D6 File Offset: 0x000155D6
		// (set) Token: 0x06000679 RID: 1657 RVA: 0x000173DE File Offset: 0x000155DE
		public GameStateManager GameStateManager { get; private set; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x000173E7 File Offset: 0x000155E7
		public bool CheatMode
		{
			get
			{
				return this.GameManager.CheatMode;
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x000173F4 File Offset: 0x000155F4
		public bool IsDevelopmentMode
		{
			get
			{
				return this.GameManager.IsDevelopmentMode;
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x00017401 File Offset: 0x00015601
		public bool IsEditModeOn
		{
			get
			{
				return this.GameManager.IsEditModeOn;
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x0600067D RID: 1661 RVA: 0x0001740E File Offset: 0x0001560E
		public UnitSpawnPrioritizations UnitSpawnPrioritization
		{
			get
			{
				return this.GameManager.UnitSpawnPrioritization;
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x0600067E RID: 1662 RVA: 0x0001741B File Offset: 0x0001561B
		public float ApplicationTime
		{
			get
			{
				return this.GameManager.ApplicationTime;
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x0600067F RID: 1663 RVA: 0x00017428 File Offset: 0x00015628
		// (set) Token: 0x06000680 RID: 1664 RVA: 0x0001742F File Offset: 0x0001562F
		public static Game Current
		{
			get
			{
				return Game._current;
			}
			internal set
			{
				Game._current = value;
				Action onGameCreated = Game.OnGameCreated;
				if (onGameCreated == null)
				{
					return;
				}
				onGameCreated();
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000681 RID: 1665 RVA: 0x00017446 File Offset: 0x00015646
		// (set) Token: 0x06000682 RID: 1666 RVA: 0x0001744E File Offset: 0x0001564E
		public IBannerVisualCreator BannerVisualCreator { get; set; }

		// Token: 0x06000683 RID: 1667 RVA: 0x00017457 File Offset: 0x00015657
		public IBannerVisual CreateBannerVisual(Banner banner)
		{
			if (this.BannerVisualCreator == null)
			{
				return null;
			}
			return this.BannerVisualCreator.CreateBannerVisual(banner);
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x00017470 File Offset: 0x00015670
		public int NextUniqueTroopSeed
		{
			get
			{
				int nextUniqueTroopSeed = this._nextUniqueTroopSeed;
				this._nextUniqueTroopSeed = nextUniqueTroopSeed + 1;
				return nextUniqueTroopSeed;
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06000685 RID: 1669 RVA: 0x0001748E File Offset: 0x0001568E
		// (set) Token: 0x06000686 RID: 1670 RVA: 0x00017496 File Offset: 0x00015696
		public DefaultCharacterAttributes DefaultCharacterAttributes { get; private set; }

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000687 RID: 1671 RVA: 0x0001749F File Offset: 0x0001569F
		// (set) Token: 0x06000688 RID: 1672 RVA: 0x000174A7 File Offset: 0x000156A7
		public DefaultSkills DefaultSkills { get; private set; }

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000689 RID: 1673 RVA: 0x000174B0 File Offset: 0x000156B0
		// (set) Token: 0x0600068A RID: 1674 RVA: 0x000174B8 File Offset: 0x000156B8
		public DefaultBannerEffects DefaultBannerEffects { get; private set; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x0600068B RID: 1675 RVA: 0x000174C1 File Offset: 0x000156C1
		// (set) Token: 0x0600068C RID: 1676 RVA: 0x000174C9 File Offset: 0x000156C9
		public DefaultItemCategories DefaultItemCategories { get; private set; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x000174D2 File Offset: 0x000156D2
		// (set) Token: 0x0600068E RID: 1678 RVA: 0x000174DA File Offset: 0x000156DA
		public EventManager EventManager { get; private set; }

		// Token: 0x0600068F RID: 1679 RVA: 0x000174E4 File Offset: 0x000156E4
		public Equipment GetDefaultEquipmentWithName(string equipmentName)
		{
			if (!this._defaultEquipments.ContainsKey(equipmentName))
			{
				Debug.FailedAssert("Equipment with name \"" + equipmentName + "\" could not be found.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Game.cs", "GetDefaultEquipmentWithName", 130);
				return null;
			}
			return this._defaultEquipments[equipmentName].Clone(false);
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x00017537 File Offset: 0x00015737
		public void SetDefaultEquipments(IReadOnlyDictionary<string, Equipment> defaultEquipments)
		{
			if (this._defaultEquipments == null)
			{
				this._defaultEquipments = defaultEquipments;
			}
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x00017548 File Offset: 0x00015748
		public static Game CreateGame(GameType gameType, GameManagerBase gameManager, int seed)
		{
			Game game = Game.CreateGame(gameType, gameManager);
			game.RandomGenerator = new MBFastRandom((uint)seed);
			return game;
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x00017560 File Offset: 0x00015760
		private Game(GameType gameType, GameManagerBase gameManager, MBObjectManager objectManager)
		{
			this.GameType = gameType;
			Game.Current = this;
			this.GameType.CurrentGame = this;
			this.GameManager = gameManager;
			this.GameManager.Game = this;
			this.EventManager = new EventManager();
			this.ObjectManager = objectManager;
			this.RandomGenerator = new MBFastRandom();
			this.InitializeParameters();
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x000175CC File Offset: 0x000157CC
		public static Game CreateGame(GameType gameType, GameManagerBase gameManager)
		{
			MBObjectManager mbobjectManager = MBObjectManager.Init();
			Game.RegisterTypes(gameType, mbobjectManager);
			return new Game(gameType, gameManager, mbobjectManager);
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x000175F0 File Offset: 0x000157F0
		public static Game LoadSaveGame(LoadResult loadResult, GameManagerBase gameManager)
		{
			MBSaveLoad.OnStartGame(loadResult);
			MBObjectManager mbobjectManager = MBObjectManager.Init();
			Game game = (Game)loadResult.Root;
			Game.RegisterTypes(game.GameType, mbobjectManager);
			loadResult.InitializeObjects();
			MBObjectManager.Instance.ReInitialize();
			GC.Collect();
			game.ObjectManager = mbobjectManager;
			game.BeginLoading(gameManager);
			return game;
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x00017643 File Offset: 0x00015843
		[LoadInitializationCallback]
		private void OnLoad()
		{
			if (this.RandomGenerator == null)
			{
				this.RandomGenerator = new MBFastRandom();
			}
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00017658 File Offset: 0x00015858
		private void BeginLoading(GameManagerBase gameManager)
		{
			Game.Current = this;
			this.GameType.CurrentGame = this;
			this.GameManager = gameManager;
			this.GameManager.Game = this;
			this.EventManager = new EventManager();
			this.InitializeParameters();
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x00017690 File Offset: 0x00015890
		private void SaveAux(MetaData metaData, string saveName, ISaveDriver driver, Action<SaveResult> onSaveCompleted)
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnBeforeSave();
			}
			SaveOutput saveOutput = SaveManager.Save(this, metaData, saveName, driver);
			if (!saveOutput.IsContinuing)
			{
				this.OnSaveCompleted(saveOutput, onSaveCompleted);
				return;
			}
			this._currentActiveSaveData = new Tuple<SaveOutput, Action<SaveResult>>(saveOutput, onSaveCompleted);
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x00017710 File Offset: 0x00015910
		private void OnSaveCompleted(SaveOutput finishedOutput, Action<SaveResult> onSaveCompleted)
		{
			finishedOutput.PrintStatus();
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnAfterSave();
			}
			Common.MemoryCleanupGC(false);
			if (onSaveCompleted != null)
			{
				onSaveCompleted(finishedOutput.Result);
			}
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x00017780 File Offset: 0x00015980
		public void Save(MetaData metaData, string saveName, ISaveDriver driver, Action<SaveResult> onSaveCompleted)
		{
			this.SaveAux(metaData, saveName, driver, onSaveCompleted);
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x0001778D File Offset: 0x0001598D
		private void InitializeParameters()
		{
			ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_core_parameters"));
			this.GameType.InitializeParameters();
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x000177B3 File Offset: 0x000159B3
		void IGameStateManagerOwner.OnStateStackEmpty()
		{
			this.Destroy();
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x000177BC File Offset: 0x000159BC
		public void Destroy()
		{
			this.CurrentState = Game.State.Destroying;
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnGameEnd();
			}
			this.GameManager.OnGameEnd(this);
			this.GameType.OnDestroy();
			this.ObjectManager.Destroy();
			this.EventManager.Clear();
			this.EventManager = null;
			GameStateManager.Current = null;
			this.GameStateManager = null;
			Game.Current = null;
			this.CurrentState = Game.State.Destroyed;
			this._currentActiveSaveData = null;
			Common.MemoryCleanupGC(false);
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x00017874 File Offset: 0x00015A74
		public void CreateGameManager()
		{
			this.GameStateManager = new GameStateManager(this, GameStateManager.GameStateManagerType.Game);
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x00017883 File Offset: 0x00015A83
		public void OnStateChanged(GameState oldState)
		{
			this.GameType.OnStateChanged(oldState);
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x00017891 File Offset: 0x00015A91
		public T AddGameHandler<T>() where T : GameHandler, new()
		{
			return this._gameEntitySystem.AddComponent<T>();
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x0001789E File Offset: 0x00015A9E
		public T GetGameHandler<T>() where T : GameHandler
		{
			return this._gameEntitySystem.GetComponent<T>();
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x000178AB File Offset: 0x00015AAB
		public void RemoveGameHandler<T>() where T : GameHandler
		{
			this._gameEntitySystem.RemoveComponent<T>();
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x000178B8 File Offset: 0x00015AB8
		public void Initialize()
		{
			if (this._gameEntitySystem == null)
			{
				this._gameEntitySystem = new EntitySystem<GameHandler>();
			}
			this.GameTextManager = new GameTextManager();
			this.GameTextManager.LoadGameTexts();
			this._gameModelManagers = new Dictionary<Type, GameModelsManager>();
			GameTexts.Initialize(this.GameTextManager);
			this.GameType.OnInitialize();
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x00017910 File Offset: 0x00015B10
		public static void RegisterTypes(GameType gameType, MBObjectManager objectManager)
		{
			if (gameType != null)
			{
				gameType.BeforeRegisterTypes(objectManager);
			}
			objectManager.RegisterType<Monster>("Monster", "Monsters", 2U, true, false);
			objectManager.RegisterType<SkeletonScale>("Scale", "Scales", 3U, true, false);
			objectManager.RegisterType<ItemObject>("Item", "Items", 4U, true, false);
			objectManager.RegisterType<ItemModifier>("ItemModifier", "ItemModifiers", 6U, true, false);
			objectManager.RegisterType<ItemModifierGroup>("ItemModifierGroup", "ItemModifierGroups", 7U, true, false);
			objectManager.RegisterType<CharacterAttribute>("CharacterAttribute", "CharacterAttributes", 8U, true, false);
			objectManager.RegisterType<SkillObject>("Skill", "Skills", 9U, true, false);
			objectManager.RegisterType<ItemCategory>("ItemCategory", "ItemCategories", 10U, true, false);
			objectManager.RegisterType<CraftingPiece>("CraftingPiece", "CraftingPieces", 11U, true, false);
			objectManager.RegisterType<CraftingTemplate>("CraftingTemplate", "CraftingTemplates", 12U, true, false);
			objectManager.RegisterType<SiegeEngineType>("SiegeEngineType", "SiegeEngineTypes", 13U, true, false);
			objectManager.RegisterType<WeaponDescription>("WeaponDescription", "WeaponDescriptions", 14U, true, false);
			objectManager.RegisterType<MBBodyProperty>("BodyProperty", "BodyProperties", 50U, true, false);
			objectManager.RegisterType<MBEquipmentRoster>("EquipmentRoster", "EquipmentRosters", 51U, true, false);
			objectManager.RegisterType<MBCharacterSkills>("SkillSet", "SkillSets", 52U, true, false);
			objectManager.RegisterType<BannerEffect>("BannerEffect", "BannerEffects", 53U, true, false);
			if (gameType != null)
			{
				gameType.OnRegisterTypes(objectManager);
			}
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x00017A6B File Offset: 0x00015C6B
		public void SetBasicModels(IEnumerable<GameModel> models)
		{
			this.BasicModels = this.AddGameModelsManager<BasicGameModels>(models);
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x00017A7C File Offset: 0x00015C7C
		internal void OnTick(float dt)
		{
			if (GameStateManager.Current == this.GameStateManager)
			{
				this.GameStateManager.OnTick(dt);
				if (this._gameEntitySystem != null)
				{
					foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
					{
						try
						{
							gameHandler.OnTick(dt);
						}
						catch (Exception ex)
						{
							Debug.Print("Exception on gameHandler tick: " + ex, 0, Debug.DebugColor.White, 17592186044416UL);
						}
					}
				}
			}
			Action<float> afterTick = this.AfterTick;
			if (afterTick != null)
			{
				afterTick(dt);
			}
			Tuple<SaveOutput, Action<SaveResult>> currentActiveSaveData = this._currentActiveSaveData;
			if (currentActiveSaveData != null && !currentActiveSaveData.Item1.IsContinuing)
			{
				this.OnSaveCompleted(this._currentActiveSaveData.Item1, this._currentActiveSaveData.Item2);
				this._currentActiveSaveData = null;
			}
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x00017B74 File Offset: 0x00015D74
		internal void OnGameNetworkBegin()
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnGameNetworkBegin();
			}
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x00017BCC File Offset: 0x00015DCC
		internal void OnGameNetworkEnd()
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnGameNetworkEnd();
			}
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x00017C24 File Offset: 0x00015E24
		internal void OnEarlyPlayerConnect(VirtualPlayer peer)
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnEarlyPlayerConnect(peer);
			}
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x00017C7C File Offset: 0x00015E7C
		internal void OnPlayerConnect(VirtualPlayer peer)
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnPlayerConnect(peer);
			}
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x00017CD4 File Offset: 0x00015ED4
		internal void OnPlayerDisconnect(VirtualPlayer peer)
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnPlayerDisconnect(peer);
			}
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x00017D2C File Offset: 0x00015F2C
		public void OnGameStart()
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnGameStart();
			}
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x00017D84 File Offset: 0x00015F84
		public bool DoLoading()
		{
			return this.GameType.DoLoadingForGameType();
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x00017D91 File Offset: 0x00015F91
		public void OnMissionIsStarting(string missionName, MissionInitializerRecord rec)
		{
			this.GameType.OnMissionIsStarting(missionName, rec);
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x00017DA0 File Offset: 0x00015FA0
		public void OnFinalize()
		{
			this.CurrentState = Game.State.Destroying;
			GameStateManager.Current.CleanStates(0);
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x00017DB4 File Offset: 0x00015FB4
		public void InitializeDefaultGameObjects()
		{
			this.DefaultCharacterAttributes = new DefaultCharacterAttributes();
			this.DefaultSkills = new DefaultSkills();
			this.DefaultBannerEffects = new DefaultBannerEffects();
			this.DefaultItemCategories = new DefaultItemCategories();
			this.DefaultSiegeEngineTypes = new DefaultSiegeEngineTypes();
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x00017DF0 File Offset: 0x00015FF0
		public void LoadBasicFiles()
		{
			this.ObjectManager.LoadXML("Monsters", false);
			this.ObjectManager.LoadXML("SkeletonScales", false);
			this.ObjectManager.LoadXML("ItemModifiers", false);
			this.ObjectManager.LoadXML("ItemModifierGroups", false);
			this.ObjectManager.LoadXML("CraftingPieces", false);
			this.ObjectManager.LoadXML("WeaponDescriptions", false);
			this.ObjectManager.LoadXML("CraftingTemplates", false);
			this.ObjectManager.LoadXML("BodyProperties", false);
			this.ObjectManager.LoadXML("SkillSets", false);
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x00017E96 File Offset: 0x00016096
		public void ItemObjectDeserialized(ItemObject itemObject)
		{
			Action<ItemObject> onItemDeserializedEvent = this.OnItemDeserializedEvent;
			if (onItemDeserializedEvent == null)
			{
				return;
			}
			onItemDeserializedEvent(itemObject);
		}

		// Token: 0x0400037B RID: 891
		public Action<float> AfterTick;

		// Token: 0x0400037E RID: 894
		private EntitySystem<GameHandler> _gameEntitySystem;

		// Token: 0x0400037F RID: 895
		private Monster _defaultMonster;

		// Token: 0x04000386 RID: 902
		private Dictionary<Type, GameModelsManager> _gameModelManagers;

		// Token: 0x0400038A RID: 906
		private static Game _current;

		// Token: 0x0400038C RID: 908
		[SaveableField(11)]
		private int _nextUniqueTroopSeed = 1;

		// Token: 0x04000392 RID: 914
		private IReadOnlyDictionary<string, Equipment> _defaultEquipments;

		// Token: 0x04000393 RID: 915
		private Tuple<SaveOutput, Action<SaveResult>> _currentActiveSaveData;

		// Token: 0x020000F7 RID: 247
		public enum State
		{
			// Token: 0x040006B6 RID: 1718
			Running,
			// Token: 0x040006B7 RID: 1719
			Destroying,
			// Token: 0x040006B8 RID: 1720
			Destroyed
		}
	}
}
