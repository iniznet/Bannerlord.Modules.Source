using System;
using System.Collections.Generic;
using Helpers;
using SandBox.BoardGames.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.BoardGames.AI
{
	// Token: 0x020000C5 RID: 197
	public class BoardGameAIMuTorere : BoardGameAIBase
	{
		// Token: 0x06000BDD RID: 3037 RVA: 0x0005DC3A File Offset: 0x0005BE3A
		public BoardGameAIMuTorere(BoardGameHelper.AIDifficulty difficulty, MissionBoardGameLogic boardGameHandler)
			: base(difficulty, boardGameHandler)
		{
			this._board = base.BoardGameHandler.Board as BoardGameMuTorere;
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x0005DC5C File Offset: 0x0005BE5C
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

		// Token: 0x06000BDF RID: 3039 RVA: 0x0005DC9C File Offset: 0x0005BE9C
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

		// Token: 0x06000BE0 RID: 3040 RVA: 0x0005DDC8 File Offset: 0x0005BFC8
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

		// Token: 0x06000BE1 RID: 3041 RVA: 0x0005DED0 File Offset: 0x0005C0D0
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

		// Token: 0x06000BE2 RID: 3042 RVA: 0x0005DF48 File Offset: 0x0005C148
		private int CanMove(bool playerOne)
		{
			List<List<Move>> list = this._board.CalculateAllValidMoves(playerOne ? BoardGameSide.Player : BoardGameSide.AI);
			if (!this._board.HasMovesAvailable(ref list))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x04000435 RID: 1077
		private readonly BoardGameMuTorere _board;
	}
}
