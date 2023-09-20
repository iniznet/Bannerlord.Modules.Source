using System;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
	public class DeploymentMissionController : MissionLogic
	{
		public DeploymentMissionController(bool isPlayerAttacker)
		{
			this._isPlayerAttacker = isPlayerAttacker;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._battleDeploymentHandler = base.Mission.GetMissionBehavior<BattleDeploymentHandler>();
			this.MissionBoundaryPlacer = base.Mission.GetMissionBehavior<MissionBoundaryPlacer>();
			this.MissionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
		}

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

		private void SwapTeams()
		{
			base.Mission.PlayerTeam = base.Mission.PlayerEnemyTeam;
		}

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
			if (mainAgent != null)
			{
				mainAgent.SetDetachableFromFormation(true);
				mainAgent.Controller = Agent.ControllerType.Player;
			}
			base.Mission.AllowAiTicking = true;
			base.Mission.DisableDying = false;
			this.MissionAgentSpawnLogic.SetReinforcementsSpawnEnabled(true, true);
			this.OnAfterDeploymentFinished();
			base.Mission.RemoveMissionBehavior(this);
		}

		public virtual void OnBeforeDeploymentFinished()
		{
			this.OnSideDeploymentFinished(base.Mission.PlayerTeam.Side);
		}

		public virtual void OnAfterDeploymentFinished()
		{
			base.Mission.RemoveMissionBehavior(this._battleDeploymentHandler);
		}

		private BattleDeploymentHandler _battleDeploymentHandler;

		protected MissionBoundaryPlacer MissionBoundaryPlacer;

		protected MissionAgentSpawnLogic MissionAgentSpawnLogic;

		private readonly bool _isPlayerAttacker;

		protected bool TeamSetupOver;
	}
}
