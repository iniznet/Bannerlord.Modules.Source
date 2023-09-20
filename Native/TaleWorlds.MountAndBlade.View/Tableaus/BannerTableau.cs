using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	public class BannerTableau
	{
		public Texture Texture { get; private set; }

		internal Camera CurrentCamera
		{
			get
			{
				if (!this._isNineGrid)
				{
					return this._defaultCamera;
				}
				return this._nineGridCamera;
			}
		}

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

		public BannerTableau()
		{
			this.SetEnabled(true);
			this.FirstTimeInit();
		}

		public void OnTick(float dt)
		{
			if (this._isEnabled && !this._isFinalized)
			{
				this.Refresh();
				TableauView view = this.View;
				if (view == null)
				{
					return;
				}
				view.SetDoNotRenderThisFrame(false);
			}
		}

		private void FirstTimeInit()
		{
			this._scene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
			this._scene.DisableStaticShadows(true);
			this._scene.SetName("BannerTableau.Scene");
			this._scene.SetDefaultLighting();
			this._defaultCamera = TableauCacheManager.CreateDefaultBannerCamera();
			this._nineGridCamera = TableauCacheManager.CreateNineGridBannerCamera();
			this._isDirty = true;
		}

		private void Refresh()
		{
			if (this._isDirty)
			{
				if (this._currentMeshEntity != null)
				{
					this._scene.RemoveEntity(this._currentMeshEntity, 111);
				}
				if (this._banner != null)
				{
					MatrixFrame identity = MatrixFrame.Identity;
					this._currentMultiMesh = this._banner.ConvertToMultiMesh();
					this._currentMeshEntity = this._scene.AddItemEntity(ref identity, this._currentMultiMesh);
					this._currentMeshEntity.ManualInvalidate();
					this._currentMultiMesh.ManualInvalidate();
					this._isDirty = false;
				}
			}
		}

		private void SetEnabled(bool enabled)
		{
			this._isEnabled = enabled;
			TableauView view = this.View;
			if (view == null)
			{
				return;
			}
			view.SetEnable(this._isEnabled);
		}

		public void SetTargetSize(int width, int height)
		{
			this._latestWidth = width;
			this._latestHeight = height;
			if (width <= 0 || height <= 0)
			{
				this._tableauSizeX = 10;
				this._tableauSizeY = 10;
			}
			else
			{
				this.RenderScale = NativeOptions.GetConfig(25) / 100f;
				this._tableauSizeX = (int)((float)width * this._customRenderScale * this.RenderScale);
				this._tableauSizeY = (int)((float)height * this._customRenderScale * this.RenderScale);
			}
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
			this.Texture = TableauView.AddTableau("BannerTableau", new RenderTargetComponent.TextureUpdateEventHandler(this.BannerTableauContinuousRenderFunction), this._scene, this._tableauSizeX, this._tableauSizeY);
			this.Texture.TableauView.SetSceneUsesContour(false);
		}

		public void SetBannerCode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this._banner = null;
			}
			else
			{
				this._banner = BannerCode.CreateFrom(value).CalculateBanner();
			}
			this._isDirty = true;
		}

		public void OnFinalize()
		{
			if (!this._isFinalized)
			{
				Scene scene = this._scene;
				if (scene != null)
				{
					scene.ClearDecals();
				}
				Scene scene2 = this._scene;
				if (scene2 != null)
				{
					scene2.ClearAll();
				}
				Scene scene3 = this._scene;
				if (scene3 != null)
				{
					scene3.ManualInvalidate();
				}
				this._scene = null;
				TableauView view = this.View;
				if (view != null)
				{
					view.SetEnable(false);
				}
				Texture texture = this.Texture;
				if (texture != null)
				{
					texture.ReleaseNextFrame();
				}
				this.Texture = null;
				Camera defaultCamera = this._defaultCamera;
				if (defaultCamera != null)
				{
					defaultCamera.ReleaseCamera();
				}
				this._defaultCamera = null;
				Camera nineGridCamera = this._nineGridCamera;
				if (nineGridCamera != null)
				{
					nineGridCamera.ReleaseCamera();
				}
				this._nineGridCamera = null;
			}
			this._isFinalized = true;
		}

		public void SetCustomRenderScale(float value)
		{
			if (!MBMath.ApproximatelyEqualsTo(this._customRenderScale, value, 1E-05f))
			{
				this._customRenderScale = value;
				if (this._latestWidth != -1 && this._latestHeight != -1)
				{
					this.SetTargetSize(this._latestWidth, this._latestHeight);
				}
			}
		}

		internal void BannerTableauContinuousRenderFunction(Texture sender, EventArgs e)
		{
			Scene scene = (Scene)sender.UserData;
			TableauView tableauView = sender.TableauView;
			if (scene == null)
			{
				tableauView.SetContinuousRendering(false);
				tableauView.SetDeleteAfterRendering(true);
				return;
			}
			scene.EnsurePostfxSystem();
			scene.SetDofMode(false);
			scene.SetMotionBlurMode(false);
			scene.SetBloom(false);
			scene.SetDynamicShadowmapCascadesRadiusMultiplier(0.31f);
			tableauView.SetRenderWithPostfx(false);
			tableauView.SetScene(scene);
			tableauView.SetCamera(this.CurrentCamera);
			tableauView.SetSceneUsesSkybox(false);
			tableauView.SetDeleteAfterRendering(false);
			tableauView.SetContinuousRendering(true);
			tableauView.SetDoNotRenderThisFrame(true);
			tableauView.SetClearColor(0U);
		}

		public void SetIsNineGrid(bool value)
		{
			this._isNineGrid = value;
			this._isDirty = true;
		}

		public void SetMeshIndexToUpdate(int value)
		{
			this._meshIndexToUpdate = value;
		}

		public void SetUpdatePositionValueManual(Vec2 value)
		{
			if (this._currentMultiMesh.MeshCount >= 1 && this._meshIndexToUpdate >= 0 && this._meshIndexToUpdate < this._currentMultiMesh.MeshCount)
			{
				Mesh meshAtIndex = this._currentMultiMesh.GetMeshAtIndex(this._meshIndexToUpdate);
				MatrixFrame localFrame = meshAtIndex.GetLocalFrame();
				localFrame.origin.x = 0f;
				localFrame.origin.y = 0f;
				localFrame.origin.x = localFrame.origin.x + value.X / 1528f;
				localFrame.origin.y = localFrame.origin.y - value.Y / 1528f;
				meshAtIndex.SetLocalFrame(localFrame);
			}
		}

		public void SetUpdateSizeValueManual(Vec2 value)
		{
			if (this._currentMultiMesh.MeshCount >= 1 && this._meshIndexToUpdate >= 0 && this._meshIndexToUpdate < this._currentMultiMesh.MeshCount)
			{
				Mesh meshAtIndex = this._currentMultiMesh.GetMeshAtIndex(this._meshIndexToUpdate);
				MatrixFrame localFrame = meshAtIndex.GetLocalFrame();
				float num = value.X / 1528f / meshAtIndex.GetBoundingBoxWidth();
				float num2 = value.Y / 1528f / meshAtIndex.GetBoundingBoxHeight();
				Vec3 eulerAngles = localFrame.rotation.GetEulerAngles();
				localFrame.rotation = Mat3.Identity;
				localFrame.rotation.ApplyEulerAngles(eulerAngles);
				localFrame.rotation.ApplyScaleLocal(new Vec3(num, num2, 1f, -1f));
				meshAtIndex.SetLocalFrame(localFrame);
			}
		}

		public void SetUpdateRotationValueManual(ValueTuple<float, bool> value)
		{
			if (this._currentMultiMesh.MeshCount >= 1 && this._meshIndexToUpdate >= 0 && this._meshIndexToUpdate < this._currentMultiMesh.MeshCount)
			{
				Mesh meshAtIndex = this._currentMultiMesh.GetMeshAtIndex(this._meshIndexToUpdate);
				MatrixFrame localFrame = meshAtIndex.GetLocalFrame();
				float num = value.Item1 * 2f * 3.1415927f;
				Vec3 scaleVector = localFrame.rotation.GetScaleVector();
				localFrame.rotation = Mat3.Identity;
				localFrame.rotation.RotateAboutUp(num);
				localFrame.rotation.ApplyScaleLocal(scaleVector);
				if (value.Item2)
				{
					localFrame.rotation.RotateAboutForward(3.1415927f);
				}
				meshAtIndex.SetLocalFrame(localFrame);
			}
		}

		private bool _isFinalized;

		private bool _isEnabled;

		private bool _isNineGrid;

		private bool _isDirty;

		private Banner _banner;

		private int _latestWidth = -1;

		private int _latestHeight = -1;

		private int _tableauSizeX;

		private int _tableauSizeY;

		private float RenderScale = 1f;

		private float _customRenderScale = 1f;

		private Scene _scene;

		private Camera _defaultCamera;

		private Camera _nineGridCamera;

		private MetaMesh _currentMultiMesh;

		private GameEntity _currentMeshEntity;

		private int _meshIndexToUpdate;
	}
}
