using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Pawns
{
	// Token: 0x020000BB RID: 187
	public class PawnMuTorere : PawnBase
	{
		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000B5E RID: 2910 RVA: 0x0005BFC1 File Offset: 0x0005A1C1
		// (set) Token: 0x06000B5F RID: 2911 RVA: 0x0005BFC9 File Offset: 0x0005A1C9
		public int X { get; set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000B60 RID: 2912 RVA: 0x0005BFD2 File Offset: 0x0005A1D2
		public override bool IsPlaced
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x0005BFD5 File Offset: 0x0005A1D5
		public PawnMuTorere(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.X = -1;
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x0005BFE6 File Offset: 0x0005A1E6
		public override void Reset()
		{
			base.Reset();
			this.X = -1;
		}
	}
}
