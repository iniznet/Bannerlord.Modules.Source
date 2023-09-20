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
	public class BoardGamePuluc : BoardGameBase
	{
		public override int TileCount
		{
			get
			{
				return 13;
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
				return true;
			}
		}

		public BoardGamePuluc(MissionBoardGameLogic mission, PlayerTurn startingPlayer)
			: base(mission, new TextObject("{=Uh057UUb}Puluc", null), startingPlayer)
		{
			base.LastDice = -1;
			this.PawnUnselectedFactor = 4287395960U;
		}

		public override void InitializeUnits()
		{
			base.PlayerOneUnits.Clear();
			base.PlayerTwoUnits.Clear();
			List<PawnBase> list = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits);
			for (int i = 0; i < 6; i++)
			{
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("player_one_unit_" + i);
				list.Add(base.InitializeUnit(new PawnPuluc(gameEntity, base.PlayerWhoStarted == PlayerTurn.PlayerOne)));
			}
			List<PawnBase> list2 = ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits);
			for (int j = 0; j < 6; j++)
			{
				GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("player_two_unit_" + j);
				list2.Add(base.InitializeUnit(new PawnPuluc(gameEntity2, base.PlayerWhoStarted > PlayerTurn.PlayerOne)));
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
			for (x = 0; x < 11; x = num)
			{
				GameEntity gameEntity = enumerable.Single((GameEntity e) => e.HasTag("tile_" + x));
				BoardGameDecal firstScriptOfType = enumerable2.Single((GameEntity e) => e.HasTag("decal_" + x)).GetFirstScriptOfType<BoardGameDecal>();
				base.Tiles[x] = new TilePuluc(gameEntity, firstScriptOfType, x);
				num = x + 1;
			}
			GameEntity firstChildEntityWithTag = MBExtensions.GetFirstChildEntityWithTag(this.BoardEntity, "tile_homebase_player");
			BoardGameDecal firstScriptOfType2 = MBExtensions.GetFirstChildEntityWithTag(this.BoardEntity, "decal_homebase_player").GetFirstScriptOfType<BoardGameDecal>();
			base.Tiles[11] = new TilePuluc(firstChildEntityWithTag, firstScriptOfType2, 11);
			GameEntity firstChildEntityWithTag2 = MBExtensions.GetFirstChildEntityWithTag(this.BoardEntity, "tile_homebase_opponent");
			BoardGameDecal firstScriptOfType3 = MBExtensions.GetFirstChildEntityWithTag(this.BoardEntity, "decal_homebase_opponent").GetFirstScriptOfType<BoardGameDecal>();
			base.Tiles[12] = new TilePuluc(firstChildEntityWithTag2, firstScriptOfType3, 12);
		}

		public override void InitializeSound()
		{
			PawnBase.PawnMoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/move_stone");
			PawnBase.PawnSelectSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/pick_stone");
			PawnBase.PawnTapSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/drop_wood");
			PawnBase.PawnRemoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/out_stone");
			this.DiceRollSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/out_stone");
		}

		public override void InitializeDiceBoard()
		{
			this.DiceBoard = Mission.Current.Scene.FindEntityWithTag("dice_board");
			this.DiceBoard.GetFirstScriptOfType<VertexAnimator>().Pause();
		}

		public override void Reset()
		{
			base.Reset();
			base.LastDice = -1;
			this.SetPawnSides();
			if (this._startState.PawnInformation == null)
			{
				this._startState = this.TakeBoardSnapshot();
			}
			this.RestoreStartingBoard();
		}

		public override List<Move> CalculateValidMoves(PawnBase pawn)
		{
			List<Move> list = null;
			PawnPuluc pawnPuluc = pawn as PawnPuluc;
			if (pawnPuluc != null && pawnPuluc.IsTopPawn)
			{
				list = new List<Move>();
				int num = ((pawnPuluc.IsInSpawn && !pawnPuluc.PlayerOne) ? 11 : pawnPuluc.X);
				bool flag = pawnPuluc.State == PawnPuluc.MovementState.MovingBackward;
				int num2 = ((pawnPuluc.PlayerOne ^ flag) ? (num + base.LastDice) : (num - base.LastDice));
				if (num2 < 0)
				{
					if (flag)
					{
						num2 = 11;
					}
					else
					{
						pawnPuluc.State = PawnPuluc.MovementState.ChangingDirection;
						num2 = -num2;
					}
				}
				else if (num2 > 10)
				{
					if (flag)
					{
						num2 = 12;
					}
					else
					{
						pawnPuluc.State = PawnPuluc.MovementState.ChangingDirection;
						num2 = 20 - num2;
					}
				}
				if (this.CanMovePawnToTile(pawnPuluc, num2))
				{
					Move move;
					move.Unit = pawnPuluc;
					move.GoalTile = base.Tiles[num2];
					list.Add(move);
				}
			}
			return list;
		}

		public override void RollDice()
		{
			base.PlayDiceRollSound();
			int num = MBRandom.RandomInt(2) + MBRandom.RandomInt(2) + MBRandom.RandomInt(2) + MBRandom.RandomInt(2);
			if (num == 0)
			{
				num = 5;
			}
			VertexAnimator firstScriptOfType = this.DiceBoard.GetFirstScriptOfType<VertexAnimator>();
			switch (num)
			{
			case 1:
				firstScriptOfType.SetAnimation(1, 125, 70f);
				break;
			case 2:
				firstScriptOfType.SetAnimation(129, 248, 70f);
				break;
			case 3:
				firstScriptOfType.SetAnimation(251, 373, 70f);
				break;
			case 4:
				firstScriptOfType.SetAnimation(379, 496, 70f);
				break;
			case 5:
				firstScriptOfType.SetAnimation(501, 626, 70f);
				break;
			}
			firstScriptOfType.PlayOnce();
			base.LastDice = num;
			this.DiceRollAnimationTimer = 0f;
			this.DiceRollAnimationRunning = true;
		}

		protected override void OnAfterBoardSetUp()
		{
			this.ReadyToPlay = true;
		}

		protected override PawnBase SelectPawn(PawnBase pawn)
		{
			PawnPuluc pawnPuluc = pawn as PawnPuluc;
			if (pawnPuluc.CapturedBy != null)
			{
				pawn = pawnPuluc.CapturedBy;
			}
			if (pawn.PlayerOne == (base.PlayerTurn == PlayerTurn.PlayerOne) && !pawn.Captured)
			{
				this.SelectedUnit = pawn;
			}
			return pawn;
		}

		protected override void SwitchPlayerTurn()
		{
			if (this.SelectedUnit != null)
			{
				PawnPuluc pawnPuluc = this.SelectedUnit as PawnPuluc;
				if (pawnPuluc.InPlay && (base.PlayerTurn == PlayerTurn.PlayerOneWaiting || base.PlayerTurn == PlayerTurn.PlayerTwoWaiting))
				{
					List<PawnPuluc> list = this.CheckIfPawnWillCapture(pawnPuluc, pawnPuluc.X);
					if (list != null && list.Count > 0)
					{
						pawnPuluc.State = PawnPuluc.MovementState.MovingBackward;
						pawnPuluc.PawnsBelow.AddRange(list);
						foreach (PawnPuluc pawnPuluc2 in list)
						{
							pawnPuluc2.IsTopPawn = false;
							pawnPuluc2.Captured = true;
							pawnPuluc2.CapturedBy = pawnPuluc;
						}
						TilePuluc tilePuluc = base.Tiles[pawnPuluc.X] as TilePuluc;
						Vec3 vec = (pawnPuluc.PlayerOne ? tilePuluc.PosRightMid : tilePuluc.PosLeftMid);
						pawnPuluc.AddGoalPosition(vec);
						pawnPuluc.MovePawnToGoalPositions(false, 0.5f, false);
					}
				}
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
			base.LastDice = -1;
			this.CheckGameEnded();
			base.SwitchPlayerTurn();
		}

		protected override bool CheckGameEnded()
		{
			bool flag = false;
			if (base.GetPlayerOneUnitsAlive() <= 0)
			{
				base.OnDefeat("str_boardgame_defeat_message");
				this.ReadyToPlay = false;
				flag = true;
			}
			if (base.GetPlayerTwoUnitsAlive() <= 0)
			{
				base.OnVictory("str_boardgame_victory_message");
				this.ReadyToPlay = false;
				flag = true;
			}
			return flag;
		}

		protected override void UpdateAllTilesPositions()
		{
			TileBase[] tiles = base.Tiles;
			for (int i = 0; i < tiles.Length; i++)
			{
				((TilePuluc)tiles[i]).UpdateTilePosition();
			}
		}

		protected override void OnBeforeEndTurn()
		{
			base.LastDice = -1;
		}

		protected override void MovePawnToTile(PawnBase pawn, TileBase tile, bool instantMove = false, bool displayMessage = true)
		{
			base.MovePawnToTile(pawn, tile, instantMove, displayMessage);
			TilePuluc tilePuluc = tile as TilePuluc;
			PawnPuluc pawnPuluc = pawn as PawnPuluc;
			if (tilePuluc.PawnOnTile == null)
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
				int x = pawnPuluc.X;
				pawnPuluc.MovingToDifferentTile = x != tilePuluc.X || pawnPuluc.State == PawnPuluc.MovementState.ChangingDirection;
				pawnPuluc.X = tilePuluc.X;
				foreach (PawnPuluc pawnPuluc2 in pawnPuluc.PawnsBelow)
				{
					pawnPuluc2.X = pawnPuluc.X;
				}
				if (pawnPuluc.X == 12 || pawnPuluc.X == 11)
				{
					this.PawnHasReachedHomeBase(pawnPuluc, instantMove, false);
					return;
				}
				if (pawnPuluc.State == PawnPuluc.MovementState.ChangingDirection)
				{
					int num;
					Vec3 vec;
					Vec3 vec2;
					if (pawn.PlayerOne)
					{
						num = 10;
						TilePuluc tilePuluc2 = base.Tiles[num] as TilePuluc;
						vec = tilePuluc2.PosRight;
						vec2 = tilePuluc2.PosRightMid;
					}
					else
					{
						num = 0;
						TilePuluc tilePuluc3 = base.Tiles[num] as TilePuluc;
						vec = tilePuluc3.PosLeft;
						vec2 = tilePuluc3.PosLeftMid;
					}
					if (x != num)
					{
						pawn.AddGoalPosition(vec);
					}
					pawn.AddGoalPosition(vec2);
					pawnPuluc.State = PawnPuluc.MovementState.MovingBackward;
				}
				Vec3 vec3;
				if (pawnPuluc.State == PawnPuluc.MovementState.MovingForward)
				{
					vec3 = (pawn.PlayerOne ? tilePuluc.PosRight : tilePuluc.PosLeft);
				}
				else
				{
					vec3 = (pawn.PlayerOne ? tilePuluc.PosRightMid : tilePuluc.PosLeftMid);
				}
				pawn.AddGoalPosition(vec3);
				pawn.MovePawnToGoalPositions(false, 0.5f, this.JustStoppedDraggingUnit);
			}
		}

		protected override void OnAfterDiceRollAnimation()
		{
			base.OnAfterDiceRollAnimation();
			if (base.LastDice != -1)
			{
				MBTextManager.SetTextVariable("DICE_ROLL", base.LastDice);
				if (base.PlayerTurn == PlayerTurn.PlayerOne)
				{
					InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_boardgame_roll_dice_player", null).ToString()));
				}
				else
				{
					InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_boardgame_roll_dice_opponent", null).ToString()));
				}
				if (base.PlayerTurn == PlayerTurn.PlayerOne)
				{
					List<List<Move>> list = this.CalculateAllValidMoves(BoardGameSide.Player);
					if (!base.HasMovesAvailable(ref list))
					{
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_boardgame_no_available_moves_player", null), 0, null, "");
						base.EndTurn();
					}
				}
			}
		}

		public void AIMakeMove(Move move)
		{
			TilePuluc tilePuluc = move.GoalTile as TilePuluc;
			PawnPuluc pawnPuluc = move.Unit as PawnPuluc;
			pawnPuluc.X = tilePuluc.X;
			foreach (PawnPuluc pawnPuluc2 in pawnPuluc.PawnsBelow)
			{
				pawnPuluc2.X = pawnPuluc.X;
			}
			if (tilePuluc.X < 11)
			{
				List<PawnPuluc> list = this.CheckIfPawnWillCapture(pawnPuluc, tilePuluc.X);
				if (list != null && list.Count > 0)
				{
					pawnPuluc.State = PawnPuluc.MovementState.MovingBackward;
					pawnPuluc.PawnsBelow.AddRange(list);
					foreach (PawnPuluc pawnPuluc3 in list)
					{
						pawnPuluc3.IsTopPawn = false;
						pawnPuluc3.Captured = true;
						pawnPuluc3.CapturedBy = pawnPuluc;
					}
				}
			}
			if (pawnPuluc.X == 12 || pawnPuluc.X == 11)
			{
				this.PawnHasReachedHomeBase(pawnPuluc, true, true);
			}
		}

		public BoardGamePuluc.BoardInformation TakeBoardSnapshot()
		{
			BoardGamePuluc.PawnInformation[] array = new BoardGamePuluc.PawnInformation[base.PlayerOneUnits.Count + base.PlayerTwoUnits.Count];
			int num = 0;
			foreach (PawnBase pawnBase in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits))
			{
				PawnPuluc pawnPuluc = (PawnPuluc)pawnBase;
				List<PawnPuluc> list = new List<PawnPuluc>();
				if (pawnPuluc.PawnsBelow != null && pawnPuluc.PawnsBelow.Count > 0)
				{
					foreach (PawnPuluc pawnPuluc2 in pawnPuluc.PawnsBelow)
					{
						list.Add(pawnPuluc2);
					}
				}
				array[num++] = new BoardGamePuluc.PawnInformation(pawnPuluc.X, pawnPuluc.IsInSpawn, pawnPuluc.IsTopPawn, pawnPuluc.State, list, pawnPuluc.Captured, pawnPuluc.Entity.GlobalPosition, pawnPuluc.CapturedBy);
			}
			foreach (PawnBase pawnBase2 in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits))
			{
				PawnPuluc pawnPuluc3 = (PawnPuluc)pawnBase2;
				List<PawnPuluc> list2 = new List<PawnPuluc>();
				if (pawnPuluc3.PawnsBelow != null && pawnPuluc3.PawnsBelow.Count > 0)
				{
					foreach (PawnPuluc pawnPuluc4 in pawnPuluc3.PawnsBelow)
					{
						list2.Add(pawnPuluc4);
					}
				}
				array[num++] = new BoardGamePuluc.PawnInformation(pawnPuluc3.X, pawnPuluc3.IsInSpawn, pawnPuluc3.IsTopPawn, pawnPuluc3.State, list2, pawnPuluc3.Captured, pawnPuluc3.Entity.GlobalPosition, pawnPuluc3.CapturedBy);
			}
			return new BoardGamePuluc.BoardInformation(ref array);
		}

		public void UndoMove(ref BoardGamePuluc.BoardInformation board)
		{
			int num = 0;
			foreach (PawnBase pawnBase in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits))
			{
				PawnPuluc pawnPuluc = (PawnPuluc)pawnBase;
				pawnPuluc.PawnsBelow.Clear();
				foreach (PawnPuluc pawnPuluc2 in board.PawnInformation[num].PawnsBelow)
				{
					pawnPuluc.PawnsBelow.Add(pawnPuluc2);
				}
				pawnPuluc.IsTopPawn = board.PawnInformation[num].IsTopPawn;
				pawnPuluc.X = board.PawnInformation[num].X;
				pawnPuluc.IsInSpawn = board.PawnInformation[num].IsInSpawn;
				pawnPuluc.State = board.PawnInformation[num].State;
				pawnPuluc.Captured = board.PawnInformation[num].IsCaptured;
				pawnPuluc.CapturedBy = board.PawnInformation[num].CapturedBy;
				num++;
			}
			foreach (PawnBase pawnBase2 in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits))
			{
				PawnPuluc pawnPuluc3 = (PawnPuluc)pawnBase2;
				pawnPuluc3.PawnsBelow.Clear();
				foreach (PawnPuluc pawnPuluc4 in board.PawnInformation[num].PawnsBelow)
				{
					pawnPuluc3.PawnsBelow.Add(pawnPuluc4);
				}
				pawnPuluc3.IsTopPawn = board.PawnInformation[num].IsTopPawn;
				pawnPuluc3.X = board.PawnInformation[num].X;
				pawnPuluc3.IsInSpawn = board.PawnInformation[num].IsInSpawn;
				pawnPuluc3.State = board.PawnInformation[num].State;
				pawnPuluc3.Captured = board.PawnInformation[num].IsCaptured;
				pawnPuluc3.CapturedBy = board.PawnInformation[num].CapturedBy;
				num++;
			}
		}

		private bool CanMovePawnToTile(PawnPuluc pawn, int tileCoord)
		{
			bool flag = false;
			if (tileCoord == 11)
			{
				flag = true;
			}
			else if (tileCoord == 12)
			{
				flag = true;
			}
			else
			{
				List<PawnPuluc> allPawnsForTileCoordinate = this.GetAllPawnsForTileCoordinate(tileCoord);
				if (allPawnsForTileCoordinate.Count == 0)
				{
					flag = true;
				}
				else
				{
					List<PawnPuluc> topPawns = this.GetTopPawns(ref allPawnsForTileCoordinate);
					if (topPawns[0].PlayerOne != pawn.PlayerOne || topPawns[0] == pawn)
					{
						flag = true;
					}
				}
			}
			return flag;
		}

		private List<PawnPuluc> GetAllPawnsForTileCoordinate(int x)
		{
			List<PawnPuluc> list = new List<PawnPuluc>();
			foreach (PawnBase pawnBase in base.PlayerOneUnits)
			{
				PawnPuluc pawnPuluc = (PawnPuluc)pawnBase;
				if (pawnPuluc.X == x)
				{
					list.Add(pawnPuluc);
				}
			}
			foreach (PawnBase pawnBase2 in base.PlayerTwoUnits)
			{
				PawnPuluc pawnPuluc2 = (PawnPuluc)pawnBase2;
				if (pawnPuluc2.X == x)
				{
					list.Add(pawnPuluc2);
				}
			}
			return list;
		}

		private List<PawnPuluc> GetTopPawns(ref List<PawnPuluc> pawns)
		{
			List<PawnPuluc> list = new List<PawnPuluc>();
			foreach (PawnPuluc pawnPuluc in pawns)
			{
				if (pawnPuluc.IsTopPawn)
				{
					list.Add(pawnPuluc);
				}
			}
			return list;
		}

		private List<PawnPuluc> CheckIfPawnWillCapture(PawnPuluc pawn, int tile)
		{
			List<PawnPuluc> allPawnsForTileCoordinate = this.GetAllPawnsForTileCoordinate(tile);
			if (allPawnsForTileCoordinate.Count > 0)
			{
				List<PawnPuluc> topPawns = this.GetTopPawns(ref allPawnsForTileCoordinate);
				if (topPawns.Count == 1)
				{
					return null;
				}
				foreach (PawnPuluc pawnPuluc in topPawns)
				{
					if (pawnPuluc != pawn)
					{
						List<PawnPuluc> list = new List<PawnPuluc>();
						list.Add(pawnPuluc);
						list.AddRange(pawnPuluc.PawnsBelow);
						return list;
					}
				}
			}
			return null;
		}

		private void RestoreStartingBoard()
		{
			int num = 0;
			foreach (PawnBase pawnBase in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerOneUnits : base.PlayerTwoUnits))
			{
				PawnPuluc pawnPuluc = (PawnPuluc)pawnBase;
				if (pawnPuluc.X != -1 && base.Tiles[pawnPuluc.X].PawnOnTile == pawnPuluc)
				{
					base.Tiles[pawnPuluc.X].PawnOnTile = null;
				}
				pawnPuluc.Reset();
				pawnPuluc.AddGoalPosition(pawnPuluc.SpawnPos);
				pawnPuluc.MovePawnToGoalPositions(false, 0.5f, false);
				num++;
			}
			foreach (PawnBase pawnBase2 in ((base.PlayerWhoStarted == PlayerTurn.PlayerOne) ? base.PlayerTwoUnits : base.PlayerOneUnits))
			{
				PawnPuluc pawnPuluc2 = (PawnPuluc)pawnBase2;
				if (pawnPuluc2.X != -1 && base.Tiles[pawnPuluc2.X].PawnOnTile == pawnPuluc2)
				{
					base.Tiles[pawnPuluc2.X].PawnOnTile = null;
				}
				pawnPuluc2.Reset();
				pawnPuluc2.AddGoalPosition(pawnPuluc2.SpawnPos);
				pawnPuluc2.MovePawnToGoalPositions(false, 0.5f, false);
				num++;
			}
		}

		private void SetPawnSides()
		{
			if (this.HasToMovePawnsAcross)
			{
				CapturedPawnsPool playerOnePool = this.PlayerOnePool;
				this.PlayerOnePool = this.PlayerTwoPool;
				this.PlayerTwoPool = playerOnePool;
				if (this._startState.PawnInformation == null)
				{
					for (int i = 0; i < base.PlayerOneUnits.Count; i++)
					{
						PawnPuluc pawnPuluc = base.PlayerTwoUnits[base.PlayerTwoUnits.Count - i - 1] as PawnPuluc;
						PawnPuluc pawnPuluc2 = base.PlayerOneUnits[i] as PawnPuluc;
						Vec3 spawnPos = pawnPuluc.SpawnPos;
						pawnPuluc.SpawnPos = pawnPuluc2.SpawnPos;
						pawnPuluc2.SpawnPos = spawnPos;
					}
				}
			}
			if (this._startState.PawnInformation != null)
			{
				int num = 0;
				int num2 = 1;
				if (base.PlayerWhoStarted != PlayerTurn.PlayerOne)
				{
					num = base.PlayerTwoUnits.Count - 1;
					num2 = -1;
				}
				for (int j = 0; j < base.PlayerOneUnits.Count; j++)
				{
					(base.PlayerOneUnits[j] as PawnPuluc).SpawnPos = this._startState.PawnInformation[num].Position;
					num += num2;
				}
				if (base.PlayerWhoStarted != PlayerTurn.PlayerOne)
				{
					num = base.PlayerOneUnits.Count + base.PlayerTwoUnits.Count - 1;
				}
				for (int k = 0; k < base.PlayerTwoUnits.Count; k++)
				{
					(base.PlayerTwoUnits[k] as PawnPuluc).SpawnPos = this._startState.PawnInformation[num].Position;
					num += num2;
				}
			}
		}

		private void PawnHasReachedHomeBase(PawnPuluc pawn, bool instantmove, bool fake = false)
		{
			foreach (PawnPuluc pawnPuluc in pawn.PawnsBelow)
			{
				if (pawnPuluc.PlayerOne == pawn.PlayerOne)
				{
					pawnPuluc.MovePawnBackToSpawn(instantmove, 0.6f, fake);
				}
				else
				{
					pawnPuluc.X = -1;
					pawnPuluc.IsInSpawn = false;
					if (!fake)
					{
						pawnPuluc.CapturedBy = null;
						base.RemovePawnFromBoard(pawnPuluc, 100f, true);
					}
				}
			}
			pawn.MovePawnBackToSpawn(instantmove, 0.6f, fake);
		}

		public const int WhitePawnCount = 6;

		public const int BlackPawnCount = 6;

		public const int TrackTileCount = 11;

		private const int PlayerHomebaseTileIndex = 11;

		private const int OpponentHomebaseTileIndex = 12;

		private BoardGamePuluc.BoardInformation _startState;

		public struct PawnInformation
		{
			public PawnInformation(int x, bool inSpawn, bool topPawn, PawnPuluc.MovementState state, List<PawnPuluc> pawnsBelow, bool captured, Vec3 position, PawnPuluc capturedBy)
			{
				this.X = x;
				this.IsInSpawn = inSpawn;
				this.IsTopPawn = topPawn;
				this.State = state;
				this.PawnsBelow = pawnsBelow;
				this.IsCaptured = captured;
				this.CapturedBy = capturedBy;
				this.Position = position;
			}

			public readonly int X;

			public readonly bool IsInSpawn;

			public readonly bool IsTopPawn;

			public readonly bool IsCaptured;

			public readonly PawnPuluc.MovementState State;

			public readonly List<PawnPuluc> PawnsBelow;

			public readonly Vec3 Position;

			public readonly PawnPuluc CapturedBy;
		}

		public struct BoardInformation
		{
			public BoardInformation(ref BoardGamePuluc.PawnInformation[] pawns)
			{
				this.PawnInformation = pawns;
			}

			public readonly BoardGamePuluc.PawnInformation[] PawnInformation;
		}
	}
}
