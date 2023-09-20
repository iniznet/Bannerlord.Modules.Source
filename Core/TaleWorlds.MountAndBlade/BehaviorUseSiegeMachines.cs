using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000125 RID: 293
	public class BehaviorUseSiegeMachines : BehaviorComponent
	{
		// Token: 0x06000DB8 RID: 3512 RVA: 0x00025D30 File Offset: 0x00023F30
		public BehaviorUseSiegeMachines(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			this._primarySiegeWeapons = new List<SiegeWeapon>();
			foreach (MissionObject missionObject in Mission.Current.ActiveMissionObjects)
			{
				IPrimarySiegeWeapon primarySiegeWeapon;
				if ((primarySiegeWeapon = missionObject as IPrimarySiegeWeapon) != null && primarySiegeWeapon.WeaponSide == this._behaviorSide)
				{
					this._primarySiegeWeapons.Add(missionObject as SiegeWeapon);
				}
			}
			this._teamAISiegeComponent = (TeamAISiegeComponent)formation.Team.TeamAI;
			base.BehaviorCoherence = 0f;
			this._stopOrder = MovementOrder.MovementOrderStop;
			this.RecreateFollowEntityOrder();
			if (this._followEntityOrder.OrderEnum != MovementOrder.MovementOrderEnum.Invalid)
			{
				this._behaviorState = BehaviorUseSiegeMachines.BehaviorState.Follow;
				base.CurrentOrder = this._followEntityOrder;
				return;
			}
			this._behaviorState = BehaviorUseSiegeMachines.BehaviorState.Stop;
			base.CurrentOrder = this._stopOrder;
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x00025E34 File Offset: 0x00024034
		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			return behaviorString;
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x00025E90 File Offset: 0x00024090
		private void RecreateFollowEntityOrder()
		{
			this._followEntityOrder = MovementOrder.MovementOrderStop;
			SiegeWeapon siegeWeapon = this._primarySiegeWeapons.FirstOrDefault(delegate(SiegeWeapon psw)
			{
				IPrimarySiegeWeapon primarySiegeWeapon;
				return !psw.IsDeactivated && (primarySiegeWeapon = psw as IPrimarySiegeWeapon) != null && !primarySiegeWeapon.HasCompletedAction();
			});
			this._followedEntity = ((siegeWeapon != null) ? siegeWeapon.WaitEntity : null);
			if (this._followedEntity != null)
			{
				this._followEntityOrder = MovementOrder.MovementOrderFollowEntity(this._followedEntity);
			}
		}

		// Token: 0x06000DBB RID: 3515 RVA: 0x00025F04 File Offset: 0x00024104
		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this._primarySiegeWeapons.Clear();
			foreach (MissionObject missionObject in Mission.Current.ActiveMissionObjects)
			{
				IPrimarySiegeWeapon primarySiegeWeapon;
				if ((primarySiegeWeapon = missionObject as IPrimarySiegeWeapon) != null && primarySiegeWeapon.WeaponSide == this._behaviorSide && !((SiegeWeapon)missionObject).IsDeactivated)
				{
					this._primarySiegeWeapons.Add(missionObject as SiegeWeapon);
				}
			}
			this.RecreateFollowEntityOrder();
			this._behaviorState = BehaviorUseSiegeMachines.BehaviorState.Unset;
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x00025FA8 File Offset: 0x000241A8
		public override void TickOccasionally()
		{
			base.TickOccasionally();
			bool flag = false;
			for (int i = this._primarySiegeWeapons.Count - 1; i >= 0; i--)
			{
				SiegeWeapon siegeWeapon = this._primarySiegeWeapons[i];
				if (siegeWeapon.IsDestroyed || siegeWeapon.IsDeactivated)
				{
					this._primarySiegeWeapons.RemoveAt(i);
					flag = true;
				}
			}
			if (flag)
			{
				this.RecreateFollowEntityOrder();
			}
			int num = 0;
			SiegeTower siegeTower = null;
			foreach (SiegeWeapon siegeWeapon2 in this._primarySiegeWeapons)
			{
				if (!((IPrimarySiegeWeapon)siegeWeapon2).HasCompletedAction())
				{
					num++;
					SiegeTower siegeTower2;
					if ((siegeTower2 = siegeWeapon2 as SiegeTower) != null)
					{
						siegeTower = siegeTower2;
					}
				}
			}
			if (num == 0)
			{
				base.CurrentOrder = this._stopOrder;
				return;
			}
			if (this._behaviorState == BehaviorUseSiegeMachines.BehaviorState.Follow)
			{
				if (this._followEntityOrder.OrderEnum == MovementOrder.MovementOrderEnum.Stop)
				{
					this.RecreateFollowEntityOrder();
				}
				base.CurrentOrder = this._followEntityOrder;
			}
			BehaviorUseSiegeMachines.BehaviorState behaviorState = ((siegeTower != null && siegeTower.HasArrivedAtTarget) ? BehaviorUseSiegeMachines.BehaviorState.ClimbSiegeTower : ((this._followEntityOrder.OrderEnum != MovementOrder.MovementOrderEnum.Invalid) ? BehaviorUseSiegeMachines.BehaviorState.Follow : BehaviorUseSiegeMachines.BehaviorState.Stop));
			if (behaviorState != this._behaviorState)
			{
				if (behaviorState == BehaviorUseSiegeMachines.BehaviorState.Follow)
				{
					base.CurrentOrder = this._followEntityOrder;
				}
				else if (behaviorState == BehaviorUseSiegeMachines.BehaviorState.ClimbSiegeTower)
				{
					this.RecreateFollowEntityOrder();
					base.CurrentOrder = this._followEntityOrder;
				}
				else
				{
					base.CurrentOrder = this._stopOrder;
				}
				this._behaviorState = behaviorState;
				bool flag2 = this._behaviorState == BehaviorUseSiegeMachines.BehaviorState.ClimbSiegeTower;
				if (!flag2)
				{
					using (List<SiegeWeapon>.Enumerator enumerator = this._primarySiegeWeapons.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SiegeLadder siegeLadder;
							if ((siegeLadder = enumerator.Current as SiegeLadder) != null && !siegeLadder.IsDisabled)
							{
								flag2 = true;
								break;
							}
						}
					}
				}
				if (flag2)
				{
					base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				}
				else if (base.Formation.QuerySystem.IsRangedFormation)
				{
					base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderScatter;
				}
				else
				{
					base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
				}
			}
			if (this._followedEntity != null && (this._behaviorState == BehaviorUseSiegeMachines.BehaviorState.Follow || this._behaviorState == BehaviorUseSiegeMachines.BehaviorState.ClimbSiegeTower))
			{
				base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtDirection(this._followedEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized());
			}
			else
			{
				base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			if (base.Formation.AI.ActiveBehavior == this)
			{
				foreach (SiegeWeapon siegeWeapon3 in this._primarySiegeWeapons)
				{
					if (!((IPrimarySiegeWeapon)siegeWeapon3).HasCompletedAction())
					{
						if (!siegeWeapon3.IsUsedByFormation(base.Formation))
						{
							base.Formation.StartUsingMachine(siegeWeapon3, false);
						}
						for (int j = siegeWeapon3.UserFormations.Count - 1; j >= 0; j--)
						{
							Formation formation = siegeWeapon3.UserFormations[j];
							if (formation != base.Formation && formation.IsAIControlled && (formation.AI.Side != this._behaviorSide || !(formation.AI.ActiveBehavior is BehaviorUseSiegeMachines)) && formation.Team == base.Formation.Team)
							{
								formation.StopUsingMachine(siegeWeapon3, false);
							}
						}
					}
				}
			}
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x00026350 File Offset: 0x00024550
		protected override void OnBehaviorActivatedAux()
		{
			base.Formation.ArrangementOrder = (base.Formation.QuerySystem.IsRangedFormation ? ArrangementOrder.ArrangementOrderScatter : ArrangementOrder.ArrangementOrderShieldWall);
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000DBE RID: 3518 RVA: 0x000263C6 File Offset: 0x000245C6
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000DBF RID: 3519 RVA: 0x000263D0 File Offset: 0x000245D0
		protected override float GetAiWeight()
		{
			float num = 0f;
			if (this._teamAISiegeComponent != null && this._primarySiegeWeapons.Count > 0)
			{
				if (this._primarySiegeWeapons.All((SiegeWeapon psw) => !(psw as IPrimarySiegeWeapon).HasCompletedAction()))
				{
					num = ((!this._teamAISiegeComponent.IsCastleBreached()) ? 0.75f : 0.25f);
				}
			}
			return num;
		}

		// Token: 0x0400035E RID: 862
		private List<SiegeWeapon> _primarySiegeWeapons;

		// Token: 0x0400035F RID: 863
		private TeamAISiegeComponent _teamAISiegeComponent;

		// Token: 0x04000360 RID: 864
		private MovementOrder _followEntityOrder;

		// Token: 0x04000361 RID: 865
		private GameEntity _followedEntity;

		// Token: 0x04000362 RID: 866
		private MovementOrder _stopOrder;

		// Token: 0x04000363 RID: 867
		private BehaviorUseSiegeMachines.BehaviorState _behaviorState;

		// Token: 0x0200045A RID: 1114
		private enum BehaviorState
		{
			// Token: 0x040018A1 RID: 6305
			Unset,
			// Token: 0x040018A2 RID: 6306
			Follow,
			// Token: 0x040018A3 RID: 6307
			ClimbSiegeTower,
			// Token: 0x040018A4 RID: 6308
			Stop
		}
	}
}
