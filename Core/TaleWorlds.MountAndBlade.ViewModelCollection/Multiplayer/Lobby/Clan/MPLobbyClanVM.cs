using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	public class MPLobbyClanVM : ViewModel
	{
		public MPLobbyClanVM(Action openInviteClanMemberPopup)
		{
			this.ClanOverview = new MPLobbyClanOverviewVM(openInviteClanMemberPopup);
			this.ClanRoster = new MPLobbyClanRosterVM();
			this._activeNotifications = new List<LobbyNotification>();
			this.TrySetClanSubPage(MPLobbyClanVM.ClanSubPages.Overview);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CloseText = GameTexts.FindText("str_close", null).ToString();
			this.ClanOverview.RefreshValues();
			this.ClanRoster.RefreshValues();
		}

		private void OnIsEnabledChanged()
		{
			if (this.IsEnabled)
			{
				this.TrySetClanSubPage(MPLobbyClanVM.ClanSubPages.Overview);
				foreach (LobbyNotification lobbyNotification in this._activeNotifications)
				{
					NetworkMain.GameClient.MarkNotificationAsRead(lobbyNotification.Id);
				}
				this._activeNotifications.Clear();
			}
		}

		public async void OnClanInfoChanged()
		{
			ClanHomeInfo clanHomeInfo = NetworkMain.GameClient.ClanHomeInfo;
			await this.ClanOverview.RefreshClanInformation(clanHomeInfo);
			this.ClanRoster.RefreshClanInformation(clanHomeInfo);
			if (!clanHomeInfo.IsInClan)
			{
				this.ExecuteClosePopup();
			}
		}

		private void ExecuteChangeEnabledSubPage(int subpageIndex)
		{
			this.TrySetClanSubPage((MPLobbyClanVM.ClanSubPages)subpageIndex);
		}

		public async void TrySetClanSubPage(MPLobbyClanVM.ClanSubPages newPage)
		{
			this.ClanOverview.IsSelected = false;
			this.ClanRoster.IsSelected = false;
			this._currentSubPage = newPage;
			this.SelectedSubPageIndex = (int)newPage;
			if (newPage == MPLobbyClanVM.ClanSubPages.Overview)
			{
				this.ClanOverview.IsSelected = true;
				await this.ClanOverview.RefreshClanInformation(NetworkMain.GameClient.ClanHomeInfo);
			}
			else if (newPage == MPLobbyClanVM.ClanSubPages.Roster)
			{
				this.ClanRoster.IsSelected = true;
				this.ClanRoster.RefreshClanInformation(NetworkMain.GameClient.ClanHomeInfo);
			}
		}

		public void OnNotificationReceived(LobbyNotification notification)
		{
			if (this.IsEnabled)
			{
				NetworkMain.GameClient.MarkNotificationAsRead(notification.Id);
				return;
			}
			this._activeNotifications.Add(notification);
		}

		public void OnPlayerNameUpdated(string playerName)
		{
			MPLobbyClanRosterVM clanRoster = this.ClanRoster;
			if (clanRoster == null)
			{
				return;
			}
			clanRoster.OnPlayerNameUpdated(playerName);
		}

		public void ExecuteOpenPopup()
		{
			this.IsEnabled = true;
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
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
					base.OnPropertyChanged("IsEnabled");
					this.OnIsEnabledChanged();
				}
			}
		}

		[DataSourceProperty]
		public int SelectedSubPageIndex
		{
			get
			{
				return this._selectedSubPageIndex;
			}
			set
			{
				if (value != this._selectedSubPageIndex)
				{
					this._selectedSubPageIndex = value;
					base.OnPropertyChanged("SelectedSubPageIndex");
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
		public MPLobbyClanOverviewVM ClanOverview
		{
			get
			{
				return this._clanOverview;
			}
			set
			{
				if (value != this._clanOverview)
				{
					this._clanOverview = value;
					base.OnPropertyChanged("ClanOverview");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyClanRosterVM ClanRoster
		{
			get
			{
				return this._clanRoster;
			}
			set
			{
				if (value != this._clanRoster)
				{
					this._clanRoster = value;
					base.OnPropertyChanged("ClanRoster");
				}
			}
		}

		private MPLobbyClanVM.ClanSubPages _currentSubPage;

		private List<LobbyNotification> _activeNotifications;

		private bool _isEnabled;

		private int _selectedSubPageIndex;

		private string _closeText;

		private MPLobbyClanOverviewVM _clanOverview;

		private MPLobbyClanRosterVM _clanRoster;

		public enum ClanSubPages
		{
			Overview,
			Roster
		}
	}
}
