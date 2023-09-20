using System;
using SandBox.Missions.AgentControllers;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	// Token: 0x0200000F RID: 15
	public class MissionAmbushIntroView : MissionView
	{
		// Token: 0x06000057 RID: 87 RVA: 0x000044A0 File Offset: 0x000026A0
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

		// Token: 0x06000058 RID: 88 RVA: 0x00004574 File Offset: 0x00002774
		public void StartIntro()
		{
			this._started = true;
			this._camera = Camera.CreateCamera();
			this._camera.FillParametersFrom(base.MissionScreen.CombatCamera);
			this._camera.Frame = this._cameraStart;
			base.MissionScreen.CustomCamera = this._camera;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000045CC File Offset: 0x000027CC
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

		// Token: 0x0600005A RID: 90 RVA: 0x00004691 File Offset: 0x00002891
		private void CleanUp()
		{
			base.MissionScreen.CustomCamera = null;
			this.IntroEndAction();
			base.Mission.RemoveMissionBehavior(this);
		}

		// Token: 0x0400001B RID: 27
		private AmbushMissionController _ambushMissionController;

		// Token: 0x0400001C RID: 28
		private AmbushIntroLogic _ambushIntroLogic;

		// Token: 0x0400001D RID: 29
		private bool _isPlayerAmbusher;

		// Token: 0x0400001E RID: 30
		private MatrixFrame _cameraStart;

		// Token: 0x0400001F RID: 31
		private MatrixFrame _cameraEnd;

		// Token: 0x04000020 RID: 32
		private float _cameraMoveSpeed = 0.1f;

		// Token: 0x04000021 RID: 33
		private float _cameraLerping;

		// Token: 0x04000022 RID: 34
		private Camera _camera;

		// Token: 0x04000023 RID: 35
		public Action IntroEndAction;

		// Token: 0x04000024 RID: 36
		private bool _started;

		// Token: 0x04000025 RID: 37
		private bool _firstTick = true;
	}
}
