using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200027D RID: 637
	public abstract class SallyOutMissionController : MissionLogic
	{
		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x060021D9 RID: 8665 RVA: 0x0007B916 File Offset: 0x00079B16
		private float BesiegedDeploymentDuration
		{
			get
			{
				return 55f;
			}
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x060021DA RID: 8666 RVA: 0x0007B91D File Offset: 0x00079B1D
		private float BesiegerActivationDuration
		{
			get
			{
				return 8f;
			}
		}

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x060021DB RID: 8667 RVA: 0x0007B924 File Offset: 0x00079B24
		public MBReadOnlyList<SiegeWeapon> BesiegerSiegeEngines
		{
			get
			{
				return this._besiegerSiegeEngines;
			}
		}

		// Token: 0x060021DC RID: 8668 RVA: 0x0007B92C File Offset: 0x00079B2C
		public override void OnBehaviorInitialize()
		{
			this.MissionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			this._sallyOutNotificationsHandler = new SallyOutMissionNotificationsHandler(this.MissionAgentSpawnLogic, this);
			Mission.Current.GetOverriddenFleePositionForAgent += this.GetSallyOutFleePositionForAgent;
		}

		// Token: 0x060021DD RID: 8669 RVA: 0x0007B968 File Offset: 0x00079B68
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

		// Token: 0x060021DE RID: 8670 RVA: 0x0007B9E9 File Offset: 0x00079BE9
		public override void OnMissionTick(float dt)
		{
			this._sallyOutNotificationsHandler.OnMissionTick(dt);
			this.UpdateTimers();
		}

		// Token: 0x060021DF RID: 8671 RVA: 0x0007B9FD File Offset: 0x00079BFD
		public override void OnDeploymentFinished()
		{
			this._besiegerSiegeEngines = SallyOutMissionController.GetBesiegerSiegeEngines();
			SallyOutMissionController.DisableSiegeEngines();
			Mission.Current.AddMissionBehavior(new SallyOutEndLogic());
			this._sallyOutNotificationsHandler.OnDeploymentFinished();
			this._besiegerActivationTimer = new BasicMissionTimer();
			this.DeactivateBesiegers();
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x0007BA3A File Offset: 0x00079C3A
		protected override void OnEndMission()
		{
			this._sallyOutNotificationsHandler.OnMissionEnd();
			Mission.Current.GetOverriddenFleePositionForAgent -= this.GetSallyOutFleePositionForAgent;
		}

		// Token: 0x060021E1 RID: 8673
		protected abstract void GetInitialTroopCounts(out int besiegedTotalTroopCount, out int besiegerTotalTroopCount);

		// Token: 0x060021E2 RID: 8674 RVA: 0x0007BA60 File Offset: 0x00079C60
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

		// Token: 0x060021E3 RID: 8675 RVA: 0x0007BBE4 File Offset: 0x00079DE4
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

		// Token: 0x060021E4 RID: 8676 RVA: 0x0007BC6C File Offset: 0x00079E6C
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

		// Token: 0x060021E5 RID: 8677 RVA: 0x0007BD0C File Offset: 0x00079F0C
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

		// Token: 0x060021E6 RID: 8678 RVA: 0x0007BD98 File Offset: 0x00079F98
		private static MissionSpawnSettings CreateSallyOutSpawnSettings(float besiegedReinforcementPercentage, float besiegerReinforcementPercentage)
		{
			return new MissionSpawnSettings(MissionSpawnSettings.InitialSpawnMethod.FreeAllocation, MissionSpawnSettings.ReinforcementTimingMethod.CustomTimer, MissionSpawnSettings.ReinforcementSpawnMethod.Fixed, 0f, 0f, 0f, 0f, 0, besiegedReinforcementPercentage, besiegerReinforcementPercentage, 1f, 0.75f);
		}

		// Token: 0x060021E7 RID: 8679 RVA: 0x0007BDD0 File Offset: 0x00079FD0
		private void OnDefenderTeamTacticalDecision(in TacticalDecision decision)
		{
			TacticalDecision tacticalDecision = decision;
			if (tacticalDecision.DecisionCode == 31)
			{
				this._sallyOutNotificationsHandler.OnBesiegedSideFallsbackToKeep();
			}
		}

		// Token: 0x060021E8 RID: 8680 RVA: 0x0007BDFC File Offset: 0x00079FFC
		private void DeactivateBesiegers()
		{
			foreach (Formation formation in base.Mission.AttackerTeam.FormationsIncludingSpecialAndEmpty)
			{
				formation.SetMovementOrder(MovementOrder.MovementOrderStop);
				formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
				formation.ReleaseFormationFromAI();
			}
		}

		// Token: 0x060021E9 RID: 8681 RVA: 0x0007BE6C File Offset: 0x0007A06C
		private void ActivateBesiegers()
		{
			Team attackerTeam = base.Mission.AttackerTeam;
			foreach (Formation formation in base.Mission.AttackerTeam.FormationsIncludingSpecialAndEmpty)
			{
				formation.SetControlledByAI(true, false);
			}
		}

		// Token: 0x060021EA RID: 8682 RVA: 0x0007BED4 File Offset: 0x0007A0D4
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

		// Token: 0x060021EB RID: 8683 RVA: 0x0007BF4C File Offset: 0x0007A14C
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

		// Token: 0x04000CA5 RID: 3237
		private const float BesiegedTotalTroopRatio = 0.25f;

		// Token: 0x04000CA6 RID: 3238
		private const float BesiegedInitialTroopRatio = 0.1f;

		// Token: 0x04000CA7 RID: 3239
		private const float BesiegedReinforcementRatio = 0.01f;

		// Token: 0x04000CA8 RID: 3240
		private const float BesiegerInitialTroopRatio = 0.1f;

		// Token: 0x04000CA9 RID: 3241
		private const float BesiegerReinforcementRatio = 0.1f;

		// Token: 0x04000CAA RID: 3242
		private const float BesiegedInitialInterval = 1f;

		// Token: 0x04000CAB RID: 3243
		private const float BesiegerInitialInterval = 90f;

		// Token: 0x04000CAC RID: 3244
		private const float BesiegerIntervalChange = 15f;

		// Token: 0x04000CAD RID: 3245
		private const int BesiegerIntervalChangeCount = 5;

		// Token: 0x04000CAE RID: 3246
		private const float PlayerToGateSquaredDistanceThreshold = 25f;

		// Token: 0x04000CAF RID: 3247
		private SallyOutMissionNotificationsHandler _sallyOutNotificationsHandler;

		// Token: 0x04000CB0 RID: 3248
		private List<CastleGate> _castleGates;

		// Token: 0x04000CB1 RID: 3249
		private BasicMissionTimer _besiegedDeploymentTimer;

		// Token: 0x04000CB2 RID: 3250
		private BasicMissionTimer _besiegerActivationTimer;

		// Token: 0x04000CB3 RID: 3251
		private MBReadOnlyList<SiegeWeapon> _besiegerSiegeEngines;

		// Token: 0x04000CB4 RID: 3252
		protected MissionAgentSpawnLogic MissionAgentSpawnLogic;
	}
}
