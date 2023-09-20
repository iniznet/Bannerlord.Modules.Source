using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000155 RID: 341
	public class TacticBreachWalls : TacticComponent
	{
		// Token: 0x06001144 RID: 4420 RVA: 0x000394E8 File Offset: 0x000376E8
		public TacticBreachWalls(Team team)
			: base(team)
		{
			Mission mission = Mission.Current;
			this._teamAISiegeAttacker = team.TeamAI as TeamAISiegeAttacker;
			this._meleeFormations = new List<Formation>();
			this._rangedFormations = new List<Formation>();
			this._cachedUsedSiegeLanes = new List<SiegeLane>();
			this._cachedUsedArcherPositions = new List<ArcherPosition>();
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x00039540 File Offset: 0x00037740
		private void BalanceAssaultLanes(List<Formation> attackerFormations)
		{
			if (attackerFormations.Count < 2)
			{
				return;
			}
			int num = attackerFormations.Sum((Formation f) => f.CountOfUnitsWithoutDetachedOnes);
			int idealCount = num / attackerFormations.Count;
			int num2 = MathF.Max((int)((float)num * 0.2f), 1);
			Func<Formation, int> <>9__2;
			Func<Formation, bool> <>9__1;
			foreach (Formation formation in attackerFormations)
			{
				int num3 = 0;
				while (idealCount - formation.CountOfUnitsWithoutDetachedOnes > num2)
				{
					Func<Formation, bool> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = (Formation af) => af.CountOfUnitsWithoutDetachedOnes > idealCount);
					}
					if (!attackerFormations.Any(func) || num3 >= attackerFormations.Count)
					{
						break;
					}
					int num4 = idealCount - formation.CountOfUnitsWithoutDetachedOnes;
					Func<Formation, int> func2;
					if ((func2 = <>9__2) == null)
					{
						func2 = (<>9__2 = (Formation df) => df.CountOfUnitsWithoutDetachedOnes - idealCount);
					}
					Formation formation2 = attackerFormations.MaxBy(func2);
					num4 = MathF.Min(num4, formation2.CountOfUnitsWithoutDetachedOnes - idealCount);
					formation2.TransferUnits(formation, num4);
					num3++;
				}
			}
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x00039690 File Offset: 0x00037890
		private bool ShouldRetreat(List<SiegeLane> lanes, int insideFormationCount)
		{
			if (this._indicators != null)
			{
				float num = base.Team.QuerySystem.RemainingPowerRatio / this._indicators.StartingPowerRatio;
				float retreatThresholdRatio = this._indicators.GetRetreatThresholdRatio(lanes, insideFormationCount);
				return num < retreatThresholdRatio;
			}
			return false;
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x000396D4 File Offset: 0x000378D4
		private void AssignMeleeFormationsToLanes(List<Formation> meleeFormationsSource, List<SiegeLane> currentLanes)
		{
			List<Formation> list = new List<Formation>(meleeFormationsSource.Count);
			list.AddRange(meleeFormationsSource);
			List<SiegeLane> list2 = currentLanes.ToList<SiegeLane>();
			for (int i = 0; i < currentLanes.Count; i++)
			{
				SiegeLane siegeLane = currentLanes[i];
				Formation lastAssignedFormation = currentLanes[i].GetLastAssignedFormation(base.Team.TeamIndex);
				if (lastAssignedFormation != null && list.Contains(lastAssignedFormation))
				{
					lastAssignedFormation.AI.Side = siegeLane.LaneSide;
					lastAssignedFormation.AI.ResetBehaviorWeights();
					TacticComponent.SetDefaultBehaviorWeights(lastAssignedFormation);
					lastAssignedFormation.AI.SetBehaviorWeight<BehaviorAssaultWalls>(1f);
					lastAssignedFormation.AI.SetBehaviorWeight<BehaviorUseSiegeMachines>(1f);
					lastAssignedFormation.AI.SetBehaviorWeight<BehaviorWaitForLadders>(1f);
					list2.Remove(siegeLane);
					list.Remove(lastAssignedFormation);
				}
			}
			while (list.Count > 0 && list2.Count > 0)
			{
				Formation largestFormation = list.MaxBy((Formation mf) => mf.CountOfUnitsWithoutLooseDetachedOnes);
				SiegeLane siegeLane2 = list2.MinBy(delegate(SiegeLane l)
				{
					WorldPosition currentAttackerPosition = l.GetCurrentAttackerPosition();
					Vec3 navMeshVec = largestFormation.QuerySystem.MedianPosition.GetNavMeshVec3();
					return currentAttackerPosition.DistanceSquaredWithLimit(navMeshVec, 10000f);
				});
				largestFormation.AI.Side = siegeLane2.LaneSide;
				largestFormation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(largestFormation);
				largestFormation.AI.SetBehaviorWeight<BehaviorAssaultWalls>(1f);
				largestFormation.AI.SetBehaviorWeight<BehaviorUseSiegeMachines>(1f);
				largestFormation.AI.SetBehaviorWeight<BehaviorWaitForLadders>(1f);
				siegeLane2.SetLastAssignedFormation(base.Team.TeamIndex, largestFormation);
				list.Remove(largestFormation);
				list2.Remove(siegeLane2);
			}
			bool flag = true;
			while (list.Count > 0)
			{
				if (list2.IsEmpty<SiegeLane>())
				{
					list2.AddRange(currentLanes);
					flag = false;
				}
				Formation nextBiggest = list.MaxBy((Formation mf) => mf.CountOfUnitsWithoutLooseDetachedOnes);
				SiegeLane siegeLane3 = list2.MinBy(delegate(SiegeLane l)
				{
					WorldPosition currentAttackerPosition2 = l.GetCurrentAttackerPosition();
					Vec3 navMeshVec2 = nextBiggest.QuerySystem.MedianPosition.GetNavMeshVec3();
					return currentAttackerPosition2.DistanceSquaredWithLimit(navMeshVec2, 10000f);
				});
				nextBiggest.AI.Side = siegeLane3.LaneSide;
				nextBiggest.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(nextBiggest);
				nextBiggest.AI.SetBehaviorWeight<BehaviorAssaultWalls>(1f);
				nextBiggest.AI.SetBehaviorWeight<BehaviorUseSiegeMachines>(1f);
				nextBiggest.AI.SetBehaviorWeight<BehaviorWaitForLadders>(1f);
				if (flag)
				{
					siegeLane3.SetLastAssignedFormation(base.Team.TeamIndex, nextBiggest);
				}
				list.Remove(nextBiggest);
				list2.Remove(siegeLane3);
			}
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x000399D8 File Offset: 0x00037BD8
		private void WellRoundedAssault(ref List<SiegeLane> currentLanes, ref List<ArcherPosition> archerPositions)
		{
			if (currentLanes.Count == 0)
			{
				Debug.Print("TeamAISiegeComponent.SiegeLanes.Count" + TeamAISiegeComponent.SiegeLanes.Count, 0, Debug.DebugColor.White, 17592186044416UL);
				for (int i = 0; i < TeamAISiegeComponent.SiegeLanes.Count; i++)
				{
					SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes[i];
					Debug.Print(string.Concat(new object[]
					{
						"lane ",
						i,
						" is breach ",
						siegeLane.IsBreach.ToString(),
						" is unusable ",
						siegeLane.CalculateIsLaneUnusable().ToString(),
						" has gate ",
						siegeLane.HasGate.ToString()
					}), 0, Debug.DebugColor.White, 17592186044416UL);
				}
				Debug.Print("_teamAISiegeAttacker.PrimarySiegeWeapons.Count " + this._teamAISiegeAttacker.PrimarySiegeWeapons.Count, 0, Debug.DebugColor.White, 17592186044416UL);
				List<SiegeLadder> list = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeLadder>().ToList<SiegeLadder>();
				Debug.Print("ladders.Count = " + list.Count, 0, Debug.DebugColor.White, 17592186044416UL);
				List<SiegeTower> list2 = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeTower>().ToList<SiegeTower>();
				Debug.Print("towers.Count = " + list2.Count, 0, Debug.DebugColor.White, 17592186044416UL);
				BatteringRam batteringRam = Mission.Current.ActiveMissionObjects.FindAllWithType<BatteringRam>().FirstOrDefault<BatteringRam>();
				Debug.Print("ram = " + batteringRam, 0, Debug.DebugColor.White, 17592186044416UL);
			}
			this.AssignMeleeFormationsToLanes(this._meleeFormations, currentLanes);
			foreach (Formation formation in this._rangedFormations)
			{
				formation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(formation);
				formation.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
			}
			if (archerPositions.Count > 0)
			{
				using (IEnumerator<Formation> enumerator2 = this._rangedFormations.OrderByDescending((Formation rf) => rf.CountOfUnits).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Formation rangedFormation = enumerator2.Current;
						if (archerPositions.IsEmpty<ArcherPosition>())
						{
							archerPositions.AddRange(this._teamAISiegeAttacker.ArcherPositions);
						}
						ArcherPosition archerPosition = null;
						if (rangedFormation.AI.ActiveBehavior is BehaviorSparseSkirmish)
						{
							archerPosition = archerPositions.FirstOrDefault((ArcherPosition ap) => ap.Entity == (rangedFormation.AI.ActiveBehavior as BehaviorSparseSkirmish).ArcherPosition);
						}
						if (archerPosition != null)
						{
							rangedFormation.AI.SetBehaviorWeight<BehaviorSparseSkirmish>(1f);
							archerPositions.Remove(archerPosition);
						}
						else
						{
							ArcherPosition archerPosition2 = archerPositions.MinBy((ArcherPosition ap) => ap.Entity.GlobalPosition.AsVec2.DistanceSquared(rangedFormation.QuerySystem.AveragePosition));
							rangedFormation.AI.SetBehaviorWeight<BehaviorSparseSkirmish>(1f);
							rangedFormation.AI.GetBehavior<BehaviorSparseSkirmish>().ArcherPosition = archerPosition2.Entity;
							archerPosition2.SetLastAssignedFormation(base.Team.TeamIndex, rangedFormation);
							archerPositions.Remove(archerPosition2);
						}
					}
				}
			}
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x00039D7C File Offset: 0x00037F7C
		private void AllInAssault()
		{
			List<Formation> list = base.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0).ToList<Formation>();
			List<SiegeLane> list2 = this.DetermineCurrentLanes();
			this.AssignMeleeFormationsToLanes(list, list2);
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x00039DC8 File Offset: 0x00037FC8
		private void StartTacticalRetreat()
		{
			this.StopUsingAllMachines();
			foreach (Formation formation in base.FormationsIncludingSpecialAndEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.AI.ResetBehaviorWeights();
					TacticComponent.SetDefaultBehaviorWeights(formation);
					formation.AI.SetBehaviorWeight<BehaviorRetreatToKeep>(1f);
				}
			}
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x00039E48 File Offset: 0x00038048
		protected override bool CheckAndSetAvailableFormationsChanged()
		{
			bool flag = false;
			int count = this.DetermineCurrentLanes().Count;
			if (this._laneCount != count)
			{
				this._laneCount = count;
				flag = true;
			}
			int aicontrolledFormationCount = base.Team.GetAIControlledFormationCount();
			bool flag2 = aicontrolledFormationCount != this._AIControlledFormationCount;
			if (flag2)
			{
				this._AIControlledFormationCount = aicontrolledFormationCount;
				this.IsTacticReapplyNeeded = true;
			}
			bool flag3 = false;
			bool flag4 = false;
			if (this._tacticState == TacticBreachWalls.TacticState.AssaultUnderRangedCover)
			{
				int num = 0;
				int num2 = 0;
				foreach (Formation formation in base.Team.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnitsWithoutDetachedOnes > 0)
					{
						if (formation.QuerySystem.IsInfantryFormation)
						{
							num++;
						}
						if (formation.QuerySystem.IsRangedFormation)
						{
							num2++;
						}
					}
				}
				if (this._meleeFormations.Count == num && this._rangedFormations.Count == num2)
				{
					goto IL_1AC;
				}
				flag3 = true;
				this._meleeFormations.Clear();
				this._rangedFormations.Clear();
				using (List<Formation>.Enumerator enumerator = base.Team.FormationsIncludingEmpty.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation2 = enumerator.Current;
						if (formation2.CountOfUnitsWithoutDetachedOnes > 0)
						{
							if (formation2.QuerySystem.IsInfantryFormation)
							{
								this._meleeFormations.Add(formation2);
							}
							if (formation2.QuerySystem.IsRangedFormation)
							{
								this._rangedFormations.Add(formation2);
							}
						}
					}
					goto IL_1AC;
				}
			}
			if (this._tacticState == TacticBreachWalls.TacticState.TotalAttack)
			{
				int formationCount = base.Team.GetFormationCount();
				if ((formationCount < count && aicontrolledFormationCount > 0) || (formationCount > count && (formationCount - aicontrolledFormationCount < count || aicontrolledFormationCount > 1)))
				{
					flag4 = true;
				}
			}
			IL_1AC:
			return flag || flag2 || flag3 || flag4;
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x0003A028 File Offset: 0x00038228
		private void MergeFormationsIfLanesBecameUnavailable(ref List<SiegeLane> currentLanes)
		{
			int count = currentLanes.Count;
			if (this._laneCount > count)
			{
				List<Formation> list = new List<Formation>();
				int num = 0;
				List<Formation> list2 = new List<Formation>();
				int num2 = 0;
				for (int i = 0; i < this._cachedUsedSiegeLanes.Count; i++)
				{
					bool flag = false;
					SiegeLane siegeLane = this._cachedUsedSiegeLanes[i];
					for (int j = 0; j < currentLanes.Count; j++)
					{
						if (siegeLane == currentLanes[j])
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						Formation formation = siegeLane.GetLastAssignedFormation(base.Team.TeamIndex);
						if (formation != null && formation.IsSplittableByAI)
						{
							num += formation.CountOfUnits;
							list.Add(formation);
						}
					}
					else
					{
						Formation formation = siegeLane.GetLastAssignedFormation(base.Team.TeamIndex);
						if (formation != null)
						{
							num2 += formation.CountOfUnits;
							list2.Add(formation);
						}
					}
				}
				int num3 = MathF.Ceiling((float)(num + num2) / (float)list2.Count);
				for (int k = 0; k < list.Count; k++)
				{
					Formation formation2 = list[k];
					int num4 = formation2.CountOfUnits;
					for (int l = 0; l < list2.Count; l++)
					{
						Formation formation3 = list2[l];
						int num5 = num3 - formation3.CountOfUnits;
						if (num5 > 0)
						{
							int num6 = MathF.Min(num4, num5);
							num4 -= num6;
							formation2.TransferUnits(formation3, num6);
						}
					}
				}
				this._AIControlledFormationCount -= num;
			}
			this._cachedUsedSiegeLanes = currentLanes;
			this._laneCount = currentLanes.Count;
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x0003A1C8 File Offset: 0x000383C8
		private void MergeFormationsIfArcherPositionsBecameUnavailable(ref List<ArcherPosition> currentArcherPositions)
		{
			int count = currentArcherPositions.Count;
			if (this._cachedUsedArcherPositions.Count > count)
			{
				List<Formation> list = new List<Formation>();
				int num = 0;
				List<Formation> list2 = new List<Formation>();
				int num2 = 0;
				for (int i = 0; i < this._cachedUsedArcherPositions.Count; i++)
				{
					bool flag = false;
					ArcherPosition archerPosition = this._cachedUsedArcherPositions[i];
					for (int j = 0; j < currentArcherPositions.Count; j++)
					{
						if (archerPosition == currentArcherPositions[j])
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						Formation formation = archerPosition.GetLastAssignedFormation(base.Team.TeamIndex);
						if (formation != null && formation.IsSplittableByAI)
						{
							num += formation.CountOfUnits;
							list.Add(formation);
						}
					}
					else
					{
						Formation formation = archerPosition.GetLastAssignedFormation(base.Team.TeamIndex);
						if (formation != null)
						{
							num2 += formation.CountOfUnits;
							list2.Add(formation);
						}
					}
				}
				int num3 = MathF.Ceiling((float)(num + num2) / (float)list2.Count);
				for (int k = 0; k < list.Count; k++)
				{
					Formation formation2 = list[k];
					int num4 = formation2.CountOfUnits;
					for (int l = 0; l < list2.Count; l++)
					{
						Formation formation3 = list2[l];
						int num5 = num3 - formation3.CountOfUnits;
						if (num5 > 0)
						{
							int num6 = MathF.Min(num4, num5);
							num4 -= num6;
							formation2.TransferUnits(formation3, num6);
						}
					}
				}
				this._AIControlledFormationCount -= num;
			}
			this._cachedUsedArcherPositions = currentArcherPositions;
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x0003A360 File Offset: 0x00038560
		protected override void ManageFormationCounts()
		{
			List<SiegeLane> list = this.DetermineCurrentLanes();
			if (this._indicators == null && base.Team.QuerySystem.EnemyUnitCount > 0)
			{
				this._indicators = new TacticBreachWalls.BreachWallsProgressIndicators(base.Team, list);
			}
			if (this._tacticState == TacticBreachWalls.TacticState.Retreating)
			{
				return;
			}
			int count = list.Count;
			if (this._tacticState == TacticBreachWalls.TacticState.AssaultUnderRangedCover)
			{
				int num = MathF.Min(this.DetermineCurrentArcherPositions(list).Count, 8 - count);
				base.ManageFormationCounts(count, num, 0, 0);
				this._meleeFormations = base.FormationsIncludingEmpty.Where((Formation f) => f.QuerySystem.IsInfantryFormation && f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
				this._rangedFormations = base.FormationsIncludingEmpty.Where((Formation f) => f.QuerySystem.IsRangedFormation && f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
				return;
			}
			if (this._tacticState == TacticBreachWalls.TacticState.TotalAttack)
			{
				base.SplitFormationClassIntoGivenNumber((Formation f) => true, count);
			}
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x0003A478 File Offset: 0x00038678
		private void CheckAndChangeState()
		{
			if (this._tacticState == TacticBreachWalls.TacticState.Retreating)
			{
				return;
			}
			bool isShockAssault = this._isShockAssault;
			List<SiegeLane> list = this.DetermineCurrentLanes();
			int num = 0;
			foreach (Formation formation in base.Team.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0 && TeamAISiegeComponent.IsFormationInsideCastle(formation, true, 0.4f))
				{
					num++;
				}
			}
			if (this.ShouldRetreat(list, num))
			{
				this._tacticState = TacticBreachWalls.TacticState.Retreating;
				this.StartTacticalRetreat();
				this.IsTacticReapplyNeeded = false;
				return;
			}
			TacticBreachWalls.TacticState tacticState = TacticBreachWalls.TacticState.TotalAttack;
			List<ArcherPosition> list2 = null;
			if (this._tacticState != TacticBreachWalls.TacticState.TotalAttack)
			{
				list2 = this.DetermineCurrentArcherPositions(list);
				if (list2.Count > 0)
				{
					int num2 = MathF.Max(this._meleeFormations.Sum((Formation mf) => mf.CountOfUnits), 1);
					int num3 = MathF.Max(this._rangedFormations.Sum((Formation rf) => rf.CountOfUnits), 1);
					int num4 = num2 + num3;
					int num5 = num4 - base.Team.FormationsIncludingEmpty.Sum((Formation f) => f.CountOfUnitsWithoutDetachedOnes);
					tacticState = (((float)num2 / (float)num3 > 0.5f && (float)num5 / (float)num4 < 0.2f) ? TacticBreachWalls.TacticState.AssaultUnderRangedCover : TacticBreachWalls.TacticState.TotalAttack);
				}
			}
			if (tacticState != this._tacticState || isShockAssault != this._isShockAssault)
			{
				if (tacticState == TacticBreachWalls.TacticState.AssaultUnderRangedCover)
				{
					this._tacticState = TacticBreachWalls.TacticState.AssaultUnderRangedCover;
					this.ManageFormationCounts();
					this.WellRoundedAssault(ref list, ref list2);
					this.IsTacticReapplyNeeded = false;
					return;
				}
				if (tacticState != TacticBreachWalls.TacticState.TotalAttack)
				{
					return;
				}
				this._tacticState = TacticBreachWalls.TacticState.TotalAttack;
				this.ManageFormationCounts();
				this.AllInAssault();
				this.IsTacticReapplyNeeded = false;
			}
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x0003A658 File Offset: 0x00038858
		private List<SiegeLane> DetermineCurrentLanes()
		{
			List<SiegeLane> list = TeamAISiegeComponent.SiegeLanes.Where((SiegeLane sl) => sl.IsBreach).ToList<SiegeLane>();
			if (list.Count >= 2)
			{
				if (this._indicators == null || this._indicators.InitialUnitCount > 100)
				{
					return list;
				}
				if (!this._isShockAssault)
				{
					this.StopUsingAllMachines();
					this._isShockAssault = true;
				}
				return list.Take(1).ToList<SiegeLane>();
			}
			else
			{
				List<SiegeLane> list2 = TeamAISiegeComponent.SiegeLanes.Where((SiegeLane sl) => !sl.CalculateIsLaneUnusable()).ToList<SiegeLane>();
				if (list2.Count <= 0)
				{
					return TeamAISiegeComponent.SiegeLanes.Where((SiegeLane sl) => sl.HasGate).ToList<SiegeLane>();
				}
				if (this._indicators != null && this._indicators.InitialUnitCount <= 100)
				{
					List<SiegeLane> list3 = new List<SiegeLane>();
					list3.Add(list2.MaxBy((SiegeLane ul) => ul.CalculateLaneCapacity()));
					if (!this._isShockAssault)
					{
						this.StopUsingAllMachines();
						this._isShockAssault = true;
					}
					return list3;
				}
				if (list.Count >= 1)
				{
					return list2.Where(delegate(SiegeLane l)
					{
						if (!l.IsBreach)
						{
							return l.PrimarySiegeWeapons.Any((IPrimarySiegeWeapon psw) => !(psw is SiegeLadder));
						}
						return true;
					}).ToList<SiegeLane>();
				}
				return list2;
			}
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x0003A7DC File Offset: 0x000389DC
		private List<ArcherPosition> DetermineCurrentArcherPositions(List<SiegeLane> currentLanes)
		{
			return this._teamAISiegeAttacker.ArcherPositions.Where((ArcherPosition ap) => currentLanes.Any((SiegeLane cl) => ap.IsArcherPositionRelatedToSide(cl.LaneSide))).ToList<ArcherPosition>();
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x0003A818 File Offset: 0x00038A18
		protected internal override void TickOccasionally()
		{
			if (!base.AreFormationsCreated)
			{
				return;
			}
			this._meleeFormations.RemoveAll((Formation mf) => mf.CountOfUnitsWithoutDetachedOnes == 0);
			this._rangedFormations.RemoveAll((Formation rf) => rf.CountOfUnitsWithoutDetachedOnes == 0);
			List<SiegeLane> list = this.DetermineCurrentLanes();
			this.MergeFormationsIfLanesBecameUnavailable(ref list);
			bool flag = this.CheckAndSetAvailableFormationsChanged();
			if (this._indicators == null && base.Team.QuerySystem.EnemyUnitCount > 0)
			{
				this._indicators = new TacticBreachWalls.BreachWallsProgressIndicators(base.Team, list);
				this._indicators.StartingPowerRatio = base.Team.QuerySystem.TotalPowerRatio;
				this._indicators.InitialLaneCount = list.Count;
				this._indicators.InitialUnitCount = base.Team.QuerySystem.AllyUnitCount;
			}
			int num = 0;
			foreach (SiegeLane siegeLane in list)
			{
				num |= MathF.PowTwo32((int)siegeLane.LaneSide);
			}
			this.IsTacticReapplyNeeded = num != this._lanesInUse;
			this._lanesInUse = num;
			if (flag)
			{
				this.ManageFormationCounts();
			}
			this.CheckAndChangeState();
			switch (this._tacticState)
			{
			case TacticBreachWalls.TacticState.AssaultUnderRangedCover:
			{
				List<ArcherPosition> list2 = this.DetermineCurrentArcherPositions(list);
				if (flag || this.IsTacticReapplyNeeded)
				{
					this._cachedUsedArcherPositions = list2;
					this.WellRoundedAssault(ref list, ref list2);
					this.IsTacticReapplyNeeded = false;
				}
				else if (this._cachedUsedArcherPositions.Count != list2.Count)
				{
					this.MergeFormationsIfArcherPositionsBecameUnavailable(ref list2);
				}
				this.BalanceAssaultLanes(this._meleeFormations.Where((Formation mf) => mf.IsAIControlled && mf.IsAITickedAfterSplit && (mf.AI.ActiveBehavior is BehaviorUseSiegeMachines || mf.AI.ActiveBehavior is BehaviorWaitForLadders)).ToList<Formation>());
				break;
			}
			case TacticBreachWalls.TacticState.TotalAttack:
				if (flag || this.IsTacticReapplyNeeded)
				{
					this.AllInAssault();
					this.IsTacticReapplyNeeded = false;
				}
				this.BalanceAssaultLanes(base.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && f.IsAIControlled && f.IsAITickedAfterSplit && (f.AI.ActiveBehavior is BehaviorUseSiegeMachines || f.AI.ActiveBehavior is BehaviorWaitForLadders)).ToList<Formation>());
				break;
			case TacticBreachWalls.TacticState.Retreating:
				if (flag || this.IsTacticReapplyNeeded)
				{
					this.StartTacticalRetreat();
					this.IsTacticReapplyNeeded = false;
				}
				break;
			}
			TeamAISiegeComponent teamAISiegeAttacker = this._teamAISiegeAttacker;
			bool flag2;
			if (list.Count((SiegeLane l) => l.IsBreach) <= 1)
			{
				if (list.Any((SiegeLane l) => l.PrimarySiegeWeapons.Any((IPrimarySiegeWeapon psw) => psw.HoldLadders)))
				{
					flag2 = list.Any((SiegeLane l) => l.PrimarySiegeWeapons.Any((IPrimarySiegeWeapon psw) => psw.SendLadders));
					goto IL_2D5;
				}
			}
			flag2 = true;
			IL_2D5:
			teamAISiegeAttacker.SetAreLaddersReady(flag2);
			this.CheckAndSetAvailableFormationsChanged();
			base.TickOccasionally();
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x0003AB1C File Offset: 0x00038D1C
		protected internal override float GetTacticWeight()
		{
			return 10f;
		}

		// Token: 0x04000474 RID: 1140
		public const float SameBehaviorFactor = 3f;

		// Token: 0x04000475 RID: 1141
		public const float SameSideFactor = 5f;

		// Token: 0x04000476 RID: 1142
		private const int ShockAssaultThresholdCount = 100;

		// Token: 0x04000477 RID: 1143
		private readonly TeamAISiegeAttacker _teamAISiegeAttacker;

		// Token: 0x04000478 RID: 1144
		private TacticBreachWalls.BreachWallsProgressIndicators _indicators;

		// Token: 0x04000479 RID: 1145
		private List<Formation> _meleeFormations;

		// Token: 0x0400047A RID: 1146
		private List<Formation> _rangedFormations;

		// Token: 0x0400047B RID: 1147
		private int _laneCount;

		// Token: 0x0400047C RID: 1148
		private List<SiegeLane> _cachedUsedSiegeLanes;

		// Token: 0x0400047D RID: 1149
		private int _lanesInUse;

		// Token: 0x0400047E RID: 1150
		private List<ArcherPosition> _cachedUsedArcherPositions;

		// Token: 0x0400047F RID: 1151
		private TacticBreachWalls.TacticState _tacticState;

		// Token: 0x04000480 RID: 1152
		private bool _isShockAssault;

		// Token: 0x0200049E RID: 1182
		private class BreachWallsProgressIndicators
		{
			// Token: 0x06003712 RID: 14098 RVA: 0x000E22B4 File Offset: 0x000E04B4
			public BreachWallsProgressIndicators(Team team, List<SiegeLane> lanes)
			{
				this.StartingPowerRatio = team.QuerySystem.RemainingPowerRatio;
				this.InitialUnitCount = team.QuerySystem.AllyUnitCount;
				this.InitialLaneCount = ((this.InitialUnitCount > 100) ? lanes.Count : 1);
				this._insideFormationEffect = 1f / (float)this.InitialLaneCount;
				this._openLaneEffect = 0.7f / (float)this.InitialLaneCount;
				this._existingLaneEffect = 0.4f / (float)this.InitialLaneCount;
			}

			// Token: 0x06003713 RID: 14099 RVA: 0x000E233C File Offset: 0x000E053C
			public float GetRetreatThresholdRatio(List<SiegeLane> lanes, int insideFormationCount)
			{
				float num = 1f;
				num -= (float)insideFormationCount * this._insideFormationEffect;
				int num2 = lanes.Count((SiegeLane l) => !l.IsOpen);
				int num3 = lanes.Count - num2 - insideFormationCount;
				if (num3 > 0)
				{
					num -= (float)num3 * this._openLaneEffect;
				}
				return num - (float)num2 * this._existingLaneEffect;
			}

			// Token: 0x040019CB RID: 6603
			public float StartingPowerRatio;

			// Token: 0x040019CC RID: 6604
			public int InitialLaneCount;

			// Token: 0x040019CD RID: 6605
			public int InitialUnitCount;

			// Token: 0x040019CE RID: 6606
			private readonly float _insideFormationEffect;

			// Token: 0x040019CF RID: 6607
			private readonly float _openLaneEffect;

			// Token: 0x040019D0 RID: 6608
			private readonly float _existingLaneEffect;
		}

		// Token: 0x0200049F RID: 1183
		private enum TacticState
		{
			// Token: 0x040019D2 RID: 6610
			Unset,
			// Token: 0x040019D3 RID: 6611
			AssaultUnderRangedCover,
			// Token: 0x040019D4 RID: 6612
			TotalAttack,
			// Token: 0x040019D5 RID: 6613
			Retreating
		}
	}
}
