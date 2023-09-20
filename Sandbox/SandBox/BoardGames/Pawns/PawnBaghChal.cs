using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.Pawns
{
	public class PawnBaghChal : PawnBase
	{
		public override bool IsPlaced
		{
			get
			{
				return this.X >= 0 && this.X < BoardGameBaghChal.BoardWidth && this.Y >= 0 && this.Y < BoardGameBaghChal.BoardHeight;
			}
		}

		public MatrixFrame InitialFrame { get; }

		public bool IsTiger { get; }

		public bool IsGoat
		{
			get
			{
				return !this.IsTiger;
			}
		}

		public PawnBaghChal(GameEntity entity, bool playerOne, bool isTiger)
			: base(entity, playerOne)
		{
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
			this.IsTiger = isTiger;
			this.InitialFrame = base.Entity.GetFrame();
		}

		public override void Reset()
		{
			base.Reset();
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
		}

		public int X;

		public int Y;

		public int PrevX;

		public int PrevY;
	}
}
