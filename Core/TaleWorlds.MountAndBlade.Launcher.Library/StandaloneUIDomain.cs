using System;
using System.Threading;
using TaleWorlds.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Launcher.Library.UserDatas;
using TaleWorlds.TwoDimension;
using TaleWorlds.TwoDimension.Standalone;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class StandaloneUIDomain : FrameworkDomain
	{
		public UserDataManager UserDataManager { get; private set; }

		public StandaloneUIDomain(GraphicsForm graphicsForm, ResourceDepot resourceDepot)
		{
			this._graphicsForm = graphicsForm;
			this._resourceDepot = resourceDepot;
			this.UserDataManager = new UserDataManager();
		}

		public override void Update()
		{
			if (this._synchronizationContext == null)
			{
				this._synchronizationContext = new SingleThreadedSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(this._synchronizationContext);
			}
			if (!this._initialized)
			{
				this.UserDataManager.LoadUserData();
				Input.Initialize(new StandaloneInputManager(this._graphicsForm), null);
				this._graphicsForm.InitializeGraphicsContext(this._resourceDepot);
				this._graphicsContext = this._graphicsForm.GraphicsContext;
				TwoDimensionPlatform twoDimensionPlatform = new TwoDimensionPlatform(this._graphicsForm, true);
				this._twoDimensionContext = new TwoDimensionContext(twoDimensionPlatform, twoDimensionPlatform, this._resourceDepot);
				StandaloneInputService standaloneInputService = new StandaloneInputService(this._graphicsForm);
				InputContext inputContext = new InputContext();
				inputContext.MouseOnMe = true;
				inputContext.IsKeysAllowed = true;
				inputContext.IsMouseButtonAllowed = true;
				inputContext.IsMouseWheelAllowed = true;
				this._gauntletUIContext = new UIContext(this._twoDimensionContext, inputContext, standaloneInputService);
				this._gauntletUIContext.IsDynamicScaleEnabled = false;
				this._gauntletUIContext.Initialize();
				this._launcherUI = new LauncherUI(this.UserDataManager, this._gauntletUIContext, new Action(this.OnCloseRequest), new Action(this.OnMinimizeRequest));
				this._launcherUI.Initialize();
				this._initialized = true;
			}
			this._resourceDepot.CheckForChanges();
			this._synchronizationContext.Tick();
			bool flag = this._launcherUI.CheckMouseOverWindowDragArea();
			this._graphicsForm.UpdateInput(flag);
			this._graphicsForm.BeginFrame();
			Input.Update();
			this._graphicsForm.Update();
			this._gauntletUIContext.UpdateInput(InputType.MouseButton | InputType.MouseWheel | InputType.Key);
			this._gauntletUIContext.Update(0.033333335f);
			this._launcherUI.Update();
			this._gauntletUIContext.LateUpdate(0.033333335f);
			this._graphicsForm.PostRender();
			this._graphicsContext.SwapBuffers();
		}

		public string AdditionalArgs
		{
			get
			{
				if (this._launcherUI == null)
				{
					return "";
				}
				return this._launcherUI.AdditionalArgs;
			}
		}

		public bool HasUnofficialModulesSelected
		{
			get
			{
				return this._launcherUI.HasUnofficialModulesSelected;
			}
		}

		public override void Destroy()
		{
			this._synchronizationContext = null;
			this._initialized = false;
			this._graphicsContext.DestroyContext();
			this._gauntletUIContext = null;
			this._launcherUI = null;
			this._graphicsForm.Destroy();
		}

		private void OnStartGameRequest()
		{
		}

		private void OnCloseRequest()
		{
			Environment.Exit(0);
		}

		private void OnMinimizeRequest()
		{
			this._graphicsForm.MinimizeWindow();
		}

		private SingleThreadedSynchronizationContext _synchronizationContext;

		private bool _initialized;

		private GraphicsForm _graphicsForm;

		private GraphicsContext _graphicsContext;

		private UIContext _gauntletUIContext;

		private TwoDimensionContext _twoDimensionContext;

		private LauncherUI _launcherUI;

		private readonly ResourceDepot _resourceDepot;
	}
}
