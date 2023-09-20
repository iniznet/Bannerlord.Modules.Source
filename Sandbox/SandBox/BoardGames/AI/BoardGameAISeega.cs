using System;
using System.Collections.Generic;
using Helpers;
using SandBox.BoardGames.MissionLogics;
using SandBox.BoardGames.Pawns;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.BoardGames.AI
{
	// Token: 0x020000C7 RID: 199
	public class BoardGameAISeega : BoardGameAIBase
	{
		// Token: 0x06000BEB RID: 3051 RVA: 0x0005E37C File Offset: 0x0005C57C
		public BoardGameAISeega(BoardGameHelper.AIDifficulty difficulty, MissionBoardGameLogic boardGameHandler)
			: base(difficulty, boardGameHandler)
		{
			this._board = base.BoardGameHandler.Board as BoardGameSeega;
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x0005E3B4 File Offset: 0x0005C5B4
		protected override void InitializeDifficulty()
		{
			switch (base.Difficulty)
			{
			case 0:
				this.MaxDepth = 2;
				return;
			case 1:
				this.MaxDepth = 3;
				return;
			case 2:
				this.MaxDepth = 4;
				return;
			default:
				return;
			}
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x0005E3F4 File Offset: 0x0005C5F4
		public override Move CalculateMovementStageMove()
		{
			Move move = Move.Invalid;
			if (this._board.IsReady)
			{
				List<List<Move>> list = this._board.CalculateAllValidMoves(BoardGameSide.AI);
				if (!this._board.HasMovesAvailable(ref list))
				{
					IEnumerable<KeyValuePair<PawnBase, int>> blockingPawns = this._board.GetBlockingPawns(false);
					InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=1bzdDYoO}All AI pawns blocked. Removing one of the player's pawns to make a move", null).ToString()));
					PawnBase key = Extensions.MaxBy<KeyValuePair<PawnBase, int>, int>(blockingPawns, (KeyValuePair<PawnBase, int> x) => x.Value).Key;
					this._board.SetPawnCaptured(key, false);
					list = this._board.CalculateAllValidMoves(BoardGameSide.AI);
				}
				BoardGameSeega.BoardInformation boardInformation = this._board.TakeBoardSnapshot();
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

		// Token: 0x06000BEE RID: 3054 RVA: 0x0005E5A8 File Offset: 0x0005C7A8
		public override bool WantsToForfeit()
		{
			if (!this.MayForfeit)
			{
				return false;
			}
			int playerOneUnitsAlive = this._board.GetPlayerOneUnitsAlive();
			int playerTwoUnitsAlive = this._board.GetPlayerTwoUnitsAlive();
			int num = ((base.Difficulty == 2) ? 2 : 1);
			if (playerTwoUnitsAlive <= 7 && playerOneUnitsAlive >= playerTwoUnitsAlive + (num + playerTwoUnitsAlive / 2))
			{
				this.MayForfeit = false;
				return true;
			}
			return false;
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x0005E600 File Offset: 0x0005C800
		public override Move CalculatePreMovementStageMove()
		{
			Move invalid = Move.Invalid;
			foreach (PawnBase pawnBase in this._board.PlayerTwoUnits)
			{
				PawnSeega pawnSeega = (PawnSeega)pawnBase;
				if (!pawnSeega.IsPlaced && !pawnSeega.Moving)
				{
					while (!invalid.IsValid)
					{
						if (base.AbortRequested)
						{
							break;
						}
						int num = MBRandom.RandomInt(0, 5);
						int num2 = MBRandom.RandomInt(0, 5);
						if (this._board.GetTile(num, num2).PawnOnTile == null && !this._board.GetTile(num, num2).Entity.HasTag("obstructed_at_start"))
						{
							invalid.Unit = pawnSeega;
							invalid.GoalTile = this._board.GetTile(num, num2);
						}
					}
					break;
				}
			}
			return invalid;
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x0005E6F0 File Offset: 0x0005C8F0
		private int NegaMax(int depth, int color, int alpha, int beta)
		{
			int num = int.MinValue;
			if (depth == 0)
			{
				return color * this.Evaluation();
			}
			foreach (PawnBase pawnBase in ((color == 1) ? this._board.PlayerTwoUnits : this._board.PlayerOneUnits))
			{
				((PawnSeega)pawnBase).UpdateMoveBackAvailable();
			}
			List<List<Move>> list = this._board.CalculateAllValidMoves((color == 1) ? BoardGameSide.AI : BoardGameSide.Player);
			if (!this._board.HasMovesAvailable(ref list))
			{
				return color * this.Evaluation();
			}
			BoardGameSeega.BoardInformation boardInformation = this._board.TakeBoardSnapshot();
			foreach (List<Move> list2 in list)
			{
				if (list2 != null)
				{
					foreach (Move move in list2)
					{
						this._board.AIMakeMove(move);
						num = MathF.Max(-this.NegaMax(depth - 1, -color, -beta, -alpha), num);
						alpha = MathF.Max(alpha, num);
						this._board.UndoMove(ref boardInformation);
						if (alpha >= beta && color == 1)
						{
							return alpha;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x0005E870 File Offset: 0x0005CA70
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
			return (int)((float)(20 * (this._board.GetPlayerTwoUnitsAlive() - this._board.GetPlayerOneUnitsAlive()) + (this.GetPlacementScore(false) - this.GetPlacementScore(true)) + 2 * (this.GetSurroundedScore(false) - this.GetSurroundedScore(true))) * num);
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x0005E90C File Offset: 0x0005CB0C
		private int GetPlacementScore(bool player)
		{
			int num = 0;
			foreach (PawnBase pawnBase in (player ? this._board.PlayerOneUnits : this._board.PlayerTwoUnits))
			{
				PawnSeega pawnSeega = (PawnSeega)pawnBase;
				if (pawnSeega.IsPlaced)
				{
					num += this._boardValues[pawnSeega.X, pawnSeega.Y];
				}
			}
			return num;
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x0005E998 File Offset: 0x0005CB98
		private int GetSurroundedScore(bool player)
		{
			int num = 0;
			foreach (PawnBase pawnBase in (player ? this._board.PlayerOneUnits : this._board.PlayerTwoUnits))
			{
				PawnSeega pawnSeega = (PawnSeega)pawnBase;
				if (pawnSeega.IsPlaced)
				{
					num += this.GetAmountSurroundingThisPawn(pawnSeega);
				}
			}
			return num;
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x0005EA14 File Offset: 0x0005CC14
		private int GetAmountSurroundingThisPawn(PawnSeega pawn)
		{
			int num = 0;
			int x = pawn.X;
			int y = pawn.Y;
			if (x > 0 && this._board.GetTile(x - 1, y).PawnOnTile != null)
			{
				num++;
			}
			if (y > 0 && this._board.GetTile(x, y - 1).PawnOnTile != null)
			{
				num++;
			}
			if (x < BoardGameSeega.BoardWidth - 1 && this._board.GetTile(x + 1, y).PawnOnTile != null)
			{
				num++;
			}
			if (y < BoardGameSeega.BoardHeight - 1 && this._board.GetTile(x, y + 1).PawnOnTile != null)
			{
				num++;
			}
			return num;
		}

		// Token: 0x04000438 RID: 1080
		private readonly BoardGameSeega _board;

		// Token: 0x04000439 RID: 1081
		private readonly int[,] _boardValues = new int[,]
		{
			{ 3, 2, 2, 2, 3 },
			{ 2, 1, 1, 1, 2 },
			{ 2, 1, 3, 1, 2 },
			{ 2, 1, 1, 1, 2 },
			{ 3, 2, 2, 2, 3 }
		};
	}
}
