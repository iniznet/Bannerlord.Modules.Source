using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class SallyOutMissionController : MissionLogic
	{
		private float BesiegedDeploymentDuration
		{
			get
			{
				return 55f;
			}
		}

		private float BesiegerActivationDuration
		{
			get
			{
				return 8f;
			}
		}

		public MBReadOnlyList<SiegeWeapon> BesiegerSiegeEngines
		{
			get
			{
				return this._besiegerSiegeEngines;
			}
		}

		public override void OnBehaviorInitialize()
		{
			this.MissionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			this._sallyOutNotificationsHandler = new SallyOutMissionNotificationsHandler(this.MissionAgentSpawnLogic, this);
			Mission.Current.GetOverriddenFleePositionForAgent += this.GetSallyOutFleePositionForAgent;
		}

		public override void AfterStart()
		{
			this._sallyOutNotificationsHandler.OnAfterStart();
			int num;
			int num2;
			this.GetInitialTroopCounts(out num, out num2);
			this.SetupInitialSpawn(num, num2);
			this._castleGates = base.Mission.MissionObjects.FindAllWithType<CastleGate>().ToList<CastleGate>();
			this._besiegedDeploymentTimer = new BasicMissionTimer();
			TeamAIComponent teamAI = base.Mission.DefenderTeam.TeamAI;
			teamAI.OnNotifyTacticalDecision = (TeamAIComponent.TacticalDecisionDelegate)Delegate.Combine(teamAI.OnNotifyTacticalDecision, new TeamAIComponent.TacticalDecisionDelegate(this.OnDefenderTeamTacticalDecision));
		}

		public override void OnMissionTick(float dt)
		{
			this._sallyOutNotificationsHandler.OnMissionTick(dt);
			this.UpdateTimers();
		}

		public override void OnDeploymentFinished()
		{
			this._besiegerSiegeEngines = SallyOutMissionController.GetBesiegerSiegeEngines();
			SallyOutMissionController.DisableSiegeEngines();
			Mission.Current.AddMissionBehavior(new SallyOutEndLogic());
			this._sallyOutNotificationsHandler.OnDeploymentFinished();
			this._besiegerActivationTimer = new BasicMissionTimer();
			this.DeactivateBesiegers();
		}

		protected override void OnEndMission()
		{
			this._sallyOutNotificationsHandler.OnMissionEnd();
			Mission.Current.GetOverriddenFleePositionForAgent -= this.GetSallyOutFleePositionForAgent;
		}

		protected abstract void GetInitialTroopCounts(out int besiegedTotalTroopCount, out int besiegerTotalTroopCount);

		private void UpdateTimers()
		{
			if (this._besiegedDeploymentTimer != null)
			{
				if (this._besiegedDeploymentTimer.ElapsedTime >= this.BesiegedDeploymentDuration)
				{
					foreach (CastleGate castleGate in this._castleGates)
					{
						castleGate.SetAutoOpenState(true);
					}
					this._besiegedDeploymentTimer = null;
					goto IL_127;
				}
				using (List<CastleGate>.Enumerator enumerator = this._castleGates.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CastleGate castleGate2 = enumerator.Current;
						if (!castleGate2.IsDestroyed && !castleGate2.IsGateOpen)
						{
							castleGate2.OpenDoor();
						}
					}
					goto IL_127;
				}
			}
			Agent mainAgent = base.Mission.MainAgent;
			if (mainAgent != null && mainAgent.IsActive())
			{
				Vec3 eyeGlobalPosition = mainAgent.GetEyeGlobalPosition();
				foreach (CastleGate castleGate3 in this._castleGates)
				{
					if (!castleGate3.IsDestroyed && !castleGate3.IsGateOpen && eyeGlobalPosition.DistanceSquared(castleGate3.GameEntity.GlobalPosition) <= 25f)
					{
						castleGate3.OpenDoor();
					}
				}
			}
			IL_127:
			if (this._besiegerActivationTimer != null && this._besiegerActivationTimer.ElapsedTime >= this.BesiegerActivationDuration)
			{
				this.ActivateBesiegers();
				this._besiegerActivationTimer = null;
			}
		}

		private void AdjustTotalTroopCounts(ref int besiegedTotalTroopCount, ref int besiegerTotalTroopCount)
		{
			float num = 0.25f;
			float num2 = 1f - num;
			int num3 = (int)((float)this.MissionAgentSpawnLogic.BattleSize * num);
			int num4 = (int)((float)this.MissionAgentSpawnLogic.BattleSize * num2);
			besiegedTotalTroopCount = MathF.Min(besiegedTotalTroopCount, num3);
			besiegerTotalTroopCount = MathF.Min(besiegerTotalTroopCount, num4);
			float num5 = num2 / num;
			if ((float)besiegerTotalTroopCount / (float)besiegedTotalTroopCount <= num5)
			{
				int num6 = (int)((float)besiegerTotalTroopCount / num5);
				besiegedTotalTroopCount = MathF.Min(num6, besiegedTotalTroopCount);
				return;
			}
			int num7 = (int)((float)besiegedTotalTroopCount * num5);
			besiegerTotalTroopCount = MathF.Min(num7, besiegerTotalTroopCount);
		}

		private void SetupInitialSpawn(int besiegedTotalTroopCount, int besiegerTotalTroopCount)
		{
			this.AdjustTotalTroopCounts(ref besiegedTotalTroopCount, ref besiegerTotalTroopCount);
			int num = besiegedTotalTroopCount + besiegerTotalTroopCount;
			int num2 = MathF.Min(besiegedTotalTroopCount, MathF.Ceiling((float)num * 0.1f));
			int num3 = MathF.Min(besiegerTotalTroopCount, MathF.Ceiling((float)num * 0.1f));
			this.MissionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Defender, true);
			this.MissionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Attacker, false);
			MissionSpawnSettings missionSpawnSettings = SallyOutMissionController.CreateSallyOutSpawnSettings(0.01f, 0.1f);
			this.MissionAgentSpawnLogic.InitWithSinglePhase(besiegedTotalTroopCount, besiegerTotalTroopCount, num2, num3, false, false, missionSpawnSettings);
			this.MissionAgentSpawnLogic.SetCustomReinforcementSpawnTimer(new SallyOutReinforcementSpawnTimer(1f, 90f, 15f, 5));
		}

		private WorldPosition? GetSallyOutFleePositionForAgent(Agent agent)
		{
			if (!agent.IsHuman)
			{
				return null;
			}
			Formation formation = agent.Formation;
			if (formation == null || formation.Team.Side == BattleSideEnum.Attacker)
			{
				return null;
			}
			bool flag = !agent.HasMount;
			bool isRangedCached = agent.IsRangedCached;
			FormationClass formationClass;
			if (flag)
			{
				formationClass = (isRangedCached ? FormationClass.Ranged : FormationClass.Infantry);
			}
			else
			{
				formationClass = (isRangedCached ? FormationClass.HorseArcher : FormationClass.Cavalry);
			}
			return new WorldPosition?(Mission.Current.DeploymentPlan.GetFormationPlan(formation.Team.Side, formationClass, DeploymentPlanType.Initial).CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3));
		}

		private static MissionSpawnSettings CreateSallyOutSpawnSettings(float besiegedReinforcementPercentage, float besiegerReinforcementPercentage)
		{
			return new MissionSpawnSettings(MissionSpawnSettings.InitialSpawnMethod.FreeAllocation, MissionSpawnSettings.ReinforcementTimingMethod.CustomTimer, MissionSpawnSettings.ReinforcementSpawnMethod.Fixed, 0f, 0f, 0f, 0f, 0, besiegedReinforcementPercentage, besiegerReinforcementPercentage, 1f, 0.75f);
		}

		private void OnDefenderTeamTacticalDecision(in TacticalDecision decision)
		{
			TacticalDecision tacticalDecision = decision;
			if (tacticalDecision.DecisionCode == 31)
			{
				this._sallyOutNotificationsHandler.OnBesiegedSideFallsbackToKeep();
			}
		}

		private void DeactivateBesiegers()
		{
			foreach (Formation formation in base.Mission.AttackerTeam.FormationsIncludingSpecialAndEmpty)
			{
				formation.SetMovementOrder(MovementOrder.MovementOrderStop);
				formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
				formation.SetControlledByAI(false, false);
			}
		}

		private void ActivateBesiegers()
		{
			Team attackerTeam = base.Mission.AttackerTeam;
			foreach (Formation formation in base.Mission.AttackerTeam.FormationsIncludingSpecialAndEmpty)
			{
				formation.SetControlledByAI(true, false);
			}
		}

		public static MBReadOnlyList<SiegeWeapon> GetBesiegerSiegeEngines()
		{
			MBList<SiegeWeapon> mblist = new MBList<SiegeWeapon>();
			using (List<MissionObject>.Enumerator enumerator = Mission.Current.ActiveMissionObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SiegeWeapon siegeWeapon;
					if ((siegeWeapon = enumerator.Current as SiegeWeapon) != null && siegeWeapon.DestructionComponent != null && siegeWeapon.Side == BattleSideEnum.Attacker)
					{
						mblist.Add(siegeWeapon);
					}
				}
			}
			return mblist;
		}

		public static void DisableSiegeEngines()
		{
			using (List<MissionObject>.Enumerator enumerator = Mission.Current.ActiveMissionObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SiegeWeapon siegeWeapon;
					if ((siegeWeapon = enumerator.Current as SiegeWeapon) != null && siegeWeapon.DestructionComponent != null && !siegeWeapon.IsDeactivated)
					{
						siegeWeapon.Disable();
						siegeWeapon.Deactivate();
					}
				}
			}
		}

		private const float BesiegedTotalTroopRatio = 0.25f;

		private const float BesiegedInitialTroopRatio = 0.1f;

		private const float BesiegedReinforcementRatio = 0.01f;

		private const float BesiegerInitialTroopRatio = 0.1f;

		private const float BesiegerReinforcementRatio = 0.1f;

		private const float BesiegedInitialInterval = 1f;

		private const float BesiegerInitialInterval = 90f;

		private const float BesiegerIntervalChange = 15f;

		private const int BesiegerIntervalChangeCount = 5;

		private const float PlayerToGateSquaredDistanceThreshold = 25f;

		private SallyOutMissionNotificationsHandler _sallyOutNotificationsHandler;

		private List<CastleGate> _castleGates;

		private BasicMissionTimer _besiegedDeploymentTimer;

		private BasicMissionTimer _besiegerActivationTimer;

		private MBReadOnlyList<SiegeWeapon> _besiegerSiegeEngines;

		protected MissionAgentSpawnLogic MissionAgentSpawnLogic;
	}
}
