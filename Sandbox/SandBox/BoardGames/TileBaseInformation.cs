using System;
using SandBox.BoardGames.Pawns;

namespace SandBox.BoardGames
{
	// Token: 0x020000AA RID: 170
	public struct TileBaseInformation
	{
		// Token: 0x06000A1A RID: 2586 RVA: 0x00053BC7 File Offset: 0x00051DC7
		public TileBaseInformation(ref PawnBase pawnOnTile)
		{
			this.PawnOnTile = pawnOnTile;
		}

		// Token: 0x04000377 RID: 887
		public PawnBase PawnOnTile;
	}
}
