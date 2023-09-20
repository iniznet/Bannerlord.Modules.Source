using System;
using TaleWorlds.Network;

namespace TaleWorlds.Core
{
	public class WaitForGameState : CoroutineState
	{
		public WaitForGameState(Type stateType)
		{
			this._stateType = stateType;
		}

		protected override bool IsFinished
		{
			get
			{
				GameState gameState = ((GameStateManager.Current != null) ? GameStateManager.Current.ActiveState : null);
				return gameState != null && this._stateType.IsInstanceOfType(gameState);
			}
		}

		private Type _stateType;
	}
}
