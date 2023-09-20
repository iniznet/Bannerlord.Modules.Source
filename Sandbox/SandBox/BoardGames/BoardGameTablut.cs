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
	// Token: 0x020000B1 RID: 177
	public class BoardGameTablut : BoardGameBase
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000AF4 RID: 2804 RVA: 0x0005A6CA File Offset: 0x000588CA
		public override int TileCount
		{
			get
			{
				return 81;
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000AF5 RID: 2805 RVA: 0x0005A6CE File Offset: 0x000588CE
		protected override bool RotateBoard
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000AF6 RID: 2806 RVA: 0x0005A6D1 File Offset: 0x000588D1
		protected override bool PreMovementStagePresent
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000AF7 RID: 2807 RVA: 0x0005A6D4 File Offset: 0x000588D4
		protected override bool DiceRollRequired
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000AF8 RID: 2808 RVA: 0x0005A6D7 File Offset: 0x000588D7
		// (set) Token: 0x06000AF9 RID: 2809 RVA: 0x0005A6DF File Offset: 0x000588DF
		private PawnTablut King { get; set; }

		// Token: 0x06000AFA RID: 2810 RVA: 0x0005A6E8 File Offset: 0x000588E8
		public BoardGameTablut(MissionBoardGameLogic mission, PlayerTurn startingPlayer)
			: base(mission, new TextObject("{=qeKskdiY}Tablut", null), startingPlayer)
		{
			this.SelectedUnit = null;
			this.PawnUnselectedFactor = 4287395960U;
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x0005A70F File Offset: 0x0005890F
		public static bool IsCitadelTile(int tileX, int tileY)
		{
			return tileX == 4 && tileY == 4;
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x0005A71C File Offset: 0x0005891C
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

		// Token: 0x06000AFD RID: 2813 RVA: 0x0005A818 File Offset: 0x00058A18
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

		// Token: 0x06000AFE RID: 2814 RVA: 0x0005A95F File Offset: 0x00058B5F
		public override void InitializeSound()
		{
			PawnBase.PawnMoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/move_stone");
			PawnBase.PawnSelectSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/pick_stone");
			PawnBase.PawnTapSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/drop_stone");
			PawnBase.PawnRemoveSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/minigame/out_stone");
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x0005A99D File Offset: 0x00058B9D
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

		// Token: 0x06000B00 RID: 2816 RVA: 0x0005A9C0 File Offset: 0x00058BC0
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

		// Token: 0x06000B01 RID: 2817 RVA: 0x0005AA78 File Offset: 0x00058C78
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

		// Token: 0x06000B02 RID: 2818 RVA: 0x0005AACA File Offset: 0x00058CCA
		protected override void OnAfterBoardSetUp()
		{
			if (this._startState.PawnInformation == null)
			{
				this._startState = this.TakeBoardSnapshot();
			}
			this.ReadyToPlay = true;
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x0005AAEC File Offset: 0x00058CEC
		protected override PawnBase SelectPawn(PawnBase pawn)
		{
			if (pawn.PlayerOne == (base.PlayerTurn == PlayerTurn.PlayerOne))
			{
				this.SelectedUnit = pawn;
			}
			return pawn;
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x0005AB08 File Offset: 0x00058D08
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

		// Token: 0x06000B05 RID: 2821 RVA: 0x0005ACA0 File Offset: 0x00058EA0
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

		// Token: 0x06000B06 RID: 2822 RVA: 0x0005AD10 File Offset: 0x00058F10
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

		// Token: 0x06000B07 RID: 2823 RVA: 0x0005AD6C File Offset: 0x00058F6C
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

		// Token: 0x06000B08 RID: 2824 RVA: 0x0005ADF0 File Offset: 0x00058FF0
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

		// Token: 0x06000B09 RID: 2825 RVA: 0x0005AEAC File Offset: 0x000590AC
		public Move GetRandomAvailableMove(PawnTablut pawn)
		{
			List<Move> list = this.CalculateValidMoves(pawn);
			return list[MBRandom.RandomInt(list.Count)];
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x0005AEC8 File Offset: 0x000590C8
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

		// Token: 0x06000B0B RID: 2827 RVA: 0x0005AF88 File Offset: 0x00059188
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

		// Token: 0x06000B0C RID: 2828 RVA: 0x0005B084 File Offset: 0x00059284
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

		// Token: 0x06000B0D RID: 2829 RVA: 0x0005B199 File Offset: 0x00059399
		private void SetTile(TileBase tile, int x, int y)
		{
			base.Tiles[y * 9 + x] = tile;
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x0005B1A9 File Offset: 0x000593A9
		private TileBase GetTile(int x, int y)
		{
			return base.Tiles[y * 9 + x];
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x0005B1B8 File Offset: 0x000593B8
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

		// Token: 0x06000B10 RID: 2832 RVA: 0x0005B2C8 File Offset: 0x000594C8
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

		// Token: 0x06000B11 RID: 2833 RVA: 0x0005B364 File Offset: 0x00059564
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

		// Token: 0x06000B12 RID: 2834 RVA: 0x0005B3AC File Offset: 0x000595AC
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

		// Token: 0x06000B13 RID: 2835 RVA: 0x0005B418 File Offset: 0x00059618
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

		// Token: 0x040003CE RID: 974
		public const int BoardWidth = 9;

		// Token: 0x040003CF RID: 975
		public const int BoardHeight = 9;

		// Token: 0x040003D0 RID: 976
		public const int AttackerPawnCount = 16;

		// Token: 0x040003D1 RID: 977
		public const int DefenderPawnCount = 9;

		// Token: 0x040003D2 RID: 978
		private BoardGameTablut.BoardInformation _startState;

		// Token: 0x020001B2 RID: 434
		public struct PawnInformation
		{
			// Token: 0x060012B3 RID: 4787 RVA: 0x00074D2D File Offset: 0x00072F2D
			public PawnInformation(int x, int y, bool captured)
			{
				this.X = x;
				this.Y = y;
				this.IsCaptured = captured;
			}

			// Token: 0x04000814 RID: 2068
			public int X;

			// Token: 0x04000815 RID: 2069
			public int Y;

			// Token: 0x04000816 RID: 2070
			public bool IsCaptured;
		}

		// Token: 0x020001B3 RID: 435
		public struct BoardInformation
		{
			// Token: 0x060012B4 RID: 4788 RVA: 0x00074D44 File Offset: 0x00072F44
			public BoardInformation(ref BoardGameTablut.PawnInformation[] pawns, PlayerTurn turn)
			{
				this.PawnInformation = pawns;
				this.Turn = turn;
			}

			// Token: 0x04000817 RID: 2071
			public readonly BoardGameTablut.PawnInformation[] PawnInformation;

			// Token: 0x04000818 RID: 2072
			public readonly PlayerTurn Turn;
		}

		// Token: 0x020001B4 RID: 436
		public enum State
		{
			// Token: 0x0400081A RID: 2074
			InProgress,
			// Token: 0x0400081B RID: 2075
			Aborted,
			// Token: 0x0400081C RID: 2076
			PlayerWon,
			// Token: 0x0400081D RID: 2077
			AIWon
		}
	}
}
