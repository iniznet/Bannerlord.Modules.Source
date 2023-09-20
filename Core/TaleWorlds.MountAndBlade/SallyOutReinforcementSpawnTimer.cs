using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class SallyOutReinforcementSpawnTimer : ICustomReinforcementSpawnTimer
	{
		public SallyOutReinforcementSpawnTimer(float besiegedInterval, float besiegerInterval, float besiegerIntervalChange, int besiegerIntervalChangeCount)
		{
			this._besiegedSideTimer = new BasicMissionTimer();
			this._besiegedInterval = besiegedInterval;
			this._besiegerSideTimer = new BasicMissionTimer();
			this._besiegerInterval = besiegerInterval;
			this._besiegerIntervalChange = besiegerIntervalChange;
			this._besiegerRemainingIntervalChanges = besiegerIntervalChangeCount;
		}

		public bool Check(BattleSideEnum side)
		{
			if (side == BattleSideEnum.Attacker)
			{
				if (this._besiegerSideTimer.ElapsedTime >= this._besiegerInterval)
				{
					if (this._besiegerRemainingIntervalChanges > 0)
					{
						this._besiegerInterval -= this._besiegerIntervalChange;
						this._besiegerRemainingIntervalChanges--;
					}
					this._besiegerSideTimer.Reset();
					return true;
				}
			}
			else if (side == BattleSideEnum.Defender && this._besiegedSideTimer.ElapsedTime >= this._besiegedInterval)
			{
				this._besiegedSideTimer.Reset();
				return true;
			}
			return false;
		}

		public void ResetTimer(BattleSideEnum side)
		{
			if (side == BattleSideEnum.Attacker)
			{
				this._besiegerSideTimer.Reset();
				return;
			}
			if (side == BattleSideEnum.Defender)
			{
				this._besiegedSideTimer.Reset();
			}
		}

		private BasicMissionTimer _besiegedSideTimer;

		private BasicMissionTimer _besiegerSideTimer;

		private float _besiegedInterval;

		private float _besiegerInterval;

		private float _besiegerIntervalChange;

		private int _besiegerRemainingIntervalChanges;
	}
}
