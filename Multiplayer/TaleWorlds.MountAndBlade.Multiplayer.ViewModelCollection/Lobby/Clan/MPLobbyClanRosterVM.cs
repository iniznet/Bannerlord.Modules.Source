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

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Clan
{
	public class MPLobbyClanRosterVM : ViewModel
	{
		public MPLobbyClanRosterVM()
		{
			this.MembersList = new MBBindingList<MPLobbyClanMemberItemVM>();
			this.MemberActionsList = new MBBindingList<StringPairItemWithActionVM>();
			this._memberComparer = new MPLobbyClanRosterVM.MemberComparer();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RosterText = new TextObject("{=oyVeCtlg}Roster", null).ToString();
			this.NameText = new TextObject("{=PDdh1sBj}Name", null).ToString();
			this.BadgeText = new TextObject("{=4PrfimcK}Badge", null).ToString();
			this.StatusText = new TextObject("{=DXczLzml}Status", null).ToString();
			this.PromoteToClanOfficerHint = new HintViewModel(new TextObject("{=oeSrXaKt}You need to demote one of the officers", null), null);
		}

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
						bool flag = clanPlayerInfo.State == 2 || clanPlayerInfo.State == 4;
						this.MembersList.Add(new MPLobbyClanMemberItemVM(member, flag, clanPlayerInfo.ActiveBadgeId, clanPlayerInfo.State, new Action<MPLobbyClanMemberItemVM>(this.ExecutePopulateActionsList)));
					}
				}
			}
			this.MembersList.Sort(this._memberComparer);
			this.IsPrivilegedMember = NetworkMain.GameClient.IsClanLeader || NetworkMain.GameClient.IsClanOfficer;
		}

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
						if (NetworkMain.GameClient.PlayersInClan.Count((ClanPlayer m) => m.Role == 1) == Parameters.ClanOfficerCount)
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
				if (NetworkMain.GameClient.SupportedFeatures.SupportsFeatures(4))
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

		private void ExecuteRequestFriendship(object memberObj)
		{
			MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = memberObj as MPLobbyClanMemberItemVM;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(mplobbyClanMemberItemVM.Id);
			NetworkMain.GameClient.AddFriend(mplobbyClanMemberItemVM.Id, flag);
		}

		private void ExecuteTerminateFriendship(object memberObj)
		{
			MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = memberObj as MPLobbyClanMemberItemVM;
			NetworkMain.GameClient.RemoveFriend(mplobbyClanMemberItemVM.Id);
		}

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

		private void ExecuteInviteToParty(object memberObj)
		{
			MPLobbyClanMemberItemVM mplobbyClanMemberItemVM = memberObj as MPLobbyClanMemberItemVM;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(mplobbyClanMemberItemVM.Id);
			NetworkMain.GameClient.InviteToParty(mplobbyClanMemberItemVM.Id, flag);
		}

		private void ExecuteViewProfile(object memberObj)
		{
			(memberObj as MPLobbyClanMemberItemVM).ExecuteShowProfile();
		}

		private void PromoteToClanLeader(PlayerId playerId)
		{
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(playerId);
			NetworkMain.GameClient.PromoteToClanLeader(playerId, flag);
		}

		private void PromoteToClanOfficer(PlayerId playerId)
		{
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(playerId);
			NetworkMain.GameClient.AssignAsClanOfficer(playerId, flag);
		}

		private void DemoteFromClanOfficer(PlayerId playerId)
		{
			NetworkMain.GameClient.RemoveClanOfficerRoleForPlayer(playerId);
		}

		private void KickFromClan(PlayerId playerId)
		{
			NetworkMain.GameClient.KickFromClan(playerId);
		}

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

		private bool _isClanLeader;

		private bool _isClanOfficer;

		private MPLobbyClanRosterVM.MemberComparer _memberComparer;

		private bool _isSelected;

		private bool _isMemberActionsActive;

		private bool _isPrivilegedMember;

		private string _rosterText;

		private string _nameText;

		private string _badgeText;

		private string _statusText;

		private MBBindingList<MPLobbyClanMemberItemVM> _membersList;

		private MBBindingList<StringPairItemWithActionVM> _memberActionsList;

		private HintViewModel _promoteToClanOfficerHint;

		private class MemberComparer : IComparer<MPLobbyClanMemberItemVM>
		{
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
