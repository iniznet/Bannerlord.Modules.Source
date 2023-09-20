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
	public class BoardGameMuTorere : BoardGameBase
	{
		public override int TileCount
		{
			get
			{
				return 9;
			}
		}

		protected override bool RotateBoard
		{
			get
			{
				return true;
			}
		}

		protected override bool PreMovementStagePresent
		{
			get
			{
				return false;
			}
		}

		protected override bool DiceRollRequired
		{
			get
			{
				return false;
			}
		}

		public BoardGameMuTorere(MissionBoardGameLogic mission, PlayerTurn startingPlayer)
			: base(mission, new TextObject("{=5siAbi69}Mu Torere", null), startingPlayer)
		{
			this.PawnUnselectedFactor = 4288711820U;
		}

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

		public override void InitializeCapturedUnitsZones()
		{
		}

		public override void InitializeSound()
		{
			PawnBase.PawnMoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/move_stone");
			PawnBase.PawnSelectSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/pick_stone");
			PawnBase.PawnTapSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/drop_wood");
			PawnBase.PawnRemoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/out_stone");
		}

		public override void Reset()
		{
			base.Reset();
			this.PreplaceUnits();
		}

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

		protected override void OnAfterBoardSetUp()
		{
			this.ReadyToPlay = true;
		}

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

		public void AIMakeMove(Move move)
		{
			TileMuTorere tileMuTorere = move.GoalTile as TileMuTorere;
			PawnMuTorere pawnMuTorere = move.Unit as PawnMuTorere;
			base.Tiles[pawnMuTorere.X].PawnOnTile = null;
			tileMuTorere.PawnOnTile = pawnMuTorere;
			pawnMuTorere.X = tileMuTorere.X;
		}

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

		public const int WhitePawnCount = 4;

		public const int BlackPawnCount = 4;

		public struct BoardInformation
		{
			public BoardInformation(ref BoardGameMuTorere.PawnInformation[] pawns, ref TileBaseInformation[] tiles)
			{
				this.PawnInformation = pawns;
				this.TileInformation = tiles;
			}

			public readonly BoardGameMuTorere.PawnInformation[] PawnInformation;

			public readonly TileBaseInformation[] TileInformation;
		}

		public struct PawnInformation
		{
			public PawnInformation(int x)
			{
				this.X = x;
			}

			public readonly int X;
		}
	}
}
