using System;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	public abstract class BaseBattleMissionController : MissionLogic
	{
		private protected bool IsPlayerAttacker { protected get; private set; }

		private protected int DeployedAttackerTroopCount { protected get; private set; }

		private protected int DeployedDefenderTroopCount { protected get; private set; }

		protected BaseBattleMissionController(bool isPlayerAttacker)
		{
			this.IsPlayerAttacker = isPlayerAttacker;
			this.game = Game.Current;
		}

		public override void EarlyStart()
		{
			this.EarlyStart();
		}

		public override void AfterStart()
		{
			base.AfterStart();
			this.CreateTeams();
			base.Mission.SetMissionMode(MissionMode.Battle, true);
		}

		protected virtual void SetupTeam(Team team)
		{
			if (team.Side == BattleSideEnum.Attacker)
			{
				this.CreateAttackerTroops();
			}
			else
			{
				this.CreateDefenderTroops();
			}
			if (team == base.Mission.PlayerTeam)
			{
				this.CreatePlayer();
			}
		}

		protected abstract void CreateDefenderTroops();

		protected abstract void CreateAttackerTroops();

		public virtual TeamAIComponent GetTeamAI(Team team, float thinkTimerTime = 5f, float applyTimerTime = 1f)
		{
			return new TeamAIGeneral(base.Mission, team, thinkTimerTime, applyTimerTime);
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
		}

		[Conditional("DEBUG")]
		private void DebugTick()
		{
			if (Input.DebugInput.IsHotKeyPressed("SwapToEnemy"))
			{
				this.BecomeEnemy();
			}
			if (Input.DebugInput.IsHotKeyDown("BaseBattleMissionControllerHotkeyBecomePlayer"))
			{
				this.BecomePlayer();
			}
		}

		protected bool IsPlayerDead()
		{
			return base.Mission.MainAgent == null || !base.Mission.MainAgent.IsActive();
		}

		public override bool MissionEnded(ref MissionResult missionResult)
		{
			if (!this.IsDeploymentFinished)
			{
				return false;
			}
			if (this.IsPlayerDead())
			{
				missionResult = MissionResult.CreateDefeated(base.Mission);
				return true;
			}
			if (base.Mission.GetMemberCountOfSide(BattleSideEnum.Attacker) == 0)
			{
				missionResult = ((base.Mission.PlayerTeam.Side == BattleSideEnum.Attacker) ? MissionResult.CreateDefeated(base.Mission) : MissionResult.CreateSuccessful(base.Mission, false));
				return true;
			}
			if (base.Mission.GetMemberCountOfSide(BattleSideEnum.Defender) == 0)
			{
				missionResult = ((base.Mission.PlayerTeam.Side == BattleSideEnum.Attacker) ? MissionResult.CreateSuccessful(base.Mission, false) : MissionResult.CreateDefeated(base.Mission));
				return true;
			}
			return false;
		}

		public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
		{
			canPlayerLeave = true;
			if (!this.IsPlayerDead() && base.Mission.IsPlayerCloseToAnEnemy(5f))
			{
				canPlayerLeave = false;
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_can_not_retreat", null), 0, null, "");
			}
			else
			{
				MissionResult missionResult = null;
				if (!this.IsPlayerDead() && !this.MissionEnded(ref missionResult))
				{
					return new InquiryData("", GameTexts.FindText("str_retreat_question", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(base.Mission.OnEndMissionResult), null, "", 0f, null, null, null);
				}
			}
			return null;
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
		}

		private void CreateTeams()
		{
			if (!base.Mission.Teams.IsEmpty<Team>())
			{
				throw new MBIllegalValueException("Number of teams is not 0.");
			}
			base.Mission.Teams.Add(BattleSideEnum.Defender, 4278190335U, 4278190335U, null, true, false, true);
			base.Mission.Teams.Add(BattleSideEnum.Attacker, 4278255360U, 4278255360U, null, true, false, true);
			if (this.IsPlayerAttacker)
			{
				base.Mission.PlayerTeam = base.Mission.AttackerTeam;
			}
			else
			{
				base.Mission.PlayerTeam = base.Mission.DefenderTeam;
			}
			TeamAIComponent teamAI = this.GetTeamAI(base.Mission.DefenderTeam, 5f, 1f);
			base.Mission.DefenderTeam.AddTeamAI(teamAI, false);
			TeamAIComponent teamAI2 = this.GetTeamAI(base.Mission.AttackerTeam, 5f, 1f);
			base.Mission.AttackerTeam.AddTeamAI(teamAI2, false);
		}

		protected bool IsDeploymentFinished
		{
			get
			{
				return base.Mission.GetMissionBehavior<DeploymentHandler>() == null;
			}
		}

		protected virtual void CreateTroop(string troopName, Team troopTeam, int troopCount, bool isReinforcement = false)
		{
			BasicCharacterObject @object = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(troopName);
			FormationClass defaultFormationClass = @object.DefaultFormationClass;
			Formation formation = troopTeam.GetFormation(defaultFormationClass);
			WorldPosition worldPosition;
			Vec2 vec;
			base.Mission.GetFormationSpawnFrame(troopTeam.Side, defaultFormationClass, isReinforcement, out worldPosition, out vec);
			formation.SetPositioning(new WorldPosition?(worldPosition), new Vec2?(vec), null);
			for (int i = 0; i < troopCount; i++)
			{
				Agent agent = base.Mission.SpawnAgent(new AgentBuildData(@object).Team(troopTeam).Formation(formation).FormationTroopSpawnCount(troopCount)
					.FormationTroopSpawnIndex(i)
					.ClothingColor1(5398358U), false);
				agent.SetWatchState(Agent.WatchState.Alarmed);
				agent.SetAlwaysAttackInMelee(true);
				this.IncrementDeploymedTroops(troopTeam.Side);
			}
		}

		protected void IncrementDeploymedTroops(BattleSideEnum side)
		{
			int num;
			if (side == BattleSideEnum.Attacker)
			{
				num = this.DeployedAttackerTroopCount;
				this.DeployedAttackerTroopCount = num + 1;
				return;
			}
			num = this.DeployedDefenderTroopCount;
			this.DeployedDefenderTroopCount = num + 1;
		}

		protected virtual void CreatePlayer()
		{
			this.game.PlayerTroop = Game.Current.ObjectManager.GetObject<BasicCharacterObject>("main_hero");
			FormationClass formationClass = base.Mission.GetFormationSpawnClass(base.Mission.PlayerTeam.Side, FormationClass.NumberOfRegularFormations, false);
			if (formationClass != FormationClass.NumberOfRegularFormations)
			{
				formationClass = this.game.PlayerTroop.DefaultFormationClass;
			}
			WorldPosition worldPosition;
			Vec2 vec;
			base.Mission.GetFormationSpawnFrame(base.Mission.PlayerTeam.Side, formationClass, false, out worldPosition, out vec);
			Mission mission = base.Mission;
			AgentBuildData agentBuildData = new AgentBuildData(this.game.PlayerTroop).Team(base.Mission.PlayerTeam);
			Vec3 groundVec = worldPosition.GetGroundVec3();
			Agent agent = mission.SpawnAgent(agentBuildData.InitialPosition(groundVec).InitialDirection(vec).Controller(Agent.ControllerType.Player), false);
			agent.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp);
			base.Mission.MainAgent = agent;
		}

		protected void BecomeEnemy()
		{
			base.Mission.MainAgent.Controller = Agent.ControllerType.AI;
			base.Mission.PlayerEnemyTeam.Leader.Controller = Agent.ControllerType.Player;
			this.SwapTeams();
		}

		protected void BecomePlayer()
		{
			base.Mission.MainAgent.Controller = Agent.ControllerType.Player;
			base.Mission.PlayerEnemyTeam.Leader.Controller = Agent.ControllerType.AI;
			this.SwapTeams();
		}

		protected void SwapTeams()
		{
			base.Mission.PlayerTeam = base.Mission.PlayerEnemyTeam;
			this.IsPlayerAttacker = !this.IsPlayerAttacker;
		}

		protected readonly Game game;
	}
}
