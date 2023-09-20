using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000160 RID: 352
	public class TacticPerimeterDefense : TacticComponent
	{
		// Token: 0x060011E4 RID: 4580 RVA: 0x00042EE8 File Offset: 0x000410E8
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

		// Token: 0x060011E5 RID: 4581 RVA: 0x00042F74 File Offset: 0x00041174
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

		// Token: 0x060011E6 RID: 4582 RVA: 0x00043290 File Offset: 0x00041490
		private bool MustRetreatToCastle()
		{
			return base.Team.QuerySystem.TotalPowerRatio / base.Team.QuerySystem.RemainingPowerRatio > 2f;
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x000432BC File Offset: 0x000414BC
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

		// Token: 0x060011E8 RID: 4584 RVA: 0x00043334 File Offset: 0x00041534
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

		// Token: 0x060011E9 RID: 4585 RVA: 0x00043354 File Offset: 0x00041554
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

		// Token: 0x060011EA RID: 4586 RVA: 0x000436C8 File Offset: 0x000418C8
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

		// Token: 0x060011EB RID: 4587 RVA: 0x000436ED File Offset: 0x000418ED
		protected internal override float GetTacticWeight()
		{
			if (this._defendPosition.IsValid)
			{
				return 10f;
			}
			return 0f;
		}

		// Token: 0x040004B6 RID: 1206
		private WorldPosition _defendPosition;

		// Token: 0x040004B7 RID: 1207
		private readonly List<TacticPerimeterDefense.EnemyCluster> _enemyClusters;

		// Token: 0x040004B8 RID: 1208
		private readonly List<TacticPerimeterDefense.DefenseFront> _defenseFronts;

		// Token: 0x040004B9 RID: 1209
		private const float RetreatThresholdValue = 2f;

		// Token: 0x040004BA RID: 1210
		private List<Formation> _meleeFormations;

		// Token: 0x040004BB RID: 1211
		private List<Formation> _rangedFormations;

		// Token: 0x040004BC RID: 1212
		private bool _isRetreatingToKeep;

		// Token: 0x020004C4 RID: 1220
		private class DefenseFront
		{
			// Token: 0x060037DA RID: 14298 RVA: 0x000E31FF File Offset: 0x000E13FF
			public DefenseFront(TacticPerimeterDefense.EnemyCluster matchedEnemyCluster, Formation meleeFormation)
			{
				this.MatchedEnemyCluster = matchedEnemyCluster;
				this.MeleeFormation = meleeFormation;
				this.RangedFormation = null;
			}

			// Token: 0x04001A8C RID: 6796
			public Formation MeleeFormation;

			// Token: 0x04001A8D RID: 6797
			public Formation RangedFormation;

			// Token: 0x04001A8E RID: 6798
			public TacticPerimeterDefense.EnemyCluster MatchedEnemyCluster;
		}

		// Token: 0x020004C5 RID: 1221
		private class EnemyCluster
		{
			// Token: 0x17000956 RID: 2390
			// (get) Token: 0x060037DB RID: 14299 RVA: 0x000E321C File Offset: 0x000E141C
			// (set) Token: 0x060037DC RID: 14300 RVA: 0x000E3224 File Offset: 0x000E1424
			public Vec2 AggregatePosition { get; private set; }

			// Token: 0x17000957 RID: 2391
			// (get) Token: 0x060037DD RID: 14301 RVA: 0x000E322D File Offset: 0x000E142D
			// (set) Token: 0x060037DE RID: 14302 RVA: 0x000E3235 File Offset: 0x000E1435
			public WorldPosition MedianAggregatePosition { get; private set; }

			// Token: 0x060037DF RID: 14303 RVA: 0x000E323E File Offset: 0x000E143E
			public EnemyCluster()
			{
				this.enemyFormations = new List<Formation>();
			}

			// Token: 0x060037E0 RID: 14304 RVA: 0x000E3254 File Offset: 0x000E1454
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

			// Token: 0x060037E1 RID: 14305 RVA: 0x000E3314 File Offset: 0x000E1514
			public void AddToCluster(Formation formation)
			{
				this.enemyFormations.Add(formation);
				float num = this.totalPower;
				this.totalPower += formation.QuerySystem.FormationPower;
				this.AggregatePosition = this.AggregatePosition * (num / this.totalPower) + formation.QuerySystem.AveragePosition * (formation.QuerySystem.FormationPower / this.totalPower);
				this.UpdateMedianPosition();
			}

			// Token: 0x060037E2 RID: 14306 RVA: 0x000E3394 File Offset: 0x000E1594
			public void RemoveFromCluster(Formation formation)
			{
				this.enemyFormations.Remove(formation);
				float num = this.totalPower;
				this.totalPower -= formation.QuerySystem.FormationPower;
				this.AggregatePosition -= formation.QuerySystem.AveragePosition * (formation.QuerySystem.FormationPower / num);
				this.AggregatePosition *= num / this.totalPower;
				this.UpdateMedianPosition();
			}

			// Token: 0x060037E3 RID: 14307 RVA: 0x000E341C File Offset: 0x000E161C
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

			// Token: 0x04001A8F RID: 6799
			public List<Formation> enemyFormations;

			// Token: 0x04001A90 RID: 6800
			public float totalPower;
		}
	}
}
