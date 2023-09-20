using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200010F RID: 271
	public class BehaviorRegroup : BehaviorComponent
	{
		// Token: 0x06000D0D RID: 3341 RVA: 0x0001FC3B File Offset: 0x0001DE3B
		public BehaviorRegroup(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 1f;
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x0001FC58 File Offset: 0x0001DE58
		protected override void CalculateCurrentOrder()
		{
			Vec2 vec;
			if (base.Formation.QuerySystem.ClosestEnemyFormation != null)
			{
				vec = (base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
			}
			else
			{
				vec = base.Formation.Direction;
			}
			WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
			medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x0001FD00 File Offset: 0x0001DF00
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x0001FD2C File Offset: 0x0001DF2C
		protected override float GetAiWeight()
		{
			FormationQuerySystem querySystem = base.Formation.QuerySystem;
			if (base.Formation.AI.ActiveBehavior == null)
			{
				return 0f;
			}
			float behaviorCoherence = base.Formation.AI.ActiveBehavior.BehaviorCoherence;
			return MBMath.Lerp(0.1f, 1.2f, MBMath.ClampFloat(behaviorCoherence * (querySystem.FormationIntegrityData.DeviationOfPositionsExcludeFarAgents + 1f) / (querySystem.IdealAverageDisplacement + 1f), 0f, 3f) / 3f, 1E-05f);
		}
	}
}
