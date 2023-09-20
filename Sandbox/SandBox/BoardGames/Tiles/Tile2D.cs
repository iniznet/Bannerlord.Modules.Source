using System;
using SandBox.BoardGames.Objects;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Tiles
{
	// Token: 0x020000B4 RID: 180
	public class Tile2D : TileBase
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000B17 RID: 2839 RVA: 0x0005B4E8 File Offset: 0x000596E8
		public int X { get; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000B18 RID: 2840 RVA: 0x0005B4F0 File Offset: 0x000596F0
		public int Y { get; }

		// Token: 0x06000B19 RID: 2841 RVA: 0x0005B4F8 File Offset: 0x000596F8
		public Tile2D(GameEntity entity, BoardGameDecal decal, int x, int y)
			: base(entity, decal)
		{
			this.X = x;
			this.Y = y;
		}
	}
}
