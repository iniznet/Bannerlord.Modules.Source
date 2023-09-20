using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200010D RID: 269
	public class BehaviorProtectGeneral : BehaviorComponent
	{
		// Token: 0x06000D03 RID: 3331 RVA: 0x0001F418 File Offset: 0x0001D618
		public BehaviorProtectGeneral(Formation formation)
			: base(formation)
		{
			base.CurrentOrder = MovementOrder.MovementOrderFollow((formation.Team.GeneralsFormation != null && formation.Team.GeneralsFormation.CountOfUnits > 0) ? formation.Team.GeneralsFormation.GetFirstUnit() : Mission.Current.MainAgent);
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x0001F473 File Offset: 0x0001D673
		public override void TickOccasionally()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06000D05 RID: 3333 RVA: 0x0001F486 File Offset: 0x0001D686
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0001F490 File Offset: 0x0001D690
		protected override float GetAiWeight()
		{
			if ((base.Formation.Team.GeneralsFormation != null && base.Formation.Team.GeneralsFormation.CountOfUnits > 0) || (base.Formation.Team.IsPlayerTeam && base.Formation.Team.IsPlayerGeneral && Mission.Current.MainAgent != null))
			{
				return 1f;
			}
			return 0f;
		}
	}
}
