using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	// Token: 0x02000023 RID: 35
	public class SceneTableau
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000164 RID: 356 RVA: 0x0000BE57 File Offset: 0x0000A057
		// (set) Token: 0x06000165 RID: 357 RVA: 0x0000BE5F File Offset: 0x0000A05F
		public Texture _texture { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000166 RID: 358 RVA: 0x0000BE68 File Offset: 0x0000A068
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

		// Token: 0x06000167 RID: 359 RVA: 0x0000BE93 File Offset: 0x0000A093
		public SceneTableau()
		{
			this.SetEnabled(true);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000BEB8 File Offset: 0x0000A0B8
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

		// Token: 0x06000169 RID: 361 RVA: 0x0000BED8 File Offset: 0x0000A0D8
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

		// Token: 0x0600016A RID: 362 RVA: 0x0000BF5C File Offset: 0x0000A15C
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
				this.RenderScale = NativeOptions.GetConfig(21) / 100f;
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

		// Token: 0x0600016B RID: 363 RVA: 0x0000C004 File Offset: 0x0000A204
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

		// Token: 0x0600016C RID: 364 RVA: 0x0000C078 File Offset: 0x0000A278
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

		// Token: 0x0600016D RID: 365 RVA: 0x0000C0C3 File Offset: 0x0000A2C3
		public void SetBannerCode(string value)
		{
			this.RefreshCharacterTableau(null);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000C0CC File Offset: 0x0000A2CC
		private void RefreshCharacterTableau(Equipment oldEquipment = null)
		{
			if (!this._initialized)
			{
				this.FirstTimeInit();
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000C0DC File Offset: 0x0000A2DC
		public void RotateCharacter(bool value)
		{
			this._isRotatingCharacter = value;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000C0E8 File Offset: 0x0000A2E8
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

		// Token: 0x06000171 RID: 369 RVA: 0x0000C175 File Offset: 0x0000A375
		private void FirstTimeInit()
		{
			this._initialized = true;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000C180 File Offset: 0x0000A380
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

		// Token: 0x040000E3 RID: 227
		private float _animationFrequencyThreshold = 2.5f;

		// Token: 0x040000E4 RID: 228
		private MatrixFrame _frame;

		// Token: 0x040000E5 RID: 229
		private Scene _tableauScene;

		// Token: 0x040000E6 RID: 230
		private Camera _continuousRenderCamera;

		// Token: 0x040000E7 RID: 231
		private GameEntity _cameraEntity;

		// Token: 0x040000E8 RID: 232
		private float _cameraRatio;

		// Token: 0x040000E9 RID: 233
		private bool _initialized;

		// Token: 0x040000EA RID: 234
		private int _tableauSizeX;

		// Token: 0x040000EB RID: 235
		private int _tableauSizeY;

		// Token: 0x040000EC RID: 236
		private SceneView View;

		// Token: 0x040000ED RID: 237
		private bool _isRotatingCharacter;

		// Token: 0x040000EE RID: 238
		private float _animationGap;

		// Token: 0x040000EF RID: 239
		private bool _isEnabled;

		// Token: 0x040000F0 RID: 240
		private float RenderScale = 1f;
	}
}
