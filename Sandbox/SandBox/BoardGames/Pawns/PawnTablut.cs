using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Pawns
{
	public class PawnTablut : PawnBase
	{
		public override bool IsPlaced
		{
			get
			{
				return this.X >= 0 && this.X < 9 && this.Y >= 0 && this.Y < 9;
			}
		}

		public PawnTablut(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.X = -1;
			this.Y = -1;
		}

		public override void Reset()
		{
			base.Reset();
			this.X = -1;
			this.Y = -1;
		}

		public int X;

		public int Y;
	}
}
