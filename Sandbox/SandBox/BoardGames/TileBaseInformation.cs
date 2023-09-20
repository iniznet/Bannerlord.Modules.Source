using System;
using SandBox.BoardGames.Pawns;

namespace SandBox.BoardGames
{
	public struct TileBaseInformation
	{
		public TileBaseInformation(ref PawnBase pawnOnTile)
		{
			this.PawnOnTile = pawnOnTile;
		}

		public PawnBase PawnOnTile;
	}
}
