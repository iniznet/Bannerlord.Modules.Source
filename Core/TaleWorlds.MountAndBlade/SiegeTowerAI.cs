using System;

namespace TaleWorlds.MountAndBlade
{
	public sealed class SiegeTowerAI : UsableMachineAIBase
	{
		private SiegeTower SiegeTower
		{
			get
			{
				return this.UsableMachine as SiegeTower;
			}
		}

		public SiegeTowerAI(SiegeTower siegeTower)
			: base(siegeTower)
		{
		}

		public override bool HasActionCompleted
		{
			get
			{
				return this.SiegeTower.MovementComponent.HasArrivedAtTarget && this.SiegeTower.State == SiegeTower.GateState.Open;
			}
		}

		protected override MovementOrder NextOrder
		{
			get
			{
				return MovementOrder.MovementOrderCharge;
			}
		}
	}
}
