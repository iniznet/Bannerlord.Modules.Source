using System;
using Helpers;
using SandBox.BoardGames.MissionLogics;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.AI
{
	public abstract class BoardGameAIBase
	{
		public BoardGameAIBase.AIState State
		{
			get
			{
				return this._state;
			}
		}

		public Move RecentMoveCalculated { get; private set; }

		public bool AbortRequested
		{
			get
			{
				return this.State == BoardGameAIBase.AIState.AbortRequested;
			}
		}

		private protected BoardGameHelper.AIDifficulty Difficulty { protected get; private set; }

		private protected MissionBoardGameLogic BoardGameHandler { protected get; private set; }

		protected BoardGameAIBase(BoardGameHelper.AIDifficulty difficulty, MissionBoardGameLogic boardGameHandler)
		{
			this._stateLock = new object();
			this.Difficulty = difficulty;
			this.BoardGameHandler = boardGameHandler;
			this.Initialize();
			this._aiTask = AsyncTask.CreateWithDelegate(new ManagedDelegate
			{
				Instance = new ManagedDelegate.DelegateDefinition(this.UpdateThinkingAboutMoveOnSeparateThread)
			}, true);
		}

		public virtual Move CalculatePreMovementStageMove()
		{
			Debug.FailedAssert("CalculatePreMovementStageMove is not implemented for " + this.BoardGameHandler.CurrentBoardGame, "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\AI\\BoardGameAIBase.cs", "CalculatePreMovementStageMove", 60);
			return Move.Invalid;
		}

		public abstract Move CalculateMovementStageMove();

		protected abstract void InitializeDifficulty();

		public virtual bool WantsToForfeit()
		{
			return false;
		}

		public void OnSetGameOver()
		{
			object stateLock = this._stateLock;
			lock (stateLock)
			{
				BoardGameAIBase.AIState state = this.State;
				if (state != BoardGameAIBase.AIState.ReadyToRun)
				{
					if (state == BoardGameAIBase.AIState.Running)
					{
						this._state = BoardGameAIBase.AIState.AbortRequested;
					}
				}
				else
				{
					this._state = BoardGameAIBase.AIState.AbortRequested;
				}
			}
			this._aiTask.Wait();
			this.Reset();
		}

		public void Initialize()
		{
			this.Reset();
			this.InitializeDifficulty();
		}

		public void SetDifficulty(BoardGameHelper.AIDifficulty difficulty)
		{
			this.Difficulty = difficulty;
			this.InitializeDifficulty();
		}

		public float HowLongDidAIThinkAboutMove()
		{
			return this._aiDecisionTimer;
		}

		public void UpdateThinkingAboutMove(float dt)
		{
			this._aiDecisionTimer += dt;
			object stateLock = this._stateLock;
			lock (stateLock)
			{
				if (this.State == BoardGameAIBase.AIState.NeedsToRun)
				{
					this._state = BoardGameAIBase.AIState.ReadyToRun;
					this._aiTask.Invoke();
				}
			}
		}

		private void UpdateThinkingAboutMoveOnSeparateThread()
		{
			if (this.BoardGameHandler.Board.InPreMovementStage)
			{
				this.CalculatePreMovementStageOnSeparateThread();
				return;
			}
			this.CalculateMovementStageMoveOnSeparateThread();
		}

		public void ResetThinking()
		{
			this._aiDecisionTimer = 0f;
			this._state = BoardGameAIBase.AIState.NeedsToRun;
		}

		public bool CanMakeMove()
		{
			return this.State == BoardGameAIBase.AIState.Done && this._aiDecisionTimer >= 1.5f;
		}

		private void Reset()
		{
			this.RecentMoveCalculated = Move.Invalid;
			this.MayForfeit = true;
			this.ResetThinking();
			this.MaxDepth = 0;
		}

		private void CalculatePreMovementStageOnSeparateThread()
		{
			if (this.OnBeginSeparateThread())
			{
				Move move = this.CalculatePreMovementStageMove();
				this.OnExitSeparateThread(move);
			}
		}

		private void CalculateMovementStageMoveOnSeparateThread()
		{
			if (this.OnBeginSeparateThread())
			{
				Move move = this.CalculateMovementStageMove();
				this.OnExitSeparateThread(move);
			}
		}

		private bool OnBeginSeparateThread()
		{
			bool flag = false;
			object stateLock = this._stateLock;
			lock (stateLock)
			{
				if (this.AbortRequested)
				{
					this._state = BoardGameAIBase.AIState.Aborted;
					flag = true;
				}
				else
				{
					this._state = BoardGameAIBase.AIState.Running;
				}
			}
			return !flag;
		}

		private void OnExitSeparateThread(Move calculatedMove)
		{
			object stateLock = this._stateLock;
			lock (stateLock)
			{
				if (this.AbortRequested)
				{
					this._state = BoardGameAIBase.AIState.Aborted;
					this.RecentMoveCalculated = Move.Invalid;
				}
				else
				{
					this._state = BoardGameAIBase.AIState.Done;
					this.RecentMoveCalculated = calculatedMove;
				}
			}
		}

		private const float AIDecisionDuration = 1.5f;

		protected bool MayForfeit;

		protected int MaxDepth;

		private float _aiDecisionTimer;

		private readonly ITask _aiTask;

		private readonly object _stateLock;

		private volatile BoardGameAIBase.AIState _state;

		public enum AIState
		{
			NeedsToRun,
			ReadyToRun,
			Running,
			AbortRequested,
			Aborted,
			Done
		}
	}
}
