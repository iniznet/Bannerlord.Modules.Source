using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x0200006A RID: 106
	public abstract class GameModelsManager
	{
		// Token: 0x060006F9 RID: 1785 RVA: 0x00018397 File Offset: 0x00016597
		protected GameModelsManager(IEnumerable<GameModel> inputComponents)
		{
			this._gameModels = inputComponents.ToMBList<GameModel>();
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x000183AC File Offset: 0x000165AC
		protected T GetGameModel<T>() where T : GameModel
		{
			for (int i = this._gameModels.Count - 1; i >= 0; i--)
			{
				T t;
				if ((t = this._gameModels[i] as T) != null)
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x000183FB File Offset: 0x000165FB
		public MBReadOnlyList<GameModel> GetGameModels()
		{
			return this._gameModels;
		}

		// Token: 0x040003A3 RID: 931
		private readonly MBList<GameModel> _gameModels;
	}
}
