using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.BoardGames.Pawns;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.BoardGames.AI
{
	// Token: 0x020000C9 RID: 201
	public class TreeNodeTablut
	{
		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x0005EBD3 File Offset: 0x0005CDD3
		// (set) Token: 0x06000BF9 RID: 3065 RVA: 0x0005EBDB File Offset: 0x0005CDDB
		public Move OpeningMove { get; private set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000BFA RID: 3066 RVA: 0x0005EBE4 File Offset: 0x0005CDE4
		private bool IsLeaf
		{
			get
			{
				return this._children == null;
			}
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x0005EBEF File Offset: 0x0005CDEF
		public TreeNodeTablut(BoardGameSide side, int depth)
		{
			this._side = side;
			this._depth = depth;
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x0005EC05 File Offset: 0x0005CE05
		public static TreeNodeTablut CreateTreeAndReturnRootNode(BoardGameTablut.BoardInformation initialBoardState, int maxDepth)
		{
			TreeNodeTablut.MaxDepth = maxDepth;
			return new TreeNodeTablut(BoardGameSide.Player, 0)
			{
				_boardState = initialBoardState
			};
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x0005EC1C File Offset: 0x0005CE1C
		public TreeNodeTablut GetChildWithBestScore()
		{
			TreeNodeTablut treeNodeTablut = null;
			if (!this.IsLeaf)
			{
				float num = float.MinValue;
				foreach (TreeNodeTablut treeNodeTablut2 in this._children)
				{
					if (treeNodeTablut2._visits > 0)
					{
						float num2 = (float)treeNodeTablut2._wins / (float)treeNodeTablut2._visits;
						if (num2 > num)
						{
							treeNodeTablut = treeNodeTablut2;
							num = num2;
						}
					}
				}
			}
			return treeNodeTablut;
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x0005ECA0 File Offset: 0x0005CEA0
		public void SelectAction()
		{
			TreeNodeTablut treeNodeTablut = this;
			while (!treeNodeTablut.IsLeaf)
			{
				treeNodeTablut = treeNodeTablut.Select();
			}
			TreeNodeTablut.ExpandResult expandResult = treeNodeTablut.Expand();
			if (expandResult == TreeNodeTablut.ExpandResult.NeedsToBeSimulated)
			{
				if (treeNodeTablut._children != null)
				{
					treeNodeTablut = treeNodeTablut.Select();
				}
				BoardGameTablut.State state = this.Simulate(ref treeNodeTablut);
				if (state != BoardGameTablut.State.Aborted)
				{
					BoardGameSide boardGameSide = ((state == BoardGameTablut.State.AIWon) ? BoardGameSide.AI : BoardGameSide.Player);
					treeNodeTablut.BackPropagate(boardGameSide);
					return;
				}
			}
			else
			{
				BoardGameSide boardGameSide2 = ((expandResult == TreeNodeTablut.ExpandResult.AIWon) ? BoardGameSide.AI : BoardGameSide.Player);
				treeNodeTablut.BackPropagate(boardGameSide2);
			}
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x0005ED0C File Offset: 0x0005CF0C
		private TreeNodeTablut Select()
		{
			TreeNodeTablut treeNodeTablut = null;
			if (!this.IsLeaf)
			{
				double num = double.MinValue;
				foreach (TreeNodeTablut treeNodeTablut2 in this._children)
				{
					if (treeNodeTablut2._visits == 0)
					{
						treeNodeTablut = treeNodeTablut2;
						break;
					}
					double num2 = (double)treeNodeTablut2._wins / (double)treeNodeTablut2._visits + (double)(1.5f * MathF.Sqrt(MathF.Log((float)this._visits) / (float)treeNodeTablut2._visits));
					if (num2 > num)
					{
						treeNodeTablut = treeNodeTablut2;
						num = num2;
					}
				}
				if (treeNodeTablut != null && treeNodeTablut._boardState.PawnInformation == null)
				{
					BoardGameAITablut.Board.UndoMove(ref treeNodeTablut._parent._boardState);
					BoardGameAITablut.Board.AIMakeMove(treeNodeTablut.OpeningMove);
					treeNodeTablut._boardState = BoardGameAITablut.Board.TakeBoardSnapshot();
				}
			}
			return treeNodeTablut;
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x0005EE00 File Offset: 0x0005D000
		private TreeNodeTablut.ExpandResult Expand()
		{
			TreeNodeTablut.ExpandResult expandResult = TreeNodeTablut.ExpandResult.NeedsToBeSimulated;
			if (this._depth < TreeNodeTablut.MaxDepth)
			{
				BoardGameAITablut.Board.UndoMove(ref this._boardState);
				BoardGameTablut.State state = BoardGameAITablut.Board.CheckGameState();
				if (state == BoardGameTablut.State.InProgress)
				{
					BoardGameSide boardGameSide = ((this._side == BoardGameSide.Player) ? BoardGameSide.AI : BoardGameSide.Player);
					List<List<Move>> list = BoardGameAITablut.Board.CalculateAllValidMoves(boardGameSide);
					int totalMovesAvailable = BoardGameAITablut.Board.GetTotalMovesAvailable(ref list);
					if (totalMovesAvailable > 0)
					{
						this._children = new List<TreeNodeTablut>(totalMovesAvailable);
						using (List<List<Move>>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								List<Move> list2 = enumerator.Current;
								foreach (Move move in list2)
								{
									TreeNodeTablut treeNodeTablut = new TreeNodeTablut(boardGameSide, this._depth + 1);
									treeNodeTablut.OpeningMove = move;
									treeNodeTablut._parent = this;
									this._children.Add(treeNodeTablut);
								}
							}
							return expandResult;
						}
					}
					Debug.FailedAssert("No available moves left but the game is in progress", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\AI\\TreeNodeTablut.cs", "Expand", 203);
				}
				else if (state != BoardGameTablut.State.Aborted)
				{
					if (state == BoardGameTablut.State.AIWon)
					{
						expandResult = TreeNodeTablut.ExpandResult.AIWon;
					}
					else
					{
						expandResult = TreeNodeTablut.ExpandResult.PlayerWon;
					}
				}
			}
			return expandResult;
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x0005EF48 File Offset: 0x0005D148
		private BoardGameTablut.State Simulate(ref TreeNodeTablut tn)
		{
			BoardGameAITablut.Board.UndoMove(ref tn._boardState);
			BoardGameTablut.State state = BoardGameAITablut.Board.CheckGameState();
			BoardGameSide side = tn._side;
			while (state == BoardGameTablut.State.InProgress)
			{
				List<PawnBase> list = ((tn._side == BoardGameSide.Player) ? BoardGameAITablut.Board.PlayerOneUnits : BoardGameAITablut.Board.PlayerTwoUnits);
				int count = list.Count;
				int num = 3;
				PawnBase pawnBase;
				bool flag;
				do
				{
					pawnBase = list[MBRandom.RandomInt(count)];
					flag = BoardGameAITablut.Board.HasAvailableMoves(pawnBase as PawnTablut);
					num--;
				}
				while (!flag && num > 0);
				if (!flag)
				{
					pawnBase = list.OrderBy((PawnBase x) => MBRandom.RandomInt()).FirstOrDefault((PawnBase x) => BoardGameAITablut.Board.HasAvailableMoves(x as PawnTablut));
					flag = pawnBase != null;
				}
				if (flag)
				{
					Move randomAvailableMove = BoardGameAITablut.Board.GetRandomAvailableMove(pawnBase as PawnTablut);
					BoardGameAITablut.Board.AIMakeMove(randomAvailableMove);
					state = BoardGameAITablut.Board.CheckGameState();
				}
				else if (tn._side == BoardGameSide.Player)
				{
					state = BoardGameTablut.State.AIWon;
				}
				else
				{
					state = BoardGameTablut.State.PlayerWon;
				}
				tn._side = ((tn._side == BoardGameSide.Player) ? BoardGameSide.AI : BoardGameSide.Player);
			}
			tn._side = side;
			return state;
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x0005F098 File Offset: 0x0005D298
		private void BackPropagate(BoardGameSide winner)
		{
			for (TreeNodeTablut treeNodeTablut = this; treeNodeTablut != null; treeNodeTablut = treeNodeTablut._parent)
			{
				treeNodeTablut._visits++;
				if (winner == treeNodeTablut._side)
				{
					treeNodeTablut._wins++;
				}
			}
		}

		// Token: 0x0400043C RID: 1084
		private const float UCTConstant = 1.5f;

		// Token: 0x0400043D RID: 1085
		private static int MaxDepth;

		// Token: 0x0400043E RID: 1086
		private readonly int _depth;

		// Token: 0x0400043F RID: 1087
		private BoardGameTablut.BoardInformation _boardState;

		// Token: 0x04000440 RID: 1088
		private TreeNodeTablut _parent;

		// Token: 0x04000441 RID: 1089
		private List<TreeNodeTablut> _children;

		// Token: 0x04000442 RID: 1090
		private BoardGameSide _side;

		// Token: 0x04000443 RID: 1091
		private int _visits;

		// Token: 0x04000444 RID: 1092
		private int _wins;

		// Token: 0x020001BC RID: 444
		private enum ExpandResult
		{
			// Token: 0x04000836 RID: 2102
			NeedsToBeSimulated,
			// Token: 0x04000837 RID: 2103
			AIWon,
			// Token: 0x04000838 RID: 2104
			PlayerWon
		}
	}
}
