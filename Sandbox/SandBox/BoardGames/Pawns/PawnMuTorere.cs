using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Pawns
{
	public class PawnMuTorere : PawnBase
	{
		public int X { get; set; }

		public override bool IsPlaced
		{
			get
			{
				return true;
			}
		}

		public PawnMuTorere(GameEntity entity, bool playerOne)
			: base(entity, playerOne)
		{
			this.X = -1;
		}

		public override void Reset()
		{
			base.Reset();
			this.X = -1;
		}
	}
}
