using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	public interface IGameStarter
	{
		void AddModel(GameModel gameModel);

		IEnumerable<GameModel> Models { get; }
	}
}
