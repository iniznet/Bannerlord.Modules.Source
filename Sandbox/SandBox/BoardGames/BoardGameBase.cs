using System;
using System.Collections.Generic;
using SandBox.BoardGames.AI;
using SandBox.BoardGames.MissionLogics;
using SandBox.BoardGames.Pawns;
using SandBox.BoardGames.Tiles;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.BoardGames
{
	// Token: 0x020000AC RID: 172
	public abstract class BoardGameBase
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000A1E RID: 2590
		public abstract int TileCount { get; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000A1F RID: 2591
		protected abstract bool RotateBoard { get; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000A20 RID: 2592
		protected abstract bool PreMovementStagePresent { get; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000A21 RID: 2593
		protected abstract bool DiceRollRequired { get; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000A22 RID: 2594 RVA: 0x00053C23 File Offset: 0x00051E23
		protected virtual int UnitsToPlacePerTurnInPreMovementStage
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000A23 RID: 2595 RVA: 0x00053C26 File Offset: 0x00051E26
		// (set) Token: 0x06000A24 RID: 2596 RVA: 0x00053C2E File Offset: 0x00051E2E
		protected virtual PawnBase SelectedUnit
		{
			get
			{
				return this._selectedUnit;
			}
			set
			{
				this.OnBeforeSelectedUnitChanged(this._selectedUnit, value);
				this._selectedUnit = value;
				this.OnAfterSelectedUnitChanged();
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000A25 RID: 2597 RVA: 0x00053C4A File Offset: 0x00051E4A
		public TextObject Name { get; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000A26 RID: 2598 RVA: 0x00053C52 File Offset: 0x00051E52
		// (set) Token: 0x06000A27 RID: 2599 RVA: 0x00053C5A File Offset: 0x00051E5A
		public bool InPreMovementStage { get; protected set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000A28 RID: 2600 RVA: 0x00053C63 File Offset: 0x00051E63
		// (set) Token: 0x06000A29 RID: 2601 RVA: 0x00053C6B File Offset: 0x00051E6B
		public TileBase[] Tiles { get; protected set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000A2A RID: 2602 RVA: 0x00053C74 File Offset: 0x00051E74
		// (set) Token: 0x06000A2B RID: 2603 RVA: 0x00053C7C File Offset: 0x00051E7C
		public List<PawnBase> PlayerOneUnits { get; protected set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000A2C RID: 2604 RVA: 0x00053C85 File Offset: 0x00051E85
		// (set) Token: 0x06000A2D RID: 2605 RVA: 0x00053C8D File Offset: 0x00051E8D
		public List<PawnBase> PlayerTwoUnits { get; protected set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000A2E RID: 2606 RVA: 0x00053C96 File Offset: 0x00051E96
		// (set) Token: 0x06000A2F RID: 2607 RVA: 0x00053C9E File Offset: 0x00051E9E
		public int LastDice { get; protected set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000A30 RID: 2608 RVA: 0x00053CA7 File Offset: 0x00051EA7
		public bool IsReady
		{
			get
			{
				return this.ReadyToPlay && !this.SettingUpBoard;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000A31 RID: 2609 RVA: 0x00053CBC File Offset: 0x00051EBC
		// (set) Token: 0x06000A32 RID: 2610 RVA: 0x00053CC4 File Offset: 0x00051EC4
		public PlayerTurn PlayerWhoStarted { get; private set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000A33 RID: 2611 RVA: 0x00053CCD File Offset: 0x00051ECD
		// (set) Token: 0x06000A34 RID: 2612 RVA: 0x00053CD5 File Offset: 0x00051ED5
		public GameOverEnum GameOverInfo { get; private set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000A35 RID: 2613 RVA: 0x00053CDE File Offset: 0x00051EDE
		// (set) Token: 0x06000A36 RID: 2614 RVA: 0x00053CE6 File Offset: 0x00051EE6
		public PlayerTurn PlayerTurn { get; protected set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000A37 RID: 2615 RVA: 0x00053CEF File Offset: 0x00051EEF
		protected IInputContext InputManager
		{
			get
			{
				return this.MissionHandler.Mission.InputManager;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000A38 RID: 2616 RVA: 0x00053D01 File Offset: 0x00051F01
		protected List<PawnBase> PawnSelectFilter { get; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000A39 RID: 2617 RVA: 0x00053D09 File Offset: 0x00051F09
		protected BoardGameAIBase AIOpponent
		{
			get
			{
				return this.MissionHandler.AIOpponent;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000A3A RID: 2618 RVA: 0x00053D16 File Offset: 0x00051F16
		private bool DiceRolled
		{
			get
			{
				return this.LastDice != -1;
			}
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x00053D24 File Offset: 0x00051F24
		protected BoardGameBase(MissionBoardGameLogic mission, TextObject name, PlayerTurn startingPlayer)
		{
			this.Name = name;
			this.MissionHandler = mission;
			this.SetStartingPlayer(startingPlayer);
			this.PlayerOnePool = new CapturedPawnsPool();
			this.PlayerTwoPool = new CapturedPawnsPool();
			this.PlayerOneUnits = new List<PawnBase>();
			this.PlayerTwoUnits = new List<PawnBase>();
			this.PawnSelectFilter = new List<PawnBase>();
		}

		// Token: 0x06000A3C RID: 2620
		public abstract void InitializeUnits();

		// Token: 0x06000A3D RID: 2621
		public abstract void InitializeTiles();

		// Token: 0x06000A3E RID: 2622
		public abstract void InitializeSound();

		// Token: 0x06000A3F RID: 2623
		public abstract List<Move> CalculateValidMoves(PawnBase pawn);

		// Token: 0x06000A40 RID: 2624
		protected abstract PawnBase SelectPawn(PawnBase pawn);

		// Token: 0x06000A41 RID: 2625
		protected abstract bool CheckGameEnded();

		// Token: 0x06000A42 RID: 2626
		protected abstract void OnAfterBoardSetUp();

		// Token: 0x06000A43 RID: 2627 RVA: 0x00053DA3 File Offset: 0x00051FA3
		protected virtual void OnAfterBoardRotated()
		{
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x00053DA5 File Offset: 0x00051FA5
		protected virtual void OnBeforeEndTurn()
		{
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00053DA7 File Offset: 0x00051FA7
		public virtual void RollDice()
		{
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x00053DA9 File Offset: 0x00051FA9
		protected virtual void UpdateAllTilesPositions()
		{
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x00053DAB File Offset: 0x00051FAB
		public virtual void InitializeDiceBoard()
		{
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x00053DB0 File Offset: 0x00051FB0
		public virtual void Reset()
		{
			this.PlayerOnePool.PawnCount = 0;
			this.PlayerTwoPool.PawnCount = 0;
			this.ClearValidMoves();
			this.SelectedUnit = null;
			this.PawnSelectFilter.Clear();
			this.GameOverInfo = GameOverEnum.GameStillInProgress;
			this._draggingSelectedUnit = false;
			this.JustStoppedDraggingUnit = false;
			this._draggingTimer = 0f;
			BoardGameAIBase aiopponent = this.MissionHandler.AIOpponent;
			if (aiopponent != null)
			{
				aiopponent.ResetThinking();
			}
			this.ReadyToPlay = false;
			this._firstTickAfterReady = true;
			this._rotationCompleted = !this.RotateBoard;
			this.SettingUpBoard = true;
			this.UnfocusAllPawns();
			for (int i = 0; i < this.TileCount; i++)
			{
				this.Tiles[i].Reset();
			}
			this.MovesLeftToEndTurn = (this.PreMovementStagePresent ? this.UnitsToPlacePerTurnInPreMovementStage : 1);
			this.LastDice = -1;
			this._waitingAIForfeitResponse = false;
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x00053E90 File Offset: 0x00052090
		protected virtual void OnPawnArrivesGoalPosition(PawnBase pawn, Vec3 prevPos, Vec3 currentPos)
		{
			if (this.IsReady && pawn.IsPlaced && !pawn.Captured && pawn.MovingToDifferentTile)
			{
				this.MovesLeftToEndTurn--;
			}
			pawn.MovingToDifferentTile = false;
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x00053EC7 File Offset: 0x000520C7
		protected virtual void HandlePreMovementStage(float dt)
		{
			Debug.FailedAssert("HandlePreMovementStage is not implemented for " + this.MissionHandler.CurrentBoardGame, "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\BoardGameBase.cs", "HandlePreMovementStage", 288);
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x00053EF8 File Offset: 0x000520F8
		public virtual void InitializeCapturedUnitsZones()
		{
			this.PlayerOnePool.Entity = Mission.Current.Scene.FindEntityWithTag((this.PlayerWhoStarted == PlayerTurn.PlayerOne) ? "captured_pawns_pool_1" : "captured_pawns_pool_2");
			this.PlayerOnePool.PawnCount = 0;
			this.PlayerTwoPool.Entity = Mission.Current.Scene.FindEntityWithTag((this.PlayerWhoStarted == PlayerTurn.PlayerOne) ? "captured_pawns_pool_2" : "captured_pawns_pool_1");
			this.PlayerTwoPool.PawnCount = 0;
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x00053F79 File Offset: 0x00052179
		protected virtual void HandlePreMovementStageAI(Move move)
		{
			Debug.FailedAssert("HandlePreMovementStageAI is not implemented for " + this.MissionHandler.CurrentBoardGame, "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\BoardGameBase.cs", "HandlePreMovementStageAI", 306);
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x00053FA9 File Offset: 0x000521A9
		public virtual void SetPawnCaptured(PawnBase pawn, bool fake = false)
		{
			pawn.Captured = true;
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x00053FB4 File Offset: 0x000521B4
		public virtual List<List<Move>> CalculateAllValidMoves(BoardGameSide side)
		{
			List<List<Move>> list = new List<List<Move>>(100);
			foreach (PawnBase pawnBase in ((side == BoardGameSide.AI) ? this.PlayerTwoUnits : this.PlayerOneUnits))
			{
				list.Add(this.CalculateValidMoves(pawnBase));
			}
			return list;
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x00054024 File Offset: 0x00052224
		protected virtual void SwitchPlayerTurn()
		{
			this.MissionHandler.Handler.SwitchTurns();
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x00054036 File Offset: 0x00052236
		protected virtual void MovePawnToTile(PawnBase pawn, TileBase tile, bool instantMove = false, bool displayMessage = true)
		{
			this.MovePawnToTileDelayed(pawn, tile, instantMove, displayMessage, 0f);
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x00054048 File Offset: 0x00052248
		protected virtual void MovePawnToTileDelayed(PawnBase pawn, TileBase tile, bool instantMove, bool displayMessage, float delay)
		{
			this.ClearValidMoves();
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x00054050 File Offset: 0x00052250
		protected virtual void OnAfterDiceRollAnimation()
		{
			if (this.LastDice != -1)
			{
				this.MissionHandler.Handler.DiceRoll(this.LastDice);
			}
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x00054071 File Offset: 0x00052271
		public void SetUserRay(Vec3 rayBegin, Vec3 rayEnd)
		{
			this._userRayBegin = rayBegin;
			this._userRayEnd = rayEnd;
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x00054084 File Offset: 0x00052284
		public void SetStartingPlayer(PlayerTurn player)
		{
			this.HasToMovePawnsAcross = this.PlayerWhoStarted != player;
			if (player == PlayerTurn.PlayerOne)
			{
				this._rotationTarget = 0f;
			}
			else if (player == PlayerTurn.PlayerTwo)
			{
				this._rotationTarget = 3.1415927f;
			}
			else
			{
				Debug.FailedAssert("Unexpected starting player caught: " + player, "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\BoardGameBase.cs", "SetStartingPlayer", 376);
			}
			this.PlayerWhoStarted = player;
			this.PlayerTurn = player;
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x000540F8 File Offset: 0x000522F8
		public void SetGameOverInfo(GameOverEnum info)
		{
			this.GameOverInfo = info;
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00054104 File Offset: 0x00052304
		public bool HasMovesAvailable(ref List<List<Move>> moves)
		{
			foreach (List<Move> list in moves)
			{
				if (list != null && list.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x00054160 File Offset: 0x00052360
		public int GetTotalMovesAvailable(ref List<List<Move>> moves)
		{
			int num = 0;
			foreach (List<Move> list in moves)
			{
				if (list != null)
				{
					num += list.Count;
				}
			}
			return num;
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x000541B8 File Offset: 0x000523B8
		public void PlayDiceRollSound()
		{
			Vec3 globalPosition = this.DiceBoard.GlobalPosition;
			this.MissionHandler.Mission.MakeSound(this.DiceRollSoundCodeID, globalPosition, true, false, -1, -1);
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x000541EC File Offset: 0x000523EC
		public int GetPlayerOneUnitsAlive()
		{
			int num = 0;
			using (List<PawnBase>.Enumerator enumerator = this.PlayerOneUnits.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Captured)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x00054248 File Offset: 0x00052448
		public int GetPlayerTwoUnitsAlive()
		{
			int num = 0;
			using (List<PawnBase>.Enumerator enumerator = this.PlayerTwoUnits.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Captured)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x000542A4 File Offset: 0x000524A4
		public int GetPlayerOneUnitsDead()
		{
			int num = 0;
			using (List<PawnBase>.Enumerator enumerator = this.PlayerOneUnits.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Captured)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x00054300 File Offset: 0x00052500
		public int GetPlayerTwoUnitsDead()
		{
			int num = 0;
			using (List<PawnBase>.Enumerator enumerator = this.PlayerTwoUnits.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Captured)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0005435C File Offset: 0x0005255C
		public void Initialize()
		{
			this.BoardEntity = Mission.Current.Scene.FindEntityWithTag("boardgame");
			this.InitializeUnits();
			this.InitializeTiles();
			this.InitializeCapturedUnitsZones();
			this.InitializeDiceBoard();
			this.InitializeSound();
			this.Reset();
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0005439C File Offset: 0x0005259C
		protected void RemovePawnFromBoard(PawnBase pawn, float speed, bool instantMove = false)
		{
			CapturedPawnsPool capturedPawnsPool = (pawn.PlayerOne ? this.PlayerOnePool : this.PlayerTwoPool);
			IEnumerable<GameEntity> children = capturedPawnsPool.Entity.GetChildren();
			GameEntity gameEntity = null;
			foreach (GameEntity gameEntity2 in children)
			{
				if (gameEntity2.HasTag("pawn_" + capturedPawnsPool.PawnCount))
				{
					gameEntity = gameEntity2;
					break;
				}
			}
			capturedPawnsPool.PawnCount++;
			Vec3 origin = gameEntity.GetGlobalFrame().origin;
			float num = pawn.Entity.GlobalPosition.z - origin.z;
			float num2 = 0.001f;
			if (num > num2)
			{
				Vec3 vec = origin;
				vec.z = pawn.Entity.GlobalPosition.z;
				pawn.AddGoalPosition(vec);
			}
			else if (num < -num2)
			{
				Vec3 globalPosition = pawn.Entity.GlobalPosition;
				globalPosition.z = origin.z;
				pawn.AddGoalPosition(globalPosition);
			}
			pawn.AddGoalPosition(origin);
			pawn.MovePawnToGoalPositions(instantMove, speed, false);
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x000544C4 File Offset: 0x000526C4
		public bool Tick(float dt)
		{
			foreach (PawnBase pawnBase in this.PlayerOneUnits)
			{
				pawnBase.Tick(dt);
			}
			foreach (PawnBase pawnBase2 in this.PlayerTwoUnits)
			{
				pawnBase2.Tick(dt);
			}
			for (int i = 0; i < this.TileCount; i++)
			{
				this.Tiles[i].Tick(dt);
			}
			if (this.MovingPawnPresent() || !this.DoneSettingUpBoard() || !this.ReadyToPlay)
			{
				return false;
			}
			if (this._firstTickAfterReady)
			{
				this._firstTickAfterReady = false;
				this.MissionHandler.Handler.Activate();
			}
			if (this.IsReady)
			{
				if (this._draggingSelectedUnit)
				{
					Vec3 userRayBegin = this._userRayBegin;
					Vec3 userRayEnd = this._userRayEnd;
					Vec3 globalPosition = this.SelectedUnit.Entity.GlobalPosition;
					float length = (userRayEnd - userRayBegin).Length;
					float num = (globalPosition - userRayBegin).Length / length;
					Vec3 vec;
					vec..ctor(userRayBegin.x + (userRayEnd.x - userRayBegin.x) * num, userRayBegin.y + (userRayEnd.y - userRayBegin.y) * num, this.SelectedUnit.PosBeforeMoving.z + 0.05f, -1f);
					Vec3 vec2 = MBMath.Lerp(globalPosition, vec, 1f, 0.005f);
					this.SelectedUnit.SetPawnAtPosition(vec2);
				}
				if (this.DiceRollAnimationRunning)
				{
					if (this.DiceRollAnimationTimer < 1f)
					{
						this.DiceRollAnimationTimer += dt;
					}
					else
					{
						this.DiceRollAnimationRunning = false;
						this.OnAfterDiceRollAnimation();
					}
				}
				if (this.MovesLeftToEndTurn == 0)
				{
					this.EndTurn();
				}
				else
				{
					this.UpdateTurn(dt);
				}
				this.CheckSwitchPlayerTurn();
				return true;
			}
			return false;
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x000546D8 File Offset: 0x000528D8
		public void ForceDice(int value)
		{
			this.LastDice = value;
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x000546E1 File Offset: 0x000528E1
		protected PawnBase InitializeUnit(PawnBase pawnToInit)
		{
			pawnToInit.OnArrivedIntermediateGoalPosition = new Action<PawnBase, Vec3, Vec3>(this.OnPawnArrivesGoalPosition);
			pawnToInit.OnArrivedFinalGoalPosition = new Action<PawnBase, Vec3, Vec3>(this.OnPawnArrivesGoalPosition);
			return pawnToInit;
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x0005470C File Offset: 0x0005290C
		protected Move HandlePlayerInput(float dt)
		{
			Move move = new Move(null, null);
			if (this.InputManager.IsHotKeyPressed("BoardGamePawnSelect") && !this._draggingSelectedUnit)
			{
				this.JustStoppedDraggingUnit = false;
				PawnBase hoveredPawnIfAny = this.GetHoveredPawnIfAny();
				TileBase hoveredTileIfAny = this.GetHoveredTileIfAny();
				if (hoveredPawnIfAny != null)
				{
					if (this.PawnSelectFilter.Count == 0 || this.PawnSelectFilter.Contains(hoveredPawnIfAny))
					{
						PawnBase selectedUnit = this.SelectedUnit;
						PawnBase pawnBase = this.SelectPawn(hoveredPawnIfAny);
						if (pawnBase.PlayerOne == (this.PlayerTurn == PlayerTurn.PlayerOne) || !pawnBase.PlayerOne == (this.PlayerTurn == PlayerTurn.PlayerTwo))
						{
							if (this.SelectedUnit != null && this.SelectedUnit == selectedUnit)
							{
								this._deselectUnit = true;
							}
						}
						else if (hoveredTileIfAny == null)
						{
							this.SelectedUnit = null;
						}
					}
				}
				else if (hoveredTileIfAny == null)
				{
					this.SelectedUnit = null;
				}
			}
			else if (this.SelectedUnit != null && this.InputManager.IsHotKeyReleased("BoardGamePawnDeselect"))
			{
				if (this._draggingSelectedUnit)
				{
					this._draggingSelectedUnit = false;
					this.JustStoppedDraggingUnit = true;
				}
				else if (this._deselectUnit)
				{
					PawnBase hoveredPawnIfAny2 = this.GetHoveredPawnIfAny();
					if (hoveredPawnIfAny2 != null && hoveredPawnIfAny2 == this.SelectedUnit)
					{
						this.SelectedUnit = null;
						this._deselectUnit = false;
					}
				}
				if (this._validMoves != null)
				{
					this.SelectedUnit.DisableCollisionBody();
					TileBase hoveredTileIfAny2 = this.GetHoveredTileIfAny();
					if (hoveredTileIfAny2 != null && (hoveredTileIfAny2.PawnOnTile == null || hoveredTileIfAny2.PawnOnTile != this.SelectedUnit))
					{
						foreach (Move move2 in this._validMoves)
						{
							if (hoveredTileIfAny2.Entity == move2.GoalTile.Entity)
							{
								move = move2;
							}
						}
					}
					this.SelectedUnit.EnableCollisionBody();
				}
				if (!move.IsValid && this.SelectedUnit != null && this.JustStoppedDraggingUnit)
				{
					this.SelectedUnit.ClearGoalPositions();
					this.SelectedUnit.AddGoalPosition(this.SelectedUnit.PosBeforeMoving);
					this.SelectedUnit.MovePawnToGoalPositions(false, 0.8f, false);
				}
				this._draggingTimer = 0f;
			}
			if (this.SelectedUnit != null && this.InputManager.IsHotKeyDown("BoardGameDragPreview"))
			{
				this._draggingTimer += dt;
				if (this._draggingTimer >= 0.2f)
				{
					this._draggingSelectedUnit = true;
					this._deselectUnit = false;
				}
			}
			return move;
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x0005499C File Offset: 0x00052B9C
		protected PawnBase GetHoveredPawnIfAny()
		{
			PawnBase pawnBase = null;
			float num;
			GameEntity gameEntity;
			Mission.Current.Scene.RayCastForClosestEntityOrTerrain(this._userRayBegin, this._userRayEnd, ref num, ref gameEntity, 0.01f, 79617);
			if (gameEntity != null)
			{
				foreach (PawnBase pawnBase2 in this.PlayerOneUnits)
				{
					if (pawnBase2.Entity.Name.Equals(gameEntity.Name))
					{
						pawnBase = pawnBase2;
						break;
					}
				}
				if (pawnBase == null)
				{
					foreach (PawnBase pawnBase3 in this.PlayerTwoUnits)
					{
						if (pawnBase3.Entity.Name.Equals(gameEntity.Name))
						{
							pawnBase = pawnBase3;
							break;
						}
					}
				}
			}
			return pawnBase;
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x00054AA0 File Offset: 0x00052CA0
		protected TileBase GetHoveredTileIfAny()
		{
			TileBase tileBase = null;
			float num;
			GameEntity gameEntity;
			Mission.Current.Scene.RayCastForClosestEntityOrTerrain(this._userRayBegin, this._userRayEnd, ref num, ref gameEntity, 0.01f, 79617);
			if (gameEntity != null)
			{
				for (int i = 0; i < this.TileCount; i++)
				{
					TileBase tileBase2 = this.Tiles[i];
					if (tileBase2.Entity.Name.Equals(gameEntity.Name))
					{
						tileBase = tileBase2;
						break;
					}
				}
			}
			return tileBase;
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x00054B1C File Offset: 0x00052D1C
		protected void CheckSwitchPlayerTurn()
		{
			if (this.PlayerTurn == PlayerTurn.PlayerOneWaiting || this.PlayerTurn == PlayerTurn.PlayerTwoWaiting)
			{
				bool flag = false;
				using (List<PawnBase>.Enumerator enumerator = this.PlayerOneUnits.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Moving)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					using (List<PawnBase>.Enumerator enumerator = this.PlayerTwoUnits.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Moving)
							{
								flag = true;
								break;
							}
						}
					}
				}
				if (!flag)
				{
					this.SwitchPlayerTurn();
				}
			}
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x00054BDC File Offset: 0x00052DDC
		protected void OnVictory(string message = "str_boardgame_victory_message")
		{
			this.MissionHandler.PlayerOneWon(message);
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x00054BEA File Offset: 0x00052DEA
		protected void OnAfterEndTurn()
		{
			this.ClearValidMoves();
			this.CheckGameEnded();
			this.MovesLeftToEndTurn = (this.InPreMovementStage ? this.UnitsToPlacePerTurnInPreMovementStage : 1);
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x00054C10 File Offset: 0x00052E10
		protected void OnDefeat(string message = "str_boardgame_defeat_message")
		{
			this.MissionHandler.PlayerTwoWon(message);
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x00054C1E File Offset: 0x00052E1E
		protected void OnDraw(string message = "str_boardgame_draw_message")
		{
			this.MissionHandler.GameWasDraw(message);
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x00054C2C File Offset: 0x00052E2C
		private void OnBeforeSelectedUnitChanged(PawnBase oldSelectedUnit, PawnBase newSelectedUnit)
		{
			if (oldSelectedUnit != null)
			{
				oldSelectedUnit.Entity.GetMetaMesh(0).SetFactor1Linear(this.PawnUnselectedFactor);
			}
			if (newSelectedUnit != null)
			{
				newSelectedUnit.Entity.GetMetaMesh(0).SetFactor1Linear(this.PawnSelectedFactor);
			}
			this.ClearValidMoves();
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x00054C68 File Offset: 0x00052E68
		protected void EndTurn()
		{
			this.OnBeforeEndTurn();
			this.SwitchToWaiting();
			this.OnAfterEndTurn();
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00054C7C File Offset: 0x00052E7C
		protected void ClearValidMoves()
		{
			this.HideAllValidTiles();
			if (this._validMoves != null)
			{
				this._validMoves.Clear();
				this._validMoves = null;
			}
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x00054CA0 File Offset: 0x00052EA0
		private void OnAfterSelectedUnitChanged()
		{
			if (this.SelectedUnit != null)
			{
				List<Move> list = this.CalculateValidMoves(this.SelectedUnit);
				if (list != null && list.Count > 0)
				{
					this._validMoves = list;
				}
				if (this.SelectedUnit.PlayerOne || this.MissionHandler.AIOpponent == null)
				{
					this.SelectedUnit.PlayPawnSelectSound();
					this.ShowAllValidTiles();
				}
			}
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x00054D00 File Offset: 0x00052F00
		private void UpdateTurn(float dt)
		{
			if (this.PlayerTurn == PlayerTurn.PlayerOne || (this.PlayerTurn == PlayerTurn.PlayerTwo && this.AIOpponent == null))
			{
				if (this.InPreMovementStage)
				{
					this.HandlePreMovementStage(dt);
					return;
				}
				if (!this.DiceRollRequired || this.DiceRolled)
				{
					Move move = this.HandlePlayerInput(dt);
					if (move.IsValid)
					{
						this.MovePawnToTile(move.Unit, move.GoalTile, false, true);
						return;
					}
				}
			}
			else if (this.PlayerTurn == PlayerTurn.PlayerTwo && this.AIOpponent != null && !this._waitingAIForfeitResponse)
			{
				if (this.AIOpponent.WantsToForfeit())
				{
					this.OnAIWantsForfeit();
				}
				if (this.DiceRollRequired && !this.DiceRolled)
				{
					this.RollDice();
				}
				this.AIOpponent.UpdateThinkingAboutMove(dt);
				if (this.AIOpponent.CanMakeMove())
				{
					this.SelectedUnit = this.AIOpponent.RecentMoveCalculated.Unit;
					if (this.SelectedUnit != null)
					{
						if (this.InPreMovementStage)
						{
							this.HandlePreMovementStageAI(this.AIOpponent.RecentMoveCalculated);
						}
						else
						{
							TileBase goalTile = this.AIOpponent.RecentMoveCalculated.GoalTile;
							this.MovePawnToTile(this.SelectedUnit, goalTile, false, true);
						}
					}
					else
					{
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_boardgame_no_available_moves_opponent", null), 0, null, "");
						this.EndTurn();
					}
					this.AIOpponent.ResetThinking();
				}
			}
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x00054E5C File Offset: 0x0005305C
		private bool DoneSettingUpBoard()
		{
			bool flag = !this.SettingUpBoard;
			if (this.SettingUpBoard)
			{
				if (this._rotationApplied != this._rotationTarget && this.RotateBoard)
				{
					float num = this._rotationTarget - this._rotationApplied;
					float num2 = 0.05f;
					float num3 = MathF.Clamp(num, -num2, num2);
					MatrixFrame globalFrame = this.BoardEntity.GetGlobalFrame();
					globalFrame.rotation.RotateAboutUp(num3);
					this.BoardEntity.SetGlobalFrame(ref globalFrame);
					this._rotationApplied += num3;
					if (MathF.Abs(this._rotationTarget - this._rotationApplied) <= 1E-05f)
					{
						this._rotationApplied = this._rotationTarget;
						this.UpdateAllPawnsPositions();
						this.UpdateAllTilesPositions();
						return flag;
					}
				}
				else
				{
					if (!this._rotationCompleted)
					{
						this._rotationCompleted = true;
						this.OnAfterBoardRotated();
						return flag;
					}
					this.SettingUpBoard = false;
					this.OnAfterBoardSetUp();
				}
			}
			return flag;
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x00054F40 File Offset: 0x00053140
		protected void HideAllValidTiles()
		{
			if (this._validMoves != null && this._validMoves.Count > 0)
			{
				foreach (Move move in this._validMoves)
				{
					move.GoalTile.SetVisibility(false);
				}
			}
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x00054FAC File Offset: 0x000531AC
		protected void ShowAllValidTiles()
		{
			if (this._validMoves != null && this._validMoves.Count > 0)
			{
				foreach (Move move in this._validMoves)
				{
					move.GoalTile.SetVisibility(true);
				}
			}
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x00055018 File Offset: 0x00053218
		private void UnfocusAllPawns()
		{
			if (this.PlayerOneUnits != null)
			{
				foreach (PawnBase pawnBase in this.PlayerOneUnits)
				{
					pawnBase.Entity.GetMetaMesh(0).SetFactor1Linear(this.PawnUnselectedFactor);
				}
			}
			if (this.PlayerTwoUnits != null)
			{
				foreach (PawnBase pawnBase2 in this.PlayerTwoUnits)
				{
					pawnBase2.Entity.GetMetaMesh(0).SetFactor1Linear(this.PawnUnselectedFactor);
				}
			}
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x000550DC File Offset: 0x000532DC
		private bool MovingPawnPresent()
		{
			bool flag = false;
			foreach (PawnBase pawnBase in this.PlayerOneUnits)
			{
				if (pawnBase.Moving || pawnBase.HasAnyGoalPosition)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (PawnBase pawnBase2 in this.PlayerTwoUnits)
				{
					if (pawnBase2.Moving || pawnBase2.HasAnyGoalPosition)
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x00055194 File Offset: 0x00053394
		private void SwitchToWaiting()
		{
			if (this.PlayerTurn == PlayerTurn.PlayerOne)
			{
				this.PlayerTurn = PlayerTurn.PlayerOneWaiting;
			}
			else if (this.PlayerTurn == PlayerTurn.PlayerTwo)
			{
				this.PlayerTurn = PlayerTurn.PlayerTwoWaiting;
			}
			this.JustStoppedDraggingUnit = false;
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x000551C0 File Offset: 0x000533C0
		protected void OnAIWantsForfeit()
		{
			if (!this._waitingAIForfeitResponse)
			{
				this._waitingAIForfeitResponse = true;
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_boardgame", null).ToString(), GameTexts.FindText("str_boardgame_forfeit_question", null).ToString(), true, true, GameTexts.FindText("str_accept", null).ToString(), GameTexts.FindText("str_reject", null).ToString(), new Action(this.OnAIForfeitAccepted), new Action(this.OnAIForfeitRejected), "", 0f, null, null, null), false, false);
			}
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x00055250 File Offset: 0x00053450
		private void UpdateAllPawnsPositions()
		{
			foreach (PawnBase pawnBase in this.PlayerOneUnits)
			{
				pawnBase.UpdatePawnPosition();
			}
			foreach (PawnBase pawnBase2 in this.PlayerTwoUnits)
			{
				pawnBase2.UpdatePawnPosition();
			}
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x000552E0 File Offset: 0x000534E0
		private void OnAIForfeitAccepted()
		{
			this.MissionHandler.AIForfeitGame();
			this._waitingAIForfeitResponse = false;
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x000552F4 File Offset: 0x000534F4
		private void OnAIForfeitRejected()
		{
			this._waitingAIForfeitResponse = false;
		}

		// Token: 0x0400037B RID: 891
		public const string StringBoardGame = "str_boardgame";

		// Token: 0x0400037C RID: 892
		public const string StringForfeitQuestion = "str_boardgame_forfeit_question";

		// Token: 0x0400037D RID: 893
		public const string StringMovePiecePlayer = "str_boardgame_move_piece_player";

		// Token: 0x0400037E RID: 894
		public const string StringMovePieceOpponent = "str_boardgame_move_piece_opponent";

		// Token: 0x0400037F RID: 895
		public const string StringCapturePiecePlayer = "str_boardgame_capture_piece_player";

		// Token: 0x04000380 RID: 896
		public const string StringCapturePieceOpponent = "str_boardgame_capture_piece_opponent";

		// Token: 0x04000381 RID: 897
		public const string StringVictoryMessage = "str_boardgame_victory_message";

		// Token: 0x04000382 RID: 898
		public const string StringDefeatMessage = "str_boardgame_defeat_message";

		// Token: 0x04000383 RID: 899
		public const string StringDrawMessage = "str_boardgame_draw_message";

		// Token: 0x04000384 RID: 900
		public const string StringNoAvailableMovesPlayer = "str_boardgame_no_available_moves_player";

		// Token: 0x04000385 RID: 901
		public const string StringNoAvailableMovesOpponent = "str_boardgame_no_available_moves_opponent";

		// Token: 0x04000386 RID: 902
		public const string StringSeegaBarrierByP1DrawMessage = "str_boardgame_seega_barrier_by_player_one_draw_message";

		// Token: 0x04000387 RID: 903
		public const string StringSeegaBarrierByP2DrawMessage = "str_boardgame_seega_barrier_by_player_two_draw_message";

		// Token: 0x04000388 RID: 904
		public const string StringSeegaBarrierByP1VictoryMessage = "str_boardgame_seega_barrier_by_player_one_victory_message";

		// Token: 0x04000389 RID: 905
		public const string StringSeegaBarrierByP2VictoryMessage = "str_boardgame_seega_barrier_by_player_two_victory_message";

		// Token: 0x0400038A RID: 906
		public const string StringSeegaBarrierByP1DefeatMessage = "str_boardgame_seega_barrier_by_player_one_defeat_message";

		// Token: 0x0400038B RID: 907
		public const string StringSeegaBarrierByP2DefeatMessage = "str_boardgame_seega_barrier_by_player_two_defeat_message";

		// Token: 0x0400038C RID: 908
		public const string StringRollDicePlayer = "str_boardgame_roll_dice_player";

		// Token: 0x0400038D RID: 909
		public const string StringRollDiceOpponent = "str_boardgame_roll_dice_opponent";

		// Token: 0x0400038E RID: 910
		protected const int InvalidDice = -1;

		// Token: 0x0400038F RID: 911
		protected const float DelayBeforeMovingAnyPawn = 0.25f;

		// Token: 0x04000390 RID: 912
		protected const float DelayBetweenPawnMovementsBegin = 0.15f;

		// Token: 0x04000391 RID: 913
		private const float DiceRollAnimationDuration = 1f;

		// Token: 0x04000392 RID: 914
		private const float DraggingDuration = 0.2f;

		// Token: 0x04000393 RID: 915
		private const int UnitsToPlacePerTurnInMovementStage = 1;

		// Token: 0x04000394 RID: 916
		protected uint PawnSelectedFactor = uint.MaxValue;

		// Token: 0x04000395 RID: 917
		protected uint PawnUnselectedFactor = 4282203453U;

		// Token: 0x04000396 RID: 918
		protected MissionBoardGameLogic MissionHandler;

		// Token: 0x04000397 RID: 919
		protected GameEntity BoardEntity;

		// Token: 0x04000398 RID: 920
		protected GameEntity DiceBoard;

		// Token: 0x04000399 RID: 921
		protected bool JustStoppedDraggingUnit;

		// Token: 0x0400039A RID: 922
		protected CapturedPawnsPool PlayerOnePool;

		// Token: 0x0400039B RID: 923
		protected bool ReadyToPlay;

		// Token: 0x0400039C RID: 924
		protected CapturedPawnsPool PlayerTwoPool;

		// Token: 0x0400039D RID: 925
		protected bool SettingUpBoard = true;

		// Token: 0x0400039E RID: 926
		protected bool HasToMovePawnsAcross;

		// Token: 0x0400039F RID: 927
		protected float DiceRollAnimationTimer;

		// Token: 0x040003A0 RID: 928
		protected int MovesLeftToEndTurn;

		// Token: 0x040003A1 RID: 929
		protected bool DiceRollAnimationRunning;

		// Token: 0x040003A2 RID: 930
		protected int DiceRollSoundCodeID;

		// Token: 0x040003A3 RID: 931
		private List<Move> _validMoves;

		// Token: 0x040003A4 RID: 932
		private PawnBase _selectedUnit;

		// Token: 0x040003A5 RID: 933
		private Vec3 _userRayBegin;

		// Token: 0x040003A6 RID: 934
		private Vec3 _userRayEnd;

		// Token: 0x040003A7 RID: 935
		private float _draggingTimer;

		// Token: 0x040003A8 RID: 936
		private bool _draggingSelectedUnit;

		// Token: 0x040003A9 RID: 937
		private float _rotationApplied;

		// Token: 0x040003AA RID: 938
		private float _rotationTarget;

		// Token: 0x040003AB RID: 939
		private bool _rotationCompleted;

		// Token: 0x040003AC RID: 940
		private bool _deselectUnit;

		// Token: 0x040003AD RID: 941
		private bool _firstTickAfterReady = true;

		// Token: 0x040003AE RID: 942
		private bool _waitingAIForfeitResponse;
	}
}
