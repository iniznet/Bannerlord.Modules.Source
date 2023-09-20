using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	public class BenchmarkScreen : ScreenBase
	{
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

		public void UpdateCamera()
		{
			this._camera.Frame = this._cameraFrame;
			this._sceneView.SetCamera(this._camera);
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._scene = null;
			this._analyzer = null;
		}

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

		private SceneView _sceneView;

		private Scene _scene;

		private Camera _camera;

		private MatrixFrame _cameraFrame;

		private Timer _cameraTimer;

		private const string _parentEntityName = "LocationEntityParent";

		private const string _sceneName = "benchmark";

		private const string _xmlPath = "../../../Tools/TestAutomation/Attachments/benchmark_scene_performance.xml";

		private List<GameEntity> _cameraLocationEntities;

		private int _currentEntityIndex = -1;

		private PerformanceAnalyzer _analyzer = new PerformanceAnalyzer();
	}
}
