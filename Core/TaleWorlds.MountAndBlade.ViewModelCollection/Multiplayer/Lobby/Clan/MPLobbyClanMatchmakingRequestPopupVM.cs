using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	public class MPLobbyClanMatchmakingRequestPopupVM : ViewModel
	{
		public MPLobbyClanMatchmakingRequestPopupVM()
		{
			this.ChallengerPartyPlayers = new MBBindingList<MPLobbyPlayerBaseVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=1pwQgr04}Matchmaking Request", null).ToString();
			this.WantsToJoinText = new TextObject("{=WHKG5Rbq}This team wants to join the match you created.", null).ToString();
			this.DoYouAcceptText = new TextObject("{=xkV9g4le}Do you accept them as your opponent?", null).ToString();
		}

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

		public void Close()
		{
			this.IsEnabled = false;
		}

		public void ExecuteAcceptMatchmaking()
		{
			NetworkMain.GameClient.AcceptJoinPremadeGameRequest(this._partyId);
			this.Close();
		}

		public void ExecuteDeclineMatchmaking()
		{
			NetworkMain.GameClient.DeclineJoinPremadeGameRequest(this._partyId);
			this.Close();
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

		private Guid _partyId;

		private bool _isEnabled;

		private bool _isClanMatch;

		private bool _isPracticeMatch;

		private string _titleText;

		private string _clanName;

		private string _wantsToJoinText;

		private string _doYouAcceptText;

		private ImageIdentifierVM _clanSigil;

		private MPLobbyPlayerBaseVM _challengerPartyLeader;

		private MBBindingList<MPLobbyPlayerBaseVM> _challengerPartyPlayers;
	}
}
