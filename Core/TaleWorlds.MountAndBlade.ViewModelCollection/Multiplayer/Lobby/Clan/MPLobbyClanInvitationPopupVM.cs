using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x02000097 RID: 151
	public class MPLobbyClanInvitationPopupVM : ViewModel
	{
		// Token: 0x06000E53 RID: 3667 RVA: 0x00030CA0 File Offset: 0x0002EEA0
		public MPLobbyClanInvitationPopupVM()
		{
			this.PartyMembersList = new MBBindingList<MPLobbyClanMemberItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x00030CBC File Offset: 0x0002EEBC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=D9zIAw9y}Clan Invite", null).ToString();
			this.InviteReceivedText = new TextObject("{=wNAl9o4A}You received an invite from", null).ToString();
			this.WantToJoinText = new TextObject("{=qa9aOxLm}Do you want to join this clan?", null).ToString();
		}

		// Token: 0x06000E55 RID: 3669 RVA: 0x00030D14 File Offset: 0x0002EF14
		public void Open(string clanName, string clanTag, bool isCreation)
		{
			GameTexts.SetVariable("STR", clanTag);
			string text = new TextObject("{=uTXYEAOg}[{STR}]", null).ToString();
			GameTexts.SetVariable("STR1", clanName);
			GameTexts.SetVariable("STR2", text);
			this.ClanNameAndTag = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.PartyMembersList.Clear();
			this.IsCreation = isCreation;
			if (isCreation)
			{
				this._invitationMode = MPLobbyClanInvitationPopupVM.InvitationMode.Creation;
				using (List<PartyPlayerInLobbyClient>.Enumerator enumerator = NetworkMain.GameClient.PlayersInParty.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PartyPlayerInLobbyClient partyPlayerInLobbyClient = enumerator.Current;
						if (partyPlayerInLobbyClient.PlayerId != NetworkMain.GameClient.PlayerID)
						{
							MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = new MPLobbyClanMemberItemVM(partyPlayerInLobbyClient.PlayerId);
							mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=c0ZdKSkn}Waiting", null).ToString();
							this.PartyMembersList.Add(mplobbyClanMemberItemVM);
						}
					}
					goto IL_E3;
				}
			}
			this._invitationMode = MPLobbyClanInvitationPopupVM.InvitationMode.Invitation;
			IL_E3:
			this.WithPlayersText = ((this.PartyMembersList.Count > 1) ? new TextObject("{=iCaRFZpG}along with these players", null).ToString() : string.Empty);
			this.IsEnabled = true;
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x00030E48 File Offset: 0x0002F048
		public void Close()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x00030E54 File Offset: 0x0002F054
		public void UpdateConfirmation(PlayerId playerId, ClanCreationAnswer answer)
		{
			foreach (MPLobbyClanMemberItemVM mplobbyClanMemberItemVM in this.PartyMembersList)
			{
				if (mplobbyClanMemberItemVM.ProvidedID == playerId)
				{
					if (answer == ClanCreationAnswer.Accepted)
					{
						mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=JTMegIk4}Accepted", null).ToString();
					}
					else if (answer == ClanCreationAnswer.Declined)
					{
						mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=FgaORzy5}Declined", null).ToString();
					}
				}
			}
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x00030EE0 File Offset: 0x0002F0E0
		private void ExecuteAcceptInvitation()
		{
			this.IsEnabled = false;
			if (this._invitationMode == MPLobbyClanInvitationPopupVM.InvitationMode.Creation)
			{
				NetworkMain.GameClient.AcceptClanCreationRequest();
				return;
			}
			NetworkMain.GameClient.AcceptClanInvitation();
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x00030F06 File Offset: 0x0002F106
		private void ExecuteDeclineInvitation()
		{
			this.IsEnabled = false;
			if (this._invitationMode == MPLobbyClanInvitationPopupVM.InvitationMode.Creation)
			{
				NetworkMain.GameClient.DeclineClanCreationRequest();
				return;
			}
			NetworkMain.GameClient.DeclineClanInvitation();
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06000E5A RID: 3674 RVA: 0x00030F2C File Offset: 0x0002F12C
		// (set) Token: 0x06000E5B RID: 3675 RVA: 0x00030F34 File Offset: 0x0002F134
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

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06000E5C RID: 3676 RVA: 0x00030F51 File Offset: 0x0002F151
		// (set) Token: 0x06000E5D RID: 3677 RVA: 0x00030F59 File Offset: 0x0002F159
		[DataSourceProperty]
		public bool IsCreation
		{
			get
			{
				return this._isCreation;
			}
			set
			{
				if (value != this._isCreation)
				{
					this._isCreation = value;
					base.OnPropertyChanged("IsCreation");
				}
			}
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06000E5E RID: 3678 RVA: 0x00030F76 File Offset: 0x0002F176
		// (set) Token: 0x06000E5F RID: 3679 RVA: 0x00030F7E File Offset: 0x0002F17E
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

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06000E60 RID: 3680 RVA: 0x00030FA0 File Offset: 0x0002F1A0
		// (set) Token: 0x06000E61 RID: 3681 RVA: 0x00030FA8 File Offset: 0x0002F1A8
		[DataSourceProperty]
		public string ClanNameAndTag
		{
			get
			{
				return this._clanNameAndTag;
			}
			set
			{
				if (value != this._clanNameAndTag)
				{
					this._clanNameAndTag = value;
					base.OnPropertyChanged("ClanNameAndTag");
				}
			}
		}

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06000E62 RID: 3682 RVA: 0x00030FCA File Offset: 0x0002F1CA
		// (set) Token: 0x06000E63 RID: 3683 RVA: 0x00030FD2 File Offset: 0x0002F1D2
		[DataSourceProperty]
		public string InviteReceivedText
		{
			get
			{
				return this._inviteReceivedText;
			}
			set
			{
				if (value != this._inviteReceivedText)
				{
					this._inviteReceivedText = value;
					base.OnPropertyChanged("InviteReceivedText");
				}
			}
		}

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06000E64 RID: 3684 RVA: 0x00030FF4 File Offset: 0x0002F1F4
		// (set) Token: 0x06000E65 RID: 3685 RVA: 0x00030FFC File Offset: 0x0002F1FC
		[DataSourceProperty]
		public string WithPlayersText
		{
			get
			{
				return this._withPlayersText;
			}
			set
			{
				if (value != this._withPlayersText)
				{
					this._withPlayersText = value;
					base.OnPropertyChanged("WithPlayersText");
				}
			}
		}

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06000E66 RID: 3686 RVA: 0x0003101E File Offset: 0x0002F21E
		// (set) Token: 0x06000E67 RID: 3687 RVA: 0x00031026 File Offset: 0x0002F226
		[DataSourceProperty]
		public string WantToJoinText
		{
			get
			{
				return this._wantToJoinText;
			}
			set
			{
				if (value != this._wantToJoinText)
				{
					this._wantToJoinText = value;
					base.OnPropertyChanged("WantToJoinText");
				}
			}
		}

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06000E68 RID: 3688 RVA: 0x00031048 File Offset: 0x0002F248
		// (set) Token: 0x06000E69 RID: 3689 RVA: 0x00031050 File Offset: 0x0002F250
		[DataSourceProperty]
		public MBBindingList<MPLobbyClanMemberItemVM> PartyMembersList
		{
			get
			{
				return this._partyMembersList;
			}
			set
			{
				if (value != this._partyMembersList)
				{
					this._partyMembersList = value;
					base.OnPropertyChanged("PartyMembersList");
				}
			}
		}

		// Token: 0x040006CC RID: 1740
		private MPLobbyClanInvitationPopupVM.InvitationMode _invitationMode;

		// Token: 0x040006CD RID: 1741
		private bool _isEnabled;

		// Token: 0x040006CE RID: 1742
		private bool _isCreation;

		// Token: 0x040006CF RID: 1743
		private string _titleText;

		// Token: 0x040006D0 RID: 1744
		private string _clanNameAndTag;

		// Token: 0x040006D1 RID: 1745
		private string _inviteReceivedText;

		// Token: 0x040006D2 RID: 1746
		private string _withPlayersText;

		// Token: 0x040006D3 RID: 1747
		private string _wantToJoinText;

		// Token: 0x040006D4 RID: 1748
		private MBBindingList<MPLobbyClanMemberItemVM> _partyMembersList;

		// Token: 0x020001EA RID: 490
		public enum InvitationMode
		{
			// Token: 0x04000E16 RID: 3606
			Creation,
			// Token: 0x04000E17 RID: 3607
			Invitation
		}
	}
}
