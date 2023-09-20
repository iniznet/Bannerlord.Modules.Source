using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class GameOverState : GameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

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

		public GameOverState.GameOverReason Reason { get; private set; }

		public GameOverState()
		{
		}

		public GameOverState(GameOverState.GameOverReason reason)
		{
			this.Reason = reason;
		}

		public static GameOverState CreateForVictory()
		{
			Game game = Game.Current;
			if (game == null)
			{
				return null;
			}
			return game.GameStateManager.CreateState<GameOverState>(new object[] { GameOverState.GameOverReason.Victory });
		}

		public static GameOverState CreateForRetirement()
		{
			Game game = Game.Current;
			if (game == null)
			{
				return null;
			}
			return game.GameStateManager.CreateState<GameOverState>(new object[] { GameOverState.GameOverReason.Retirement });
		}

		public static GameOverState CreateForClanDestroyed()
		{
			Game game = Game.Current;
			if (game == null)
			{
				return null;
			}
			return game.GameStateManager.CreateState<GameOverState>(new object[] { GameOverState.GameOverReason.ClanDestroyed });
		}

		private IGameOverStateHandler _handler;

		public enum GameOverReason
		{
			Retirement,
			ClanDestroyed,
			Victory
		}
	}
}
