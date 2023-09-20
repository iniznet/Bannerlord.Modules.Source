using System;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000124 RID: 292
	public class BehaviorUseMurderHole : BehaviorComponent
	{
		// Token: 0x06000DB4 RID: 3508 RVA: 0x00025BE0 File Offset: 0x00023DE0
		public BehaviorUseMurderHole(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			WorldPosition worldPosition = new WorldPosition(base.Formation.Team.Mission.Scene, UIntPtr.Zero, (formation.Team.TeamAI as TeamAISiegeDefender).MurderHolePosition, false);
			this._outerGate = (formation.Team.TeamAI as TeamAISiegeDefender).OuterGate;
			this._innerGate = (formation.Team.TeamAI as TeamAISiegeDefender).InnerGate;
			this._batteringRam = base.Formation.Team.Mission.ActiveMissionObjects.FindAllWithType<BatteringRam>().FirstOrDefault<BatteringRam>();
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			base.BehaviorCoherence = 0f;
		}

		// Token: 0x06000DB5 RID: 3509 RVA: 0x00025CAE File Offset: 0x00023EAE
		public override void TickOccasionally()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x00025CC4 File Offset: 0x00023EC4
		public bool IsMurderHoleActive()
		{
			return (this._batteringRam != null && this._batteringRam.HasArrivedAtTarget && !this._innerGate.IsDestroyed) || (this._outerGate.IsDestroyed && !this._innerGate.IsDestroyed);
		}

		// Token: 0x06000DB7 RID: 3511 RVA: 0x00025D12 File Offset: 0x00023F12
		protected override float GetAiWeight()
		{
			return 10f * (this.IsMurderHoleActive() ? 1f : 0f);
		}

		// Token: 0x0400035B RID: 859
		private readonly CastleGate _outerGate;

		// Token: 0x0400035C RID: 860
		private readonly CastleGate _innerGate;

		// Token: 0x0400035D RID: 861
		private readonly BatteringRam _batteringRam;
	}
}
