using System;
using System.Diagnostics;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200027A RID: 634
	public class RecordMissionLogic : MissionLogic
	{
		// Token: 0x060021CA RID: 8650 RVA: 0x0007B53F File Offset: 0x0007973F
		public override void OnBehaviorInitialize()
		{
			base.Mission.Recorder.StartRecording();
		}

		// Token: 0x060021CB RID: 8651 RVA: 0x0007B554 File Offset: 0x00079754
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._lastRecordedTime + 0.02f < base.Mission.CurrentTime)
			{
				this._lastRecordedTime = base.Mission.CurrentTime;
				base.Mission.Recorder.RecordCurrentState();
			}
		}

		// Token: 0x060021CC RID: 8652 RVA: 0x0007B5A4 File Offset: 0x000797A4
		public override void OnEndMissionInternal()
		{
			base.OnEndMissionInternal();
			base.Mission.Recorder.BackupRecordToFile("Mission_record_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}_", DateTime.Now) + Process.GetCurrentProcess().Id, Game.Current.GameType.GetType().Name, base.Mission.SceneLevels);
			GameNetwork.ResetMissionData();
		}

		// Token: 0x04000C9E RID: 3230
		private float _lastRecordedTime = -1f;
	}
}
