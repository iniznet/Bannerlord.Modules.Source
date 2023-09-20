﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TacticSallyOutHitAndRun : TacticComponent
	{
		protected override void ManageFormationCounts()
		{
			List<IPrimarySiegeWeapon> list = this._teamAISallyOutAttacker.PrimarySiegeWeapons.Where(delegate(IPrimarySiegeWeapon psw)
			{
				SiegeWeapon siegeWeapon;
				return (siegeWeapon = psw as SiegeWeapon) != null && !siegeWeapon.IsDisabled && siegeWeapon.IsDestructible;
			}).ToList<IPrimarySiegeWeapon>();
			int num = list.Count;
			bool flag = false;
			foreach (UsableMachine usableMachine in this._teamAISallyOutAttacker.BesiegerRangedSiegeWeapons)
			{
				if (!usableMachine.IsDisabled && !usableMachine.IsDestroyed)
				{
					flag = true;
					break;
				}
			}
			num = MathF.Max(num, 1 + ((list.Count > 0 && flag) ? 1 : 0));
			int num2 = MathF.Min(this._teamAISallyOutAttacker.ArcherPositions.Count<GameEntity>(), 7 - num);
			object obj = base.FormationsIncludingEmpty.Count((Formation f) => f.CountOfUnits > 0 && (f.QuerySystem.IsCavalryFormation || f.QuerySystem.IsRangedCavalryFormation)) > 0 && base.Team.QuerySystem.CavalryRatio + base.Team.QuerySystem.RangedCavalryRatio > 0.1f;
			bool flag2 = true;
			object obj2 = obj;
			if (obj2 == null)
			{
				num = 1;
				flag2 = base.FormationsIncludingEmpty.Count((Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation) > 0 && base.Team.QuerySystem.CavalryRatio + base.Team.QuerySystem.RangedCavalryRatio + base.Team.QuerySystem.InfantryRatio > 0.15f;
				if (!flag2)
				{
					num2 = 1;
				}
			}
			base.SplitFormationClassIntoGivenNumber((Formation f) => f.QuerySystem.IsInfantryFormation, 1);
			base.SplitFormationClassIntoGivenNumber((Formation f) => f.QuerySystem.IsRangedFormation, num2);
			base.SplitFormationClassIntoGivenNumber((Formation f) => f.QuerySystem.IsCavalryFormation || f.QuerySystem.IsRangedCavalryFormation, num);
			this._mainInfantryFormation = base.FormationsIncludingEmpty.FirstOrDefault((Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation);
			if (this._mainInfantryFormation != null)
			{
				this._mainInfantryFormation.AI.IsMainFormation = true;
			}
			this._archerFormations = base.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsRangedFormation).ToMBList<Formation>();
			this._cavalryFormations.Clear();
			this._cavalryFormations = base.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && (f.QuerySystem.IsCavalryFormation || f.QuerySystem.IsRangedCavalryFormation)).ToMBList<Formation>();
			if (obj2 == null)
			{
				if (this._mainInfantryFormation != null)
				{
					this._cavalryFormations.Add(this._mainInfantryFormation);
					this._mainInfantryFormation.AI.IsMainFormation = false;
					this._mainInfantryFormation = null;
				}
				if (!flag2)
				{
					this._cavalryFormations.AddRange(this._archerFormations);
					this._archerFormations.Clear();
				}
			}
			bool flag3 = list.Count == 0 || (list.Count == 1 && flag && this._cavalryFormations.Count + ((this._mainInfantryFormation != null) ? 1 : 0) > 1);
			for (int i = 0; i < this._cavalryFormations.Count - (flag3 ? 1 : 0); i++)
			{
				this._cavalryFormations[i].AI.Side = list[i % list.Count].WeaponSide;
			}
			for (int j = 0; j < this._archerFormations.Count - (flag3 ? 1 : 0); j++)
			{
				this._archerFormations[j].AI.Side = list[j % list.Count].WeaponSide;
			}
			if (this._cavalryFormations.Count > 0 && flag3)
			{
				if (list.Any((IPrimarySiegeWeapon psw) => psw != null && psw.WeaponSide == FormationAI.BehaviorSide.Middle))
				{
					this._cavalryFormations[0].AI.Side = FormationAI.BehaviorSide.Left;
				}
				else
				{
					this._cavalryFormations[this._cavalryFormations.Count - 1].AI.Side = FormationAI.BehaviorSide.Middle;
				}
			}
			if (this._archerFormations.Count > 0 && flag3)
			{
				if (list.Any((IPrimarySiegeWeapon psw) => psw != null && psw.WeaponSide == FormationAI.BehaviorSide.Middle))
				{
					this._archerFormations[0].AI.Side = FormationAI.BehaviorSide.Left;
				}
				else
				{
					this._archerFormations[this._archerFormations.Count - 1].AI.Side = FormationAI.BehaviorSide.Middle;
				}
			}
			this._AIControlledFormationCount = base.Team.GetAIControlledFormationCount();
		}

		private void DestroySiegeWeapons()
		{
			if (this._mainInfantryFormation != null)
			{
				this._mainInfantryFormation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._mainInfantryFormation);
				BehaviorDefend behaviorDefend = this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorDefend>(1f);
				Vec2 vec = (this._teamAISallyOutAttacker.OuterGate.GameEntity.GlobalPosition.AsVec2 - this._teamAISallyOutAttacker.InnerGate.GameEntity.GlobalPosition.AsVec2).Normalized();
				WorldPosition worldPosition = new WorldPosition(this._mainInfantryFormation.Team.Mission.Scene, UIntPtr.Zero, this._teamAISallyOutAttacker.OuterGate.GameEntity.GlobalPosition, false);
				worldPosition.SetVec2(worldPosition.AsVec2 + (3f + this._mainInfantryFormation.Depth) * vec);
				behaviorDefend.DefensePosition = worldPosition;
				this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorDestroySiegeWeapons>(1f);
				this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorCharge>(0.5f);
			}
			GameEntity[] array = this._teamAISallyOutAttacker.ArcherPositions.ToArray<GameEntity>();
			int num = array.Length;
			if (num > 0)
			{
				Formation[] array2 = this._archerFormations.ToArray();
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].AI.ResetBehaviorWeights();
					TacticComponent.SetDefaultBehaviorWeights(array2[i]);
					array2[i].AI.SetBehaviorWeight<BehaviorShootFromCastleWalls>(1f);
					array2[i].AI.GetBehavior<BehaviorShootFromCastleWalls>().ArcherPosition = array[i % num];
					array2[i].AI.SetBehaviorWeight<BehaviorDestroySiegeWeapons>(10f);
				}
			}
			foreach (Formation formation in this._cavalryFormations)
			{
				formation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(formation);
				formation.AI.SetBehaviorWeight<BehaviorDestroySiegeWeapons>(1f);
				formation.AI.SetBehaviorWeight<BehaviorCharge>(0.5f);
			}
		}

		private void CavalryRetreat()
		{
			if (this._mainInfantryFormation != null)
			{
				this._mainInfantryFormation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._mainInfantryFormation);
				BehaviorDefend behaviorDefend = this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorDefend>(1f);
				Vec2 vec = (this._teamAISallyOutAttacker.OuterGate.GameEntity.GlobalPosition.AsVec2 - this._teamAISallyOutAttacker.InnerGate.GameEntity.GlobalPosition.AsVec2).Normalized();
				WorldPosition worldPosition = new WorldPosition(this._mainInfantryFormation.Team.Mission.Scene, UIntPtr.Zero, this._teamAISallyOutAttacker.OuterGate.GameEntity.GlobalPosition, false);
				worldPosition.SetVec2(worldPosition.AsVec2 + (3f + this._mainInfantryFormation.Depth) * vec);
				behaviorDefend.DefensePosition = worldPosition;
			}
			GameEntity[] array = this._teamAISallyOutAttacker.ArcherPositions.ToArray<GameEntity>();
			int num = array.Length;
			if (num > 0)
			{
				Formation[] array2 = this._archerFormations.ToArray();
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].AI.ResetBehaviorWeights();
					TacticComponent.SetDefaultBehaviorWeights(array2[i]);
					array2[i].AI.SetBehaviorWeight<BehaviorShootFromCastleWalls>(1f);
					array2[i].AI.GetBehavior<BehaviorShootFromCastleWalls>().ArcherPosition = array[i % num];
					array2[i].AI.SetBehaviorWeight<BehaviorDestroySiegeWeapons>(10f);
				}
			}
			foreach (Formation formation in this._cavalryFormations)
			{
				formation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(formation);
				formation.AI.SetBehaviorWeight<BehaviorRetreatToCastle>(3f);
			}
		}

		private void InfantryRetreat()
		{
			if (this._mainInfantryFormation != null)
			{
				this._mainInfantryFormation.AI.Side = FormationAI.BehaviorSide.Middle;
				this._mainInfantryFormation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._mainInfantryFormation);
				this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorDefendCastleKeyPosition>(1f);
			}
		}

		private void HeadOutFromTheCastle()
		{
			if (this._mainInfantryFormation != null)
			{
				this._mainInfantryFormation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._mainInfantryFormation);
				this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorStop>(1000f);
			}
			GameEntity[] array = this._teamAISallyOutAttacker.ArcherPositions.ToArray<GameEntity>();
			if (array.Length != 0)
			{
				Formation[] array2 = this._archerFormations.ToArray();
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].AI.ResetBehaviorWeights();
					TacticComponent.SetDefaultBehaviorWeights(array2[i]);
					array2[i].AI.SetBehaviorWeight<BehaviorShootFromCastleWalls>(1f);
					array2[i].AI.GetBehavior<BehaviorShootFromCastleWalls>().ArcherPosition = array[i % array.Length];
				}
			}
			foreach (Formation formation in this._cavalryFormations)
			{
				formation.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(formation);
				formation.AI.SetBehaviorWeight<BehaviorDestroySiegeWeapons>(1f);
			}
		}

		public TacticSallyOutHitAndRun(Team team)
			: base(team)
		{
			this._archerFormations = new MBList<Formation>();
			this._cavalryFormations = new MBList<Formation>();
			this._teamAISallyOutAttacker = team.TeamAI as TeamAISallyOutAttacker;
			this._state = TacticSallyOutHitAndRun.TacticState.HeadingOutFromCastle;
			this._destructibleEnemySiegeWeapons = (from sw in Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeWeapon>()
				where sw.Side != team.Side && sw.IsDestructible
				select sw).ToList<SiegeWeapon>();
			this.ManageFormationCounts();
			this.HeadOutFromTheCastle();
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
			if (!flag && (this._mainInfantryFormation == null || (this._mainInfantryFormation.CountOfUnits != 0 && this._mainInfantryFormation.QuerySystem.IsInfantryFormation)))
			{
				if (this._archerFormations.Count > 0)
				{
					if (this._archerFormations.Any((Formation af) => af.CountOfUnits == 0 || !af.QuerySystem.IsRangedFormation))
					{
						return true;
					}
				}
				if (this._cavalryFormations.Count > 0)
				{
					return this._cavalryFormations.Any((Formation cf) => cf.CountOfUnits == 0);
				}
				return false;
			}
			return true;
		}

		private void CheckAndChangeState()
		{
			switch (this._state)
			{
			case TacticSallyOutHitAndRun.TacticState.HeadingOutFromCastle:
				if (this._cavalryFormations.All((Formation cf) => !TeamAISiegeComponent.IsFormationInsideCastle(cf, false, 0.4f)))
				{
					this._state = TacticSallyOutHitAndRun.TacticState.DestroyingSiegeWeapons;
					this.DestroySiegeWeapons();
					return;
				}
				break;
			case TacticSallyOutHitAndRun.TacticState.DestroyingSiegeWeapons:
				if (this._destructibleEnemySiegeWeapons.All((SiegeWeapon desw) => desw.IsDestroyed) || this._cavalryFormations.All(delegate(Formation cf)
				{
					if (!(cf.AI.ActiveBehavior is BehaviorDestroySiegeWeapons) || cf.GetReadonlyMovementOrderReference() == MovementOrder.MovementOrderRetreat)
					{
						return true;
					}
					if ((cf.AI.ActiveBehavior as BehaviorDestroySiegeWeapons).LastTargetWeapon == null)
					{
						return false;
					}
					Vec3 globalPosition = (cf.AI.ActiveBehavior as BehaviorDestroySiegeWeapons).LastTargetWeapon.GameEntity.GlobalPosition;
					return base.Team.QuerySystem.GetLocalEnemyPower(globalPosition.AsVec2) > base.Team.QuerySystem.GetLocalAllyPower(globalPosition.AsVec2) + cf.QuerySystem.FormationPower;
				}))
				{
					this._state = TacticSallyOutHitAndRun.TacticState.CavalryRetreating;
					this.CavalryRetreat();
					TeamAIComponent teamAI = base.Team.TeamAI;
					TacticalDecision tacticalDecision = new TacticalDecision(this, 31, null, null, null, null);
					teamAI.NotifyTacticalDecision(tacticalDecision);
					return;
				}
				break;
			case TacticSallyOutHitAndRun.TacticState.CavalryRetreating:
				if (this._cavalryFormations.IsEmpty<Formation>() || TeamAISiegeComponent.IsFormationGroupInsideCastle(this._cavalryFormations, false, 0.4f))
				{
					this._state = TacticSallyOutHitAndRun.TacticState.InfantryRetreating;
					this.InfantryRetreat();
				}
				break;
			default:
				return;
			}
		}

		protected internal override void TickOccasionally()
		{
			if (!base.AreFormationsCreated)
			{
				return;
			}
			if (this.CheckAndSetAvailableFormationsChanged() || this.IsTacticReapplyNeeded)
			{
				this.ManageFormationCounts();
				switch (this._state)
				{
				case TacticSallyOutHitAndRun.TacticState.HeadingOutFromCastle:
					this.HeadOutFromTheCastle();
					break;
				case TacticSallyOutHitAndRun.TacticState.DestroyingSiegeWeapons:
					this.DestroySiegeWeapons();
					break;
				case TacticSallyOutHitAndRun.TacticState.CavalryRetreating:
					this.CavalryRetreat();
					break;
				case TacticSallyOutHitAndRun.TacticState.InfantryRetreating:
					this.InfantryRetreat();
					break;
				}
				this.IsTacticReapplyNeeded = false;
			}
			this.CheckAndChangeState();
			base.TickOccasionally();
		}

		protected internal override float GetTacticWeight()
		{
			return 10f;
		}

		private TacticSallyOutHitAndRun.TacticState _state;

		private Formation _mainInfantryFormation;

		private MBList<Formation> _archerFormations;

		private MBList<Formation> _cavalryFormations;

		private readonly TeamAISallyOutAttacker _teamAISallyOutAttacker;

		private readonly List<SiegeWeapon> _destructibleEnemySiegeWeapons;

		private enum TacticState
		{
			HeadingOutFromCastle,
			DestroyingSiegeWeapons,
			CavalryRetreating,
			InfantryRetreating
		}
	}
}