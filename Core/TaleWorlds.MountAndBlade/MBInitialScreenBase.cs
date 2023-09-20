using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200038D RID: 909
	public class MBInitialScreenBase : ScreenBase, IGameStateListener
	{
		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x060031C9 RID: 12745 RVA: 0x000CF0E8 File Offset: 0x000CD2E8
		// (set) Token: 0x060031CA RID: 12746 RVA: 0x000CF0F0 File Offset: 0x000CD2F0
		private protected InitialState _state { protected get; private set; }

		// Token: 0x060031CB RID: 12747 RVA: 0x000CF0F9 File Offset: 0x000CD2F9
		public MBInitialScreenBase(InitialState state)
		{
			this._state = state;
		}

		// Token: 0x060031CC RID: 12748 RVA: 0x000CF108 File Offset: 0x000CD308
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x060031CD RID: 12749 RVA: 0x000CF10A File Offset: 0x000CD30A
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x060031CE RID: 12750 RVA: 0x000CF10C File Offset: 0x000CD30C
		void IGameStateListener.OnInitialize()
		{
			this._state.OnInitialMenuOptionInvoked += this.OnExecutedInitialStateOption;
		}

		// Token: 0x060031CF RID: 12751 RVA: 0x000CF125 File Offset: 0x000CD325
		void IGameStateListener.OnFinalize()
		{
			this._state.OnInitialMenuOptionInvoked -= this.OnExecutedInitialStateOption;
		}

		// Token: 0x060031D0 RID: 12752 RVA: 0x000CF13E File Offset: 0x000CD33E
		private void OnExecutedInitialStateOption(InitialStateOption target)
		{
		}

		// Token: 0x060031D1 RID: 12753 RVA: 0x000CF140 File Offset: 0x000CD340
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._sceneLayer = new SceneLayer("SceneLayer", true, true);
			base.AddLayer(this._sceneLayer);
			this._sceneLayer.SceneView.SetResolutionScaling(true);
			this._sceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._camera = Camera.CreateCamera();
			Common.MemoryCleanupGC(false);
			if (Game.Current != null)
			{
				Game.Current.Destroy();
			}
			MBMusicManager.Initialize();
		}

		// Token: 0x060031D2 RID: 12754 RVA: 0x000CF1C3 File Offset: 0x000CD3C3
		protected override void OnFinalize()
		{
			this._camera = null;
			this._sceneLayer = null;
			this._cameraAnimationEntity = null;
			this._scene = null;
			base.OnFinalize();
		}

		// Token: 0x060031D3 RID: 12755 RVA: 0x000CF1E8 File Offset: 0x000CD3E8
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._buttonInvokeMessage)
			{
				this._buttonInvokeMessage = false;
				Module.CurrentModule.ExecuteInitialStateOptionWithId(this._buttonToInvoke);
			}
			if (this._sceneLayer == null)
			{
				Console.WriteLine("InitialScreen::OnFrameTick scene view null");
			}
			if (this._scene == null)
			{
				return;
			}
			if (this._sceneLayer != null && this._sceneLayer.SceneView.ReadyToRender())
			{
				if (this._frameCountSinceReadyToRender > 8)
				{
					Utilities.DisableGlobalLoadingWindow();
					LoadingWindow.DisableGlobalLoadingWindow();
				}
				else
				{
					this._frameCountSinceReadyToRender++;
				}
			}
			if (this._sceneLayer != null)
			{
				this._sceneLayer.SetCamera(this._camera);
			}
			SoundManager.SetListenerFrame(this._camera.Frame);
			this._scene.Tick(dt);
			if (Input.IsKeyDown(InputKey.LeftControl) && Input.IsKeyReleased(InputKey.E))
			{
				MBInitialScreenBase.OnEditModeEnterPress();
			}
		}

		// Token: 0x060031D4 RID: 12756 RVA: 0x000CF2C5 File Offset: 0x000CD4C5
		protected override void OnActivate()
		{
			base.OnActivate();
			if (Utilities.renderingActive)
			{
				this.RefreshScene();
			}
			this._frameCountSinceReadyToRender = 0;
			if (NativeConfig.DoLocalizationCheckAtStartup)
			{
				LocalizedTextManager.CheckValidity(new List<string>());
			}
		}

		// Token: 0x060031D5 RID: 12757 RVA: 0x000CF2F4 File Offset: 0x000CD4F4
		private void RefreshScene()
		{
			if (this._scene == null)
			{
				this._scene = Scene.CreateNewScene(true, false, DecalAtlasGroup.All, "mono_renderscene");
				this._scene.SetName("MBInitialScreenBase");
				this._scene.SetPlaySoundEventsAfterReadyToRender(true);
				SceneInitializationData sceneInitializationData = new SceneInitializationData(true);
				this._scene.Read("main_menu_a", ref sceneInitializationData, "");
				for (int i = 0; i < 40; i++)
				{
					this._scene.Tick(0.1f);
				}
				Vec3 vec = default(Vec3);
				this._scene.FindEntityWithTag("camera_instance").GetCameraParamsFromCameraScript(this._camera, ref vec);
			}
			SoundManager.SetListenerFrame(this._camera.Frame);
			if (this._sceneLayer != null)
			{
				this._sceneLayer.SetScene(this._scene);
				this._sceneLayer.SceneView.SetEnable(true);
				this._sceneLayer.SceneView.SetSceneUsesShadows(true);
			}
			this._cameraAnimationEntity = GameEntity.CreateEmpty(this._scene, true);
		}

		// Token: 0x060031D6 RID: 12758 RVA: 0x000CF3FF File Offset: 0x000CD5FF
		private void OnSceneEditorWindowOpen()
		{
			GameStateManager.Current.CleanAndPushState(GameStateManager.Current.CreateState<EditorState>(), 0);
		}

		// Token: 0x060031D7 RID: 12759 RVA: 0x000CF416 File Offset: 0x000CD616
		protected override void OnDeactivate()
		{
			this._sceneLayer.SceneView.SetEnable(false);
			this._sceneLayer.SceneView.ClearAll(true, true);
			this._scene.ManualInvalidate();
			this._scene = null;
			base.OnDeactivate();
		}

		// Token: 0x060031D8 RID: 12760 RVA: 0x000CF453 File Offset: 0x000CD653
		protected override void OnPause()
		{
			LoadingWindow.DisableGlobalLoadingWindow();
			base.OnPause();
			if (this._scene != null)
			{
				this._scene.FinishSceneSounds();
			}
		}

		// Token: 0x060031D9 RID: 12761 RVA: 0x000CF479 File Offset: 0x000CD679
		protected override void OnResume()
		{
			base.OnResume();
			if (this._scene != null)
			{
				int frameCountSinceReadyToRender = this._frameCountSinceReadyToRender;
			}
		}

		// Token: 0x060031DA RID: 12762 RVA: 0x000CF498 File Offset: 0x000CD698
		public static void DoExitButtonAction()
		{
			MBAPI.IMBScreen.OnExitButtonClick();
		}

		// Token: 0x060031DB RID: 12763 RVA: 0x000CF4A4 File Offset: 0x000CD6A4
		public bool StartedRendering()
		{
			return this._sceneLayer.SceneView.ReadyToRender();
		}

		// Token: 0x060031DC RID: 12764 RVA: 0x000CF4B6 File Offset: 0x000CD6B6
		public static void OnEditModeEnterPress()
		{
			MBAPI.IMBScreen.OnEditModeEnterPress();
		}

		// Token: 0x060031DD RID: 12765 RVA: 0x000CF4C2 File Offset: 0x000CD6C2
		public static void OnEditModeEnterRelease()
		{
			MBAPI.IMBScreen.OnEditModeEnterRelease();
		}

		// Token: 0x040014E4 RID: 5348
		private Camera _camera;

		// Token: 0x040014E5 RID: 5349
		protected SceneLayer _sceneLayer;

		// Token: 0x040014E6 RID: 5350
		private int _frameCountSinceReadyToRender;

		// Token: 0x040014E7 RID: 5351
		private const int _numOfFramesToWaitAfterReadyToRender = 8;

		// Token: 0x040014E8 RID: 5352
		private GameEntity _cameraAnimationEntity;

		// Token: 0x040014E9 RID: 5353
		private Scene _scene;

		// Token: 0x040014EA RID: 5354
		private bool _buttonInvokeMessage;

		// Token: 0x040014EB RID: 5355
		private string _buttonToInvoke;
	}
}
