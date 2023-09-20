using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x0200009F RID: 159
	public class MPLobbyClanRosterVM : ViewModel
	{
		// Token: 0x06000F34 RID: 3892 RVA: 0x000329D9 File Offset: 0x00030BD9
		public MPLobbyClanRosterVM()
		{
			this.MembersList = new MBBindingList<MPLobbyClanMemberItemVM>();
			this.MemberActionsList = new MBBindingList<StringPairItemWithActionVM>();
			this._memberComparer = new MPLobbyClanRosterVM.MemberComparer();
			this.RefreshValues();
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x00032A08 File Offset: 0x00030C08
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RosterText = new TextObject("{=oyVeCtlg}Roster", null).ToString();
			this.NameText = new TextObject("{=PDdh1sBj}Name", null).ToString();
			this.BadgeText = new TextObject("{=4PrfimcK}Badge", null).ToString();
			this.StatusText = new TextObject("{=DXczLzml}Status", null).ToString();
			this.PromoteToClanOfficerHint = new HintViewModel(new TextObject("{=oeSrXaKt}You need to demote one of the officers", null), null);
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x00032A8C File Offset: 0x00030C8C
		public void RefreshClanInformation(ClanHomeInfo info)
		{
			if (info == null || info.ClanInfo == null)
			{
				return;
			}
			this._isClanLeader = NetworkMain.GameClient.IsClanLeader;
			this._isClanOfficer = NetworkMain.GameClient.IsClanOfficer;
			this.MembersList.Clear();
			ClanPlayer[] players = info.ClanInfo.Players;
			for (int j = 0; j < players.Length; j++)
			{
				ClanPlayer member = players[j];
				if (!MultiplayerPlayerHelper.IsBlocked(member.PlayerId))
				{
					ClanPlayerInfo clanPlayerInfo = info.ClanPlayerInfos.First((ClanPlayerInfo i) => i.PlayerId.Equals(member.PlayerId));
					if (clanPlayerInfo != null)
					{
						bool flag = clanPlayerInfo.State == AnotherPlayerState.AtLobby || clanPlayerInfo.State == AnotherPlayerState.InMultiplayerGame;
						this.MembersList.Add(new MPLobbyClanMemberItemVM(member, flag, clanPlayerInfo.ActiveBadgeId, clanPlayerInfo.State, new Action<MPLobbyClanMemberItemVM>(this.ExecutePopulateActionsList)));
					}
				}
			}
			this.MembersList.Sort(this._memberComparer);
			this.IsPrivilegedMember = NetworkMain.GameClient.IsClanLeader || NetworkMain.GameClient.IsClanOfficer;
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x00032BA4 File Offset: 0x00030DA4
		public void OnPlayerNameUpdated(string playerName)
		{
			for (int i = 0; i < this.MembersList.Count; i++)
			{
				MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = this.MembersList[i];
				if (mplobbyClanMemberItemVM.Id == NetworkMain.GameClient.PlayerID)
				{
					mplobbyClanMemberItemVM.UpdateNameAndAvatar(true);
				}
			}
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x00032BF4 File Offset: 0x00030DF4
		private void ExecutePopulateActionsList(MPLobbyClanMemberItemVM member)
		{
			this.MemberActionsList.Clear();
			if (NetworkMain.GameClient.PlayerID != member.Id)
			{
				if (this._isClanLeader)
				{
					this.MemberActionsList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecutePromoteToClanLeader), new TextObject("{=GRpGNYHW}Promote To Clan Leader", null).ToString(), "PromoteToClanLeader", member));
					if (NetworkMain.GameClient.IsPlayerClanOfficer(member.Id))
					{
						this.MemberActionsList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteDemoteFromClanOfficer), new TextObject("{=gowlLS2b}Demote From Clan Officer", null).ToString(), "DemoteFromClanOfficer", member));
					}
					else
					{
						StringPairItemWithActionVM stringPairItemWithActionVM = new StringPairItemWithActionVM(new Action<object>(this.ExecutePromoteToClanOfficer), new TextObject("{=BXI1ObU8}Promote To Clan Officer", null).ToString(), "PromoteToClanOfficer", member);
						if (NetworkMain.GameClient.PlayersInClan.Count((ClanPlayer m) => m.Role == ClanPlayerRole.Officer) == Parameters.ClanOfficerCount)
						{
							stringPairItemWithActionVM.IsEnabled = false;
							stringPairItemWithActionVM.Hint = this.PromoteToClanOfficerHint;
						}
						this.MemberActionsList.Add(stringPairItemWithActionVM);
					}
				}
				if ((this._isClanOfficer || this._isClanLeader) && !NetworkMain.GameClient.IsPlayerClanLeader(member.Id) && (!this._isClanOfficer || !NetworkMain.GameClient.IsPlayerClanOfficer(member.Id)))
				{
					this.MemberActionsList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteKickFromClan), new TextObject("{=S8pZEPni}Kick From Clan", null).ToString(), "KickFromClan", member));
				}
				if (NetworkMain.GameClient.FriendInfos.All((FriendInfo f) => f.Id != member.Id))
				{
					this.MemberActionsList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteRequestFriendship), GameTexts.FindText("str_mp_scoreboard_context_request_friendship", null).ToString(), "RequestFriendship", member));
				}
				else
				{
					this.MemberActionsList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteTerminateFriendship), new TextObject("{=2YIVRuRa}Remove From Friends", null).ToString(), "TerminateFriendship", member));
				}
				if (NetworkMain.GameClient.SupportedFeatures.SupportsFeatures(Features.Party))
				{
					this.MemberActionsList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteInviteToParty), new TextObject("{=RzROgBkv}Invite To Party", null).ToString(), "InviteToParty", member));
				}
				MultiplayerPlayerContextMenuHelper.AddLobbyViewProfileOptions(member, this.MemberActionsList);
			}
			if (this.MemberActionsList.Count > 0)
			{
				this.IsMemberActionsActive = false;
				this.IsMemberActionsActive = true;
			}
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x00032ED0 File Offset: 0x000310D0
		private void ExecuteRequestFriendship(object memberObj)
		{
			MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = memberObj as MPLobbyClanMemberItemVM;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(mplobbyClanMemberItemVM.Id);
			NetworkMain.GameClient.AddFriend(mplobbyClanMemberItemVM.Id, flag);
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x00032F14 File Offset: 0x00031114
		private void ExecuteTerminateFriendship(object memberObj)
		{
			MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = memberObj as MPLobbyClanMemberItemVM;
			NetworkMain.GameClient.RemoveFriend(mplobbyClanMemberItemVM.Id);
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x00032F38 File Offset: 0x00031138
		private void ExecutePromoteToClanLeader(object memberObj)
		{
			MPLobbyClanMemberItemVM member = memberObj as MPLobbyClanMemberItemVM;
			GameTexts.SetVariable("MEMBER_NAME", member.Name);
			string text = new TextObject("{=GRpGNYHW}Promote To Clan Leader", null).ToString();
			string text2 = new TextObject("{=Z0TW2cub}Are you sure want to promote {MEMBER_NAME} as clan leader? You will lose your leadership.", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
			{
				this.PromoteToClanLeader(member.Id);
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x00032FE0 File Offset: 0x000311E0
		private void ExecutePromoteToClanOfficer(object memberObj)
		{
			MPLobbyClanMemberItemVM member = memberObj as MPLobbyClanMemberItemVM;
			GameTexts.SetVariable("MEMBER_NAME", member.Name);
			string text = new TextObject("{=BXI1ObU8}Promote To Clan Officer", null).ToString();
			string text2 = new TextObject("{=MS4Ng2iw}Are you sure want to promote {MEMBER_NAME} as clan officer?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
			{
				this.PromoteToClanOfficer(member.Id);
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x00033088 File Offset: 0x00031288
		private void ExecuteDemoteFromClanOfficer(object memberObj)
		{
			MPLobbyClanMemberItemVM member = memberObj as MPLobbyClanMemberItemVM;
			GameTexts.SetVariable("MEMBER_NAME", member.Name);
			string text = new TextObject("{=gowlLS2b}Demote From Clan Officer", null).ToString();
			string text2 = new TextObject("{=pSb1P6ZA}Are you sure want to demote {MEMBER_NAME} from clan officers?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
			{
				this.DemoteFromClanOfficer(member.Id);
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x00033130 File Offset: 0x00031330
		private void ExecuteKickFromClan(object memberObj)
		{
			MPLobbyClanMemberItemVM member = memberObj as MPLobbyClanMemberItemVM;
			GameTexts.SetVariable("MEMBER_NAME", member.Name);
			string text = new TextObject("{=S8pZEPni}Kick From Clan", null).ToString();
			string text2 = new TextObject("{=L6eaNe2q}Are you sure want to kick {MEMBER_NAME} from clan?", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
			{
				this.KickFromClan(member.Id);
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x000331D8 File Offset: 0x000313D8
		private void ExecuteInviteToParty(object memberObj)
		{
			MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = memberObj as MPLobbyClanMemberItemVM;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(mplobbyClanMemberItemVM.Id);
			NetworkMain.GameClient.InviteToParty(mplobbyClanMemberItemVM.Id, flag);
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x0003321B File Offset: 0x0003141B
		private void ExecuteViewProfile(object memberObj)
		{
			(memberObj as MPLobbyClanMemberItemVM).ExecuteShowProfile();
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x00033228 File Offset: 0x00031428
		private void PromoteToClanLeader(PlayerId playerId)
		{
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(playerId);
			NetworkMain.GameClient.PromoteToClanLeader(playerId, flag);
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x0003325C File Offset: 0x0003145C
		private void PromoteToClanOfficer(PlayerId playerId)
		{
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(playerId);
			NetworkMain.GameClient.AssignAsClanOfficer(playerId, flag);
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x0003328E File Offset: 0x0003148E
		private void DemoteFromClanOfficer(PlayerId playerId)
		{
			NetworkMain.GameClient.RemoveClanOfficerRoleForPlayer(playerId);
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x0003329B File Offset: 0x0003149B
		private void KickFromClan(PlayerId playerId)
		{
			NetworkMain.GameClient.KickFromClan(playerId);
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06000F45 RID: 3909 RVA: 0x000332A8 File Offset: 0x000314A8
		// (set) Token: 0x06000F46 RID: 3910 RVA: 0x000332B0 File Offset: 0x000314B0
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06000F47 RID: 3911 RVA: 0x000332CE File Offset: 0x000314CE
		// (set) Token: 0x06000F48 RID: 3912 RVA: 0x000332D6 File Offset: 0x000314D6
		[DataSourceProperty]
		public bool IsMemberActionsActive
		{
			get
			{
				return this._isMemberActionsActive;
			}
			set
			{
				if (value != this._isMemberActionsActive)
				{
					this._isMemberActionsActive = value;
					base.OnPropertyChangedWithValue(value, "IsMemberActionsActive");
				}
			}
		}

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06000F49 RID: 3913 RVA: 0x000332F4 File Offset: 0x000314F4
		// (set) Token: 0x06000F4A RID: 3914 RVA: 0x000332FC File Offset: 0x000314FC
		[DataSourceProperty]
		public bool IsPrivilegedMember
		{
			get
			{
				return this._isPrivilegedMember;
			}
			set
			{
				if (value != this._isPrivilegedMember)
				{
					this._isPrivilegedMember = value;
					base.OnPropertyChangedWithValue(value, "IsPrivilegedMember");
				}
			}
		}

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06000F4B RID: 3915 RVA: 0x0003331A File Offset: 0x0003151A
		// (set) Token: 0x06000F4C RID: 3916 RVA: 0x00033322 File Offset: 0x00031522
		[DataSourceProperty]
		public string RosterText
		{
			get
			{
				return this._rosterText;
			}
			set
			{
				if (value != this._rosterText)
				{
					this._rosterText = value;
					base.OnPropertyChangedWithValue<string>(value, "RosterText");
				}
			}
		}

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06000F4D RID: 3917 RVA: 0x00033345 File Offset: 0x00031545
		// (set) Token: 0x06000F4E RID: 3918 RVA: 0x0003334D File Offset: 0x0003154D
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06000F4F RID: 3919 RVA: 0x00033370 File Offset: 0x00031570
		// (set) Token: 0x06000F50 RID: 3920 RVA: 0x00033378 File Offset: 0x00031578
		[DataSourceProperty]
		public string BadgeText
		{
			get
			{
				return this._badgeText;
			}
			set
			{
				if (value != this._badgeText)
				{
					this._badgeText = value;
					base.OnPropertyChangedWithValue<string>(value, "BadgeText");
				}
			}
		}

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06000F51 RID: 3921 RVA: 0x0003339B File Offset: 0x0003159B
		// (set) Token: 0x06000F52 RID: 3922 RVA: 0x000333A3 File Offset: 0x000315A3
		[DataSourceProperty]
		public string StatusText
		{
			get
			{
				return this._statusText;
			}
			set
			{
				if (value != this._statusText)
				{
					this._statusText = value;
					base.OnPropertyChangedWithValue<string>(value, "StatusText");
				}
			}
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06000F53 RID: 3923 RVA: 0x000333C6 File Offset: 0x000315C6
		// (set) Token: 0x06000F54 RID: 3924 RVA: 0x000333CE File Offset: 0x000315CE
		[DataSourceProperty]
		public MBBindingList<MPLobbyClanMemberItemVM> MembersList
		{
			get
			{
				return this._membersList;
			}
			set
			{
				if (value != this._membersList)
				{
					this._membersList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyClanMemberItemVM>>(value, "MembersList");
				}
			}
		}

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06000F55 RID: 3925 RVA: 0x000333EC File Offset: 0x000315EC
		// (set) Token: 0x06000F56 RID: 3926 RVA: 0x000333F4 File Offset: 0x000315F4
		[DataSourceProperty]
		public MBBindingList<StringPairItemWithActionVM> MemberActionsList
		{
			get
			{
				return this._memberActionsList;
			}
			set
			{
				if (value != this._memberActionsList)
				{
					this._memberActionsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemWithActionVM>>(value, "MemberActionsList");
				}
			}
		}

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06000F57 RID: 3927 RVA: 0x00033412 File Offset: 0x00031612
		// (set) Token: 0x06000F58 RID: 3928 RVA: 0x0003341A File Offset: 0x0003161A
		[DataSourceProperty]
		public HintViewModel PromoteToClanOfficerHint
		{
			get
			{
				return this._promoteToClanOfficerHint;
			}
			set
			{
				if (value != this._promoteToClanOfficerHint)
				{
					this._promoteToClanOfficerHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PromoteToClanOfficerHint");
				}
			}
		}

		// Token: 0x04000731 RID: 1841
		private bool _isClanLeader;

		// Token: 0x04000732 RID: 1842
		private bool _isClanOfficer;

		// Token: 0x04000733 RID: 1843
		private MPLobbyClanRosterVM.MemberComparer _memberComparer;

		// Token: 0x04000734 RID: 1844
		private bool _isSelected;

		// Token: 0x04000735 RID: 1845
		private bool _isMemberActionsActive;

		// Token: 0x04000736 RID: 1846
		private bool _isPrivilegedMember;

		// Token: 0x04000737 RID: 1847
		private string _rosterText;

		// Token: 0x04000738 RID: 1848
		private string _nameText;

		// Token: 0x04000739 RID: 1849
		private string _badgeText;

		// Token: 0x0400073A RID: 1850
		private string _statusText;

		// Token: 0x0400073B RID: 1851
		private MBBindingList<MPLobbyClanMemberItemVM> _membersList;

		// Token: 0x0400073C RID: 1852
		private MBBindingList<StringPairItemWithActionVM> _memberActionsList;

		// Token: 0x0400073D RID: 1853
		private HintViewModel _promoteToClanOfficerHint;

		// Token: 0x020001F4 RID: 500
		private class MemberComparer : IComparer<MPLobbyClanMemberItemVM>
		{
			// Token: 0x06001AA2 RID: 6818 RVA: 0x00056374 File Offset: 0x00054574
			public int Compare(MPLobbyClanMemberItemVM x, MPLobbyClanMemberItemVM y)
			{
				if (y.Rank != x.Rank)
				{
					return y.Rank.CompareTo(x.Rank);
				}
				return y.IsOnline.CompareTo(x.IsOnline);
			}
		}
	}
}
