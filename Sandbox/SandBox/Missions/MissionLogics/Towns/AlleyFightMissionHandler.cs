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
	// Token: 0x0200005A RID: 90
	public class AlleyFightMissionHandler : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		// Token: 0x060003E2 RID: 994 RVA: 0x0001C092 File Offset: 0x0001A292
		public AlleyFightMissionHandler(TroopRoster playerSideTroops, TroopRoster rivalSideTroops)
		{
			this._playerSideTroops = playerSideTroops;
			this._rivalSideTroops = rivalSideTroops;
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0001C0C0 File Offset: 0x0001A2C0
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

		// Token: 0x060003E4 RID: 996 RVA: 0x0001C16C File Offset: 0x0001A36C
		public override void AfterStart()
		{
			base.Mission.Teams.Add(0, Clan.PlayerClan.Color, Clan.PlayerClan.Color2, Clan.PlayerClan.Banner, true, false, true);
			base.Mission.Teams.Add(1, Clan.BanditFactions.First<Clan>().Color, Clan.BanditFactions.First<Clan>().Color2, Clan.BanditFactions.First<Clan>().Banner, true, false, true);
			base.Mission.PlayerTeam = base.Mission.DefenderTeam;
			base.Mission.AddTroopsToDeploymentPlan(0, 0, 0, this._playerSideTroops.TotalManCount, 0);
			base.Mission.AddTroopsToDeploymentPlan(1, 0, 0, this._rivalSideTroops.TotalManCount, 0);
			base.Mission.MakeDefaultDeploymentPlans();
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0001C244 File Offset: 0x0001A444
		public override InquiryData OnEndMissionRequest(out bool canLeave)
		{
			canLeave = true;
			return new InquiryData("", GameTexts.FindText("str_give_up_fight", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(base.Mission.OnEndMissionResult), null, "", 0f, null, null, null);
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x0001C2AF File Offset: 0x0001A4AF
		public override void OnRetreatMission()
		{
			Campaign.Current.GetCampaignBehavior<IAlleyCampaignBehavior>().OnPlayerRetreatedFromMission();
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0001C2C0 File Offset: 0x0001A4C0
		public override void OnRenderingStarted()
		{
			Mission.Current.SetMissionMode(2, true);
			this.SpawnAgentsForBothSides();
			base.Mission.PlayerTeam.PlayerOrderController.SelectAllFormations(false);
			base.Mission.PlayerTeam.PlayerOrderController.SetOrder(4);
			base.Mission.PlayerEnemyTeam.MasterOrderController.SelectAllFormations(false);
			base.Mission.PlayerEnemyTeam.MasterOrderController.SetOrder(4);
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x0001C338 File Offset: 0x0001A538
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

		// Token: 0x060003E9 RID: 1001 RVA: 0x0001C428 File Offset: 0x0001A628
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

		// Token: 0x060003EA RID: 1002 RVA: 0x0001C4E1 File Offset: 0x0001A6E1
		public void StartSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x0001C4E3 File Offset: 0x0001A6E3
		public void StopSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0001C4E5 File Offset: 0x0001A6E5
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return true;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x0001C4E8 File Offset: 0x0001A6E8
		public bool IsSideDepleted(BattleSideEnum side)
		{
			if (side != 1)
			{
				return this._playerSideAliveAgents.Count == 0;
			}
			return this._rivalSideAliveAgents.Count == 0;
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x0001C50B File Offset: 0x0001A70B
		public float GetReinforcementInterval()
		{
			return float.MaxValue;
		}

		// Token: 0x040001D4 RID: 468
		private TroopRoster _playerSideTroops;

		// Token: 0x040001D5 RID: 469
		private TroopRoster _rivalSideTroops;

		// Token: 0x040001D6 RID: 470
		private List<Agent> _playerSideAliveAgents = new List<Agent>();

		// Token: 0x040001D7 RID: 471
		private List<Agent> _rivalSideAliveAgents = new List<Agent>();
	}
}
