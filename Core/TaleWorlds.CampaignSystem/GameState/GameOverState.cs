using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x02000337 RID: 823
	public class GameOverState : GameState
	{
		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x06002E3A RID: 11834 RVA: 0x000BFE36 File Offset: 0x000BE036
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B03 RID: 2819
		// (get) Token: 0x06002E3B RID: 11835 RVA: 0x000BFE39 File Offset: 0x000BE039
		// (set) Token: 0x06002E3C RID: 11836 RVA: 0x000BFE41 File Offset: 0x000BE041
		public IGameOverStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x06002E3D RID: 11837 RVA: 0x000BFE4A File Offset: 0x000BE04A
		// (set) Token: 0x06002E3E RID: 11838 RVA: 0x000BFE52 File Offset: 0x000BE052
		public GameOverState.GameOverReason Reason { get; private set; }

		// Token: 0x06002E3F RID: 11839 RVA: 0x000BFE5B File Offset: 0x000BE05B
		public GameOverState()
		{
		}

		// Token: 0x06002E40 RID: 11840 RVA: 0x000BFE63 File Offset: 0x000BE063
		public GameOverState(GameOverState.GameOverReason reason)
		{
			this.Reason = reason;
		}

		// Token: 0x06002E41 RID: 11841 RVA: 0x000BFE72 File Offset: 0x000BE072
		public static GameOverState CreateForVictory()
		{
			Game game = Game.Current;
			if (game == null)
			{
				return null;
			}
			return game.GameStateManager.CreateState<GameOverState>(new object[] { GameOverState.GameOverReason.Victory });
		}

		// Token: 0x06002E42 RID: 11842 RVA: 0x000BFE98 File Offset: 0x000BE098
		public static GameOverState CreateForRetirement()
		{
			Game game = Game.Current;
			if (game == null)
			{
				return null;
			}
			return game.GameStateManager.CreateState<GameOverState>(new object[] { GameOverState.GameOverReason.Retirement });
		}

		// Token: 0x06002E43 RID: 11843 RVA: 0x000BFEBE File Offset: 0x000BE0BE
		public static GameOverState CreateForClanDestroyed()
		{
			Game game = Game.Current;
			if (game == null)
			{
				return null;
			}
			return game.GameStateManager.CreateState<GameOverState>(new object[] { GameOverState.GameOverReason.ClanDestroyed });
		}

		// Token: 0x04000DEA RID: 3562
		private IGameOverStateHandler _handler;

		// Token: 0x0200067A RID: 1658
		public enum GameOverReason
		{
			// Token: 0x04001B33 RID: 6963
			Retirement,
			// Token: 0x04001B34 RID: 6964
			ClanDestroyed,
			// Token: 0x04001B35 RID: 6965
			Victory
		}
	}
}
