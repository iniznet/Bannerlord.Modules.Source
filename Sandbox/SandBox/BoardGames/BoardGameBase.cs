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
	public abstract class BoardGameBase
	{
		public abstract int TileCount { get; }

		protected abstract bool RotateBoard { get; }

		protected abstract bool PreMovementStagePresent { get; }

		protected abstract bool DiceRollRequired { get; }

		protected virtual int UnitsToPlacePerTurnInPreMovementStage
		{
			get
			{
				return 1;
			}
		}

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

		public TextObject Name { get; }

		public bool InPreMovementStage { get; protected set; }

		public TileBase[] Tiles { get; protected set; }

		public List<PawnBase> PlayerOneUnits { get; protected set; }

		public List<PawnBase> PlayerTwoUnits { get; protected set; }

		public int LastDice { get; protected set; }

		public bool IsReady
		{
			get
			{
				return this.ReadyToPlay && !this.SettingUpBoard;
			}
		}

		public PlayerTurn PlayerWhoStarted { get; private set; }

		public GameOverEnum GameOverInfo { get; private set; }

		public PlayerTurn PlayerTurn { get; protected set; }

		protected IInputContext InputManager
		{
			get
			{
				return this.MissionHandler.Mission.InputManager;
			}
		}

		protected List<PawnBase> PawnSelectFilter { get; }

		protected BoardGameAIBase AIOpponent
		{
			get
			{
				return this.MissionHandler.AIOpponent;
			}
		}

		private bool DiceRolled
		{
			get
			{
				return this.LastDice != -1;
			}
		}

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

		public abstract void InitializeUnits();

		public abstract void InitializeTiles();

		public abstract void InitializeSound();

		public abstract List<Move> CalculateValidMoves(PawnBase pawn);

		protected abstract PawnBase SelectPawn(PawnBase pawn);

		protected abstract bool CheckGameEnded();

		protected abstract void OnAfterBoardSetUp();

		protected virtual void OnAfterBoardRotated()
		{
		}

		protected virtual void OnBeforeEndTurn()
		{
		}

		public virtual void RollDice()
		{
		}

		protected virtual void UpdateAllTilesPositions()
		{
		}

		public virtual void InitializeDiceBoard()
		{
		}

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

		protected virtual void OnPawnArrivesGoalPosition(PawnBase pawn, Vec3 prevPos, Vec3 currentPos)
		{
			if (this.IsReady && pawn.IsPlaced && !pawn.Captured && pawn.MovingToDifferentTile)
			{
				this.MovesLeftToEndTurn--;
			}
			pawn.MovingToDifferentTile = false;
		}

		protected virtual void HandlePreMovementStage(float dt)
		{
			Debug.FailedAssert("HandlePreMovementStage is not implemented for " + this.MissionHandler.CurrentBoardGame, "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\BoardGameBase.cs", "HandlePreMovementStage", 288);
		}

		public virtual void InitializeCapturedUnitsZones()
		{
			this.PlayerOnePool.Entity = Mission.Current.Scene.FindEntityWithTag((this.PlayerWhoStarted == PlayerTurn.PlayerOne) ? "captured_pawns_pool_1" : "captured_pawns_pool_2");
			this.PlayerOnePool.PawnCount = 0;
			this.PlayerTwoPool.Entity = Mission.Current.Scene.FindEntityWithTag((this.PlayerWhoStarted == PlayerTurn.PlayerOne) ? "captured_pawns_pool_2" : "captured_pawns_pool_1");
			this.PlayerTwoPool.PawnCount = 0;
		}

		protected virtual void HandlePreMovementStageAI(Move move)
		{
			Debug.FailedAssert("HandlePreMovementStageAI is not implemented for " + this.MissionHandler.CurrentBoardGame, "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\BoardGameBase.cs", "HandlePreMovementStageAI", 306);
		}

		public virtual void SetPawnCaptured(PawnBase pawn, bool fake = false)
		{
			pawn.Captured = true;
		}

		public virtual List<List<Move>> CalculateAllValidMoves(BoardGameSide side)
		{
			List<List<Move>> list = new List<List<Move>>(100);
			foreach (PawnBase pawnBase in ((side == BoardGameSide.AI) ? this.PlayerTwoUnits : this.PlayerOneUnits))
			{
				list.Add(this.CalculateValidMoves(pawnBase));
			}
			return list;
		}

		protected virtual void SwitchPlayerTurn()
		{
			this.MissionHandler.Handler.SwitchTurns();
		}

		protected virtual void MovePawnToTile(PawnBase pawn, TileBase tile, bool instantMove = false, bool displayMessage = true)
		{
			this.MovePawnToTileDelayed(pawn, tile, instantMove, displayMessage, 0f);
		}

		protected virtual void MovePawnToTileDelayed(PawnBase pawn, TileBase tile, bool instantMove, bool displayMessage, float delay)
		{
			this.ClearValidMoves();
		}

		protected virtual void OnAfterDiceRollAnimation()
		{
			if (this.LastDice != -1)
			{
				this.MissionHandler.Handler.DiceRoll(this.LastDice);
			}
		}

		public void SetUserRay(Vec3 rayBegin, Vec3 rayEnd)
		{
			this._userRayBegin = rayBegin;
			this._userRayEnd = rayEnd;
		}

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

		public void SetGameOverInfo(GameOverEnum info)
		{
			this.GameOverInfo = info;
		}

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

		public void PlayDiceRollSound()
		{
			Vec3 globalPosition = this.DiceBoard.GlobalPosition;
			this.MissionHandler.Mission.MakeSound(this.DiceRollSoundCodeID, globalPosition, true, false, -1, -1);
		}

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

		public void ForceDice(int value)
		{
			this.LastDice = value;
		}

		protected PawnBase InitializeUnit(PawnBase pawnToInit)
		{
			pawnToInit.OnArrivedIntermediateGoalPosition = new Action<PawnBase, Vec3, Vec3>(this.OnPawnArrivesGoalPosition);
			pawnToInit.OnArrivedFinalGoalPosition = new Action<PawnBase, Vec3, Vec3>(this.OnPawnArrivesGoalPosition);
			return pawnToInit;
		}

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

		protected void OnVictory(string message = "str_boardgame_victory_message")
		{
			this.MissionHandler.PlayerOneWon(message);
		}

		protected void OnAfterEndTurn()
		{
			this.ClearValidMoves();
			this.CheckGameEnded();
			this.MovesLeftToEndTurn = (this.InPreMovementStage ? this.UnitsToPlacePerTurnInPreMovementStage : 1);
		}

		protected void OnDefeat(string message = "str_boardgame_defeat_message")
		{
			this.MissionHandler.PlayerTwoWon(message);
		}

		protected void OnDraw(string message = "str_boardgame_draw_message")
		{
			this.MissionHandler.GameWasDraw(message);
		}

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

		protected void EndTurn()
		{
			this.OnBeforeEndTurn();
			this.SwitchToWaiting();
			this.OnAfterEndTurn();
		}

		protected void ClearValidMoves()
		{
			this.HideAllValidTiles();
			if (this._validMoves != null)
			{
				this._validMoves.Clear();
				this._validMoves = null;
			}
		}

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

		protected void OnAIWantsForfeit()
		{
			if (!this._waitingAIForfeitResponse)
			{
				this._waitingAIForfeitResponse = true;
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_boardgame", null).ToString(), GameTexts.FindText("str_boardgame_forfeit_question", null).ToString(), true, true, GameTexts.FindText("str_accept", null).ToString(), GameTexts.FindText("str_reject", null).ToString(), new Action(this.OnAIForfeitAccepted), new Action(this.OnAIForfeitRejected), "", 0f, null, null, null), false, false);
			}
		}

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

		private void OnAIForfeitAccepted()
		{
			this.MissionHandler.AIForfeitGame();
			this._waitingAIForfeitResponse = false;
		}

		private void OnAIForfeitRejected()
		{
			this._waitingAIForfeitResponse = false;
		}

		public const string StringBoardGame = "str_boardgame";

		public const string StringForfeitQuestion = "str_boardgame_forfeit_question";

		public const string StringMovePiecePlayer = "str_boardgame_move_piece_player";

		public const string StringMovePieceOpponent = "str_boardgame_move_piece_opponent";

		public const string StringCapturePiecePlayer = "str_boardgame_capture_piece_player";

		public const string StringCapturePieceOpponent = "str_boardgame_capture_piece_opponent";

		public const string StringVictoryMessage = "str_boardgame_victory_message";

		public const string StringDefeatMessage = "str_boardgame_defeat_message";

		public const string StringDrawMessage = "str_boardgame_draw_message";

		public const string StringNoAvailableMovesPlayer = "str_boardgame_no_available_moves_player";

		public const string StringNoAvailableMovesOpponent = "str_boardgame_no_available_moves_opponent";

		public const string StringSeegaBarrierByP1DrawMessage = "str_boardgame_seega_barrier_by_player_one_draw_message";

		public const string StringSeegaBarrierByP2DrawMessage = "str_boardgame_seega_barrier_by_player_two_draw_message";

		public const string StringSeegaBarrierByP1VictoryMessage = "str_boardgame_seega_barrier_by_player_one_victory_message";

		public const string StringSeegaBarrierByP2VictoryMessage = "str_boardgame_seega_barrier_by_player_two_victory_message";

		public const string StringSeegaBarrierByP1DefeatMessage = "str_boardgame_seega_barrier_by_player_one_defeat_message";

		public const string StringSeegaBarrierByP2DefeatMessage = "str_boardgame_seega_barrier_by_player_two_defeat_message";

		public const string StringRollDicePlayer = "str_boardgame_roll_dice_player";

		public const string StringRollDiceOpponent = "str_boardgame_roll_dice_opponent";

		protected const int InvalidDice = -1;

		protected const float DelayBeforeMovingAnyPawn = 0.25f;

		protected const float DelayBetweenPawnMovementsBegin = 0.15f;

		private const float DiceRollAnimationDuration = 1f;

		private const float DraggingDuration = 0.2f;

		private const int UnitsToPlacePerTurnInMovementStage = 1;

		protected uint PawnSelectedFactor = uint.MaxValue;

		protected uint PawnUnselectedFactor = 4282203453U;

		protected MissionBoardGameLogic MissionHandler;

		protected GameEntity BoardEntity;

		protected GameEntity DiceBoard;

		protected bool JustStoppedDraggingUnit;

		protected CapturedPawnsPool PlayerOnePool;

		protected bool ReadyToPlay;

		protected CapturedPawnsPool PlayerTwoPool;

		protected bool SettingUpBoard = true;

		protected bool HasToMovePawnsAcross;

		protected float DiceRollAnimationTimer;

		protected int MovesLeftToEndTurn;

		protected bool DiceRollAnimationRunning;

		protected int DiceRollSoundCodeID;

		private List<Move> _validMoves;

		private PawnBase _selectedUnit;

		private Vec3 _userRayBegin;

		private Vec3 _userRayEnd;

		private float _draggingTimer;

		private bool _draggingSelectedUnit;

		private float _rotationApplied;

		private float _rotationTarget;

		private bool _rotationCompleted;

		private bool _deselectUnit;

		private bool _firstTickAfterReady = true;

		private bool _waitingAIForfeitResponse;
	}
}
