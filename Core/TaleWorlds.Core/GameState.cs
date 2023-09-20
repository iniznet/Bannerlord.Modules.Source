using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public abstract class GameState : MBObjectBase
	{
		public GameState Predecessor
		{
			get
			{
				return this.GameStateManager.FindPredecessor(this);
			}
		}

		public bool IsActive
		{
			get
			{
				return this.GameStateManager != null && this.GameStateManager.ActiveState == this;
			}
		}

		public IReadOnlyCollection<IGameStateListener> Listeners
		{
			get
			{
				return this._listeners.AsReadOnly();
			}
		}

		public GameStateManager GameStateManager { get; internal set; }

		public virtual bool IsMusicMenuState
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsMenuState
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsMission
		{
			get
			{
				return false;
			}
		}

		protected GameState()
		{
			this._listeners = new List<IGameStateListener>();
		}

		public bool RegisterListener(IGameStateListener listener)
		{
			if (this._listeners.Contains(listener))
			{
				return false;
			}
			this._listeners.Add(listener);
			return true;
		}

		public bool UnregisterListener(IGameStateListener listener)
		{
			return this._listeners.Remove(listener);
		}

		public T GetListenerOfType<T>()
		{
			using (List<IGameStateListener>.Enumerator enumerator = this._listeners.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IGameStateListener gameStateListener;
					if ((gameStateListener = enumerator.Current) is T)
					{
						return (T)((object)gameStateListener);
					}
				}
			}
			return default(T);
		}

		internal void HandleInitialize()
		{
			this.OnInitialize();
			foreach (IGameStateListener gameStateListener in this._listeners)
			{
				gameStateListener.OnInitialize();
			}
		}

		protected virtual void OnInitialize()
		{
		}

		internal void HandleFinalize()
		{
			this.OnFinalize();
			foreach (IGameStateListener gameStateListener in this._listeners)
			{
				gameStateListener.OnFinalize();
			}
			this._listeners = null;
			this.GameStateManager = null;
		}

		protected virtual void OnFinalize()
		{
		}

		internal void HandleActivate()
		{
			GameState.NumberOfListenerActivations = 0;
			if (this.IsActive)
			{
				this.OnActivate();
				if (this.IsActive && this._listeners.Count != 0 && GameState.NumberOfListenerActivations == 0)
				{
					foreach (IGameStateListener gameStateListener in this._listeners)
					{
						gameStateListener.OnActivate();
					}
					GameState.NumberOfListenerActivations++;
				}
				if (!string.IsNullOrEmpty(GameStateManager.StateActivateCommand))
				{
					bool flag;
					CommandLineFunctionality.CallFunction(GameStateManager.StateActivateCommand, "", out flag);
				}
				Debug.ReportMemoryBookmark("GameState Activated: " + base.GetType().Name);
			}
		}

		public bool Activated { get; private set; }

		protected virtual void OnActivate()
		{
			this.Activated = true;
		}

		internal void HandleDeactivate()
		{
			this.OnDeactivate();
			foreach (IGameStateListener gameStateListener in this._listeners)
			{
				gameStateListener.OnDeactivate();
			}
		}

		protected virtual void OnDeactivate()
		{
			this.Activated = false;
		}

		protected internal virtual void OnTick(float dt)
		{
		}

		protected internal virtual void OnIdleTick(float dt)
		{
		}

		public int Level;

		private List<IGameStateListener> _listeners;

		public static int NumberOfListenerActivations;
	}
}
