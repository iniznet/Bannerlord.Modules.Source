using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Towns
{
	// Token: 0x0200005C RID: 92
	public class TownAmbushMissionController : MissionLogic
	{
		// Token: 0x06000406 RID: 1030 RVA: 0x0001CFF2 File Offset: 0x0001B1F2
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0001D00C File Offset: 0x0001B20C
		public override void AfterStart()
		{
			base.Mission.SetMissionMode(0, true);
			this._missionAgentHandler.SpawnPlayer(false, true, false, false, false, "");
			this._missionAgentHandler.SpawnThugs();
			this._missionAgentHandler.SpawnGuards();
			foreach (Agent agent in base.Mission.Agents)
			{
				CharacterObject characterObject = (CharacterObject)agent.Character;
				if (agent.IsEnemyOf(base.Mission.MainAgent) && agent.IsHuman)
				{
					agent.SetWatchState(2);
					agent.SetTeam(base.Mission.PlayerEnemyTeam, true);
					this._enemiesAliveCount++;
				}
				else if (agent.IsHuman && !agent.IsEnemyOf(base.Mission.MainAgent) && (characterObject.Occupation == 2 || characterObject.Occupation == 24))
				{
					this._guardRaycheckTimers.Add(agent, new Timer(base.Mission.CurrentTime, 2f, true));
				}
			}
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0001D13C File Offset: 0x0001B33C
		public override void OnMissionTick(float dt)
		{
			if (base.Mission.MainAgent == null)
			{
				return;
			}
			List<Agent> list = new List<Agent>();
			foreach (Agent agent in base.Mission.Agents)
			{
				CharacterObject characterObject = (CharacterObject)agent.Character;
				if ((characterObject.Occupation == 24 || characterObject.Occupation == 2) && agent.IsHuman && !agent.IsEnemyOf(base.Mission.MainAgent) && agent.CurrentWatchState != 2 && this.GuardCanSeeThePlayer(agent, ref list))
				{
					agent.SetTeam(base.Mission.PlayerTeam, true);
					agent.SetWatchState(2);
					agent.WieldInitialWeapons(2);
					agent.DisableScriptedMovement();
				}
			}
			foreach (Agent agent2 in list)
			{
				this._guardRaycheckTimers.Remove(agent2);
			}
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x0001D260 File Offset: 0x0001B460
		private bool GuardCanSeeThePlayer(Agent guard, ref List<Agent> alarmedGuards)
		{
			if (guard.Position.DistanceSquared(Agent.Main.Position) <= 225f)
			{
				Timer timer = this._guardRaycheckTimers[guard];
				if (timer.Check(base.Mission.CurrentTime))
				{
					timer.Reset(base.Mission.CurrentTime);
					Vec3 eyeGlobalPosition = guard.GetEyeGlobalPosition();
					Vec3 vec = (Agent.Main.GetEyeGlobalPosition() - eyeGlobalPosition).NormalizedCopy();
					float num = 15f;
					float num2;
					if (base.Mission.Scene.RayCastForClosestEntityOrTerrain(eyeGlobalPosition, eyeGlobalPosition + vec * (num + 0.01f), ref num2, 0.01f, 6404041))
					{
						num = num2;
					}
					if (base.Mission.RayCastForClosestAgent(eyeGlobalPosition, eyeGlobalPosition + vec * (num + 0.01f), ref num2, guard.Index, 0.01f) == Agent.Main)
					{
						alarmedGuards.Add(guard);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x0001D360 File Offset: 0x0001B560
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectedAgent.IsEnemyOf(base.Mission.MainAgent))
			{
				this._enemiesAliveCount--;
				foreach (LocationCharacter locationCharacter in CampaignMission.Current.Location.GetCharacterList())
				{
					if (locationCharacter.Character == affectedAgent.Character)
					{
						CampaignMission.Current.Location.RemoveLocationCharacter(locationCharacter);
						break;
					}
				}
			}
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x0001D3F0 File Offset: 0x0001B5F0
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			if (this._enemiesAliveCount == 0)
			{
				missionResult = MissionResult.CreateSuccessful(base.Mission, false);
				return true;
			}
			if (this.IsPlayerDead())
			{
				missionResult = MissionResult.CreateDefeated(base.Mission);
				return true;
			}
			return false;
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0001D422 File Offset: 0x0001B622
		public bool IsPlayerDead()
		{
			return base.Mission.MainAgent == null || !base.Mission.MainAgent.IsActive();
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x0001D448 File Offset: 0x0001B648
		protected override void OnEndMission()
		{
			Settlement encounterSettlement = PlayerEncounter.EncounterSettlement;
			if (Mission.Current.MainAgent == null || !Mission.Current.MainAgent.IsActive())
			{
				Campaign.Current.GameMenuManager.SetNextMenu("menu_town_thugs_failed");
			}
			else if (Mission.Current.MainAgent != null && Mission.Current.MainAgent.IsActive() && this._enemiesAliveCount > 0)
			{
				Campaign.Current.GameMenuManager.SetNextMenu("menu_town_thugs_escaped");
			}
			else
			{
				Campaign.Current.GameMenuManager.SetNextMenu("menu_town_thugs_succeeded");
			}
			base.Mission.EndMission();
		}

		// Token: 0x040001E0 RID: 480
		private MissionAgentHandler _missionAgentHandler;

		// Token: 0x040001E1 RID: 481
		private int _enemiesAliveCount;

		// Token: 0x040001E2 RID: 482
		private const int _guardsNoticeRange = 15;

		// Token: 0x040001E3 RID: 483
		private Dictionary<Agent, Timer> _guardRaycheckTimers = new Dictionary<Agent, Timer>();

		// Token: 0x040001E4 RID: 484
		private const float _raycheckMaxTime = 2f;
	}
}
