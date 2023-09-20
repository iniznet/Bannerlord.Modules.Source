using System;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	// Token: 0x02000018 RID: 24
	public class MissionHideoutCinematicView : MissionView
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00008BC8 File Offset: 0x00006DC8
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (!this._isInitialized)
			{
				this.InitializeView();
				return;
			}
			if (!Game.Current.GameStateManager.ActiveStateDisabledByUser && (this._currentState == HideoutCinematicController.HideoutCinematicState.Cinematic || this._nextState == HideoutCinematicController.HideoutCinematicState.Cinematic))
			{
				this.UpdateCamera(dt);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00008C18 File Offset: 0x00006E18
		private void SetCameraFrame(Vec3 position, Vec3 direction, out MatrixFrame cameraFrame)
		{
			cameraFrame.origin = position;
			cameraFrame.rotation.s = Vec3.Side;
			cameraFrame.rotation.f = Vec3.Up;
			cameraFrame.rotation.u = -direction;
			cameraFrame.rotation.Orthonormalize();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00008C68 File Offset: 0x00006E68
		private void SetupCamera()
		{
			this._camera = Camera.CreateCamera();
			Camera combatCamera = base.MissionScreen.CombatCamera;
			if (combatCamera != null)
			{
				this._camera.FillParametersFrom(combatCamera);
			}
			else
			{
				Debug.FailedAssert("Combat camera is null.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Missions\\MissionHideoutCinematicView.cs", "SetupCamera", 66);
			}
			Vec3 vec;
			this._cinematicLogicController.GetBossStandingEyePosition(out vec);
			Vec3 vec2;
			this._cinematicLogicController.GetPlayerStandingEyePosition(out vec2);
			Vec3 vec3 = (vec - vec2).NormalizedCopy();
			float num;
			float num2;
			float num3;
			this._cinematicLogicController.GetScenePrefabParameters(out num, out num2, out num3);
			float num4 = num + num2 + 1.5f * num3;
			this._cameraSpeed = num4 / MathF.Max(this._cinematicLogicController.CinematicDuration, 0.1f);
			this._cameraMoveDir = -vec3;
			this.SetCameraFrame(vec, vec3, out this._cameraFrame);
			Vec3 vec4 = this._cameraFrame.origin + this._cameraOffset.x * this._cameraFrame.rotation.s + this._cameraOffset.y * this._cameraFrame.rotation.f + this._cameraOffset.z * this._cameraFrame.rotation.u;
			Vec3 vec5 = (vec - vec4).NormalizedCopy();
			this.SetCameraFrame(vec4, vec5, out this._cameraFrame);
			this._camera.Frame = this._cameraFrame;
			base.MissionScreen.CustomCamera = this._camera;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00008E00 File Offset: 0x00007000
		private void UpdateCamera(float dt)
		{
			Vec3 vec = this._cameraFrame.origin + this._cameraMoveDir * this._cameraSpeed * dt;
			Vec3 vec2;
			this._cinematicLogicController.GetBossStandingEyePosition(out vec2);
			Vec3 vec3 = (vec2 - vec).NormalizedCopy();
			this.SetCameraFrame(vec, vec3, out this._cameraFrame);
			this._camera.Frame = this._cameraFrame;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00008E71 File Offset: 0x00007071
		private void ReleaseCamera()
		{
			base.MissionScreen.UpdateFreeCamera(base.MissionScreen.CustomCamera.Frame);
			base.MissionScreen.CustomCamera = null;
			this._camera.ReleaseCamera();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00008EA5 File Offset: 0x000070A5
		private void OnCinematicStateChanged(HideoutCinematicController.HideoutCinematicState state)
		{
			if (this._isInitialized)
			{
				this._currentState = state;
				if (this._currentState == HideoutCinematicController.HideoutCinematicState.PreCinematic)
				{
					this.SetupCamera();
					return;
				}
				if (this._currentState == HideoutCinematicController.HideoutCinematicState.PostCinematic)
				{
					this.ReleaseCamera();
				}
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00008ED5 File Offset: 0x000070D5
		private void OnCinematicTransition(HideoutCinematicController.HideoutCinematicState nextState, float duration)
		{
			if (this._isInitialized)
			{
				if (nextState == HideoutCinematicController.HideoutCinematicState.PreCinematic || nextState == HideoutCinematicController.HideoutCinematicState.PostCinematic)
				{
					this._cameraFadeViewController.BeginFadeOut(duration);
				}
				else if (nextState == HideoutCinematicController.HideoutCinematicState.Cinematic || nextState == HideoutCinematicController.HideoutCinematicState.Completed)
				{
					this._cameraFadeViewController.BeginFadeIn(duration);
				}
				this._nextState = nextState;
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00008F10 File Offset: 0x00007110
		private void InitializeView()
		{
			this._cinematicLogicController = base.Mission.GetMissionBehavior<HideoutCinematicController>();
			this._cameraFadeViewController = base.Mission.GetMissionBehavior<MissionCameraFadeView>();
			this._isInitialized = this._cinematicLogicController != null && this._cameraFadeViewController != null;
			HideoutCinematicController cinematicLogicController = this._cinematicLogicController;
			if (cinematicLogicController == null)
			{
				return;
			}
			cinematicLogicController.SetStateTransitionCallback(new HideoutCinematicController.OnHideoutCinematicStateChanged(this.OnCinematicStateChanged), new HideoutCinematicController.OnHideoutCinematicTransition(this.OnCinematicTransition));
		}

		// Token: 0x0400006A RID: 106
		private bool _isInitialized;

		// Token: 0x0400006B RID: 107
		private HideoutCinematicController _cinematicLogicController;

		// Token: 0x0400006C RID: 108
		private MissionCameraFadeView _cameraFadeViewController;

		// Token: 0x0400006D RID: 109
		private HideoutCinematicController.HideoutCinematicState _currentState;

		// Token: 0x0400006E RID: 110
		private HideoutCinematicController.HideoutCinematicState _nextState;

		// Token: 0x0400006F RID: 111
		private Camera _camera;

		// Token: 0x04000070 RID: 112
		private MatrixFrame _cameraFrame = MatrixFrame.Identity;

		// Token: 0x04000071 RID: 113
		private readonly Vec3 _cameraOffset = new Vec3(0.3f, 0.3f, 1.2f, -1f);

		// Token: 0x04000072 RID: 114
		private Vec3 _cameraMoveDir = Vec3.Forward;

		// Token: 0x04000073 RID: 115
		private float _cameraSpeed;
	}
}
