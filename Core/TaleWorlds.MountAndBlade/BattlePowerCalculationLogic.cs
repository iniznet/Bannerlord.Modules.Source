using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class BattlePowerCalculationLogic : MissionLogic
	{
		public bool IsTeamPowersCalculated { get; private set; }

		public BattlePowerCalculationLogic()
		{
			this._sidePowerData = new Dictionary<Team, float>[2];
			for (int i = 0; i < 2; i++)
			{
				this._sidePowerData[i] = new Dictionary<Team, float>();
			}
			this.IsTeamPowersCalculated = false;
		}

		public float GetTotalTeamPower(Team team)
		{
			if (!this.IsTeamPowersCalculated)
			{
				this.CalculateTeamPowers();
			}
			return this._sidePowerData[(int)team.Side][team];
		}

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

		private Dictionary<Team, float>[] _sidePowerData;
	}
}
