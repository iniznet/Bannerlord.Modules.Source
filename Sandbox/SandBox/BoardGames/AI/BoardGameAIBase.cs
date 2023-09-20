using System;
using Helpers;
using SandBox.BoardGames.MissionLogics;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.AI
{
	// Token: 0x020000C3 RID: 195
	public abstract class BoardGameAIBase
	{
		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000BBD RID: 3005 RVA: 0x0005D524 File Offset: 0x0005B724
		public BoardGameAIBase.AIState State
		{
			get
			{
				return this._state;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000BBE RID: 3006 RVA: 0x0005D52E File Offset: 0x0005B72E
		// (set) Token: 0x06000BBF RID: 3007 RVA: 0x0005D536 File Offset: 0x0005B736
		public Move RecentMoveCalculated { get; private set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000BC0 RID: 3008 RVA: 0x0005D53F File Offset: 0x0005B73F
		public bool AbortRequested
		{
			get
			{
				return this.State == BoardGameAIBase.AIState.AbortRequested;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000BC1 RID: 3009 RVA: 0x0005D54A File Offset: 0x0005B74A
		// (set) Token: 0x06000BC2 RID: 3010 RVA: 0x0005D552 File Offset: 0x0005B752
		private protected BoardGameHelper.AIDifficulty Difficulty { protected get; private set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000BC3 RID: 3011 RVA: 0x0005D55B File Offset: 0x0005B75B
		// (set) Token: 0x06000BC4 RID: 3012 RVA: 0x0005D563 File Offset: 0x0005B763
		private protected MissionBoardGameLogic BoardGameHandler { protected get; private set; }

		// Token: 0x06000BC5 RID: 3013 RVA: 0x0005D56C File Offset: 0x0005B76C
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

		// Token: 0x06000BC6 RID: 3014 RVA: 0x0005D5C3 File Offset: 0x0005B7C3
		public virtual Move CalculatePreMovementStageMove()
		{
			Debug.FailedAssert("CalculatePreMovementStageMove is not implemented for " + this.BoardGameHandler.CurrentBoardGame, "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\BoardGames\\AI\\BoardGameAIBase.cs", "CalculatePreMovementStageMove", 60);
			return Move.Invalid;
		}

		// Token: 0x06000BC7 RID: 3015
		public abstract Move CalculateMovementStageMove();

		// Token: 0x06000BC8 RID: 3016
		protected abstract void InitializeDifficulty();

		// Token: 0x06000BC9 RID: 3017 RVA: 0x0005D5F5 File Offset: 0x0005B7F5
		public virtual bool WantsToForfeit()
		{
			return false;
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x0005D5F8 File Offset: 0x0005B7F8
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

		// Token: 0x06000BCB RID: 3019 RVA: 0x0005D668 File Offset: 0x0005B868
		public void Initialize()
		{
			this.Reset();
			this.InitializeDifficulty();
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x0005D676 File Offset: 0x0005B876
		public void SetDifficulty(BoardGameHelper.AIDifficulty difficulty)
		{
			this.Difficulty = difficulty;
			this.InitializeDifficulty();
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x0005D685 File Offset: 0x0005B885
		public float HowLongDidAIThinkAboutMove()
		{
			return this._aiDecisionTimer;
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x0005D690 File Offset: 0x0005B890
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

		// Token: 0x06000BCF RID: 3023 RVA: 0x0005D6F4 File Offset: 0x0005B8F4
		private void UpdateThinkingAboutMoveOnSeparateThread()
		{
			if (this.BoardGameHandler.Board.InPreMovementStage)
			{
				this.CalculatePreMovementStageOnSeparateThread();
				return;
			}
			this.CalculateMovementStageMoveOnSeparateThread();
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x0005D715 File Offset: 0x0005B915
		public void ResetThinking()
		{
			this._aiDecisionTimer = 0f;
			this._state = BoardGameAIBase.AIState.NeedsToRun;
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0005D72B File Offset: 0x0005B92B
		public bool CanMakeMove()
		{
			return this.State == BoardGameAIBase.AIState.Done && this._aiDecisionTimer >= 1.5f;
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x0005D748 File Offset: 0x0005B948
		private void Reset()
		{
			this.RecentMoveCalculated = Move.Invalid;
			this.MayForfeit = true;
			this.ResetThinking();
			this.MaxDepth = 0;
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x0005D76C File Offset: 0x0005B96C
		private void CalculatePreMovementStageOnSeparateThread()
		{
			if (this.OnBeginSeparateThread())
			{
				Move move = this.CalculatePreMovementStageMove();
				this.OnExitSeparateThread(move);
			}
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x0005D790 File Offset: 0x0005B990
		private void CalculateMovementStageMoveOnSeparateThread()
		{
			if (this.OnBeginSeparateThread())
			{
				Move move = this.CalculateMovementStageMove();
				this.OnExitSeparateThread(move);
			}
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x0005D7B4 File Offset: 0x0005B9B4
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

		// Token: 0x06000BD6 RID: 3030 RVA: 0x0005D814 File Offset: 0x0005BA14
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

		// Token: 0x0400042A RID: 1066
		private const float AIDecisionDuration = 1.5f;

		// Token: 0x0400042B RID: 1067
		protected bool MayForfeit;

		// Token: 0x0400042C RID: 1068
		protected int MaxDepth;

		// Token: 0x0400042D RID: 1069
		private float _aiDecisionTimer;

		// Token: 0x0400042E RID: 1070
		private readonly ITask _aiTask;

		// Token: 0x0400042F RID: 1071
		private readonly object _stateLock;

		// Token: 0x04000430 RID: 1072
		private volatile BoardGameAIBase.AIState _state;

		// Token: 0x020001BA RID: 442
		public enum AIState
		{
			// Token: 0x0400082D RID: 2093
			NeedsToRun,
			// Token: 0x0400082E RID: 2094
			ReadyToRun,
			// Token: 0x0400082F RID: 2095
			Running,
			// Token: 0x04000830 RID: 2096
			AbortRequested,
			// Token: 0x04000831 RID: 2097
			Aborted,
			// Token: 0x04000832 RID: 2098
			Done
		}
	}
}
