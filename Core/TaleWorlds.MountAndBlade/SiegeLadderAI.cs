using System;

namespace TaleWorlds.MountAndBlade
{
	public sealed class SiegeLadderAI : UsableMachineAIBase
	{
		public SiegeLadderAI(SiegeLadder ladder)
			: base(ladder)
		{
		}

		public SiegeLadder Ladder
		{
			get
			{
				return this.UsableMachine as SiegeLadder;
			}
		}

		public override bool HasActionCompleted
		{
			get
			{
				return false;
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
