using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.InputSystem;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.GameKeyCategory;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.PlatformService;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View
{
	public class ViewSubModule : MBSubModuleBase
	{
		public static Dictionary<Tuple<Material, BannerCode>, Material> BannerTexturedMaterialCache
		{
			get
			{
				return ViewSubModule._instance._bannerTexturedMaterialCache;
			}
			set
			{
				ViewSubModule._instance._bannerTexturedMaterialCache = value;
			}
		}

		public static GameStateScreenManager GameStateScreenManager
		{
			get
			{
				return ViewSubModule._instance._gameStateScreenManager;
			}
		}

		private void InitializeHotKeyManager()
		{
			string text = "BannerlordGameKeys.xml";
			HotKeyManager.Initialize(new PlatformFilePath(EngineFilePaths.ConfigsPath, text), !ScreenManager.IsEnterButtonRDown);
			HotKeyManager.RegisterInitialContexts(new List<GameKeyContext>
			{
				new GenericGameKeyContext(),
				new GenericCampaignPanelsGameKeyCategory("GenericCampaignPanelsGameKeyCategory"),
				new GenericPanelGameKeyCategory("GenericPanelGameKeyCategory"),
				new ArmyManagementHotkeyCategory(),
				new BoardGameHotkeyCategory(),
				new ChatLogHotKeyCategory(),
				new CombatHotKeyCategory(),
				new CraftingHotkeyCategory(),
				new FaceGenHotkeyCategory(),
				new InventoryHotKeyCategory(),
				new PartyHotKeyCategory(),
				new MapHotKeyCategory(),
				new MapNotificationHotKeyCategory(),
				new MissionOrderHotkeyCategory(),
				new MultiplayerHotkeyCategory(),
				new ScoreboardHotKeyCategory(),
				new ConversationHotKeyCategory(),
				new CheatsHotKeyCategory(),
				new PhotoModeHotKeyCategory(),
				new PollHotkeyCategory()
			}, false);
		}

		private void InitializeBannerVisualManager()
		{
			if (BannerManager.Instance == null)
			{
				BannerManager.Initialize();
				BannerManager.Instance.LoadBannerIcons(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/banner_icons.xml");
			}
		}

		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			ViewSubModule._instance = this;
			this.InitializeHotKeyManager();
			this.InitializeBannerVisualManager();
			CraftedDataViewManager.Initialize();
			ScreenManager.OnPushScreen += new ScreenManager.OnPushScreenEvent(this.OnScreenManagerPushScreen);
			this._gameStateScreenManager = new GameStateScreenManager();
			Module.CurrentModule.GlobalGameStateManager.RegisterListener(this._gameStateScreenManager);
			MBMusicManager.Create();
			TextObject coreContentDisabledReason = new TextObject("{=V8BXjyYq}Disabled during installation.", null);
			if (Module.CurrentModule.StartupInfo.StartupType != 3)
			{
				Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Multiplayer", new TextObject("{=YDYnuBmC}Multiplayer", null), 9997, new Action(this.StartMultiplayer), () => new ValueTuple<bool, TextObject>(Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)));
			}
			if (Utilities.EditModeEnabled)
			{
				Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Editor", new TextObject("{=bUh0x6rA}Editor", null), -1, delegate
				{
					MBInitialScreenBase.OnEditModeEnterPress();
				}, () => new ValueTuple<bool, TextObject>(Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)));
			}
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Options", new TextObject("{=NqarFr4P}Options", null), 9998, delegate
			{
				ScreenManager.PushScreen(ViewCreator.CreateOptionsScreen(true));
			}, () => new ValueTuple<bool, TextObject>(false, TextObject.Empty)));
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Credits", new TextObject("{=ODQmOrIw}Credits", null), 9999, delegate
			{
				ScreenManager.PushScreen(ViewCreator.CreateCreditsScreen());
			}, () => new ValueTuple<bool, TextObject>(false, TextObject.Empty)));
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Exit", new TextObject("{=YbpzLHzk}Exit Game", null), 10000, delegate
			{
				MBInitialScreenBase.DoExitButtonAction();
			}, () => new ValueTuple<bool, TextObject>(Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)));
			Module.CurrentModule.ImguiProfilerTick += this.OnImguiProfilerTick;
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			ViewModel.CollectPropertiesAndMethods();
			EngineController.OnConstrainedStateChanged += this.OnConstrainedStateChange;
		}

		private void OnConstrainedStateChange(bool isConstrained)
		{
			ScreenManager.OnConstrainStateChanged(isConstrained);
		}

		private async void StartMultiplayer()
		{
			if (!this._isConnectingToMultiplayer)
			{
				this._isConnectingToMultiplayer = true;
				bool flag = NetworkMain.GameClient != null && await NetworkMain.GameClient.CheckConnection();
				bool isConnected = flag;
				PlatformServices.Instance.CheckPrivilege(0, true, delegate(bool result)
				{
					if (!isConnected || !result)
					{
						string text = new TextObject("{=ksq1IBh3}No connection", null).ToString();
						string text2 = new TextObject("{=5VIbo2Cb}No connection could be established to the lobby server. Check your internet connection and try again.", null).ToString();
						InformationManager.ShowInquiry(new InquiryData(text, text2, false, true, "", new TextObject("{=dismissnotification}Dismiss", null).ToString(), null, delegate
						{
							InformationManager.HideInquiry();
						}, "", 0f, null, null, null), false, false);
						return;
					}
					MBGameManager.StartNewGame(new MultiplayerGameManager());
				});
				this._isConnectingToMultiplayer = false;
			}
		}

		protected override void OnSubModuleUnloaded()
		{
			ScreenManager.OnPushScreen -= new ScreenManager.OnPushScreenEvent(this.OnScreenManagerPushScreen);
			TableauCacheManager.ClearManager();
			BannerlordTableauManager.ClearManager();
			CraftedDataViewManager.Clear();
			Module.CurrentModule.ImguiProfilerTick -= this.OnImguiProfilerTick;
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			ViewSubModule._instance = null;
			EngineController.OnConstrainedStateChanged -= this.OnConstrainedStateChange;
			base.OnSubModuleUnloaded();
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			if (!this._initialized)
			{
				HotKeyManager.Load();
				BannerlordTableauManager.InitializeCharacterTableauRenderSystem();
				TableauCacheManager.InitializeManager();
				this._initialized = true;
			}
		}

		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			if (Input.DebugInput.IsHotKeyPressed("ToggleUI"))
			{
				MBDebug.DisableUI(new List<string>());
			}
			HotKeyManager.Tick(dt);
			MBMusicManager mbmusicManager = MBMusicManager.Current;
			if (mbmusicManager != null)
			{
				mbmusicManager.Update(dt);
			}
			TableauCacheManager tableauCacheManager = TableauCacheManager.Current;
			if (tableauCacheManager == null)
			{
				return;
			}
			tableauCacheManager.Tick();
		}

		protected override void AfterAsyncTickTick(float dt)
		{
			MBMusicManager mbmusicManager = MBMusicManager.Current;
			if (mbmusicManager == null)
			{
				return;
			}
			mbmusicManager.Update(dt);
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			MissionWeapon.OnGetWeaponDataHandler = new MissionWeapon.OnGetWeaponDataDelegate(ItemCollectionElementViewExtensions.OnGetWeaponData);
		}

		public override void OnCampaignStart(Game game, object starterObject)
		{
			Game.Current.GameStateManager.RegisterListener(this._gameStateScreenManager);
			this._newGameInitialization = false;
		}

		public override void OnMultiplayerGameStart(Game game, object starterObject)
		{
			Game.Current.GameStateManager.RegisterListener(this._gameStateScreenManager);
		}

		public override void OnGameLoaded(Game game, object initializerObject)
		{
			Game.Current.GameStateManager.RegisterListener(this._gameStateScreenManager);
		}

		public override void OnGameInitializationFinished(Game game)
		{
			base.OnGameInitializationFinished(game);
			foreach (ItemObject itemObject in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
			{
				if (itemObject.MultiMeshName != "")
				{
					MBUnusedResourceManager.SetMeshUsed(itemObject.MultiMeshName);
				}
				HorseComponent horseComponent = itemObject.HorseComponent;
				if (horseComponent != null)
				{
					foreach (KeyValuePair<string, bool> keyValuePair in horseComponent.AdditionalMeshesNameList)
					{
						MBUnusedResourceManager.SetMeshUsed(keyValuePair.Key);
					}
				}
				if (itemObject.PrimaryWeapon != null)
				{
					MBUnusedResourceManager.SetMeshUsed(itemObject.HolsterMeshName);
					MBUnusedResourceManager.SetMeshUsed(itemObject.HolsterWithWeaponMeshName);
					MBUnusedResourceManager.SetMeshUsed(itemObject.FlyingMeshName);
					MBUnusedResourceManager.SetBodyUsed(itemObject.BodyName);
					MBUnusedResourceManager.SetBodyUsed(itemObject.HolsterBodyName);
					MBUnusedResourceManager.SetBodyUsed(itemObject.CollisionBodyName);
				}
			}
		}

		public override void BeginGameStart(Game game)
		{
			base.BeginGameStart(game);
			Game.Current.BannerVisualCreator = new BannerVisualCreator();
		}

		public override bool DoLoading(Game game)
		{
			if (this._newGameInitialization)
			{
				return true;
			}
			this._newGameInitialization = true;
			return this._newGameInitialization;
		}

		public override void OnGameEnd(Game game)
		{
			MissionWeapon.OnGetWeaponDataHandler = null;
			CraftedDataViewManager.Clear();
		}

		private void OnImguiProfilerTick()
		{
			TableauCacheManager.Current.OnImguiProfilerTick();
		}

		private void OnGamepadActiveStateChanged()
		{
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			game.EventManager.TriggerEvent<GamepadActiveStateChangedEvent>(new GamepadActiveStateChangedEvent());
		}

		private void OnScreenManagerPushScreen(ScreenBase pushedScreen)
		{
		}

		private Dictionary<Tuple<Material, BannerCode>, Material> _bannerTexturedMaterialCache;

		private GameStateScreenManager _gameStateScreenManager;

		private bool _newGameInitialization;

		private static ViewSubModule _instance;

		private bool _initialized;

		private bool _isConnectingToMultiplayer;
	}
}
