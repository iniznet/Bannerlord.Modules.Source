using System;
using System.Collections.Generic;
using SandBox.Missions.Handlers;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace SandBox.Missions.AgentControllers
{
	// Token: 0x02000068 RID: 104
	public class AmbushMissionController : BaseBattleMissionController
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000472 RID: 1138 RVA: 0x00020A7C File Offset: 0x0001EC7C
		// (set) Token: 0x06000473 RID: 1139 RVA: 0x00020A84 File Offset: 0x0001EC84
		public bool IsPlayerAmbusher
		{
			get
			{
				return base.IsPlayerAttacker;
			}
			private set
			{
			}
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00020A86 File Offset: 0x0001EC86
		public AmbushMissionController(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
			this._checkPoints = new List<GameEntity>();
			this._defenderSpawnPoints = new List<GameEntity>();
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x00020AAC File Offset: 0x0001ECAC
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._ambushDeploymentHandler = base.Mission.GetMissionBehavior<AmbushBattleDeploymentHandler>();
			this._ambushIntroLogic = base.Mission.GetMissionBehavior<AmbushIntroLogic>();
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00020AD8 File Offset: 0x0001ECD8
		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.SetMissionMode(4, true);
			int num = 0;
			GameEntity gameEntity;
			do
			{
				gameEntity = Mission.Current.Scene.FindEntityWithTag("checkpoint_" + num.ToString());
				if (gameEntity != null)
				{
					this._checkPoints.Add(gameEntity);
					num++;
				}
			}
			while (gameEntity != null);
			num = 0;
			do
			{
				gameEntity = Mission.Current.Scene.FindEntityWithTag("spawnpoint_defender_" + num.ToString());
				if (gameEntity != null)
				{
					this._defenderSpawnPoints.Add(gameEntity);
					num++;
				}
			}
			while (gameEntity != null);
			if (base.Mission.PlayerTeam.Side == 1)
			{
				this.SetupTeam(base.Mission.AttackerTeam);
			}
			else
			{
				this.SetupTeam(base.Mission.AttackerTeam);
				this.SetupTeam(base.Mission.DefenderTeam);
			}
			this._playerSoloTeam = base.Mission.Teams.Add(base.Mission.PlayerTeam.Side, uint.MaxValue, uint.MaxValue, null, true, false, false);
			base.Mission.AttackerTeam.SetIsEnemyOf(base.Mission.DefenderTeam, false);
			base.Mission.DefenderTeam.SetIsEnemyOf(base.Mission.AttackerTeam, false);
			base.Mission.AttackerTeam.SetIsEnemyOf(this._playerSoloTeam, false);
			base.Mission.DefenderTeam.SetIsEnemyOf(this._playerSoloTeam, true);
			base.Mission.AttackerTeam.ExpireAIQuerySystem();
			base.Mission.DefenderTeam.ExpireAIQuerySystem();
			Agent.Main.Controller = 1;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x00020C8C File Offset: 0x0001EE8C
		public override void OnMissionTick(float dt)
		{
			if (this._firstTick)
			{
				this._firstTick = false;
				if (!this.IsPlayerAmbusher)
				{
					base.Mission.AddMissionBehavior(new MissionBoundaryCrossingHandler());
					this._ambushIntroLogic.StartIntro();
				}
			}
			base.OnMissionTick(dt);
			this.UpdateAgents();
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00020CD8 File Offset: 0x0001EED8
		protected override void CreateDefenderTroops()
		{
			this.CreateTroop("guard", base.Mission.DefenderTeam, 30, false);
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00020CF3 File Offset: 0x0001EEF3
		protected override void CreateAttackerTroops()
		{
			this.CreateTroop("guard", base.Mission.AttackerTeam, 10, false);
			this.CreateTroop("archer", base.Mission.AttackerTeam, 15, false);
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00020D28 File Offset: 0x0001EF28
		protected override void CreateTroop(string troopName, Team troopTeam, int troopCount, bool isReinforcement = false)
		{
			if (troopTeam.Side == 1)
			{
				CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>(troopName);
				FormationClass defaultFormationClass = @object.DefaultFormationClass;
				Formation formation = troopTeam.GetFormation(defaultFormationClass);
				WorldPosition worldPosition;
				Vec2 vec;
				base.Mission.GetFormationSpawnFrame(troopTeam.Side, defaultFormationClass, isReinforcement, ref worldPosition, ref vec);
				formation.SetPositioning(new WorldPosition?(worldPosition), new Vec2?(vec), null);
				for (int i = 0; i < troopCount; i++)
				{
					Agent agent = base.Mission.SpawnAgent(new AgentBuildData(@object).Team(troopTeam).Formation(formation).FormationTroopSpawnCount(troopCount)
						.FormationTroopSpawnIndex(i), false);
					agent.SetAlwaysAttackInMelee(true);
					(agent.AddController(typeof(AmbushBattleAgentController)) as AmbushBattleAgentController).IsAttacker = true;
					base.IncrementDeploymedTroops(1);
				}
				return;
			}
			CharacterObject object2 = this.game.ObjectManager.GetObject<CharacterObject>(troopName);
			for (int j = 0; j < troopCount; j++)
			{
				int count = this._defenderSpawnPoints.Count;
				this._columns = MathF.Ceiling((float)troopCount / (float)count);
				int num = base.DeployedDefenderTroopCount - base.DeployedDefenderTroopCount / this._columns * this._columns;
				MatrixFrame globalFrame = this._defenderSpawnPoints[base.DeployedDefenderTroopCount / this._columns].GetGlobalFrame();
				globalFrame.origin = globalFrame.TransformToParent(new Vec3(1f, 0f, 0f, -1f) * (float)num * 1f);
				Mission mission = base.Mission;
				AgentBuildData agentBuildData = new AgentBuildData(object2).Team(troopTeam).InitialPosition(ref globalFrame.origin);
				Vec2 vec2 = globalFrame.rotation.f.AsVec2;
				vec2 = vec2.Normalized();
				(mission.SpawnAgent(agentBuildData.InitialDirection(ref vec2), false).AddController(typeof(AmbushBattleAgentController)) as AmbushBattleAgentController).IsAttacker = false;
				base.IncrementDeploymedTroops(0);
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00020F20 File Offset: 0x0001F120
		public void OnPlayerDeploymentFinish(bool doDebugPause = false)
		{
			if (base.Mission.PlayerTeam.Side == 1)
			{
				this.SetupTeam(base.Mission.DefenderTeam);
			}
			base.Mission.RemoveMissionBehavior(this._ambushDeploymentHandler);
			base.Mission.AddMissionBehavior(new MissionBoundaryCrossingHandler());
			this._ambushIntroLogic.StartIntro();
			if (this.PlayerDeploymentFinish != null)
			{
				this.PlayerDeploymentFinish();
			}
			Agent.Main.SetTeam(this._playerSoloTeam, true);
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00020FA1 File Offset: 0x0001F1A1
		public void OnIntroductionFinish()
		{
			if (!base.IsPlayerAttacker)
			{
				this.StartFighting();
			}
			if (this.IntroFinish != null)
			{
				this.IntroFinish();
			}
			Agent.Main.Controller = 2;
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00020FD0 File Offset: 0x0001F1D0
		private void UpdateAgents()
		{
			int num = 0;
			int num2 = 0;
			foreach (Agent agent in base.Mission.Agents)
			{
				if (base.Mission.Mode == 4 && agent.IsAIControlled && agent.CurrentWatchState == 1 && agent.IsHuman)
				{
					this.StartFighting();
				}
				if (!this.IsPlayerAmbusher && Agent.Main.IsAIControlled)
				{
					Vec2 movementDirection = Agent.Main.GetMovementDirection();
					WorldPosition worldPosition = Agent.Main.GetWorldPosition();
					worldPosition.SetVec2(worldPosition.AsVec2 + movementDirection * 5f);
					Agent.Main.DisableScriptedMovement();
					Agent.Main.SetScriptedPosition(ref worldPosition, false, 16);
				}
				AmbushBattleAgentController controller = agent.GetController<AmbushBattleAgentController>();
				if (controller != null)
				{
					controller.UpdateState();
					if (!controller.IsAttacker && !controller.Aggressive)
					{
						if (num == 0)
						{
							if (controller.CheckArrivedAtWayPoint(this._checkPoints[this._currentPositionIndex]))
							{
								this._currentPositionIndex++;
								if (this._currentPositionIndex >= this._checkPoints.Count)
								{
									MBDebug.ShowWarning("The enemy has gotten away.");
								}
								else
								{
									WorldPosition worldPosition2;
									worldPosition2..ctor(Mission.Current.Scene, UIntPtr.Zero, this._checkPoints[this._currentPositionIndex].GlobalPosition, false);
									agent.SetScriptedPosition(ref worldPosition2, false, 16);
								}
							}
						}
						else
						{
							WorldPosition worldPosition3;
							Vec2 vec;
							if (num % this._columns != 0)
							{
								Agent agent2 = base.Mission.Agents[num2 - 1];
								worldPosition3 = agent2.GetWorldPosition();
								vec = agent2.GetMovementDirection();
								vec.RotateCCW(-1.5707964f);
							}
							else
							{
								Agent agent3 = base.Mission.Agents[num2 - this._columns];
								worldPosition3 = agent3.GetWorldPosition();
								vec = agent.Position.AsVec2 - agent3.Position.AsVec2;
								vec.Normalize();
							}
							worldPosition3.SetVec2(worldPosition3.AsVec2 + vec * 1f);
							agent.DisableScriptedMovement();
							agent.SetScriptedPosition(ref worldPosition3, false, 16);
						}
						num++;
					}
				}
				num2++;
			}
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0002124C File Offset: 0x0001F44C
		private void StartFighting()
		{
			base.Mission.AttackerTeam.SetIsEnemyOf(base.Mission.DefenderTeam, true);
			base.Mission.DefenderTeam.SetIsEnemyOf(base.Mission.AttackerTeam, true);
			if (base.Mission.PlayerAllyTeam != null)
			{
				base.Mission.PlayerAllyTeam.SetIsEnemyOf(base.Mission.PlayerEnemyTeam, true);
				base.Mission.PlayerEnemyTeam.SetIsEnemyOf(base.Mission.PlayerAllyTeam, true);
			}
			base.Mission.SetMissionMode(2, false);
			foreach (Agent agent in base.Mission.Agents)
			{
				AmbushBattleAgentController controller = agent.GetController<AmbushBattleAgentController>();
				if (controller != null)
				{
					controller.Aggressive = true;
					if (!controller.IsAttacker)
					{
						agent.DisableScriptedMovement();
						FormationClass formationClass = agent.Character.GetFormationClass();
						agent.Formation = base.Mission.DefenderTeam.GetFormation(formationClass);
						agent.Formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
					}
				}
				if (agent.IsPlayerControlled)
				{
					agent.SetTeam(base.Mission.PlayerTeam, true);
				}
			}
			base.Mission.DefenderTeam.MasterOrderController.SelectAllFormations(false);
			base.Mission.DefenderTeam.MasterOrderController.SetOrder(6);
			base.Mission.DefenderTeam.MasterOrderController.SetOrder(4);
			foreach (Formation formation in base.Mission.DefenderTeam.FormationsIncludingSpecialAndEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					base.Mission.DefenderTeam.MasterOrderController.DeselectFormation(formation);
				}
			}
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x0600047F RID: 1151 RVA: 0x00021440 File Offset: 0x0001F640
		// (remove) Token: 0x06000480 RID: 1152 RVA: 0x00021478 File Offset: 0x0001F678
		public event AmbushMissionEventDelegate PlayerDeploymentFinish;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000481 RID: 1153 RVA: 0x000214B0 File Offset: 0x0001F6B0
		// (remove) Token: 0x06000482 RID: 1154 RVA: 0x000214E8 File Offset: 0x0001F6E8
		public event AmbushMissionEventDelegate IntroFinish;

		// Token: 0x04000228 RID: 552
		private AmbushBattleDeploymentHandler _ambushDeploymentHandler;

		// Token: 0x04000229 RID: 553
		private AmbushIntroLogic _ambushIntroLogic;

		// Token: 0x0400022A RID: 554
		private readonly List<GameEntity> _checkPoints;

		// Token: 0x0400022B RID: 555
		private readonly List<GameEntity> _defenderSpawnPoints;

		// Token: 0x0400022C RID: 556
		private int _currentPositionIndex;

		// Token: 0x0400022D RID: 557
		private int _columns;

		// Token: 0x0400022E RID: 558
		private const float UnitSpread = 1f;

		// Token: 0x0400022F RID: 559
		private Team _playerSoloTeam;

		// Token: 0x04000230 RID: 560
		private bool _firstTick = true;
	}
}
