using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets;
using TaleWorlds.MountAndBlade.Launcher.Library.UserDatas;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x02000007 RID: 7
	public class LauncherUI
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000030 RID: 48 RVA: 0x00002534 File Offset: 0x00000734
		// (remove) Token: 0x06000031 RID: 49 RVA: 0x00002568 File Offset: 0x00000768
		public static event Action<string> OnAddHintInformation;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000032 RID: 50 RVA: 0x0000259C File Offset: 0x0000079C
		// (remove) Token: 0x06000033 RID: 51 RVA: 0x000025D0 File Offset: 0x000007D0
		public static event Action OnHideHintInformation;

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002603 File Offset: 0x00000803
		public bool HasUnofficialModulesSelected
		{
			get
			{
				return this._viewModel.ModsData.Modules.Any((LauncherModuleVM m) => !m.IsOfficial && m.IsSelected);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000263C File Offset: 0x0000083C
		public LauncherUI(UserDataManager userDataManager, UIContext context, Action onClose, Action onMinimize)
		{
			this._context = context;
			this._twoDimensionContext = this._context.TwoDimensionContext;
			this._userDataManager = userDataManager;
			this._onClose = onClose;
			this._onMinimize = onMinimize;
			this._stopwatch = new Stopwatch();
			this._stopwatch.Start();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002694 File Offset: 0x00000894
		public void Initialize()
		{
			this._spriteData = this._context.SpriteData;
			this._spriteData.SpriteCategories["ui_launcher"].Load(this._twoDimensionContext.ResourceContext, this._twoDimensionContext.ResourceDepot);
			this._spriteData.SpriteCategories["ui_fonts_launcher"].Load(this._twoDimensionContext.ResourceContext, this._twoDimensionContext.ResourceDepot);
			this._material = new PrimitivePolygonMaterial(new Color(0.5f, 0.5f, 0.5f, 1f));
			this._widgetFactory = new WidgetFactory(this._context.ResourceDepot, "Prefabs");
			this._widgetFactory.PrefabExtensionContext.AddExtension(new PrefabDatabindingExtension());
			this._widgetFactory.Initialize(null);
			this._viewModel = new LauncherVM(this._userDataManager, this._onClose, this._onMinimize);
			this._movie = GauntletMovie.Load(this._context, this._widgetFactory, "UILauncher", this._viewModel, false, true);
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000027B3 File Offset: 0x000009B3
		public string AdditionalArgs
		{
			get
			{
				if (this._viewModel == null)
				{
					return "";
				}
				return this._viewModel.GameTypeArgument + " " + this._viewModel.ModsData.ModuleListCode + this._viewModel.ContinueGameArgument;
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000027F4 File Offset: 0x000009F4
		public void Update()
		{
			DrawObject2D.CreateTriangleTopologyMeshWithPolygonCoordinates(new List<Vector2>
			{
				new Vector2(0f, 0f),
				new Vector2(0f, this._twoDimensionContext.Height),
				new Vector2(this._twoDimensionContext.Width, this._twoDimensionContext.Height),
				new Vector2(this._twoDimensionContext.Width, 0f)
			});
			this._movie.Update();
			if (this._stopwatch.IsRunning)
			{
				this._stopwatch.Stop();
				Debug.Print("Total startup time: " + ((float)this._stopwatch.ElapsedMilliseconds / 1000f).ToString("0.0000"), 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000028D5 File Offset: 0x00000AD5
		public bool CheckMouseOverWindowDragArea()
		{
			return this._context.EventManager.HoveredView is LauncherDragWindowAreaWidget;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000028EF File Offset: 0x00000AEF
		public bool HitTest()
		{
			return this._movie != null && this._context.HitTest(this._movie.RootWidget);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002911 File Offset: 0x00000B11
		public static void AddHintInformation(string message)
		{
			Action<string> onAddHintInformation = LauncherUI.OnAddHintInformation;
			if (onAddHintInformation == null)
			{
				return;
			}
			onAddHintInformation(message);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002923 File Offset: 0x00000B23
		public static void HideHintInformation()
		{
			Action onHideHintInformation = LauncherUI.OnHideHintInformation;
			if (onHideHintInformation == null)
			{
				return;
			}
			onHideHintInformation();
		}

		// Token: 0x04000016 RID: 22
		private Material _material;

		// Token: 0x04000017 RID: 23
		private TwoDimensionContext _twoDimensionContext;

		// Token: 0x04000018 RID: 24
		private UIContext _context;

		// Token: 0x04000019 RID: 25
		private IGauntletMovie _movie;

		// Token: 0x0400001A RID: 26
		private LauncherVM _viewModel;

		// Token: 0x0400001B RID: 27
		private SpriteData _spriteData;

		// Token: 0x0400001C RID: 28
		private WidgetFactory _widgetFactory;

		// Token: 0x0400001D RID: 29
		private UserDataManager _userDataManager;

		// Token: 0x0400001E RID: 30
		private readonly Action _onClose;

		// Token: 0x0400001F RID: 31
		private readonly Action _onMinimize;

		// Token: 0x04000020 RID: 32
		private Stopwatch _stopwatch;
	}
}
