using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x02000095 RID: 149
	public class MPLobbyClanCreationInformationVM : ViewModel
	{
		// Token: 0x06000DEE RID: 3566 RVA: 0x0002FB9E File Offset: 0x0002DD9E
		public MPLobbyClanCreationInformationVM(Action openClanCreationPopup)
		{
			this._openClanCreationPopup = openClanCreationPopup;
			this.PartyMembers = new MBBindingList<MPLobbyClanMemberItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x0002FBC0 File Offset: 0x0002DDC0
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

		// Token: 0x06000DF0 RID: 3568 RVA: 0x0002FC58 File Offset: 0x0002DE58
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

		// Token: 0x06000DF1 RID: 3569 RVA: 0x0002FEB8 File Offset: 0x0002E0B8
		public void OnFriendListUpdated(bool forceUpdate = false)
		{
			foreach (MPLobbyClanMemberItemVM mplobbyClanMemberItemVM in this.PartyMembers)
			{
				mplobbyClanMemberItemVM.UpdateNameAndAvatar(forceUpdate);
			}
		}

		// Token: 0x06000DF2 RID: 3570 RVA: 0x0002FF04 File Offset: 0x0002E104
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

		// Token: 0x06000DF3 RID: 3571 RVA: 0x0002FF52 File Offset: 0x0002E152
		public void ExecuteOpenPopup()
		{
			this.IsEnabled = true;
		}

		// Token: 0x06000DF4 RID: 3572 RVA: 0x0002FF5B File Offset: 0x0002E15B
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x0002FF64 File Offset: 0x0002E164
		private void ExecuteOpenClanCreationPopup()
		{
			this.ExecuteClosePopup();
			this._openClanCreationPopup();
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06000DF6 RID: 3574 RVA: 0x0002FF77 File Offset: 0x0002E177
		// (set) Token: 0x06000DF7 RID: 3575 RVA: 0x0002FF7F File Offset: 0x0002E17F
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

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06000DF8 RID: 3576 RVA: 0x0002FF9C File Offset: 0x0002E19C
		// (set) Token: 0x06000DF9 RID: 3577 RVA: 0x0002FFA4 File Offset: 0x0002E1A4
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

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06000DFA RID: 3578 RVA: 0x0002FFC1 File Offset: 0x0002E1C1
		// (set) Token: 0x06000DFB RID: 3579 RVA: 0x0002FFC9 File Offset: 0x0002E1C9
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

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06000DFC RID: 3580 RVA: 0x0002FFE6 File Offset: 0x0002E1E6
		// (set) Token: 0x06000DFD RID: 3581 RVA: 0x0002FFEE File Offset: 0x0002E1EE
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

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06000DFE RID: 3582 RVA: 0x0003000B File Offset: 0x0002E20B
		// (set) Token: 0x06000DFF RID: 3583 RVA: 0x00030013 File Offset: 0x0002E213
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

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06000E00 RID: 3584 RVA: 0x00030030 File Offset: 0x0002E230
		// (set) Token: 0x06000E01 RID: 3585 RVA: 0x00030038 File Offset: 0x0002E238
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

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06000E02 RID: 3586 RVA: 0x0003005A File Offset: 0x0002E25A
		// (set) Token: 0x06000E03 RID: 3587 RVA: 0x00030062 File Offset: 0x0002E262
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

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06000E04 RID: 3588 RVA: 0x00030084 File Offset: 0x0002E284
		// (set) Token: 0x06000E05 RID: 3589 RVA: 0x0003008C File Offset: 0x0002E28C
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

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06000E06 RID: 3590 RVA: 0x000300AE File Offset: 0x0002E2AE
		// (set) Token: 0x06000E07 RID: 3591 RVA: 0x000300B6 File Offset: 0x0002E2B6
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

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06000E08 RID: 3592 RVA: 0x000300D8 File Offset: 0x0002E2D8
		// (set) Token: 0x06000E09 RID: 3593 RVA: 0x000300E0 File Offset: 0x0002E2E0
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

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06000E0A RID: 3594 RVA: 0x00030102 File Offset: 0x0002E302
		// (set) Token: 0x06000E0B RID: 3595 RVA: 0x0003010A File Offset: 0x0002E30A
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

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06000E0C RID: 3596 RVA: 0x0003012C File Offset: 0x0002E32C
		// (set) Token: 0x06000E0D RID: 3597 RVA: 0x00030134 File Offset: 0x0002E334
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

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06000E0E RID: 3598 RVA: 0x00030157 File Offset: 0x0002E357
		// (set) Token: 0x06000E0F RID: 3599 RVA: 0x0003015F File Offset: 0x0002E35F
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

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06000E10 RID: 3600 RVA: 0x0003017C File Offset: 0x0002E37C
		// (set) Token: 0x06000E11 RID: 3601 RVA: 0x00030184 File Offset: 0x0002E384
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

		// Token: 0x040006A6 RID: 1702
		private Action _openClanCreationPopup;

		// Token: 0x040006A7 RID: 1703
		private bool _isEnabled;

		// Token: 0x040006A8 RID: 1704
		private bool _canCreateClan;

		// Token: 0x040006A9 RID: 1705
		private bool _doesHaveEnoughPlayersToCreateClan;

		// Token: 0x040006AA RID: 1706
		private int _currentPlayerCount;

		// Token: 0x040006AB RID: 1707
		private int _requiredPlayerCount;

		// Token: 0x040006AC RID: 1708
		private string _createClanText;

		// Token: 0x040006AD RID: 1709
		private string _createClanDescriptionText;

		// Token: 0x040006AE RID: 1710
		private string _dontHaveEnoughPlayersInPartyText;

		// Token: 0x040006AF RID: 1711
		private string _partyMemberCountText;

		// Token: 0x040006B0 RID: 1712
		private string _playerText;

		// Token: 0x040006B1 RID: 1713
		private string _createYourClanText;

		// Token: 0x040006B2 RID: 1714
		private string _closeText;

		// Token: 0x040006B3 RID: 1715
		private MBBindingList<MPLobbyClanMemberItemVM> _partyMembers;

		// Token: 0x040006B4 RID: 1716
		private HintViewModel _cantCreateHint;
	}
}
