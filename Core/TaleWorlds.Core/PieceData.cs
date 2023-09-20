using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000045 RID: 69
	public struct PieceData
	{
		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600055E RID: 1374 RVA: 0x00013CB4 File Offset: 0x00011EB4
		// (set) Token: 0x0600055F RID: 1375 RVA: 0x00013CBC File Offset: 0x00011EBC
		public CraftingPiece.PieceTypes PieceType { get; private set; }

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x00013CC5 File Offset: 0x00011EC5
		// (set) Token: 0x06000561 RID: 1377 RVA: 0x00013CCD File Offset: 0x00011ECD
		public int Order { get; private set; }

		// Token: 0x06000562 RID: 1378 RVA: 0x00013CD6 File Offset: 0x00011ED6
		public PieceData(CraftingPiece.PieceTypes pieceType, int order)
		{
			this.PieceType = pieceType;
			this.Order = order;
		}
	}
}
