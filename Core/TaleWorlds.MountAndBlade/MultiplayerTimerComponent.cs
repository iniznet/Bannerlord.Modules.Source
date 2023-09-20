using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerTimerComponent : MissionNetwork
	{
		public bool IsTimerRunning { get; private set; }

		public void StartTimerAsServer(float duration)
		{
			this._missionTimer = new MissionTimer(duration);
			this.IsTimerRunning = true;
		}

		public void StartTimerAsClient(float startTime, float duration)
		{
			this._missionTimer = MissionTimer.CreateSynchedTimerClient(startTime, duration);
			this.IsTimerRunning = true;
		}

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

		public bool CheckIfTimerPassed()
		{
			return this.IsTimerRunning && this._missionTimer.Check(false);
		}

		public MissionTime GetCurrentTimerStartTime()
		{
			return this._missionTimer.GetStartTime();
		}

		private MissionTimer _missionTimer;
	}
}
