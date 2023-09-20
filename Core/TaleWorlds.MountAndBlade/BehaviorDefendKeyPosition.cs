using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000101 RID: 257
	public class BehaviorDefendKeyPosition : BehaviorComponent
	{
		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06000CAE RID: 3246 RVA: 0x0001BCAF File Offset: 0x00019EAF
		// (set) Token: 0x06000CAF RID: 3247 RVA: 0x0001BCBC File Offset: 0x00019EBC
		public WorldPosition DefensePosition
		{
			get
			{
				return this._behaviorPosition.Value;
			}
			set
			{
				this._defensePosition = value;
			}
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x0001BCC8 File Offset: 0x00019EC8
		public BehaviorDefendKeyPosition(Formation formation)
			: base(formation)
		{
			this._behaviorPosition = new QueryData<WorldPosition>(() => Mission.Current.FindBestDefendingPosition(this.EnemyClusterPosition, this._defensePosition), 5f);
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x0001BD14 File Offset: 0x00019F14
		protected override void CalculateCurrentOrder()
		{
			Vec2 vec;
			if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				vec = base.Formation.Direction;
			}
			else
			{
				vec = ((base.Formation.Direction.DotProduct((base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized()) < 0.5f) ? (base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition) : base.Formation.Direction).Normalized();
			}
			if (this.DefensePosition.IsValid)
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(this.DefensePosition);
			}
			else
			{
				WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
				medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
			}
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x0001BE48 File Offset: 0x0001A048
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.QuerySystem.HasShield && base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) < base.Formation.Depth * base.Formation.Depth * 4f)
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
				return;
			}
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x0001BEFC File Offset: 0x0001A0FC
		protected override void OnBehaviorActivatedAux()
		{
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x0001BF49 File Offset: 0x0001A149
		protected override float GetAiWeight()
		{
			return 10f;
		}

		// Token: 0x04000308 RID: 776
		private WorldPosition _defensePosition = WorldPosition.Invalid;

		// Token: 0x04000309 RID: 777
		public WorldPosition EnemyClusterPosition = WorldPosition.Invalid;

		// Token: 0x0400030A RID: 778
		private readonly QueryData<WorldPosition> _behaviorPosition;
	}
}
