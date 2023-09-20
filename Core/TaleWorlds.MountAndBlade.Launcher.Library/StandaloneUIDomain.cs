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
	// Token: 0x0200000A RID: 10
	public class StandaloneUIDomain : FrameworkDomain
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002BF5 File Offset: 0x00000DF5
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002BFD File Offset: 0x00000DFD
		public UserDataManager UserDataManager { get; private set; }

		// Token: 0x0600004C RID: 76 RVA: 0x00002C06 File Offset: 0x00000E06
		public StandaloneUIDomain(GraphicsForm graphicsForm, ResourceDepot resourceDepot)
		{
			this._graphicsForm = graphicsForm;
			this._resourceDepot = resourceDepot;
			this.UserDataManager = new UserDataManager();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002C28 File Offset: 0x00000E28
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
				TwoDimensionPlatform twoDimensionPlatform = new TwoDimensionPlatform(this._graphicsForm);
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00002DEA File Offset: 0x00000FEA
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002E05 File Offset: 0x00001005
		public bool HasUnofficialModulesSelected
		{
			get
			{
				return this._launcherUI.HasUnofficialModulesSelected;
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002E12 File Offset: 0x00001012
		public override void Destroy()
		{
			this._synchronizationContext = null;
			this._initialized = false;
			this._graphicsContext.DestroyContext();
			this._gauntletUIContext = null;
			this._launcherUI = null;
			this._graphicsForm.Destroy();
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002E46 File Offset: 0x00001046
		private void OnStartGameRequest()
		{
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002E48 File Offset: 0x00001048
		private void OnCloseRequest()
		{
			Environment.Exit(0);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002E50 File Offset: 0x00001050
		private void OnMinimizeRequest()
		{
			this._graphicsForm.MinimizeWindow();
		}

		// Token: 0x0400002C RID: 44
		private SingleThreadedSynchronizationContext _synchronizationContext;

		// Token: 0x0400002D RID: 45
		private bool _initialized;

		// Token: 0x0400002E RID: 46
		private GraphicsForm _graphicsForm;

		// Token: 0x0400002F RID: 47
		private GraphicsContext _graphicsContext;

		// Token: 0x04000030 RID: 48
		private UIContext _gauntletUIContext;

		// Token: 0x04000031 RID: 49
		private TwoDimensionContext _twoDimensionContext;

		// Token: 0x04000032 RID: 50
		private LauncherUI _launcherUI;

		// Token: 0x04000033 RID: 51
		private readonly ResourceDepot _resourceDepot;
	}
}
