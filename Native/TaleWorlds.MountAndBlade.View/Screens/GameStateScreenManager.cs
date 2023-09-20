using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	public class GameStateScreenManager : IGameStateManagerListener
	{
		private GameStateManager GameStateManager
		{
			get
			{
				return GameStateManager.Current;
			}
		}

		public GameStateScreenManager()
		{
			this._screenTypes = new Dictionary<Type, Type>();
			Assembly[] viewAssemblies = GameStateScreenManager.GetViewAssemblies();
			Assembly assembly = typeof(GameStateScreen).Assembly;
			this.CheckAssemblyScreens(assembly);
			foreach (Assembly assembly2 in viewAssemblies)
			{
				this.CheckAssemblyScreens(assembly2);
			}
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 25)
			{
				if (!BannerlordConfig.ForceVSyncInMenus)
				{
					Utilities.SetForceVsync(false);
					return;
				}
				if (this.GameStateManager.ActiveState.IsMenuState)
				{
					Utilities.SetForceVsync(true);
				}
			}
		}

		private void CheckAssemblyScreens(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(GameStateScreen), false);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					foreach (GameStateScreen gameStateScreen in customAttributes)
					{
						if (this._screenTypes.ContainsKey(gameStateScreen.GameStateType))
						{
							this._screenTypes[gameStateScreen.GameStateType] = type;
						}
						else
						{
							this._screenTypes.Add(gameStateScreen.GameStateType, type);
						}
					}
				}
			}
		}

		public static Assembly[] GetViewAssemblies()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly assembly = typeof(GameStateScreen).Assembly;
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
				for (int j = 0; j < referencedAssemblies.Length; j++)
				{
					if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
					{
						list.Add(assembly2);
						break;
					}
				}
			}
			return list.ToArray();
		}

		public ScreenBase CreateScreen(GameState state)
		{
			Type type = null;
			if (this._screenTypes.TryGetValue(state.GetType(), out type))
			{
				return Activator.CreateInstance(type, new object[] { state }) as ScreenBase;
			}
			return null;
		}

		public void BuildScreens()
		{
			int num = 0;
			foreach (GameState gameState in this.GameStateManager.GameStates)
			{
				ScreenBase screenBase = this.CreateScreen(gameState);
				gameState.RegisterListener(screenBase as IGameStateListener);
				if (screenBase != null)
				{
					if (num == 0)
					{
						ScreenManager.CleanAndPushScreen(screenBase);
					}
					else
					{
						ScreenManager.PushScreen(screenBase);
					}
				}
				num++;
			}
		}

		void IGameStateManagerListener.OnCreateState(GameState gameState)
		{
			ScreenBase screenBase = this.CreateScreen(gameState);
			gameState.RegisterListener(screenBase as IGameStateListener);
		}

		void IGameStateManagerListener.OnPushState(GameState gameState, bool isTopGameState)
		{
			if (!gameState.IsMenuState)
			{
				Utilities.ClearOldResourcesAndObjects();
			}
			if (gameState.IsMenuState && BannerlordConfig.ForceVSyncInMenus)
			{
				Utilities.SetForceVsync(true);
			}
			else if (!gameState.IsMenuState)
			{
				Utilities.SetForceVsync(false);
			}
			ScreenBase listenerOfType;
			if ((listenerOfType = gameState.GetListenerOfType<ScreenBase>()) != null)
			{
				if (isTopGameState)
				{
					ScreenManager.CleanAndPushScreen(listenerOfType);
					return;
				}
				ScreenManager.PushScreen(listenerOfType);
			}
		}

		void IGameStateManagerListener.OnPopState(GameState gameState)
		{
			if (!gameState.IsMenuState)
			{
				Utilities.ClearOldResourcesAndObjects();
			}
			if (gameState.IsMenuState && BannerlordConfig.ForceVSyncInMenus)
			{
				Utilities.SetForceVsync(false);
			}
			if (this.GameStateManager.ActiveState != null && this.GameStateManager.ActiveState.IsMenuState && BannerlordConfig.ForceVSyncInMenus)
			{
				Utilities.SetForceVsync(true);
			}
			ScreenManager.PopScreen();
		}

		void IGameStateManagerListener.OnCleanStates()
		{
			ScreenManager.CleanScreens();
		}

		void IGameStateManagerListener.OnSavedGameLoadFinished()
		{
			this.BuildScreens();
		}

		private Dictionary<Type, Type> _screenTypes;
	}
}
