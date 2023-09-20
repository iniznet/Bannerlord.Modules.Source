using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002DB RID: 731
	public class BasicGameStarter : IGameStarter
	{
		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06002834 RID: 10292 RVA: 0x0009BA47 File Offset: 0x00099C47
		IEnumerable<GameModel> IGameStarter.Models
		{
			get
			{
				return this._models;
			}
		}

		// Token: 0x06002835 RID: 10293 RVA: 0x0009BA4F File Offset: 0x00099C4F
		public BasicGameStarter()
		{
			this._models = new List<GameModel>();
		}

		// Token: 0x06002836 RID: 10294 RVA: 0x0009BA62 File Offset: 0x00099C62
		void IGameStarter.AddModel(GameModel gameModel)
		{
			this._models.Add(gameModel);
		}

		// Token: 0x04000EC1 RID: 3777
		private List<GameModel> _models;
	}
}
