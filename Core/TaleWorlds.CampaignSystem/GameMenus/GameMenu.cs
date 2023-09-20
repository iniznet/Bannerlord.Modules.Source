using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	// Token: 0x020000E0 RID: 224
	public class GameMenu
	{
		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06001369 RID: 4969 RVA: 0x00056D6E File Offset: 0x00054F6E
		// (set) Token: 0x0600136A RID: 4970 RVA: 0x00056D76 File Offset: 0x00054F76
		public GameMenu.MenuAndOptionType Type { get; private set; }

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x0600136B RID: 4971 RVA: 0x00056D7F File Offset: 0x00054F7F
		// (set) Token: 0x0600136C RID: 4972 RVA: 0x00056D87 File Offset: 0x00054F87
		public string StringId { get; private set; }

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x0600136D RID: 4973 RVA: 0x00056D90 File Offset: 0x00054F90
		// (set) Token: 0x0600136E RID: 4974 RVA: 0x00056D98 File Offset: 0x00054F98
		public object RelatedObject { get; private set; }

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x0600136F RID: 4975 RVA: 0x00056DA1 File Offset: 0x00054FA1
		// (set) Token: 0x06001370 RID: 4976 RVA: 0x00056DA9 File Offset: 0x00054FA9
		public TextObject MenuTitle { get; private set; }

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x06001371 RID: 4977 RVA: 0x00056DB2 File Offset: 0x00054FB2
		// (set) Token: 0x06001372 RID: 4978 RVA: 0x00056DBA File Offset: 0x00054FBA
		public GameOverlays.MenuOverlayType OverlayType { get; private set; }

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06001373 RID: 4979 RVA: 0x00056DC3 File Offset: 0x00054FC3
		// (set) Token: 0x06001374 RID: 4980 RVA: 0x00056DCB File Offset: 0x00054FCB
		public bool IsReady { get; private set; }

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06001375 RID: 4981 RVA: 0x00056DD4 File Offset: 0x00054FD4
		public int MenuItemAmount
		{
			get
			{
				return this._menuItems.Count;
			}
		}

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x06001376 RID: 4982 RVA: 0x00056DE1 File Offset: 0x00054FE1
		public object CurrentRepeatableObject
		{
			get
			{
				if (this.MenuRepeatObjects.Count <= this.CurrentRepeatableIndex)
				{
					return null;
				}
				return this.MenuRepeatObjects[this.CurrentRepeatableIndex];
			}
		}

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x06001377 RID: 4983 RVA: 0x00056E09 File Offset: 0x00055009
		// (set) Token: 0x06001378 RID: 4984 RVA: 0x00056E11 File Offset: 0x00055011
		public bool IsWaitMenu { get; private set; }

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x06001379 RID: 4985 RVA: 0x00056E1A File Offset: 0x0005501A
		// (set) Token: 0x0600137A RID: 4986 RVA: 0x00056E22 File Offset: 0x00055022
		public bool IsWaitActive { get; private set; }

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x00056E2B File Offset: 0x0005502B
		public bool IsEmpty
		{
			get
			{
				return this.MenuRepeatObjects.Count == 0 && this.MenuItemAmount == 0;
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x0600137C RID: 4988 RVA: 0x00056E45 File Offset: 0x00055045
		// (set) Token: 0x0600137D RID: 4989 RVA: 0x00056E4D File Offset: 0x0005504D
		public float Progress { get; private set; }

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x0600137E RID: 4990 RVA: 0x00056E56 File Offset: 0x00055056
		// (set) Token: 0x0600137F RID: 4991 RVA: 0x00056E5E File Offset: 0x0005505E
		public float TargetWaitHours { get; private set; }

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x06001380 RID: 4992 RVA: 0x00056E67 File Offset: 0x00055067
		// (set) Token: 0x06001381 RID: 4993 RVA: 0x00056E6F File Offset: 0x0005506F
		public OnTickDelegate OnTick { get; private set; }

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x06001382 RID: 4994 RVA: 0x00056E78 File Offset: 0x00055078
		// (set) Token: 0x06001383 RID: 4995 RVA: 0x00056E80 File Offset: 0x00055080
		public OnConditionDelegate OnCondition { get; private set; }

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x06001384 RID: 4996 RVA: 0x00056E89 File Offset: 0x00055089
		// (set) Token: 0x06001385 RID: 4997 RVA: 0x00056E91 File Offset: 0x00055091
		public OnConsequenceDelegate OnConsequence { get; private set; }

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x06001386 RID: 4998 RVA: 0x00056E9A File Offset: 0x0005509A
		// (set) Token: 0x06001387 RID: 4999 RVA: 0x00056EA2 File Offset: 0x000550A2
		public int CurrentRepeatableIndex { get; set; }

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x06001388 RID: 5000 RVA: 0x00056EAB File Offset: 0x000550AB
		public IEnumerable<GameMenuOption> MenuOptions
		{
			get
			{
				return this._menuItems;
			}
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x00056EB3 File Offset: 0x000550B3
		internal GameMenu(string idString)
		{
			this.StringId = idString;
			this._menuItems = new List<GameMenuOption>();
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x00056ED8 File Offset: 0x000550D8
		internal void Initialize(TextObject text, OnInitDelegate initDelegate, GameOverlays.MenuOverlayType overlay, GameMenu.MenuFlags flags = GameMenu.MenuFlags.None, object relatedObject = null)
		{
			this.CurrentRepeatableIndex = 0;
			this.LastSelectedMenuObject = null;
			this._defaultText = text;
			this.OnInit = initDelegate;
			this.OverlayType = overlay;
			this.AutoSelectFirst = (flags & GameMenu.MenuFlags.AutoSelectFirst) > GameMenu.MenuFlags.None;
			this.RelatedObject = relatedObject;
			this.IsReady = true;
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x00056F24 File Offset: 0x00055124
		internal void Initialize(TextObject text, OnInitDelegate initDelegate, OnConditionDelegate condition, OnConsequenceDelegate consequence, OnTickDelegate tick, GameMenu.MenuAndOptionType type, GameOverlays.MenuOverlayType overlay, float targetWaitHours = 0f, GameMenu.MenuFlags flags = GameMenu.MenuFlags.None, object relatedObject = null)
		{
			this.CurrentRepeatableIndex = 0;
			this.LastSelectedMenuObject = null;
			this._defaultText = text;
			this.OnInit = initDelegate;
			this.OverlayType = overlay;
			this.AutoSelectFirst = (flags & GameMenu.MenuFlags.AutoSelectFirst) > GameMenu.MenuFlags.None;
			this.RelatedObject = relatedObject;
			this.OnConsequence = consequence;
			this.OnCondition = condition;
			this.Type = type;
			this.OnTick = tick;
			this.TargetWaitHours = targetWaitHours;
			this.IsWaitMenu = type > GameMenu.MenuAndOptionType.RegularMenuOption;
			this.IsReady = true;
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x00056FA3 File Offset: 0x000551A3
		private void AddOption(GameMenuOption newOption, int index = -1)
		{
			if (index >= 0 && this._menuItems.Count >= index)
			{
				this._menuItems.Insert(index, newOption);
				return;
			}
			this._menuItems.Add(newOption);
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x00056FD1 File Offset: 0x000551D1
		public bool GetMenuOptionConditionsHold(Game game, MenuContext menuContext, int menuItemNumber)
		{
			if (this.IsWaitMenu)
			{
				return this._menuItems[menuItemNumber].GetConditionsHold(game, menuContext) && this.RunWaitMenuCondition(menuContext);
			}
			return this._menuItems[menuItemNumber].GetConditionsHold(game, menuContext);
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x0005700D File Offset: 0x0005520D
		public TextObject GetMenuOptionText(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].Text;
		}

		// Token: 0x0600138F RID: 5007 RVA: 0x00057020 File Offset: 0x00055220
		public GameMenuOption GetGameMenuOption(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber];
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x0005702E File Offset: 0x0005522E
		public TextObject GetMenuOptionText2(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].Text2;
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x00057041 File Offset: 0x00055241
		public string GetMenuOptionIdString(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].IdString;
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x00057054 File Offset: 0x00055254
		public TextObject GetMenuOptionTooltip(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].Tooltip;
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x00057067 File Offset: 0x00055267
		public bool GetMenuOptionIsLeave(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].IsLeave;
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x0005707A File Offset: 0x0005527A
		public void SetProgressOfWaitingInMenu(float progress)
		{
			this.Progress = progress;
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x00057083 File Offset: 0x00055283
		public void SetTargetedWaitingTimeAndInitialProgress(float targetedWaitingTime, float initialProgress)
		{
			this.TargetWaitHours = targetedWaitingTime;
			this.SetProgressOfWaitingInMenu(initialProgress);
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x00057094 File Offset: 0x00055294
		public GameMenuOption GetLeaveMenuOption(Game game, MenuContext menuContext)
		{
			for (int i = 0; i < this._menuItems.Count; i++)
			{
				if (this._menuItems[i].IsLeave && this._menuItems[i].IsEnabled && this._menuItems[i].GetConditionsHold(game, menuContext))
				{
					return this._menuItems[i];
				}
			}
			return null;
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x00057100 File Offset: 0x00055300
		public void RunOnTick(MenuContext menuContext, float dt)
		{
			if (this.IsWaitMenu && this.IsWaitActive)
			{
				if (this.OnTick != null)
				{
					MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.MenuTitle);
					this.OnTick(menuCallbackArgs, CampaignTime.Now - this._previousTickTime);
					this._previousTickTime = CampaignTime.Now;
				}
				if (this.Progress >= 1f)
				{
					this.EndWait();
					this.RunWaitMenuConsequence(menuContext);
				}
			}
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x00057174 File Offset: 0x00055374
		public bool RunWaitMenuCondition(MenuContext menuContext)
		{
			if (this.OnCondition != null)
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.MenuTitle);
				bool flag = this.OnCondition(menuCallbackArgs);
				if (flag && !this.IsWaitActive)
				{
					menuContext.GameMenu.StartWait();
				}
				return flag;
			}
			return true;
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x000571BC File Offset: 0x000553BC
		public void RunWaitMenuConsequence(MenuContext menuContext)
		{
			if (this.OnConsequence != null)
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.MenuTitle);
				this.OnConsequence(menuCallbackArgs);
			}
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x000571EC File Offset: 0x000553EC
		public void RunMenuOptionConsequence(MenuContext menuContext, int menuItemNumber)
		{
			if (menuItemNumber >= this._menuItems.Count)
			{
				menuItemNumber = this._menuItems.Count - 1;
			}
			if (this._menuItems[menuItemNumber].IsLeave && this.IsWaitMenu)
			{
				this.EndWait();
			}
			this._menuItems[menuItemNumber].RunConsequence(menuContext);
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x00057249 File Offset: 0x00055449
		public void StartWait()
		{
			this._previousTickTime = CampaignTime.Now;
			this.IsWaitActive = true;
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForward;
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x00057268 File Offset: 0x00055468
		public void EndWait()
		{
			this.IsWaitActive = false;
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x0005727C File Offset: 0x0005547C
		public void RunOnInit(Game game, MenuContext menuContext)
		{
			MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.MenuTitle);
			if (this.OnInit != null)
			{
				Debug.Print("[GAME MENU] " + menuContext.GameMenu.StringId, 0, Debug.DebugColor.White, 17592186044416UL);
				this.OnInit(menuCallbackArgs);
				this.MenuTitle = menuCallbackArgs.MenuTitle;
			}
			CampaignEventDispatcher.Instance.OnGameMenuOpened(menuCallbackArgs);
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x000572E8 File Offset: 0x000554E8
		public void PreInit(MenuContext menuContext)
		{
			MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.MenuTitle);
			CampaignEventDispatcher.Instance.BeforeGameMenuOpened(menuCallbackArgs);
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x00057310 File Offset: 0x00055510
		public void AfterInit(MenuContext menuContext)
		{
			MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.MenuTitle);
			CampaignEventDispatcher.Instance.AfterGameMenuOpened(menuCallbackArgs);
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x00057335 File Offset: 0x00055535
		public TextObject GetText()
		{
			return this._defaultText;
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x060013A1 RID: 5025 RVA: 0x0005733D File Offset: 0x0005553D
		// (set) Token: 0x060013A2 RID: 5026 RVA: 0x00057345 File Offset: 0x00055545
		public bool AutoSelectFirst { get; private set; }

		// Token: 0x060013A3 RID: 5027 RVA: 0x00057350 File Offset: 0x00055550
		public static void ActivateGameMenu(string menuId)
		{
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			if (Campaign.Current.CurrentMenuContext == null)
			{
				Campaign.Current.GameMenuManager.SetNextMenu(menuId);
				MapState mapState = Game.Current.GameStateManager.LastOrDefault<MapState>();
				if (mapState != null)
				{
					mapState.EnterMenuMode();
				}
				if (mapState != null)
				{
					MenuContext menuContext = mapState.MenuContext;
					bool? flag;
					if (menuContext == null)
					{
						flag = null;
					}
					else
					{
						GameMenu gameMenu = menuContext.GameMenu;
						flag = ((gameMenu != null) ? new bool?(gameMenu.IsWaitMenu) : null);
					}
					bool? flag2 = flag;
					bool flag3 = true;
					if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
					{
						mapState.MenuContext.GameMenu.StartWait();
						return;
					}
				}
			}
			else
			{
				GameMenu.SwitchToMenu(menuId);
			}
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x00057404 File Offset: 0x00055604
		public static void SwitchToMenu(string menuId)
		{
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			MenuContext currentMenuContext = Campaign.Current.CurrentMenuContext;
			if (currentMenuContext != null)
			{
				currentMenuContext.SwitchToMenu(menuId);
				if (currentMenuContext.GameMenu.IsWaitMenu && Campaign.Current.TimeControlMode == CampaignTimeControlMode.Stop)
				{
					currentMenuContext.GameMenu.StartWait();
				}
				MenuContext currentMenuContext2 = Campaign.Current.CurrentMenuContext;
				if (currentMenuContext2 != null && currentMenuContext2.GameMenu.StringId == menuId)
				{
					currentMenuContext2.GameMenu.AfterInit(currentMenuContext2);
					return;
				}
			}
			else
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\GameMenus\\GameMenu.cs", "SwitchToMenu", 362);
			}
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x0005749C File Offset: 0x0005569C
		public static void ExitToLast()
		{
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			Campaign.Current.GameMenuManager.ExitToLast();
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x000574B8 File Offset: 0x000556B8
		internal void AddOption(string optionId, TextObject optionText, GameMenuOption.OnConditionDelegate condition, GameMenuOption.OnConsequenceDelegate consequence, int index = -1, bool isLeave = false, bool isRepeatable = false, object relatedObject = null)
		{
			this.AddOption(new GameMenuOption(GameMenu.MenuAndOptionType.RegularMenuOption, optionId, optionText, optionText, condition, consequence, isLeave, isRepeatable, relatedObject), index);
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x000574DF File Offset: 0x000556DF
		internal void RemoveMenuOption(GameMenuOption option)
		{
			this._menuItems.Remove(option);
		}

		// Token: 0x040006D7 RID: 1751
		private TextObject _defaultText;

		// Token: 0x040006DA RID: 1754
		public OnInitDelegate OnInit;

		// Token: 0x040006DC RID: 1756
		public List<object> MenuRepeatObjects = new List<object>();

		// Token: 0x040006DD RID: 1757
		public object LastSelectedMenuObject;

		// Token: 0x040006E5 RID: 1765
		private CampaignTime _previousTickTime;

		// Token: 0x040006E6 RID: 1766
		private readonly List<GameMenuOption> _menuItems;

		// Token: 0x020004EF RID: 1263
		public enum MenuFlags
		{
			// Token: 0x0400152F RID: 5423
			None,
			// Token: 0x04001530 RID: 5424
			AutoSelectFirst
		}

		// Token: 0x020004F0 RID: 1264
		public enum MenuAndOptionType
		{
			// Token: 0x04001532 RID: 5426
			RegularMenuOption,
			// Token: 0x04001533 RID: 5427
			WaitMenuShowProgressAndHoursOption,
			// Token: 0x04001534 RID: 5428
			WaitMenuShowOnlyProgressOption,
			// Token: 0x04001535 RID: 5429
			WaitMenuHideProgressAndHoursOption
		}
	}
}
