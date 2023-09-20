using System;
using SandBox.BoardGames.Objects;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Tiles
{
	// Token: 0x020000B6 RID: 182
	public class TileMuTorere : Tile1D
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000B20 RID: 2848 RVA: 0x0005B5A6 File Offset: 0x000597A6
		public int XLeftTile { get; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000B21 RID: 2849 RVA: 0x0005B5AE File Offset: 0x000597AE
		public int XRightTile { get; }

		// Token: 0x06000B22 RID: 2850 RVA: 0x0005B5B6 File Offset: 0x000597B6
		public TileMuTorere(GameEntity entity, BoardGameDecal decal, int x, int xLeft, int xRight)
			: base(entity, decal, x)
		{
			this.XLeftTile = xLeft;
			this.XRightTile = xRight;
		}
	}
}
