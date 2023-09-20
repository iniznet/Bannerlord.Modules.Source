using System;
using SandBox.BoardGames.Objects;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Tiles
{
	// Token: 0x020000B3 RID: 179
	public class Tile1D : TileBase
	{
		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000B15 RID: 2837 RVA: 0x0005B4CF File Offset: 0x000596CF
		public int X { get; }

		// Token: 0x06000B16 RID: 2838 RVA: 0x0005B4D7 File Offset: 0x000596D7
		public Tile1D(GameEntity entity, BoardGameDecal decal, int x)
			: base(entity, decal)
		{
			this.X = x;
		}
	}
}
