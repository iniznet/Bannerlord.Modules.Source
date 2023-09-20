using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.Engine.GauntletUI
{
	public static class UIResourceManager
	{
		public static ResourceDepot UIResourceDepot { get; private set; }

		public static WidgetFactory WidgetFactory { get; private set; }

		public static SpriteData SpriteData { get; private set; }

		public static BrushFactory BrushFactory { get; private set; }

		public static FontFactory FontFactory { get; private set; }

		public static TwoDimensionEngineResourceContext ResourceContext { get; private set; }

		private static bool _uiDebugMode
		{
			get
			{
				return UIConfig.DebugModeEnabled || NativeConfig.GetUIDebugMode;
			}
		}

		public static void Initialize(ResourceDepot resourceDepot, List<string> assemblyOrder)
		{
			UIResourceManager.UIResourceDepot = resourceDepot;
			UIResourceManager.WidgetFactory = new WidgetFactory(UIResourceManager.UIResourceDepot, "Prefabs");
			UIResourceManager.WidgetFactory.PrefabExtensionContext.AddExtension(new PrefabDatabindingExtension());
			UIResourceManager.WidgetFactory.Initialize(assemblyOrder);
			UIResourceManager.SpriteData = new SpriteData("SpriteData");
			UIResourceManager.SpriteData.Load(UIResourceManager.UIResourceDepot);
			UIResourceManager.FontFactory = new FontFactory(UIResourceManager.UIResourceDepot);
			UIResourceManager.FontFactory.LoadAllFonts(UIResourceManager.SpriteData);
			UIResourceManager.BrushFactory = new BrushFactory(UIResourceManager.UIResourceDepot, "Brushes", UIResourceManager.SpriteData, UIResourceManager.FontFactory);
			UIResourceManager.BrushFactory.Initialize();
			UIResourceManager.ResourceContext = new TwoDimensionEngineResourceContext();
		}

		public static void Update()
		{
			if (UIResourceManager._latestUIDebugModeState != UIResourceManager._uiDebugMode)
			{
				if (UIResourceManager._uiDebugMode)
				{
					UIResourceManager.UIResourceDepot.StartWatchingChangesInDepot();
				}
				else
				{
					UIResourceManager.UIResourceDepot.StopWatchingChangesInDepot();
				}
				UIResourceManager._latestUIDebugModeState = UIResourceManager._uiDebugMode;
			}
			if (UIResourceManager._uiDebugMode)
			{
				UIResourceManager.UIResourceDepot.CheckForChanges();
			}
		}

		public static void OnLanguageChange(string newLanguageCode)
		{
			UIResourceManager.FontFactory.OnLanguageChange(newLanguageCode);
		}

		public static void Clear()
		{
			UIResourceManager.WidgetFactory = null;
			UIResourceManager.SpriteData = null;
			UIResourceManager.BrushFactory = null;
			UIResourceManager.FontFactory = null;
		}

		private static bool _latestUIDebugModeState;
	}
}
