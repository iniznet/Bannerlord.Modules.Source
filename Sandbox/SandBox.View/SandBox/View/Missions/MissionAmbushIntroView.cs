using System;
using SandBox.Missions.AgentControllers;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	public class MissionAmbushIntroView : MissionView
	{
		public override void AfterStart()
		{
			base.AfterStart();
			this._ambushMissionController = base.Mission.GetMissionBehavior<AmbushMissionController>();
			this._ambushIntroLogic = base.Mission.GetMissionBehavior<AmbushIntroLogic>();
			this._isPlayerAmbusher = this._ambushMissionController.IsPlayerAmbusher;
			this._cameraStart = base.Mission.Scene.FindEntityWithTag(this._isPlayerAmbusher ? "intro_camera_attacker_start" : "intro_camera_defender_start").GetGlobalFrame();
			this._cameraEnd = base.Mission.Scene.FindEntityWithTag(this._isPlayerAmbusher ? "intro_camera_attacker_end" : "intro_camera_defender_end").GetGlobalFrame();
			this.IntroEndAction = new Action(this._ambushIntroLogic.OnIntroEnded);
			this._ambushIntroLogic.StartIntroAction = new Action(this.StartIntro);
		}

		public void StartIntro()
		{
			this._started = true;
			this._camera = Camera.CreateCamera();
			this._camera.FillParametersFrom(base.MissionScreen.CombatCamera);
			this._camera.Frame = this._cameraStart;
			base.MissionScreen.CustomCamera = this._camera;
		}

		public override void OnMissionTick(float dt)
		{
			if (this._firstTick)
			{
				this._firstTick = false;
			}
			if (!this._started)
			{
				return;
			}
			if (this._cameraLerping < 1f)
			{
				MatrixFrame matrixFrame;
				matrixFrame.origin = MBMath.Lerp(this._cameraStart.origin, this._cameraEnd.origin, this._cameraLerping, 1E-05f);
				matrixFrame.rotation = MBMath.Lerp(ref this._cameraStart.rotation, ref this._cameraEnd.rotation, this._cameraLerping, 1E-05f);
				this._camera.Frame = matrixFrame;
				this._cameraLerping += this._cameraMoveSpeed * dt;
				return;
			}
			this._camera.Frame = this._cameraEnd;
			this.CleanUp();
		}

		private void CleanUp()
		{
			base.MissionScreen.CustomCamera = null;
			this.IntroEndAction();
			base.Mission.RemoveMissionBehavior(this);
		}

		private AmbushMissionController _ambushMissionController;

		private AmbushIntroLogic _ambushIntroLogic;

		private bool _isPlayerAmbusher;

		private MatrixFrame _cameraStart;

		private MatrixFrame _cameraEnd;

		private float _cameraMoveSpeed = 0.1f;

		private float _cameraLerping;

		private Camera _camera;

		public Action IntroEndAction;

		private bool _started;

		private bool _firstTick = true;
	}
}
