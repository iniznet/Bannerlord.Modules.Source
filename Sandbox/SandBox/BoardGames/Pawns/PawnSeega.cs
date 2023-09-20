using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Pawns
{
	// Token: 0x020000BD RID: 189
	public class PawnSeega : PawnBase
	{
		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000B72 RID: 2930 RVA: 0x0005C3E2 File Offset: 0x0005A5E2
		public override bool IsPlaced
		{
			get
			{
				return this.X >= 0 && this.X < BoardGameSeega.BoardWidth && this.Y >= 0 && this.Y < BoardGameSeega.BoardHeight;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000B73 RID: 2931 RVA: 0x0005C412 File Offset: 0x0005A612
		// (set) Token: 0x06000B74 RID: 2932 RVA: 0x0005C41A File Offset: 0x0005A61A
		public bool MovedThisTurn { get; private set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000B75 RID: 2933 RVA: 0x0005C423 File Offset: 0x0005A623
		// (set) Token: 0x06000B76 RID: 2934 RVA: 0x0005C42B File Offset: 0x0005A62B
		public int PrevX
		{
			get
			{
				return this._prevX;
			}
			set
			{
				this._prevX = value;
				if (value >= 0)
				{
					this.MovedThisTurn = true;
					return;
				}
				this.MovedThisTurn = false;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000B77 RID: 2935 RVA: 0x0005C447 File Offset: 0x0005A647
		// (set) Token: 0x06000B78 RID: 2936 RVA: 0x0005C44F File Offset: 0x0005A64F
		public int PrevY
		{
			get
			{
				return this._prevY;
			}
			set
			{
				this._prevY = value;
				if (value >= 0)
				{
					this.MovedThisTurn = true;
					return;
				}
				this.MovedThisTurn = false;
			}
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0005C46B File Offset: 0x0005A66B
		public PawnSeega(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
			this.MovedThisTurn = false;
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x0005C498 File Offset: 0x0005A698
		public override void Reset()
		{
			base.Reset();
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
			this.MovedThisTurn = false;
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x0005C4C3 File Offset: 0x0005A6C3
		public void UpdateMoveBackAvailable()
		{
			if (this.MovedThisTurn)
			{
				this.MovedThisTurn = false;
				return;
			}
			this.PrevX = -1;
			this.PrevY = -1;
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x0005C4E3 File Offset: 0x0005A6E3
		public void AISetMovedThisTurn(bool moved)
		{
			this.MovedThisTurn = moved;
		}

		// Token: 0x0400040E RID: 1038
		public int X;

		// Token: 0x0400040F RID: 1039
		public int Y;

		// Token: 0x04000410 RID: 1040
		private int _prevX;

		// Token: 0x04000411 RID: 1041
		private int _prevY;
	}
}
