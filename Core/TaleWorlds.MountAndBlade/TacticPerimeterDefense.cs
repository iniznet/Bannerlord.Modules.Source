using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TacticPerimeterDefense : TacticComponent
	{
		public TacticPerimeterDefense(Team team)
			: base(team)
		{
			Scene scene = Mission.Current.Scene;
			FleePosition fleePosition = Mission.Current.GetFleePositionsForSide(BattleSideEnum.Defender).FirstOrDefault((FleePosition fp) => fp.GetSide() == BattleSideEnum.Defender);
			if (fleePosition != null)
			{
				this._defendPosition = fleePosition.GameEntity.GlobalPosition.ToWorldPosition();
			}
			else
			{
				this._defendPosition = WorldPosition.Invalid;
			}
			this._enemyClusters = new List<TacticPerimeterDefense.EnemyCluster>();
			this._defenseFronts = new List<TacticPerimeterDefense.DefenseFront>();
		}

		private void DetermineEnemyClusters()
		{
			List<Formation> list = new List<Formation>();
			float num = 0f;
			foreach (Team team in base.Team.Mission.Teams)
			{
				if (team.IsEnemyOf(base.Team))
				{
					num += team.QuerySystem.TeamPower;
				}
			}
			foreach (Team team2 in base.Team.Mission.Teams)
			{
				if (team2.IsEnemyOf(base.Team))
				{
					for (int i = 0; i < Math.Min(team2.FormationsIncludingSpecialAndEmpty.Count, 8); i++)
					{
						Formation enemyFormation = team2.FormationsIncludingSpecialAndEmpty[i];
						if (enemyFormation.CountOfUnits > 0 && enemyFormation.QuerySystem.FormationPower < MathF.Min(base.Team.QuerySystem.TeamPower, num) / 4f)
						{
							if (!this._enemyClusters.Any((TacticPerimeterDefense.EnemyCluster ec) => ec.enemyFormations.IndexOf(enemyFormation) >= 0))
							{
								list.Add(enemyFormation);
							}
						}
						else
						{
							TacticPerimeterDefense.EnemyCluster enemyCluster = this._enemyClusters.FirstOrDefault((TacticPerimeterDefense.EnemyCluster ec) => ec.enemyFormations.IndexOf(enemyFormation) >= 0);
							if (enemyCluster != null)
							{
								if ((double)(this._defendPosition.AsVec2 - enemyCluster.AggregatePosition).DotProduct(this._defendPosition.AsVec2 - enemyFormation.QuerySystem.AveragePosition) >= 0.70710678118)
								{
									goto IL_216;
								}
								enemyCluster.RemoveFromCluster(enemyFormation);
							}
							List<TacticPerimeterDefense.EnemyCluster> list2 = this._enemyClusters.Where((TacticPerimeterDefense.EnemyCluster c) => (double)(this._defendPosition.AsVec2 - c.AggregatePosition).DotProduct(this._defendPosition.AsVec2 - enemyFormation.QuerySystem.MedianPosition.AsVec2) >= 0.70710678118).ToList<TacticPerimeterDefense.EnemyCluster>();
							if (list2.Count > 0)
							{
								list2.MaxBy((TacticPerimeterDefense.EnemyCluster ec) => (this._defendPosition.AsVec2 - ec.AggregatePosition).DotProduct(this._defendPosition.AsVec2 - enemyFormation.QuerySystem.MedianPosition.AsVec2)).AddToCluster(enemyFormation);
							}
							else
							{
								TacticPerimeterDefense.EnemyCluster enemyCluster2 = new TacticPerimeterDefense.EnemyCluster();
								enemyCluster2.AddToCluster(enemyFormation);
								this._enemyClusters.Add(enemyCluster2);
							}
						}
						IL_216:;
					}
				}
			}
			if (this._enemyClusters.Count > 0)
			{
				using (List<Formation>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Formation skippedFormation = enumerator2.Current;
						this._enemyClusters.MaxBy((TacticPerimeterDefense.EnemyCluster ec) => (this._defendPosition.AsVec2 - ec.AggregatePosition).DotProduct(this._defendPosition.AsVec2 - skippedFormation.QuerySystem.MedianPosition.AsVec2)).AddToCluster(skippedFormation);
					}
				}
			}
		}

		private bool MustRetreatToCastle()
		{
			return base.Team.QuerySystem.TotalPowerRatio / base.Team.QuerySystem.RemainingPowerRatio > 2f;
		}

		private void StartRetreatToKeep()
		{
			foreach (Formation formation in base.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.AI.ResetBehaviorWeights();
					TacticComponent.SetDefaultBehaviorWeights(formation);
					formation.AI.SetBehaviorWeight<BehaviorRetreatToKeep>(1f);
				}
			}
		}

		private void CheckAndChangeState()
		{
			if (this.MustRetreatToCastle())
			{
				if (this._isRetreatingToKeep)
				{
					return;
				}
				this._isRetreatingToKeep = true;
				this.StartRetreatToKeep();
			}
		}

		private void ArrangeDefenseFronts()
		{
			this._meleeFormations = base.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && (f.QuerySystem.IsInfantryFormation || f.QuerySystem.IsCavalryFormation)).ToList<Formation>();
			this._rangedFormations = base.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && (f.QuerySystem.IsRangedFormation || f.QuerySystem.IsRangedCavalryFormation)).ToList<Formation>();
			int num = MathF.Min(8 - this._rangedFormations.Count, this._enemyClusters.Count);
			if (this._meleeFormations.Count != num)
			{
				base.SplitFormationClassIntoGivenNumber((Formation f) => f.QuerySystem.IsInfantryFormation || f.QuerySystem.IsCavalryFormation, num);
				this._meleeFormations = base.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && (f.QuerySystem.IsInfantryFormation || f.QuerySystem.IsCavalryFormation)).ToList<Formation>();
			}
			int num2 = MathF.Min(8 - num, this._enemyClusters.Count);
			if (this._rangedFormations.Count != num2)
			{
				base.SplitFormationClassIntoGivenNumber((Formation f) => f.QuerySystem.IsRangedFormation || f.QuerySystem.IsRangedCavalryFormation, num2);
				this._rangedFormations = base.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && (f.QuerySystem.IsRangedFormation || f.QuerySystem.IsRangedCavalryFormation)).ToList<Formation>();
			}
			foreach (TacticPerimeterDefense.DefenseFront defenseFront in this._defenseFronts)
			{
				defenseFront.MatchedEnemyCluster.UpdateClusterData();
				BehaviorDefendKeyPosition behaviorDefendKeyPosition = defenseFront.MeleeFormation.AI.SetBehaviorWeight<BehaviorDefendKeyPosition>(1f);
				behaviorDefendKeyPosition.EnemyClusterPosition = defenseFront.MatchedEnemyCluster.MedianAggregatePosition;
				behaviorDefendKeyPosition.EnemyClusterPosition.SetVec2(defenseFront.MatchedEnemyCluster.AggregatePosition);
			}
			IEnumerable<TacticPerimeterDefense.EnemyCluster> enumerable = this._enemyClusters.Where((TacticPerimeterDefense.EnemyCluster ec) => this._defenseFronts.All((TacticPerimeterDefense.DefenseFront df) => df.MatchedEnemyCluster != ec));
			List<Formation> list = this._meleeFormations.Where((Formation mf) => this._defenseFronts.All((TacticPerimeterDefense.DefenseFront df) => df.MeleeFormation != mf)).ToList<Formation>();
			List<Formation> list2 = this._rangedFormations.Where((Formation rf) => this._defenseFronts.All((TacticPerimeterDefense.DefenseFront df) => df.RangedFormation != rf)).ToList<Formation>();
			foreach (TacticPerimeterDefense.EnemyCluster enemyCluster in enumerable)
			{
				if (list.IsEmpty<Formation>())
				{
					break;
				}
				Formation formation = list[list.Count - 1];
				TacticPerimeterDefense.DefenseFront defenseFront2 = new TacticPerimeterDefense.DefenseFront(enemyCluster, formation);
				formation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(formation);
				BehaviorDefendKeyPosition behaviorDefendKeyPosition2 = formation.AI.SetBehaviorWeight<BehaviorDefendKeyPosition>(1f);
				behaviorDefendKeyPosition2.DefensePosition = this._defendPosition;
				behaviorDefendKeyPosition2.EnemyClusterPosition = enemyCluster.MedianAggregatePosition;
				behaviorDefendKeyPosition2.EnemyClusterPosition.SetVec2(enemyCluster.AggregatePosition);
				list.Remove(formation);
				if (!list2.IsEmpty<Formation>())
				{
					Formation formation2 = list2[list2.Count - 1];
					formation2.AI.ResetBehaviorWeights();
					TacticComponent.SetDefaultBehaviorWeights(formation2);
					formation2.AI.SetBehaviorWeight<BehaviorSkirmishBehindFormation>(1f).ReferenceFormation = formation;
					defenseFront2.RangedFormation = formation2;
					list2.Remove(formation2);
					this._defenseFronts.Add(defenseFront2);
				}
			}
		}

		protected internal override void TickOccasionally()
		{
			if (!base.AreFormationsCreated)
			{
				return;
			}
			this.CheckAndChangeState();
			if (!this._isRetreatingToKeep)
			{
				this.DetermineEnemyClusters();
				this.ArrangeDefenseFronts();
			}
		}

		protected internal override float GetTacticWeight()
		{
			if (this._defendPosition.IsValid)
			{
				return 10f;
			}
			return 0f;
		}

		private WorldPosition _defendPosition;

		private readonly List<TacticPerimeterDefense.EnemyCluster> _enemyClusters;

		private readonly List<TacticPerimeterDefense.DefenseFront> _defenseFronts;

		private const float RetreatThresholdValue = 2f;

		private List<Formation> _meleeFormations;

		private List<Formation> _rangedFormations;

		private bool _isRetreatingToKeep;

		private class DefenseFront
		{
			public DefenseFront(TacticPerimeterDefense.EnemyCluster matchedEnemyCluster, Formation meleeFormation)
			{
				this.MatchedEnemyCluster = matchedEnemyCluster;
				this.MeleeFormation = meleeFormation;
				this.RangedFormation = null;
			}

			public Formation MeleeFormation;

			public Formation RangedFormation;

			public TacticPerimeterDefense.EnemyCluster MatchedEnemyCluster;
		}

		private class EnemyCluster
		{
			public Vec2 AggregatePosition { get; private set; }

			public WorldPosition MedianAggregatePosition { get; private set; }

			public EnemyCluster()
			{
				this.enemyFormations = new List<Formation>();
			}

			public void UpdateClusterData()
			{
				this.totalPower = this.enemyFormations.Sum((Formation ef) => ef.QuerySystem.FormationPower);
				this.AggregatePosition = Vec2.Zero;
				foreach (Formation formation in this.enemyFormations)
				{
					this.AggregatePosition += formation.QuerySystem.AveragePosition * (formation.QuerySystem.FormationPower / this.totalPower);
				}
				this.UpdateMedianPosition();
			}

			public void AddToCluster(Formation formation)
			{
				this.enemyFormations.Add(formation);
				float num = this.totalPower;
				this.totalPower += formation.QuerySystem.FormationPower;
				this.AggregatePosition = this.AggregatePosition * (num / this.totalPower) + formation.QuerySystem.AveragePosition * (formation.QuerySystem.FormationPower / this.totalPower);
				this.UpdateMedianPosition();
			}

			public void RemoveFromCluster(Formation formation)
			{
				this.enemyFormations.Remove(formation);
				float num = this.totalPower;
				this.totalPower -= formation.QuerySystem.FormationPower;
				this.AggregatePosition -= formation.QuerySystem.AveragePosition * (formation.QuerySystem.FormationPower / num);
				this.AggregatePosition *= num / this.totalPower;
				this.UpdateMedianPosition();
			}

			private void UpdateMedianPosition()
			{
				float num = float.MaxValue;
				foreach (Formation formation in this.enemyFormations)
				{
					float num2 = formation.QuerySystem.MedianPosition.AsVec2.DistanceSquared(this.AggregatePosition);
					if (num2 < num)
					{
						num = num2;
						this.MedianAggregatePosition = formation.QuerySystem.MedianPosition;
					}
				}
			}

			public List<Formation> enemyFormations;

			public float totalPower;
		}
	}
}
