using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000264 RID: 612
	public class BattlePowerCalculationLogic : MissionLogic
	{
		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x060020D0 RID: 8400 RVA: 0x00075858 File Offset: 0x00073A58
		// (set) Token: 0x060020D1 RID: 8401 RVA: 0x00075860 File Offset: 0x00073A60
		public bool IsTeamPowersCalculated { get; private set; }

		// Token: 0x060020D2 RID: 8402 RVA: 0x0007586C File Offset: 0x00073A6C
		public BattlePowerCalculationLogic()
		{
			this._sidePowerData = new Dictionary<Team, float>[2];
			for (int i = 0; i < 2; i++)
			{
				this._sidePowerData[i] = new Dictionary<Team, float>();
			}
			this.IsTeamPowersCalculated = false;
		}

		// Token: 0x060020D3 RID: 8403 RVA: 0x000758AB File Offset: 0x00073AAB
		public float GetTotalTeamPower(Team team)
		{
			if (!this.IsTeamPowersCalculated)
			{
				this.CalculateTeamPowers();
			}
			return this._sidePowerData[(int)team.Side][team];
		}

		// Token: 0x060020D4 RID: 8404 RVA: 0x000758D0 File Offset: 0x00073AD0
		private void CalculateTeamPowers()
		{
			Mission.TeamCollection teams = base.Mission.Teams;
			foreach (Team team in teams)
			{
				this._sidePowerData[(int)team.Side].Add(team, 0f);
			}
			MissionAgentSpawnLogic missionBehavior = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			for (int i = 0; i < 2; i++)
			{
				BattleSideEnum battleSideEnum = (BattleSideEnum)i;
				IEnumerable<IAgentOriginBase> allTroopsForSide = missionBehavior.GetAllTroopsForSide(battleSideEnum);
				Dictionary<Team, float> dictionary = this._sidePowerData[i];
				bool flag = base.Mission.PlayerTeam != null && base.Mission.PlayerTeam.Side == battleSideEnum;
				foreach (IAgentOriginBase agentOriginBase in allTroopsForSide)
				{
					Team agentTeam = Mission.GetAgentTeam(agentOriginBase, flag);
					BasicCharacterObject troop = agentOriginBase.Troop;
					Dictionary<Team, float> dictionary2 = dictionary;
					Team team2 = agentTeam;
					dictionary2[team2] += troop.GetPower();
				}
			}
			foreach (Team team3 in teams)
			{
				team3.QuerySystem.Expire();
			}
			this.IsTeamPowersCalculated = true;
		}

		// Token: 0x04000C1B RID: 3099
		private Dictionary<Team, float>[] _sidePowerData;
	}
}
