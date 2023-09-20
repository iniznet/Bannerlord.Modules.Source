using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class BattleReinforcementsSpawnController : MissionLogic
	{
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<IMissionAgentSpawnLogic>();
		}

		public override void AfterStart()
		{
			foreach (Team team in base.Mission.Teams)
			{
				foreach (Formation formation in team.FormationsIncludingEmpty)
				{
					formation.OnBeforeMovementOrderApplied += this.OnBeforeFormationMovementOrderApplied;
				}
			}
		}

		public override void OnMissionTick(float dt)
		{
			for (int i = 0; i < 2; i++)
			{
				if (this._sideRequiresUpdate[i])
				{
					this.UpdateSide((BattleSideEnum)i);
					this._sideRequiresUpdate[i] = false;
				}
			}
		}

		protected override void OnEndMission()
		{
			foreach (Team team in base.Mission.Teams)
			{
				foreach (Formation formation in team.FormationsIncludingEmpty)
				{
					formation.OnBeforeMovementOrderApplied -= this.OnBeforeFormationMovementOrderApplied;
				}
			}
		}

		private void UpdateSide(BattleSideEnum side)
		{
			if (this.IsBattleSideRetreating(side))
			{
				if (!this._sideReinforcementSuspended[(int)side] && this._missionAgentSpawnLogic.IsSideSpawnEnabled(side))
				{
					this._missionAgentSpawnLogic.StopSpawner(side);
					this._sideReinforcementSuspended[(int)side] = true;
					return;
				}
			}
			else if (this._sideReinforcementSuspended[(int)side])
			{
				this._missionAgentSpawnLogic.StartSpawner(side);
				this._sideReinforcementSuspended[(int)side] = false;
			}
		}

		private bool IsBattleSideRetreating(BattleSideEnum side)
		{
			bool flag = true;
			foreach (Team team in base.Mission.Teams)
			{
				if (team.Side == side)
				{
					foreach (Formation formation in team.FormationsIncludingEmpty)
					{
						if (formation.CountOfUnits > 0 && formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Retreat)
						{
							flag = false;
							break;
						}
					}
				}
			}
			return flag;
		}

		private unsafe void OnBeforeFormationMovementOrderApplied(Formation formation, MovementOrder.MovementOrderEnum orderEnum)
		{
			if (formation.GetReadonlyMovementOrderReference()->OrderEnum == MovementOrder.MovementOrderEnum.Retreat || orderEnum == MovementOrder.MovementOrderEnum.Retreat)
			{
				int side = (int)formation.Team.Side;
				this._sideRequiresUpdate[side] = true;
			}
		}

		private IMissionAgentSpawnLogic _missionAgentSpawnLogic;

		private bool[] _sideReinforcementSuspended = new bool[2];

		private bool[] _sideRequiresUpdate = new bool[2];
	}
}
