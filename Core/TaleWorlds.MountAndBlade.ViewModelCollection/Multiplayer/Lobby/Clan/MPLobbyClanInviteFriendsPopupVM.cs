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
	public class MPLobbyClanInviteFriendsPopupVM : ViewModel
	{
		public MPLobbyClanInviteFriendsPopupVM(Func<MBBindingList<MPLobbyPlayerBaseVM>> getAllFriends)
		{
			this._getAllFriends = getAllFriends;
			this.OnlineFriends = new MBBindingList<MPLobbyPlayerBaseVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=v4hVLpap}Invite Players to Clan", null).ToString();
			this.InviteText = new TextObject("{=aZnS9ECC}Invite", null).ToString();
			this.CloseText = new TextObject("{=yQtzabbe}Close", null).ToString();
			this.SelectPlayersText = new TextObject("{=ZAejS7WF}Select players to invite to your clan", null).ToString();
		}

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

		private void ResetSelection()
		{
			foreach (MPLobbyPlayerBaseVM mplobbyPlayerBaseVM in this.OnlineFriends)
			{
				mplobbyPlayerBaseVM.IsSelected = false;
			}
		}

		public void ExecuteClosePopup()
		{
			if (this.IsEnabled)
			{
				this.ResetSelection();
				this.IsEnabled = false;
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
					base.OnPropertyChanged("IsEnabled");
				}
			}
		}

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

		private Func<MBBindingList<MPLobbyPlayerBaseVM>> _getAllFriends;

		private bool _isEnabled;

		private string _titleText;

		private string _inviteText;

		private string _closeText;

		private string _selectPlayersText;

		private MBBindingList<MPLobbyPlayerBaseVM> _onlineFriends;
	}
}
