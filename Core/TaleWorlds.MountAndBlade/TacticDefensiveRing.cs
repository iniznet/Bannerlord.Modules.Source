using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200015C RID: 348
	public class TacticDefensiveRing : TacticComponent
	{
		// Token: 0x060011B4 RID: 4532 RVA: 0x00040D7B File Offset: 0x0003EF7B
		public TacticDefensiveRing(Team team)
			: base(team)
		{
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x00040D84 File Offset: 0x0003EF84
		protected override void ManageFormationCounts()
		{
			base.AssignTacticFormations1121();
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x00040D8C File Offset: 0x0003EF8C
		private void Defend()
		{
			if (base.Team.IsPlayerTeam && !base.Team.IsPlayerGeneral && base.Team.IsPlayerSergeant)
			{
				base.SoundTacticalHorn(TacticComponent.MoveHornSoundIndex);
			}
			if (this._mainInfantry != null)
			{
				this._mainInfantry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
				this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefensiveRing>(1f).TacticalDefendPosition = this._mainRingPosition;
			}
			if (this._archers != null && this._mainInfantry != null)
			{
				this._archers.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._archers);
				this._archers.AI.SetBehaviorWeight<BehaviorFireFromInfantryCover>(1f);
				this._archers.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
			}
			if (this._leftCavalry != null)
			{
				this._leftCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._leftCavalry);
				this._leftCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Left;
				this._leftCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
			}
			if (this._rightCavalry != null)
			{
				this._rightCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
				this._rightCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Right;
				this._rightCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
			}
			if (this._rangedCavalry != null)
			{
				this._rangedCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
			}
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00040F5C File Offset: 0x0003F15C
		protected override bool CheckAndSetAvailableFormationsChanged()
		{
			int aicontrolledFormationCount = base.Team.GetAIControlledFormationCount();
			bool flag = aicontrolledFormationCount != this._AIControlledFormationCount;
			if (flag)
			{
				this._AIControlledFormationCount = aicontrolledFormationCount;
				this.IsTacticReapplyNeeded = true;
			}
			return flag || (this._mainInfantry != null && (this._mainInfantry.CountOfUnits == 0 || !this._mainInfantry.QuerySystem.IsInfantryFormation)) || (this._archers != null && (this._archers.CountOfUnits == 0 || !this._archers.QuerySystem.IsRangedFormation)) || (this._leftCavalry != null && (this._leftCavalry.CountOfUnits == 0 || !this._leftCavalry.QuerySystem.IsCavalryFormation)) || (this._rightCavalry != null && (this._rightCavalry.CountOfUnits == 0 || !this._rightCavalry.QuerySystem.IsCavalryFormation)) || (this._rangedCavalry != null && (this._rangedCavalry.CountOfUnits == 0 || !this._rangedCavalry.QuerySystem.IsRangedCavalryFormation));
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x0004106C File Offset: 0x0003F26C
		protected internal override void TickOccasionally()
		{
			if (!base.AreFormationsCreated)
			{
				return;
			}
			if (this.CheckAndSetAvailableFormationsChanged() || this.IsTacticReapplyNeeded)
			{
				this.ManageFormationCounts();
				this.Defend();
				this.IsTacticReapplyNeeded = false;
			}
			base.TickOccasionally();
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x000410A0 File Offset: 0x0003F2A0
		protected internal override bool ResetTacticalPositions()
		{
			this.DetermineRingPosition();
			return true;
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x000410AC File Offset: 0x0003F2AC
		protected internal override float GetTacticWeight()
		{
			if (base.Team.TeamAI.IsDefenseApplicable)
			{
				if (base.CheckAndDetermineFormation(ref this._mainInfantry, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation))
				{
					if (base.CheckAndDetermineFormation(ref this._archers, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsRangedFormation))
					{
						float num = (float)MathF.Max(0, this._mainInfantry.CountOfUnits) * (this._mainInfantry.MaximumInterval + this._mainInfantry.UnitDiameter) / 6.2831855f;
						float num2 = MathF.Sqrt((float)this._archers.CountOfUnits);
						float num3 = this._archers.UnitDiameter * num2 + this._archers.Interval * (num2 - 1f);
						if (num < num3)
						{
							return 0f;
						}
						if (!base.Team.TeamAI.IsCurrentTactic(this) || this._mainRingPosition == null || !this.IsTacticalPositionEligible(this._mainRingPosition))
						{
							this.DetermineRingPosition();
						}
						if (this._mainRingPosition == null)
						{
							return 0f;
						}
						return MathF.Min(base.Team.QuerySystem.InfantryRatio, base.Team.QuerySystem.RangedRatio) * 2f * 1.5f * this.GetTacticalPositionScore(this._mainRingPosition) * TacticComponent.CalculateNotEngagingTacticalAdvantage(base.Team.QuerySystem) / MathF.Sqrt(base.Team.QuerySystem.RemainingPowerRatio);
					}
				}
			}
			return 0f;
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x0004123C File Offset: 0x0003F43C
		private bool IsTacticalPositionEligible(TacticalPosition tacticalPosition)
		{
			Formation mainInfantry = this._mainInfantry;
			Vec2 vec;
			float num;
			if (mainInfantry == null)
			{
				vec = base.Team.QuerySystem.AveragePosition;
				num = vec.Distance(tacticalPosition.Position.AsVec2);
			}
			else
			{
				vec = mainInfantry.QuerySystem.AveragePosition;
				num = vec.Distance(tacticalPosition.Position.AsVec2);
			}
			float num2 = num;
			vec = base.Team.QuerySystem.AverageEnemyPosition;
			Formation mainInfantry2 = this._mainInfantry;
			float num3 = vec.Distance((mainInfantry2 != null) ? mainInfantry2.QuerySystem.AveragePosition : base.Team.QuerySystem.AveragePosition);
			return (num2 <= 20f || num2 <= num3 * 0.5f) && tacticalPosition.IsInsurmountable;
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x000412F8 File Offset: 0x0003F4F8
		private float GetTacticalPositionScore(TacticalPosition tacticalPosition)
		{
			if (base.CheckAndDetermineFormation(ref this._mainInfantry, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation))
			{
				if (base.CheckAndDetermineFormation(ref this._archers, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsRangedFormation))
				{
					float num = MBMath.Lerp(1f, 1.5f, MBMath.ClampFloat(tacticalPosition.Slope, 0f, 60f) / 60f, 1E-05f);
					Formation formation = base.Team.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsRangedFormation).MaxBy((Formation f) => f.CountOfUnits);
					float num2 = MathF.Max(formation.Arrangement.RankDepth, formation.Arrangement.FlankWidth);
					float num3 = MBMath.ClampFloat(tacticalPosition.Width / num2, 0.7f, 1f);
					float num4 = (tacticalPosition.IsInsurmountable ? 1.5f : 1f);
					float cavalryFactor = this.GetCavalryFactor(tacticalPosition);
					float num5 = this._mainInfantry.QuerySystem.AveragePosition.Distance(tacticalPosition.Position.AsVec2);
					float num6 = MBMath.Lerp(0.7f, 1f, (150f - MBMath.ClampFloat(num5, 50f, 150f)) / 100f, 1E-05f);
					return num * num3 * num4 * cavalryFactor * num6;
				}
			}
			return 0f;
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x000414A8 File Offset: 0x0003F6A8
		private List<TacticalPosition> ExtractPossibleTacticalPositionsFromTacticalRegion(TacticalRegion tacticalRegion)
		{
			List<TacticalPosition> list = new List<TacticalPosition>();
			foreach (TacticalPosition tacticalPosition in tacticalRegion.LinkedTacticalPositions)
			{
				if (tacticalPosition.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.HighGround)
				{
					list.Add(tacticalPosition);
				}
			}
			if (tacticalRegion.tacticalRegionType == TacticalRegion.TacticalRegionTypeEnum.DifficultTerrain || tacticalRegion.tacticalRegionType == TacticalRegion.TacticalRegionTypeEnum.Opening)
			{
				Vec2 vec = (base.Team.QuerySystem.AverageEnemyPosition - tacticalRegion.Position.AsVec2).Normalized();
				TacticalPosition tacticalPosition2 = new TacticalPosition(tacticalRegion.Position, vec, tacticalRegion.radius, 0f, true, TacticalPosition.TacticalPositionTypeEnum.Regional, tacticalRegion.tacticalRegionType);
				list.Add(tacticalPosition2);
			}
			return list;
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x00041574 File Offset: 0x0003F774
		private float GetCavalryFactor(TacticalPosition tacticalPosition)
		{
			if (tacticalPosition.TacticalRegionMembership == TacticalRegion.TacticalRegionTypeEnum.Opening)
			{
				return 1f;
			}
			float num = base.Team.QuerySystem.TeamPower;
			float num2 = 0f;
			foreach (Team team in base.Team.Mission.Teams)
			{
				if (team.IsEnemyOf(base.Team))
				{
					num2 += team.QuerySystem.TeamPower;
				}
			}
			num -= num * (base.Team.QuerySystem.CavalryRatio + base.Team.QuerySystem.RangedCavalryRatio) * 0.5f;
			num2 -= num2 * (base.Team.QuerySystem.EnemyCavalryRatio + base.Team.QuerySystem.EnemyRangedCavalryRatio) * 0.5f;
			return num / num2 / base.Team.QuerySystem.RemainingPowerRatio;
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x00041678 File Offset: 0x0003F878
		private void DetermineRingPosition()
		{
			IEnumerable<ValueTuple<TacticalPosition, float>> enumerable = from tp in base.Team.TeamAI.TacticalPositions
				where tp.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.HighGround && this.IsTacticalPositionEligible(tp)
				select new ValueTuple<TacticalPosition, float>(tp, this.GetTacticalPositionScore(tp));
			IEnumerable<ValueTuple<TacticalPosition, float>> enumerable2 = from tpftr in base.Team.TeamAI.TacticalRegions.SelectMany((TacticalRegion r) => this.ExtractPossibleTacticalPositionsFromTacticalRegion(r))
				where (tpftr.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.Regional || tpftr.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.HighGround) && this.IsTacticalPositionEligible(tpftr)
				select tpftr into tp
				select new ValueTuple<TacticalPosition, float>(tp, this.GetTacticalPositionScore(tp));
			List<ValueTuple<TacticalPosition, float>> list = enumerable.Concat(enumerable2).ToList<ValueTuple<TacticalPosition, float>>();
			if (list.Count > 0)
			{
				TacticalPosition item = list.MaxBy(([TupleElementNames(new string[] { "tp", null })] ValueTuple<TacticalPosition, float> pst) => pst.Item2).Item1;
				if (item != this._mainRingPosition)
				{
					this._mainRingPosition = item;
					this.IsTacticReapplyNeeded = true;
					return;
				}
			}
			else
			{
				this._mainRingPosition = null;
			}
		}

		// Token: 0x040004AE RID: 1198
		private const float DefendersAdvantage = 1.5f;

		// Token: 0x040004AF RID: 1199
		private TacticalPosition _mainRingPosition;
	}
}
