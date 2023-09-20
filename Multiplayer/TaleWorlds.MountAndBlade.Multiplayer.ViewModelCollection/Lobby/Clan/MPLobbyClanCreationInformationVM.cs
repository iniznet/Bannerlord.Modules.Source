using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Clan
{
	public class MPLobbyClanCreationInformationVM : ViewModel
	{
		public MPLobbyClanCreationInformationVM(Action openClanCreationPopup)
		{
			this._openClanCreationPopup = openClanCreationPopup;
			this.PartyMembers = new MBBindingList<MPLobbyClanMemberItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CloseText = GameTexts.FindText("str_close", null).ToString();
			this.CreateClanText = new TextObject("{=ECb8IPbA}Create Clan", null).ToString();
			this.CreateClanDescriptionText = new TextObject("{=aWzdkfvn}Currently you are not a member of a clan or you don't own a clan. You need to create a party from non-clan member players to form your own clan.", null).ToString();
			this.CreateYourClanText = new TextObject("{=kF3b8cH1}Create Your Clan", null).ToString();
			this.DontHaveEnoughPlayersInPartyText = new TextObject("{=bynNUfSr}Your party does not have enough members to create a clan.", null).ToString();
			this.PlayerText = new TextObject("{=RN6zHak0}Player", null).ToString();
		}

		public void RefreshWith(ClanHomeInfo info)
		{
			if (info == null)
			{
				this.CanCreateClan = false;
				this.CantCreateHint = new HintViewModel(new TextObject("{=EQAjujjO}Clan creation information can't be retrieved", null), null);
				this.DoesHaveEnoughPlayersToCreateClan = false;
				this.PartyMemberCountText = new TextObject("{=y1AGNqyV}Clan creation is not available", null).ToString();
				this.PartyMembers.Clear();
				this.PartyMembers.Add(new MPLobbyClanMemberItemVM(NetworkMain.GameClient.PlayerID));
				return;
			}
			this.CanCreateClan = info.CanCreateClan && NetworkMain.GameClient.IsPartyLeader;
			this.CantCreateHint = new HintViewModel();
			if (!NetworkMain.GameClient.IsPartyLeader)
			{
				this.CantCreateHint = new HintViewModel(new TextObject("{=OiWquyWY}You have to be the leader of the party to create a clan", null), null);
			}
			if (info.NotEnoughPlayersInfo == null)
			{
				this.DoesHaveEnoughPlayersToCreateClan = true;
			}
			else
			{
				this.CurrentPlayerCount = info.NotEnoughPlayersInfo.CurrentPlayerCount;
				this.RequiredPlayerCount = info.NotEnoughPlayersInfo.RequiredPlayerCount;
				this.DoesHaveEnoughPlayersToCreateClan = this.CurrentPlayerCount == this.RequiredPlayerCount;
				GameTexts.SetVariable("LEFT", this.CurrentPlayerCount);
				GameTexts.SetVariable("RIGHT", this.RequiredPlayerCount);
				GameTexts.SetVariable("STR1", GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString());
				GameTexts.SetVariable("STR2", this.PlayerText);
				this.PartyMemberCountText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			}
			this.PartyMembers.Clear();
			if (NetworkMain.GameClient.IsInParty)
			{
				using (List<PartyPlayerInLobbyClient>.Enumerator enumerator = NetworkMain.GameClient.PlayersInParty.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PartyPlayerInLobbyClient partyPlayerInLobbyClient = enumerator.Current;
						MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = new MPLobbyClanMemberItemVM(partyPlayerInLobbyClient.PlayerId);
						if (info.PlayerNotEligibleInfos != null)
						{
							foreach (PlayerNotEligibleInfo playerNotEligibleInfo in info.PlayerNotEligibleInfos)
							{
								if (playerNotEligibleInfo.PlayerId == partyPlayerInLobbyClient.PlayerId)
								{
									foreach (PlayerNotEligibleError playerNotEligibleError in playerNotEligibleInfo.Errors)
									{
										mplobbyClanMemberItemVM.SetNotEligibleInfo(playerNotEligibleError);
									}
								}
							}
						}
						this.PartyMembers.Add(mplobbyClanMemberItemVM);
					}
					return;
				}
			}
			this.PartyMembers.Add(new MPLobbyClanMemberItemVM(NetworkMain.GameClient.PlayerID));
		}

		public void OnFriendListUpdated(bool forceUpdate = false)
		{
			foreach (MPLobbyClanMemberItemVM mplobbyClanMemberItemVM in this.PartyMembers)
			{
				mplobbyClanMemberItemVM.UpdateNameAndAvatar(forceUpdate);
			}
		}

		public void OnPlayerNameUpdated()
		{
			for (int i = 0; i < this.PartyMembers.Count; i++)
			{
				MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = this.PartyMembers[i];
				if (mplobbyClanMemberItemVM.Id == NetworkMain.GameClient.PlayerID)
				{
					mplobbyClanMemberItemVM.UpdateNameAndAvatar(true);
				}
			}
		}

		public void ExecuteOpenPopup()
		{
			this.IsEnabled = true;
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		private void ExecuteOpenClanCreationPopup()
		{
			this.ExecuteClosePopup();
			this._openClanCreationPopup();
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
		public bool CanCreateClan
		{
			get
			{
				return this._canCreateClan;
			}
			set
			{
				if (value != this._canCreateClan)
				{
					this._canCreateClan = value;
					base.OnPropertyChanged("CanCreateClan");
				}
			}
		}

		[DataSourceProperty]
		public bool DoesHaveEnoughPlayersToCreateClan
		{
			get
			{
				return this._doesHaveEnoughPlayersToCreateClan;
			}
			set
			{
				if (value != this._doesHaveEnoughPlayersToCreateClan)
				{
					this._doesHaveEnoughPlayersToCreateClan = value;
					base.OnPropertyChanged("DoesHaveEnoughPlayersToCreateClan");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentPlayerCount
		{
			get
			{
				return this._currentPlayerCount;
			}
			set
			{
				if (value != this._currentPlayerCount)
				{
					this._currentPlayerCount = value;
					base.OnPropertyChanged("CurrentPlayerCount");
				}
			}
		}

		[DataSourceProperty]
		public int RequiredPlayerCount
		{
			get
			{
				return this._requiredPlayerCount;
			}
			set
			{
				if (value != this._requiredPlayerCount)
				{
					this._requiredPlayerCount = value;
					base.OnPropertyChanged("RequiredPlayerCount");
				}
			}
		}

		[DataSourceProperty]
		public string CreateClanText
		{
			get
			{
				return this._createClanText;
			}
			set
			{
				if (value != this._createClanText)
				{
					this._createClanText = value;
					base.OnPropertyChanged("CreateClanText");
				}
			}
		}

		[DataSourceProperty]
		public string CreateClanDescriptionText
		{
			get
			{
				return this._createClanDescriptionText;
			}
			set
			{
				if (value != this._createClanDescriptionText)
				{
					this._createClanDescriptionText = value;
					base.OnPropertyChanged("CreateClanDescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public string DontHaveEnoughPlayersInPartyText
		{
			get
			{
				return this._dontHaveEnoughPlayersInPartyText;
			}
			set
			{
				if (value != this._dontHaveEnoughPlayersInPartyText)
				{
					this._dontHaveEnoughPlayersInPartyText = value;
					base.OnPropertyChanged("DontHaveEnoughPlayersInPartyText");
				}
			}
		}

		[DataSourceProperty]
		public string PartyMemberCountText
		{
			get
			{
				return this._partyMemberCountText;
			}
			set
			{
				if (value != this._partyMemberCountText)
				{
					this._partyMemberCountText = value;
					base.OnPropertyChanged("PartyMemberCountText");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerText
		{
			get
			{
				return this._playerText;
			}
			set
			{
				if (value != this._playerText)
				{
					this._playerText = value;
					base.OnPropertyChanged("PlayerText");
				}
			}
		}

		[DataSourceProperty]
		public string CreateYourClanText
		{
			get
			{
				return this._createYourClanText;
			}
			set
			{
				if (value != this._createYourClanText)
				{
					this._createYourClanText = value;
					base.OnPropertyChanged("CreateYourClanText");
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
		public MBBindingList<MPLobbyClanMemberItemVM> PartyMembers
		{
			get
			{
				return this._partyMembers;
			}
			set
			{
				if (value != this._partyMembers)
				{
					this._partyMembers = value;
					base.OnPropertyChanged("PartyMembers");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel CantCreateHint
		{
			get
			{
				return this._cantCreateHint;
			}
			set
			{
				if (value != this._cantCreateHint)
				{
					this._cantCreateHint = value;
					base.OnPropertyChanged("CantCreateHint");
				}
			}
		}

		private Action _openClanCreationPopup;

		private bool _isEnabled;

		private bool _canCreateClan;

		private bool _doesHaveEnoughPlayersToCreateClan;

		private int _currentPlayerCount;

		private int _requiredPlayerCount;

		private string _createClanText;

		private string _createClanDescriptionText;

		private string _dontHaveEnoughPlayersInPartyText;

		private string _partyMemberCountText;

		private string _playerText;

		private string _createYourClanText;

		private string _closeText;

		private MBBindingList<MPLobbyClanMemberItemVM> _partyMembers;

		private HintViewModel _cantCreateHint;
	}
}
