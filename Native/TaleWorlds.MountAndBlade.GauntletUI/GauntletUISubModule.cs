using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Options;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x0200000F RID: 15
	public class GauntletUISubModule : MBSubModuleBase
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000045BA File Offset: 0x000027BA
		// (set) Token: 0x06000068 RID: 104 RVA: 0x000045C1 File Offset: 0x000027C1
		public static GauntletUISubModule Instance { get; private set; }

		// Token: 0x0600006A RID: 106 RVA: 0x000045D4 File Offset: 0x000027D4
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
			GauntletGamepadNavigationManager.Initialize(NativeOptions.GetConfig(16) == 0f);
			GauntletUISubModule.Instance = this;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000484C File Offset: 0x00002A4C
		private void OnControllerDisconnected()
		{
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000484E File Offset: 0x00002A4E
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == null)
			{
				UIResourceManager.OnLanguageChange(BannerlordConfig.Language);
				ScreenManager.UpdateLayout();
				return;
			}
			if (changedManagedOptionsType == 27)
			{
				ScreenManager.OnScaleChange(BannerlordConfig.UIScale);
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004874 File Offset: 0x00002A74
		protected override void OnSubModuleUnloaded()
		{
			ScreenManager.OnControllerDisconnected -= new ScreenManager.OnControllerDisconnectedEvent(this.OnControllerDisconnected);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			UIResourceManager.Clear();
			LoadingWindow.Destroy();
			if (GauntletGamepadNavigationManager.Instance != null)
			{
				GauntletGamepadNavigationManager.Instance.OnFinalize();
			}
			GauntletUISubModule.Instance = null;
			base.OnSubModuleUnloaded();
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000048DC File Offset: 0x00002ADC
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
					PropertyBasedTooltipVM.AddTooltipType(typeof(List<TooltipProperty>), new Action<PropertyBasedTooltipVM, object[]>(GauntletUISubModule.CustomTooltipAction));
					this._queryManager = new GauntletQueryManager();
					this._queryManager.Initialize();
				}
				this._loadingWindowManager = new LoadingWindowManager();
				LoadingWindow.Initialize(this._loadingWindowManager);
				UIResourceManager.OnLanguageChange(BannerlordConfig.Language);
				ScreenManager.OnScaleChange(BannerlordConfig.UIScale);
				this._initialized = true;
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000498A File Offset: 0x00002B8A
		public override void OnMultiplayerGameStart(Game game, object starterObject)
		{
			base.OnMultiplayerGameStart(game, starterObject);
			if (!this._isMultiplayer)
			{
				this._loadingWindowManager.SetCurrentModeIsMultiplayer(true);
				this._isMultiplayer = true;
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000049AF File Offset: 0x00002BAF
		public override void BeginGameStart(Game game)
		{
			base.BeginGameStart(game);
			if (!Utilities.CommandLineArgumentExists("VisualTests"))
			{
				this._queryManager.InitializeKeyVisuals();
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000049CF File Offset: 0x00002BCF
		public override void OnGameEnd(Game game)
		{
			base.OnGameEnd(game);
			if (this._isMultiplayer)
			{
				this._loadingWindowManager.SetCurrentModeIsMultiplayer(false);
				this._isMultiplayer = false;
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000049F3 File Offset: 0x00002BF3
		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			UIResourceManager.Update();
			if (GauntletGamepadNavigationManager.Instance != null && ScreenManager.GetMouseVisibility())
			{
				GauntletGamepadNavigationManager.Instance.Update(dt);
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004A1A File Offset: 0x00002C1A
		private static void CustomTooltipAction(PropertyBasedTooltipVM propertyBasedTooltip, object[] args)
		{
			propertyBasedTooltip.UpdateTooltip(args[0] as List<TooltipProperty>);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004A2A File Offset: 0x00002C2A
		[CommandLineFunctionality.CommandLineArgumentFunction("clear", "chatlog")]
		public static string ClearChatLog(List<string> strings)
		{
			InformationManager.ClearAllMessages();
			return "Chatlog cleared!";
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004A38 File Offset: 0x00002C38
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

		// Token: 0x04000044 RID: 68
		private bool _initialized;

		// Token: 0x04000045 RID: 69
		private bool _isMultiplayer;

		// Token: 0x04000046 RID: 70
		private GauntletQueryManager _queryManager;

		// Token: 0x04000047 RID: 71
		private LoadingWindowManager _loadingWindowManager;

		// Token: 0x04000048 RID: 72
		private SpriteCategory _fullBackgroundCategory;

		// Token: 0x04000049 RID: 73
		private SpriteCategory _backgroundCategory;

		// Token: 0x0400004A RID: 74
		private SpriteCategory _fullscreensCategory;
	}
}
