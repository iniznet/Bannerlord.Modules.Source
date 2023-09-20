using System;

namespace TaleWorlds.Core
{
	public struct PieceData
	{
		public CraftingPiece.PieceTypes PieceType { get; private set; }

		public int Order { get; private set; }

		public PieceData(CraftingPiece.PieceTypes pieceType, int order)
		{
			this.PieceType = pieceType;
			this.Order = order;
		}
	}
}
