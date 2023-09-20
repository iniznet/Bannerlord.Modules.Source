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
	public class MBInitialScreenBase : ScreenBase, IGameStateListener
	{
		private protected InitialState _state { protected get; private set; }

		public MBInitialScreenBase(InitialState state)
		{
			this._state = state;
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
		}

		void IGameStateListener.OnInitialize()
		{
			this._state.OnInitialMenuOptionInvoked += this.OnExecutedInitialStateOption;
		}

		void IGameStateListener.OnFinalize()
		{
			this._state.OnInitialMenuOptionInvoked -= this.OnExecutedInitialStateOption;
		}

		private void OnExecutedInitialStateOption(InitialStateOption target)
		{
		}

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

		protected override void OnFinalize()
		{
			this._camera = null;
			this._sceneLayer = null;
			this._cameraAnimationEntity = null;
			this._scene = null;
			base.OnFinalize();
		}

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

		private void OnSceneEditorWindowOpen()
		{
			GameStateManager.Current.CleanAndPushState(GameStateManager.Current.CreateState<EditorState>(), 0);
		}

		protected override void OnDeactivate()
		{
			this._sceneLayer.SceneView.SetEnable(false);
			this._sceneLayer.SceneView.ClearAll(true, true);
			this._scene.ManualInvalidate();
			this._scene = null;
			base.OnDeactivate();
		}

		protected override void OnPause()
		{
			LoadingWindow.DisableGlobalLoadingWindow();
			base.OnPause();
			if (this._scene != null)
			{
				this._scene.FinishSceneSounds();
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (this._scene != null)
			{
				int frameCountSinceReadyToRender = this._frameCountSinceReadyToRender;
			}
		}

		public static void DoExitButtonAction()
		{
			MBAPI.IMBScreen.OnExitButtonClick();
		}

		public bool StartedRendering()
		{
			return this._sceneLayer.SceneView.ReadyToRender();
		}

		public static void OnEditModeEnterPress()
		{
			MBAPI.IMBScreen.OnEditModeEnterPress();
		}

		public static void OnEditModeEnterRelease()
		{
			MBAPI.IMBScreen.OnEditModeEnterRelease();
		}

		private Camera _camera;

		protected SceneLayer _sceneLayer;

		private int _frameCountSinceReadyToRender;

		private const int _numOfFramesToWaitAfterReadyToRender = 8;

		private GameEntity _cameraAnimationEntity;

		private Scene _scene;

		private bool _buttonInvokeMessage;

		private string _buttonToInvoke;
	}
}
