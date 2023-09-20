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
	[SaveableRootClass(5000)]
	public sealed class Game : IGameStateManagerOwner
	{
		public static event Action OnGameCreated;

		public event Action<ItemObject> OnItemDeserializedEvent;

		public Game.State CurrentState { get; private set; }

		public IMonsterMissionDataCreator MonsterMissionDataCreator { get; set; }

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

		[SaveableProperty(3)]
		public GameType GameType { get; private set; }

		public DefaultSiegeEngineTypes DefaultSiegeEngineTypes { get; private set; }

		public MBObjectManager ObjectManager { get; private set; }

		[SaveableProperty(8)]
		public BasicCharacterObject PlayerTroop { get; set; }

		[SaveableProperty(12)]
		internal MBFastRandom RandomGenerator { get; private set; }

		public BasicGameModels BasicModels { get; private set; }

		public T AddGameModelsManager<T>(IEnumerable<GameModel> inputComponents) where T : GameModelsManager
		{
			T t = (T)((object)Activator.CreateInstance(typeof(T), new object[] { inputComponents }));
			this._gameModelManagers.Add(typeof(T), t);
			return t;
		}

		public GameManagerBase GameManager { get; private set; }

		public GameTextManager GameTextManager { get; private set; }

		public GameStateManager GameStateManager { get; private set; }

		public bool CheatMode
		{
			get
			{
				return this.GameManager.CheatMode;
			}
		}

		public bool IsDevelopmentMode
		{
			get
			{
				return this.GameManager.IsDevelopmentMode;
			}
		}

		public bool IsEditModeOn
		{
			get
			{
				return this.GameManager.IsEditModeOn;
			}
		}

		public UnitSpawnPrioritizations UnitSpawnPrioritization
		{
			get
			{
				return this.GameManager.UnitSpawnPrioritization;
			}
		}

		public float ApplicationTime
		{
			get
			{
				return this.GameManager.ApplicationTime;
			}
		}

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

		public IBannerVisualCreator BannerVisualCreator { get; set; }

		public IBannerVisual CreateBannerVisual(Banner banner)
		{
			if (this.BannerVisualCreator == null)
			{
				return null;
			}
			return this.BannerVisualCreator.CreateBannerVisual(banner);
		}

		public int NextUniqueTroopSeed
		{
			get
			{
				int nextUniqueTroopSeed = this._nextUniqueTroopSeed;
				this._nextUniqueTroopSeed = nextUniqueTroopSeed + 1;
				return nextUniqueTroopSeed;
			}
		}

		public DefaultCharacterAttributes DefaultCharacterAttributes { get; private set; }

		public DefaultSkills DefaultSkills { get; private set; }

		public DefaultBannerEffects DefaultBannerEffects { get; private set; }

		public DefaultItemCategories DefaultItemCategories { get; private set; }

		public EventManager EventManager { get; private set; }

		public Equipment GetDefaultEquipmentWithName(string equipmentName)
		{
			if (!this._defaultEquipments.ContainsKey(equipmentName))
			{
				Debug.FailedAssert("Equipment with name \"" + equipmentName + "\" could not be found.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Game.cs", "GetDefaultEquipmentWithName", 130);
				return null;
			}
			return this._defaultEquipments[equipmentName].Clone(false);
		}

		public void SetDefaultEquipments(IReadOnlyDictionary<string, Equipment> defaultEquipments)
		{
			if (this._defaultEquipments == null)
			{
				this._defaultEquipments = defaultEquipments;
			}
		}

		public static Game CreateGame(GameType gameType, GameManagerBase gameManager, int seed)
		{
			Game game = Game.CreateGame(gameType, gameManager);
			game.RandomGenerator = new MBFastRandom((uint)seed);
			return game;
		}

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

		public static Game CreateGame(GameType gameType, GameManagerBase gameManager)
		{
			MBObjectManager mbobjectManager = MBObjectManager.Init();
			Game.RegisterTypes(gameType, mbobjectManager);
			return new Game(gameType, gameManager, mbobjectManager);
		}

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

		[LoadInitializationCallback]
		private void OnLoad()
		{
			if (this.RandomGenerator == null)
			{
				this.RandomGenerator = new MBFastRandom();
			}
		}

		private void BeginLoading(GameManagerBase gameManager)
		{
			Game.Current = this;
			this.GameType.CurrentGame = this;
			this.GameManager = gameManager;
			this.GameManager.Game = this;
			this.EventManager = new EventManager();
			this.InitializeParameters();
		}

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

		public void Save(MetaData metaData, string saveName, ISaveDriver driver, Action<SaveResult> onSaveCompleted)
		{
			this.SaveAux(metaData, saveName, driver, onSaveCompleted);
		}

		private void InitializeParameters()
		{
			ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_core_parameters"));
			this.GameType.InitializeParameters();
		}

		void IGameStateManagerOwner.OnStateStackEmpty()
		{
			this.Destroy();
		}

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

		public void CreateGameManager()
		{
			this.GameStateManager = new GameStateManager(this, GameStateManager.GameStateManagerType.Game);
		}

		public void OnStateChanged(GameState oldState)
		{
			this.GameType.OnStateChanged(oldState);
		}

		public T AddGameHandler<T>() where T : GameHandler, new()
		{
			return this._gameEntitySystem.AddComponent<T>();
		}

		public T GetGameHandler<T>() where T : GameHandler
		{
			return this._gameEntitySystem.GetComponent<T>();
		}

		public void RemoveGameHandler<T>() where T : GameHandler
		{
			this._gameEntitySystem.RemoveComponent<T>();
		}

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

		public void SetBasicModels(IEnumerable<GameModel> models)
		{
			this.BasicModels = this.AddGameModelsManager<BasicGameModels>(models);
		}

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

		internal void OnGameNetworkBegin()
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnGameNetworkBegin();
			}
		}

		internal void OnGameNetworkEnd()
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnGameNetworkEnd();
			}
		}

		internal void OnEarlyPlayerConnect(VirtualPlayer peer)
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnEarlyPlayerConnect(peer);
			}
		}

		internal void OnPlayerConnect(VirtualPlayer peer)
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnPlayerConnect(peer);
			}
		}

		internal void OnPlayerDisconnect(VirtualPlayer peer)
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnPlayerDisconnect(peer);
			}
		}

		public void OnGameStart()
		{
			foreach (GameHandler gameHandler in this._gameEntitySystem.Components)
			{
				gameHandler.OnGameStart();
			}
		}

		public bool DoLoading()
		{
			return this.GameType.DoLoadingForGameType();
		}

		public void OnMissionIsStarting(string missionName, MissionInitializerRecord rec)
		{
			this.GameType.OnMissionIsStarting(missionName, rec);
		}

		public void OnFinalize()
		{
			this.CurrentState = Game.State.Destroying;
			GameStateManager.Current.CleanStates(0);
		}

		public void InitializeDefaultGameObjects()
		{
			this.DefaultCharacterAttributes = new DefaultCharacterAttributes();
			this.DefaultSkills = new DefaultSkills();
			this.DefaultBannerEffects = new DefaultBannerEffects();
			this.DefaultItemCategories = new DefaultItemCategories();
			this.DefaultSiegeEngineTypes = new DefaultSiegeEngineTypes();
		}

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

		public void ItemObjectDeserialized(ItemObject itemObject)
		{
			Action<ItemObject> onItemDeserializedEvent = this.OnItemDeserializedEvent;
			if (onItemDeserializedEvent == null)
			{
				return;
			}
			onItemDeserializedEvent(itemObject);
		}

		public Action<float> AfterTick;

		private EntitySystem<GameHandler> _gameEntitySystem;

		private Monster _defaultMonster;

		private Dictionary<Type, GameModelsManager> _gameModelManagers;

		private static Game _current;

		[SaveableField(11)]
		private int _nextUniqueTroopSeed = 1;

		private IReadOnlyDictionary<string, Equipment> _defaultEquipments;

		private Tuple<SaveOutput, Action<SaveResult>> _currentActiveSaveData;

		public enum State
		{
			Running,
			Destroying,
			Destroyed
		}
	}
}
