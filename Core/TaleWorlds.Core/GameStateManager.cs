using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x02000071 RID: 113
	public class GameStateManager
	{
		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000725 RID: 1829 RVA: 0x00018739 File Offset: 0x00016939
		// (set) Token: 0x06000726 RID: 1830 RVA: 0x00018740 File Offset: 0x00016940
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

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000727 RID: 1831 RVA: 0x00018759 File Offset: 0x00016959
		public IReadOnlyCollection<IGameStateManagerListener> Listeners
		{
			get
			{
				return this._listeners.AsReadOnly();
			}
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000728 RID: 1832 RVA: 0x00018766 File Offset: 0x00016966
		// (set) Token: 0x06000729 RID: 1833 RVA: 0x0001876E File Offset: 0x0001696E
		public GameStateManager.GameStateManagerType CurrentType { get; private set; }

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x0600072A RID: 1834 RVA: 0x00018777 File Offset: 0x00016977
		// (set) Token: 0x0600072B RID: 1835 RVA: 0x0001877F File Offset: 0x0001697F
		public IGameStateManagerOwner Owner { get; private set; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x0600072C RID: 1836 RVA: 0x00018788 File Offset: 0x00016988
		public IEnumerable<GameState> GameStates
		{
			get
			{
				return this._gameStates.AsReadOnly();
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x00018795 File Offset: 0x00016995
		public bool ActiveStateDisabledByUser
		{
			get
			{
				return this._activeStateDisableRequests.Count > 0;
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x0600072E RID: 1838 RVA: 0x000187A5 File Offset: 0x000169A5
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

		// Token: 0x0600072F RID: 1839 RVA: 0x000187D0 File Offset: 0x000169D0
		public GameStateManager(IGameStateManagerOwner owner, GameStateManager.GameStateManagerType gameStateManagerType)
		{
			this.Owner = owner;
			this.CurrentType = gameStateManagerType;
			this._gameStateJobs = new Queue<GameStateManager.GameStateJob>();
			this._gameStates = new List<GameState>();
			this._listeners = new List<IGameStateManagerListener>();
			this._activeStateDisableRequests = new List<WeakReference>();
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x00018820 File Offset: 0x00016A20
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

		// Token: 0x06000731 RID: 1841 RVA: 0x00018850 File Offset: 0x00016A50
		public bool RegisterListener(IGameStateManagerListener listener)
		{
			if (this._listeners.Contains(listener))
			{
				return false;
			}
			this._listeners.Add(listener);
			return true;
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x0001886F File Offset: 0x00016A6F
		public bool UnregisterListener(IGameStateManagerListener listener)
		{
			return this._listeners.Remove(listener);
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00018880 File Offset: 0x00016A80
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

		// Token: 0x06000734 RID: 1844 RVA: 0x000188EC File Offset: 0x00016AEC
		public void RegisterActiveStateDisableRequest(object requestingInstance)
		{
			if (!this._activeStateDisableRequests.Contains(requestingInstance))
			{
				this._activeStateDisableRequests.Add(new WeakReference(requestingInstance));
			}
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00018910 File Offset: 0x00016B10
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

		// Token: 0x06000736 RID: 1846 RVA: 0x0001895C File Offset: 0x00016B5C
		public void OnSavedGameLoadFinished()
		{
			foreach (IGameStateManagerListener gameStateManagerListener in this._listeners)
			{
				gameStateManagerListener.OnSavedGameLoadFinished();
			}
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x000189AC File Offset: 0x00016BAC
		public T LastOrDefault<T>() where T : GameState
		{
			return this._gameStates.LastOrDefault((GameState g) => g is T) as T;
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x000189E4 File Offset: 0x00016BE4
		public T CreateState<T>() where T : GameState, new()
		{
			T t = new T();
			this.HandleCreateState(t);
			return t;
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x00018A04 File Offset: 0x00016C04
		public T CreateState<T>(params object[] parameters) where T : GameState, new()
		{
			GameState gameState = (GameState)Activator.CreateInstance(typeof(T), parameters);
			this.HandleCreateState(gameState);
			return (T)((object)gameState);
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x00018A34 File Offset: 0x00016C34
		private void HandleCreateState(GameState state)
		{
			state.GameStateManager = this;
			foreach (IGameStateManagerListener gameStateManagerListener in this._listeners)
			{
				gameStateManagerListener.OnCreateState(state);
			}
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x00018A8C File Offset: 0x00016C8C
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

		// Token: 0x0600073C RID: 1852 RVA: 0x00018AC0 File Offset: 0x00016CC0
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

		// Token: 0x0600073D RID: 1853 RVA: 0x00018B10 File Offset: 0x00016D10
		public void PushState(GameState gameState, int level = 0)
		{
			GameStateManager.GameStateJob gameStateJob = new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.Push, gameState, level);
			this._gameStateJobs.Enqueue(gameStateJob);
			this.DoGameStateJobs();
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00018B3C File Offset: 0x00016D3C
		public void PopState(int level = 0)
		{
			GameStateManager.GameStateJob gameStateJob = new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.Pop, null, level);
			this._gameStateJobs.Enqueue(gameStateJob);
			this.DoGameStateJobs();
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x00018B68 File Offset: 0x00016D68
		public void CleanAndPushState(GameState gameState, int level = 0)
		{
			GameStateManager.GameStateJob gameStateJob = new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.CleanAndPushState, gameState, level);
			this._gameStateJobs.Enqueue(gameStateJob);
			this.DoGameStateJobs();
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x00018B94 File Offset: 0x00016D94
		public void CleanStates(int level = 0)
		{
			GameStateManager.GameStateJob gameStateJob = new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.CleanStates, null, level);
			this._gameStateJobs.Enqueue(gameStateJob);
			this.DoGameStateJobs();
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x00018BC0 File Offset: 0x00016DC0
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

		// Token: 0x06000742 RID: 1858 RVA: 0x00018CB8 File Offset: 0x00016EB8
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

		// Token: 0x06000743 RID: 1859 RVA: 0x00018DC8 File Offset: 0x00016FC8
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

		// Token: 0x06000744 RID: 1860 RVA: 0x00018E70 File Offset: 0x00017070
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

		// Token: 0x06000745 RID: 1861 RVA: 0x00018FAC File Offset: 0x000171AC
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

		// Token: 0x040003A9 RID: 937
		private static GameStateManager _current;

		// Token: 0x040003AA RID: 938
		public static string StateActivateCommand;

		// Token: 0x040003AD RID: 941
		private readonly List<GameState> _gameStates;

		// Token: 0x040003AE RID: 942
		private readonly List<IGameStateManagerListener> _listeners;

		// Token: 0x040003AF RID: 943
		private readonly List<WeakReference> _activeStateDisableRequests;

		// Token: 0x040003B0 RID: 944
		private readonly Queue<GameStateManager.GameStateJob> _gameStateJobs;

		// Token: 0x020000F8 RID: 248
		public enum GameStateManagerType
		{
			// Token: 0x040006BA RID: 1722
			Game,
			// Token: 0x040006BB RID: 1723
			Global
		}

		// Token: 0x020000F9 RID: 249
		private struct GameStateJob
		{
			// Token: 0x06000A28 RID: 2600 RVA: 0x00021171 File Offset: 0x0001F371
			public GameStateJob(GameStateManager.GameStateJob.JobType job, GameState gameState, int popLevel)
			{
				this.Job = job;
				this.GameState = gameState;
				this.PopLevel = popLevel;
			}

			// Token: 0x040006BC RID: 1724
			public readonly GameStateManager.GameStateJob.JobType Job;

			// Token: 0x040006BD RID: 1725
			public readonly GameState GameState;

			// Token: 0x040006BE RID: 1726
			public readonly int PopLevel;

			// Token: 0x0200011B RID: 283
			public enum JobType
			{
				// Token: 0x0400073B RID: 1851
				None,
				// Token: 0x0400073C RID: 1852
				Push,
				// Token: 0x0400073D RID: 1853
				Pop,
				// Token: 0x0400073E RID: 1854
				CleanAndPushState,
				// Token: 0x0400073F RID: 1855
				CleanStates
			}
		}
	}
}
