using System;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar
{
	public class MapBarVM : ViewModel
	{
		public MapBarVM(INavigationHandler navigationHandler, IMapStateHandler mapStateHandler, Func<MapBarShortcuts> getMapBarShortcuts, Action openArmyManagement)
		{
			this._openArmyManagement = openArmyManagement;
			this._mapStateHandler = mapStateHandler;
			this._refreshTimeSpan = ((Campaign.Current.GetSimplifiedTimeControlMode() == CampaignTimeControlMode.UnstoppableFastForward) ? 0.1f : 2f);
			this._needToBePartOfKingdomText = GameTexts.FindText("str_need_to_be_a_part_of_kingdom", null);
			this._cannotGatherWhileInEventText = GameTexts.FindText("str_cannot_gather_army_while_in_event", null);
			this._needToBeLeaderToManageText = GameTexts.FindText("str_need_to_be_leader_of_army_to_manage", null);
			this._mercenaryCannotManageText = GameTexts.FindText("str_mercenary_cannot_manage_army", null);
			this._cannotGatherWhileInConversationText = GameTexts.FindText("str_cannot_gather_army_during_conversation", null);
			this._cannotGatherWhileInSiegeText = GameTexts.FindText("str_cannot_gather_army_during_siege", null);
			this.TutorialNotification = new ElementNotificationVM();
			this.MapInfo = new MapInfoVM();
			this.MapTimeControl = new MapTimeControlVM(getMapBarShortcuts, new Action(this.OnTimeControlChange), delegate
			{
				mapStateHandler.ResetCamera(false, false);
			});
			this.MapNavigation = new MapNavigationVM(navigationHandler, getMapBarShortcuts);
			this.GatherArmyHint = new HintViewModel();
			this.OnRefresh();
			this.IsEnabled = true;
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.MapInfo.RefreshValues();
			this.MapTimeControl.RefreshValues();
			this.MapNavigation.RefreshValues();
		}

		public void OnRefresh()
		{
			this.MapInfo.Refresh();
			this.MapTimeControl.Refresh();
			this.MapNavigation.Refresh();
		}

		public void Tick(float dt)
		{
			int simplifiedTimeControlMode = (int)Campaign.Current.GetSimplifiedTimeControlMode();
			this._refreshTimeSpan -= dt;
			if (this._refreshTimeSpan < 0f)
			{
				this.OnRefresh();
				this._refreshTimeSpan = ((simplifiedTimeControlMode == 2) ? 0.1f : 0.2f);
			}
			this.MapInfo.Tick();
			this.MapTimeControl.Tick();
			this.MapNavigation.Tick();
			if (this._mapStateHandler != null)
			{
				this.IsCameraCentered = this._mapStateHandler.IsCameraLockedToPlayerParty();
			}
			this.IsGatherArmyVisible = this.GetIsGatherArmyVisible();
			if (this.IsGatherArmyVisible)
			{
				this.UpdateCanGatherArmyAndReason();
			}
		}

		private void UpdateCanGatherArmyAndReason()
		{
			bool flag = true;
			TextObject textObject = null;
			IFaction mapFaction = Hero.MainHero.MapFaction;
			if (mapFaction != null && !mapFaction.IsKingdomFaction)
			{
				textObject = this._needToBePartOfKingdomText;
				flag = false;
			}
			else if (MobileParty.MainParty.MapEvent != null)
			{
				textObject = this._cannotGatherWhileInEventText;
				flag = false;
			}
			else if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty)
			{
				textObject = this._needToBeLeaderToManageText;
				flag = false;
			}
			else if (Clan.PlayerClan.IsUnderMercenaryService)
			{
				textObject = this._mercenaryCannotManageText;
				flag = false;
			}
			else if (PlayerEncounter.Current != null && PlayerEncounter.EncounterSettlement == null)
			{
				textObject = this._cannotGatherWhileInConversationText;
				flag = false;
			}
			else if (PlayerSiege.PlayerSiegeEvent != null)
			{
				textObject = this._cannotGatherWhileInSiegeText;
				flag = false;
			}
			this.CanGatherArmy = flag;
			this.GatherArmyHint.HintText = (this.CanGatherArmy ? TextObject.Empty : textObject);
		}

		private bool GetIsGatherArmyVisible()
		{
			if (this.MapTimeControl.IsInMap)
			{
				MobileParty mainParty = MobileParty.MainParty;
				if (((mainParty != null) ? mainParty.Army : null) == null && !Hero.MainHero.IsPrisoner && Hero.MainHero.PartyBelongedTo != null && MobileParty.MainParty.MapEvent == null)
				{
					return this.MapTimeControl.IsCenterPanelEnabled;
				}
			}
			return false;
		}

		private void OnTimeControlChange()
		{
			this._refreshTimeSpan = ((Campaign.Current.GetSimplifiedTimeControlMode() == CampaignTimeControlMode.UnstoppableFastForward) ? 0.1f : 2f);
		}

		private void ExecuteResetCamera()
		{
			IMapStateHandler mapStateHandler = this._mapStateHandler;
			if (mapStateHandler == null)
			{
				return;
			}
			mapStateHandler.FastMoveCameraToMainParty();
		}

		public void ExecuteArmyManagement()
		{
			this._openArmyManagement();
		}

		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = string.Empty;
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = this._latestTutorialElementID;
					if (this._latestTutorialElementID == "PartySpeedLabel" && !this.MapInfo.IsInfoBarExtended)
					{
						this.MapInfo.IsInfoBarExtended = true;
					}
				}
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this._mapStateHandler = null;
			MapNavigationVM mapNavigation = this._mapNavigation;
			if (mapNavigation != null)
			{
				mapNavigation.OnFinalize();
			}
			MapTimeControlVM mapTimeControl = this._mapTimeControl;
			if (mapTimeControl != null)
			{
				mapTimeControl.OnFinalize();
			}
			this._mapInfo = null;
			this._mapNavigation = null;
			this._mapTimeControl = null;
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			EventManager eventManager = game.EventManager;
			if (eventManager == null)
			{
				return;
			}
			eventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		[DataSourceProperty]
		public MapInfoVM MapInfo
		{
			get
			{
				return this._mapInfo;
			}
			set
			{
				if (value != this._mapInfo)
				{
					this._mapInfo = value;
					base.OnPropertyChangedWithValue<MapInfoVM>(value, "MapInfo");
				}
			}
		}

		[DataSourceProperty]
		public MapTimeControlVM MapTimeControl
		{
			get
			{
				return this._mapTimeControl;
			}
			set
			{
				if (value != this._mapTimeControl)
				{
					this._mapTimeControl = value;
					base.OnPropertyChangedWithValue<MapTimeControlVM>(value, "MapTimeControl");
				}
			}
		}

		[DataSourceProperty]
		public MapNavigationVM MapNavigation
		{
			get
			{
				return this._mapNavigation;
			}
			set
			{
				if (value != this._mapNavigation)
				{
					this._mapNavigation = value;
					base.OnPropertyChangedWithValue<MapNavigationVM>(value, "MapNavigation");
				}
			}
		}

		[DataSourceProperty]
		public bool IsGatherArmyVisible
		{
			get
			{
				return this._isGatherArmyVisible;
			}
			set
			{
				if (value != this._isGatherArmyVisible)
				{
					this._isGatherArmyVisible = value;
					base.OnPropertyChangedWithValue(value, "IsGatherArmyVisible");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInInfoMode
		{
			get
			{
				return this._isInInfoMode;
			}
			set
			{
				if (value != this._isInInfoMode)
				{
					this._isInInfoMode = value;
					base.OnPropertyChangedWithValue(value, "IsInInfoMode");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool CanGatherArmy
		{
			get
			{
				return this._canGatherArmy;
			}
			set
			{
				if (value != this._canGatherArmy)
				{
					this._canGatherArmy = value;
					base.OnPropertyChangedWithValue(value, "CanGatherArmy");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel GatherArmyHint
		{
			get
			{
				return this._gatherArmyHint;
			}
			set
			{
				if (value != this._gatherArmyHint)
				{
					this._gatherArmyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "GatherArmyHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCameraCentered
		{
			get
			{
				return this._isCameraCentered;
			}
			set
			{
				if (value != this._isCameraCentered)
				{
					this._isCameraCentered = value;
					base.OnPropertyChangedWithValue(value, "IsCameraCentered");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentScreen
		{
			get
			{
				return this._currentScreen;
			}
			set
			{
				if (this._currentScreen != value)
				{
					this._currentScreen = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentScreen");
				}
			}
		}

		[DataSourceProperty]
		public ElementNotificationVM TutorialNotification
		{
			get
			{
				return this._tutorialNotification;
			}
			set
			{
				if (value != this._tutorialNotification)
				{
					this._tutorialNotification = value;
					base.OnPropertyChangedWithValue<ElementNotificationVM>(value, "TutorialNotification");
				}
			}
		}

		private IMapStateHandler _mapStateHandler;

		private readonly TextObject _needToBePartOfKingdomText;

		private readonly TextObject _cannotGatherWhileInEventText;

		private readonly TextObject _needToBeLeaderToManageText;

		private readonly TextObject _mercenaryCannotManageText;

		private readonly TextObject _cannotGatherWhileInConversationText;

		private readonly TextObject _cannotGatherWhileInSiegeText;

		private readonly Action _openArmyManagement;

		private float _refreshTimeSpan;

		private string _latestTutorialElementID;

		private bool _isGatherArmyVisible;

		private MapInfoVM _mapInfo;

		private MapTimeControlVM _mapTimeControl;

		private MapNavigationVM _mapNavigation;

		private bool _isEnabled;

		private bool _isCameraCentered;

		private bool _canGatherArmy;

		private bool _isInInfoMode;

		private string _currentScreen;

		private HintViewModel _gatherArmyHint;

		private ElementNotificationVM _tutorialNotification;
	}
}
