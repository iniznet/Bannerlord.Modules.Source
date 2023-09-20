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
	// Token: 0x0200001D RID: 29
	public class ViewSubModule : MBSubModuleBase
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00007755 File Offset: 0x00005955
		// (set) Token: 0x060000E0 RID: 224 RVA: 0x00007761 File Offset: 0x00005961
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

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x0000776E File Offset: 0x0000596E
		public static GameStateScreenManager GameStateScreenManager
		{
			get
			{
				return ViewSubModule._instance._gameStateScreenManager;
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x0000777C File Offset: 0x0000597C
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

		// Token: 0x060000E3 RID: 227 RVA: 0x00007898 File Offset: 0x00005A98
		private void InitializeBannerVisualManager()
		{
			if (BannerManager.Instance == null)
			{
				BannerManager.Initialize();
				BannerManager.Instance.LoadBannerIcons(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/banner_icons.xml");
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000078C4 File Offset: 0x00005AC4
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

		// Token: 0x060000E5 RID: 229 RVA: 0x00007B3B File Offset: 0x00005D3B
		private void OnConstrainedStateChange(bool isConstrained)
		{
			ScreenManager.OnConstrainStateChanged(isConstrained);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00007B44 File Offset: 0x00005D44
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

		// Token: 0x060000E7 RID: 231 RVA: 0x00007B80 File Offset: 0x00005D80
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

		// Token: 0x060000E8 RID: 232 RVA: 0x00007C00 File Offset: 0x00005E00
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

		// Token: 0x060000E9 RID: 233 RVA: 0x00007C20 File Offset: 0x00005E20
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

		// Token: 0x060000EA RID: 234 RVA: 0x00007C76 File Offset: 0x00005E76
		protected override void AfterAsyncTickTick(float dt)
		{
			MBMusicManager mbmusicManager = MBMusicManager.Current;
			if (mbmusicManager == null)
			{
				return;
			}
			mbmusicManager.Update(dt);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00007C88 File Offset: 0x00005E88
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			MissionWeapon.OnGetWeaponDataHandler = new MissionWeapon.OnGetWeaponDataDelegate(ItemCollectionElementViewExtensions.OnGetWeaponData);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00007C9B File Offset: 0x00005E9B
		public override void OnCampaignStart(Game game, object starterObject)
		{
			Game.Current.GameStateManager.RegisterListener(this._gameStateScreenManager);
			this._newGameInitialization = false;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00007CBA File Offset: 0x00005EBA
		public override void OnMultiplayerGameStart(Game game, object starterObject)
		{
			Game.Current.GameStateManager.RegisterListener(this._gameStateScreenManager);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007CD2 File Offset: 0x00005ED2
		public override void OnGameLoaded(Game game, object initializerObject)
		{
			Game.Current.GameStateManager.RegisterListener(this._gameStateScreenManager);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00007CEC File Offset: 0x00005EEC
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

		// Token: 0x060000F0 RID: 240 RVA: 0x00007E08 File Offset: 0x00006008
		public override void BeginGameStart(Game game)
		{
			base.BeginGameStart(game);
			Game.Current.BannerVisualCreator = new BannerVisualCreator();
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00007E20 File Offset: 0x00006020
		public override bool DoLoading(Game game)
		{
			if (this._newGameInitialization)
			{
				return true;
			}
			this._newGameInitialization = true;
			return this._newGameInitialization;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00007E39 File Offset: 0x00006039
		public override void OnGameEnd(Game game)
		{
			MissionWeapon.OnGetWeaponDataHandler = null;
			CraftedDataViewManager.Clear();
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00007E46 File Offset: 0x00006046
		private void OnImguiProfilerTick()
		{
			TableauCacheManager.Current.OnImguiProfilerTick();
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00007E52 File Offset: 0x00006052
		private void OnGamepadActiveStateChanged()
		{
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			game.EventManager.TriggerEvent<GamepadActiveStateChangedEvent>(new GamepadActiveStateChangedEvent());
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007E6D File Offset: 0x0000606D
		private void OnScreenManagerPushScreen(ScreenBase pushedScreen)
		{
		}

		// Token: 0x04000046 RID: 70
		private Dictionary<Tuple<Material, BannerCode>, Material> _bannerTexturedMaterialCache;

		// Token: 0x04000047 RID: 71
		private GameStateScreenManager _gameStateScreenManager;

		// Token: 0x04000048 RID: 72
		private bool _newGameInitialization;

		// Token: 0x04000049 RID: 73
		private static ViewSubModule _instance;

		// Token: 0x0400004A RID: 74
		private bool _initialized;

		// Token: 0x0400004B RID: 75
		private bool _isConnectingToMultiplayer;
	}
}
