using System;

namespace TaleWorlds.MountAndBlade
{
	public sealed class BatteringRamAI : UsableMachineAIBase
	{
		public BatteringRamAI(BatteringRam batteringRam)
			: base(batteringRam)
		{
		}

		private BatteringRam BatteringRam
		{
			get
			{
				return this.UsableMachine as BatteringRam;
			}
		}

		public override bool HasActionCompleted
		{
			get
			{
				return this.BatteringRam.IsDeactivated;
			}
		}

		protected override MovementOrder NextOrder
		{
			get
			{
				TeamAISiegeComponent teamAISiegeComponent;
				if ((teamAISiegeComponent = Mission.Current.Teams[0].TeamAI as TeamAISiegeComponent) != null && teamAISiegeComponent.InnerGate != null && !teamAISiegeComponent.InnerGate.IsDestroyed)
				{
					return MovementOrder.MovementOrderAttackEntity(teamAISiegeComponent.InnerGate.GameEntity, false);
				}
				return MovementOrder.MovementOrderCharge;
			}
		}
	}
}
