using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x02000067 RID: 103
	public class MPLobbyBadgeSelectionPopupVM : ViewModel
	{
		// Token: 0x170002EA RID: 746
		// (get) Token: 0x0600097B RID: 2427 RVA: 0x000235DC File Offset: 0x000217DC
		// (set) Token: 0x0600097C RID: 2428 RVA: 0x000235E4 File Offset: 0x000217E4
		public List<LobbyNotification> ActiveNotifications { get; private set; }

		// Token: 0x0600097D RID: 2429 RVA: 0x000235ED File Offset: 0x000217ED
		public MPLobbyBadgeSelectionPopupVM(Action onBadgeNotificationRead, Action onBadgeSelectionUpdated, Action<MPLobbyAchievementBadgeGroupVM> onBadgeProgressInfoRequested)
		{
			this._onBadgeNotificationRead = onBadgeNotificationRead;
			this._onBadgeSelectionUpdated = onBadgeSelectionUpdated;
			this._onBadgeProgressInfoRequested = onBadgeProgressInfoRequested;
			this.ActiveNotifications = new List<LobbyNotification>();
			this.Badges = new MBBindingList<MPLobbyBadgeItemVM>();
			this.AchivementBadgeGroups = new MBBindingList<MPLobbyAchievementBadgeGroupVM>();
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x0002362C File Offset: 0x0002182C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CloseText = GameTexts.FindText("str_close", null).ToString();
			this.BadgesText = new TextObject("{=nqYaiEo2}My Badges", null).ToString();
			this.SpecialBadgesText = new TextObject("{=yI9EV0II}Special Badges", null).ToString();
			this.AchievementBadgesText = new TextObject("{=n6yb5VCI}Achievement Badges", null).ToString();
			this.AchivementBadgeGroups.ApplyActionOnAllItems(delegate(MPLobbyAchievementBadgeGroupVM g)
			{
				g.RefreshValues();
			});
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x000236C1 File Offset: 0x000218C1
		public void RefreshPlayerData(PlayerData playerData)
		{
			this.UpdateBadges(false);
			this.UpdateBadgeSelection();
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x000236D0 File Offset: 0x000218D0
		public void RefreshKeyBindings(HotKey inspectProgressKey)
		{
			this._inspectProgressKey = inspectProgressKey;
			foreach (MPLobbyAchievementBadgeGroupVM mplobbyAchievementBadgeGroupVM in this.AchivementBadgeGroups)
			{
				mplobbyAchievementBadgeGroupVM.RefreshKeyBindings(inspectProgressKey);
			}
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x00023724 File Offset: 0x00021924
		public async void UpdateBadges(bool shouldClear = false)
		{
			Badge[] array = await NetworkMain.GameClient.GetPlayerBadges();
			this._playerEarnedBadges = array;
			if (shouldClear)
			{
				this.Badges.Clear();
			}
			if (!this.Badges.Any((MPLobbyBadgeItemVM b) => b.Badge == null))
			{
				this.Badges.Add(new MPLobbyBadgeItemVM(null, new Action(this.UpdateBadgeSelection), (Badge b) => true, new Action<MPLobbyBadgeItemVM>(this.OnBadgeInspected)));
			}
			if (BadgeManager.Badges != null)
			{
				using (List<Badge>.Enumerator enumerator = BadgeManager.Badges.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Badge badge = enumerator.Current;
						if ((badge.IsActive && !badge.IsVisibleOnlyWhenEarned) || this._playerEarnedBadges.Contains(badge))
						{
							if (badge.GroupId != null)
							{
								MPLobbyAchievementBadgeGroupVM mplobbyAchievementBadgeGroupVM = this.AchivementBadgeGroups.FirstOrDefault((MPLobbyAchievementBadgeGroupVM g) => g.GroupID == badge.GroupId);
								if (mplobbyAchievementBadgeGroupVM == null)
								{
									mplobbyAchievementBadgeGroupVM = new MPLobbyAchievementBadgeGroupVM(badge.GroupId, this._onBadgeProgressInfoRequested);
									mplobbyAchievementBadgeGroupVM.RefreshKeyBindings(this._inspectProgressKey);
									this.AchivementBadgeGroups.Add(mplobbyAchievementBadgeGroupVM);
									mplobbyAchievementBadgeGroupVM.OnGroupBadgeAdded(new MPLobbyBadgeItemVM(badge, new Action(this.UpdateBadgeSelection), new Func<Badge, bool>(this.HasPlayerEarnedBadge), new Action<MPLobbyBadgeItemVM>(this.OnBadgeInspected)));
								}
								else
								{
									MPLobbyBadgeItemVM mplobbyBadgeItemVM = mplobbyAchievementBadgeGroupVM.Badges.FirstOrDefault((MPLobbyBadgeItemVM b) => b.Badge == badge);
									if (mplobbyBadgeItemVM == null)
									{
										mplobbyAchievementBadgeGroupVM.OnGroupBadgeAdded(new MPLobbyBadgeItemVM(badge, new Action(this.UpdateBadgeSelection), new Func<Badge, bool>(this.HasPlayerEarnedBadge), new Action<MPLobbyBadgeItemVM>(this.OnBadgeInspected)));
									}
									else
									{
										mplobbyBadgeItemVM.UpdateWith(badge);
									}
								}
							}
							else
							{
								MPLobbyBadgeItemVM mplobbyBadgeItemVM2 = this.Badges.SingleOrDefault((MPLobbyBadgeItemVM b) => b.Badge == badge);
								if (mplobbyBadgeItemVM2 == null)
								{
									this.Badges.Add(new MPLobbyBadgeItemVM(badge, new Action(this.UpdateBadgeSelection), new Func<Badge, bool>(this.HasPlayerEarnedBadge), new Action<MPLobbyBadgeItemVM>(this.OnBadgeInspected)));
								}
								else
								{
									mplobbyBadgeItemVM2.UpdateWith(badge);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x00023768 File Offset: 0x00021968
		public void UpdateBadgeSelection()
		{
			foreach (MPLobbyBadgeItemVM mplobbyBadgeItemVM in this.Badges)
			{
				mplobbyBadgeItemVM.UpdateIsSelected();
			}
			foreach (MPLobbyAchievementBadgeGroupVM mplobbyAchievementBadgeGroupVM in this.AchivementBadgeGroups)
			{
				mplobbyAchievementBadgeGroupVM.UpdateBadgeSelection();
			}
			Action onBadgeSelectionUpdated = this._onBadgeSelectionUpdated;
			if (onBadgeSelectionUpdated == null)
			{
				return;
			}
			onBadgeSelectionUpdated();
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x000237FC File Offset: 0x000219FC
		private bool HasPlayerEarnedBadge(Badge badge)
		{
			Badge[] playerEarnedBadges = this._playerEarnedBadges;
			return playerEarnedBadges != null && playerEarnedBadges.Contains(badge);
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x00023810 File Offset: 0x00021A10
		public void OnNotificationReceived(LobbyNotification notification)
		{
			string badgeID = notification.Parameters["badge_id"];
			IEnumerable<MPLobbyBadgeItemVM> badges = this.Badges;
			Func<MPLobbyBadgeItemVM, bool> <>9__0;
			Func<MPLobbyBadgeItemVM, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (MPLobbyBadgeItemVM badge) => badge.BadgeId == badgeID);
			}
			foreach (MPLobbyBadgeItemVM mplobbyBadgeItemVM in badges.Where(func))
			{
				mplobbyBadgeItemVM.HasNotification = true;
			}
			this.ActiveNotifications.Add(notification);
			this.RefreshNotificationInfo();
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x000238B0 File Offset: 0x00021AB0
		public void OnBadgeInspected(MPLobbyBadgeItemVM badge)
		{
			this.InspectedBadge = badge;
			if (badge != null)
			{
				string badgeID = badge.BadgeId;
				foreach (LobbyNotification lobbyNotification in this.ActiveNotifications.Where((LobbyNotification n) => n.Parameters["badge_id"] == badgeID))
				{
					NetworkMain.GameClient.MarkNotificationAsRead(lobbyNotification.Id);
				}
				this.ActiveNotifications.RemoveAll((LobbyNotification n) => n.Parameters["badge_id"] == badgeID);
				this.RefreshNotificationInfo();
				Action onBadgeNotificationRead = this._onBadgeNotificationRead;
				if (onBadgeNotificationRead == null)
				{
					return;
				}
				onBadgeNotificationRead();
			}
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x00023964 File Offset: 0x00021B64
		private void RefreshNotificationInfo()
		{
			this.HasNotifications = this.ActiveNotifications.Count > 0;
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x0002397A File Offset: 0x00021B7A
		public void Open()
		{
			this.IsEnabled = true;
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x00023983 File Offset: 0x00021B83
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0002398C File Offset: 0x00021B8C
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey == null)
			{
				return;
			}
			cancelInputKey.OnFinalize();
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x000239A4 File Offset: 0x00021BA4
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x0600098B RID: 2443 RVA: 0x000239B3 File Offset: 0x00021BB3
		// (set) Token: 0x0600098C RID: 2444 RVA: 0x000239BB File Offset: 0x00021BBB
		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChanged("CancelInputKey");
				}
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600098D RID: 2445 RVA: 0x000239D8 File Offset: 0x00021BD8
		// (set) Token: 0x0600098E RID: 2446 RVA: 0x000239E0 File Offset: 0x00021BE0
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

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600098F RID: 2447 RVA: 0x000239FE File Offset: 0x00021BFE
		// (set) Token: 0x06000990 RID: 2448 RVA: 0x00023A06 File Offset: 0x00021C06
		[DataSourceProperty]
		public bool HasNotifications
		{
			get
			{
				return this._hasNotifications;
			}
			set
			{
				if (value != this._hasNotifications)
				{
					this._hasNotifications = value;
					base.OnPropertyChangedWithValue(value, "HasNotifications");
				}
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06000991 RID: 2449 RVA: 0x00023A24 File Offset: 0x00021C24
		// (set) Token: 0x06000992 RID: 2450 RVA: 0x00023A2C File Offset: 0x00021C2C
		[DataSourceProperty]
		public string CloseText
		{
			get
			{
				return this._closeText;
			}
			set
			{
				if (value != this._closeText)
				{
					this._closeText = value;
					base.OnPropertyChangedWithValue<string>(value, "CloseText");
				}
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x00023A4F File Offset: 0x00021C4F
		// (set) Token: 0x06000994 RID: 2452 RVA: 0x00023A57 File Offset: 0x00021C57
		[DataSourceProperty]
		public string BadgesText
		{
			get
			{
				return this._badgesText;
			}
			set
			{
				if (value != this._badgesText)
				{
					this._badgesText = value;
					base.OnPropertyChangedWithValue<string>(value, "BadgesText");
				}
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000995 RID: 2453 RVA: 0x00023A7A File Offset: 0x00021C7A
		// (set) Token: 0x06000996 RID: 2454 RVA: 0x00023A82 File Offset: 0x00021C82
		[DataSourceProperty]
		public string SpecialBadgesText
		{
			get
			{
				return this._specialBadgesText;
			}
			set
			{
				if (value != this._specialBadgesText)
				{
					this._specialBadgesText = value;
					base.OnPropertyChangedWithValue<string>(value, "SpecialBadgesText");
				}
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000997 RID: 2455 RVA: 0x00023AA5 File Offset: 0x00021CA5
		// (set) Token: 0x06000998 RID: 2456 RVA: 0x00023AAD File Offset: 0x00021CAD
		[DataSourceProperty]
		public string AchievementBadgesText
		{
			get
			{
				return this._achievementBadgesText;
			}
			set
			{
				if (value != this._achievementBadgesText)
				{
					this._achievementBadgesText = value;
					base.OnPropertyChangedWithValue<string>(value, "AchievementBadgesText");
				}
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000999 RID: 2457 RVA: 0x00023AD0 File Offset: 0x00021CD0
		// (set) Token: 0x0600099A RID: 2458 RVA: 0x00023AD8 File Offset: 0x00021CD8
		[DataSourceProperty]
		public MBBindingList<MPLobbyBadgeItemVM> Badges
		{
			get
			{
				return this._badges;
			}
			set
			{
				if (value != this._badges)
				{
					this._badges = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyBadgeItemVM>>(value, "Badges");
				}
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600099B RID: 2459 RVA: 0x00023AF6 File Offset: 0x00021CF6
		// (set) Token: 0x0600099C RID: 2460 RVA: 0x00023AFE File Offset: 0x00021CFE
		[DataSourceProperty]
		public MBBindingList<MPLobbyAchievementBadgeGroupVM> AchivementBadgeGroups
		{
			get
			{
				return this._achievementBadgeGroups;
			}
			set
			{
				if (value != this._achievementBadgeGroups)
				{
					this._achievementBadgeGroups = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyAchievementBadgeGroupVM>>(value, "AchivementBadgeGroups");
				}
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600099D RID: 2461 RVA: 0x00023B1C File Offset: 0x00021D1C
		// (set) Token: 0x0600099E RID: 2462 RVA: 0x00023B24 File Offset: 0x00021D24
		[DataSourceProperty]
		public MPLobbyBadgeItemVM InspectedBadge
		{
			get
			{
				return this._inspectedBadge;
			}
			set
			{
				if (value != this._inspectedBadge)
				{
					this._inspectedBadge = value;
					base.OnPropertyChangedWithValue<MPLobbyBadgeItemVM>(value, "InspectedBadge");
				}
			}
		}

		// Token: 0x040004A8 RID: 1192
		private Badge[] _playerEarnedBadges;

		// Token: 0x040004AA RID: 1194
		private Action _onBadgeNotificationRead;

		// Token: 0x040004AB RID: 1195
		private Action<MPLobbyAchievementBadgeGroupVM> _onBadgeProgressInfoRequested;

		// Token: 0x040004AC RID: 1196
		private Action _onBadgeSelectionUpdated;

		// Token: 0x040004AD RID: 1197
		private HotKey _inspectProgressKey;

		// Token: 0x040004AE RID: 1198
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040004AF RID: 1199
		private bool _isEnabled;

		// Token: 0x040004B0 RID: 1200
		private bool _hasNotifications;

		// Token: 0x040004B1 RID: 1201
		private string _closeText;

		// Token: 0x040004B2 RID: 1202
		private string _badgesText;

		// Token: 0x040004B3 RID: 1203
		private string _specialBadgesText;

		// Token: 0x040004B4 RID: 1204
		private string _achievementBadgesText;

		// Token: 0x040004B5 RID: 1205
		private MBBindingList<MPLobbyBadgeItemVM> _badges;

		// Token: 0x040004B6 RID: 1206
		private MBBindingList<MPLobbyAchievementBadgeGroupVM> _achievementBadgeGroups;

		// Token: 0x040004B7 RID: 1207
		private MPLobbyBadgeItemVM _inspectedBadge;
	}
}
