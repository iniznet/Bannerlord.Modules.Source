using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar
{
	// Token: 0x02000050 RID: 80
	public class MapNavigationVM : ViewModel
	{
		// Token: 0x060005E5 RID: 1509 RVA: 0x0001C430 File Offset: 0x0001A630
		public MapNavigationVM(INavigationHandler navigationHandler, Func<MapBarShortcuts> getMapBarShortcuts)
		{
			this._navigationHandler = navigationHandler;
			this._getMapBarShortcuts = getMapBarShortcuts;
			IFaction mapFaction = Hero.MainHero.MapFaction;
			this.IsKingdomEnabled = mapFaction != null && mapFaction.IsKingdomFaction;
			this.IsPartyEnabled = true;
			this.IsInventoryEnabled = true;
			this.IsClanEnabled = true;
			this.IsCharacterDeveloperEnabled = true;
			this.IsQuestsEnabled = true;
			this.IsKingdomActive = false;
			this.IsPartyActive = false;
			this.IsInventoryActive = false;
			this.IsClanActive = false;
			this.IsCharacterDeveloperActive = false;
			this.IsQuestsActive = false;
			this.SkillsHint = new BasicTooltipViewModel();
			this.EscapeMenuHint = new BasicTooltipViewModel();
			this.QuestsHint = new BasicTooltipViewModel();
			this.InventoryHint = new BasicTooltipViewModel();
			this.PartyHint = new BasicTooltipViewModel();
			this.KingdomHint = new BasicTooltipViewModel();
			this.ClanHint = new BasicTooltipViewModel();
			this.CharacterAlertHint = new BasicTooltipViewModel();
			this.QuestAlertHint = new BasicTooltipViewModel();
			this.PartyAlertHint = new BasicTooltipViewModel();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			this.RefreshValues();
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x0001C54E File Offset: 0x0001A74E
		private void OnGamepadActiveStateChanged()
		{
			this.RefreshValues();
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x0001C558 File Offset: 0x0001A758
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._shortcuts = this._getMapBarShortcuts();
			this.EncyclopediaHint = new HintViewModel(GameTexts.FindText("str_encyclopedia", null), null);
			if (Input.IsGamepadActive)
			{
				this.SkillsHint.SetHintCallback(() => GameTexts.FindText("str_character", null).ToString());
				this.EscapeMenuHint.SetHintCallback(() => GameTexts.FindText("str_escape_menu", null).ToString());
				this.QuestsHint.SetHintCallback(() => GameTexts.FindText("str_quest", null).ToString());
				this.InventoryHint.SetHintCallback(() => GameTexts.FindText("str_inventory", null).ToString());
				this.PartyHint.SetHintCallback(() => GameTexts.FindText("str_party", null).ToString());
				this.KingdomHint.SetHintCallback(() => GameTexts.FindText("str_kingdom", null).ToString());
				this.ClanHint.SetHintCallback(() => GameTexts.FindText("str_clan", null).ToString());
			}
			else
			{
				this.SkillsHint.SetHintCallback(delegate
				{
					GameTexts.SetVariable("TEXT", GameTexts.FindText("str_character", null).ToString());
					GameTexts.SetVariable("HOTKEY", this._shortcuts.CharacterHotkey);
					return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
				});
				this.EscapeMenuHint.SetHintCallback(delegate
				{
					GameTexts.SetVariable("TEXT", GameTexts.FindText("str_escape_menu", null));
					GameTexts.SetVariable("HOTKEY", this._shortcuts.EscapeMenuHotkey);
					return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
				});
				this.QuestsHint.SetHintCallback(delegate
				{
					GameTexts.SetVariable("TEXT", GameTexts.FindText("str_quest", null).ToString());
					GameTexts.SetVariable("HOTKEY", this._shortcuts.QuestHotkey);
					return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
				});
				this.InventoryHint.SetHintCallback(delegate
				{
					GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory", null).ToString());
					GameTexts.SetVariable("HOTKEY", this._shortcuts.InventoryHotkey);
					return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
				});
				this.PartyHint.SetHintCallback(delegate
				{
					GameTexts.SetVariable("TEXT", GameTexts.FindText("str_party", null).ToString());
					GameTexts.SetVariable("HOTKEY", this._shortcuts.PartyHotkey);
					return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
				});
			}
			this.CampHint = new HintViewModel(GameTexts.FindText("str_camp", null), null);
			this.FinanceHint = new HintViewModel(GameTexts.FindText("str_finance", null), null);
			this.CenterCameraHint = new HintViewModel(GameTexts.FindText("str_return_to_hero", null), null);
			IFaction mapFaction = Hero.MainHero.MapFaction;
			if (mapFaction != null && mapFaction.IsKingdomFaction)
			{
				this.KingdomHint.SetHintCallback(() => GameTexts.FindText("str_kingdom", null).ToString());
			}
			else
			{
				this.KingdomHint.SetHintCallback(() => GameTexts.FindText("str_need_to_be_a_part_of_kingdom", null).ToString());
			}
			this.AlertText = GameTexts.FindText("str_map_bar_alert", null).ToString();
			this.CharacterAlertHint.SetHintCallback(() => PlayerUpdateTracker.Current.GetCharacterNotificationText());
			this.QuestAlertHint.SetHintCallback(() => PlayerUpdateTracker.Current.GetQuestNotificationText());
			this.PartyAlertHint.SetHintCallback(() => PlayerUpdateTracker.Current.GetPartyNotificationText());
			this.Refresh();
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x0001C885 File Offset: 0x0001AA85
		public override void OnFinalize()
		{
			base.OnFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			this._navigationHandler = null;
			this._getMapBarShortcuts = null;
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x0001C8BB File Offset: 0x0001AABB
		public void Refresh()
		{
			this.RefreshAlertValues();
			PlayerUpdateTracker.Current.UpdatePartyNotification();
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x0001C8CD File Offset: 0x0001AACD
		public void Tick()
		{
			this.RefreshPermissionValues();
			this.RefreshStates();
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x0001C8DC File Offset: 0x0001AADC
		private void RefreshPermissionValues()
		{
			NavigationPermissionItem kingdomPermission = this._navigationHandler.KingdomPermission;
			this.IsKingdomEnabled = kingdomPermission.IsAuthorized;
			NavigationPermissionItem clanPermission = this._navigationHandler.ClanPermission;
			this.IsClanEnabled = clanPermission.IsAuthorized;
			bool flag = false;
			bool flag2 = false;
			if (this._latestIsKingdomEnabled != this.IsKingdomEnabled)
			{
				flag = true;
				this._latestIsKingdomEnabled = this.IsKingdomEnabled;
			}
			if (this._latestIsClanEnabled != this.IsClanEnabled)
			{
				flag2 = true;
				this._latestIsClanEnabled = this.IsClanEnabled;
			}
			if (this._latestIsGamepadActive != Input.IsGamepadActive)
			{
				flag = true;
				flag2 = true;
				this._latestIsGamepadActive = Input.IsGamepadActive;
			}
			if (flag2)
			{
				this.UpdateClanHint(clanPermission);
			}
			if (flag)
			{
				this.UpdateKingdomHint(kingdomPermission);
			}
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x0001C988 File Offset: 0x0001AB88
		private void UpdateKingdomHint(NavigationPermissionItem kingdomPermission)
		{
			if (!this.IsKingdomEnabled)
			{
				this.KingdomHint.SetHintCallback(delegate
				{
					TextObject reasonString = kingdomPermission.ReasonString;
					if (reasonString == null)
					{
						return null;
					}
					return reasonString.ToString();
				});
				return;
			}
			if (Input.IsGamepadActive)
			{
				this.KingdomHint.SetHintCallback(() => GameTexts.FindText("str_kingdom", null).ToString());
				return;
			}
			this.KingdomHint.SetHintCallback(delegate
			{
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_kingdom", null).ToString());
				GameTexts.SetVariable("HOTKEY", this._shortcuts.KingdomHotkey);
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x0001CA14 File Offset: 0x0001AC14
		private void UpdateClanHint(NavigationPermissionItem clanPermission)
		{
			if (!this.IsClanEnabled)
			{
				this.ClanHint.SetHintCallback(delegate
				{
					TextObject reasonString = clanPermission.ReasonString;
					if (reasonString == null)
					{
						return null;
					}
					return reasonString.ToString();
				});
				return;
			}
			if (Input.IsGamepadActive)
			{
				this.ClanHint.SetHintCallback(() => GameTexts.FindText("str_clan", null).ToString());
				return;
			}
			this.ClanHint.SetHintCallback(delegate
			{
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_clan", null).ToString());
				GameTexts.SetVariable("HOTKEY", this._shortcuts.ClanHotkey);
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x0001CAA0 File Offset: 0x0001ACA0
		private void RefreshAlertValues()
		{
			this.QuestsAlert = PlayerUpdateTracker.Current.IsQuestNotificationActive;
			this.SkillAlert = PlayerUpdateTracker.Current.IsCharacterNotificationActive;
			this.PartyAlert = PlayerUpdateTracker.Current.IsPartyNotificationActive;
			this.KingdomAlert = PlayerUpdateTracker.Current.IsKingdomNotificationActive;
			this.ClanAlert = PlayerUpdateTracker.Current.IsClanNotificationActive;
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x0001CB00 File Offset: 0x0001AD00
		private void RefreshStates()
		{
			this.IsPartyEnabled = this._navigationHandler.PartyEnabled;
			this.IsInventoryEnabled = this._navigationHandler.InventoryEnabled;
			this.IsCharacterDeveloperEnabled = this._navigationHandler.CharacterDeveloperEnabled;
			this.IsQuestsEnabled = this._navigationHandler.QuestsEnabled;
			this.IsEscapeMenuEnabled = this._navigationHandler.EscapeMenuEnabled;
			this.IsKingdomActive = this._navigationHandler.KingdomActive;
			this.IsPartyActive = this._navigationHandler.PartyActive;
			this.IsInventoryActive = this._navigationHandler.InventoryActive;
			this.IsClanActive = this._navigationHandler.ClanActive;
			this.IsCharacterDeveloperActive = this._navigationHandler.CharacterDeveloperActive;
			this.IsQuestsActive = this._navigationHandler.QuestsActive;
			this.IsEscapeMenuActive = this._navigationHandler.EscapeMenuActive;
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0001CBD9 File Offset: 0x0001ADD9
		public void ExecuteOpenQuests()
		{
			this._navigationHandler.OpenQuests();
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0001CBE6 File Offset: 0x0001ADE6
		public void ExecuteOpenInventory()
		{
			this._navigationHandler.OpenInventory();
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x0001CBF3 File Offset: 0x0001ADF3
		public void ExecuteOpenParty()
		{
			this._navigationHandler.OpenParty();
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x0001CC00 File Offset: 0x0001AE00
		public void ExecuteOpenCharacterDeveloper()
		{
			this._navigationHandler.OpenCharacterDeveloper();
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x0001CC0D File Offset: 0x0001AE0D
		public void ExecuteOpenKingdom()
		{
			this._navigationHandler.OpenKingdom();
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0001CC1A File Offset: 0x0001AE1A
		public void ExecuteOpenClan()
		{
			this._navigationHandler.OpenClan();
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x0001CC27 File Offset: 0x0001AE27
		public void ExecuteOpenEscapeMenu()
		{
			this._navigationHandler.OpenEscapeMenu();
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x0001CC34 File Offset: 0x0001AE34
		public void ExecuteOpenMainHeroEncyclopedia()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.EncyclopediaLink);
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x0001CC4F File Offset: 0x0001AE4F
		public void ExecuteOpenMainHeroClanEncyclopedia()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.Clan.EncyclopediaLink);
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0001CC6F File Offset: 0x0001AE6F
		public void ExecuteOpenMainHeroKingdomEncyclopedia()
		{
			if (Hero.MainHero.MapFaction != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.MapFaction.EncyclopediaLink);
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060005FA RID: 1530 RVA: 0x0001CC9B File Offset: 0x0001AE9B
		// (set) Token: 0x060005FB RID: 1531 RVA: 0x0001CCA3 File Offset: 0x0001AEA3
		[DataSourceProperty]
		public BasicTooltipViewModel PartyAlertHint
		{
			get
			{
				return this._partyAlertHint;
			}
			set
			{
				if (value != this._partyAlertHint)
				{
					this._partyAlertHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "PartyAlertHint");
				}
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x0001CCC1 File Offset: 0x0001AEC1
		// (set) Token: 0x060005FD RID: 1533 RVA: 0x0001CCC9 File Offset: 0x0001AEC9
		[DataSourceProperty]
		public BasicTooltipViewModel CharacterAlertHint
		{
			get
			{
				return this._characterAlertHint;
			}
			set
			{
				if (value != this._characterAlertHint)
				{
					this._characterAlertHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "CharacterAlertHint");
				}
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060005FE RID: 1534 RVA: 0x0001CCE7 File Offset: 0x0001AEE7
		// (set) Token: 0x060005FF RID: 1535 RVA: 0x0001CCEF File Offset: 0x0001AEEF
		[DataSourceProperty]
		public BasicTooltipViewModel QuestAlertHint
		{
			get
			{
				return this._questAlertHint;
			}
			set
			{
				if (value != this._questAlertHint)
				{
					this._questAlertHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "QuestAlertHint");
				}
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000600 RID: 1536 RVA: 0x0001CD0D File Offset: 0x0001AF0D
		// (set) Token: 0x06000601 RID: 1537 RVA: 0x0001CD15 File Offset: 0x0001AF15
		[DataSourceProperty]
		public string AlertText
		{
			get
			{
				return this._alertText;
			}
			set
			{
				if (value != this._alertText)
				{
					this._alertText = value;
					base.OnPropertyChangedWithValue<string>(value, "AlertText");
				}
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000602 RID: 1538 RVA: 0x0001CD38 File Offset: 0x0001AF38
		// (set) Token: 0x06000603 RID: 1539 RVA: 0x0001CD40 File Offset: 0x0001AF40
		[DataSourceProperty]
		public bool SkillAlert
		{
			get
			{
				return this._skillAlert;
			}
			set
			{
				if (value != this._skillAlert)
				{
					this._skillAlert = value;
					base.OnPropertyChangedWithValue(value, "SkillAlert");
				}
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000604 RID: 1540 RVA: 0x0001CD5E File Offset: 0x0001AF5E
		// (set) Token: 0x06000605 RID: 1541 RVA: 0x0001CD66 File Offset: 0x0001AF66
		[DataSourceProperty]
		public bool QuestsAlert
		{
			get
			{
				return this._questsAlert;
			}
			set
			{
				if (value != this._questsAlert)
				{
					this._questsAlert = value;
					base.OnPropertyChangedWithValue(value, "QuestsAlert");
				}
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x0001CD84 File Offset: 0x0001AF84
		// (set) Token: 0x06000607 RID: 1543 RVA: 0x0001CD8C File Offset: 0x0001AF8C
		[DataSourceProperty]
		public bool PartyAlert
		{
			get
			{
				return this._partyAlert;
			}
			set
			{
				if (value != this._partyAlert)
				{
					this._partyAlert = value;
					base.OnPropertyChangedWithValue(value, "PartyAlert");
				}
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x0001CDAA File Offset: 0x0001AFAA
		// (set) Token: 0x06000609 RID: 1545 RVA: 0x0001CDB2 File Offset: 0x0001AFB2
		[DataSourceProperty]
		public bool KingdomAlert
		{
			get
			{
				return this._kingdomAlert;
			}
			set
			{
				if (value != this._kingdomAlert)
				{
					this._kingdomAlert = value;
					base.OnPropertyChangedWithValue(value, "KingdomAlert");
				}
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x0001CDD0 File Offset: 0x0001AFD0
		// (set) Token: 0x0600060B RID: 1547 RVA: 0x0001CDD8 File Offset: 0x0001AFD8
		[DataSourceProperty]
		public bool ClanAlert
		{
			get
			{
				return this._clanAlert;
			}
			set
			{
				if (value != this._clanAlert)
				{
					this._clanAlert = value;
					base.OnPropertyChangedWithValue(value, "ClanAlert");
				}
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x0001CDF6 File Offset: 0x0001AFF6
		// (set) Token: 0x0600060D RID: 1549 RVA: 0x0001CDFE File Offset: 0x0001AFFE
		[DataSourceProperty]
		public bool InventoryAlert
		{
			get
			{
				return this._inventoryAlert;
			}
			set
			{
				if (value != this._inventoryAlert)
				{
					this._inventoryAlert = value;
					base.OnPropertyChangedWithValue(value, "InventoryAlert");
				}
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x0001CE1C File Offset: 0x0001B01C
		// (set) Token: 0x0600060F RID: 1551 RVA: 0x0001CE24 File Offset: 0x0001B024
		[DataSourceProperty]
		public bool IsEscapeMenuEnabled
		{
			get
			{
				return this._isEscapeMenuEnabled;
			}
			set
			{
				if (value != this._isEscapeMenuEnabled)
				{
					this._isEscapeMenuEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEscapeMenuEnabled");
				}
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x0001CE42 File Offset: 0x0001B042
		// (set) Token: 0x06000611 RID: 1553 RVA: 0x0001CE4A File Offset: 0x0001B04A
		[DataSourceProperty]
		public bool IsKingdomEnabled
		{
			get
			{
				return this._isKingdomEnabled;
			}
			set
			{
				if (value != this._isKingdomEnabled)
				{
					this._isKingdomEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsKingdomEnabled");
				}
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x0001CE68 File Offset: 0x0001B068
		// (set) Token: 0x06000613 RID: 1555 RVA: 0x0001CE70 File Offset: 0x0001B070
		[DataSourceProperty]
		public bool IsPartyEnabled
		{
			get
			{
				return this._isPartyEnabled;
			}
			set
			{
				if (value != this._isPartyEnabled)
				{
					this._isPartyEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsPartyEnabled");
				}
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x0001CE8E File Offset: 0x0001B08E
		// (set) Token: 0x06000615 RID: 1557 RVA: 0x0001CE96 File Offset: 0x0001B096
		[DataSourceProperty]
		public bool IsInventoryEnabled
		{
			get
			{
				return this._isInventoryEnabled;
			}
			set
			{
				if (value != this._isInventoryEnabled)
				{
					this._isInventoryEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsInventoryEnabled");
				}
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x0001CEB4 File Offset: 0x0001B0B4
		// (set) Token: 0x06000617 RID: 1559 RVA: 0x0001CEBC File Offset: 0x0001B0BC
		[DataSourceProperty]
		public bool IsQuestsEnabled
		{
			get
			{
				return this._isQuestsEnabled;
			}
			set
			{
				if (value != this._isQuestsEnabled)
				{
					this._isQuestsEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsQuestsEnabled");
				}
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x0001CEDA File Offset: 0x0001B0DA
		// (set) Token: 0x06000619 RID: 1561 RVA: 0x0001CEE2 File Offset: 0x0001B0E2
		[DataSourceProperty]
		public bool IsCharacterDeveloperEnabled
		{
			get
			{
				return this._isCharacterDeveloperEnabled;
			}
			set
			{
				if (value != this._isCharacterDeveloperEnabled)
				{
					this._isCharacterDeveloperEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCharacterDeveloperEnabled");
				}
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x0001CF00 File Offset: 0x0001B100
		// (set) Token: 0x0600061B RID: 1563 RVA: 0x0001CF08 File Offset: 0x0001B108
		[DataSourceProperty]
		public bool IsClanEnabled
		{
			get
			{
				return this._isClanEnabled;
			}
			set
			{
				if (value != this._isClanEnabled)
				{
					this._isClanEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsClanEnabled");
				}
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x0600061C RID: 1564 RVA: 0x0001CF26 File Offset: 0x0001B126
		// (set) Token: 0x0600061D RID: 1565 RVA: 0x0001CF2E File Offset: 0x0001B12E
		[DataSourceProperty]
		public bool IsKingdomActive
		{
			get
			{
				return this._isKingdomActive;
			}
			set
			{
				if (value != this._isKingdomActive)
				{
					this._isKingdomActive = value;
					base.OnPropertyChangedWithValue(value, "IsKingdomActive");
				}
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x0001CF4C File Offset: 0x0001B14C
		// (set) Token: 0x0600061F RID: 1567 RVA: 0x0001CF54 File Offset: 0x0001B154
		[DataSourceProperty]
		public bool IsPartyActive
		{
			get
			{
				return this._isPartyActive;
			}
			set
			{
				if (value != this._isPartyActive)
				{
					this._isPartyActive = value;
					base.OnPropertyChangedWithValue(value, "IsPartyActive");
				}
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000620 RID: 1568 RVA: 0x0001CF72 File Offset: 0x0001B172
		// (set) Token: 0x06000621 RID: 1569 RVA: 0x0001CF7A File Offset: 0x0001B17A
		[DataSourceProperty]
		public bool IsInventoryActive
		{
			get
			{
				return this._isInventoryActive;
			}
			set
			{
				if (value != this._isInventoryActive)
				{
					this._isInventoryActive = value;
					base.OnPropertyChangedWithValue(value, "IsInventoryActive");
				}
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x0001CF98 File Offset: 0x0001B198
		// (set) Token: 0x06000623 RID: 1571 RVA: 0x0001CFA0 File Offset: 0x0001B1A0
		[DataSourceProperty]
		public bool IsQuestsActive
		{
			get
			{
				return this._isQuestsActive;
			}
			set
			{
				if (value != this._isQuestsActive)
				{
					this._isQuestsActive = value;
					base.OnPropertyChangedWithValue(value, "IsQuestsActive");
				}
			}
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000624 RID: 1572 RVA: 0x0001CFBE File Offset: 0x0001B1BE
		// (set) Token: 0x06000625 RID: 1573 RVA: 0x0001CFC6 File Offset: 0x0001B1C6
		[DataSourceProperty]
		public bool IsCharacterDeveloperActive
		{
			get
			{
				return this._isCharacterDeveloperActive;
			}
			set
			{
				if (value != this._isCharacterDeveloperActive)
				{
					this._isCharacterDeveloperActive = value;
					base.OnPropertyChangedWithValue(value, "IsCharacterDeveloperActive");
				}
			}
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000626 RID: 1574 RVA: 0x0001CFE4 File Offset: 0x0001B1E4
		// (set) Token: 0x06000627 RID: 1575 RVA: 0x0001CFEC File Offset: 0x0001B1EC
		[DataSourceProperty]
		public bool IsClanActive
		{
			get
			{
				return this._isClanActive;
			}
			set
			{
				if (value != this._isClanActive)
				{
					this._isClanActive = value;
					base.OnPropertyChangedWithValue(value, "IsClanActive");
				}
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000628 RID: 1576 RVA: 0x0001D00A File Offset: 0x0001B20A
		// (set) Token: 0x06000629 RID: 1577 RVA: 0x0001D012 File Offset: 0x0001B212
		[DataSourceProperty]
		public bool IsEscapeMenuActive
		{
			get
			{
				return this._isEscapeMenuActive;
			}
			set
			{
				if (value != this._isEscapeMenuActive)
				{
					this._isEscapeMenuActive = value;
					base.OnPropertyChangedWithValue(value, "IsEscapeMenuActive");
				}
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x0001D030 File Offset: 0x0001B230
		// (set) Token: 0x0600062B RID: 1579 RVA: 0x0001D038 File Offset: 0x0001B238
		[DataSourceProperty]
		public HintViewModel FinanceHint
		{
			get
			{
				return this._financeHint;
			}
			set
			{
				if (value != this._financeHint)
				{
					this._financeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FinanceHint");
				}
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x0600062C RID: 1580 RVA: 0x0001D056 File Offset: 0x0001B256
		// (set) Token: 0x0600062D RID: 1581 RVA: 0x0001D05E File Offset: 0x0001B25E
		[DataSourceProperty]
		public HintViewModel EncyclopediaHint
		{
			get
			{
				return this._encyclopediaHint;
			}
			set
			{
				if (value != this._encyclopediaHint)
				{
					this._encyclopediaHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EncyclopediaHint");
				}
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x0001D07C File Offset: 0x0001B27C
		// (set) Token: 0x0600062F RID: 1583 RVA: 0x0001D084 File Offset: 0x0001B284
		[DataSourceProperty]
		public BasicTooltipViewModel EscapeMenuHint
		{
			get
			{
				return this._escapeMenuHint;
			}
			set
			{
				if (value != this._escapeMenuHint)
				{
					this._escapeMenuHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "EscapeMenuHint");
				}
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x0001D0A2 File Offset: 0x0001B2A2
		// (set) Token: 0x06000631 RID: 1585 RVA: 0x0001D0AA File Offset: 0x0001B2AA
		[DataSourceProperty]
		public BasicTooltipViewModel SkillsHint
		{
			get
			{
				return this._skillsHint;
			}
			set
			{
				if (value != this._skillsHint)
				{
					this._skillsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SkillsHint");
				}
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000632 RID: 1586 RVA: 0x0001D0C8 File Offset: 0x0001B2C8
		// (set) Token: 0x06000633 RID: 1587 RVA: 0x0001D0D0 File Offset: 0x0001B2D0
		[DataSourceProperty]
		public BasicTooltipViewModel QuestsHint
		{
			get
			{
				return this._questsHint;
			}
			set
			{
				if (value != this._questsHint)
				{
					this._questsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "QuestsHint");
				}
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000634 RID: 1588 RVA: 0x0001D0EE File Offset: 0x0001B2EE
		// (set) Token: 0x06000635 RID: 1589 RVA: 0x0001D0F6 File Offset: 0x0001B2F6
		[DataSourceProperty]
		public BasicTooltipViewModel InventoryHint
		{
			get
			{
				return this._inventoryHint;
			}
			set
			{
				if (value != this._inventoryHint)
				{
					this._inventoryHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "InventoryHint");
				}
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000636 RID: 1590 RVA: 0x0001D114 File Offset: 0x0001B314
		// (set) Token: 0x06000637 RID: 1591 RVA: 0x0001D11C File Offset: 0x0001B31C
		[DataSourceProperty]
		public BasicTooltipViewModel PartyHint
		{
			get
			{
				return this._partyHint;
			}
			set
			{
				if (value != this._partyHint)
				{
					this._partyHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "PartyHint");
				}
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000638 RID: 1592 RVA: 0x0001D13A File Offset: 0x0001B33A
		// (set) Token: 0x06000639 RID: 1593 RVA: 0x0001D142 File Offset: 0x0001B342
		[DataSourceProperty]
		public BasicTooltipViewModel KingdomHint
		{
			get
			{
				return this._kingdomHint;
			}
			set
			{
				if (value != this._kingdomHint)
				{
					this._kingdomHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "KingdomHint");
				}
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600063A RID: 1594 RVA: 0x0001D160 File Offset: 0x0001B360
		// (set) Token: 0x0600063B RID: 1595 RVA: 0x0001D168 File Offset: 0x0001B368
		[DataSourceProperty]
		public BasicTooltipViewModel ClanHint
		{
			get
			{
				return this._clanHint;
			}
			set
			{
				if (value != this._clanHint)
				{
					this._clanHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ClanHint");
				}
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x0001D186 File Offset: 0x0001B386
		// (set) Token: 0x0600063D RID: 1597 RVA: 0x0001D18E File Offset: 0x0001B38E
		[DataSourceProperty]
		public HintViewModel CenterCameraHint
		{
			get
			{
				return this._centerCameraHint;
			}
			set
			{
				if (value != this._centerCameraHint)
				{
					this._centerCameraHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CenterCameraHint");
				}
			}
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x0001D1AC File Offset: 0x0001B3AC
		// (set) Token: 0x0600063F RID: 1599 RVA: 0x0001D1B4 File Offset: 0x0001B3B4
		[DataSourceProperty]
		public HintViewModel CampHint
		{
			get
			{
				return this._campHint;
			}
			set
			{
				if (value != this._campHint)
				{
					this._campHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CampHint");
				}
			}
		}

		// Token: 0x0400029B RID: 667
		private INavigationHandler _navigationHandler;

		// Token: 0x0400029C RID: 668
		private Func<MapBarShortcuts> _getMapBarShortcuts;

		// Token: 0x0400029D RID: 669
		private MapBarShortcuts _shortcuts;

		// Token: 0x0400029E RID: 670
		private bool _latestIsGamepadActive;

		// Token: 0x0400029F RID: 671
		private bool _latestIsKingdomEnabled;

		// Token: 0x040002A0 RID: 672
		private bool _latestIsClanEnabled;

		// Token: 0x040002A1 RID: 673
		private string _alertText;

		// Token: 0x040002A2 RID: 674
		private bool _skillAlert;

		// Token: 0x040002A3 RID: 675
		private bool _questsAlert;

		// Token: 0x040002A4 RID: 676
		private bool _partyAlert;

		// Token: 0x040002A5 RID: 677
		private bool _kingdomAlert;

		// Token: 0x040002A6 RID: 678
		private bool _clanAlert;

		// Token: 0x040002A7 RID: 679
		private bool _inventoryAlert;

		// Token: 0x040002A8 RID: 680
		private bool _isKingdomEnabled;

		// Token: 0x040002A9 RID: 681
		private bool _isClanEnabled;

		// Token: 0x040002AA RID: 682
		private bool _isQuestsEnabled;

		// Token: 0x040002AB RID: 683
		private bool _isEscapeMenuEnabled;

		// Token: 0x040002AC RID: 684
		private bool _isInventoryEnabled;

		// Token: 0x040002AD RID: 685
		private bool _isCharacterDeveloperEnabled;

		// Token: 0x040002AE RID: 686
		private bool _isPartyEnabled;

		// Token: 0x040002AF RID: 687
		private bool _isKingdomActive;

		// Token: 0x040002B0 RID: 688
		private bool _isClanActive;

		// Token: 0x040002B1 RID: 689
		private bool _isEscapeMenuActive;

		// Token: 0x040002B2 RID: 690
		private bool _isQuestsActive;

		// Token: 0x040002B3 RID: 691
		private bool _isInventoryActive;

		// Token: 0x040002B4 RID: 692
		private bool _isCharacterDeveloperActive;

		// Token: 0x040002B5 RID: 693
		private bool _isPartyActive;

		// Token: 0x040002B6 RID: 694
		private HintViewModel _encyclopediaHint;

		// Token: 0x040002B7 RID: 695
		private BasicTooltipViewModel _skillsHint;

		// Token: 0x040002B8 RID: 696
		private BasicTooltipViewModel _escapeMenuHint;

		// Token: 0x040002B9 RID: 697
		private BasicTooltipViewModel _questsHint;

		// Token: 0x040002BA RID: 698
		private BasicTooltipViewModel _inventoryHint;

		// Token: 0x040002BB RID: 699
		private BasicTooltipViewModel _partyHint;

		// Token: 0x040002BC RID: 700
		private HintViewModel _financeHint;

		// Token: 0x040002BD RID: 701
		private HintViewModel _centerCameraHint;

		// Token: 0x040002BE RID: 702
		private HintViewModel _campHint;

		// Token: 0x040002BF RID: 703
		private BasicTooltipViewModel _kingdomHint;

		// Token: 0x040002C0 RID: 704
		private BasicTooltipViewModel _clanHint;

		// Token: 0x040002C1 RID: 705
		private BasicTooltipViewModel _characterAlertHint;

		// Token: 0x040002C2 RID: 706
		private BasicTooltipViewModel _questAlertHint;

		// Token: 0x040002C3 RID: 707
		private BasicTooltipViewModel _partyAlertHint;
	}
}
