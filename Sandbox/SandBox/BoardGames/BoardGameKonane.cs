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
	public class BoardGameKonane : BoardGameBase
	{
		public override int TileCount
		{
			get
			{
				return BoardGameKonane.BoardWidth * BoardGameKonane.BoardHeight;
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
				return true;
			}
		}

		protected override bool DiceRollRequired
		{
			get
			{
				return false;
			}
		}

		public BoardGameKonane(MissionBoardGameLogic mission, PlayerTurn startingPlayer)
			: base(mission, new TextObject("{=5DSafcSC}Konane", null), startingPlayer)
		{
			if (base.Tiles == null)
			{
				base.Tiles = new TileBase[this.TileCount];
			}
			this.SelectedUnit = null;
			this.PawnUnselectedFactor = 4287395960U;
		}

		public override void InitializeUnits()
		{
			base.PlayerOneUnits.Clear();
			base.PlayerTwoUnits.Clear();
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			for (int i = 0; i < 18; i++)
			{
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("player_one_unit_" + i);
				list.Add(base.InitializeUnit(new PawnKonane(gameEntity, base.PlayerWhoStarted == PlayerTurn.PlayerOne)));
			}
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			for (int j = 0; j < 18; j++)
			{
				GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("player_two_unit_" + j);
				list2.Add(base.InitializeUnit(new PawnKonane(gameEntity2, base.PlayerWhoStarted > PlayerTurn.PlayerOne)));
			}
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
			int num;
			for (x = 0; x < BoardGameKonane.BoardWidth; x = num)
			{
				int y;
				for (y = 0; y < BoardGameKonane.BoardHeight; y = num)
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
			base.InPreMovementStage = true;
			if (this._startState.PawnInformation == null)
			{
				this.PreplaceUnits();
				return;
			}
			this.RestoreStartingBoard();
		}

		public override List<Move> CalculateValidMoves(PawnBase pawn)
		{
			List<Move> list = new List<Move>();
			PawnKonane pawnKonane = pawn as PawnKonane;
			if (pawn != null)
			{
				int x = pawnKonane.X;
				int y = pawnKonane.Y;
				if (!base.InPreMovementStage && pawn.IsPlaced)
				{
					if (x > 1)
					{
						PawnBase pawnOnTile = this.GetTile(x - 1, y).PawnOnTile;
						PawnBase pawnOnTile2 = this.GetTile(x - 2, y).PawnOnTile;
						if (pawnOnTile != null && pawnOnTile2 == null && pawnOnTile.PlayerOne != pawn.PlayerOne)
						{
							Move move;
							move.Unit = pawn;
							move.GoalTile = this.GetTile(x - 2, y);
							list.Add(move);
							if (x > 3)
							{
								PawnBase pawnOnTile3 = this.GetTile(x - 3, y).PawnOnTile;
								PawnBase pawnOnTile4 = this.GetTile(x - 4, y).PawnOnTile;
								if (pawnOnTile3 != null && pawnOnTile4 == null && pawnOnTile3.PlayerOne != pawn.PlayerOne)
								{
									Move move2;
									move2.Unit = pawn;
									move2.GoalTile = this.GetTile(x - 4, y);
									list.Add(move2);
								}
							}
						}
					}
					if (x < BoardGameKonane.BoardWidth - 2)
					{
						PawnBase pawnOnTile5 = this.GetTile(x + 1, y).PawnOnTile;
						PawnBase pawnOnTile6 = this.GetTile(x + 2, y).PawnOnTile;
						if (pawnOnTile5 != null && pawnOnTile6 == null && pawnOnTile5.PlayerOne != pawn.PlayerOne)
						{
							Move move3;
							move3.Unit = pawn;
							move3.GoalTile = this.GetTile(x + 2, y);
							list.Add(move3);
							if (x < 2)
							{
								PawnBase pawnOnTile7 = this.GetTile(x + 3, y).PawnOnTile;
								PawnBase pawnOnTile8 = this.GetTile(x + 4, y).PawnOnTile;
								if (pawnOnTile7 != null && pawnOnTile8 == null && pawnOnTile7.PlayerOne != pawn.PlayerOne)
								{
									Move move4;
									move4.Unit = pawn;
									move4.GoalTile = this.GetTile(x + 4, y);
									list.Add(move4);
								}
							}
						}
					}
					if (y > 1)
					{
						PawnBase pawnOnTile9 = this.GetTile(x, y - 1).PawnOnTile;
						PawnBase pawnOnTile10 = this.GetTile(x, y - 2).PawnOnTile;
						if (pawnOnTile9 != null && pawnOnTile10 == null && pawnOnTile9.PlayerOne != pawn.PlayerOne)
						{
							Move move5;
							move5.Unit = pawn;
							move5.GoalTile = this.GetTile(x, y - 2);
							list.Add(move5);
							if (y > 3)
							{
								PawnBase pawnOnTile11 = this.GetTile(x, y - 3).PawnOnTile;
								PawnBase pawnOnTile12 = this.GetTile(x, y - 4).PawnOnTile;
								if (pawnOnTile11 != null && pawnOnTile12 == null && pawnOnTile11.PlayerOne != pawn.PlayerOne)
								{
									Move move6;
									move6.Unit = pawn;
									move6.GoalTile = this.GetTile(x, y - 4);
									list.Add(move6);
								}
							}
						}
					}
					if (y < BoardGameKonane.BoardHeight - 2)
					{
						PawnBase pawnOnTile13 = this.GetTile(x, y + 1).PawnOnTile;
						PawnBase pawnOnTile14 = this.GetTile(x, y + 2).PawnOnTile;
						if (pawnOnTile13 != null && pawnOnTile14 == null && pawnOnTile13.PlayerOne != pawn.PlayerOne)
						{
							Move move7;
							move7.Unit = pawn;
							move7.GoalTile = this.GetTile(x, y + 2);
							list.Add(move7);
							if (y < 2)
							{
								PawnBase pawnOnTile15 = this.GetTile(x, y + 3).PawnOnTile;
								PawnBase pawnOnTile16 = this.GetTile(x, y + 4).PawnOnTile;
								if (pawnOnTile15 != null && pawnOnTile16 == null && pawnOnTile15.PlayerOne != pawn.PlayerOne)
								{
									Move move8;
									move8.Unit = pawn;
									move8.GoalTile = this.GetTile(x, y + 4);
									list.Add(move8);
								}
							}
						}
					}
				}
			}
			return list;
		}

		public override void SetPawnCaptured(PawnBase pawn, bool fake = false)
		{
			base.SetPawnCaptured(pawn, fake);
			PawnKonane pawnKonane = pawn as PawnKonane;
			this.GetTile(pawnKonane.X, pawnKonane.Y).PawnOnTile = null;
			pawnKonane.PrevX = pawnKonane.X;
			pawnKonane.PrevY = pawnKonane.Y;
			pawnKonane.X = -1;
			pawnKonane.Y = -1;
			if (!fake)
			{
				base.RemovePawnFromBoard(pawnKonane, 0.6f, false);
			}
		}

		protected override PawnBase SelectPawn(PawnBase pawn)
		{
			if (base.PlayerTurn == PlayerTurn.PlayerOne)
			{
				if (pawn.PlayerOne)
				{
					if (base.InPreMovementStage)
					{
						if (!pawn.IsPlaced)
						{
							this.SelectedUnit = pawn;
						}
					}
					else
					{
						this.SelectedUnit = pawn;
					}
				}
			}
			else if (base.AIOpponent == null && !pawn.PlayerOne)
			{
				if (base.InPreMovementStage)
				{
					if (!pawn.IsPlaced)
					{
						this.SelectedUnit = pawn;
					}
				}
				else
				{
					this.SelectedUnit = pawn;
				}
			}
			return pawn;
		}

		protected override void HandlePreMovementStage(float dt)
		{
			if (base.InputManager.IsHotKeyPressed("BoardGamePawnSelect"))
			{
				PawnBase hoveredPawnIfAny = base.GetHoveredPawnIfAny();
				if (hoveredPawnIfAny != null && this.RemovablePawns.Contains(hoveredPawnIfAny))
				{
					this.SetPawnCaptured(hoveredPawnIfAny, false);
					this.UnFocusRemovablePawns();
					base.EndTurn();
					return;
				}
			}
			else
			{
				this.SelectedUnit = null;
			}
		}

		protected override void HandlePreMovementStageAI(Move move)
		{
			this.SetPawnCaptured(move.Unit, false);
			base.EndTurn();
		}

		protected override void MovePawnToTileDelayed(PawnBase pawn, TileBase tile, bool instantMove, bool displayMessage, float delay)
		{
			base.MovePawnToTileDelayed(pawn, tile, instantMove, displayMessage, delay);
			Tile2D tile2D = tile as Tile2D;
			PawnKonane pawnKonane = pawn as PawnKonane;
			if (tile2D.PawnOnTile == null && pawnKonane != null)
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
				Vec3 globalPosition = tile2D.Entity.GlobalPosition;
				float num = 0.5f;
				if (!base.InPreMovementStage)
				{
					num = 0.3f;
				}
				pawnKonane.MovingToDifferentTile = pawnKonane.X != tile2D.X || pawnKonane.Y != tile2D.Y;
				pawnKonane.PrevX = pawnKonane.X;
				pawnKonane.PrevY = pawnKonane.Y;
				pawnKonane.X = tile2D.X;
				pawnKonane.Y = tile2D.Y;
				if (pawnKonane.PrevX != -1 && pawnKonane.PrevY != -1)
				{
					this.GetTile(pawnKonane.PrevX, pawnKonane.PrevY).PawnOnTile = null;
				}
				tile.PawnOnTile = pawnKonane;
				if (instantMove || base.InPreMovementStage || this.JustStoppedDraggingUnit)
				{
					pawnKonane.AddGoalPosition(globalPosition);
					pawnKonane.MovePawnToGoalPositionsDelayed(instantMove, num, true, delay);
				}
				else
				{
					Tile2D tile2D2 = this.GetTile(pawnKonane.PrevX, pawnKonane.PrevY) as Tile2D;
					this.SetAllGoalPositions(pawnKonane, tile2D2, num);
				}
				if (instantMove && !base.InPreMovementStage)
				{
					this.CheckWhichPawnsAreCaptured(pawnKonane, false);
				}
				else if (pawnKonane == this.SelectedUnit && instantMove)
				{
					this.SelectedUnit = null;
				}
				base.ClearValidMoves();
			}
		}

		protected override void SwitchPlayerTurn()
		{
			if ((base.PlayerTurn == PlayerTurn.PlayerOneWaiting || base.PlayerTurn == PlayerTurn.PlayerTwoWaiting) && !base.InPreMovementStage && this.SelectedUnit != null)
			{
				this.CheckWhichPawnsAreCaptured(this.SelectedUnit as PawnKonane, false);
			}
			this.SelectedUnit = null;
			bool flag = false;
			if (base.InPreMovementStage)
			{
				base.InPreMovementStage = !this.CheckPlacementStageOver();
				flag = !base.InPreMovementStage;
			}
			if (!flag)
			{
				if (base.PlayerTurn == PlayerTurn.PlayerOneWaiting)
				{
					base.PlayerTurn = PlayerTurn.PlayerTwo;
				}
				else if (base.PlayerTurn == PlayerTurn.PlayerTwoWaiting)
				{
					base.PlayerTurn = PlayerTurn.PlayerOne;
				}
			}
			if (base.InPreMovementStage)
			{
				if (base.PlayerTurn == PlayerTurn.PlayerOne)
				{
					this.CheckForRemovablePawns(true);
				}
				else if (base.PlayerTurn == PlayerTurn.PlayerTwo)
				{
					this.CheckForRemovablePawns(false);
				}
			}
			else if (flag)
			{
				base.EndTurn();
			}
			else
			{
				this.CheckGameEnded();
			}
			base.SwitchPlayerTurn();
		}

		protected override bool CheckGameEnded()
		{
			bool flag = false;
			if (base.PlayerTurn == PlayerTurn.PlayerTwo)
			{
				List<List<Move>> list = this.CalculateAllValidMoves(BoardGameSide.AI);
				if (!base.HasMovesAvailable(ref list))
				{
					base.OnVictory("str_boardgame_victory_message");
					this.ReadyToPlay = false;
					flag = true;
				}
			}
			else if (base.PlayerTurn == PlayerTurn.PlayerOne)
			{
				List<List<Move>> list2 = this.CalculateAllValidMoves(BoardGameSide.Player);
				if (!base.HasMovesAvailable(ref list2))
				{
					base.OnDefeat("str_boardgame_defeat_message");
					this.ReadyToPlay = false;
					flag = true;
				}
			}
			return flag;
		}

		protected override void OnAfterBoardSetUp()
		{
			if (this._startState.PawnInformation == null)
			{
				this._startState = this.TakeBoardSnapshot();
			}
			this.ReadyToPlay = true;
			this.CheckForRemovablePawns(base.PlayerWhoStarted == PlayerTurn.PlayerOne);
		}

		public void AIMakeMove(Move move)
		{
			Tile2D tile2D = move.GoalTile as Tile2D;
			PawnKonane pawnKonane = move.Unit as PawnKonane;
			if (tile2D.PawnOnTile == null)
			{
				pawnKonane.PrevX = pawnKonane.X;
				pawnKonane.PrevY = pawnKonane.Y;
				pawnKonane.X = tile2D.X;
				pawnKonane.Y = tile2D.Y;
				this.GetTile(pawnKonane.PrevX, pawnKonane.PrevY).PawnOnTile = null;
				tile2D.PawnOnTile = pawnKonane;
				this.CheckWhichPawnsAreCaptured(pawnKonane, true);
			}
		}

		public int CheckForRemovablePawns(bool playerOne)
		{
			this.UnFocusRemovablePawns();
			int num = (playerOne ? base.GetPlayerTwoUnitsDead() : base.GetPlayerOneUnitsDead());
			if (num == 0)
			{
				using (List<PawnBase>.Enumerator enumerator = (playerOne ? base.PlayerOneUnits : base.PlayerTwoUnits).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PawnBase pawnBase = enumerator.Current;
						PawnKonane pawnKonane = (PawnKonane)pawnBase;
						if (pawnKonane.X == 0 && pawnKonane.Y == 0)
						{
							this.RemovablePawns.Add(pawnKonane);
						}
						else if (pawnKonane.X == 5 && pawnKonane.Y == 0)
						{
							this.RemovablePawns.Add(pawnKonane);
						}
						else if (pawnKonane.X == 0 && pawnKonane.Y == 5)
						{
							this.RemovablePawns.Add(pawnKonane);
						}
						else if (pawnKonane.X == 5 && pawnKonane.Y == 5)
						{
							this.RemovablePawns.Add(pawnKonane);
						}
						else if (pawnKonane.X == 2 && pawnKonane.Y == 2)
						{
							this.RemovablePawns.Add(pawnKonane);
						}
						else if (pawnKonane.X == 3 && pawnKonane.Y == 2)
						{
							this.RemovablePawns.Add(pawnKonane);
						}
						else if (pawnKonane.X == 2 && pawnKonane.Y == 3)
						{
							this.RemovablePawns.Add(pawnKonane);
						}
						else if (pawnKonane.X == 3 && pawnKonane.Y == 3)
						{
							this.RemovablePawns.Add(pawnKonane);
						}
					}
					goto IL_40C;
				}
			}
			if (num == 1)
			{
				using (List<PawnBase>.Enumerator enumerator = (playerOne ? base.PlayerTwoUnits : base.PlayerOneUnits).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PawnBase pawnBase2 = enumerator.Current;
						PawnKonane pawnKonane2 = (PawnKonane)pawnBase2;
						if (pawnKonane2.X == -1 && pawnKonane2.Y == -1)
						{
							if (pawnKonane2.PrevX == 0 && pawnKonane2.PrevY == 0)
							{
								this.RemovablePawns.Add(this.GetTile(1, 0).PawnOnTile);
								this.RemovablePawns.Add(this.GetTile(0, 1).PawnOnTile);
							}
							else if (pawnKonane2.PrevX == 5 && pawnKonane2.PrevY == 0)
							{
								this.RemovablePawns.Add(this.GetTile(4, 0).PawnOnTile);
								this.RemovablePawns.Add(this.GetTile(5, 1).PawnOnTile);
							}
							else if (pawnKonane2.PrevX == 0 && pawnKonane2.PrevY == 5)
							{
								this.RemovablePawns.Add(this.GetTile(0, 4).PawnOnTile);
								this.RemovablePawns.Add(this.GetTile(1, 5).PawnOnTile);
							}
							else if (pawnKonane2.PrevX == 5 && pawnKonane2.PrevY == 5)
							{
								this.RemovablePawns.Add(this.GetTile(5, 4).PawnOnTile);
								this.RemovablePawns.Add(this.GetTile(4, 5).PawnOnTile);
							}
							if (pawnKonane2.PrevX == 2 && pawnKonane2.PrevY == 2)
							{
								this.RemovablePawns.Add(this.GetTile(2, 3).PawnOnTile);
								this.RemovablePawns.Add(this.GetTile(3, 2).PawnOnTile);
								break;
							}
							if (pawnKonane2.PrevX == 3 && pawnKonane2.PrevY == 2)
							{
								this.RemovablePawns.Add(this.GetTile(2, 2).PawnOnTile);
								this.RemovablePawns.Add(this.GetTile(3, 3).PawnOnTile);
								break;
							}
							if (pawnKonane2.PrevX == 2 && pawnKonane2.PrevY == 3)
							{
								this.RemovablePawns.Add(this.GetTile(3, 3).PawnOnTile);
								this.RemovablePawns.Add(this.GetTile(2, 2).PawnOnTile);
								break;
							}
							if (pawnKonane2.PrevX == 3 && pawnKonane2.PrevY == 3)
							{
								this.RemovablePawns.Add(this.GetTile(2, 3).PawnOnTile);
								this.RemovablePawns.Add(this.GetTile(3, 2).PawnOnTile);
								break;
							}
							break;
						}
					}
					goto IL_40C;
				}
			}
			Debug.FailedAssert("[DEBUG]This should not be reached!", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\BoardGameKonane.cs", "CheckForRemovablePawns", 654);
			IL_40C:
			this.FocusRemovablePawns();
			return this.RemovablePawns.Count;
		}

		public BoardGameKonane.BoardInformation TakeBoardSnapshot()
		{
			BoardGameKonane.PawnInformation[] array = new BoardGameKonane.PawnInformation[base.PlayerOneUnits.Count + base.PlayerTwoUnits.Count];
			TileBaseInformation[,] array2 = new TileBaseInformation[BoardGameKonane.BoardWidth, BoardGameKonane.BoardHeight];
			int num = 0;
			foreach (PawnBase pawnBase in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits))
			{
				PawnKonane pawnKonane = (PawnKonane)pawnBase;
				array[num++] = new BoardGameKonane.PawnInformation(pawnKonane.X, pawnKonane.Y, pawnKonane.PrevX, pawnKonane.PrevY, pawnKonane.Captured, pawnKonane.Entity.GlobalPosition);
			}
			foreach (PawnBase pawnBase2 in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits))
			{
				PawnKonane pawnKonane2 = (PawnKonane)pawnBase2;
				array[num++] = new BoardGameKonane.PawnInformation(pawnKonane2.X, pawnKonane2.Y, pawnKonane2.PrevX, pawnKonane2.PrevY, pawnKonane2.Captured, pawnKonane2.Entity.GlobalPosition);
			}
			for (int i = 0; i < BoardGameKonane.BoardWidth; i++)
			{
				for (int j = 0; j < BoardGameKonane.BoardHeight; j++)
				{
					array2[i, j] = new TileBaseInformation(ref this.GetTile(i, j).PawnOnTile);
				}
			}
			return new BoardGameKonane.BoardInformation(ref array, ref array2);
		}

		public void UndoMove(ref BoardGameKonane.BoardInformation board)
		{
			int num = 0;
			foreach (PawnBase pawnBase in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits))
			{
				PawnKonane pawnKonane = (PawnKonane)pawnBase;
				pawnKonane.X = board.PawnInformation[num].X;
				pawnKonane.Y = board.PawnInformation[num].Y;
				pawnKonane.PrevX = board.PawnInformation[num].PrevX;
				pawnKonane.PrevY = board.PawnInformation[num].PrevY;
				pawnKonane.Captured = board.PawnInformation[num].IsCaptured;
				num++;
			}
			foreach (PawnBase pawnBase2 in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits))
			{
				PawnKonane pawnKonane2 = (PawnKonane)pawnBase2;
				pawnKonane2.X = board.PawnInformation[num].X;
				pawnKonane2.Y = board.PawnInformation[num].Y;
				pawnKonane2.PrevX = board.PawnInformation[num].PrevX;
				pawnKonane2.PrevY = board.PawnInformation[num].PrevY;
				pawnKonane2.Captured = board.PawnInformation[num].IsCaptured;
				num++;
			}
			for (int i = 0; i < BoardGameKonane.BoardWidth; i++)
			{
				for (int j = 0; j < BoardGameKonane.BoardHeight; j++)
				{
					this.GetTile(i, j).PawnOnTile = board.TileInformation[i, j].PawnOnTile;
				}
			}
		}

		protected void CheckWhichPawnsAreCaptured(PawnKonane pawn, bool fake = false)
		{
			int x = pawn.X;
			int y = pawn.Y;
			int prevX = pawn.PrevX;
			int prevY = pawn.PrevY;
			bool flag = false;
			if (x == -1 || y == -1 || prevX == -1 || prevY == -1)
			{
				Debug.FailedAssert("x == -1 || y == -1 || prevX == -1 || prevY == -1", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\BoardGameKonane.cs", "CheckWhichPawnsAreCaptured", 737);
			}
			Vec2i vec2i;
			vec2i..ctor(x - prevX, y - prevY);
			if (vec2i.X == 4 || vec2i.Y == 4 || vec2i.X == -4 || vec2i.Y == -4)
			{
				flag = true;
			}
			else if (vec2i.X == 2 || vec2i.Y == 2 || vec2i.X == -2 || vec2i.Y == -2)
			{
				flag = false;
			}
			else
			{
				Debug.FailedAssert("CheckWhichPawnsAreCaptured", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\BoardGameKonane.cs", "CheckWhichPawnsAreCaptured", 752);
			}
			if (!flag)
			{
				Vec2i vec2i2;
				vec2i2..ctor(vec2i.X / 2, vec2i.Y / 2);
				Vec2i vec2i3;
				vec2i3..ctor(x - vec2i2.X, y - vec2i2.Y);
				this.SetPawnCaptured(this.GetTile(vec2i3.X, vec2i3.Y).PawnOnTile, fake);
				return;
			}
			Vec2i vec2i4;
			vec2i4..ctor(vec2i.X / 4, vec2i.Y / 4);
			Vec2i vec2i5;
			vec2i5..ctor(x - vec2i4.X, y - vec2i4.Y);
			Vec2i vec2i6;
			vec2i6..ctor(x - vec2i4.X - vec2i4.X * 2, y - vec2i4.Y - vec2i4.Y * 2);
			this.SetPawnCaptured(this.GetTile(vec2i5.X, vec2i5.Y).PawnOnTile, fake);
			this.SetPawnCaptured(this.GetTile(vec2i6.X, vec2i6.Y).PawnOnTile, fake);
		}

		private void SetTile(TileBase tile, int x, int y)
		{
			base.Tiles[y * BoardGameKonane.BoardWidth + x] = tile;
		}

		private TileBase GetTile(int x, int y)
		{
			return base.Tiles[y * BoardGameKonane.BoardWidth + x];
		}

		private void FocusRemovablePawns()
		{
			foreach (PawnBase pawnBase in this.RemovablePawns)
			{
				((PawnKonane)pawnBase).Entity.GetMetaMesh(0).SetFactor1Linear(this.PawnSelectedFactor);
			}
		}

		private void UnFocusRemovablePawns()
		{
			foreach (PawnBase pawnBase in this.RemovablePawns)
			{
				((PawnKonane)pawnBase).Entity.GetMetaMesh(0).SetFactor1Linear(this.PawnUnselectedFactor);
			}
			this.RemovablePawns.Clear();
		}

		private void SetAllGoalPositions(PawnKonane pawn, Tile2D prevTile, float speed)
		{
			Vec3 globalPosition = prevTile.Entity.GlobalPosition;
			Vec3 globalPosition2 = this.GetTile(pawn.X, pawn.Y).Entity.GlobalPosition;
			bool flag = false;
			Vec2i vec2i;
			vec2i..ctor(pawn.X - prevTile.X, pawn.Y - prevTile.Y);
			if (vec2i.X == 4 || vec2i.Y == 4 || vec2i.X == -4 || vec2i.Y == -4)
			{
				flag = true;
			}
			if (!flag)
			{
				pawn.AddGoalPosition(globalPosition2);
			}
			else
			{
				Vec2i vec2i2;
				vec2i2..ctor(vec2i.X / 4, vec2i.Y / 4);
				pawn.AddGoalPosition(this.GetTile(prevTile.X + 2 * vec2i2.X, prevTile.Y + 2 * vec2i2.Y).Entity.GlobalPosition);
				pawn.AddGoalPosition(globalPosition2);
			}
			pawn.MovePawnToGoalPositions(false, speed, false);
		}

		private bool CheckPlacementStageOver()
		{
			bool flag = false;
			if (base.GetPlayerOneUnitsDead() + base.GetPlayerTwoUnitsDead() == 2)
			{
				flag = true;
			}
			return flag;
		}

		private void PreplaceUnits()
		{
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			for (int i = 0; i < 18; i++)
			{
				int num = i % 3 * 2;
				int num2 = i / 3;
				float num3 = 0.15f * (float)(i + 1) + 0.25f;
				if (num2 % 2 == 0)
				{
					this.MovePawnToTileDelayed(list[i], this.GetTile(num, num2), false, false, num3);
					this.MovePawnToTileDelayed(list2[i], this.GetTile(num + 1, num2), false, false, num3);
				}
				else
				{
					this.MovePawnToTileDelayed(list[i], this.GetTile(num + 1, num2), false, false, num3);
					this.MovePawnToTileDelayed(list2[i], this.GetTile(num, num2), false, false, num3);
				}
			}
		}

		private void RestoreStartingBoard()
		{
			int num = 0;
			foreach (PawnBase pawnBase in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits))
			{
				PawnKonane pawnKonane = (PawnKonane)pawnBase;
				if (this._startState.PawnInformation[num].X != -1)
				{
					if (this._startState.PawnInformation[num].X != pawnKonane.X && this._startState.PawnInformation[num].Y != pawnKonane.Y)
					{
						pawnKonane.Reset();
						TileBase tile = this.GetTile(this._startState.PawnInformation[num].X, this._startState.PawnInformation[num].Y);
						this.MovePawnToTile(pawnKonane, tile, false, true);
					}
				}
				else if (!pawnKonane.Entity.GlobalPosition.NearlyEquals(this._startState.PawnInformation[num].Position, 1E-05f))
				{
					if (pawnKonane.X != -1 && this.GetTile(pawnKonane.X, pawnKonane.Y).PawnOnTile == pawnKonane)
					{
						this.GetTile(pawnKonane.X, pawnKonane.Y).PawnOnTile = null;
					}
					pawnKonane.Reset();
					pawnKonane.AddGoalPosition(this._startState.PawnInformation[num].Position);
					pawnKonane.MovePawnToGoalPositions(false, 0.5f, false);
				}
				num++;
			}
			foreach (PawnBase pawnBase2 in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits))
			{
				PawnKonane pawnKonane2 = (PawnKonane)pawnBase2;
				if (this._startState.PawnInformation[num].X != -1)
				{
					if (this._startState.PawnInformation[num].X != pawnKonane2.X && this._startState.PawnInformation[num].Y != pawnKonane2.Y)
					{
						TileBase tile2 = this.GetTile(this._startState.PawnInformation[num].X, this._startState.PawnInformation[num].Y);
						this.MovePawnToTile(pawnKonane2, tile2, false, true);
					}
				}
				else
				{
					if (pawnKonane2.X != -1 && this.GetTile(pawnKonane2.X, pawnKonane2.Y).PawnOnTile == pawnKonane2)
					{
						this.GetTile(pawnKonane2.X, pawnKonane2.Y).PawnOnTile = null;
					}
					pawnKonane2.Reset();
					pawnKonane2.AddGoalPosition(this._startState.PawnInformation[num].Position);
					pawnKonane2.MovePawnToGoalPositions(false, 0.5f, false);
				}
				num++;
			}
		}

		public const int WhitePawnCount = 18;

		public const int BlackPawnCount = 18;

		public static readonly int BoardWidth = 6;

		public static readonly int BoardHeight = 6;

		public List<PawnBase> RemovablePawns = new List<PawnBase>();

		private BoardGameKonane.BoardInformation _startState;

		public struct BoardInformation
		{
			public BoardInformation(ref BoardGameKonane.PawnInformation[] pawns, ref TileBaseInformation[,] tiles)
			{
				this.PawnInformation = pawns;
				this.TileInformation = tiles;
			}

			public readonly BoardGameKonane.PawnInformation[] PawnInformation;

			public readonly TileBaseInformation[,] TileInformation;
		}

		public struct PawnInformation
		{
			public PawnInformation(int x, int y, int prevX, int prevY, bool captured, Vec3 position)
			{
				this.X = x;
				this.Y = y;
				this.PrevX = prevX;
				this.PrevY = prevY;
				this.IsCaptured = captured;
				this.Position = position;
			}

			public readonly int X;

			public readonly int Y;

			public readonly int PrevX;

			public readonly int PrevY;

			public readonly bool IsCaptured;

			public readonly Vec3 Position;
		}
	}
}
