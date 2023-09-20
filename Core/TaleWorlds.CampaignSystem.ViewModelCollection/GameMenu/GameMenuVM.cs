using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu
{
	public class GameMenuVM : ViewModel
	{
		public bool IsInspected
		{
			set
			{
				if (this._isInspected == value)
				{
					return;
				}
				this._isInspected = value;
			}
		}

		public MenuContext MenuContext { get; private set; }

		public GameMenuVM(MenuContext menuContext)
		{
			this.MenuContext = menuContext;
			this._gameMenuManager = Campaign.Current.GameMenuManager;
			this.ItemList = new MBBindingList<GameMenuItemVM>();
			this.ProgressItemList = new MBBindingList<GameMenuItemProgressVM>();
			this._shortcutKeys = new Dictionary<GameMenuOption.LeaveType, GameKey>();
			this._menuTextAttributeStrings = new Dictionary<string, string>();
			this._menuTextAttributes = new Dictionary<string, object>();
			this.Background = menuContext.CurrentBackgroundMeshName;
			this.IsInSiegeMode = PlayerSiege.PlayerSiegeEvent != null;
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ItemList.ApplyActionOnAllItems(delegate(GameMenuItemVM x)
			{
				x.RefreshValues();
			});
			this.ProgressItemList.ApplyActionOnAllItems(delegate(GameMenuItemProgressVM x)
			{
				x.RefreshValues();
			});
			this.Refresh(true);
		}

		public void Refresh(bool forceUpdateItems)
		{
			TextObject menuTitle = this.MenuContext.GameMenu.MenuTitle;
			this.TitleText = ((menuTitle != null) ? menuTitle.ToString() : null);
			GameMenu gameMenu = this.MenuContext.GameMenu;
			this.IsEncounterMenu = gameMenu != null && gameMenu.OverlayType == GameOverlays.MenuOverlayType.Encounter;
			this.Background = (string.IsNullOrEmpty(this.MenuContext.CurrentBackgroundMeshName) ? "wait_guards_stop" : this.MenuContext.CurrentBackgroundMeshName);
			if (forceUpdateItems)
			{
				this.ItemList.Clear();
				this.ProgressItemList.Clear();
				int virtualMenuOptionAmount = this._gameMenuManager.GetVirtualMenuOptionAmount(this.MenuContext);
				for (int i = 0; i < virtualMenuOptionAmount; i++)
				{
					this._gameMenuManager.SetCurrentRepeatableIndex(this.MenuContext, i);
					if (this._gameMenuManager.GetVirtualMenuOptionConditionsHold(this.MenuContext, i))
					{
						TextObject textObject;
						TextObject textObject2;
						if (this._gameMenuManager.GetVirtualGameMenuOption(this.MenuContext, i).IsRepeatable)
						{
							textObject = new TextObject(this._gameMenuManager.GetVirtualMenuOptionText(this.MenuContext, i).ToString(), null);
							textObject2 = new TextObject(this._gameMenuManager.GetVirtualMenuOptionText2(this.MenuContext, i).ToString(), null);
						}
						else
						{
							textObject = this._gameMenuManager.GetVirtualMenuOptionText(this.MenuContext, i);
							textObject2 = this._gameMenuManager.GetVirtualMenuOptionText2(this.MenuContext, i);
						}
						TextObject virtualMenuOptionTooltip = this._gameMenuManager.GetVirtualMenuOptionTooltip(this.MenuContext, i);
						TextObject textObject3 = textObject;
						TextObject textObject4 = textObject2;
						TextObject textObject5 = virtualMenuOptionTooltip;
						GameMenu.MenuAndOptionType virtualMenuAndOptionType = this._gameMenuManager.GetVirtualMenuAndOptionType(this.MenuContext);
						GameMenuOption virtualGameMenuOption = this._gameMenuManager.GetVirtualGameMenuOption(this.MenuContext, i);
						GameKey gameKey = (this._shortcutKeys.ContainsKey(virtualGameMenuOption.OptionLeaveType) ? this._shortcutKeys[virtualGameMenuOption.OptionLeaveType] : null);
						GameMenuItemVM gameMenuItemVM = new GameMenuItemVM(this.MenuContext, i, textObject3, (textObject4 == TextObject.Empty) ? textObject3 : textObject4, textObject5, virtualMenuAndOptionType, virtualGameMenuOption, gameKey);
						if (!string.IsNullOrEmpty(this._latestTutorialElementID))
						{
							gameMenuItemVM.IsHighlightEnabled = gameMenuItemVM.OptionID == this._latestTutorialElementID;
						}
						this.ItemList.Add(gameMenuItemVM);
						if (virtualMenuAndOptionType == GameMenu.MenuAndOptionType.WaitMenuShowOnlyProgressOption || virtualMenuAndOptionType == GameMenu.MenuAndOptionType.WaitMenuShowProgressAndHoursOption)
						{
							this.ProgressItemList.Add(new GameMenuItemProgressVM(this.MenuContext, i));
						}
					}
				}
			}
			MobileParty mainParty = MobileParty.MainParty;
			if (((mainParty != null) ? mainParty.MapEvent : null) != null)
			{
				int[] array = new int[2];
				foreach (PartyBase partyBase in MobileParty.MainParty.MapEvent.InvolvedParties)
				{
					BattleSideEnum side = partyBase.Side;
					BattleSideEnum side2 = PartyBase.MainParty.Side;
					if (partyBase.Side != BattleSideEnum.None)
					{
						array[(int)partyBase.Side] += partyBase.NumberOfHealthyMembers;
					}
				}
				if (MobileParty.MainParty.MapEvent.IsRaid && !this._plunderEventRegistered)
				{
					this.PlunderItems = new MBBindingList<GameMenuPlunderItemVM>();
					CampaignEvents.ItemsLooted.AddNonSerializedListener(this, new Action<MobileParty, ItemRoster>(this.OnItemsPlundered));
					this._plunderEventRegistered = true;
				}
			}
			else if (this._plunderEventRegistered)
			{
				CampaignEvents.ItemsLooted.ClearListeners(this);
				this._plunderEventRegistered = false;
				MBBindingList<GameMenuPlunderItemVM> plunderItems = this.PlunderItems;
				if (plunderItems != null)
				{
					plunderItems.Clear();
				}
			}
			this._requireContextTextUpdate = true;
		}

		public void OnFrameTick()
		{
			this.IsInSiegeMode = PlayerSiege.PlayerSiegeEvent != null;
			if (this._requireContextTextUpdate)
			{
				this._menuText = this._gameMenuManager.GetMenuText(this.MenuContext);
				this.ContextText = this._menuText.ToString();
				this._menuTextAttributes.Clear();
				this._menuTextAttributeStrings.Clear();
				TextObject menuText = this._menuText;
				if (((menuText != null) ? menuText.Attributes : null) != null)
				{
					foreach (KeyValuePair<string, object> keyValuePair in this._menuText.Attributes)
					{
						this._menuTextAttributes[keyValuePair.Key] = keyValuePair.Value;
						this._menuTextAttributeStrings[keyValuePair.Key] = keyValuePair.Value.ToString();
					}
				}
				this._requireContextTextUpdate = false;
			}
			foreach (GameMenuItemVM gameMenuItemVM in this.ItemList)
			{
				gameMenuItemVM.Refresh();
			}
			foreach (GameMenuItemProgressVM gameMenuItemProgressVM in this.ProgressItemList)
			{
				gameMenuItemProgressVM.OnTick();
			}
			if (Campaign.Current.GameMode == CampaignGameMode.Campaign)
			{
				this.IsNight = Campaign.Current.IsNight;
			}
			this._requireContextTextUpdate = this.IsMenuTextChanged();
		}

		private bool IsMenuTextChanged()
		{
			GameMenuManager gameMenuManager = this._gameMenuManager;
			TextObject textObject = ((gameMenuManager != null) ? gameMenuManager.GetMenuText(this.MenuContext) : null);
			if (this._menuText != textObject)
			{
				return true;
			}
			int count = this._menuTextAttributes.Count;
			TextObject menuText = this._menuText;
			int? num;
			if (menuText == null)
			{
				num = null;
			}
			else
			{
				Dictionary<string, object> attributes = menuText.Attributes;
				num = ((attributes != null) ? new int?(attributes.Count) : null);
			}
			int? num2 = num;
			if (!((count == num2.GetValueOrDefault()) & (num2 != null)))
			{
				return true;
			}
			foreach (string text in this._menuTextAttributes.Keys)
			{
				object obj = null;
				object obj2 = this._menuTextAttributes[text];
				TextObject menuText2 = this._menuText;
				if (menuText2 == null || !menuText2.Attributes.TryGetValue(text, out obj))
				{
					return true;
				}
				if (obj2 != obj)
				{
					return true;
				}
				if (this._menuTextAttributeStrings[text] != obj.ToString())
				{
					return true;
				}
			}
			return false;
		}

		public void UpdateMenuContext(MenuContext newMenuContext)
		{
			this.MenuContext = newMenuContext;
			this.Refresh(true);
		}

		public override void OnFinalize()
		{
			CampaignEvents.ItemsLooted.ClearListeners(this);
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this._gameMenuManager = null;
			this.MenuContext = null;
			this.ItemList.ApplyActionOnAllItems(delegate(GameMenuItemVM x)
			{
				x.OnFinalize();
			});
			this.ItemList.Clear();
			this.ItemList = null;
		}

		public void AddHotKey(GameMenuOption.LeaveType leaveType, GameKey gameKey)
		{
			if (this._shortcutKeys.ContainsKey(leaveType))
			{
				this._shortcutKeys[leaveType] = gameKey;
				return;
			}
			this._shortcutKeys.Add(leaveType, gameKey);
		}

		private void OnItemsPlundered(MobileParty mobileParty, ItemRoster newItems)
		{
			if (mobileParty == MobileParty.MainParty)
			{
				foreach (ItemRosterElement itemRosterElement in newItems)
				{
					for (int i = 0; i < itemRosterElement.Amount; i++)
					{
						this.PlunderItems.Add(new GameMenuPlunderItemVM(itemRosterElement));
					}
				}
			}
		}

		public void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		protected virtual GameMenuItemVM CreateGameMenuItemVM(int indexOfMenuCondition)
		{
			GameMenuOption virtualGameMenuOption = this._gameMenuManager.GetVirtualGameMenuOption(this.MenuContext, indexOfMenuCondition);
			GameKey gameKey = (this._shortcutKeys.ContainsKey(virtualGameMenuOption.OptionLeaveType) ? this._shortcutKeys[virtualGameMenuOption.OptionLeaveType] : null);
			return new GameMenuItemVM(this.MenuContext, indexOfMenuCondition, TextObject.Empty, TextObject.Empty, TextObject.Empty, GameMenu.MenuAndOptionType.RegularMenuOption, virtualGameMenuOption, gameKey);
		}

		[DataSourceProperty]
		public bool IsNight
		{
			get
			{
				return this._isNight;
			}
			set
			{
				if (value != this._isNight)
				{
					this._isNight = value;
					base.OnPropertyChangedWithValue(value, "IsNight");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInSiegeMode
		{
			get
			{
				return this._isInSiegeMode;
			}
			set
			{
				if (value != this._isInSiegeMode)
				{
					this._isInSiegeMode = value;
					base.OnPropertyChangedWithValue(value, "IsInSiegeMode");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEncounterMenu
		{
			get
			{
				return this._isEncounterMenu;
			}
			set
			{
				if (value != this._isEncounterMenu)
				{
					this._isEncounterMenu = value;
					base.OnPropertyChangedWithValue(value, "IsEncounterMenu");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string ContextText
		{
			get
			{
				return this._contextText;
			}
			set
			{
				if (value != this._contextText)
				{
					this._contextText = value;
					base.OnPropertyChangedWithValue<string>(value, "ContextText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<GameMenuItemVM> ItemList
		{
			get
			{
				return this._itemList;
			}
			set
			{
				if (value != this._itemList)
				{
					this._itemList = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameMenuItemVM>>(value, "ItemList");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<GameMenuItemProgressVM> ProgressItemList
		{
			get
			{
				return this._progressItemList;
			}
			set
			{
				if (value != this._progressItemList)
				{
					this._progressItemList = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameMenuItemProgressVM>>(value, "ProgressItemList");
				}
			}
		}

		[DataSourceProperty]
		public string Background
		{
			get
			{
				return this._background;
			}
			set
			{
				if (value != this._background)
				{
					this._background = value;
					base.OnPropertyChangedWithValue<string>(value, "Background");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<GameMenuPlunderItemVM> PlunderItems
		{
			get
			{
				return this._plunderItems;
			}
			set
			{
				if (value != this._plunderItems)
				{
					this._plunderItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameMenuPlunderItemVM>>(value, "PlunderItems");
				}
			}
		}

		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				this._latestTutorialElementID = obj.NewNotificationElementID;
			}
			if (this._latestTutorialElementID != null)
			{
				if (this._latestTutorialElementID != string.Empty)
				{
					if (this._latestTutorialElementID == "town_backstreet" && !this._isTavernButtonHighlightApplied)
					{
						this._isTavernButtonHighlightApplied = this.SetGameMenuButtonHighlightState(this._latestTutorialElementID, true);
					}
					else if (this._latestTutorialElementID != "town_backstreet" && this._isTavernButtonHighlightApplied)
					{
						this._isTavernButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("town_backstreet", false);
					}
					if (this._latestTutorialElementID == "sell_all_prisoners" && !this._isSellPrisonerButtonHighlightApplied)
					{
						this._isSellPrisonerButtonHighlightApplied = this.SetGameMenuButtonHighlightState(this._latestTutorialElementID, true);
					}
					else if (this._latestTutorialElementID != "sell_all_prisoners" && this._isSellPrisonerButtonHighlightApplied)
					{
						this._isSellPrisonerButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("sell_all_prisoners", false);
					}
					if (this._latestTutorialElementID == "storymode_tutorial_village_buy" && !this._isShopButtonHighlightApplied)
					{
						this._isShopButtonHighlightApplied = this.SetGameMenuButtonHighlightState(this._latestTutorialElementID, true);
					}
					else if (this._latestTutorialElementID != "storymode_tutorial_village_buy" && this._isShopButtonHighlightApplied)
					{
						this._isShopButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("storymode_tutorial_village_buy", false);
					}
					if (this._latestTutorialElementID == "storymode_tutorial_village_recruit" && !this._isRecruitButtonHighlightApplied)
					{
						this._isRecruitButtonHighlightApplied = this.SetGameMenuButtonHighlightState(this._latestTutorialElementID, true);
					}
					else if (this._latestTutorialElementID != "storymode_tutorial_village_recruit" && this._isRecruitButtonHighlightApplied)
					{
						this._isRecruitButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("storymode_tutorial_village_recruit", false);
					}
					if (this._latestTutorialElementID == "hostile_action" && !this._isHostileActionButtonHighlightApplied)
					{
						this._isHostileActionButtonHighlightApplied = this.SetGameMenuButtonHighlightState(this._latestTutorialElementID, true);
					}
					else if (this._latestTutorialElementID != "hostile_action" && this._isHostileActionButtonHighlightApplied)
					{
						this._isHostileActionButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("hostile_action", false);
					}
					if (this._latestTutorialElementID == "town_besiege" && !this._isTownBesiegeButtonHighlightApplied)
					{
						this._isTownBesiegeButtonHighlightApplied = this.SetGameMenuButtonHighlightState(this._latestTutorialElementID, true);
					}
					else if (this._latestTutorialElementID != "town_besiege" && this._isTownBesiegeButtonHighlightApplied)
					{
						this._isTownBesiegeButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("town_besiege", false);
					}
					if (this._latestTutorialElementID == "storymode_tutorial_village_enter" && !this._isEnterTutorialVillageButtonHighlightApplied)
					{
						this._isEnterTutorialVillageButtonHighlightApplied = this.SetGameMenuButtonHighlightState(this._latestTutorialElementID, true);
						return;
					}
					if (this._latestTutorialElementID != "storymode_tutorial_village_enter" && this._isEnterTutorialVillageButtonHighlightApplied)
					{
						this._isEnterTutorialVillageButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("storymode_tutorial_village_enter", false);
						return;
					}
				}
				else
				{
					if (this._isTavernButtonHighlightApplied)
					{
						this._isTavernButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("town_backstreet", false);
					}
					if (this._isSellPrisonerButtonHighlightApplied)
					{
						this._isSellPrisonerButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("sell_all_prisoners", false);
					}
					if (this._isShopButtonHighlightApplied)
					{
						this._isShopButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("storymode_tutorial_village_buy", false);
					}
					if (this._isRecruitButtonHighlightApplied)
					{
						this._isRecruitButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("storymode_tutorial_village_recruit", false);
					}
					if (this._isHostileActionButtonHighlightApplied)
					{
						this._isHostileActionButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("hostile_action", false);
					}
					if (this._isTownBesiegeButtonHighlightApplied)
					{
						this._isTownBesiegeButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("town_besiege", false);
					}
					if (this._isEnterTutorialVillageButtonHighlightApplied)
					{
						this._isEnterTutorialVillageButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("storymode_tutorial_village_enter", false);
						return;
					}
				}
			}
			else
			{
				if (this._isTavernButtonHighlightApplied)
				{
					this._isTavernButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("town_backstreet", false);
				}
				if (this._isSellPrisonerButtonHighlightApplied)
				{
					this._isSellPrisonerButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("sell_all_prisoners", false);
				}
				if (this._isShopButtonHighlightApplied)
				{
					this._isShopButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("storymode_tutorial_village_buy", false);
				}
				if (this._isRecruitButtonHighlightApplied)
				{
					this._isRecruitButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("storymode_tutorial_village_recruit", false);
				}
				if (this._isHostileActionButtonHighlightApplied)
				{
					this._isHostileActionButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("hostile_action", false);
				}
				if (this._isTownBesiegeButtonHighlightApplied)
				{
					this._isTownBesiegeButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("town_besiege", false);
				}
				if (this._isEnterTutorialVillageButtonHighlightApplied)
				{
					this._isEnterTutorialVillageButtonHighlightApplied = !this.SetGameMenuButtonHighlightState("storymode_tutorial_village_enter", false);
				}
			}
		}

		private bool SetGameMenuButtonHighlightState(string buttonID, bool state)
		{
			foreach (GameMenuItemVM gameMenuItemVM in this.ItemList)
			{
				if (gameMenuItemVM.OptionID == buttonID)
				{
					gameMenuItemVM.IsHighlightEnabled = state;
					return true;
				}
			}
			return false;
		}

		private bool _isInspected;

		private bool _plunderEventRegistered;

		private GameMenuManager _gameMenuManager;

		private Dictionary<GameMenuOption.LeaveType, GameKey> _shortcutKeys;

		private Dictionary<string, string> _menuTextAttributeStrings;

		private Dictionary<string, object> _menuTextAttributes;

		private TextObject _menuText = TextObject.Empty;

		private MBBindingList<GameMenuItemVM> _itemList;

		private MBBindingList<GameMenuItemProgressVM> _progressItemList;

		private string _titleText;

		private string _contextText;

		private string _background;

		private bool _isNight;

		private bool _isInSiegeMode;

		private bool _isEncounterMenu;

		private MBBindingList<GameMenuPlunderItemVM> _plunderItems;

		private string _latestTutorialElementID;

		private bool _isTavernButtonHighlightApplied;

		private bool _isSellPrisonerButtonHighlightApplied;

		private bool _isShopButtonHighlightApplied;

		private bool _isRecruitButtonHighlightApplied;

		private bool _isHostileActionButtonHighlightApplied;

		private bool _isTownBesiegeButtonHighlightApplied;

		private bool _isEnterTutorialVillageButtonHighlightApplied;

		private bool _requireContextTextUpdate;
	}
}
