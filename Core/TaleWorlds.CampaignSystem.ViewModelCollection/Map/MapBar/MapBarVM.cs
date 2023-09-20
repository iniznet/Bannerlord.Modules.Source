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
	// Token: 0x0200004E RID: 78
	public class MapBarVM : ViewModel
	{
		// Token: 0x0600057B RID: 1403 RVA: 0x0001B320 File Offset: 0x00019520
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

		// Token: 0x0600057C RID: 1404 RVA: 0x0001B453 File Offset: 0x00019653
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.MapInfo.RefreshValues();
			this.MapTimeControl.RefreshValues();
			this.MapNavigation.RefreshValues();
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x0001B47C File Offset: 0x0001967C
		public void OnRefresh()
		{
			this.MapInfo.Refresh();
			this.MapTimeControl.Refresh();
			this.MapNavigation.Refresh();
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x0001B4A0 File Offset: 0x000196A0
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

		// Token: 0x0600057F RID: 1407 RVA: 0x0001B544 File Offset: 0x00019744
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

		// Token: 0x06000580 RID: 1408 RVA: 0x0001B624 File Offset: 0x00019824
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

		// Token: 0x06000581 RID: 1409 RVA: 0x0001B682 File Offset: 0x00019882
		private void OnTimeControlChange()
		{
			this._refreshTimeSpan = ((Campaign.Current.GetSimplifiedTimeControlMode() == CampaignTimeControlMode.UnstoppableFastForward) ? 0.1f : 2f);
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x0001B6A3 File Offset: 0x000198A3
		private void ExecuteResetCamera()
		{
			IMapStateHandler mapStateHandler = this._mapStateHandler;
			if (mapStateHandler == null)
			{
				return;
			}
			mapStateHandler.FastMoveCameraToMainParty();
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x0001B6B5 File Offset: 0x000198B5
		public void ExecuteArmyManagement()
		{
			this._openArmyManagement();
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x0001B6C4 File Offset: 0x000198C4
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

		// Token: 0x06000585 RID: 1413 RVA: 0x0001B74C File Offset: 0x0001994C
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

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x0001B7C2 File Offset: 0x000199C2
		// (set) Token: 0x06000587 RID: 1415 RVA: 0x0001B7CA File Offset: 0x000199CA
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

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x0001B7E8 File Offset: 0x000199E8
		// (set) Token: 0x06000589 RID: 1417 RVA: 0x0001B7F0 File Offset: 0x000199F0
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

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x0600058A RID: 1418 RVA: 0x0001B80E File Offset: 0x00019A0E
		// (set) Token: 0x0600058B RID: 1419 RVA: 0x0001B816 File Offset: 0x00019A16
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

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x0600058C RID: 1420 RVA: 0x0001B834 File Offset: 0x00019A34
		// (set) Token: 0x0600058D RID: 1421 RVA: 0x0001B83C File Offset: 0x00019A3C
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

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x0001B85A File Offset: 0x00019A5A
		// (set) Token: 0x0600058F RID: 1423 RVA: 0x0001B862 File Offset: 0x00019A62
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

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000590 RID: 1424 RVA: 0x0001B880 File Offset: 0x00019A80
		// (set) Token: 0x06000591 RID: 1425 RVA: 0x0001B888 File Offset: 0x00019A88
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

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000592 RID: 1426 RVA: 0x0001B8A6 File Offset: 0x00019AA6
		// (set) Token: 0x06000593 RID: 1427 RVA: 0x0001B8AE File Offset: 0x00019AAE
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

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000594 RID: 1428 RVA: 0x0001B8CC File Offset: 0x00019ACC
		// (set) Token: 0x06000595 RID: 1429 RVA: 0x0001B8D4 File Offset: 0x00019AD4
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

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000596 RID: 1430 RVA: 0x0001B8F2 File Offset: 0x00019AF2
		// (set) Token: 0x06000597 RID: 1431 RVA: 0x0001B8FA File Offset: 0x00019AFA
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

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x0001B918 File Offset: 0x00019B18
		// (set) Token: 0x06000599 RID: 1433 RVA: 0x0001B920 File Offset: 0x00019B20
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

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x0001B943 File Offset: 0x00019B43
		// (set) Token: 0x0600059B RID: 1435 RVA: 0x0001B94B File Offset: 0x00019B4B
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

		// Token: 0x0400025F RID: 607
		private IMapStateHandler _mapStateHandler;

		// Token: 0x04000260 RID: 608
		private readonly TextObject _needToBePartOfKingdomText;

		// Token: 0x04000261 RID: 609
		private readonly TextObject _cannotGatherWhileInEventText;

		// Token: 0x04000262 RID: 610
		private readonly TextObject _needToBeLeaderToManageText;

		// Token: 0x04000263 RID: 611
		private readonly TextObject _mercenaryCannotManageText;

		// Token: 0x04000264 RID: 612
		private readonly TextObject _cannotGatherWhileInConversationText;

		// Token: 0x04000265 RID: 613
		private readonly TextObject _cannotGatherWhileInSiegeText;

		// Token: 0x04000266 RID: 614
		private readonly Action _openArmyManagement;

		// Token: 0x04000267 RID: 615
		private float _refreshTimeSpan;

		// Token: 0x04000268 RID: 616
		private string _latestTutorialElementID;

		// Token: 0x04000269 RID: 617
		private bool _isGatherArmyVisible;

		// Token: 0x0400026A RID: 618
		private MapInfoVM _mapInfo;

		// Token: 0x0400026B RID: 619
		private MapTimeControlVM _mapTimeControl;

		// Token: 0x0400026C RID: 620
		private MapNavigationVM _mapNavigation;

		// Token: 0x0400026D RID: 621
		private bool _isEnabled;

		// Token: 0x0400026E RID: 622
		private bool _isCameraCentered;

		// Token: 0x0400026F RID: 623
		private bool _canGatherArmy;

		// Token: 0x04000270 RID: 624
		private bool _isInInfoMode;

		// Token: 0x04000271 RID: 625
		private string _currentScreen;

		// Token: 0x04000272 RID: 626
		private HintViewModel _gatherArmyHint;

		// Token: 0x04000273 RID: 627
		private ElementNotificationVM _tutorialNotification;
	}
}
