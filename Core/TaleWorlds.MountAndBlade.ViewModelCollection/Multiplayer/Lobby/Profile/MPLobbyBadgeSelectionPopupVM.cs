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
	public class MPLobbyBadgeSelectionPopupVM : ViewModel
	{
		public List<LobbyNotification> ActiveNotifications { get; private set; }

		public MPLobbyBadgeSelectionPopupVM(Action onBadgeNotificationRead, Action onBadgeSelectionUpdated, Action<MPLobbyAchievementBadgeGroupVM> onBadgeProgressInfoRequested)
		{
			this._onBadgeNotificationRead = onBadgeNotificationRead;
			this._onBadgeSelectionUpdated = onBadgeSelectionUpdated;
			this._onBadgeProgressInfoRequested = onBadgeProgressInfoRequested;
			this.ActiveNotifications = new List<LobbyNotification>();
			this.Badges = new MBBindingList<MPLobbyBadgeItemVM>();
			this.AchivementBadgeGroups = new MBBindingList<MPLobbyAchievementBadgeGroupVM>();
		}

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

		public void RefreshPlayerData(PlayerData playerData)
		{
			this.UpdateBadges(false);
			this.UpdateBadgeSelection();
		}

		public void RefreshKeyBindings(HotKey inspectProgressKey)
		{
			this._inspectProgressKey = inspectProgressKey;
			foreach (MPLobbyAchievementBadgeGroupVM mplobbyAchievementBadgeGroupVM in this.AchivementBadgeGroups)
			{
				mplobbyAchievementBadgeGroupVM.RefreshKeyBindings(inspectProgressKey);
			}
		}

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

		private bool HasPlayerEarnedBadge(Badge badge)
		{
			Badge[] playerEarnedBadges = this._playerEarnedBadges;
			return playerEarnedBadges != null && playerEarnedBadges.Contains(badge);
		}

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

		private void RefreshNotificationInfo()
		{
			this.HasNotifications = this.ActiveNotifications.Count > 0;
		}

		public void Open()
		{
			this.IsEnabled = true;
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

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

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

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

		private Badge[] _playerEarnedBadges;

		private Action _onBadgeNotificationRead;

		private Action<MPLobbyAchievementBadgeGroupVM> _onBadgeProgressInfoRequested;

		private Action _onBadgeSelectionUpdated;

		private HotKey _inspectProgressKey;

		private InputKeyItemVM _cancelInputKey;

		private bool _isEnabled;

		private bool _hasNotifications;

		private string _closeText;

		private string _badgesText;

		private string _specialBadgesText;

		private string _achievementBadgesText;

		private MBBindingList<MPLobbyBadgeItemVM> _badges;

		private MBBindingList<MPLobbyAchievementBadgeGroupVM> _achievementBadgeGroups;

		private MPLobbyBadgeItemVM _inspectedBadge;
	}
}
