using System;
using Helpers;
using SandBox.BoardGames.MissionLogics;

namespace SandBox.BoardGames.AI
{
	public class BoardGameAITablut : BoardGameAIBase
	{
		public BoardGameAITablut(BoardGameHelper.AIDifficulty difficulty, MissionBoardGameLogic boardGameHandler)
			: base(difficulty, boardGameHandler)
		{
			BoardGameAITablut.Board = base.BoardGameHandler.Board as BoardGameTablut;
		}

		public override Move CalculateMovementStageMove()
		{
			Move openingMove;
			openingMove.GoalTile = null;
			openingMove.Unit = null;
			if (BoardGameAITablut.Board.IsReady)
			{
				BoardGameTablut.BoardInformation boardInformation = BoardGameAITablut.Board.TakeBoardSnapshot();
				TreeNodeTablut treeNodeTablut = TreeNodeTablut.CreateTreeAndReturnRootNode(boardInformation, this.MaxDepth);
				int num = 0;
				while (num < this._sampleCount && !base.AbortRequested)
				{
					treeNodeTablut.SelectAction();
					num++;
				}
				if (!base.AbortRequested)
				{
					BoardGameAITablut.Board.UndoMove(ref boardInformation);
					TreeNodeTablut childWithBestScore = treeNodeTablut.GetChildWithBestScore();
					if (childWithBestScore != null)
					{
						openingMove = childWithBestScore.OpeningMove;
					}
				}
			}
			if (!base.AbortRequested)
			{
				bool isValid = openingMove.IsValid;
			}
			return openingMove;
		}

		protected override void InitializeDifficulty()
		{
			switch (base.Difficulty)
			{
			case 0:
				this.MaxDepth = 3;
				this._sampleCount = 30000;
				return;
			case 1:
				this.MaxDepth = 4;
				this._sampleCount = 47000;
				return;
			case 2:
				this.MaxDepth = 5;
				this._sampleCount = 64000;
				return;
			default:
				return;
			}
		}

		public static BoardGameTablut Board;

		private int _sampleCount;
	}
}
