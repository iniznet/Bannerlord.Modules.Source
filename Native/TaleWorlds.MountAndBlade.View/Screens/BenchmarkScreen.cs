using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x02000028 RID: 40
	public class BenchmarkScreen : ScreenBase
	{
		// Token: 0x060001AD RID: 429 RVA: 0x0000EE28 File Offset: 0x0000D028
		protected override void OnActivate()
		{
			base.OnActivate();
			this._scene = Scene.CreateNewScene(true, true, 0, "mono_renderscene");
			this._scene.SetName("BenchmarkScreen");
			this._scene.Read("benchmark");
			this._cameraFrame = this._scene.ReadAndCalculateInitialCamera();
			this._scene.SetUseConstantTime(true);
			this._sceneView = SceneView.CreateSceneView();
			this._sceneView.SetScene(this._scene);
			this._sceneView.SetSceneUsesShadows(true);
			this._camera = Camera.CreateCamera();
			this.UpdateCamera();
			this._cameraTimer = new Timer(MBCommon.GetApplicationTime() - 5f, 5f, true);
			GameEntity gameEntity = this._scene.FindEntityWithName("LocationEntityParent");
			this._cameraLocationEntities = gameEntity.GetChildren().ToList<GameEntity>();
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000EF02 File Offset: 0x0000D102
		public void UpdateCamera()
		{
			this._camera.Frame = this._cameraFrame;
			this._sceneView.SetCamera(this._camera);
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000EF26 File Offset: 0x0000D126
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._scene = null;
			this._analyzer = null;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000EF3C File Offset: 0x0000D13C
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._cameraTimer.Check(MBCommon.GetApplicationTime()))
			{
				this._currentEntityIndex++;
				if (this._currentEntityIndex >= this._cameraLocationEntities.Count)
				{
					this._analyzer.FinalizeAndWrite("../../../Tools/TestAutomation/Attachments/benchmark_scene_performance.xml");
					ScreenManager.PopScreen();
					return;
				}
				GameEntity gameEntity = this._cameraLocationEntities[this._currentEntityIndex];
				this._cameraFrame = gameEntity.GetGlobalFrame();
				this.UpdateCamera();
				this._analyzer.Start(gameEntity.Name);
				this._cameraTimer.Reset(MBCommon.GetApplicationTime());
			}
			this._analyzer.Tick(dt);
		}

		// Token: 0x04000113 RID: 275
		private SceneView _sceneView;

		// Token: 0x04000114 RID: 276
		private Scene _scene;

		// Token: 0x04000115 RID: 277
		private Camera _camera;

		// Token: 0x04000116 RID: 278
		private MatrixFrame _cameraFrame;

		// Token: 0x04000117 RID: 279
		private Timer _cameraTimer;

		// Token: 0x04000118 RID: 280
		private const string _parentEntityName = "LocationEntityParent";

		// Token: 0x04000119 RID: 281
		private const string _sceneName = "benchmark";

		// Token: 0x0400011A RID: 282
		private const string _xmlPath = "../../../Tools/TestAutomation/Attachments/benchmark_scene_performance.xml";

		// Token: 0x0400011B RID: 283
		private List<GameEntity> _cameraLocationEntities;

		// Token: 0x0400011C RID: 284
		private int _currentEntityIndex = -1;

		// Token: 0x0400011D RID: 285
		private PerformanceAnalyzer _analyzer = new PerformanceAnalyzer();
	}
}
