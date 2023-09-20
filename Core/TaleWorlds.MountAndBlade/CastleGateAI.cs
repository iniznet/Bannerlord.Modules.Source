using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000148 RID: 328
	public class CastleGateAI : UsableMachineAIBase
	{
		// Token: 0x060010CD RID: 4301 RVA: 0x00036FF7 File Offset: 0x000351F7
		public void ResetInitialGateState(CastleGate.GateState newInitialState)
		{
			this._initialState = newInitialState;
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x00037000 File Offset: 0x00035200
		public CastleGateAI(CastleGate gate)
			: base(gate)
		{
			this._initialState = gate.State;
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x060010CF RID: 4303 RVA: 0x00037015 File Offset: 0x00035215
		public override bool HasActionCompleted
		{
			get
			{
				return ((CastleGate)this.UsableMachine).State != this._initialState;
			}
		}

		// Token: 0x04000442 RID: 1090
		private CastleGate.GateState _initialState;
	}
}
