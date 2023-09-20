using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Towns
{
	public class AlleyFightMissionHandler : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		public AlleyFightMissionHandler(TroopRoster playerSideTroops, TroopRoster rivalSideTroops)
		{
			this._playerSideTroops = playerSideTroops;
			this._rivalSideTroops = rivalSideTroops;
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (this._playerSideAliveAgents.Contains(affectedAgent))
			{
				this._playerSideAliveAgents.Remove(affectedAgent);
				this._playerSideTroops.RemoveTroop(affectedAgent.Character as CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			}
			else if (this._rivalSideAliveAgents.Contains(affectedAgent))
			{
				this._rivalSideAliveAgents.Remove(affectedAgent);
				this._rivalSideTroops.RemoveTroop(affectedAgent.Character as CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			}
			if (affectedAgent == Agent.Main)
			{
				Campaign.Current.GetCampaignBehavior<IAlleyCampaignBehavior>().OnPlayerDiedInMission();
			}
		}

		public override void AfterStart()
		{
			base.Mission.Teams.Add(0, Clan.PlayerClan.Color, Clan.PlayerClan.Color2, Clan.PlayerClan.Banner, true, false, true);
			base.Mission.Teams.Add(1, Clan.BanditFactions.First<Clan>().Color, Clan.BanditFactions.First<Clan>().Color2, Clan.BanditFactions.First<Clan>().Banner, true, false, true);
			base.Mission.PlayerTeam = base.Mission.DefenderTeam;
			base.Mission.AddTroopsToDeploymentPlan(0, 0, 0, this._playerSideTroops.TotalManCount, 0);
			base.Mission.AddTroopsToDeploymentPlan(1, 0, 0, this._rivalSideTroops.TotalManCount, 0);
			base.Mission.MakeDefaultDeploymentPlans();
		}

		public override InquiryData OnEndMissionRequest(out bool canLeave)
		{
			canLeave = true;
			return new InquiryData("", GameTexts.FindText("str_give_up_fight", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(base.Mission.OnEndMissionResult), null, "", 0f, null, null, null);
		}

		public override void OnRetreatMission()
		{
			Campaign.Current.GetCampaignBehavior<IAlleyCampaignBehavior>().OnPlayerRetreatedFromMission();
		}

		public override void OnRenderingStarted()
		{
			Mission.Current.SetMissionMode(2, true);
			this.SpawnAgentsForBothSides();
			base.Mission.PlayerTeam.PlayerOrderController.SelectAllFormations(false);
			base.Mission.PlayerTeam.PlayerOrderController.SetOrder(4);
			base.Mission.PlayerEnemyTeam.MasterOrderController.SelectAllFormations(false);
			base.Mission.PlayerEnemyTeam.MasterOrderController.SetOrder(4);
		}

		private void SpawnAgentsForBothSides()
		{
			Mission.Current.PlayerEnemyTeam.SetIsEnemyOf(Mission.Current.PlayerTeam, true);
			foreach (TroopRosterElement troopRosterElement in this._playerSideTroops.GetTroopRoster())
			{
				for (int i = 0; i < troopRosterElement.Number; i++)
				{
					this.SpawnATroop(troopRosterElement.Character, true);
				}
			}
			foreach (TroopRosterElement troopRosterElement2 in this._rivalSideTroops.GetTroopRoster())
			{
				for (int j = 0; j < troopRosterElement2.Number; j++)
				{
					this.SpawnATroop(troopRosterElement2.Character, false);
				}
			}
		}

		private void SpawnATroop(CharacterObject character, bool isPlayerSide)
		{
			SimpleAgentOrigin simpleAgentOrigin = new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor));
			Agent agent = Mission.Current.SpawnTroop(simpleAgentOrigin, isPlayerSide, true, false, false, 0, 0, true, true, true, null, null, null, null, 10, false);
			if (isPlayerSide)
			{
				this._playerSideAliveAgents.Add(agent);
			}
			else
			{
				this._rivalSideAliveAgents.Add(agent);
			}
			AgentFlag agentFlags = agent.GetAgentFlags();
			agent.SetAgentFlags((agentFlags | 65536) & -1048577);
			if (agent.IsAIControlled)
			{
				agent.SetWatchState(2);
			}
			if (isPlayerSide)
			{
				agent.SetTeam(Mission.Current.PlayerTeam, true);
				return;
			}
			agent.SetTeam(Mission.Current.PlayerEnemyTeam, true);
		}

		public void StartSpawner(BattleSideEnum side)
		{
		}

		public void StopSpawner(BattleSideEnum side)
		{
		}

		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return true;
		}

		public bool IsSideDepleted(BattleSideEnum side)
		{
			if (side != 1)
			{
				return this._playerSideAliveAgents.Count == 0;
			}
			return this._rivalSideAliveAgents.Count == 0;
		}

		public float GetReinforcementInterval()
		{
			return float.MaxValue;
		}

		private TroopRoster _playerSideTroops;

		private TroopRoster _rivalSideTroops;

		private List<Agent> _playerSideAliveAgents = new List<Agent>();

		private List<Agent> _rivalSideAliveAgents = new List<Agent>();
	}
}
