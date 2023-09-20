using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class BattleSideDeploymentPlan
	{
		public bool SpawnWithHorses
		{
			get
			{
				return this._spawnWithHorses;
			}
		}

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

		public void SetSpawnWithHorses(bool value)
		{
			this._spawnWithHorses = value;
			this._initialPlan.SetSpawnWithHorses(value);
			foreach (DeploymentPlan deploymentPlan in this._reinforcementPlans)
			{
				deploymentPlan.SetSpawnWithHorses(value);
			}
		}

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

		public bool IsFirstPlan(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.PlanCount == 1;
			}
			return this._reinforcementPlansCreated && this._currentReinforcementPlan.PlanCount == 1;
		}

		public bool IsPlanMade(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.IsPlanMade;
			}
			return this._reinforcementPlansCreated && this._currentReinforcementPlan.IsPlanMade;
		}

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

		public bool HasDeploymentBoundaries(DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.HasDeploymentBoundaries;
			}
			return this._reinforcementPlansCreated && this._currentReinforcementPlan.HasDeploymentBoundaries;
		}

		public IFormationDeploymentPlan GetFormationPlan(FormationClass fClass, DeploymentPlanType planType)
		{
			if (planType == DeploymentPlanType.Initial)
			{
				return this._initialPlan.GetFormationPlan(fClass);
			}
			return this._currentReinforcementPlan.GetFormationPlan(fClass);
		}

		public bool IsInitialPlanSuitableForFormations(ValueTuple<int, int>[] troopDataPerFormationClass)
		{
			return this._initialPlan.IsPlanSuitableForFormations(troopDataPerFormationClass);
		}

		public bool IsPositionInsideInitialDeploymentBoundaries(in Vec2 position)
		{
			return this._initialPlan.IsPositionInsideDeploymentBoundaries(position);
		}

		public Vec2 GetClosestInitialDeploymentBoundaryPosition(in Vec2 position)
		{
			return this._initialPlan.GetClosestBoundaryPosition(position);
		}

		public readonly BattleSideEnum Side;

		private readonly Mission _mission;

		private readonly DeploymentPlan _initialPlan;

		private bool _spawnWithHorses;

		private bool _reinforcementPlansCreated;

		private readonly List<DeploymentPlan> _reinforcementPlans;

		private DeploymentPlan _currentReinforcementPlan;
	}
}
