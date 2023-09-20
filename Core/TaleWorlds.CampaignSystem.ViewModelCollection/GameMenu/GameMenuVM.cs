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
	// Token: 0x02000088 RID: 136
	public class GameMenuVM : ViewModel
	{
		// Token: 0x17000453 RID: 1107
		// (set) Token: 0x06000D53 RID: 3411 RVA: 0x00035CE3 File Offset: 0x00033EE3
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

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06000D54 RID: 3412 RVA: 0x00035CF6 File Offset: 0x00033EF6
		// (set) Token: 0x06000D55 RID: 3413 RVA: 0x00035CFE File Offset: 0x00033EFE
		public MenuContext MenuContext { get; private set; }

		// Token: 0x06000D56 RID: 3414 RVA: 0x00035D08 File Offset: 0x00033F08
		public GameMenuVM(MenuContext menuContext)
		{
			this.MenuContext = menuContext;
			this._gameMenuManager = Campaign.Current.GameMenuManager;
			this.ItemList = new MBBindingList<GameMenuItemVM>();
			this.ProgressItemList = new MBBindingList<GameMenuItemProgressVM>();
			this._shortcutKeys = new Dictionary<GameMenuOption.LeaveType, GameKey>();
			this.Background = menuContext.CurrentBackgroundMeshName;
			this.IsInSiegeMode = PlayerSiege.PlayerSiegeEvent != null;
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x00035D94 File Offset: 0x00033F94
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

		// Token: 0x06000D58 RID: 3416 RVA: 0x00035E04 File Offset: 0x00034004
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

		// Token: 0x06000D59 RID: 3417 RVA: 0x0003615C File Offset: 0x0003435C
		public void OnFrameTick()
		{
			this.IsInSiegeMode = PlayerSiege.PlayerSiegeEvent != null;
			if (this._requireContextTextUpdate)
			{
				this._menuText = this._gameMenuManager.GetMenuText(this.MenuContext);
				this._menuTextAttributes = ((this._menuText.Attributes == null) ? null : new Dictionary<string, object>(this._menuText.Attributes));
				this.ContextText = this._menuText.ToString();
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
			this.CheckMenuTextChanged();
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x00036270 File Offset: 0x00034470
		private void CheckMenuTextChanged()
		{
			if (this._menuTextAttributes != null && this._menuText.Attributes != null)
			{
				if (this._menuTextAttributes.Count != this._menuText.Attributes.Count)
				{
					this._requireContextTextUpdate = true;
					return;
				}
				using (Dictionary<string, object>.KeyCollection.Enumerator enumerator = this._menuTextAttributes.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						if (!this._menuText.Attributes.ContainsKey(text))
						{
							this._requireContextTextUpdate = true;
							break;
						}
						object obj = this._menuText.Attributes[text];
						object obj2 = this._menuTextAttributes[text];
						TextObject textObject;
						TextObject textObject2;
						if ((textObject = obj as TextObject) != null && (textObject2 = obj2 as TextObject) != null)
						{
							if (!textObject.Equals(textObject2))
							{
								this._requireContextTextUpdate = true;
								break;
							}
						}
						else if (!obj.Equals(obj2))
						{
							this._requireContextTextUpdate = true;
							break;
						}
					}
					return;
				}
			}
			this._requireContextTextUpdate = this._menuTextAttributes != this._menuText.Attributes;
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x00036398 File Offset: 0x00034598
		public void UpdateMenuContext(MenuContext newMenuContext)
		{
			this.MenuContext = newMenuContext;
			this.Refresh(true);
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x000363A8 File Offset: 0x000345A8
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

		// Token: 0x06000D5D RID: 3421 RVA: 0x00036425 File Offset: 0x00034625
		public void AddHotKey(GameMenuOption.LeaveType leaveType, GameKey gameKey)
		{
			if (this._shortcutKeys.ContainsKey(leaveType))
			{
				this._shortcutKeys[leaveType] = gameKey;
				return;
			}
			this._shortcutKeys.Add(leaveType, gameKey);
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x00036450 File Offset: 0x00034650
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

		// Token: 0x06000D5F RID: 3423 RVA: 0x000364BC File Offset: 0x000346BC
		public void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x000364D0 File Offset: 0x000346D0
		protected virtual GameMenuItemVM CreateGameMenuItemVM(int indexOfMenuCondition)
		{
			GameMenuOption virtualGameMenuOption = this._gameMenuManager.GetVirtualGameMenuOption(this.MenuContext, indexOfMenuCondition);
			GameKey gameKey = (this._shortcutKeys.ContainsKey(virtualGameMenuOption.OptionLeaveType) ? this._shortcutKeys[virtualGameMenuOption.OptionLeaveType] : null);
			return new GameMenuItemVM(this.MenuContext, indexOfMenuCondition, TextObject.Empty, TextObject.Empty, TextObject.Empty, GameMenu.MenuAndOptionType.RegularMenuOption, virtualGameMenuOption, gameKey);
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06000D61 RID: 3425 RVA: 0x00036536 File Offset: 0x00034736
		// (set) Token: 0x06000D62 RID: 3426 RVA: 0x0003653E File Offset: 0x0003473E
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

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06000D63 RID: 3427 RVA: 0x0003655C File Offset: 0x0003475C
		// (set) Token: 0x06000D64 RID: 3428 RVA: 0x00036564 File Offset: 0x00034764
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

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06000D65 RID: 3429 RVA: 0x00036582 File Offset: 0x00034782
		// (set) Token: 0x06000D66 RID: 3430 RVA: 0x0003658A File Offset: 0x0003478A
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

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06000D67 RID: 3431 RVA: 0x000365A8 File Offset: 0x000347A8
		// (set) Token: 0x06000D68 RID: 3432 RVA: 0x000365B0 File Offset: 0x000347B0
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

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06000D69 RID: 3433 RVA: 0x000365D3 File Offset: 0x000347D3
		// (set) Token: 0x06000D6A RID: 3434 RVA: 0x000365DB File Offset: 0x000347DB
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

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06000D6B RID: 3435 RVA: 0x000365FE File Offset: 0x000347FE
		// (set) Token: 0x06000D6C RID: 3436 RVA: 0x00036606 File Offset: 0x00034806
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

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06000D6D RID: 3437 RVA: 0x00036624 File Offset: 0x00034824
		// (set) Token: 0x06000D6E RID: 3438 RVA: 0x0003662C File Offset: 0x0003482C
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

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06000D6F RID: 3439 RVA: 0x0003664A File Offset: 0x0003484A
		// (set) Token: 0x06000D70 RID: 3440 RVA: 0x00036652 File Offset: 0x00034852
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

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06000D71 RID: 3441 RVA: 0x00036675 File Offset: 0x00034875
		// (set) Token: 0x06000D72 RID: 3442 RVA: 0x0003667D File Offset: 0x0003487D
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

		// Token: 0x06000D73 RID: 3443 RVA: 0x0003669C File Offset: 0x0003489C
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

		// Token: 0x06000D74 RID: 3444 RVA: 0x00036B1C File Offset: 0x00034D1C
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

		// Token: 0x04000626 RID: 1574
		private bool _isInspected;

		// Token: 0x04000627 RID: 1575
		private bool _plunderEventRegistered;

		// Token: 0x04000628 RID: 1576
		private GameMenuManager _gameMenuManager;

		// Token: 0x04000629 RID: 1577
		private Dictionary<GameMenuOption.LeaveType, GameKey> _shortcutKeys;

		// Token: 0x0400062A RID: 1578
		private Dictionary<string, object> _menuTextAttributes;

		// Token: 0x0400062B RID: 1579
		private TextObject _menuText = TextObject.Empty;

		// Token: 0x0400062D RID: 1581
		private MBBindingList<GameMenuItemVM> _itemList;

		// Token: 0x0400062E RID: 1582
		private MBBindingList<GameMenuItemProgressVM> _progressItemList;

		// Token: 0x0400062F RID: 1583
		private string _titleText;

		// Token: 0x04000630 RID: 1584
		private string _contextText;

		// Token: 0x04000631 RID: 1585
		private string _background;

		// Token: 0x04000632 RID: 1586
		private bool _isNight;

		// Token: 0x04000633 RID: 1587
		private bool _isInSiegeMode;

		// Token: 0x04000634 RID: 1588
		private bool _isEncounterMenu;

		// Token: 0x04000635 RID: 1589
		private MBBindingList<GameMenuPlunderItemVM> _plunderItems;

		// Token: 0x04000636 RID: 1590
		private string _latestTutorialElementID;

		// Token: 0x04000637 RID: 1591
		private bool _isTavernButtonHighlightApplied;

		// Token: 0x04000638 RID: 1592
		private bool _isSellPrisonerButtonHighlightApplied;

		// Token: 0x04000639 RID: 1593
		private bool _isShopButtonHighlightApplied;

		// Token: 0x0400063A RID: 1594
		private bool _isRecruitButtonHighlightApplied;

		// Token: 0x0400063B RID: 1595
		private bool _isHostileActionButtonHighlightApplied;

		// Token: 0x0400063C RID: 1596
		private bool _isTownBesiegeButtonHighlightApplied;

		// Token: 0x0400063D RID: 1597
		private bool _isEnterTutorialVillageButtonHighlightApplied;

		// Token: 0x0400063E RID: 1598
		private bool _requireContextTextUpdate;
	}
}
