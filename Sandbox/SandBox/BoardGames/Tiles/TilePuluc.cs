using System;
using SandBox.BoardGames.Objects;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.Tiles
{
	// Token: 0x020000B7 RID: 183
	public class TilePuluc : Tile1D
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000B23 RID: 2851 RVA: 0x0005B5D1 File Offset: 0x000597D1
		// (set) Token: 0x06000B24 RID: 2852 RVA: 0x0005B5D9 File Offset: 0x000597D9
		public Vec3 PosLeft { get; private set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000B25 RID: 2853 RVA: 0x0005B5E2 File Offset: 0x000597E2
		// (set) Token: 0x06000B26 RID: 2854 RVA: 0x0005B5EA File Offset: 0x000597EA
		public Vec3 PosLeftMid { get; private set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000B27 RID: 2855 RVA: 0x0005B5F3 File Offset: 0x000597F3
		// (set) Token: 0x06000B28 RID: 2856 RVA: 0x0005B5FB File Offset: 0x000597FB
		public Vec3 PosRight { get; private set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000B29 RID: 2857 RVA: 0x0005B604 File Offset: 0x00059804
		// (set) Token: 0x06000B2A RID: 2858 RVA: 0x0005B60C File Offset: 0x0005980C
		public Vec3 PosRightMid { get; private set; }

		// Token: 0x06000B2B RID: 2859 RVA: 0x0005B615 File Offset: 0x00059815
		public TilePuluc(GameEntity entity, BoardGameDecal decal, int x)
			: base(entity, decal, x)
		{
			this.UpdateTilePosition();
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x0005B628 File Offset: 0x00059828
		public void UpdateTilePosition()
		{
			MatrixFrame globalFrame = base.Entity.GetGlobalFrame();
			MetaMesh tileMesh = base.Entity.GetFirstScriptOfType<Tile>().TileMesh;
			Vec3 vec = tileMesh.GetBoundingBox().max - tileMesh.GetBoundingBox().min;
			Mat3 mat = globalFrame.rotation.TransformToParent(tileMesh.Frame.rotation);
			Vec3 vec2 = mat.TransformToParent(new Vec3(0f, vec.y / 6f, 0f, -1f));
			Vec3 vec3 = mat.TransformToParent(new Vec3(0f, vec.y / 3f, 0f, -1f));
			Vec3 globalPosition = base.Entity.GlobalPosition;
			this.PosLeft = globalPosition + vec3;
			this.PosLeftMid = globalPosition + vec2;
			this.PosRight = globalPosition - vec3;
			this.PosRightMid = globalPosition - vec2;
		}
	}
}
