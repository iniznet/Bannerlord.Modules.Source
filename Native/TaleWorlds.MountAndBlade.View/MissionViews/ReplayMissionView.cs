using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public class ReplayMissionView : MissionView
	{
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._resetTime = 0f;
			this._replayMissionLogic = base.Mission.GetMissionBehavior<ReplayMissionLogic>();
		}

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

		public void OverrideInput(bool isOverridden)
		{
			this._isInputOverridden = isOverridden;
		}

		public void ResetReplay()
		{
			this._resetTime = base.Mission.CurrentTime;
			base.Mission.ResetMission();
			base.Mission.Teams.Clear();
			base.Mission.Recorder.RestartRecord();
			MBCommon.UnPauseGameEngine();
			base.Mission.Scene.TimeSpeed = 1f;
		}

		public void Rewind(float time)
		{
			this._resetTime = MathF.Min(this._resetTime + time, base.Mission.CurrentTime);
			base.Mission.ResetMission();
			base.Mission.Teams.Clear();
			base.Mission.Recorder.RestartRecord();
		}

		public void FastForward(float time)
		{
			this._resetTime -= time;
		}

		public void Pause()
		{
			if (!MBCommon.IsPaused && MBMath.ApproximatelyEqualsTo(base.Mission.Scene.TimeSpeed, 1f, 1E-05f))
			{
				MBCommon.PauseGameEngine();
			}
		}

		public void Resume()
		{
			if (MBCommon.IsPaused || !MBMath.ApproximatelyEqualsTo(base.Mission.Scene.TimeSpeed, 1f, 1E-05f))
			{
				MBCommon.UnPauseGameEngine();
				base.Mission.Scene.TimeSpeed = 1f;
			}
		}

		private float _resetTime;

		private bool _isInputOverridden;

		private ReplayMissionLogic _replayMissionLogic;
	}
}
