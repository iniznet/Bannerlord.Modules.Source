using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	// Token: 0x020000E3 RID: 227
	public class GameMenuCallbackManager
	{
		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x060013B0 RID: 5040 RVA: 0x000574EE File Offset: 0x000556EE
		public static GameMenuCallbackManager Instance
		{
			get
			{
				return Campaign.Current.GameMenuCallbackManager;
			}
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x000574FA File Offset: 0x000556FA
		public GameMenuCallbackManager()
		{
			this.FillInitializationHandlers();
			this.FillEventHandlers();
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x00057510 File Offset: 0x00055710
		private void FillInitializationHandlers()
		{
			this._gameMenuInitializationHandlers = new Dictionary<string, GameMenuInitializationHandlerDelegate>();
			Assembly assembly = typeof(GameMenuInitializationHandler).Assembly;
			this.FillInitializationHandlerWith(assembly);
			foreach (Assembly assembly2 in GameMenuCallbackManager.GeAssemblies())
			{
				this.FillInitializationHandlerWith(assembly2);
			}
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x00057560 File Offset: 0x00055760
		private static Assembly[] GeAssemblies()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly assembly = typeof(GameMenu).Assembly;
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

		// Token: 0x060013B4 RID: 5044 RVA: 0x000575E9 File Offset: 0x000557E9
		public void OnGameLoad()
		{
			this.FillInitializationHandlers();
			this.FillEventHandlers();
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x000575F8 File Offset: 0x000557F8
		private void FillInitializationHandlerWith(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(GameMenuInitializationHandler), false);
					if (customAttributes != null && customAttributes.Length != 0)
					{
						foreach (GameMenuInitializationHandler gameMenuInitializationHandler in customAttributes)
						{
							GameMenuInitializationHandlerDelegate gameMenuInitializationHandlerDelegate = Delegate.CreateDelegate(typeof(GameMenuInitializationHandlerDelegate), methodInfo) as GameMenuInitializationHandlerDelegate;
							if (!this._gameMenuInitializationHandlers.ContainsKey(gameMenuInitializationHandler.MenuId))
							{
								this._gameMenuInitializationHandlers.Add(gameMenuInitializationHandler.MenuId, gameMenuInitializationHandlerDelegate);
							}
							else
							{
								this._gameMenuInitializationHandlers[gameMenuInitializationHandler.MenuId] = gameMenuInitializationHandlerDelegate;
							}
						}
					}
				}
			}
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x000576DC File Offset: 0x000558DC
		private void FillEventHandlers()
		{
			this._eventHandlers = new Dictionary<string, Dictionary<string, GameMenuEventHandlerDelegate>>();
			Assembly assembly = typeof(GameMenuEventHandler).Assembly;
			this.FillEventHandlersWith(assembly);
			foreach (Assembly assembly2 in GameMenuCallbackManager.GeAssemblies())
			{
				this.FillEventHandlersWith(assembly2);
			}
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x0005772C File Offset: 0x0005592C
		private void FillEventHandlersWith(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(GameMenuEventHandler), false);
					if (customAttributes != null && customAttributes.Length != 0)
					{
						foreach (GameMenuEventHandler gameMenuEventHandler in customAttributes)
						{
							GameMenuEventHandlerDelegate gameMenuEventHandlerDelegate = Delegate.CreateDelegate(typeof(GameMenuEventHandlerDelegate), methodInfo) as GameMenuEventHandlerDelegate;
							Dictionary<string, GameMenuEventHandlerDelegate> dictionary;
							if (!this._eventHandlers.TryGetValue(gameMenuEventHandler.MenuId, out dictionary))
							{
								dictionary = new Dictionary<string, GameMenuEventHandlerDelegate>();
								this._eventHandlers.Add(gameMenuEventHandler.MenuId, dictionary);
							}
							if (!dictionary.ContainsKey(gameMenuEventHandler.MenuOptionId))
							{
								dictionary.Add(gameMenuEventHandler.MenuOptionId, gameMenuEventHandlerDelegate);
							}
							else
							{
								dictionary[gameMenuEventHandler.MenuOptionId] = gameMenuEventHandlerDelegate;
							}
						}
					}
				}
			}
		}

		// Token: 0x060013B8 RID: 5048 RVA: 0x00057840 File Offset: 0x00055A40
		public void InitializeState(string menuId, MenuContext state)
		{
			GameMenuInitializationHandlerDelegate gameMenuInitializationHandlerDelegate = null;
			if (this._gameMenuInitializationHandlers.TryGetValue(menuId, out gameMenuInitializationHandlerDelegate))
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(state, null);
				gameMenuInitializationHandlerDelegate(menuCallbackArgs);
			}
		}

		// Token: 0x060013B9 RID: 5049 RVA: 0x00057870 File Offset: 0x00055A70
		public void OnConsequence(string menuId, GameMenuOption gameMenuOption, MenuContext state)
		{
			Dictionary<string, GameMenuEventHandlerDelegate> dictionary = null;
			if (this._eventHandlers.TryGetValue(menuId, out dictionary))
			{
				GameMenuEventHandlerDelegate gameMenuEventHandlerDelegate = null;
				if (dictionary.TryGetValue(gameMenuOption.IdString, out gameMenuEventHandlerDelegate))
				{
					MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(state, gameMenuOption.Text);
					gameMenuEventHandlerDelegate(menuCallbackArgs);
				}
			}
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x000578B5 File Offset: 0x00055AB5
		public TextObject GetMenuOptionTooltip(MenuContext menuContext, int menuItemNumber)
		{
			if (menuContext.GameMenu != null)
			{
				return menuContext.GameMenu.GetMenuOptionTooltip(menuItemNumber);
			}
			if (menuContext.GameMenu == null)
			{
				throw new MBMisuseException("Current game menu empty, can not run GetMenuOptionText");
			}
			return TextObject.Empty;
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x000578E4 File Offset: 0x00055AE4
		public TextObject GetVirtualMenuOptionTooltip(MenuContext menuContext, int virtualMenuItemIndex)
		{
			if (menuContext.GameMenu != null)
			{
				int num = ((menuContext.GameMenu.MenuRepeatObjects.Count > 0) ? menuContext.GameMenu.MenuRepeatObjects.Count : 1);
				if (virtualMenuItemIndex < num)
				{
					return this.GetMenuOptionTooltip(menuContext, 0);
				}
				return this.GetMenuOptionTooltip(menuContext, virtualMenuItemIndex + 1 - num);
			}
			else
			{
				if (menuContext.GameMenu == null)
				{
					throw new MBMisuseException("Current game menu empty, can not run GetVirtualMenuOptionText");
				}
				return TextObject.Empty;
			}
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x00057954 File Offset: 0x00055B54
		public TextObject GetVirtualMenuOptionText(MenuContext menuContext, int virtualMenuItemIndex)
		{
			if (menuContext.GameMenu != null)
			{
				int num = ((menuContext.GameMenu.MenuRepeatObjects.Count > 0) ? menuContext.GameMenu.MenuRepeatObjects.Count : 1);
				if (virtualMenuItemIndex < num)
				{
					return this.GetMenuOptionText(menuContext, 0);
				}
				return this.GetMenuOptionText(menuContext, virtualMenuItemIndex + 1 - num);
			}
			else
			{
				if (menuContext.GameMenu == null)
				{
					throw new MBMisuseException("Current game menu empty, can not run GetVirtualMenuOptionText");
				}
				return TextObject.Empty;
			}
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x000579C2 File Offset: 0x00055BC2
		public TextObject GetMenuOptionText(MenuContext menuContext, int menuItemNumber)
		{
			if (menuContext.GameMenu != null)
			{
				return menuContext.GameMenu.GetMenuOptionText(menuItemNumber);
			}
			if (menuContext.GameMenu == null)
			{
				throw new MBMisuseException("Current game menu empty, can not run GetMenuOptionText");
			}
			return TextObject.Empty;
		}

		// Token: 0x040006E9 RID: 1769
		private Dictionary<string, GameMenuInitializationHandlerDelegate> _gameMenuInitializationHandlers;

		// Token: 0x040006EA RID: 1770
		private Dictionary<string, Dictionary<string, GameMenuEventHandlerDelegate>> _eventHandlers;
	}
}
