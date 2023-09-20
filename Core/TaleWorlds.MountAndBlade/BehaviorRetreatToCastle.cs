using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000113 RID: 275
	public class BehaviorRetreatToCastle : BehaviorComponent
	{
		// Token: 0x06000D2C RID: 3372 RVA: 0x000208BC File Offset: 0x0001EABC
		public BehaviorRetreatToCastle(Formation formation)
			: base(formation)
		{
			WorldPosition worldPosition = Mission.Current.DeploymentPlan.GetFormationPlan(formation.Team.Side, FormationClass.Cavalry, DeploymentPlanType.Initial).CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3);
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			base.BehaviorCoherence = 0f;
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x0002090A File Offset: 0x0001EB0A
		public override void TickOccasionally()
		{
			base.TickOccasionally();
			if (base.Formation.AI.ActiveBehavior == this)
			{
				base.Formation.SetMovementOrder(base.CurrentOrder);
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06000D2E RID: 3374 RVA: 0x00020936 File Offset: 0x0001EB36
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000D2F RID: 3375 RVA: 0x0002093D File Offset: 0x0001EB3D
		protected override float GetAiWeight()
		{
			return 1f;
		}
	}
}
