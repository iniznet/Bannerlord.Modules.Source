using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000084 RID: 132
	public interface IGameStateManagerOwner
	{
		// Token: 0x060007B1 RID: 1969
		void OnStateStackEmpty();

		// Token: 0x060007B2 RID: 1970
		void OnStateChanged(GameState oldState);
	}
}
