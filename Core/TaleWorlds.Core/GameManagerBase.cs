using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	public abstract class GameManagerBase
	{
		public static GameManagerBase Current { get; private set; }

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

		public void Initialize()
		{
			if (!this._initialized)
			{
				this._initialized = true;
			}
		}

		protected GameManagerBase()
		{
			GameManagerBase.Current = this;
			this._entitySystem = new EntitySystem<GameManagerComponent>();
			this._stepNo = GameManagerLoadingSteps.PreInitializeZerothStep;
		}

		public IEnumerable<GameManagerComponent> Components
		{
			get
			{
				return this._entitySystem.Components;
			}
		}

		public GameManagerComponent AddComponent(Type componentType)
		{
			GameManagerComponent gameManagerComponent = this._entitySystem.AddComponent(componentType);
			gameManagerComponent.GameManager = this;
			return gameManagerComponent;
		}

		public T AddComponent<T>() where T : GameManagerComponent, new()
		{
			return (T)((object)this.AddComponent(typeof(T)));
		}

		public GameManagerComponent GetComponent(Type componentType)
		{
			return this._entitySystem.GetComponent(componentType);
		}

		public T GetComponent<T>() where T : GameManagerComponent
		{
			return this._entitySystem.GetComponent<T>();
		}

		public IEnumerable<T> GetComponents<T>() where T : GameManagerComponent
		{
			return this._entitySystem.GetComponents<T>();
		}

		public void RemoveComponent<T>() where T : GameManagerComponent
		{
			T component = this._entitySystem.GetComponent<T>();
			this.RemoveComponent(component);
		}

		public void RemoveComponent(GameManagerComponent component)
		{
			this._entitySystem.RemoveComponent(component);
		}

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

		public virtual void OnGameEnd(Game game)
		{
			GameManagerBase.Current = null;
			this.Game = null;
		}

		protected virtual void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			nextStep = GameManagerLoadingSteps.None;
		}

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

		public virtual void OnLoadFinished()
		{
		}

		public virtual void InitializeGameStarter(Game game, IGameStarter starterObject)
		{
		}

		public abstract void OnGameStart(Game game, IGameStarter gameStarter);

		public abstract void BeginGameStart(Game game);

		public abstract void OnNewCampaignStart(Game game, object starterObject);

		public abstract void OnAfterCampaignStart(Game game);

		public abstract void RegisterSubModuleObjects(bool isSavedCampaign);

		public abstract void AfterRegisterSubModuleObjects(bool isSavedCampaign);

		public abstract void OnGameInitializationFinished(Game game);

		public abstract void OnNewGameCreated(Game game, object initializerObject);

		public abstract void OnGameLoaded(Game game, object initializerObject);

		public abstract void OnAfterGameInitializationFinished(Game game, object initializerObject);

		public abstract float ApplicationTime { get; }

		public abstract bool CheatMode { get; }

		public abstract bool IsDevelopmentMode { get; }

		public abstract bool IsEditModeOn { get; }

		public abstract UnitSpawnPrioritizations UnitSpawnPrioritization { get; }

		private EntitySystem<GameManagerComponent> _entitySystem;

		private GameManagerLoadingSteps _stepNo;

		private Game _game;

		private bool _initialized;
	}
}
