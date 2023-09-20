using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x0200005E RID: 94
	public class MPLobbyPartyInvitationPopupVM : ViewModel
	{
		// Token: 0x060007F7 RID: 2039 RVA: 0x0001E9D4 File Offset: 0x0001CBD4
		public MPLobbyPartyInvitationPopupVM()
		{
			this.RefreshValues();
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x0001E9E2 File Offset: 0x0001CBE2
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Title = new TextObject("{=QDNcl3DH}Party Invitation", null).ToString();
			this.Message = new TextObject("{=AaAcmalE}You've been invited to join a party by", null).ToString();
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0001EA16 File Offset: 0x0001CC16
		public void OpenWith(PlayerId invitingPlayerID)
		{
			this.InvitingPlayer = new MPLobbyPlayerBaseVM(invitingPlayerID, "", null, null);
			this.IsEnabled = true;
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x0001EA32 File Offset: 0x0001CC32
		public void Close()
		{
			this.IsEnabled = false;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x0001EA3B File Offset: 0x0001CC3B
		private void ExecuteAccept()
		{
			this.IsEnabled = false;
			NetworkMain.GameClient.AcceptPartyInvitation();
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x0001EA4E File Offset: 0x0001CC4E
		private void ExecuteDecline()
		{
			this.IsEnabled = false;
			NetworkMain.GameClient.DeclinePartyInvitation();
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x060007FD RID: 2045 RVA: 0x0001EA61 File Offset: 0x0001CC61
		// (set) Token: 0x060007FE RID: 2046 RVA: 0x0001EA69 File Offset: 0x0001CC69
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

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x060007FF RID: 2047 RVA: 0x0001EA87 File Offset: 0x0001CC87
		// (set) Token: 0x06000800 RID: 2048 RVA: 0x0001EA8F File Offset: 0x0001CC8F
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

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000801 RID: 2049 RVA: 0x0001EAB2 File Offset: 0x0001CCB2
		// (set) Token: 0x06000802 RID: 2050 RVA: 0x0001EABA File Offset: 0x0001CCBA
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

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000803 RID: 2051 RVA: 0x0001EADD File Offset: 0x0001CCDD
		// (set) Token: 0x06000804 RID: 2052 RVA: 0x0001EAE5 File Offset: 0x0001CCE5
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

		// Token: 0x04000409 RID: 1033
		private bool _isEnabled;

		// Token: 0x0400040A RID: 1034
		private string _title;

		// Token: 0x0400040B RID: 1035
		private string _message;

		// Token: 0x0400040C RID: 1036
		private MPLobbyPlayerBaseVM _invitingPlayer;
	}
}
