using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Information.RundownTooltip;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Options;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class GauntletUISubModule : MBSubModuleBase
	{
		public static GauntletUISubModule Instance { get; private set; }

		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			ResourceDepot resourceDepot = new ResourceDepot();
			resourceDepot.AddLocation(BasePath.Name, "GUI/GauntletUI/");
			List<string> list = new List<string>();
			string[] modulesNames = Utilities.GetModulesNames();
			for (int i = 0; i < modulesNames.Length; i++)
			{
				ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo(modulesNames[i]);
				if (moduleInfo != null)
				{
					string folderPath = moduleInfo.FolderPath;
					if (Directory.Exists(folderPath + "/GUI/"))
					{
						resourceDepot.AddLocation(folderPath, "/GUI/");
					}
					foreach (SubModuleInfo subModuleInfo in moduleInfo.SubModules)
					{
						if (subModuleInfo != null && subModuleInfo.DLLExists && !string.IsNullOrEmpty(subModuleInfo.DLLName))
						{
							list.Add(subModuleInfo.DLLName);
						}
					}
				}
			}
			resourceDepot.CollectResources();
			CustomWidgetManager.Initilize();
			BannerlordCustomWidgetManager.Initialize();
			UIResourceManager.Initialize(resourceDepot, list);
			UIResourceManager.WidgetFactory.GeneratedPrefabContext.CollectPrefabs();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			this._fullBackgroundCategory = spriteData.SpriteCategories["ui_fullbackgrounds"];
			this._fullBackgroundCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
			this._backgroundCategory = spriteData.SpriteCategories["ui_backgrounds"];
			this._backgroundCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
			this._fullscreensCategory = spriteData.SpriteCategories["ui_fullscreens"];
			this._fullscreensCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
			SpriteCategory[] array = spriteData.SpriteCategories.Values.Where((SpriteCategory x) => x.AlwaysLoad).ToArray<SpriteCategory>();
			int num = array.Length;
			float num2 = 0.2f / (float)(num - 1);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].Load(resourceContext, resourceDepot);
				Utilities.SetLoadingScreenPercentage(0.4f + (float)j * num2);
			}
			Utilities.SetLoadingScreenPercentage(0.6f);
			ScreenManager.OnControllerDisconnected += new ScreenManager.OnControllerDisconnectedEvent(this.OnControllerDisconnected);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			Input.OnControllerTypeChanged = (Action<Input.ControllerTypes>)Delegate.Combine(Input.OnControllerTypeChanged, new Action<Input.ControllerTypes>(this.OnControllerTypeChanged));
			GauntletGamepadNavigationManager.Initialize(NativeOptions.GetConfig(20) == 0f);
			GauntletUISubModule.Instance = this;
		}

		private void OnControllerTypeChanged(Input.ControllerTypes newType)
		{
			bool isTouchpadMouseActive = this._isTouchpadMouseActive;
			if (newType == 4 || newType == 2)
			{
				this._isTouchpadMouseActive = NativeOptions.GetConfig(18) != 0f;
			}
			if (isTouchpadMouseActive != this._isTouchpadMouseActive && !(ScreenManager.TopScreen is GauntletInitialScreen))
			{
				object obj = new TextObject("{=qkPfC3Cb}Warning", null);
				TextObject textObject = new TextObject("{=LDRV5PxX}Touchpad Mouse setting won't take affect correctly until returning to initial menu! Exiting to main menu is recommended!", null);
				InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, null, null, "", 0f, null, null, null), false, true);
			}
		}

		private void OnControllerDisconnected()
		{
		}

		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == null)
			{
				UIResourceManager.OnLanguageChange(BannerlordConfig.Language);
				ScreenManager.UpdateLayout();
				return;
			}
			if (changedManagedOptionsType == 29)
			{
				ScreenManager.OnScaleChange(BannerlordConfig.UIScale);
			}
		}

		protected override void OnSubModuleUnloaded()
		{
			ScreenManager.OnControllerDisconnected -= new ScreenManager.OnControllerDisconnectedEvent(this.OnControllerDisconnected);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			Input.OnControllerTypeChanged = (Action<Input.ControllerTypes>)Delegate.Remove(Input.OnControllerTypeChanged, new Action<Input.ControllerTypes>(this.OnControllerTypeChanged));
			UIResourceManager.Clear();
			LoadingWindow.Destroy();
			if (GauntletGamepadNavigationManager.Instance != null)
			{
				GauntletGamepadNavigationManager.Instance.OnFinalize();
			}
			GauntletUISubModule.Instance = null;
			base.OnSubModuleUnloaded();
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			if (!this._initialized)
			{
				if (!Utilities.CommandLineArgumentExists("VisualTests"))
				{
					GauntletInformationView.Initialize();
					GauntletGameNotification.Initialize();
					GauntletSceneNotification.Initialize();
					GauntletSceneNotification.Current.RegisterContextProvider(new NativeSceneNotificationContextProvider());
					GauntletChatLogView.Initialize();
					GauntletGamepadCursor.Initialize();
					InformationManager.RegisterTooltip<List<TooltipProperty>, PropertyBasedTooltipVM>(new Action<PropertyBasedTooltipVM, object[]>(PropertyBasedTooltipVM.RefreshGenericPropertyBasedTooltip), "PropertyBasedTooltip");
					InformationManager.RegisterTooltip<RundownLineVM, RundownTooltipVM>(new Action<RundownTooltipVM, object[]>(RundownTooltipVM.RefreshGenericRundownTooltip), "RundownTooltip");
					InformationManager.RegisterTooltip<string, HintVM>(new Action<HintVM, object[]>(HintVM.RefreshGenericHintTooltip), "HintTooltip");
					this._queryManager = new GauntletQueryManager();
					this._queryManager.Initialize();
					this._queryManager.InitializeKeyVisuals();
				}
				this._loadingWindowManager = new LoadingWindowManager();
				LoadingWindow.Initialize(this._loadingWindowManager);
				UIResourceManager.OnLanguageChange(BannerlordConfig.Language);
				ScreenManager.OnScaleChange(BannerlordConfig.UIScale);
				this._initialized = true;
			}
		}

		public override void OnMultiplayerGameStart(Game game, object starterObject)
		{
			base.OnMultiplayerGameStart(game, starterObject);
			if (!this._isMultiplayer)
			{
				this._loadingWindowManager.SetCurrentModeIsMultiplayer(true);
				this._isMultiplayer = true;
			}
		}

		public override void OnGameEnd(Game game)
		{
			base.OnGameEnd(game);
			if (this._isMultiplayer)
			{
				this._loadingWindowManager.SetCurrentModeIsMultiplayer(false);
				this._isMultiplayer = false;
			}
		}

		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			UIResourceManager.Update();
			if (GauntletGamepadNavigationManager.Instance != null && ScreenManager.GetMouseVisibility())
			{
				GauntletGamepadNavigationManager.Instance.Update(dt);
			}
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("clear", "chatlog")]
		public static string ClearChatLog(List<string> strings)
		{
			InformationManager.ClearAllMessages();
			return "Chatlog cleared!";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("can_focus_while_in_mission", "chatlog")]
		public static string SetCanFocusWhileInMission(List<string> strings)
		{
			if (strings[0] == "0" || strings[0] == "1")
			{
				GauntletChatLogView.Current.SetCanFocusWhileInMission(strings[0] == "1");
				return "Chat window will" + ((strings[0] == "1") ? " " : " NOT ") + " be able to gain focus now.";
			}
			return "Wrong input";
		}

		private bool _initialized;

		private bool _isMultiplayer;

		private GauntletQueryManager _queryManager;

		private LoadingWindowManager _loadingWindowManager;

		private SpriteCategory _fullBackgroundCategory;

		private SpriteCategory _backgroundCategory;

		private SpriteCategory _fullscreensCategory;

		private bool _isTouchpadMouseActive;
	}
}
