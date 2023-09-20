using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	// Token: 0x02000066 RID: 102
	public abstract class GameManagerBase
	{
		// Token: 0x1700025C RID: 604
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00017ED5 File Offset: 0x000160D5
		// (set) Token: 0x060006C3 RID: 1731 RVA: 0x00017EDC File Offset: 0x000160DC
		public static GameManagerBase Current { get; private set; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00017EE4 File Offset: 0x000160E4
		// (set) Token: 0x060006C5 RID: 1733 RVA: 0x00017EEC File Offset: 0x000160EC
		public Game Game
		{
			get
			{
				return this._game;
			}
			internal set
			{
				if (value == null)
				{
					this._game = null;
					this._initialized = false;
					return;
				}
				this._game = value;
				this.Initialize();
			}
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x00017F0D File Offset: 0x0001610D
		public void Initialize()
		{
			if (!this._initialized)
			{
				this._initialized = true;
			}
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x00017F1E File Offset: 0x0001611E
		protected GameManagerBase()
		{
			GameManagerBase.Current = this;
			this._entitySystem = new EntitySystem<GameManagerComponent>();
			this._stepNo = GameManagerLoadingSteps.PreInitializeZerothStep;
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x00017F3E File Offset: 0x0001613E
		public IEnumerable<GameManagerComponent> Components
		{
			get
			{
				return this._entitySystem.Components;
			}
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x00017F4B File Offset: 0x0001614B
		public GameManagerComponent AddComponent(Type componentType)
		{
			GameManagerComponent gameManagerComponent = this._entitySystem.AddComponent(componentType);
			gameManagerComponent.GameManager = this;
			return gameManagerComponent;
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x00017F60 File Offset: 0x00016160
		public T AddComponent<T>() where T : GameManagerComponent, new()
		{
			return (T)((object)this.AddComponent(typeof(T)));
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x00017F77 File Offset: 0x00016177
		public GameManagerComponent GetComponent(Type componentType)
		{
			return this._entitySystem.GetComponent(componentType);
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x00017F85 File Offset: 0x00016185
		public T GetComponent<T>() where T : GameManagerComponent
		{
			return this._entitySystem.GetComponent<T>();
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x00017F92 File Offset: 0x00016192
		public IEnumerable<T> GetComponents<T>() where T : GameManagerComponent
		{
			return this._entitySystem.GetComponents<T>();
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x00017FA0 File Offset: 0x000161A0
		public void RemoveComponent<T>() where T : GameManagerComponent
		{
			T component = this._entitySystem.GetComponent<T>();
			this.RemoveComponent(component);
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00017FC5 File Offset: 0x000161C5
		public void RemoveComponent(GameManagerComponent component)
		{
			this._entitySystem.RemoveComponent(component);
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x00017FD4 File Offset: 0x000161D4
		public void OnTick(float dt)
		{
			foreach (GameManagerComponent gameManagerComponent in this._entitySystem.Components)
			{
				gameManagerComponent.OnTick();
			}
			if (this.Game != null)
			{
				this.Game.OnTick(dt);
			}
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x00018040 File Offset: 0x00016240
		public void OnGameNetworkBegin()
		{
			foreach (GameManagerComponent gameManagerComponent in this._entitySystem.Components)
			{
				gameManagerComponent.OnGameNetworkBegin();
			}
			if (this.Game != null)
			{
				this.Game.OnGameNetworkBegin();
			}
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x000180A8 File Offset: 0x000162A8
		public void OnGameNetworkEnd()
		{
			foreach (GameManagerComponent gameManagerComponent in this._entitySystem.Components)
			{
				gameManagerComponent.OnGameNetworkEnd();
			}
			if (this.Game != null)
			{
				this.Game.OnGameNetworkEnd();
			}
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x00018110 File Offset: 0x00016310
		public void OnPlayerConnect(VirtualPlayer peer)
		{
			foreach (GameManagerComponent gameManagerComponent in this._entitySystem.Components)
			{
				gameManagerComponent.OnEarlyPlayerConnect(peer);
			}
			if (this.Game != null)
			{
				this.Game.OnEarlyPlayerConnect(peer);
			}
			foreach (GameManagerComponent gameManagerComponent2 in this._entitySystem.Components)
			{
				gameManagerComponent2.OnPlayerConnect(peer);
			}
			if (this.Game != null)
			{
				this.Game.OnPlayerConnect(peer);
			}
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x000181D4 File Offset: 0x000163D4
		public void OnPlayerDisconnect(VirtualPlayer peer)
		{
			foreach (GameManagerComponent gameManagerComponent in this._entitySystem.Components)
			{
				gameManagerComponent.OnPlayerDisconnect(peer);
			}
			if (this.Game != null)
			{
				this.Game.OnPlayerDisconnect(peer);
			}
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x00018240 File Offset: 0x00016440
		public virtual void OnGameEnd(Game game)
		{
			GameManagerBase.Current = null;
			this.Game = null;
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x0001824F File Offset: 0x0001644F
		protected virtual void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			nextStep = GameManagerLoadingSteps.None;
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x00018254 File Offset: 0x00016454
		public bool DoLoadingForGameManager()
		{
			bool flag = false;
			GameManagerLoadingSteps gameManagerLoadingSteps = GameManagerLoadingSteps.None;
			switch (this._stepNo)
			{
			case GameManagerLoadingSteps.PreInitializeZerothStep:
				this.DoLoadingForGameManager(GameManagerLoadingSteps.PreInitializeZerothStep, out gameManagerLoadingSteps);
				if (gameManagerLoadingSteps == GameManagerLoadingSteps.FirstInitializeFirstStep)
				{
					this._stepNo++;
				}
				break;
			case GameManagerLoadingSteps.FirstInitializeFirstStep:
				this.DoLoadingForGameManager(GameManagerLoadingSteps.FirstInitializeFirstStep, out gameManagerLoadingSteps);
				if (gameManagerLoadingSteps == GameManagerLoadingSteps.WaitSecondStep)
				{
					this._stepNo++;
				}
				break;
			case GameManagerLoadingSteps.WaitSecondStep:
				this.DoLoadingForGameManager(GameManagerLoadingSteps.WaitSecondStep, out gameManagerLoadingSteps);
				if (gameManagerLoadingSteps == GameManagerLoadingSteps.SecondInitializeThirdState)
				{
					this._stepNo++;
				}
				break;
			case GameManagerLoadingSteps.SecondInitializeThirdState:
				this.DoLoadingForGameManager(GameManagerLoadingSteps.SecondInitializeThirdState, out gameManagerLoadingSteps);
				if (gameManagerLoadingSteps == GameManagerLoadingSteps.PostInitializeFourthState)
				{
					this._stepNo++;
				}
				break;
			case GameManagerLoadingSteps.PostInitializeFourthState:
				this.DoLoadingForGameManager(GameManagerLoadingSteps.PostInitializeFourthState, out gameManagerLoadingSteps);
				if (gameManagerLoadingSteps == GameManagerLoadingSteps.FinishLoadingFifthStep)
				{
					this._stepNo++;
				}
				break;
			case GameManagerLoadingSteps.FinishLoadingFifthStep:
				this.DoLoadingForGameManager(GameManagerLoadingSteps.FinishLoadingFifthStep, out gameManagerLoadingSteps);
				if (gameManagerLoadingSteps == GameManagerLoadingSteps.None)
				{
					this._stepNo++;
					flag = true;
				}
				break;
			case GameManagerLoadingSteps.LoadingIsOver:
				flag = true;
				break;
			}
			return flag;
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x00018352 File Offset: 0x00016552
		public virtual void OnLoadFinished()
		{
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x00018354 File Offset: 0x00016554
		public virtual void InitializeGameStarter(Game game, IGameStarter starterObject)
		{
		}

		// Token: 0x060006DA RID: 1754
		public abstract void OnGameStart(Game game, IGameStarter gameStarter);

		// Token: 0x060006DB RID: 1755
		public abstract void BeginGameStart(Game game);

		// Token: 0x060006DC RID: 1756
		public abstract void OnNewCampaignStart(Game game, object starterObject);

		// Token: 0x060006DD RID: 1757
		public abstract void OnAfterCampaignStart(Game game);

		// Token: 0x060006DE RID: 1758
		public abstract void RegisterSubModuleObjects(bool isSavedCampaign);

		// Token: 0x060006DF RID: 1759
		public abstract void AfterRegisterSubModuleObjects(bool isSavedCampaign);

		// Token: 0x060006E0 RID: 1760
		public abstract void OnGameInitializationFinished(Game game);

		// Token: 0x060006E1 RID: 1761
		public abstract void OnNewGameCreated(Game game, object initializerObject);

		// Token: 0x060006E2 RID: 1762
		public abstract void OnGameLoaded(Game game, object initializerObject);

		// Token: 0x060006E3 RID: 1763
		public abstract void OnAfterGameInitializationFinished(Game game, object initializerObject);

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060006E4 RID: 1764
		public abstract float ApplicationTime { get; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x060006E5 RID: 1765
		public abstract bool CheatMode { get; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x060006E6 RID: 1766
		public abstract bool IsDevelopmentMode { get; }

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060006E7 RID: 1767
		public abstract bool IsEditModeOn { get; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060006E8 RID: 1768
		public abstract UnitSpawnPrioritizations UnitSpawnPrioritization { get; }

		// Token: 0x0400039D RID: 925
		private EntitySystem<GameManagerComponent> _entitySystem;

		// Token: 0x0400039E RID: 926
		private GameManagerLoadingSteps _stepNo;

		// Token: 0x040003A0 RID: 928
		private Game _game;

		// Token: 0x040003A1 RID: 929
		private bool _initialized;
	}
}
