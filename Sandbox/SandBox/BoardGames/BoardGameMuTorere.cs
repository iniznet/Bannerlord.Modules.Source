using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.BoardGames.MissionLogics;
using SandBox.BoardGames.Objects;
using SandBox.BoardGames.Pawns;
using SandBox.BoardGames.Tiles;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.BoardGames
{
	// Token: 0x020000AE RID: 174
	public class BoardGameMuTorere : BoardGameBase
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000A99 RID: 2713 RVA: 0x00056E36 File Offset: 0x00055036
		public override int TileCount
		{
			get
			{
				return 9;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000A9A RID: 2714 RVA: 0x00056E3A File Offset: 0x0005503A
		protected override bool RotateBoard
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000A9B RID: 2715 RVA: 0x00056E3D File Offset: 0x0005503D
		protected override bool PreMovementStagePresent
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000A9C RID: 2716 RVA: 0x00056E40 File Offset: 0x00055040
		protected override bool DiceRollRequired
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00056E43 File Offset: 0x00055043
		public BoardGameMuTorere(MissionBoardGameLogic mission, PlayerTurn startingPlayer)
			: base(mission, new TextObject("{=5siAbi69}Mu Torere", null), startingPlayer)
		{
			this.PawnUnselectedFactor = 4288711820U;
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x00056E64 File Offset: 0x00055064
		public override void InitializeUnits()
		{
			base.PlayerOneUnits.Clear();
			base.PlayerTwoUnits.Clear();
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			for (int i = 0; i < 4; i++)
			{
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("player_one_unit_" + i);
				list.Add(base.InitializeUnit(new PawnMuTorere(gameEntity, base.PlayerWhoStarted == PlayerTurn.PlayerOne)));
			}
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			for (int j = 0; j < 4; j++)
			{
				GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("player_two_unit_" + j);
				list2.Add(base.InitializeUnit(new PawnMuTorere(gameEntity2, base.PlayerWhoStarted > PlayerTurn.PlayerOne)));
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x00056F4C File Offset: 0x0005514C
		public override void InitializeTiles()
		{
			if (base.Tiles == null)
			{
				base.Tiles = new TileBase[this.TileCount];
			}
			int x;
			IEnumerable<GameEntity> enumerable = from x in this.BoardEntity.GetChildren()
				where x.Tags.Any((string t) => t.Contains("tile_"))
				select x;
			IEnumerable<GameEntity> enumerable2 = from x in this.BoardEntity.GetChildren()
				where x.Tags.Any((string t) => t.Contains("decal_"))
				select x;
			int num;
			for (x = 0; x < this.TileCount; x = num)
			{
				GameEntity gameEntity = enumerable.Single((GameEntity e) => e.HasTag("tile_" + x));
				BoardGameDecal firstScriptOfType = enumerable2.Single((GameEntity e) => e.HasTag("decal_" + x)).GetFirstScriptOfType<BoardGameDecal>();
				num = x;
				int num2;
				int num3;
				if (num != 0)
				{
					if (num != 1)
					{
						if (num != 8)
						{
							num2 = x - 1;
							num3 = x + 1;
						}
						else
						{
							num2 = 7;
							num3 = 1;
						}
					}
					else
					{
						num2 = 8;
						num3 = 2;
					}
				}
				else
				{
					num3 = (num2 = -1);
				}
				base.Tiles[x] = new TileMuTorere(gameEntity, firstScriptOfType, x, num2, num3);
				num = x + 1;
			}
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00057096 File Offset: 0x00055296
		public override void InitializeCapturedUnitsZones()
		{
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00057098 File Offset: 0x00055298
		public override void InitializeSound()
		{
			PawnBase.PawnMoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/move_stone");
			PawnBase.PawnSelectSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/pick_stone");
			PawnBase.PawnTapSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/drop_wood");
			PawnBase.PawnRemoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/out_stone");
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x000570D6 File Offset: 0x000552D6
		public override void Reset()
		{
			base.Reset();
			this.PreplaceUnits();
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x000570E4 File Offset: 0x000552E4
		public override List<Move> CalculateValidMoves(PawnBase pawn)
		{
			List<Move> list = new List<Move>();
			PawnMuTorere pawnMuTorere = pawn as PawnMuTorere;
			if (pawnMuTorere != null)
			{
				TileMuTorere tileMuTorere = this.FindAvailableTile() as TileMuTorere;
				if (pawnMuTorere.X == 0)
				{
					Move move;
					move.Unit = pawn;
					move.GoalTile = tileMuTorere;
					list.Add(move);
				}
				else if (tileMuTorere.X != 0)
				{
					if (pawnMuTorere.X == tileMuTorere.XLeftTile || pawnMuTorere.X == tileMuTorere.XRightTile)
					{
						Move move2;
						move2.Unit = pawn;
						move2.GoalTile = tileMuTorere;
						list.Add(move2);
					}
				}
				else
				{
					TileMuTorere tileMuTorere2 = this.FindTileByCoordinate(pawnMuTorere.X);
					PawnBase pawnOnTile = base.Tiles[tileMuTorere2.XLeftTile].PawnOnTile;
					PawnBase pawnOnTile2 = base.Tiles[tileMuTorere2.XRightTile].PawnOnTile;
					if (pawnOnTile.PlayerOne != pawnMuTorere.PlayerOne || pawnOnTile2.PlayerOne != pawnMuTorere.PlayerOne)
					{
						Move move3;
						move3.Unit = pawn;
						move3.GoalTile = tileMuTorere;
						list.Add(move3);
					}
				}
			}
			return list;
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x000571E0 File Offset: 0x000553E0
		protected override PawnBase SelectPawn(PawnBase pawn)
		{
			if (base.PlayerTurn == PlayerTurn.PlayerOne)
			{
				if (pawn.PlayerOne)
				{
					this.SelectedUnit = pawn;
				}
			}
			else if (base.AIOpponent == null && !pawn.PlayerOne)
			{
				this.SelectedUnit = pawn;
			}
			return pawn;
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x00057214 File Offset: 0x00055414
		protected override void MovePawnToTileDelayed(PawnBase pawn, TileBase tile, bool instantMove, bool displayMessage, float delay)
		{
			base.MovePawnToTileDelayed(pawn, tile, instantMove, displayMessage, delay);
			TileMuTorere tileMuTorere = tile as TileMuTorere;
			PawnMuTorere pawnMuTorere = pawn as PawnMuTorere;
			if (tileMuTorere.PawnOnTile == null && pawnMuTorere != null)
			{
				if (displayMessage)
				{
					if (base.PlayerTurn == PlayerTurn.PlayerOne)
					{
						InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_boardgame_move_piece_player", null).ToString()));
					}
					else
					{
						InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_boardgame_move_piece_opponent", null).ToString()));
					}
				}
				if (pawnMuTorere.X != -1)
				{
					base.Tiles[pawnMuTorere.X].PawnOnTile = null;
				}
				tileMuTorere.PawnOnTile = pawnMuTorere;
				pawnMuTorere.MovingToDifferentTile = pawnMuTorere.X != tileMuTorere.X;
				pawnMuTorere.X = tileMuTorere.X;
				Vec3 globalPosition = tileMuTorere.Entity.GlobalPosition;
				pawnMuTorere.AddGoalPosition(globalPosition);
				pawnMuTorere.MovePawnToGoalPositionsDelayed(instantMove, 0.6f, this.JustStoppedDraggingUnit, delay);
				if (pawnMuTorere == this.SelectedUnit)
				{
					this.SelectedUnit = null;
				}
			}
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x0005730C File Offset: 0x0005550C
		protected override void SwitchPlayerTurn()
		{
			if (base.PlayerTurn == PlayerTurn.PlayerOneWaiting)
			{
				base.PlayerTurn = PlayerTurn.PlayerTwo;
			}
			else if (base.PlayerTurn == PlayerTurn.PlayerTwoWaiting)
			{
				base.PlayerTurn = PlayerTurn.PlayerOne;
			}
			this.CheckGameEnded();
			base.SwitchPlayerTurn();
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x00057340 File Offset: 0x00055540
		protected override bool CheckGameEnded()
		{
			bool flag = false;
			List<List<Move>> list = this.CalculateAllValidMoves((base.PlayerTurn == PlayerTurn.PlayerOne) ? BoardGameSide.Player : BoardGameSide.AI);
			if (base.GetTotalMovesAvailable(ref list) <= 0)
			{
				if (base.PlayerTurn == PlayerTurn.PlayerOne)
				{
					base.OnDefeat("str_boardgame_defeat_message");
					this.ReadyToPlay = false;
					flag = true;
				}
				else if (base.PlayerTurn == PlayerTurn.PlayerTwo)
				{
					base.OnVictory("str_boardgame_victory_message");
					this.ReadyToPlay = false;
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x000573A9 File Offset: 0x000555A9
		protected override void OnAfterBoardSetUp()
		{
			this.ReadyToPlay = true;
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x000573B4 File Offset: 0x000555B4
		public TileMuTorere FindTileByCoordinate(int x)
		{
			TileMuTorere tileMuTorere = null;
			for (int i = 0; i < this.TileCount; i++)
			{
				TileMuTorere tileMuTorere2 = base.Tiles[i] as TileMuTorere;
				if (tileMuTorere2.X == x)
				{
					tileMuTorere = tileMuTorere2;
				}
			}
			return tileMuTorere;
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x000573F0 File Offset: 0x000555F0
		public BoardGameMuTorere.BoardInformation TakePawnsSnapshot()
		{
			BoardGameMuTorere.PawnInformation[] array = new BoardGameMuTorere.PawnInformation[base.PlayerOneUnits.Count + base.PlayerTwoUnits.Count];
			TileBaseInformation[] array2 = new TileBaseInformation[this.TileCount];
			int num = 0;
			foreach (PawnBase pawnBase in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits))
			{
				PawnMuTorere pawnMuTorere = (PawnMuTorere)pawnBase;
				BoardGameMuTorere.PawnInformation pawnInformation = new BoardGameMuTorere.PawnInformation(pawnMuTorere.X);
				array[num++] = pawnInformation;
			}
			foreach (PawnBase pawnBase2 in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits))
			{
				PawnMuTorere pawnMuTorere2 = (PawnMuTorere)pawnBase2;
				BoardGameMuTorere.PawnInformation pawnInformation2 = new BoardGameMuTorere.PawnInformation(pawnMuTorere2.X);
				array[num++] = pawnInformation2;
			}
			for (int i = 0; i < this.TileCount; i++)
			{
				array2[i] = new TileBaseInformation(ref base.Tiles[i].PawnOnTile);
			}
			return new BoardGameMuTorere.BoardInformation(ref array, ref array2);
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x00057540 File Offset: 0x00055740
		public void UndoMove(ref BoardGameMuTorere.BoardInformation board)
		{
			int num = 0;
			foreach (PawnBase pawnBase in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits))
			{
				((PawnMuTorere)pawnBase).X = board.PawnInformation[num++].X;
			}
			foreach (PawnBase pawnBase2 in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits))
			{
				((PawnMuTorere)pawnBase2).X = board.PawnInformation[num++].X;
			}
			for (int i = 0; i < this.TileCount; i++)
			{
				base.Tiles[i].PawnOnTile = board.TileInformation[i].PawnOnTile;
			}
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x00057654 File Offset: 0x00055854
		public void AIMakeMove(Move move)
		{
			TileMuTorere tileMuTorere = move.GoalTile as TileMuTorere;
			PawnMuTorere pawnMuTorere = move.Unit as PawnMuTorere;
			base.Tiles[pawnMuTorere.X].PawnOnTile = null;
			tileMuTorere.PawnOnTile = pawnMuTorere;
			pawnMuTorere.X = tileMuTorere.X;
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x000576A0 File Offset: 0x000558A0
		public TileBase FindAvailableTile()
		{
			foreach (TileBase tileBase in base.Tiles)
			{
				if (tileBase.PawnOnTile == null)
				{
					return tileBase;
				}
			}
			return null;
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x000576D4 File Offset: 0x000558D4
		private void PreplaceUnits()
		{
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			for (int i = 0; i < 4; i++)
			{
				this.MovePawnToTileDelayed(list[i], base.Tiles[i + 1], false, false, 0.15f * (float)(i + 1) + 0.25f);
				this.MovePawnToTileDelayed(list2[i], base.Tiles[8 - i], false, false, 0.15f * (float)(i + 1) + 0.5f);
			}
		}

		// Token: 0x040003BF RID: 959
		public const int WhitePawnCount = 4;

		// Token: 0x040003C0 RID: 960
		public const int BlackPawnCount = 4;

		// Token: 0x020001A4 RID: 420
		public struct BoardInformation
		{
			// Token: 0x06001290 RID: 4752 RVA: 0x0007499D File Offset: 0x00072B9D
			public BoardInformation(ref BoardGameMuTorere.PawnInformation[] pawns, ref TileBaseInformation[] tiles)
			{
				this.PawnInformation = pawns;
				this.TileInformation = tiles;
			}

			// Token: 0x040007E8 RID: 2024
			public readonly BoardGameMuTorere.PawnInformation[] PawnInformation;

			// Token: 0x040007E9 RID: 2025
			public readonly TileBaseInformation[] TileInformation;
		}

		// Token: 0x020001A5 RID: 421
		public struct PawnInformation
		{
			// Token: 0x06001291 RID: 4753 RVA: 0x000749AF File Offset: 0x00072BAF
			public PawnInformation(int x)
			{
				this.X = x;
			}

			// Token: 0x040007EA RID: 2026
			public readonly int X;
		}
	}
}
