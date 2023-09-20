using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000333 RID: 819
	public class CameraDisplay : ScriptComponentBehavior
	{
		// Token: 0x06002C27 RID: 11303 RVA: 0x000AB6F8 File Offset: 0x000A98F8
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

		// Token: 0x06002C28 RID: 11304 RVA: 0x000AB770 File Offset: 0x000A9970
		private void SetCamera()
		{
			Vec2 realScreenResolution = Screen.RealScreenResolution;
			float num = realScreenResolution.x / realScreenResolution.y;
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			this._myCamera.SetFovVertical(0.7853982f, num, 0.2f, 200f);
			this._myCamera.Frame = globalFrame;
			this._sceneView.SetCamera(this._myCamera);
		}

		// Token: 0x06002C29 RID: 11305 RVA: 0x000AB7D5 File Offset: 0x000A99D5
		private void RenderCameraFrustrum()
		{
			this._myCamera.RenderFrustrum();
		}

		// Token: 0x06002C2A RID: 11306 RVA: 0x000AB7E2 File Offset: 0x000A99E2
		protected internal override void OnEditorInit()
		{
			this.BuildView();
		}

		// Token: 0x06002C2B RID: 11307 RVA: 0x000AB7EA File Offset: 0x000A99EA
		protected internal override void OnInit()
		{
			this.BuildView();
		}

		// Token: 0x06002C2C RID: 11308 RVA: 0x000AB7F2 File Offset: 0x000A99F2
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

		// Token: 0x06002C2D RID: 11309 RVA: 0x000AB827 File Offset: 0x000A9A27
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			this._sceneView = null;
			this._myCamera = null;
		}

		// Token: 0x040010BC RID: 4284
		private Camera _myCamera;

		// Token: 0x040010BD RID: 4285
		private SceneView _sceneView;

		// Token: 0x040010BE RID: 4286
		public int renderOrder;
	}
}
