using System;
using System.Collections.Generic;
using Helpers;
using SandBox.BoardGames.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.BoardGames.AI
{
	public class BoardGameAIMuTorere : BoardGameAIBase
	{
		public BoardGameAIMuTorere(BoardGameHelper.AIDifficulty difficulty, MissionBoardGameLogic boardGameHandler)
			: base(difficulty, boardGameHandler)
		{
			this._board = base.BoardGameHandler.Board as BoardGameMuTorere;
		}

		protected override void InitializeDifficulty()
		{
			switch (base.Difficulty)
			{
			case 0:
				this.MaxDepth = 3;
				return;
			case 1:
				this.MaxDepth = 5;
				return;
			case 2:
				this.MaxDepth = 7;
				return;
			default:
				return;
			}
		}

		public override Move CalculateMovementStageMove()
		{
			Move move;
			move.GoalTile = null;
			move.Unit = null;
			if (this._board.IsReady)
			{
				List<List<Move>> list = this._board.CalculateAllValidMoves(BoardGameSide.AI);
				BoardGameMuTorere.BoardInformation boardInformation = this._board.TakePawnsSnapshot();
				if (this._board.HasMovesAvailable(ref list))
				{
					int num = int.MinValue;
					foreach (List<Move> list2 in list)
					{
						if (base.AbortRequested)
						{
							break;
						}
						foreach (Move move2 in list2)
						{
							if (base.AbortRequested)
							{
								break;
							}
							this._board.AIMakeMove(move2);
							int num2 = -this.NegaMax(this.MaxDepth, -1);
							this._board.UndoMove(ref boardInformation);
							if (num2 > num)
							{
								move = move2;
								num = num2;
							}
						}
					}
				}
			}
			if (!base.AbortRequested)
			{
				bool isValid = move.IsValid;
			}
			return move;
		}

		private int NegaMax(int depth, int color)
		{
			int num = int.MinValue;
			if (depth == 0)
			{
				return color * this.Evaluation() * ((this._board.PlayerWhoStarted == PlayerTurn.PlayerOne) ? 1 : (-1));
			}
			BoardGameMuTorere.BoardInformation boardInformation = this._board.TakePawnsSnapshot();
			List<List<Move>> list = this._board.CalculateAllValidMoves((color == 1) ? BoardGameSide.AI : BoardGameSide.Player);
			if (!this._board.HasMovesAvailable(ref list))
			{
				return color * this.Evaluation();
			}
			foreach (List<Move> list2 in list)
			{
				foreach (Move move in list2)
				{
					this._board.AIMakeMove(move);
					num = MathF.Max(num, -this.NegaMax(depth - 1, -color));
					this._board.UndoMove(ref boardInformation);
				}
			}
			return num;
		}

		private int Evaluation()
		{
			float num = MBRandom.RandomFloat;
			switch (base.Difficulty)
			{
			case 0:
				num = num * 2f - 1f;
				break;
			case 1:
				num = num * 1.7f - 0.7f;
				break;
			case 2:
				num = num * 1.4f - 0.4f;
				break;
			}
			return (int)(num * 100f * (float)(this.CanMove(false) - this.CanMove(true)));
		}

		private int CanMove(bool playerOne)
		{
			List<List<Move>> list = this._board.CalculateAllValidMoves(playerOne ? BoardGameSide.Player : BoardGameSide.AI);
			if (!this._board.HasMovesAvailable(ref list))
			{
				return 0;
			}
			return 1;
		}

		private readonly BoardGameMuTorere _board;
	}
}
