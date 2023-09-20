using System;
using System.Collections.Generic;
using Helpers;
using SandBox.BoardGames.MissionLogics;
using SandBox.BoardGames.Pawns;

namespace SandBox.BoardGames.AI
{
	// Token: 0x020000C6 RID: 198
	public class BoardGameAIPuluc : BoardGameAIBase
	{
		// Token: 0x06000BE3 RID: 3043 RVA: 0x0005DF7A File Offset: 0x0005C17A
		public BoardGameAIPuluc(BoardGameHelper.AIDifficulty difficulty, MissionBoardGameLogic boardGameHandler)
			: base(difficulty, boardGameHandler)
		{
			this._board = base.BoardGameHandler.Board as BoardGamePuluc;
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x0005DFB4 File Offset: 0x0005C1B4
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

		// Token: 0x06000BE5 RID: 3045 RVA: 0x0005DFF4 File Offset: 0x0005C1F4
		public override Move CalculateMovementStageMove()
		{
			Move move;
			move.GoalTile = null;
			move.Unit = null;
			if (this._board.IsReady)
			{
				this.ExpectiMax(this.MaxDepth, BoardGameSide.AI, false, ref move);
			}
			if (!base.AbortRequested)
			{
				bool isValid = move.IsValid;
			}
			return move;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0005E040 File Offset: 0x0005C240
		private float ExpectiMax(int depth, BoardGameSide side, bool chanceNode, ref Move bestMove)
		{
			float num;
			if (depth == 0)
			{
				num = (float)this.Evaluation();
				if (side == BoardGameSide.Player)
				{
					num = -num;
				}
			}
			else if (chanceNode)
			{
				num = 0f;
				for (int i = 0; i < 5; i++)
				{
					int lastDice = this._board.LastDice;
					this._board.ForceDice((i == 0) ? 5 : i);
					num += this._diceProbabilities[i] * this.ExpectiMax(depth - 1, side, false, ref bestMove);
					this._board.ForceDice(lastDice);
				}
			}
			else
			{
				BoardGamePuluc.BoardInformation boardInformation = this._board.TakeBoardSnapshot();
				List<List<Move>> list = this._board.CalculateAllValidMoves(side);
				if (this._board.HasMovesAvailable(ref list))
				{
					num = float.MinValue;
					using (List<List<Move>>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							List<Move> list2 = enumerator.Current;
							if (list2 != null)
							{
								foreach (Move move in list2)
								{
									this._board.AIMakeMove(move);
									BoardGameSide boardGameSide = ((side == BoardGameSide.AI) ? BoardGameSide.Player : BoardGameSide.AI);
									float num2 = -this.ExpectiMax(depth - 1, boardGameSide, true, ref bestMove);
									this._board.UndoMove(ref boardInformation);
									if (num < num2)
									{
										num = num2;
										if (depth == this.MaxDepth)
										{
											bestMove = move;
										}
									}
								}
							}
						}
						return num;
					}
				}
				num = (float)this.Evaluation();
				if (side == BoardGameSide.Player)
				{
					num = -num;
				}
			}
			return num;
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x0005E1D4 File Offset: 0x0005C3D4
		private int Evaluation()
		{
			return 20 * (this._board.GetPlayerTwoUnitsAlive() - this._board.GetPlayerOneUnitsAlive()) + 5 * (this.GetUnitsBeingCaptured(true) - this.GetUnitsBeingCaptured(false)) + (this.GetUnitsInPlay(false) - this.GetUnitsInPlay(true));
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x0005E214 File Offset: 0x0005C414
		private int GetUnitsInSpawn(bool playerOne)
		{
			int num = 0;
			using (List<PawnBase>.Enumerator enumerator = (playerOne ? this._board.PlayerOneUnits : this._board.PlayerTwoUnits).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((PawnPuluc)enumerator.Current).IsInSpawn)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x0005E288 File Offset: 0x0005C488
		private int GetUnitsBeingCaptured(bool playerOne)
		{
			int num = 0;
			using (List<PawnBase>.Enumerator enumerator = (playerOne ? this._board.PlayerOneUnits : this._board.PlayerTwoUnits).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!((PawnPuluc)enumerator.Current).IsTopPawn)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0005E2FC File Offset: 0x0005C4FC
		private int GetUnitsInPlay(bool playerOne)
		{
			int num = 0;
			foreach (PawnBase pawnBase in (playerOne ? this._board.PlayerOneUnits : this._board.PlayerTwoUnits))
			{
				PawnPuluc pawnPuluc = (PawnPuluc)pawnBase;
				if (pawnPuluc.InPlay && pawnPuluc.IsTopPawn)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x04000436 RID: 1078
		private readonly BoardGamePuluc _board;

		// Token: 0x04000437 RID: 1079
		private readonly float[] _diceProbabilities = new float[] { 0.0625f, 0.25f, 0.375f, 0.25f, 0.0625f };
	}
}
