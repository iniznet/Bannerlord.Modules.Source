using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class CameraDisplay : ScriptComponentBehavior
	{
		private void BuildView()
		{
			this._sceneView = SceneView.CreateSceneView();
			this._myCamera = Camera.CreateCamera();
			this._sceneView.SetScene(base.GameEntity.Scene);
			this._sceneView.SetPostfxFromConfig();
			this._sceneView.SetRenderOption(View.ViewRenderOptions.ClearColor, false);
			this._sceneView.SetRenderOption(View.ViewRenderOptions.ClearDepth, true);
			this._sceneView.SetScale(new Vec2(0.2f, 0.2f));
		}

		private void SetCamera()
		{
			Vec2 realScreenResolution = Screen.RealScreenResolution;
			float num = realScreenResolution.x / realScreenResolution.y;
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			this._myCamera.SetFovVertical(0.7853982f, num, 0.2f, 200f);
			this._myCamera.Frame = globalFrame;
			this._sceneView.SetCamera(this._myCamera);
		}

		private void RenderCameraFrustrum()
		{
			this._myCamera.RenderFrustrum();
		}

		protected internal override void OnEditorInit()
		{
			this.BuildView();
		}

		protected internal override void OnInit()
		{
			this.BuildView();
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (MBEditor.IsEntitySelected(base.GameEntity))
			{
				this.RenderCameraFrustrum();
				this._sceneView.SetEnable(true);
				return;
			}
			this._sceneView.SetEnable(false);
		}

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			this._sceneView = null;
			this._myCamera = null;
		}

		private Camera _myCamera;

		private SceneView _sceneView;

		public int renderOrder;
	}
}
