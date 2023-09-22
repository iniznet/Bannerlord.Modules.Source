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
	public class AmbushMissionController : BaseBattleMissionController
	{
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

		public AmbushMissionController(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
			this._checkPoints = new List<GameEntity>();
			this._defenderSpawnPoints = new List<GameEntity>();
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._ambushDeploymentHandler = base.Mission.GetMissionBehavior<AmbushBattleDeploymentHandler>();
			this._ambushIntroLogic = base.Mission.GetMissionBehavior<AmbushIntroLogic>();
		}

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

		protected override void CreateDefenderTroops()
		{
			this.CreateTroop("guard", base.Mission.DefenderTeam, 30, false);
		}

		protected override void CreateAttackerTroops()
		{
			this.CreateTroop("guard", base.Mission.AttackerTeam, 10, false);
			this.CreateTroop("archer", base.Mission.AttackerTeam, 15, false);
		}

		protected void CreateTroop(string troopName, Team troopTeam, int troopCount, bool isReinforcement = false)
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
					(base.Mission.SpawnAgent(new AgentBuildData(@object).Team(troopTeam).Formation(formation).FormationTroopSpawnCount(troopCount)
						.FormationTroopSpawnIndex(i), false).AddController(typeof(AmbushBattleAgentController)) as AmbushBattleAgentController).IsAttacker = true;
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
						FormationClass agentTroopClass = base.Mission.GetAgentTroopClass(0, agent.Character);
						agent.Formation = base.Mission.DefenderTeam.GetFormation(agentTroopClass);
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

		public event AmbushMissionEventDelegate PlayerDeploymentFinish;

		public event AmbushMissionEventDelegate IntroFinish;

		private AmbushBattleDeploymentHandler _ambushDeploymentHandler;

		private AmbushIntroLogic _ambushIntroLogic;

		private readonly List<GameEntity> _checkPoints;

		private readonly List<GameEntity> _defenderSpawnPoints;

		private int _currentPositionIndex;

		private int _columns;

		private const float UnitSpread = 1f;

		private Team _playerSoloTeam;

		private bool _firstTick = true;
	}
}
