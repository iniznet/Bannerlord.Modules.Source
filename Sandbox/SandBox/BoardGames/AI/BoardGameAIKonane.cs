using System;
using System.Collections.Generic;
using Helpers;
using SandBox.BoardGames.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.BoardGames.AI
{
	public class BoardGameAIKonane : BoardGameAIBase
	{
		public BoardGameAIKonane(BoardGameHelper.AIDifficulty difficulty, MissionBoardGameLogic boardGameHandler)
			: base(difficulty, boardGameHandler)
		{
			this._board = base.BoardGameHandler.Board as BoardGameKonane;
		}

		protected override void InitializeDifficulty()
		{
			switch (base.Difficulty)
			{
			case 0:
				this.MaxDepth = 2;
				return;
			case 1:
				this.MaxDepth = 5;
				return;
			case 2:
				this.MaxDepth = 8;
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
				BoardGameKonane.BoardInformation boardInformation = this._board.TakeBoardSnapshot();
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
							int num2 = -this.NegaMax(this.MaxDepth, -1, -2147483647, int.MaxValue);
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

		public override Move CalculatePreMovementStageMove()
		{
			Move invalid = Move.Invalid;
			int num = this._board.CheckForRemovablePawns(false);
			int num2 = MBRandom.RandomInt(0, num);
			invalid.Unit = this._board.RemovablePawns[num2];
			return invalid;
		}

		private int NegaMax(int depth, int color, int alpha, int beta)
		{
			if (depth == 0)
			{
				return color * this.Evaluation();
			}
			List<List<Move>> list = this._board.CalculateAllValidMoves((color == 1) ? BoardGameSide.AI : BoardGameSide.Player);
			if (!this._board.HasMovesAvailable(ref list))
			{
				return color * this.Evaluation();
			}
			BoardGameKonane.BoardInformation boardInformation = this._board.TakeBoardSnapshot();
			foreach (List<Move> list2 in list)
			{
				foreach (Move move in list2)
				{
					this._board.AIMakeMove(move);
					int num = -this.NegaMax(depth - 1, -color, -beta, -alpha);
					this._board.UndoMove(ref boardInformation);
					if (num >= beta)
					{
						return num;
					}
					alpha = MathF.Max(num, alpha);
				}
			}
			return alpha;
		}

		private int Evaluation()
		{
			float num = MBRandom.RandomFloat;
			switch (base.Difficulty)
			{
			case 0:
				num = num * 0.7f + 0.5f;
				break;
			case 1:
				num = num * 0.5f + 0.65f;
				break;
			case 2:
				num = num * 0.35f + 0.75f;
				break;
			}
			List<List<Move>> list = this._board.CalculateAllValidMoves(BoardGameSide.Player);
			List<List<Move>> list2 = this._board.CalculateAllValidMoves(BoardGameSide.AI);
			int totalMovesAvailable = this._board.GetTotalMovesAvailable(ref list);
			int totalMovesAvailable2 = this._board.GetTotalMovesAvailable(ref list2);
			int num2 = MathF.Min(totalMovesAvailable, 1);
			int num3 = MathF.Min(totalMovesAvailable2, 1);
			return (int)((float)(100 * (num3 - num2) + 20 * (this._board.GetPlayerTwoUnitsAlive() - this._board.GetPlayerOneUnitsAlive()) + 5 * (totalMovesAvailable2 - totalMovesAvailable)) * num);
		}

		private readonly BoardGameKonane _board;
	}
}
