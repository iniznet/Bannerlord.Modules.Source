using System;

namespace TaleWorlds.MountAndBlade
{
	public class CastleGateAI : UsableMachineAIBase
	{
		public void ResetInitialGateState(CastleGate.GateState newInitialState)
		{
			this._initialState = newInitialState;
		}

		public CastleGateAI(CastleGate gate)
			: base(gate)
		{
			this._initialState = gate.State;
		}

		public override bool HasActionCompleted
		{
			get
			{
				return ((CastleGate)this.UsableMachine).State != this._initialState;
			}
		}

		private CastleGate.GateState _initialState;
	}
}
