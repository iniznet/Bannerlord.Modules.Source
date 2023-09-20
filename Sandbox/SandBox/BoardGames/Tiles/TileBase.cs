using System;
using SandBox.BoardGames.Objects;
using SandBox.BoardGames.Pawns;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.Tiles
{
	public abstract class TileBase
	{
		public GameEntity Entity { get; }

		public BoardGameDecal ValidMoveDecal { get; }

		protected TileBase(GameEntity entity, BoardGameDecal decal)
		{
			this.Entity = entity;
			this.ValidMoveDecal = decal;
		}

		public virtual void Reset()
		{
			this.PawnOnTile = null;
		}

		public void Tick(float dt)
		{
			int num = (this._showTile ? 1 : (-1));
			this._tileFadeTimer += (float)num * dt * 5f;
			this._tileFadeTimer = MBMath.ClampFloat(this._tileFadeTimer, 0f, 1f);
			this.ValidMoveDecal.SetAlpha(this._tileFadeTimer);
		}

		public void SetVisibility(bool isVisible)
		{
			this._showTile = isVisible;
		}

		public PawnBase PawnOnTile;

		private bool _showTile;

		private float _tileFadeTimer;

		private const float TileFadeDuration = 0.2f;
	}
}
