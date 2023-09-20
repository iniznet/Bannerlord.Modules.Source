using System;
using SandBox.BoardGames.Pawns;
using SandBox.BoardGames.Tiles;

namespace SandBox.BoardGames
{
	public struct Move
	{
		public bool IsValid
		{
			get
			{
				return this.Unit != null && this.GoalTile != null;
			}
		}

		public Move(PawnBase unit, TileBase goalTile)
		{
			this.Unit = unit;
			this.GoalTile = goalTile;
		}

		public static readonly Move Invalid = new Move
		{
			Unit = null,
			GoalTile = null
		};

		public PawnBase Unit;

		public TileBase GoalTile;
	}
}
