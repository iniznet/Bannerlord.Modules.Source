using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	public class SceneTableau
	{
		public Texture _texture { get; private set; }

		public bool? IsReady
		{
			get
			{
				SceneView view = this.View;
				if (view == null)
				{
					return null;
				}
				return new bool?(view.ReadyToRender());
			}
		}

		public SceneTableau()
		{
			this.SetEnabled(true);
		}

		private void SetEnabled(bool enabled)
		{
			this._isEnabled = enabled;
			SceneView view = this.View;
			if (view == null)
			{
				return;
			}
			view.SetEnable(this._isEnabled);
		}

		private void CreateTexture()
		{
			this._texture = Texture.CreateRenderTarget("SceneTableau", this._tableauSizeX, this._tableauSizeY, true, false, false, false);
			this.View = SceneView.CreateSceneView();
			this.View.SetScene(this._tableauScene);
			this.View.SetRenderTarget(this._texture);
			this.View.SetAutoDepthTargetCreation(true);
			this.View.SetSceneUsesSkybox(true);
			this.View.SetClearColor(4294902015U);
		}

		public void SetTargetSize(int width, int height)
		{
			this._isRotatingCharacter = false;
			if (width <= 0 || height <= 0)
			{
				this._tableauSizeX = 10;
				this._tableauSizeY = 10;
			}
			else
			{
				this.RenderScale = NativeOptions.GetConfig(25) / 100f;
				this._tableauSizeX = (int)((float)width * this.RenderScale);
				this._tableauSizeY = (int)((float)height * this.RenderScale);
			}
			this._cameraRatio = (float)this._tableauSizeX / (float)this._tableauSizeY;
			SceneView view = this.View;
			SceneView view2 = this.View;
			if (view2 != null)
			{
				view2.SetEnable(false);
			}
			SceneView view3 = this.View;
			if (view3 != null)
			{
				view3.AddClearTask(true);
			}
			this.CreateTexture();
		}

		public void OnFinalize()
		{
			if (this._continuousRenderCamera != null)
			{
				this._continuousRenderCamera.ReleaseCameraEntity();
				this._continuousRenderCamera = null;
				this._cameraEntity = null;
			}
			SceneView view = this.View;
			if (view != null)
			{
				view.SetEnable(false);
			}
			SceneView view2 = this.View;
			if (view2 != null)
			{
				view2.AddClearTask(false);
			}
			this._texture.ReleaseNextFrame();
			this._texture = null;
			this._tableauScene = null;
		}

		public void SetScene(object scene)
		{
			Scene scene2;
			if ((scene2 = scene as Scene) != null)
			{
				this._tableauScene = scene2;
				if (this._tableauSizeX != 0 && this._tableauSizeY != 0)
				{
					this.CreateTexture();
					return;
				}
			}
			else
			{
				Debug.FailedAssert("Given scene object is not Scene type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Tableaus\\SceneTableau.cs", "SetScene", 120);
			}
		}

		public void SetBannerCode(string value)
		{
			this.RefreshCharacterTableau(null);
		}

		private void RefreshCharacterTableau(Equipment oldEquipment = null)
		{
			if (!this._initialized)
			{
				this.FirstTimeInit();
			}
		}

		public void RotateCharacter(bool value)
		{
			this._isRotatingCharacter = value;
		}

		public void OnTick(float dt)
		{
			if (this._animationFrequencyThreshold > this._animationGap)
			{
				this._animationGap += dt;
			}
			if (this.View != null)
			{
				if (this._continuousRenderCamera == null)
				{
					GameEntity gameEntity = this._tableauScene.FindEntityWithTag("customcamera");
					if (gameEntity != null)
					{
						this._continuousRenderCamera = Camera.CreateCamera();
						Vec3 vec = default(Vec3);
						gameEntity.GetCameraParamsFromCameraScript(this._continuousRenderCamera, ref vec);
						this._cameraEntity = gameEntity;
					}
				}
				this.PopupSceneContinuousRenderFunction();
			}
		}

		private void FirstTimeInit()
		{
			this._initialized = true;
		}

		private void PopupSceneContinuousRenderFunction()
		{
			GameEntity gameEntity = this._tableauScene.FindEntityWithTag("customcamera");
			this._tableauScene.SetShadow(true);
			this._tableauScene.EnsurePostfxSystem();
			this._tableauScene.SetMotionBlurMode(true);
			this._tableauScene.SetBloom(true);
			this._tableauScene.SetDynamicShadowmapCascadesRadiusMultiplier(1f);
			this.View.SetRenderWithPostfx(true);
			this.View.SetSceneUsesShadows(true);
			this.View.SetScene(this._tableauScene);
			this.View.SetSceneUsesSkybox(true);
			this.View.SetClearColor(4278190080U);
			this.View.SetFocusedShadowmap(false, ref this._frame.origin, 1.55f);
			this.View.SetEnable(true);
			if (gameEntity != null)
			{
				Vec3 vec = default(Vec3);
				gameEntity.GetCameraParamsFromCameraScript(this._continuousRenderCamera, ref vec);
				if (this._continuousRenderCamera != null)
				{
					Camera continuousRenderCamera = this._continuousRenderCamera;
					this.View.SetCamera(continuousRenderCamera);
				}
			}
		}

		private float _animationFrequencyThreshold = 2.5f;

		private MatrixFrame _frame;

		private Scene _tableauScene;

		private Camera _continuousRenderCamera;

		private GameEntity _cameraEntity;

		private float _cameraRatio;

		private bool _initialized;

		private int _tableauSizeX;

		private int _tableauSizeY;

		private SceneView View;

		private bool _isRotatingCharacter;

		private float _animationGap;

		private bool _isEnabled;

		private float RenderScale = 1f;
	}
}
