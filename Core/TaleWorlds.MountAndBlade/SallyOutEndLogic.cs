using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200027C RID: 636
	public class SallyOutEndLogic : MissionLogic
	{
		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x060021D3 RID: 8659 RVA: 0x0007B6C5 File Offset: 0x000798C5
		// (set) Token: 0x060021D4 RID: 8660 RVA: 0x0007B6CD File Offset: 0x000798CD
		public bool IsSallyOutOver { get; private set; }

		// Token: 0x060021D5 RID: 8661 RVA: 0x0007B6D8 File Offset: 0x000798D8
		public override void OnMissionTick(float dt)
		{
			if (this.CheckTimer(dt))
			{
				if (this._checkState == SallyOutEndLogic.EndConditionCheckState.Deactive)
				{
					using (IEnumerator<Team> enumerator = base.Mission.Teams.Where((Team t) => t.Side == BattleSideEnum.Defender).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Team team = enumerator.Current;
							foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
							{
								if (formation.CountOfUnits > 0 && formation.CountOfUnits > 0 && !TeamAISiegeComponent.IsFormationInsideCastle(formation, true, 0.1f))
								{
									this._checkState = SallyOutEndLogic.EndConditionCheckState.Active;
									return;
								}
							}
						}
						return;
					}
				}
				if (this._checkState == SallyOutEndLogic.EndConditionCheckState.Idle)
				{
					this._checkState = SallyOutEndLogic.EndConditionCheckState.Active;
				}
			}
		}

		// Token: 0x060021D6 RID: 8662 RVA: 0x0007B7D0 File Offset: 0x000799D0
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			if (this.IsSallyOutOver)
			{
				missionResult = MissionResult.CreateSuccessful(base.Mission, false);
				return true;
			}
			if (this._checkState != SallyOutEndLogic.EndConditionCheckState.Active)
			{
				return false;
			}
			foreach (Team team in base.Mission.Teams)
			{
				BattleSideEnum side = team.Side;
				if (side != BattleSideEnum.Defender)
				{
					if (side == BattleSideEnum.Attacker && TeamAISiegeComponent.IsFormationGroupInsideCastle(team.FormationsIncludingSpecialAndEmpty, false, 0.1f))
					{
						this._checkState = SallyOutEndLogic.EndConditionCheckState.Idle;
						return false;
					}
				}
				else if (team.FormationsIncludingEmpty.Any((Formation f) => f.CountOfUnits > 0 && !TeamAISiegeComponent.IsFormationInsideCastle(f, false, 0.9f)))
				{
					this._checkState = SallyOutEndLogic.EndConditionCheckState.Idle;
					return false;
				}
			}
			this.IsSallyOutOver = true;
			missionResult = MissionResult.CreateSuccessful(base.Mission, false);
			return true;
		}

		// Token: 0x060021D7 RID: 8663 RVA: 0x0007B8C0 File Offset: 0x00079AC0
		private bool CheckTimer(float dt)
		{
			this._dtSum += dt;
			if (this._dtSum < this._nextCheckTime)
			{
				return false;
			}
			this._dtSum = 0f;
			this._nextCheckTime = 0.8f + MBRandom.RandomFloat * 0.4f;
			return true;
		}

		// Token: 0x04000CA1 RID: 3233
		private SallyOutEndLogic.EndConditionCheckState _checkState;

		// Token: 0x04000CA3 RID: 3235
		private float _nextCheckTime;

		// Token: 0x04000CA4 RID: 3236
		private float _dtSum;

		// Token: 0x02000588 RID: 1416
		private enum EndConditionCheckState
		{
			// Token: 0x04001D79 RID: 7545
			Deactive,
			// Token: 0x04001D7A RID: 7546
			Active,
			// Token: 0x04001D7B RID: 7547
			Idle
		}
	}
}
