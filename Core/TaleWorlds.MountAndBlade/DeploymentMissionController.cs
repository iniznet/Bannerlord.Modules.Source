using System;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000269 RID: 617
	public class DeploymentMissionController : MissionLogic
	{
		// Token: 0x060020F4 RID: 8436 RVA: 0x00076664 File Offset: 0x00074864
		public DeploymentMissionController(bool isPlayerAttacker)
		{
			this._isPlayerAttacker = isPlayerAttacker;
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x00076673 File Offset: 0x00074873
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._battleDeploymentHandler = base.Mission.GetMissionBehavior<BattleDeploymentHandler>();
			this.MissionBoundaryPlacer = base.Mission.GetMissionBehavior<MissionBoundaryPlacer>();
			this.MissionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x000766B0 File Offset: 0x000748B0
		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.AllowAiTicking = false;
			for (int i = 0; i < 2; i++)
			{
				this.MissionAgentSpawnLogic.SetSpawnTroops((BattleSideEnum)i, false, false);
			}
			this.MissionAgentSpawnLogic.SetReinforcementsSpawnEnabled(false, true);
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x000766F8 File Offset: 0x000748F8
		private void SetupTeams()
		{
			Utilities.SetLoadingScreenPercentage(0.92f);
			base.Mission.DisableDying = true;
			BattleSideEnum battleSideEnum = (this._isPlayerAttacker ? BattleSideEnum.Defender : BattleSideEnum.Attacker);
			BattleSideEnum battleSideEnum2 = (this._isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			this.SetupTeamsOfSide(battleSideEnum);
			this.OnSideDeploymentFinished(battleSideEnum);
			if (this._isPlayerAttacker)
			{
				foreach (Agent agent in base.Mission.Agents)
				{
					if (agent.IsHuman && agent.Team != null && agent.Team.Side == battleSideEnum)
					{
						agent.SetRenderCheckEnabled(false);
						agent.AgentVisuals.SetVisible(false);
						Agent mountAgent = agent.MountAgent;
						if (mountAgent != null)
						{
							mountAgent.SetRenderCheckEnabled(false);
						}
						Agent mountAgent2 = agent.MountAgent;
						if (mountAgent2 != null)
						{
							mountAgent2.AgentVisuals.SetVisible(false);
						}
					}
				}
			}
			this.SetupTeamsOfSide(battleSideEnum2);
			base.Mission.IsTeleportingAgents = true;
			Utilities.SetLoadingScreenPercentage(0.96f);
			if (!MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle())
			{
				this.FinishDeployment();
			}
		}

		// Token: 0x060020F8 RID: 8440 RVA: 0x00076820 File Offset: 0x00074A20
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			Agent mainAgent = base.Mission.MainAgent;
			if (mainAgent != null && mainAgent.Controller != Agent.ControllerType.AI)
			{
				mainAgent.Controller = Agent.ControllerType.AI;
				mainAgent.SetIsAIPaused(true);
				mainAgent.SetDetachableFromFormation(false);
			}
			if (!this.TeamSetupOver && base.Mission.Scene != null)
			{
				this.SetupTeams();
				this.TeamSetupOver = true;
			}
		}

		// Token: 0x060020F9 RID: 8441 RVA: 0x00076889 File Offset: 0x00074A89
		[Conditional("DEBUG")]
		private void DebugTick()
		{
			if (Input.DebugInput.IsHotKeyPressed("SwapToEnemy"))
			{
				base.Mission.MainAgent.Controller = Agent.ControllerType.AI;
				base.Mission.PlayerEnemyTeam.Leader.Controller = Agent.ControllerType.Player;
				this.SwapTeams();
			}
		}

		// Token: 0x060020FA RID: 8442 RVA: 0x000768C9 File Offset: 0x00074AC9
		private void SwapTeams()
		{
			base.Mission.PlayerTeam = base.Mission.PlayerEnemyTeam;
		}

		// Token: 0x060020FB RID: 8443 RVA: 0x000768E4 File Offset: 0x00074AE4
		protected virtual void SetupTeamsOfSide(BattleSideEnum side)
		{
			this.MissionAgentSpawnLogic.SetSpawnTroops(side, true, true);
			Team team = ((side == BattleSideEnum.Attacker) ? base.Mission.AttackerTeam : base.Mission.DefenderTeam);
			foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.ApplyActionOnEachUnit(delegate(Agent agent)
					{
						if (agent.IsAIControlled)
						{
							agent.AIStateFlags &= ~Agent.AIStateFlag.Alarmed;
							agent.SetIsAIPaused(true);
						}
					}, null);
				}
			}
			Team team2 = ((side == BattleSideEnum.Attacker) ? base.Mission.AttackerAllyTeam : base.Mission.DefenderAllyTeam);
			if (team2 != null)
			{
				foreach (Formation formation2 in team2.FormationsIncludingSpecialAndEmpty)
				{
					if (formation2.CountOfUnits > 0)
					{
						formation2.ApplyActionOnEachUnit(delegate(Agent agent)
						{
							if (agent.IsAIControlled)
							{
								agent.AIStateFlags &= ~Agent.AIStateFlag.Alarmed;
								agent.SetIsAIPaused(true);
							}
						}, null);
					}
				}
			}
			this.MissionAgentSpawnLogic.OnBattleSideDeployed(team.Side);
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x00076A28 File Offset: 0x00074C28
		protected void OnSideDeploymentFinished(BattleSideEnum side)
		{
			Team team = ((side == BattleSideEnum.Attacker) ? base.Mission.AttackerTeam : base.Mission.DefenderTeam);
			if (side != base.Mission.PlayerTeam.Side)
			{
				base.Mission.IsTeleportingAgents = true;
				this.DeployFormationsOfTeam(team);
				Team team2 = ((side == BattleSideEnum.Attacker) ? base.Mission.AttackerAllyTeam : base.Mission.DefenderAllyTeam);
				if (team2 != null)
				{
					this.DeployFormationsOfTeam(team2);
				}
				base.Mission.IsTeleportingAgents = false;
			}
		}

		// Token: 0x060020FD RID: 8445 RVA: 0x00076AAC File Offset: 0x00074CAC
		protected void DeployFormationsOfTeam(Team team)
		{
			foreach (Formation formation in team.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.SetControlledByAI(true, false);
				}
			}
			team.QuerySystem.Expire();
			base.Mission.AllowAiTicking = true;
			base.Mission.ForceTickOccasionally = true;
			team.ResetTactic();
			bool isTeleportingAgents = Mission.Current.IsTeleportingAgents;
			base.Mission.IsTeleportingAgents = true;
			team.Tick(0f);
			base.Mission.IsTeleportingAgents = isTeleportingAgents;
			base.Mission.AllowAiTicking = false;
			base.Mission.ForceTickOccasionally = false;
		}

		// Token: 0x060020FE RID: 8446 RVA: 0x00076B78 File Offset: 0x00074D78
		public void FinishDeployment()
		{
			this.OnBeforeDeploymentFinished();
			if (this._isPlayerAttacker)
			{
				foreach (Agent agent in base.Mission.Agents)
				{
					if (agent.IsHuman && agent.Team != null && agent.Team.Side == BattleSideEnum.Defender)
					{
						agent.SetRenderCheckEnabled(true);
						agent.AgentVisuals.SetVisible(true);
						Agent mountAgent = agent.MountAgent;
						if (mountAgent != null)
						{
							mountAgent.SetRenderCheckEnabled(true);
						}
						Agent mountAgent2 = agent.MountAgent;
						if (mountAgent2 != null)
						{
							mountAgent2.AgentVisuals.SetVisible(true);
						}
					}
				}
			}
			base.Mission.IsTeleportingAgents = false;
			Mission.Current.OnDeploymentFinished();
			foreach (Agent agent2 in base.Mission.Agents)
			{
				if (agent2.IsAIControlled)
				{
					agent2.AIStateFlags |= Agent.AIStateFlag.Alarmed;
					agent2.SetIsAIPaused(false);
					if (agent2.GetAgentFlags().HasAnyFlag(AgentFlag.CanWieldWeapon))
					{
						agent2.ResetEnemyCaches();
					}
					HumanAIComponent humanAIComponent = agent2.HumanAIComponent;
					if (humanAIComponent != null)
					{
						humanAIComponent.SyncBehaviorParamsIfNecessary();
					}
				}
			}
			Agent mainAgent = base.Mission.MainAgent;
			mainAgent.SetDetachableFromFormation(true);
			mainAgent.Controller = Agent.ControllerType.Player;
			base.Mission.AllowAiTicking = true;
			base.Mission.DisableDying = false;
			this.MissionAgentSpawnLogic.SetReinforcementsSpawnEnabled(true, true);
			this.OnAfterDeploymentFinished();
			base.Mission.RemoveMissionBehavior(this);
		}

		// Token: 0x060020FF RID: 8447 RVA: 0x00076D20 File Offset: 0x00074F20
		public virtual void OnBeforeDeploymentFinished()
		{
			this.OnSideDeploymentFinished(base.Mission.PlayerTeam.Side);
		}

		// Token: 0x06002100 RID: 8448 RVA: 0x00076D38 File Offset: 0x00074F38
		public virtual void OnAfterDeploymentFinished()
		{
			base.Mission.RemoveMissionBehavior(this._battleDeploymentHandler);
		}

		// Token: 0x04000C25 RID: 3109
		private BattleDeploymentHandler _battleDeploymentHandler;

		// Token: 0x04000C26 RID: 3110
		protected MissionBoundaryPlacer MissionBoundaryPlacer;

		// Token: 0x04000C27 RID: 3111
		protected MissionAgentSpawnLogic MissionAgentSpawnLogic;

		// Token: 0x04000C28 RID: 3112
		private readonly bool _isPlayerAttacker;

		// Token: 0x04000C29 RID: 3113
		protected bool TeamSetupOver;
	}
}
