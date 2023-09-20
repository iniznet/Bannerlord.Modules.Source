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
	public class LauncherUI
	{
		public static event Action<string> OnAddHintInformation;

		public static event Action OnHideHintInformation;

		public bool HasUnofficialModulesSelected
		{
			get
			{
				return this._viewModel.ModsData.Modules.Any((LauncherModuleVM m) => !m.IsOfficial && m.IsSelected);
			}
		}

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

		public bool CheckMouseOverWindowDragArea()
		{
			return this._context.EventManager.HoveredView is LauncherDragWindowAreaWidget;
		}

		public bool HitTest()
		{
			return this._movie != null && this._context.HitTest(this._movie.RootWidget);
		}

		public static void AddHintInformation(string message)
		{
			Action<string> onAddHintInformation = LauncherUI.OnAddHintInformation;
			if (onAddHintInformation == null)
			{
				return;
			}
			onAddHintInformation(message);
		}

		public static void HideHintInformation()
		{
			Action onHideHintInformation = LauncherUI.OnHideHintInformation;
			if (onHideHintInformation == null)
			{
				return;
			}
			onHideHintInformation();
		}

		private Material _material;

		private TwoDimensionContext _twoDimensionContext;

		private UIContext _context;

		private IGauntletMovie _movie;

		private LauncherVM _viewModel;

		private SpriteData _spriteData;

		private WidgetFactory _widgetFactory;

		private UserDataManager _userDataManager;

		private readonly Action _onClose;

		private readonly Action _onMinimize;

		private Stopwatch _stopwatch;
	}
}
