using System;
using SandBox.BoardGames.Objects;
using SandBox.BoardGames.Pawns;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.Tiles
{
	// Token: 0x020000B5 RID: 181
	public abstract class TileBase
	{
		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000B1A RID: 2842 RVA: 0x0005B511 File Offset: 0x00059711
		public GameEntity Entity { get; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000B1B RID: 2843 RVA: 0x0005B519 File Offset: 0x00059719
		public BoardGameDecal ValidMoveDecal { get; }

		// Token: 0x06000B1C RID: 2844 RVA: 0x0005B521 File Offset: 0x00059721
		protected TileBase(GameEntity entity, BoardGameDecal decal)
		{
			this.Entity = entity;
			this.ValidMoveDecal = decal;
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x0005B537 File Offset: 0x00059737
		public virtual void Reset()
		{
			this.PawnOnTile = null;
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x0005B540 File Offset: 0x00059740
		public void Tick(float dt)
		{
			int num = (this._showTile ? 1 : (-1));
			this._tileFadeTimer += (float)num * dt * 5f;
			this._tileFadeTimer = MBMath.ClampFloat(this._tileFadeTimer, 0f, 1f);
			this.ValidMoveDecal.SetAlpha(this._tileFadeTimer);
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x0005B59D File Offset: 0x0005979D
		public void SetVisibility(bool isVisible)
		{
			this._showTile = isVisible;
		}

		// Token: 0x040003D9 RID: 985
		public PawnBase PawnOnTile;

		// Token: 0x040003DA RID: 986
		private bool _showTile;

		// Token: 0x040003DB RID: 987
		private float _tileFadeTimer;

		// Token: 0x040003DC RID: 988
		private const float TileFadeDuration = 0.2f;
	}
}
