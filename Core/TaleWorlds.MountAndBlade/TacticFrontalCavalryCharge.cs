using System;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200015D RID: 349
	public class TacticFrontalCavalryCharge : TacticComponent
	{
		// Token: 0x060011C5 RID: 4549 RVA: 0x000417B1 File Offset: 0x0003F9B1
		public TacticFrontalCavalryCharge(Team team)
			: base(team)
		{
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x000417BC File Offset: 0x0003F9BC
		protected override void ManageFormationCounts()
		{
			base.ManageFormationCounts(1, 1, 1, 1);
			this._mainInfantry = TacticComponent.ChooseAndSortByPriority(base.FormationsIncludingEmpty, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation, (Formation f) => f.IsAIControlled, (Formation f) => f.QuerySystem.FormationPower).FirstOrDefault<Formation>();
			if (this._mainInfantry != null)
			{
				this._mainInfantry.AI.IsMainFormation = true;
			}
			this._archers = TacticComponent.ChooseAndSortByPriority(base.FormationsIncludingEmpty, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsRangedFormation, (Formation f) => f.IsAIControlled, (Formation f) => f.QuerySystem.FormationPower).FirstOrDefault<Formation>();
			this._cavalry = TacticComponent.ChooseAndSortByPriority(base.FormationsIncludingEmpty, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsCavalryFormation, (Formation f) => f.IsAIControlled, (Formation f) => f.QuerySystem.FormationPower).FirstOrDefault<Formation>();
			this._rangedCavalry = TacticComponent.ChooseAndSortByPriority(base.FormationsIncludingEmpty, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsRangedCavalryFormation, (Formation f) => f.IsAIControlled, (Formation f) => f.QuerySystem.FormationPower).FirstOrDefault<Formation>();
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x000419B8 File Offset: 0x0003FBB8
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
				this._mainInfantry.AI.SetBehaviorWeight<BehaviorAdvance>(1f);
			}
			if (this._archers != null)
			{
				this._archers.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._archers);
				this._archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(1f);
				this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
				this._archers.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
			}
			if (this._cavalry != null)
			{
				this._cavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._cavalry);
				this._cavalry.AI.SetBehaviorWeight<BehaviorAdvance>(1f);
				this._cavalry.AI.SetBehaviorWeight<BehaviorVanguard>(1f);
			}
			if (this._rangedCavalry != null)
			{
				this._rangedCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
			}
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x00041B34 File Offset: 0x0003FD34
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
				this._mainInfantry.AI.SetBehaviorWeight<BehaviorAdvance>(1f);
				this._mainInfantry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
			}
			if (this._archers != null)
			{
				this._archers.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._archers);
				this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
				this._archers.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
			}
			if (this._cavalry != null)
			{
				this._cavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._cavalry);
				this._cavalry.AI.SetBehaviorWeight<BehaviorFlank>(1f);
				this._cavalry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
			}
			if (this._rangedCavalry != null)
			{
				this._rangedCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
			}
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x00041CB0 File Offset: 0x0003FEB0
		private bool HasBattleBeenJoined()
		{
			Formation cavalry = this._cavalry;
			return ((cavalry != null) ? cavalry.QuerySystem.ClosestEnemyFormation : null) == null || this._cavalry.AI.ActiveBehavior is BehaviorCharge || this._cavalry.AI.ActiveBehavior is BehaviorTacticalCharge || this._cavalry.QuerySystem.MedianPosition.AsVec2.Distance(this._cavalry.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / this._cavalry.QuerySystem.ClosestEnemyFormation.MovementSpeedMaximum <= 7f + (this._hasBattleBeenJoined ? 7f : 0f);
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x00041D80 File Offset: 0x0003FF80
		protected override bool CheckAndSetAvailableFormationsChanged()
		{
			int aicontrolledFormationCount = base.Team.GetAIControlledFormationCount();
			bool flag = aicontrolledFormationCount != this._AIControlledFormationCount;
			if (flag)
			{
				this._AIControlledFormationCount = aicontrolledFormationCount;
				this.IsTacticReapplyNeeded = true;
			}
			return flag || (this._mainInfantry != null && (this._mainInfantry.CountOfUnits == 0 || !this._mainInfantry.QuerySystem.IsInfantryFormation)) || (this._archers != null && (this._archers.CountOfUnits == 0 || !this._archers.QuerySystem.IsRangedFormation)) || (this._cavalry != null && (this._cavalry.CountOfUnits == 0 || !this._cavalry.QuerySystem.IsCavalryFormation)) || (this._rangedCavalry != null && (this._rangedCavalry.CountOfUnits == 0 || !this._rangedCavalry.QuerySystem.IsRangedCavalryFormation));
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x00041E64 File Offset: 0x00040064
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

		// Token: 0x060011CC RID: 4556 RVA: 0x00041EE8 File Offset: 0x000400E8
		protected internal override float GetTacticWeight()
		{
			float num = base.Team.QuerySystem.RangedCavalryRatio * (float)base.Team.QuerySystem.MemberCount;
			return base.Team.QuerySystem.CavalryRatio * (float)base.Team.QuerySystem.MemberCount / ((float)base.Team.QuerySystem.MemberCount - num) * MathF.Sqrt(base.Team.QuerySystem.RemainingPowerRatio);
		}

		// Token: 0x040004B0 RID: 1200
		private Formation _cavalry;

		// Token: 0x040004B1 RID: 1201
		private bool _hasBattleBeenJoined;
	}
}
