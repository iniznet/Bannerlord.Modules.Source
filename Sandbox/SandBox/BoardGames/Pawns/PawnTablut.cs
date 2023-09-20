using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Pawns
{
	// Token: 0x020000BE RID: 190
	public class PawnTablut : PawnBase
	{
		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000B7D RID: 2941 RVA: 0x0005C4EC File Offset: 0x0005A6EC
		public override bool IsPlaced
		{
			get
			{
				return this.X >= 0 && this.X < 9 && this.Y >= 0 && this.Y < 9;
			}
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x0005C516 File Offset: 0x0005A716
		public PawnTablut(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.X = -1;
			this.Y = -1;
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x0005C52E File Offset: 0x0005A72E
		public override void Reset()
		{
			base.Reset();
			this.X = -1;
			this.Y = -1;
		}

		// Token: 0x04000413 RID: 1043
		public int X;

		// Token: 0x04000414 RID: 1044
		public int Y;
	}
}
