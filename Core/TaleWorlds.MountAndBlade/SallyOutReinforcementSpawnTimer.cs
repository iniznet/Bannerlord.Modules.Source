using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200027F RID: 639
	public class SallyOutReinforcementSpawnTimer : ICustomReinforcementSpawnTimer
	{
		// Token: 0x060021FA RID: 8698 RVA: 0x0007C4A9 File Offset: 0x0007A6A9
		public SallyOutReinforcementSpawnTimer(float besiegedInterval, float besiegerInterval, float besiegerIntervalChange, int besiegerIntervalChangeCount)
		{
			this._besiegedSideTimer = new BasicMissionTimer();
			this._besiegedInterval = besiegedInterval;
			this._besiegerSideTimer = new BasicMissionTimer();
			this._besiegerInterval = besiegerInterval;
			this._besiegerIntervalChange = besiegerIntervalChange;
			this._besiegerRemainingIntervalChanges = besiegerIntervalChangeCount;
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x0007C4E4 File Offset: 0x0007A6E4
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

		// Token: 0x060021FC RID: 8700 RVA: 0x0007C563 File Offset: 0x0007A763
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

		// Token: 0x04000CC1 RID: 3265
		private BasicMissionTimer _besiegedSideTimer;

		// Token: 0x04000CC2 RID: 3266
		private BasicMissionTimer _besiegerSideTimer;

		// Token: 0x04000CC3 RID: 3267
		private float _besiegedInterval;

		// Token: 0x04000CC4 RID: 3268
		private float _besiegerInterval;

		// Token: 0x04000CC5 RID: 3269
		private float _besiegerIntervalChange;

		// Token: 0x04000CC6 RID: 3270
		private int _besiegerRemainingIntervalChanges;
	}
}
