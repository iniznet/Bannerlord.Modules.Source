using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x0200009C RID: 156
	public class MPLobbyClanMatchmakingRequestPopupVM : ViewModel
	{
		// Token: 0x06000EBE RID: 3774 RVA: 0x00031C4A File Offset: 0x0002FE4A
		public MPLobbyClanMatchmakingRequestPopupVM()
		{
			this.ChallengerPartyPlayers = new MBBindingList<MPLobbyPlayerBaseVM>();
			this.RefreshValues();
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x00031C64 File Offset: 0x0002FE64
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=1pwQgr04}Matchmaking Request", null).ToString();
			this.WantsToJoinText = new TextObject("{=WHKG5Rbq}This team wants to join the match you created.", null).ToString();
			this.DoYouAcceptText = new TextObject("{=xkV9g4le}Do you accept them as your opponent?", null).ToString();
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x00031CBC File Offset: 0x0002FEBC
		public void OpenWith(string clanName, string clanSigilCode, Guid partyId, PlayerId[] challengerPlayerIDs, PlayerId challengerPartyLeaderID, PremadeGameType premadeGameType)
		{
			this.ChallengerPartyPlayers.Clear();
			this.IsClanMatch = false;
			this.IsPracticeMatch = false;
			this._partyId = partyId;
			if (premadeGameType == PremadeGameType.Clan)
			{
				this.IsClanMatch = true;
				this.ClanName = clanName;
				this.ClanSigil = new ImageIdentifierVM(BannerCode.CreateFrom(clanSigilCode), true);
			}
			else if (premadeGameType == PremadeGameType.Practice)
			{
				this.IsPracticeMatch = true;
				this.ChallengerPartyLeader = new MPLobbyPlayerBaseVM(challengerPartyLeaderID, "", null, null);
				foreach (PlayerId playerId in challengerPlayerIDs)
				{
					this.ChallengerPartyPlayers.Add(new MPLobbyPlayerBaseVM(playerId, "", null, null));
				}
			}
			this.IsEnabled = true;
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x00031D67 File Offset: 0x0002FF67
		public void Close()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x00031D70 File Offset: 0x0002FF70
		public void ExecuteAcceptMatchmaking()
		{
			NetworkMain.GameClient.AcceptJoinPremadeGameRequest(this._partyId);
			this.Close();
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x00031D88 File Offset: 0x0002FF88
		public void ExecuteDeclineMatchmaking()
		{
			NetworkMain.GameClient.DeclineJoinPremadeGameRequest(this._partyId);
			this.Close();
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06000EC4 RID: 3780 RVA: 0x00031DA0 File Offset: 0x0002FFA0
		// (set) Token: 0x06000EC5 RID: 3781 RVA: 0x00031DA8 File Offset: 0x0002FFA8
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

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06000EC6 RID: 3782 RVA: 0x00031DC5 File Offset: 0x0002FFC5
		// (set) Token: 0x06000EC7 RID: 3783 RVA: 0x00031DCD File Offset: 0x0002FFCD
		[DataSourceProperty]
		public bool IsClanMatch
		{
			get
			{
				return this._isClanMatch;
			}
			set
			{
				if (value != this._isClanMatch)
				{
					this._isClanMatch = value;
					base.OnPropertyChanged("IsClanMatch");
				}
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06000EC8 RID: 3784 RVA: 0x00031DEA File Offset: 0x0002FFEA
		// (set) Token: 0x06000EC9 RID: 3785 RVA: 0x00031DF2 File Offset: 0x0002FFF2
		[DataSourceProperty]
		public bool IsPracticeMatch
		{
			get
			{
				return this._isPracticeMatch;
			}
			set
			{
				if (value != this._isPracticeMatch)
				{
					this._isPracticeMatch = value;
					base.OnPropertyChanged("IsPracticeMatch");
				}
			}
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06000ECA RID: 3786 RVA: 0x00031E0F File Offset: 0x0003000F
		// (set) Token: 0x06000ECB RID: 3787 RVA: 0x00031E17 File Offset: 0x00030017
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

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06000ECC RID: 3788 RVA: 0x00031E39 File Offset: 0x00030039
		// (set) Token: 0x06000ECD RID: 3789 RVA: 0x00031E41 File Offset: 0x00030041
		[DataSourceProperty]
		public string ClanName
		{
			get
			{
				return this._clanName;
			}
			set
			{
				if (value != this._clanName)
				{
					this._clanName = value;
					base.OnPropertyChanged("ClanName");
				}
			}
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06000ECE RID: 3790 RVA: 0x00031E63 File Offset: 0x00030063
		// (set) Token: 0x06000ECF RID: 3791 RVA: 0x00031E6B File Offset: 0x0003006B
		[DataSourceProperty]
		public string WantsToJoinText
		{
			get
			{
				return this._wantsToJoinText;
			}
			set
			{
				if (value != this._wantsToJoinText)
				{
					this._wantsToJoinText = value;
					base.OnPropertyChanged("WantsToJoinText");
				}
			}
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06000ED0 RID: 3792 RVA: 0x00031E8D File Offset: 0x0003008D
		// (set) Token: 0x06000ED1 RID: 3793 RVA: 0x00031E95 File Offset: 0x00030095
		[DataSourceProperty]
		public string DoYouAcceptText
		{
			get
			{
				return this._doYouAcceptText;
			}
			set
			{
				if (value != this._doYouAcceptText)
				{
					this._doYouAcceptText = value;
					base.OnPropertyChanged("DoYouAcceptText");
				}
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06000ED2 RID: 3794 RVA: 0x00031EB7 File Offset: 0x000300B7
		// (set) Token: 0x06000ED3 RID: 3795 RVA: 0x00031EBF File Offset: 0x000300BF
		[DataSourceProperty]
		public ImageIdentifierVM ClanSigil
		{
			get
			{
				return this._clanSigil;
			}
			set
			{
				if (value != this._clanSigil)
				{
					this._clanSigil = value;
					base.OnPropertyChanged("ClanSigil");
				}
			}
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06000ED4 RID: 3796 RVA: 0x00031EDC File Offset: 0x000300DC
		// (set) Token: 0x06000ED5 RID: 3797 RVA: 0x00031EE4 File Offset: 0x000300E4
		[DataSourceProperty]
		public MPLobbyPlayerBaseVM ChallengerPartyLeader
		{
			get
			{
				return this._challengerPartyLeader;
			}
			set
			{
				if (value != this._challengerPartyLeader)
				{
					this._challengerPartyLeader = value;
					base.OnPropertyChangedWithValue<MPLobbyPlayerBaseVM>(value, "ChallengerPartyLeader");
				}
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06000ED6 RID: 3798 RVA: 0x00031F02 File Offset: 0x00030102
		// (set) Token: 0x06000ED7 RID: 3799 RVA: 0x00031F0A File Offset: 0x0003010A
		[DataSourceProperty]
		public MBBindingList<MPLobbyPlayerBaseVM> ChallengerPartyPlayers
		{
			get
			{
				return this._challengerPartyPlayers;
			}
			set
			{
				if (value != this._challengerPartyPlayers)
				{
					this._challengerPartyPlayers = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyPlayerBaseVM>>(value, "ChallengerPartyPlayers");
				}
			}
		}

		// Token: 0x04000700 RID: 1792
		private Guid _partyId;

		// Token: 0x04000701 RID: 1793
		private bool _isEnabled;

		// Token: 0x04000702 RID: 1794
		private bool _isClanMatch;

		// Token: 0x04000703 RID: 1795
		private bool _isPracticeMatch;

		// Token: 0x04000704 RID: 1796
		private string _titleText;

		// Token: 0x04000705 RID: 1797
		private string _clanName;

		// Token: 0x04000706 RID: 1798
		private string _wantsToJoinText;

		// Token: 0x04000707 RID: 1799
		private string _doYouAcceptText;

		// Token: 0x04000708 RID: 1800
		private ImageIdentifierVM _clanSigil;

		// Token: 0x04000709 RID: 1801
		private MPLobbyPlayerBaseVM _challengerPartyLeader;

		// Token: 0x0400070A RID: 1802
		private MBBindingList<MPLobbyPlayerBaseVM> _challengerPartyPlayers;
	}
}
