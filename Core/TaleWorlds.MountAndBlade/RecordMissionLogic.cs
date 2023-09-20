using System;
using System.Diagnostics;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class RecordMissionLogic : MissionLogic
	{
		public override void OnBehaviorInitialize()
		{
			base.Mission.Recorder.StartRecording();
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._lastRecordedTime + 0.02f < base.Mission.CurrentTime)
			{
				this._lastRecordedTime = base.Mission.CurrentTime;
				base.Mission.Recorder.RecordCurrentState();
			}
		}

		public override void OnEndMissionInternal()
		{
			base.OnEndMissionInternal();
			base.Mission.Recorder.BackupRecordToFile("Mission_record_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}_", DateTime.Now) + Process.GetCurrentProcess().Id, Game.Current.GameType.GetType().Name, base.Mission.SceneLevels);
			GameNetwork.ResetMissionData();
		}

		private float _lastRecordedTime = -1f;
	}
}
