using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	public class BrightnessDemoTableau
	{
		public Texture Texture { get; private set; }

		private TableauView View
		{
			get
			{
				if (this.Texture != null)
				{
					return this.Texture.TableauView;
				}
				return null;
			}
		}

		private void SetEnabled(bool enabled)
		{
			this._isEnabled = enabled;
			TableauView view = this.View;
			if (!this._initialized)
			{
				this.SetScene();
			}
			if (view == null)
			{
				return;
			}
			view.SetEnable(this._isEnabled);
		}

		public void SetDemoType(int demoType)
		{
			this._demoType = demoType;
			this._initialized = false;
			this.RefreshDemoTableau();
		}

		public void SetTargetSize(int width, int height)
		{
			int num;
			int num2;
			if (width <= 0 || height <= 0)
			{
				num = 10;
				num2 = 10;
			}
			else
			{
				this.RenderScale = NativeOptions.GetConfig(21) / 100f;
				num = (int)((float)width * this.RenderScale);
				num2 = (int)((float)height * this.RenderScale);
			}
			if (num != this._tableauSizeX || num2 != this._tableauSizeY)
			{
				this._tableauSizeX = num;
				this._tableauSizeY = num2;
				TableauView view = this.View;
				if (view != null)
				{
					view.SetEnable(false);
				}
				TableauView view2 = this.View;
				if (view2 != null)
				{
					view2.AddClearTask(true);
				}
				Texture texture = this.Texture;
				if (texture != null)
				{
					texture.ReleaseNextFrame();
				}
				this.Texture = TableauView.AddTableau("BrightnessDemo", new RenderTargetComponent.TextureUpdateEventHandler(this.SceneTableauContinuousRenderFunction), this._tableauScene, this._tableauSizeX, this._tableauSizeY);
			}
		}

		public void OnFinalize()
		{
			if (this._continuousRenderCamera != null)
			{
				this._continuousRenderCamera.ReleaseCameraEntity();
				this._continuousRenderCamera = null;
			}
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(false);
			}
			TableauView view2 = this.View;
			if (view2 != null)
			{
				view2.AddClearTask(false);
			}
			this.Texture = null;
			this._tableauScene = null;
		}

		public void SetScene()
		{
			this._tableauScene = Scene.CreateNewScene(true, true, 0, "mono_renderscene");
			switch (this._demoType)
			{
			case 0:
				this._demoTexture = Texture.GetFromResource("brightness_calibration_wide");
				this._tableauScene.SetAtmosphereWithName("brightness_calibration_screen");
				break;
			case 1:
				this._demoTexture = Texture.GetFromResource("calibration_image_1");
				this._tableauScene.SetAtmosphereWithName("TOD_11_00_SemiCloudy");
				break;
			case 2:
				this._demoTexture = Texture.GetFromResource("calibration_image_2");
				this._tableauScene.SetAtmosphereWithName("TOD_05_00_SemiCloudy");
				break;
			case 3:
				this._demoTexture = Texture.GetFromResource("calibration_image_3");
				this._tableauScene.SetAtmosphereWithName("exposure_calibration_interior");
				break;
			default:
				Debug.FailedAssert(string.Format("Undefined Brightness demo type({0})", this._demoType), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Tableaus\\BrightnessDemoTableau.cs", "SetScene", 130);
				break;
			}
			this._tableauScene.SetDepthOfFieldParameters(0f, 0f, false);
		}

		private void RefreshDemoTableau()
		{
			if (!this._initialized)
			{
				this.SetEnabled(true);
			}
		}

		public void OnTick(float dt)
		{
			if (this._continuousRenderCamera == null)
			{
				this._continuousRenderCamera = Camera.CreateCamera();
			}
			TableauView view = this.View;
			if (view == null)
			{
				return;
			}
			view.SetDoNotRenderThisFrame(false);
		}

		internal void SceneTableauContinuousRenderFunction(Texture sender, EventArgs e)
		{
			Scene scene = (Scene)sender.UserData;
			TableauView tableauView = sender.TableauView;
			tableauView.SetEnable(true);
			if (scene == null)
			{
				tableauView.SetContinuousRendering(false);
				tableauView.SetDeleteAfterRendering(true);
				return;
			}
			scene.SetShadow(false);
			scene.EnsurePostfxSystem();
			scene.SetDofMode(false);
			scene.SetMotionBlurMode(false);
			scene.SetBloom(true);
			scene.SetDynamicShadowmapCascadesRadiusMultiplier(0.31f);
			scene.SetExternalInjectionTexture(this._demoTexture);
			tableauView.SetRenderWithPostfx(true);
			if (this._continuousRenderCamera != null)
			{
				Camera continuousRenderCamera = this._continuousRenderCamera;
				tableauView.SetCamera(continuousRenderCamera);
				tableauView.SetScene(scene);
				tableauView.SetSceneUsesSkybox(false);
				tableauView.SetDeleteAfterRendering(false);
				tableauView.SetContinuousRendering(true);
				tableauView.SetDoNotRenderThisFrame(true);
				tableauView.SetClearColor(4278190080U);
				tableauView.SetFocusedShadowmap(true, ref this._frame.origin, 1.55f);
			}
		}

		private MatrixFrame _frame;

		private Scene _tableauScene;

		private Texture _demoTexture;

		private Camera _continuousRenderCamera;

		private bool _initialized;

		private int _tableauSizeX;

		private int _tableauSizeY;

		private int _demoType = -1;

		private bool _isEnabled;

		private float RenderScale = 1f;
	}
}
