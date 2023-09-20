using System;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	// Token: 0x020003EE RID: 1006
	public abstract class BaseBattleMissionController : MissionLogic
	{
		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x060034A8 RID: 13480 RVA: 0x000DAC4F File Offset: 0x000D8E4F
		// (set) Token: 0x060034A9 RID: 13481 RVA: 0x000DAC57 File Offset: 0x000D8E57
		private protected bool IsPlayerAttacker { protected get; private set; }

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x060034AA RID: 13482 RVA: 0x000DAC60 File Offset: 0x000D8E60
		// (set) Token: 0x060034AB RID: 13483 RVA: 0x000DAC68 File Offset: 0x000D8E68
		private protected int DeployedAttackerTroopCount { protected get; private set; }

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x060034AC RID: 13484 RVA: 0x000DAC71 File Offset: 0x000D8E71
		// (set) Token: 0x060034AD RID: 13485 RVA: 0x000DAC79 File Offset: 0x000D8E79
		private protected int DeployedDefenderTroopCount { protected get; private set; }

		// Token: 0x060034AE RID: 13486 RVA: 0x000DAC82 File Offset: 0x000D8E82
		protected BaseBattleMissionController(bool isPlayerAttacker)
		{
			this.IsPlayerAttacker = isPlayerAttacker;
			this.game = Game.Current;
		}

		// Token: 0x060034AF RID: 13487 RVA: 0x000DAC9C File Offset: 0x000D8E9C
		public override void EarlyStart()
		{
			this.EarlyStart();
		}

		// Token: 0x060034B0 RID: 13488 RVA: 0x000DACA4 File Offset: 0x000D8EA4
		public override void AfterStart()
		{
			base.AfterStart();
			this.CreateTeams();
			base.Mission.SetMissionMode(MissionMode.Battle, true);
		}

		// Token: 0x060034B1 RID: 13489 RVA: 0x000DACBF File Offset: 0x000D8EBF
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

		// Token: 0x060034B2 RID: 13490
		protected abstract void CreateDefenderTroops();

		// Token: 0x060034B3 RID: 13491
		protected abstract void CreateAttackerTroops();

		// Token: 0x060034B4 RID: 13492 RVA: 0x000DACEC File Offset: 0x000D8EEC
		public virtual TeamAIComponent GetTeamAI(Team team, float thinkTimerTime = 5f, float applyTimerTime = 1f)
		{
			return new TeamAIGeneral(base.Mission, team, thinkTimerTime, applyTimerTime);
		}

		// Token: 0x060034B5 RID: 13493 RVA: 0x000DACFC File Offset: 0x000D8EFC
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
		}

		// Token: 0x060034B6 RID: 13494 RVA: 0x000DAD05 File Offset: 0x000D8F05
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

		// Token: 0x060034B7 RID: 13495 RVA: 0x000DAD35 File Offset: 0x000D8F35
		protected bool IsPlayerDead()
		{
			return base.Mission.MainAgent == null || !base.Mission.MainAgent.IsActive();
		}

		// Token: 0x060034B8 RID: 13496 RVA: 0x000DAD5C File Offset: 0x000D8F5C
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

		// Token: 0x060034B9 RID: 13497 RVA: 0x000DAE08 File Offset: 0x000D9008
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

		// Token: 0x060034BA RID: 13498 RVA: 0x000DAEBF File Offset: 0x000D90BF
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
		}

		// Token: 0x060034BB RID: 13499 RVA: 0x000DAEC4 File Offset: 0x000D90C4
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

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x060034BC RID: 13500 RVA: 0x000DAFC0 File Offset: 0x000D91C0
		protected bool IsDeploymentFinished
		{
			get
			{
				return base.Mission.GetMissionBehavior<DeploymentHandler>() == null;
			}
		}

		// Token: 0x060034BD RID: 13501 RVA: 0x000DAFD0 File Offset: 0x000D91D0
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

		// Token: 0x060034BE RID: 13502 RVA: 0x000DB090 File Offset: 0x000D9290
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

		// Token: 0x060034BF RID: 13503 RVA: 0x000DB0C4 File Offset: 0x000D92C4
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

		// Token: 0x060034C0 RID: 13504 RVA: 0x000DB1A1 File Offset: 0x000D93A1
		protected void BecomeEnemy()
		{
			base.Mission.MainAgent.Controller = Agent.ControllerType.AI;
			base.Mission.PlayerEnemyTeam.Leader.Controller = Agent.ControllerType.Player;
			this.SwapTeams();
		}

		// Token: 0x060034C1 RID: 13505 RVA: 0x000DB1D0 File Offset: 0x000D93D0
		protected void BecomePlayer()
		{
			base.Mission.MainAgent.Controller = Agent.ControllerType.Player;
			base.Mission.PlayerEnemyTeam.Leader.Controller = Agent.ControllerType.AI;
			this.SwapTeams();
		}

		// Token: 0x060034C2 RID: 13506 RVA: 0x000DB1FF File Offset: 0x000D93FF
		protected void SwapTeams()
		{
			base.Mission.PlayerTeam = base.Mission.PlayerEnemyTeam;
			this.IsPlayerAttacker = !this.IsPlayerAttacker;
		}

		// Token: 0x0400168D RID: 5773
		protected readonly Game game;
	}
}
