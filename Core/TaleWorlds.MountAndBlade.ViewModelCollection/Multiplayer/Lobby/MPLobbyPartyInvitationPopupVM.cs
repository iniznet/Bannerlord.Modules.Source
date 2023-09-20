using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	public class MPLobbyPartyInvitationPopupVM : ViewModel
	{
		public MPLobbyPartyInvitationPopupVM()
		{
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Title = new TextObject("{=QDNcl3DH}Party Invitation", null).ToString();
			this.Message = new TextObject("{=AaAcmalE}You've been invited to join a party by", null).ToString();
		}

		public void OpenWith(PlayerId invitingPlayerID)
		{
			this.InvitingPlayer = new MPLobbyPlayerBaseVM(invitingPlayerID, "", null, null);
			this.IsEnabled = true;
		}

		public void Close()
		{
			this.IsEnabled = false;
		}

		private void ExecuteAccept()
		{
			this.IsEnabled = false;
			NetworkMain.GameClient.AcceptPartyInvitation();
		}

		private void ExecuteDecline()
		{
			this.IsEnabled = false;
			NetworkMain.GameClient.DeclinePartyInvitation();
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
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public string Message
		{
			get
			{
				return this._message;
			}
			set
			{
				if (value != this._message)
				{
					this._message = value;
					base.OnPropertyChangedWithValue<string>(value, "Message");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyPlayerBaseVM InvitingPlayer
		{
			get
			{
				return this._invitingPlayer;
			}
			set
			{
				if (value != this._invitingPlayer)
				{
					this._invitingPlayer = value;
					base.OnPropertyChangedWithValue<MPLobbyPlayerBaseVM>(value, "InvitingPlayer");
				}
			}
		}

		private bool _isEnabled;

		private string _title;

		private string _message;

		private MPLobbyPlayerBaseVM _invitingPlayer;
	}
}
