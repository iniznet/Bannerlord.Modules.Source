using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Pawns
{
	public class PawnSeega : PawnBase
	{
		public override bool IsPlaced
		{
			get
			{
				return this.X >= 0 && this.X < BoardGameSeega.BoardWidth && this.Y >= 0 && this.Y < BoardGameSeega.BoardHeight;
			}
		}

		public bool MovedThisTurn { get; private set; }

		public int PrevX
		{
			get
			{
				return this._prevX;
			}
			set
			{
				this._prevX = value;
				if (value >= 0)
				{
					this.MovedThisTurn = true;
					return;
				}
				this.MovedThisTurn = false;
			}
		}

		public int PrevY
		{
			get
			{
				return this._prevY;
			}
			set
			{
				this._prevY = value;
				if (value >= 0)
				{
					this.MovedThisTurn = true;
					return;
				}
				this.MovedThisTurn = false;
			}
		}

		public PawnSeega(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
			this.MovedThisTurn = false;
		}

		public override void Reset()
		{
			base.Reset();
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
			this.MovedThisTurn = false;
		}

		public void UpdateMoveBackAvailable()
		{
			if (this.MovedThisTurn)
			{
				this.MovedThisTurn = false;
				return;
			}
			this.PrevX = -1;
			this.PrevY = -1;
		}

		public void AISetMovedThisTurn(bool moved)
		{
			this.MovedThisTurn = moved;
		}

		public int X;

		public int Y;

		private int _prevX;

		private int _prevY;
	}
}
