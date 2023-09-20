using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x02000054 RID: 84
	public class ReplayMissionView : MissionView
	{
		// Token: 0x060003AD RID: 941 RVA: 0x0001F67B File Offset: 0x0001D87B
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._resetTime = 0f;
			this._replayMissionLogic = base.Mission.GetMissionBehavior<ReplayMissionLogic>();
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0001F6A0 File Offset: 0x0001D8A0
		public override void OnPreMissionTick(float dt)
		{
			base.OnPreMissionTick(dt);
			base.Mission.Recorder.ProcessRecordUntilTime(base.Mission.CurrentTime - this._resetTime);
			bool isInputOverridden = this._isInputOverridden;
			if (base.Mission.CurrentState == 2 && base.Mission.Recorder.IsEndOfRecord())
			{
				if (MBEditor._isEditorMissionOn)
				{
					MBEditor.LeaveEditMissionMode();
					return;
				}
				base.Mission.EndMission();
			}
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0001F715 File Offset: 0x0001D915
		public void OverrideInput(bool isOverridden)
		{
			this._isInputOverridden = isOverridden;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x0001F720 File Offset: 0x0001D920
		public void ResetReplay()
		{
			this._resetTime = base.Mission.CurrentTime;
			base.Mission.ResetMission();
			base.Mission.Teams.Clear();
			base.Mission.Recorder.RestartRecord();
			MBCommon.UnPauseGameEngine();
			base.Mission.Scene.TimeSpeed = 1f;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0001F784 File Offset: 0x0001D984
		public void Rewind(float time)
		{
			this._resetTime = MathF.Min(this._resetTime + time, base.Mission.CurrentTime);
			base.Mission.ResetMission();
			base.Mission.Teams.Clear();
			base.Mission.Recorder.RestartRecord();
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0001F7DA File Offset: 0x0001D9DA
		public void FastForward(float time)
		{
			this._resetTime -= time;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0001F7EA File Offset: 0x0001D9EA
		public void Pause()
		{
			if (!MBCommon.IsPaused && MBMath.ApproximatelyEqualsTo(base.Mission.Scene.TimeSpeed, 1f, 1E-05f))
			{
				MBCommon.PauseGameEngine();
			}
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0001F81C File Offset: 0x0001DA1C
		public void Resume()
		{
			if (MBCommon.IsPaused || !MBMath.ApproximatelyEqualsTo(base.Mission.Scene.TimeSpeed, 1f, 1E-05f))
			{
				MBCommon.UnPauseGameEngine();
				base.Mission.Scene.TimeSpeed = 1f;
			}
		}

		// Token: 0x04000267 RID: 615
		private float _resetTime;

		// Token: 0x04000268 RID: 616
		private bool _isInputOverridden;

		// Token: 0x04000269 RID: 617
		private ReplayMissionLogic _replayMissionLogic;
	}
}
