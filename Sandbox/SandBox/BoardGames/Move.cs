using System;
using SandBox.BoardGames.Pawns;
using SandBox.BoardGames.Tiles;

namespace SandBox.BoardGames
{
	// Token: 0x020000AB RID: 171
	public struct Move
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000A1B RID: 2587 RVA: 0x00053BD1 File Offset: 0x00051DD1
		public bool IsValid
		{
			get
			{
				return this.Unit != null && this.GoalTile != null;
			}
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00053BE6 File Offset: 0x00051DE6
		public Move(PawnBase unit, TileBase goalTile)
		{
			this.Unit = unit;
			this.GoalTile = goalTile;
		}

		// Token: 0x04000378 RID: 888
		public static readonly Move Invalid = new Move
		{
			Unit = null,
			GoalTile = null
		};

		// Token: 0x04000379 RID: 889
		public PawnBase Unit;

		// Token: 0x0400037A RID: 890
		public TileBase GoalTile;
	}
}
