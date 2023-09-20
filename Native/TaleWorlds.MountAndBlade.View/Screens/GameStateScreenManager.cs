using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x0200002E RID: 46
	public class GameStateScreenManager : IGameStateManagerListener
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060001BF RID: 447 RVA: 0x0000F054 File Offset: 0x0000D254
		private GameStateManager GameStateManager
		{
			get
			{
				return GameStateManager.Current;
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000F05C File Offset: 0x0000D25C
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

		// Token: 0x060001C1 RID: 449 RVA: 0x0000F0D0 File Offset: 0x0000D2D0
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

		// Token: 0x060001C2 RID: 450 RVA: 0x0000F100 File Offset: 0x0000D300
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

		// Token: 0x060001C3 RID: 451 RVA: 0x0000F1A0 File Offset: 0x0000D3A0
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

		// Token: 0x060001C4 RID: 452 RVA: 0x0000F22C File Offset: 0x0000D42C
		public ScreenBase CreateScreen(GameState state)
		{
			Type type = null;
			if (this._screenTypes.TryGetValue(state.GetType(), out type))
			{
				return Activator.CreateInstance(type, new object[] { state }) as ScreenBase;
			}
			return null;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000F268 File Offset: 0x0000D468
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

		// Token: 0x060001C6 RID: 454 RVA: 0x0000F2E4 File Offset: 0x0000D4E4
		void IGameStateManagerListener.OnCreateState(GameState gameState)
		{
			ScreenBase screenBase = this.CreateScreen(gameState);
			gameState.RegisterListener(screenBase as IGameStateListener);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000F308 File Offset: 0x0000D508
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

		// Token: 0x060001C8 RID: 456 RVA: 0x0000F364 File Offset: 0x0000D564
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

		// Token: 0x060001C9 RID: 457 RVA: 0x0000F3C4 File Offset: 0x0000D5C4
		void IGameStateManagerListener.OnCleanStates()
		{
			ScreenManager.CleanScreens();
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000F3CB File Offset: 0x0000D5CB
		void IGameStateManagerListener.OnSavedGameLoadFinished()
		{
			this.BuildScreens();
		}

		// Token: 0x04000120 RID: 288
		private Dictionary<Type, Type> _screenTypes;
	}
}
