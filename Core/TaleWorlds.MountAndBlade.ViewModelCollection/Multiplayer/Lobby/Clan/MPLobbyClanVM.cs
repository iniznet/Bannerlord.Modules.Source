using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x020000A1 RID: 161
	public class MPLobbyClanVM : ViewModel
	{
		// Token: 0x06000F6D RID: 3949 RVA: 0x00033643 File Offset: 0x00031843
		public MPLobbyClanVM(Action openInviteClanMemberPopup)
		{
			this.ClanOverview = new MPLobbyClanOverviewVM(openInviteClanMemberPopup);
			this.ClanRoster = new MPLobbyClanRosterVM();
			this._activeNotifications = new List<LobbyNotification>();
			this.TrySetClanSubPage(MPLobbyClanVM.ClanSubPages.Overview);
			this.RefreshValues();
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0003367A File Offset: 0x0003187A
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CloseText = GameTexts.FindText("str_close", null).ToString();
			this.ClanOverview.RefreshValues();
			this.ClanRoster.RefreshValues();
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x000336B0 File Offset: 0x000318B0
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

		// Token: 0x06000F70 RID: 3952 RVA: 0x00033728 File Offset: 0x00031928
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

		// Token: 0x06000F71 RID: 3953 RVA: 0x00033761 File Offset: 0x00031961
		private void ExecuteChangeEnabledSubPage(int subpageIndex)
		{
			this.TrySetClanSubPage((MPLobbyClanVM.ClanSubPages)subpageIndex);
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x0003376C File Offset: 0x0003196C
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

		// Token: 0x06000F73 RID: 3955 RVA: 0x000337AD File Offset: 0x000319AD
		public void OnNotificationReceived(LobbyNotification notification)
		{
			if (this.IsEnabled)
			{
				NetworkMain.GameClient.MarkNotificationAsRead(notification.Id);
				return;
			}
			this._activeNotifications.Add(notification);
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x000337D4 File Offset: 0x000319D4
		public void OnPlayerNameUpdated(string playerName)
		{
			MPLobbyClanRosterVM clanRoster = this.ClanRoster;
			if (clanRoster == null)
			{
				return;
			}
			clanRoster.OnPlayerNameUpdated(playerName);
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x000337E7 File Offset: 0x000319E7
		public void ExecuteOpenPopup()
		{
			this.IsEnabled = true;
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x000337F0 File Offset: 0x000319F0
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06000F77 RID: 3959 RVA: 0x000337F9 File Offset: 0x000319F9
		// (set) Token: 0x06000F78 RID: 3960 RVA: 0x00033801 File Offset: 0x00031A01
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

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06000F79 RID: 3961 RVA: 0x00033824 File Offset: 0x00031A24
		// (set) Token: 0x06000F7A RID: 3962 RVA: 0x0003382C File Offset: 0x00031A2C
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

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06000F7B RID: 3963 RVA: 0x00033849 File Offset: 0x00031A49
		// (set) Token: 0x06000F7C RID: 3964 RVA: 0x00033851 File Offset: 0x00031A51
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

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06000F7D RID: 3965 RVA: 0x00033874 File Offset: 0x00031A74
		// (set) Token: 0x06000F7E RID: 3966 RVA: 0x0003387C File Offset: 0x00031A7C
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

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06000F7F RID: 3967 RVA: 0x00033899 File Offset: 0x00031A99
		// (set) Token: 0x06000F80 RID: 3968 RVA: 0x000338A1 File Offset: 0x00031AA1
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

		// Token: 0x04000745 RID: 1861
		private MPLobbyClanVM.ClanSubPages _currentSubPage;

		// Token: 0x04000746 RID: 1862
		private List<LobbyNotification> _activeNotifications;

		// Token: 0x04000747 RID: 1863
		private bool _isEnabled;

		// Token: 0x04000748 RID: 1864
		private int _selectedSubPageIndex;

		// Token: 0x04000749 RID: 1865
		private string _closeText;

		// Token: 0x0400074A RID: 1866
		private MPLobbyClanOverviewVM _clanOverview;

		// Token: 0x0400074B RID: 1867
		private MPLobbyClanRosterVM _clanRoster;

		// Token: 0x020001FD RID: 509
		public enum ClanSubPages
		{
			// Token: 0x04000E3B RID: 3643
			Overview,
			// Token: 0x04000E3C RID: 3644
			Roster
		}
	}
}
