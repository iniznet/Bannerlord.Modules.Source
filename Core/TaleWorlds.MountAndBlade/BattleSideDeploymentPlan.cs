using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001F4 RID: 500
	public class BattleSideDeploymentPlan
	{
		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x06001BCF RID: 7119 RVA: 0x000628CB File Offset: 0x00060ACB
		public bool SpawnWithHorses
		{
			get
			{
				return this._spawnWithHorses;
			}
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x000628D4 File Offset: 0x00060AD4
		public BattleSideDeploymentPlan(Mission mission, BattleSideEnum side)
		{
			this._mission = mission;
			this.Side = side;
			this._spawnWithHorses = false;
			this._initialPlan = DeploymentPlan.CreateInitialPlan(this._mission, side);
			this._reinforcementPlans = new List<DeploymentPlan>();
			this._reinforcementPlansCreated = false;
			this._currentReinforcementPlan = this._initialPlan;
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x0006292C File Offset: 0x00060B2C
		public void CreateReinforcementPlans()
		{
			if (!this._reinforcementPlansCreated)
			{
				if (this._mission.HasSpawnPath)
				{
					foreach (SpawnPathData spawnPathData in this._mission.GetReinforcementPathsDataOfSide(this.Side))
					{
						DeploymentPlan deploymentPlan = DeploymentPlan.CreateReinforcementPlanWithSpawnPath(this._mission, this.Side, spawnPathData);
						this._reinforcementPlans.Add(deploymentPlan);
					}
					this._currentReinforcementPlan = this._reinforcementPlans[0];
				}
				else
				{
					DeploymentPlan deploymentPlan2 = DeploymentPlan.CreateReinforcementPlan(this._mission, this.Side);
					this._reinforcementPlans.Add(deploymentPlan2);
					this._currentReinforcementPlan = deploymentPlan2;
				}
				this._reinforcementPlansCreated = true;
			}
		}

		// Token: 0x06001BD2 RID: 7122 RVA: 0x000629FC File Offset: 0x00060BFC
		public void SetSpawnWithHorses(bool value)
		{
			this._spawnWithHorses = value;
			this._initialPlan.SetSpawnWithHorses(value);
			foreach (DeploymentPlan deploymentPlan in this._reinforcementPlans)
			{
				deploymentPlan.SetSpawnWithHorses(value);
			}
		}

		// Token: 0x06001BD3 RID: 7123 RVA: 0x00062A60 File Offset: 0x00060C60
		public void PlanBattleDeployment(FormationSceneSpawnEntry[,] formationSceneSpawnEntries, DeploymentPlanType planType, float spawnPathOffset)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				if (!this._initialPlan.IsPlanMade)
				{
					this._initialPlan.PlanBattleDeployment(formationSceneSpawnEntries, spawnPathOffset);
					return;
				}
			}
			else if (planType == DeploymentPlanType.Reinforcement)
			{
				foreach (DeploymentPlan deploymentPlan in this._reinforcementPlans)
				{
					if (!deploymentPlan.IsPlanMade)
					{
						deploymentPlan.PlanBattleDeployment(formationSceneSpawnEntries, 0f);
					}
				}
			}
		}

		// Token: 0x06001BD4 RID: 7124 RVA: 0x00062AE4 File Offset: 0x00060CE4
		public void UpdateReinforcementPlans()
		{
			if (!this._reinforcementPlansCreated || this._reinforcementPlans.Count <= 1)
			{
				return;
			}
			foreach (DeploymentPlan deploymentPlan in this._reinforcementPlans)
			{
				deploymentPlan.UpdateSafetyScore();
			}
			if (!this._currentReinforcementPlan.IsSafeToDeploy)
			{
				this._currentReinforcementPlan = this._reinforcementPlans.MaxBy((DeploymentPlan plan) => plan.SafetyScore);
			}
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x00062B88 File Offset: 0x00060D88
		public void ClearPlans(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				this._initialPlan.ClearPlan();
				return;
			}
			if (planType == DeploymentPlanType.Reinforcement)
			{
				foreach (DeploymentPlan deploymentPlan in this._reinforcementPlans)
				{
					deploymentPlan.ClearPlan();
				}
			}
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x00062BEC File Offset: 0x00060DEC
		public void ClearAddedTroops(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				this._initialPlan.ClearAddedTroops();
				return;
			}
			foreach (DeploymentPlan deploymentPlan in this._reinforcementPlans)
			{
				deploymentPlan.ClearAddedTroops();
			}
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x00062C4C File Offset: 0x00060E4C
		public void AddTroops(FormationClass formationClass, int footTroopCount, int mountedTroopCount, DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				this._initialPlan.AddTroops(formationClass, footTroopCount, mountedTroopCount);
				return;
			}
			foreach (DeploymentPlan deploymentPlan in this._reinforcementPlans)
			{
				deploymentPlan.AddTroops(formationClass, footTroopCount, mountedTroopCount);
			}
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x00062CB4 File Offset: 0x00060EB4
		public bool IsFirstPlan(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.PlanCount == 1;
			}
			return this._reinforcementPlansCreated && this._currentReinforcementPlan.PlanCount == 1;
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x00062CE0 File Offset: 0x00060EE0
		public bool IsPlanMade(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.IsPlanMade;
			}
			return this._reinforcementPlansCreated && this._currentReinforcementPlan.IsPlanMade;
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x00062D06 File Offset: 0x00060F06
		public float GetSpawnPathOffset(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.SpawnPathOffset;
			}
			if (!this._reinforcementPlansCreated)
			{
				return 0f;
			}
			return this._currentReinforcementPlan.SpawnPathOffset;
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x00062D30 File Offset: 0x00060F30
		public int GetTroopCount(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.TroopCount;
			}
			if (!this._reinforcementPlansCreated)
			{
				return 0;
			}
			return this._currentReinforcementPlan.TroopCount;
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x00062D56 File Offset: 0x00060F56
		public MatrixFrame GetDeploymentFrame(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.DeploymentFrame;
			}
			if (!this._reinforcementPlansCreated)
			{
				return MatrixFrame.Identity;
			}
			return this._currentReinforcementPlan.DeploymentFrame;
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x00062D80 File Offset: 0x00060F80
		public MBReadOnlyDictionary<string, List<Vec2>> GetDeploymentBoundaries(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.DeploymentBoundaries;
			}
			if (!this._reinforcementPlansCreated)
			{
				return null;
			}
			return this._currentReinforcementPlan.DeploymentBoundaries;
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x00062DA6 File Offset: 0x00060FA6
		public float GetDeploymentWidth(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.DeploymentWidth;
			}
			if (!this._reinforcementPlansCreated)
			{
				return 0f;
			}
			return this._currentReinforcementPlan.DeploymentWidth;
		}

		// Token: 0x06001BDF RID: 7135 RVA: 0x00062DD0 File Offset: 0x00060FD0
		public bool HasDeploymentBoundaries(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.HasDeploymentBoundaries;
			}
			return this._reinforcementPlansCreated && this._currentReinforcementPlan.HasDeploymentBoundaries;
		}

		// Token: 0x06001BE0 RID: 7136 RVA: 0x00062DF6 File Offset: 0x00060FF6
		public IFormationDeploymentPlan GetFormationPlan(FormationClass fClass, DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.GetFormationPlan(fClass);
			}
			return this._currentReinforcementPlan.GetFormationPlan(fClass);
		}

		// Token: 0x06001BE1 RID: 7137 RVA: 0x00062E14 File Offset: 0x00061014
		public bool IsInitialPlanSuitableForFormations(ValueTuple<int, int>[] troopDataPerFormationClass)
		{
			return this._initialPlan.IsPlanSuitableForFormations(troopDataPerFormationClass);
		}

		// Token: 0x06001BE2 RID: 7138 RVA: 0x00062E22 File Offset: 0x00061022
		public bool IsPositionInsideInitialDeploymentBoundaries(in Vec2 position)
		{
			return this._initialPlan.IsPositionInsideDeploymentBoundaries(position);
		}

		// Token: 0x06001BE3 RID: 7139 RVA: 0x00062E30 File Offset: 0x00061030
		public Vec2 GetClosestInitialDeploymentBoundaryPosition(in Vec2 position)
		{
			return this._initialPlan.GetClosestBoundaryPosition(position);
		}

		// Token: 0x040008FE RID: 2302
		public readonly BattleSideEnum Side;

		// Token: 0x040008FF RID: 2303
		private readonly Mission _mission;

		// Token: 0x04000900 RID: 2304
		private readonly DeploymentPlan _initialPlan;

		// Token: 0x04000901 RID: 2305
		private bool _spawnWithHorses;

		// Token: 0x04000902 RID: 2306
		private bool _reinforcementPlansCreated;

		// Token: 0x04000903 RID: 2307
		private readonly List<DeploymentPlan> _reinforcementPlans;

		// Token: 0x04000904 RID: 2308
		private DeploymentPlan _currentReinforcementPlan;
	}
}
