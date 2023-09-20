using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar
{
	public class MapNavigationVM : ViewModel
	{
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

		private void OnGamepadActiveStateChanged()
		{
			this.RefreshValues();
		}

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

		public override void OnFinalize()
		{
			base.OnFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			this._navigationHandler = null;
			this._getMapBarShortcuts = null;
		}

		public void Refresh()
		{
			this.RefreshAlertValues();
			PlayerUpdateTracker.Current.UpdatePartyNotification();
		}

		public void Tick()
		{
			this.RefreshPermissionValues();
			this.RefreshStates();
		}

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

		private void RefreshAlertValues()
		{
			this.QuestsAlert = PlayerUpdateTracker.Current.IsQuestNotificationActive;
			this.SkillAlert = PlayerUpdateTracker.Current.IsCharacterNotificationActive;
			this.PartyAlert = PlayerUpdateTracker.Current.IsPartyNotificationActive;
			this.KingdomAlert = PlayerUpdateTracker.Current.IsKingdomNotificationActive;
			this.ClanAlert = PlayerUpdateTracker.Current.IsClanNotificationActive;
		}

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

		public void ExecuteOpenQuests()
		{
			this._navigationHandler.OpenQuests();
		}

		public void ExecuteOpenInventory()
		{
			this._navigationHandler.OpenInventory();
		}

		public void ExecuteOpenParty()
		{
			this._navigationHandler.OpenParty();
		}

		public void ExecuteOpenCharacterDeveloper()
		{
			this._navigationHandler.OpenCharacterDeveloper();
		}

		public void ExecuteOpenKingdom()
		{
			this._navigationHandler.OpenKingdom();
		}

		public void ExecuteOpenClan()
		{
			this._navigationHandler.OpenClan();
		}

		public void ExecuteOpenEscapeMenu()
		{
			this._navigationHandler.OpenEscapeMenu();
		}

		public void ExecuteOpenMainHeroEncyclopedia()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.EncyclopediaLink);
		}

		public void ExecuteOpenMainHeroClanEncyclopedia()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.Clan.EncyclopediaLink);
		}

		public void ExecuteOpenMainHeroKingdomEncyclopedia()
		{
			if (Hero.MainHero.MapFaction != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.MapFaction.EncyclopediaLink);
			}
		}

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

		private INavigationHandler _navigationHandler;

		private Func<MapBarShortcuts> _getMapBarShortcuts;

		private MapBarShortcuts _shortcuts;

		private bool _latestIsGamepadActive;

		private bool _latestIsKingdomEnabled;

		private bool _latestIsClanEnabled;

		private string _alertText;

		private bool _skillAlert;

		private bool _questsAlert;

		private bool _partyAlert;

		private bool _kingdomAlert;

		private bool _clanAlert;

		private bool _inventoryAlert;

		private bool _isKingdomEnabled;

		private bool _isClanEnabled;

		private bool _isQuestsEnabled;

		private bool _isEscapeMenuEnabled;

		private bool _isInventoryEnabled;

		private bool _isCharacterDeveloperEnabled;

		private bool _isPartyEnabled;

		private bool _isKingdomActive;

		private bool _isClanActive;

		private bool _isEscapeMenuActive;

		private bool _isQuestsActive;

		private bool _isInventoryActive;

		private bool _isCharacterDeveloperActive;

		private bool _isPartyActive;

		private HintViewModel _encyclopediaHint;

		private BasicTooltipViewModel _skillsHint;

		private BasicTooltipViewModel _escapeMenuHint;

		private BasicTooltipViewModel _questsHint;

		private BasicTooltipViewModel _inventoryHint;

		private BasicTooltipViewModel _partyHint;

		private HintViewModel _financeHint;

		private HintViewModel _centerCameraHint;

		private HintViewModel _campHint;

		private BasicTooltipViewModel _kingdomHint;

		private BasicTooltipViewModel _clanHint;

		private BasicTooltipViewModel _characterAlertHint;

		private BasicTooltipViewModel _questAlertHint;

		private BasicTooltipViewModel _partyAlertHint;
	}
}
