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
	public class BoardGameTablut : BoardGameBase
	{
		public override int TileCount
		{
			get
			{
				return 81;
			}
		}

		protected override bool RotateBoard
		{
			get
			{
				return false;
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

		private PawnTablut King { get; set; }

		public BoardGameTablut(MissionBoardGameLogic mission, PlayerTurn startingPlayer)
			: base(mission, new TextObject("{=qeKskdiY}Tablut", null), startingPlayer)
		{
			this.SelectedUnit = null;
			this.PawnUnselectedFactor = 4287395960U;
		}

		public static bool IsCitadelTile(int tileX, int tileY)
		{
			return tileX == 4 && tileY == 4;
		}

		public override void InitializeUnits()
		{
			base.PlayerOneUnits.Clear();
			base.PlayerTwoUnits.Clear();
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			for (int i = 0; i < 16; i++)
			{
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("player_one_unit_" + i);
				list.Add(base.InitializeUnit(new PawnTablut(gameEntity, base.PlayerWhoStarted == PlayerTurn.PlayerOne)));
			}
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			for (int j = 0; j < 9; j++)
			{
				GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("player_two_unit_" + j);
				list2.Add(base.InitializeUnit(new PawnTablut(gameEntity2, base.PlayerWhoStarted > PlayerTurn.PlayerOne)));
			}
			this.King = list2[0] as PawnTablut;
		}

		public override void InitializeTiles()
		{
			int x;
			IEnumerable<GameEntity> enumerable = from x in this.BoardEntity.GetChildren()
				where x.Tags.Any((string t) => t.Contains("tile_"))
				select x;
			IEnumerable<GameEntity> enumerable2 = from x in this.BoardEntity.GetChildren()
				where x.Tags.Any((string t) => t.Contains("decal_"))
				select x;
			if (base.Tiles == null)
			{
				base.Tiles = new TileBase[this.TileCount];
			}
			int num;
			for (x = 0; x < 9; x = num)
			{
				int y;
				for (y = 0; y < 9; y = num)
				{
					GameEntity gameEntity = enumerable.Single((GameEntity e) => e.HasTag(string.Concat(new object[] { "tile_", x, "_", y })));
					BoardGameDecal firstScriptOfType = enumerable2.Single((GameEntity e) => e.HasTag(string.Concat(new object[] { "decal_", x, "_", y }))).GetFirstScriptOfType<BoardGameDecal>();
					Tile2D tile2D = new Tile2D(gameEntity, firstScriptOfType, x, y);
					this.SetTile(tile2D, x, y);
					num = y + 1;
				}
				num = x + 1;
			}
		}

		public override void InitializeSound()
		{
			PawnBase.PawnMoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/move_stone");
			PawnBase.PawnSelectSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/pick_stone");
			PawnBase.PawnTapSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/drop_stone");
			PawnBase.PawnRemoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/out_stone");
		}

		public override void Reset()
		{
			base.Reset();
			if (this._startState.PawnInformation == null)
			{
				this.PreplaceUnits();
				return;
			}
			this.RestoreStartingBoard();
		}

		public override List<Move> CalculateValidMoves(PawnBase pawn)
		{
			List<Move> list = new List<Move>(16);
			if (pawn.IsPlaced && !pawn.Captured)
			{
				PawnTablut pawnTablut = pawn as PawnTablut;
				int i = pawnTablut.X;
				int j = pawnTablut.Y;
				while (i > 0)
				{
					i--;
					if (!this.AddValidMove(list, pawn, i, j))
					{
						break;
					}
				}
				i = pawnTablut.X;
				while (i < 8)
				{
					i++;
					if (!this.AddValidMove(list, pawn, i, j))
					{
						break;
					}
				}
				i = pawnTablut.X;
				while (j < 8)
				{
					j++;
					if (!this.AddValidMove(list, pawn, i, j))
					{
						break;
					}
				}
				j = pawnTablut.Y;
				while (j > 0)
				{
					j--;
					if (!this.AddValidMove(list, pawn, i, j))
					{
						break;
					}
				}
				j = pawnTablut.Y;
			}
			return list;
		}

		public override void SetPawnCaptured(PawnBase pawn, bool fake = false)
		{
			base.SetPawnCaptured(pawn, fake);
			PawnTablut pawnTablut = pawn as PawnTablut;
			this.GetTile(pawnTablut.X, pawnTablut.Y).PawnOnTile = null;
			pawnTablut.X = -1;
			pawnTablut.Y = -1;
			if (!fake)
			{
				base.RemovePawnFromBoard(pawnTablut, 0.6f, false);
			}
		}

		protected override void OnAfterBoardSetUp()
		{
			if (this._startState.PawnInformation == null)
			{
				this._startState = this.TakeBoardSnapshot();
			}
			this.ReadyToPlay = true;
		}

		protected override PawnBase SelectPawn(PawnBase pawn)
		{
			if (pawn.PlayerOne == (base.PlayerTurn == PlayerTurn.PlayerOne))
			{
				this.SelectedUnit = pawn;
			}
			return pawn;
		}

		protected override void MovePawnToTileDelayed(PawnBase pawn, TileBase tile, bool instantMove, bool displayMessage, float delay)
		{
			base.MovePawnToTileDelayed(pawn, tile, instantMove, displayMessage, delay);
			Tile2D tile2D = tile as Tile2D;
			PawnTablut pawnTablut = pawn as PawnTablut;
			if (tile2D.PawnOnTile == null)
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
				Vec3 globalPosition = pawnTablut.Entity.GlobalPosition;
				Vec3 globalPosition2 = tile2D.Entity.GlobalPosition;
				if (pawnTablut.X != -1 && pawnTablut.Y != -1)
				{
					this.GetTile(pawnTablut.X, pawnTablut.Y).PawnOnTile = null;
				}
				pawnTablut.MovingToDifferentTile = pawnTablut.X != tile2D.X || pawnTablut.Y != tile2D.Y;
				pawnTablut.X = tile2D.X;
				pawnTablut.Y = tile2D.Y;
				tile2D.PawnOnTile = pawnTablut;
				if (this.SettingUpBoard && globalPosition2.z > globalPosition.z)
				{
					Vec3 vec = globalPosition;
					vec.z += 2f * (globalPosition2.z - globalPosition.z);
					pawnTablut.AddGoalPosition(vec);
					pawnTablut.MovePawnToGoalPositionsDelayed(instantMove, 0.5f, this.JustStoppedDraggingUnit, delay);
				}
				pawnTablut.AddGoalPosition(globalPosition2);
				pawnTablut.MovePawnToGoalPositionsDelayed(instantMove, 0.5f, this.JustStoppedDraggingUnit, delay);
				if (instantMove)
				{
					this.CheckIfPawnCaptures(this.SelectedUnit as PawnTablut, false);
				}
				if (pawnTablut == this.SelectedUnit && instantMove)
				{
					this.SelectedUnit = null;
				}
			}
		}

		protected override void SwitchPlayerTurn()
		{
			if ((base.PlayerTurn == PlayerTurn.PlayerOneWaiting || base.PlayerTurn == PlayerTurn.PlayerTwoWaiting) && this.SelectedUnit != null)
			{
				this.CheckIfPawnCaptures(this.SelectedUnit as PawnTablut, false);
			}
			this.SelectedUnit = null;
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
			BoardGameTablut.State state = this.CheckGameState();
			bool flag = true;
			switch (state)
			{
			case BoardGameTablut.State.InProgress:
				flag = false;
				break;
			case BoardGameTablut.State.PlayerWon:
				base.OnVictory("str_boardgame_victory_message");
				this.ReadyToPlay = false;
				break;
			case BoardGameTablut.State.AIWon:
				base.OnDefeat("str_boardgame_defeat_message");
				this.ReadyToPlay = false;
				break;
			}
			return flag;
		}

		public bool AIMakeMove(Move move)
		{
			Tile2D tile2D = move.GoalTile as Tile2D;
			PawnTablut pawnTablut = move.Unit as PawnTablut;
			if (tile2D.PawnOnTile == null)
			{
				if (pawnTablut.X != -1 && pawnTablut.Y != -1)
				{
					this.GetTile(pawnTablut.X, pawnTablut.Y).PawnOnTile = null;
				}
				pawnTablut.X = tile2D.X;
				pawnTablut.Y = tile2D.Y;
				tile2D.PawnOnTile = pawnTablut;
				this.CheckIfPawnCaptures(pawnTablut, true);
				return true;
			}
			return false;
		}

		public bool HasAvailableMoves(PawnTablut pawn)
		{
			bool flag = false;
			if (pawn.IsPlaced && !pawn.Captured)
			{
				int x = pawn.X;
				int y = pawn.Y;
				flag = (x > 0 && this.GetTile(x - 1, y).PawnOnTile == null && !BoardGameTablut.IsCitadelTile(x - 1, y)) || (x < 8 && this.GetTile(x + 1, y).PawnOnTile == null && !BoardGameTablut.IsCitadelTile(x + 1, y)) || (y > 0 && this.GetTile(x, y - 1).PawnOnTile == null && !BoardGameTablut.IsCitadelTile(x, y - 1)) || (y < 8 && this.GetTile(x, y + 1).PawnOnTile == null && !BoardGameTablut.IsCitadelTile(x, y + 1));
			}
			return flag;
		}

		public Move GetRandomAvailableMove(PawnTablut pawn)
		{
			List<Move> list = this.CalculateValidMoves(pawn);
			return list[MBRandom.RandomInt(list.Count)];
		}

		public BoardGameTablut.BoardInformation TakeBoardSnapshot()
		{
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			BoardGameTablut.PawnInformation[] array = new BoardGameTablut.PawnInformation[25];
			for (int i = 0; i < 25; i++)
			{
				PawnTablut pawnTablut;
				if (i < 16)
				{
					pawnTablut = list[i] as PawnTablut;
				}
				else
				{
					pawnTablut = list2[i - 16] as PawnTablut;
				}
				BoardGameTablut.PawnInformation pawnInformation;
				pawnInformation.X = pawnTablut.X;
				pawnInformation.Y = pawnTablut.Y;
				pawnInformation.IsCaptured = pawnTablut.Captured;
				array[i] = pawnInformation;
			}
			PlayerTurn playerTurn = base.PlayerTurn;
			return new BoardGameTablut.BoardInformation(ref array, playerTurn);
		}

		public void UndoMove(ref BoardGameTablut.BoardInformation board)
		{
			for (int i = 0; i < this.TileCount; i++)
			{
				base.Tiles[i].PawnOnTile = null;
			}
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			for (int j = 0; j < 25; j++)
			{
				BoardGameTablut.PawnInformation pawnInformation = board.PawnInformation[j];
				PawnTablut pawnTablut;
				if (j < 16)
				{
					pawnTablut = list[j] as PawnTablut;
				}
				else
				{
					pawnTablut = list2[j - 16] as PawnTablut;
				}
				pawnTablut.X = pawnInformation.X;
				pawnTablut.Y = pawnInformation.Y;
				pawnTablut.Captured = pawnInformation.IsCaptured;
				if (pawnTablut.IsPlaced)
				{
					this.GetTile(pawnTablut.X, pawnTablut.Y).PawnOnTile = pawnTablut;
				}
			}
			base.PlayerTurn = board.Turn;
		}

		public BoardGameTablut.State CheckGameState()
		{
			BoardGameTablut.State state;
			if (!base.AIOpponent.AbortRequested)
			{
				state = BoardGameTablut.State.InProgress;
				if (base.PlayerTurn == PlayerTurn.PlayerOne || base.PlayerTurn == PlayerTurn.PlayerTwo)
				{
					bool flag = base.PlayerWhoStarted == PlayerTurn.PlayerOne;
					if (this.King.Captured)
					{
						state = (flag ? BoardGameTablut.State.PlayerWon : BoardGameTablut.State.AIWon);
					}
					else if (this.King.X == 0 || this.King.X == 8 || this.King.Y == 0 || this.King.Y == 8)
					{
						state = (flag ? BoardGameTablut.State.AIWon : BoardGameTablut.State.PlayerWon);
					}
					else
					{
						bool flag2 = false;
						bool flag3 = base.PlayerTurn == PlayerTurn.PlayerOne;
						List<PawnBase> list = (flag3 ? base.PlayerOneUnits : base.PlayerTwoUnits);
						int count = list.Count;
						for (int i = 0; i < count; i++)
						{
							PawnBase pawnBase = list[i];
							if (pawnBase.IsPlaced && !pawnBase.Captured && this.HasAvailableMoves(pawnBase as PawnTablut))
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							state = (flag3 ? BoardGameTablut.State.AIWon : BoardGameTablut.State.PlayerWon);
						}
					}
				}
			}
			else
			{
				state = BoardGameTablut.State.Aborted;
			}
			return state;
		}

		private void SetTile(TileBase tile, int x, int y)
		{
			base.Tiles[y * 9 + x] = tile;
		}

		private TileBase GetTile(int x, int y)
		{
			return base.Tiles[y * 9 + x];
		}

		private void PreplaceUnits()
		{
			int[] array = new int[]
			{
				3, 0, 4, 0, 5, 0, 4, 1, 0, 3,
				0, 4, 0, 5, 1, 4, 8, 3, 8, 4,
				8, 5, 7, 4, 3, 8, 4, 8, 5, 8,
				4, 7
			};
			int[] array2 = new int[]
			{
				4, 4, 4, 3, 4, 2, 5, 4, 6, 4,
				3, 4, 2, 4, 4, 5, 4, 6
			};
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				int num = array[i * 2];
				int num2 = array[i * 2 + 1];
				this.MovePawnToTileDelayed(list[i], this.GetTile(num, num2), false, false, 0.15f * (float)(i + 1) + 0.25f);
			}
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			int count2 = list2.Count;
			for (int j = 0; j < count2; j++)
			{
				int num3 = array2[j * 2];
				int num4 = array2[j * 2 + 1];
				this.MovePawnToTileDelayed(list2[j], this.GetTile(num3, num4), false, false, 0.15f * (float)(j + 1) + 0.25f);
			}
		}

		private void RestoreStartingBoard()
		{
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			for (int i = 0; i < 25; i++)
			{
				PawnBase pawnBase;
				if (i < 16)
				{
					pawnBase = list[i];
				}
				else
				{
					pawnBase = list2[i - 16];
				}
				BoardGameTablut.PawnInformation pawnInformation = this._startState.PawnInformation[i];
				TileBase tile = this.GetTile(pawnInformation.X, pawnInformation.Y);
				pawnBase.Reset();
				this.MovePawnToTile(pawnBase, tile, false, false);
			}
		}

		private bool AddValidMove(List<Move> moves, PawnBase pawn, int x, int y)
		{
			bool flag = false;
			TileBase tile = this.GetTile(x, y);
			if (tile.PawnOnTile == null && !BoardGameTablut.IsCitadelTile(x, y))
			{
				Move move;
				move.Unit = pawn;
				move.GoalTile = tile;
				moves.Add(move);
				flag = true;
			}
			return flag;
		}

		private void CheckIfPawnCapturedEnemyPawn(PawnTablut pawn, bool fake, TileBase victimTile, Tile2D helperTile)
		{
			PawnBase pawnOnTile = victimTile.PawnOnTile;
			if (pawnOnTile != null && pawnOnTile.PlayerOne != pawn.PlayerOne)
			{
				PawnBase pawnOnTile2 = helperTile.PawnOnTile;
				if (pawnOnTile2 != null)
				{
					if (pawnOnTile2.PlayerOne == pawn.PlayerOne)
					{
						this.SetPawnCaptured(pawnOnTile, fake);
						return;
					}
				}
				else if (BoardGameTablut.IsCitadelTile(helperTile.X, helperTile.Y))
				{
					this.SetPawnCaptured(pawnOnTile, fake);
				}
			}
		}

		private void CheckIfPawnCaptures(PawnTablut pawn, bool fake = false)
		{
			int x = pawn.X;
			int y = pawn.Y;
			if (x > 1)
			{
				this.CheckIfPawnCapturedEnemyPawn(pawn, fake, this.GetTile(x - 1, y), this.GetTile(x - 2, y) as Tile2D);
			}
			if (x < 7)
			{
				this.CheckIfPawnCapturedEnemyPawn(pawn, fake, this.GetTile(x + 1, y), this.GetTile(x + 2, y) as Tile2D);
			}
			if (y > 1)
			{
				this.CheckIfPawnCapturedEnemyPawn(pawn, fake, this.GetTile(x, y - 1), this.GetTile(x, y - 2) as Tile2D);
			}
			if (y < 7)
			{
				this.CheckIfPawnCapturedEnemyPawn(pawn, fake, this.GetTile(x, y + 1), this.GetTile(x, y + 2) as Tile2D);
			}
		}

		public const int BoardWidth = 9;

		public const int BoardHeight = 9;

		public const int AttackerPawnCount = 16;

		public const int DefenderPawnCount = 9;

		private BoardGameTablut.BoardInformation _startState;

		public struct PawnInformation
		{
			public PawnInformation(int x, int y, bool captured)
			{
				this.X = x;
				this.Y = y;
				this.IsCaptured = captured;
			}

			public int X;

			public int Y;

			public bool IsCaptured;
		}

		public struct BoardInformation
		{
			public BoardInformation(ref BoardGameTablut.PawnInformation[] pawns, PlayerTurn turn)
			{
				this.PawnInformation = pawns;
				this.Turn = turn;
			}

			public readonly BoardGameTablut.PawnInformation[] PawnInformation;

			public readonly PlayerTurn Turn;
		}

		public enum State
		{
			InProgress,
			Aborted,
			PlayerWon,
			AIWon
		}
	}
}
