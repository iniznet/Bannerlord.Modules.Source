using System;
using TaleWorlds.Network;

namespace TaleWorlds.Core
{
	// Token: 0x020000C7 RID: 199
	public class WaitForGameState : CoroutineState
	{
		// Token: 0x060009B3 RID: 2483 RVA: 0x0001FFA1 File Offset: 0x0001E1A1
		public WaitForGameState(Type stateType)
		{
			this._stateType = stateType;
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x060009B4 RID: 2484 RVA: 0x0001FFB0 File Offset: 0x0001E1B0
		protected override bool IsFinished
		{
			get
			{
				GameState gameState = ((GameStateManager.Current != null) ? GameStateManager.Current.ActiveState : null);
				return gameState != null && this._stateType.IsInstanceOfType(gameState);
			}
		}

		// Token: 0x040005B8 RID: 1464
		private Type _stateType;
	}
}
