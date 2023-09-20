using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.Pawns
{
	// Token: 0x020000B8 RID: 184
	public class PawnBaghChal : PawnBase
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000B2D RID: 2861 RVA: 0x0005B721 File Offset: 0x00059921
		public override bool IsPlaced
		{
			get
			{
				return this.X >= 0 && this.X < BoardGameBaghChal.BoardWidth && this.Y >= 0 && this.Y < BoardGameBaghChal.BoardHeight;
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000B2E RID: 2862 RVA: 0x0005B751 File Offset: 0x00059951
		public MatrixFrame InitialFrame { get; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000B2F RID: 2863 RVA: 0x0005B759 File Offset: 0x00059959
		public bool IsTiger { get; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000B30 RID: 2864 RVA: 0x0005B761 File Offset: 0x00059961
		public bool IsGoat
		{
			get
			{
				return !this.IsTiger;
			}
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0005B76C File Offset: 0x0005996C
		public PawnBaghChal(GameEntity entity, bool playerOne, bool isTiger)
			: base(entity, playerOne)
		{
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
			this.IsTiger = isTiger;
			this.InitialFrame = base.Entity.GetFrame();
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x0005B7AA File Offset: 0x000599AA
		public override void Reset()
		{
			base.Reset();
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
		}

		// Token: 0x040003E5 RID: 997
		public int X;

		// Token: 0x040003E6 RID: 998
		public int Y;

		// Token: 0x040003E7 RID: 999
		public int PrevX;

		// Token: 0x040003E8 RID: 1000
		public int PrevY;
	}
}
