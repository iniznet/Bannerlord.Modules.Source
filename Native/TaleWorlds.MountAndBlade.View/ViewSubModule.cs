using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.InputSystem;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.GameKeyCategory;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.View.Tableaus;
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

		private void InitializeHotKeyManager(bool loadKeys)
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
			}, loadKeys);
		}

		private void InitializeBannerVisualManager()
		{
			if (BannerManager.Instance == null)
			{
				BannerManager.Initialize();
				BannerManager.Instance.LoadBannerIcons(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/banner_icons.xml");
				string[] modulesNames = Utilities.GetModulesNames();
				for (int i = 0; i < modulesNames.Length; i++)
				{
					ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo(modulesNames[i]);
					if (moduleInfo != null && !moduleInfo.IsNative)
					{
						string text = moduleInfo.FolderPath + "/ModuleData/banner_icons.xml";
						if (File.Exists(text))
						{
							BannerManager.Instance.LoadBannerIcons(text);
						}
					}
				}
			}
		}

		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			ViewSubModule._instance = this;
			this.InitializeHotKeyManager(false);
			this.InitializeBannerVisualManager();
			CraftedDataViewManager.Initialize();
			ScreenManager.OnPushScreen += new ScreenManager.OnPushScreenEvent(this.OnScreenManagerPushScreen);
			this._gameStateScreenManager = new GameStateScreenManager();
			Module.CurrentModule.GlobalGameStateManager.RegisterListener(this._gameStateScreenManager);
			MBMusicManager.Create();
			TextObject coreContentDisabledReason = new TextObject("{=V8BXjyYq}Disabled during installation.", null);
			if (Utilities.EditModeEnabled)
			{
				Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Editor", new TextObject("{=bUh0x6rA}Editor", null), -1, delegate
				{
					MBInitialScreenBase.OnEditModeEnterPress();
				}, () => new ValueTuple<bool, TextObject>(Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason), null));
			}
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Options", new TextObject("{=NqarFr4P}Options", null), 9998, delegate
			{
				ScreenManager.PushScreen(ViewCreator.CreateOptionsScreen(true));
			}, () => new ValueTuple<bool, TextObject>(false, TextObject.Empty), null));
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Credits", new TextObject("{=ODQmOrIw}Credits", null), 9999, delegate
			{
				ScreenManager.PushScreen(ViewCreator.CreateCreditsScreen());
			}, () => new ValueTuple<bool, TextObject>(false, TextObject.Empty), null));
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Exit", new TextObject("{=YbpzLHzk}Exit Game", null), 10000, delegate
			{
				MBInitialScreenBase.DoExitButtonAction();
			}, () => new ValueTuple<bool, TextObject>(Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason), null));
			Module.CurrentModule.ImguiProfilerTick += this.OnImguiProfilerTick;
			Input.OnControllerTypeChanged = (Action<Input.ControllerTypes>)Delegate.Combine(Input.OnControllerTypeChanged, new Action<Input.ControllerTypes>(this.OnControllerTypeChanged));
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Combine(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
			ViewModel.CollectPropertiesAndMethods();
			HyperlinkTexts.IsPlayStationGamepadActive = new Func<bool>(this.GetIsPlaystationGamepadActive);
			EngineController.OnConstrainedStateChanged += this.OnConstrainedStateChange;
		}

		private void OnConstrainedStateChange(bool isConstrained)
		{
			ScreenManager.OnConstrainStateChanged(isConstrained);
		}

		private bool GetIsPlaystationGamepadActive()
		{
			return Input.ControllerType == 4 || Input.ControllerType == 2;
		}

		private void OnControllerTypeChanged(Input.ControllerTypes newType)
		{
			this.ReInitializeHotKeyManager();
		}

		private void OnNativeOptionChanged(NativeOptions.NativeOptionsType changedNativeOptionsType)
		{
			if (changedNativeOptionsType == 18)
			{
				this.ReInitializeHotKeyManager();
			}
		}

		private void ReInitializeHotKeyManager()
		{
			this.InitializeHotKeyManager(true);
		}

		protected override void OnSubModuleUnloaded()
		{
			ScreenManager.OnPushScreen -= new ScreenManager.OnPushScreenEvent(this.OnScreenManagerPushScreen);
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Remove(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
			TableauCacheManager.ClearManager();
			BannerlordTableauManager.ClearManager();
			CraftedDataViewManager.Clear();
			Module.CurrentModule.ImguiProfilerTick -= this.OnImguiProfilerTick;
			Input.OnControllerTypeChanged = (Action<Input.ControllerTypes>)Delegate.Remove(Input.OnControllerTypeChanged, new Action<Input.ControllerTypes>(this.OnControllerTypeChanged));
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

		private void OnScreenManagerPushScreen(ScreenBase pushedScreen)
		{
		}

		private Dictionary<Tuple<Material, BannerCode>, Material> _bannerTexturedMaterialCache;

		private GameStateScreenManager _gameStateScreenManager;

		private bool _newGameInitialization;

		private static ViewSubModule _instance;

		private bool _initialized;
	}
}
