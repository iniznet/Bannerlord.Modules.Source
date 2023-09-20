using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000083 RID: 131
	public interface IGameStateManagerListener
	{
		// Token: 0x060007AC RID: 1964
		void OnCreateState(GameState gameState);

		// Token: 0x060007AD RID: 1965
		void OnPushState(GameState gameState, bool isTopGameState);

		// Token: 0x060007AE RID: 1966
		void OnPopState(GameState gameState);

		// Token: 0x060007AF RID: 1967
		void OnCleanStates();

		// Token: 0x060007B0 RID: 1968
		void OnSavedGameLoadFinished();
	}
}
