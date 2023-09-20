using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Pawns
{
	// Token: 0x020000BA RID: 186
	public class PawnKonane : PawnBase
	{
		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000B5B RID: 2907 RVA: 0x0005BF47 File Offset: 0x0005A147
		public override bool IsPlaced
		{
			get
			{
				return this.X >= 0 && this.X < BoardGameKonane.BoardWidth && this.Y >= 0 && this.Y < BoardGameKonane.BoardHeight;
			}
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x0005BF77 File Offset: 0x0005A177
		public PawnKonane(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x0005BF9D File Offset: 0x0005A19D
		public override void Reset()
		{
			base.Reset();
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
		}

		// Token: 0x04000401 RID: 1025
		public int X;

		// Token: 0x04000402 RID: 1026
		public int Y;

		// Token: 0x04000403 RID: 1027
		public int PrevX;

		// Token: 0x04000404 RID: 1028
		public int PrevY;
	}
}
