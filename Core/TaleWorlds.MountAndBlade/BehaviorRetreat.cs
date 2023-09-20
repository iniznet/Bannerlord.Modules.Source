using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000112 RID: 274
	public class BehaviorRetreat : BehaviorComponent
	{
		// Token: 0x06000D27 RID: 3367 RVA: 0x000207EF File Offset: 0x0001E9EF
		public BehaviorRetreat(Formation formation)
			: base(formation)
		{
			base.CurrentOrder = MovementOrder.MovementOrderRetreat;
			base.BehaviorCoherence = 0f;
		}

		// Token: 0x06000D28 RID: 3368 RVA: 0x0002080E File Offset: 0x0001EA0E
		public override void TickOccasionally()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x00020821 File Offset: 0x0001EA21
		protected override void OnBehaviorActivatedAux()
		{
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06000D2A RID: 3370 RVA: 0x00020823 File Offset: 0x0001EA23
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x0002082C File Offset: 0x0001EA2C
		protected override float GetAiWeight()
		{
			float casualtyPowerLossOfFormation = Mission.Current.GetMissionBehavior<CasualtyHandler>().GetCasualtyPowerLossOfFormation(base.Formation);
			float num = MathF.Sqrt(casualtyPowerLossOfFormation / (base.Formation.QuerySystem.FormationPower + casualtyPowerLossOfFormation));
			return MBMath.ClampFloat(base.Formation.Team.QuerySystem.TotalPowerRatio, 0.1f, 3f) / MBMath.ClampFloat(base.Formation.Team.QuerySystem.RemainingPowerRatio, 0.1f, 3f) * (0.05f + num);
		}
	}
}
