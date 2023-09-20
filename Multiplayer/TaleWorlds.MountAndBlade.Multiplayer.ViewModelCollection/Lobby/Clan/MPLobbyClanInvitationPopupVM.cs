using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Clan
{
	public class MPLobbyClanInvitationPopupVM : ViewModel
	{
		public MPLobbyClanInvitationPopupVM()
		{
			this.PartyMembersList = new MBBindingList<MPLobbyClanMemberItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=D9zIAw9y}Clan Invite", null).ToString();
			this.InviteReceivedText = new TextObject("{=wNAl9o4A}You received an invite from", null).ToString();
			this.WantToJoinText = new TextObject("{=qa9aOxLm}Do you want to join this clan?", null).ToString();
		}

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

		public void Close()
		{
			this.IsEnabled = false;
		}

		public void UpdateConfirmation(PlayerId playerId, ClanCreationAnswer answer)
		{
			foreach (MPLobbyClanMemberItemVM mplobbyClanMemberItemVM in this.PartyMembersList)
			{
				if (mplobbyClanMemberItemVM.ProvidedID == playerId)
				{
					if (answer == 1)
					{
						mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=JTMegIk4}Accepted", null).ToString();
					}
					else if (answer == 2)
					{
						mplobbyClanMemberItemVM.InviteAcceptInfo = new TextObject("{=FgaORzy5}Declined", null).ToString();
					}
				}
			}
		}

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

		private MPLobbyClanInvitationPopupVM.InvitationMode _invitationMode;

		private bool _isEnabled;

		private bool _isCreation;

		private string _titleText;

		private string _clanNameAndTag;

		private string _inviteReceivedText;

		private string _withPlayersText;

		private string _wantToJoinText;

		private MBBindingList<MPLobbyClanMemberItemVM> _partyMembersList;

		public enum InvitationMode
		{
			Creation,
			Invitation
		}
	}
}
