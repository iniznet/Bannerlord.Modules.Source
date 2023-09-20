using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Pawns
{
	public class PawnKonane : PawnBase
	{
		public override bool IsPlaced
		{
			get
			{
				return this.X >= 0 && this.X < BoardGameKonane.BoardWidth && this.Y >= 0 && this.Y < BoardGameKonane.BoardHeight;
			}
		}

		public PawnKonane(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.X = -1;
			this.Y = -1;
			this.PrevX = -1;
			this.PrevY = -1;
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
