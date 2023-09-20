using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.BoardGames.Pawns;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.BoardGames.AI
{
	public class TreeNodeTablut
	{
		public Move OpeningMove { get; private set; }

		private bool IsLeaf
		{
			get
			{
				return this._children == null;
			}
		}

		public TreeNodeTablut(BoardGameSide side, int depth)
		{
			this._side = side;
			this._depth = depth;
		}

		public static TreeNodeTablut CreateTreeAndReturnRootNode(BoardGameTablut.BoardInformation initialBoardState, int maxDepth)
		{
			TreeNodeTablut.MaxDepth = maxDepth;
			return new TreeNodeTablut(BoardGameSide.Player, 0)
			{
				_boardState = initialBoardState
			};
		}

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

		private const float UCTConstant = 1.5f;

		private static int MaxDepth;

		private readonly int _depth;

		private BoardGameTablut.BoardInformation _boardState;

		private TreeNodeTablut _parent;

		private List<TreeNodeTablut> _children;

		private BoardGameSide _side;

		private int _visits;

		private int _wins;

		private enum ExpandResult
		{
			NeedsToBeSimulated,
			AIWon,
			PlayerWon
		}
	}
}
