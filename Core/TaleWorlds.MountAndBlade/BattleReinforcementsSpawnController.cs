using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000265 RID: 613
	public class BattleReinforcementsSpawnController : MissionLogic
	{
		// Token: 0x060020D5 RID: 8405 RVA: 0x00075A48 File Offset: 0x00073C48
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<IMissionAgentSpawnLogic>();
		}

		// Token: 0x060020D6 RID: 8406 RVA: 0x00075A64 File Offset: 0x00073C64
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

		// Token: 0x060020D7 RID: 8407 RVA: 0x00075B00 File Offset: 0x00073D00
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

		// Token: 0x060020D8 RID: 8408 RVA: 0x00075B34 File Offset: 0x00073D34
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

		// Token: 0x060020D9 RID: 8409 RVA: 0x00075BD0 File Offset: 0x00073DD0
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

		// Token: 0x060020DA RID: 8410 RVA: 0x00075C38 File Offset: 0x00073E38
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

		// Token: 0x060020DB RID: 8411 RVA: 0x00075CF0 File Offset: 0x00073EF0
		private unsafe void OnBeforeFormationMovementOrderApplied(Formation formation, MovementOrder.MovementOrderEnum orderEnum)
		{
			if (formation.GetReadonlyMovementOrderReference()->OrderEnum == MovementOrder.MovementOrderEnum.Retreat || orderEnum == MovementOrder.MovementOrderEnum.Retreat)
			{
				int side = (int)formation.Team.Side;
				this._sideRequiresUpdate[side] = true;
			}
		}

		// Token: 0x04000C1C RID: 3100
		private IMissionAgentSpawnLogic _missionAgentSpawnLogic;

		// Token: 0x04000C1D RID: 3101
		private bool[] _sideReinforcementSuspended = new bool[2];

		// Token: 0x04000C1E RID: 3102
		private bool[] _sideRequiresUpdate = new bool[2];
	}
}
