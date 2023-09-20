using System;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TacticRangedHarrassmentOffensive : TacticComponent
	{
		public TacticRangedHarrassmentOffensive(Team team)
			: base(team)
		{
		}

		protected override void ManageFormationCounts()
		{
			base.AssignTacticFormations1121();
		}

		private void Advance()
		{
			if (base.Team.IsPlayerTeam && !base.Team.IsPlayerGeneral && base.Team.IsPlayerSergeant)
			{
				base.SoundTacticalHorn(TacticComponent.MoveHornSoundIndex);
			}
			if (this._mainInfantry != null)
			{
				this._mainInfantry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
				this._mainInfantry.AI.SetBehaviorWeight<BehaviorCautiousAdvance>(1f);
			}
			if (this._archers != null)
			{
				this._archers.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._archers);
				this._archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(1f);
				this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
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

		private void Attack()
		{
			if (base.Team.IsPlayerTeam && !base.Team.IsPlayerGeneral && base.Team.IsPlayerSergeant)
			{
				base.SoundTacticalHorn(TacticComponent.AttackHornSoundIndex);
			}
			if (this._mainInfantry != null)
			{
				this._mainInfantry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
				this._mainInfantry.AI.SetBehaviorWeight<BehaviorCautiousAdvance>(1f);
				this._mainInfantry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
			}
			if (this._archers != null)
			{
				this._archers.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._archers);
				this._archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(1f);
				this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
				this._archers.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
			}
			if (this._leftCavalry != null)
			{
				this._leftCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._leftCavalry);
				this._leftCavalry.AI.SetBehaviorWeight<BehaviorFlank>(1f);
				this._leftCavalry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
			}
			if (this._rightCavalry != null)
			{
				this._rightCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
				this._rightCavalry.AI.SetBehaviorWeight<BehaviorFlank>(1f);
				this._rightCavalry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
			}
			if (this._rangedCavalry != null)
			{
				this._rangedCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
			}
		}

		private bool HasBattleBeenJoined()
		{
			Formation mainInfantry = this._mainInfantry;
			return ((mainInfantry != null) ? mainInfantry.QuerySystem.ClosestEnemyFormation : null) == null || this._mainInfantry.AI.ActiveBehavior is BehaviorCharge || this._mainInfantry.AI.ActiveBehavior is BehaviorTacticalCharge || this._mainInfantry.QuerySystem.MedianPosition.AsVec2.Distance(this._mainInfantry.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / this._mainInfantry.QuerySystem.ClosestEnemyFormation.MovementSpeedMaximum <= 5f + (this._hasBattleBeenJoined ? 5f : 0f);
		}

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

		protected internal override void TickOccasionally()
		{
			if (!base.AreFormationsCreated)
			{
				return;
			}
			if (this.CheckAndSetAvailableFormationsChanged())
			{
				this.ManageFormationCounts();
				if (this._hasBattleBeenJoined)
				{
					this.Attack();
				}
				else
				{
					this.Advance();
				}
				this.IsTacticReapplyNeeded = false;
			}
			bool flag = this.HasBattleBeenJoined();
			if (flag != this._hasBattleBeenJoined || this.IsTacticReapplyNeeded)
			{
				this._hasBattleBeenJoined = flag;
				if (this._hasBattleBeenJoined)
				{
					this.Attack();
				}
				else
				{
					this.Advance();
				}
				this.IsTacticReapplyNeeded = false;
			}
			base.TickOccasionally();
		}

		protected internal override float GetTacticWeight()
		{
			if (base.Team.FormationsIncludingEmpty.All((Formation f) => f.CountOfUnits == 0 || !f.QuerySystem.IsRangedFormation))
			{
				return 0f;
			}
			float num = base.Team.QuerySystem.RangedCavalryRatio * (float)base.Team.QuerySystem.MemberCount;
			float num2 = base.Team.QuerySystem.RangedRatio * (float)base.Team.QuerySystem.MemberCount / ((float)base.Team.QuerySystem.MemberCount - num);
			float num3 = base.Team.QuerySystem.RangedRatio + base.Team.QuerySystem.RangedCavalryRatio;
			float num4 = base.Team.QuerySystem.EnemyRangedRatio + base.Team.QuerySystem.EnemyRangedCavalryRatio;
			float num5 = MBMath.ClampFloat((num4 > 0f) ? (num3 / num4) : 2f, 0.5f, 2f);
			return num2 * num5 * MathF.Sqrt(base.Team.QuerySystem.RemainingPowerRatio);
		}

		private bool _hasBattleBeenJoined;
	}
}
