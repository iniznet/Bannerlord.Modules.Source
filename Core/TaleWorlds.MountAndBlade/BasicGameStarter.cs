using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class BasicGameStarter : IGameStarter
	{
		IEnumerable<GameModel> IGameStarter.Models
		{
			get
			{
				return this._models;
			}
		}

		public BasicGameStarter()
		{
			this._models = new List<GameModel>();
		}

		void IGameStarter.AddModel(GameModel gameModel)
		{
			this._models.Add(gameModel);
		}

		private List<GameModel> _models;
	}
}
