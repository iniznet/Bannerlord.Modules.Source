using System;
using Helpers;
using SandBox.BoardGames.MissionLogics;

namespace SandBox.BoardGames.AI
{
	// Token: 0x020000C8 RID: 200
	public class BoardGameAITablut : BoardGameAIBase
	{
		// Token: 0x06000BF5 RID: 3061 RVA: 0x0005EAB6 File Offset: 0x0005CCB6
		public BoardGameAITablut(BoardGameHelper.AIDifficulty difficulty, MissionBoardGameLogic boardGameHandler)
			: base(difficulty, boardGameHandler)
		{
			BoardGameAITablut.Board = base.BoardGameHandler.Board as BoardGameTablut;
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0005EAD8 File Offset: 0x0005CCD8
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

		// Token: 0x06000BF7 RID: 3063 RVA: 0x0005EB74 File Offset: 0x0005CD74
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

		// Token: 0x0400043A RID: 1082
		public static BoardGameTablut Board;

		// Token: 0x0400043B RID: 1083
		private int _sampleCount;
	}
}
