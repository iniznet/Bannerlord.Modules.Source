using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x02000098 RID: 152
	public class MPLobbyClanInviteFriendsPopupVM : ViewModel
	{
		// Token: 0x06000E6A RID: 3690 RVA: 0x0003106D File Offset: 0x0002F26D
		public MPLobbyClanInviteFriendsPopupVM(Func<MBBindingList<MPLobbyPlayerBaseVM>> getAllFriends)
		{
			this._getAllFriends = getAllFriends;
			this.OnlineFriends = new MBBindingList<MPLobbyPlayerBaseVM>();
			this.RefreshValues();
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x00031090 File Offset: 0x0002F290
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=v4hVLpap}Invite Players to Clan", null).ToString();
			this.InviteText = new TextObject("{=aZnS9ECC}Invite", null).ToString();
			this.CloseText = new TextObject("{=yQtzabbe}Close", null).ToString();
			this.SelectPlayersText = new TextObject("{=ZAejS7WF}Select players to invite to your clan", null).ToString();
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x000310FC File Offset: 0x0002F2FC
		public void Open()
		{
			if (NetworkMain.GameClient.ClanID == Guid.Empty || NetworkMain.GameClient.ClanInfo == null)
			{
				return;
			}
			IEnumerable<PlayerId> enumerable = NetworkMain.GameClient.ClanInfo.Players.Select((ClanPlayer c) => c.PlayerId);
			this.OnlineFriends.Clear();
			using (IEnumerator<MPLobbyPlayerBaseVM> enumerator = this._getAllFriends().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MPLobbyPlayerBaseVM onlineFriend = enumerator.Current;
					if (!enumerable.Contains(onlineFriend.ProvidedID) && !this.OnlineFriends.Any((MPLobbyPlayerBaseVM f) => f.ProvidedID == onlineFriend.ProvidedID))
					{
						this.OnlineFriends.Add(onlineFriend);
					}
				}
			}
			this.IsEnabled = true;
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x000311F8 File Offset: 0x0002F3F8
		private void ExecuteSendInvitation()
		{
			foreach (MPLobbyPlayerBaseVM mplobbyPlayerBaseVM in this.OnlineFriends)
			{
				if (mplobbyPlayerBaseVM.IsSelected)
				{
					mplobbyPlayerBaseVM.ExecuteInviteToClan();
				}
			}
			this.ExecuteClosePopup();
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x00031254 File Offset: 0x0002F454
		private void ResetSelection()
		{
			foreach (MPLobbyPlayerBaseVM mplobbyPlayerBaseVM in this.OnlineFriends)
			{
				mplobbyPlayerBaseVM.IsSelected = false;
			}
		}

		// Token: 0x06000E6F RID: 3695 RVA: 0x000312A0 File Offset: 0x0002F4A0
		public void ExecuteClosePopup()
		{
			if (this.IsEnabled)
			{
				this.ResetSelection();
				this.IsEnabled = false;
			}
		}

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06000E70 RID: 3696 RVA: 0x000312B7 File Offset: 0x0002F4B7
		// (set) Token: 0x06000E71 RID: 3697 RVA: 0x000312BF File Offset: 0x0002F4BF
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
				}
			}
		}

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06000E72 RID: 3698 RVA: 0x000312DC File Offset: 0x0002F4DC
		// (set) Token: 0x06000E73 RID: 3699 RVA: 0x000312E4 File Offset: 0x0002F4E4
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
					base.OnPropertyChanged("TitleText");
				}
			}
		}

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06000E74 RID: 3700 RVA: 0x00031306 File Offset: 0x0002F506
		// (set) Token: 0x06000E75 RID: 3701 RVA: 0x0003130E File Offset: 0x0002F50E
		[DataSourceProperty]
		public string InviteText
		{
			get
			{
				return this._inviteText;
			}
			set
			{
				if (value != this._inviteText)
				{
					this._inviteText = value;
					base.OnPropertyChanged("InviteText");
				}
			}
		}

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06000E76 RID: 3702 RVA: 0x00031330 File Offset: 0x0002F530
		// (set) Token: 0x06000E77 RID: 3703 RVA: 0x00031338 File Offset: 0x0002F538
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
					base.OnPropertyChanged("CloseText");
				}
			}
		}

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06000E78 RID: 3704 RVA: 0x0003135A File Offset: 0x0002F55A
		// (set) Token: 0x06000E79 RID: 3705 RVA: 0x00031362 File Offset: 0x0002F562
		[DataSourceProperty]
		public string SelectPlayersText
		{
			get
			{
				return this._selectPlayersText;
			}
			set
			{
				if (value != this._selectPlayersText)
				{
					this._selectPlayersText = value;
					base.OnPropertyChanged("SelectPlayersText");
				}
			}
		}

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06000E7A RID: 3706 RVA: 0x00031384 File Offset: 0x0002F584
		// (set) Token: 0x06000E7B RID: 3707 RVA: 0x0003138C File Offset: 0x0002F58C
		[DataSourceProperty]
		public MBBindingList<MPLobbyPlayerBaseVM> OnlineFriends
		{
			get
			{
				return this._onlineFriends;
			}
			set
			{
				if (value != this._onlineFriends)
				{
					this._onlineFriends = value;
					base.OnPropertyChanged("OnlineFriends");
				}
			}
		}

		// Token: 0x040006D5 RID: 1749
		private Func<MBBindingList<MPLobbyPlayerBaseVM>> _getAllFriends;

		// Token: 0x040006D6 RID: 1750
		private bool _isEnabled;

		// Token: 0x040006D7 RID: 1751
		private string _titleText;

		// Token: 0x040006D8 RID: 1752
		private string _inviteText;

		// Token: 0x040006D9 RID: 1753
		private string _closeText;

		// Token: 0x040006DA RID: 1754
		private string _selectPlayersText;

		// Token: 0x040006DB RID: 1755
		private MBBindingList<MPLobbyPlayerBaseVM> _onlineFriends;
	}
}
