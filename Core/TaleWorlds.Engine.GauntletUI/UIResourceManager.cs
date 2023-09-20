using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.Engine.GauntletUI
{
	// Token: 0x02000008 RID: 8
	public static class UIResourceManager
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000049 RID: 73 RVA: 0x0000318B File Offset: 0x0000138B
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00003192 File Offset: 0x00001392
		public static ResourceDepot UIResourceDepot { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004B RID: 75 RVA: 0x0000319A File Offset: 0x0000139A
		// (set) Token: 0x0600004C RID: 76 RVA: 0x000031A1 File Offset: 0x000013A1
		public static WidgetFactory WidgetFactory { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000031A9 File Offset: 0x000013A9
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000031B0 File Offset: 0x000013B0
		public static SpriteData SpriteData { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000031B8 File Offset: 0x000013B8
		// (set) Token: 0x06000050 RID: 80 RVA: 0x000031BF File Offset: 0x000013BF
		public static BrushFactory BrushFactory { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000051 RID: 81 RVA: 0x000031C7 File Offset: 0x000013C7
		// (set) Token: 0x06000052 RID: 82 RVA: 0x000031CE File Offset: 0x000013CE
		public static FontFactory FontFactory { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000031D6 File Offset: 0x000013D6
		// (set) Token: 0x06000054 RID: 84 RVA: 0x000031DD File Offset: 0x000013DD
		public static TwoDimensionEngineResourceContext ResourceContext { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000031E5 File Offset: 0x000013E5
		private static bool _uiDebugMode
		{
			get
			{
				return UIConfig.DebugModeEnabled || NativeConfig.GetUIDebugMode;
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000031F8 File Offset: 0x000013F8
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

		// Token: 0x06000057 RID: 87 RVA: 0x000032AC File Offset: 0x000014AC
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

		// Token: 0x06000058 RID: 88 RVA: 0x000032FD File Offset: 0x000014FD
		public static void OnLanguageChange(string newLanguageCode)
		{
			UIResourceManager.FontFactory.OnLanguageChange(newLanguageCode);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000330A File Offset: 0x0000150A
		public static void Clear()
		{
			UIResourceManager.WidgetFactory = null;
			UIResourceManager.SpriteData = null;
			UIResourceManager.BrushFactory = null;
			UIResourceManager.FontFactory = null;
		}

		// Token: 0x0400001A RID: 26
		private static bool _latestUIDebugModeState;
	}
}
