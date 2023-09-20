using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	// Token: 0x02000067 RID: 103
	public interface IGameStarter
	{
		// Token: 0x060006E9 RID: 1769
		void AddModel(GameModel gameModel);

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060006EA RID: 1770
		IEnumerable<GameModel> Models { get; }
	}
}
