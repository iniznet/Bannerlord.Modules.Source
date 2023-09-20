using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x02000070 RID: 112
	public abstract class GameState : MBObjectBase
	{
		// Token: 0x17000266 RID: 614
		// (get) Token: 0x0600070D RID: 1805 RVA: 0x0001842E File Offset: 0x0001662E
		public GameState Predecessor
		{
			get
			{
				return this.GameStateManager.FindPredecessor(this);
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600070E RID: 1806 RVA: 0x0001843C File Offset: 0x0001663C
		public bool IsActive
		{
			get
			{
				return this.GameStateManager != null && this.GameStateManager.ActiveState == this;
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x0600070F RID: 1807 RVA: 0x00018456 File Offset: 0x00016656
		public IReadOnlyCollection<IGameStateListener> Listeners
		{
			get
			{
				return this._listeners.AsReadOnly();
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000710 RID: 1808 RVA: 0x00018463 File Offset: 0x00016663
		// (set) Token: 0x06000711 RID: 1809 RVA: 0x0001846B File Offset: 0x0001666B
		public GameStateManager GameStateManager { get; internal set; }

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06000712 RID: 1810 RVA: 0x00018474 File Offset: 0x00016674
		public virtual bool IsMusicMenuState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000713 RID: 1811 RVA: 0x00018477 File Offset: 0x00016677
		public virtual bool IsMenuState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000714 RID: 1812 RVA: 0x0001847A File Offset: 0x0001667A
		public virtual bool IsMission
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0001847D File Offset: 0x0001667D
		protected GameState()
		{
			this._listeners = new List<IGameStateListener>();
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x00018490 File Offset: 0x00016690
		public bool RegisterListener(IGameStateListener listener)
		{
			if (this._listeners.Contains(listener))
			{
				return false;
			}
			this._listeners.Add(listener);
			return true;
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x000184AF File Offset: 0x000166AF
		public bool UnregisterListener(IGameStateListener listener)
		{
			return this._listeners.Remove(listener);
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x000184C0 File Offset: 0x000166C0
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

		// Token: 0x06000719 RID: 1817 RVA: 0x0001852C File Offset: 0x0001672C
		internal void HandleInitialize()
		{
			this.OnInitialize();
			foreach (IGameStateListener gameStateListener in this._listeners)
			{
				gameStateListener.OnInitialize();
			}
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x00018584 File Offset: 0x00016784
		protected virtual void OnInitialize()
		{
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00018588 File Offset: 0x00016788
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

		// Token: 0x0600071C RID: 1820 RVA: 0x000185EC File Offset: 0x000167EC
		protected virtual void OnFinalize()
		{
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x000185F0 File Offset: 0x000167F0
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

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x0600071E RID: 1822 RVA: 0x000186B8 File Offset: 0x000168B8
		// (set) Token: 0x0600071F RID: 1823 RVA: 0x000186C0 File Offset: 0x000168C0
		public bool Activated { get; private set; }

		// Token: 0x06000720 RID: 1824 RVA: 0x000186C9 File Offset: 0x000168C9
		protected virtual void OnActivate()
		{
			this.Activated = true;
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x000186D4 File Offset: 0x000168D4
		internal void HandleDeactivate()
		{
			this.OnDeactivate();
			foreach (IGameStateListener gameStateListener in this._listeners)
			{
				gameStateListener.OnDeactivate();
			}
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x0001872C File Offset: 0x0001692C
		protected virtual void OnDeactivate()
		{
			this.Activated = false;
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x00018735 File Offset: 0x00016935
		protected internal virtual void OnTick(float dt)
		{
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00018737 File Offset: 0x00016937
		protected internal virtual void OnIdleTick(float dt)
		{
		}

		// Token: 0x040003A4 RID: 932
		public int Level;

		// Token: 0x040003A5 RID: 933
		private List<IGameStateListener> _listeners;

		// Token: 0x040003A6 RID: 934
		public static int NumberOfListenerActivations;
	}
}
