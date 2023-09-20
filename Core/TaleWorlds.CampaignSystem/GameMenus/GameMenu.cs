using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	public class GameMenu
	{
		public GameMenu.MenuAndOptionType Type { get; private set; }

		public string StringId { get; private set; }

		public object RelatedObject { get; private set; }

		public TextObject MenuTitle { get; private set; }

		public GameOverlays.MenuOverlayType OverlayType { get; private set; }

		public bool IsReady { get; private set; }

		public int MenuItemAmount
		{
			get
			{
				return this._menuItems.Count;
			}
		}

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

		public bool IsWaitMenu { get; private set; }

		public bool IsWaitActive { get; private set; }

		public bool IsEmpty
		{
			get
			{
				return this.MenuRepeatObjects.Count == 0 && this.MenuItemAmount == 0;
			}
		}

		public float Progress { get; private set; }

		public float TargetWaitHours { get; private set; }

		public OnTickDelegate OnTick { get; private set; }

		public OnConditionDelegate OnCondition { get; private set; }

		public OnConsequenceDelegate OnConsequence { get; private set; }

		public int CurrentRepeatableIndex { get; set; }

		public IEnumerable<GameMenuOption> MenuOptions
		{
			get
			{
				return this._menuItems;
			}
		}

		internal GameMenu(string idString)
		{
			this.StringId = idString;
			this._menuItems = new List<GameMenuOption>();
		}

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

		private void AddOption(GameMenuOption newOption, int index = -1)
		{
			if (index >= 0 && this._menuItems.Count >= index)
			{
				this._menuItems.Insert(index, newOption);
				return;
			}
			this._menuItems.Add(newOption);
		}

		public bool GetMenuOptionConditionsHold(Game game, MenuContext menuContext, int menuItemNumber)
		{
			if (this.IsWaitMenu)
			{
				return this._menuItems[menuItemNumber].GetConditionsHold(game, menuContext) && this.RunWaitMenuCondition(menuContext);
			}
			return this._menuItems[menuItemNumber].GetConditionsHold(game, menuContext);
		}

		public TextObject GetMenuOptionText(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].Text;
		}

		public GameMenuOption GetGameMenuOption(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber];
		}

		public TextObject GetMenuOptionText2(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].Text2;
		}

		public string GetMenuOptionIdString(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].IdString;
		}

		public TextObject GetMenuOptionTooltip(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].Tooltip;
		}

		public bool GetMenuOptionIsLeave(int menuItemNumber)
		{
			return this._menuItems[menuItemNumber].IsLeave;
		}

		public void SetProgressOfWaitingInMenu(float progress)
		{
			this.Progress = progress;
		}

		public void SetTargetedWaitingTimeAndInitialProgress(float targetedWaitingTime, float initialProgress)
		{
			this.TargetWaitHours = targetedWaitingTime;
			this.SetProgressOfWaitingInMenu(initialProgress);
		}

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

		public void RunWaitMenuConsequence(MenuContext menuContext)
		{
			if (this.OnConsequence != null)
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.MenuTitle);
				this.OnConsequence(menuCallbackArgs);
			}
		}

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

		public void StartWait()
		{
			this._previousTickTime = CampaignTime.Now;
			this.IsWaitActive = true;
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForward;
		}

		public void EndWait()
		{
			this.IsWaitActive = false;
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
		}

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

		public void PreInit(MenuContext menuContext)
		{
			MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.MenuTitle);
			CampaignEventDispatcher.Instance.BeforeGameMenuOpened(menuCallbackArgs);
		}

		public void AfterInit(MenuContext menuContext)
		{
			MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(menuContext, this.MenuTitle);
			CampaignEventDispatcher.Instance.AfterGameMenuOpened(menuCallbackArgs);
		}

		public TextObject GetText()
		{
			return this._defaultText;
		}

		public bool AutoSelectFirst { get; private set; }

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

		public static void ExitToLast()
		{
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			Campaign.Current.GameMenuManager.ExitToLast();
		}

		internal void AddOption(string optionId, TextObject optionText, GameMenuOption.OnConditionDelegate condition, GameMenuOption.OnConsequenceDelegate consequence, int index = -1, bool isLeave = false, bool isRepeatable = false, object relatedObject = null)
		{
			this.AddOption(new GameMenuOption(GameMenu.MenuAndOptionType.RegularMenuOption, optionId, optionText, optionText, condition, consequence, isLeave, isRepeatable, relatedObject), index);
		}

		internal void RemoveMenuOption(GameMenuOption option)
		{
			this._menuItems.Remove(option);
		}

		private TextObject _defaultText;

		public OnInitDelegate OnInit;

		public List<object> MenuRepeatObjects = new List<object>();

		public object LastSelectedMenuObject;

		private CampaignTime _previousTickTime;

		private readonly List<GameMenuOption> _menuItems;

		public enum MenuFlags
		{
			None,
			AutoSelectFirst
		}

		public enum MenuAndOptionType
		{
			RegularMenuOption,
			WaitMenuShowProgressAndHoursOption,
			WaitMenuShowOnlyProgressOption,
			WaitMenuHideProgressAndHoursOption
		}
	}
}
