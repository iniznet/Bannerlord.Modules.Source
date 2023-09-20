using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public abstract class GameModelsManager
	{
		protected GameModelsManager(IEnumerable<GameModel> inputComponents)
		{
			this._gameModels = inputComponents.ToMBList<GameModel>();
		}

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

		public MBReadOnlyList<GameModel> GetGameModels()
		{
			return this._gameModels;
		}

		private readonly MBList<GameModel> _gameModels;
	}
}
