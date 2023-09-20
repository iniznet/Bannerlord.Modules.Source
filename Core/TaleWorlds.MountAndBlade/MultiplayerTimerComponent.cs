using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002AF RID: 687
	public class MultiplayerTimerComponent : MissionNetwork
	{
		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x06002600 RID: 9728 RVA: 0x000902CC File Offset: 0x0008E4CC
		// (set) Token: 0x06002601 RID: 9729 RVA: 0x000902D4 File Offset: 0x0008E4D4
		public bool IsTimerRunning { get; private set; }

		// Token: 0x06002602 RID: 9730 RVA: 0x000902DD File Offset: 0x0008E4DD
		public void StartTimerAsServer(float duration)
		{
			this._missionTimer = new MissionTimer(duration);
			this.IsTimerRunning = true;
		}

		// Token: 0x06002603 RID: 9731 RVA: 0x000902F2 File Offset: 0x0008E4F2
		public void StartTimerAsClient(float startTime, float duration)
		{
			this._missionTimer = MissionTimer.CreateSynchedTimerClient(startTime, duration);
			this.IsTimerRunning = true;
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x00090308 File Offset: 0x0008E508
		public float GetRemainingTime(bool isSynched)
		{
			if (!this.IsTimerRunning)
			{
				return 0f;
			}
			float remainingTimeInSeconds = this._missionTimer.GetRemainingTimeInSeconds(isSynched);
			if (isSynched)
			{
				return MathF.Min(remainingTimeInSeconds, this._missionTimer.GetTimerDuration());
			}
			return remainingTimeInSeconds;
		}

		// Token: 0x06002605 RID: 9733 RVA: 0x00090346 File Offset: 0x0008E546
		public bool CheckIfTimerPassed()
		{
			return this.IsTimerRunning && this._missionTimer.Check(false);
		}

		// Token: 0x06002606 RID: 9734 RVA: 0x0009035E File Offset: 0x0008E55E
		public MissionTime GetCurrentTimerStartTime()
		{
			return this._missionTimer.GetStartTime();
		}

		// Token: 0x04000E19 RID: 3609
		private MissionTimer _missionTimer;
	}
}
