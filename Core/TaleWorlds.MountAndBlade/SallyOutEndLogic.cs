using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class SallyOutEndLogic : MissionLogic
	{
		public bool IsSallyOutOver { get; private set; }

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

		private SallyOutEndLogic.EndConditionCheckState _checkState;

		private float _nextCheckTime;

		private float _dtSum;

		private enum EndConditionCheckState
		{
			Deactive,
			Active,
			Idle
		}
	}
}
