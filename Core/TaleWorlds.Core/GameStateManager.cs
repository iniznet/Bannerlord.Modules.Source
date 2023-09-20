using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public class GameStateManager
	{
		public static GameStateManager Current
		{
			get
			{
				return GameStateManager._current;
			}
			set
			{
				GameStateManager current = GameStateManager._current;
				if (current != null)
				{
					current.CleanStates(0);
				}
				GameStateManager._current = value;
			}
		}

		public IReadOnlyCollection<IGameStateManagerListener> Listeners
		{
			get
			{
				return this._listeners.AsReadOnly();
			}
		}

		public GameStateManager.GameStateManagerType CurrentType { get; private set; }

		public IGameStateManagerOwner Owner { get; private set; }

		public IEnumerable<GameState> GameStates
		{
			get
			{
				return this._gameStates.AsReadOnly();
			}
		}

		public bool ActiveStateDisabledByUser
		{
			get
			{
				return this._activeStateDisableRequests.Count > 0;
			}
		}

		public GameState ActiveState
		{
			get
			{
				if (this._gameStates.Count <= 0)
				{
					return null;
				}
				return this._gameStates[this._gameStates.Count - 1];
			}
		}

		public GameStateManager(IGameStateManagerOwner owner, GameStateManager.GameStateManagerType gameStateManagerType)
		{
			this.Owner = owner;
			this.CurrentType = gameStateManagerType;
			this._gameStateJobs = new Queue<GameStateManager.GameStateJob>();
			this._gameStates = new List<GameState>();
			this._listeners = new List<IGameStateManagerListener>();
			this._activeStateDisableRequests = new List<WeakReference>();
		}

		internal GameState FindPredecessor(GameState gameState)
		{
			GameState gameState2 = null;
			int num = this._gameStates.IndexOf(gameState);
			if (num > 0)
			{
				gameState2 = this._gameStates[num - 1];
			}
			return gameState2;
		}

		public bool RegisterListener(IGameStateManagerListener listener)
		{
			if (this._listeners.Contains(listener))
			{
				return false;
			}
			this._listeners.Add(listener);
			return true;
		}

		public bool UnregisterListener(IGameStateManagerListener listener)
		{
			return this._listeners.Remove(listener);
		}

		public T GetListenerOfType<T>()
		{
			using (List<IGameStateManagerListener>.Enumerator enumerator = this._listeners.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IGameStateManagerListener gameStateManagerListener;
					if ((gameStateManagerListener = enumerator.Current) is T)
					{
						return (T)((object)gameStateManagerListener);
					}
				}
			}
			return default(T);
		}

		public void RegisterActiveStateDisableRequest(object requestingInstance)
		{
			if (!this._activeStateDisableRequests.Contains(requestingInstance))
			{
				this._activeStateDisableRequests.Add(new WeakReference(requestingInstance));
			}
		}

		public void UnregisterActiveStateDisableRequest(object requestingInstance)
		{
			for (int i = 0; i < this._activeStateDisableRequests.Count; i++)
			{
				WeakReference weakReference = this._activeStateDisableRequests[i];
				if (((weakReference != null) ? weakReference.Target : null) == requestingInstance)
				{
					this._activeStateDisableRequests.RemoveAt(i);
					return;
				}
			}
		}

		public void OnSavedGameLoadFinished()
		{
			foreach (IGameStateManagerListener gameStateManagerListener in this._listeners)
			{
				gameStateManagerListener.OnSavedGameLoadFinished();
			}
		}

		public T LastOrDefault<T>() where T : GameState
		{
			return this._gameStates.LastOrDefault((GameState g) => g is T) as T;
		}

		public T CreateState<T>() where T : GameState, new()
		{
			T t = new T();
			this.HandleCreateState(t);
			return t;
		}

		public T CreateState<T>(params object[] parameters) where T : GameState, new()
		{
			GameState gameState = (GameState)Activator.CreateInstance(typeof(T), parameters);
			this.HandleCreateState(gameState);
			return (T)((object)gameState);
		}

		private void HandleCreateState(GameState state)
		{
			state.GameStateManager = this;
			foreach (IGameStateManagerListener gameStateManagerListener in this._listeners)
			{
				gameStateManagerListener.OnCreateState(state);
			}
		}

		public void OnTick(float dt)
		{
			this.CleanRequests();
			if (this.ActiveState != null)
			{
				if (this.ActiveStateDisabledByUser)
				{
					this.ActiveState.OnIdleTick(dt);
					return;
				}
				this.ActiveState.OnTick(dt);
			}
		}

		private void CleanRequests()
		{
			for (int i = this._activeStateDisableRequests.Count - 1; i >= 0; i--)
			{
				WeakReference weakReference = this._activeStateDisableRequests[i];
				if (weakReference == null || !weakReference.IsAlive)
				{
					this._activeStateDisableRequests.RemoveAt(i);
				}
			}
		}

		public void PushState(GameState gameState, int level = 0)
		{
			GameStateManager.GameStateJob gameStateJob = new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.Push, gameState, level);
			this._gameStateJobs.Enqueue(gameStateJob);
			this.DoGameStateJobs();
		}

		public void PopState(int level = 0)
		{
			GameStateManager.GameStateJob gameStateJob = new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.Pop, null, level);
			this._gameStateJobs.Enqueue(gameStateJob);
			this.DoGameStateJobs();
		}

		public void CleanAndPushState(GameState gameState, int level = 0)
		{
			GameStateManager.GameStateJob gameStateJob = new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.CleanAndPushState, gameState, level);
			this._gameStateJobs.Enqueue(gameStateJob);
			this.DoGameStateJobs();
		}

		public void CleanStates(int level = 0)
		{
			GameStateManager.GameStateJob gameStateJob = new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.CleanStates, null, level);
			this._gameStateJobs.Enqueue(gameStateJob);
			this.DoGameStateJobs();
		}

		private void OnPushState(GameState gameState)
		{
			GameState activeState = this.ActiveState;
			bool flag = this._gameStates.Count == 0;
			int num = this._gameStates.FindLastIndex((GameState state) => state.Level <= gameState.Level);
			if (num == -1)
			{
				this._gameStates.Add(gameState);
			}
			else
			{
				this._gameStates.Insert(num + 1, gameState);
			}
			GameState activeState2 = this.ActiveState;
			if (activeState2 != activeState)
			{
				if (activeState != null && activeState.Activated)
				{
					activeState.HandleDeactivate();
				}
				foreach (IGameStateManagerListener gameStateManagerListener in this._listeners)
				{
					gameStateManagerListener.OnPushState(activeState2, flag);
				}
				activeState2.HandleInitialize();
				activeState2.HandleActivate();
				this.Owner.OnStateChanged(activeState);
			}
			Common.MemoryCleanupGC(false);
		}

		private void OnPopState(int level)
		{
			GameState activeState = this.ActiveState;
			int num = this._gameStates.FindLastIndex((GameState state) => state.Level == level);
			GameState gameState = this._gameStates[num];
			gameState.HandleDeactivate();
			gameState.HandleFinalize();
			this._gameStates.RemoveAt(num);
			GameState activeState2 = this.ActiveState;
			foreach (IGameStateManagerListener gameStateManagerListener in this._listeners)
			{
				gameStateManagerListener.OnPopState(gameState);
			}
			if (activeState2 != activeState)
			{
				if (activeState2 != null)
				{
					activeState2.HandleActivate();
				}
				else if (this._gameStateJobs.Count == 0 || (this._gameStateJobs.Peek().Job != GameStateManager.GameStateJob.JobType.Push && this._gameStateJobs.Peek().Job != GameStateManager.GameStateJob.JobType.CleanAndPushState))
				{
					this.Owner.OnStateStackEmpty();
				}
				this.Owner.OnStateChanged(gameState);
			}
			Common.MemoryCleanupGC(false);
		}

		private void OnCleanAndPushState(GameState gameState)
		{
			int num = -1;
			for (int i = 0; i < this._gameStates.Count; i++)
			{
				if (this._gameStates[i].Level >= gameState.Level)
				{
					num = i - 1;
					break;
				}
			}
			GameState activeState = this.ActiveState;
			for (int j = this._gameStates.Count - 1; j > num; j--)
			{
				GameState gameState2 = this._gameStates[j];
				if (gameState2.Activated)
				{
					gameState2.HandleDeactivate();
				}
				gameState2.HandleFinalize();
				this._gameStates.RemoveAt(j);
			}
			this.OnPushState(gameState);
			this.Owner.OnStateChanged(activeState);
		}

		private void OnCleanStates(int popLevel)
		{
			int num = -1;
			for (int i = 0; i < this._gameStates.Count; i++)
			{
				if (this._gameStates[i].Level >= popLevel)
				{
					num = i - 1;
					break;
				}
			}
			GameState activeState = this.ActiveState;
			for (int j = this._gameStates.Count - 1; j > num; j--)
			{
				GameState gameState = this._gameStates[j];
				if (gameState.Activated)
				{
					gameState.HandleDeactivate();
				}
				gameState.HandleFinalize();
				this._gameStates.RemoveAt(j);
			}
			foreach (IGameStateManagerListener gameStateManagerListener in this._listeners)
			{
				gameStateManagerListener.OnCleanStates();
			}
			GameState activeState2 = this.ActiveState;
			if (activeState != activeState2)
			{
				if (activeState2 != null)
				{
					activeState2.HandleActivate();
				}
				else if (this._gameStateJobs.Count == 0 || (this._gameStateJobs.Peek().Job != GameStateManager.GameStateJob.JobType.Push && this._gameStateJobs.Peek().Job != GameStateManager.GameStateJob.JobType.CleanAndPushState))
				{
					this.Owner.OnStateStackEmpty();
				}
				this.Owner.OnStateChanged(activeState);
			}
		}

		private void DoGameStateJobs()
		{
			while (this._gameStateJobs.Count > 0)
			{
				GameStateManager.GameStateJob gameStateJob = this._gameStateJobs.Dequeue();
				switch (gameStateJob.Job)
				{
				case GameStateManager.GameStateJob.JobType.Push:
					this.OnPushState(gameStateJob.GameState);
					break;
				case GameStateManager.GameStateJob.JobType.Pop:
					this.OnPopState(gameStateJob.PopLevel);
					break;
				case GameStateManager.GameStateJob.JobType.CleanAndPushState:
					this.OnCleanAndPushState(gameStateJob.GameState);
					break;
				case GameStateManager.GameStateJob.JobType.CleanStates:
					this.OnCleanStates(gameStateJob.PopLevel);
					break;
				}
			}
		}

		private static GameStateManager _current;

		public static string StateActivateCommand;

		private readonly List<GameState> _gameStates;

		private readonly List<IGameStateManagerListener> _listeners;

		private readonly List<WeakReference> _activeStateDisableRequests;

		private readonly Queue<GameStateManager.GameStateJob> _gameStateJobs;

		public enum GameStateManagerType
		{
			Game,
			Global
		}

		private struct GameStateJob
		{
			public GameStateJob(GameStateManager.GameStateJob.JobType job, GameState gameState, int popLevel)
			{
				this.Job = job;
				this.GameState = gameState;
				this.PopLevel = popLevel;
			}

			public readonly GameStateManager.GameStateJob.JobType Job;

			public readonly GameState GameState;

			public readonly int PopLevel;

			public enum JobType
			{
				None,
				Push,
				Pop,
				CleanAndPushState,
				CleanStates
			}
		}
	}
}
