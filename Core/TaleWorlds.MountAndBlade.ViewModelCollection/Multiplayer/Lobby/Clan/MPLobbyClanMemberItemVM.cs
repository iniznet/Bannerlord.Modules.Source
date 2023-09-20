using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x0200009D RID: 157
	public class MPLobbyClanMemberItemVM : MPLobbyPlayerBaseVM
	{
		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06000ED8 RID: 3800 RVA: 0x00031F28 File Offset: 0x00030128
		// (set) Token: 0x06000ED9 RID: 3801 RVA: 0x00031F30 File Offset: 0x00030130
		public PlayerId Id { get; private set; }

		// Token: 0x06000EDA RID: 3802 RVA: 0x00031F39 File Offset: 0x00030139
		public MPLobbyClanMemberItemVM(PlayerId playerId)
			: base(playerId, "", null, null)
		{
			this.Id = playerId;
			this.RefreshValues();
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x00031F58 File Offset: 0x00030158
		public MPLobbyClanMemberItemVM(ClanPlayer member, bool isOnline, string selectedBadgeID, AnotherPlayerState state, Action<MPLobbyClanMemberItemVM> executeActivate = null)
			: base(member.PlayerId, "", null, null)
		{
			this._member = member;
			this.Id = this._member.PlayerId;
			this.IsOnline = isOnline;
			this._executeActivate = executeActivate;
			base.SelectedBadgeID = selectedBadgeID;
			base.StateText = GameTexts.FindText("str_multiplayer_lobby_state", state.ToString()).ToString();
			this.IsClanLeader = this._member.Role == ClanPlayerRole.Leader;
			this.Rank = (int)this._member.Role;
			this.RankHint = new HintViewModel();
			this.RefreshValues();
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x00032000 File Offset: 0x00030200
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NotEligibleInfo = "";
			ClanPlayer member = this._member;
			if (member != null && member.Role == ClanPlayerRole.Leader)
			{
				this.RankHint.HintText = new TextObject("{=SrfYbg3x}Leader", null);
				return;
			}
			ClanPlayer member2 = this._member;
			if (member2 != null && member2.Role == ClanPlayerRole.Officer)
			{
				this.RankHint.HintText = new TextObject("{=ZYF2t1VI}Officer", null);
			}
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x0003207C File Offset: 0x0003027C
		public void SetNotEligibleInfo(PlayerNotEligibleError notEligibleError)
		{
			string text = "";
			if (notEligibleError == PlayerNotEligibleError.AlreadyInClan)
			{
				text = new TextObject("{=zEMWM4h3}Already In a Clan", null).ToString();
			}
			else if (notEligibleError == PlayerNotEligibleError.NotAtLobby)
			{
				text = new TextObject("{=hPbppi6E}Not At The Lobby", null).ToString();
			}
			else if (notEligibleError == PlayerNotEligibleError.DoesNotSupportFeature)
			{
				text = new TextObject("{=MsokbMx2}Does not support Clan feature", null).ToString();
			}
			if (string.IsNullOrEmpty(this.NotEligibleInfo))
			{
				this.NotEligibleInfo = text;
				return;
			}
			GameTexts.SetVariable("LEFT", this.NotEligibleInfo);
			GameTexts.SetVariable("RIGHT", text);
			this.NotEligibleInfo = GameTexts.FindText("str_LEFT_comma_RIGHT", null).ToString();
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x00032117 File Offset: 0x00030317
		private void ExecuteSelection()
		{
			Action<MPLobbyClanMemberItemVM> executeActivate = this._executeActivate;
			if (executeActivate == null)
			{
				return;
			}
			executeActivate(this);
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06000EDF RID: 3807 RVA: 0x0003212A File Offset: 0x0003032A
		// (set) Token: 0x06000EE0 RID: 3808 RVA: 0x00032132 File Offset: 0x00030332
		[DataSourceProperty]
		public bool IsOnline
		{
			get
			{
				return this._isOnline;
			}
			set
			{
				if (value != this._isOnline)
				{
					this._isOnline = value;
					base.OnPropertyChangedWithValue(value, "IsOnline");
				}
			}
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06000EE1 RID: 3809 RVA: 0x00032150 File Offset: 0x00030350
		// (set) Token: 0x06000EE2 RID: 3810 RVA: 0x00032158 File Offset: 0x00030358
		[DataSourceProperty]
		public bool IsClanLeader
		{
			get
			{
				return this._isClanLeader;
			}
			set
			{
				if (value != this._isClanLeader)
				{
					this._isClanLeader = value;
					base.OnPropertyChangedWithValue(value, "IsClanLeader");
				}
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06000EE3 RID: 3811 RVA: 0x00032176 File Offset: 0x00030376
		// (set) Token: 0x06000EE4 RID: 3812 RVA: 0x0003217E File Offset: 0x0003037E
		[DataSourceProperty]
		public string NotEligibleInfo
		{
			get
			{
				return this._notEligibleInfo;
			}
			set
			{
				if (value != this._notEligibleInfo)
				{
					this._notEligibleInfo = value;
					base.OnPropertyChangedWithValue<string>(value, "NotEligibleInfo");
				}
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06000EE5 RID: 3813 RVA: 0x000321A1 File Offset: 0x000303A1
		// (set) Token: 0x06000EE6 RID: 3814 RVA: 0x000321A9 File Offset: 0x000303A9
		[DataSourceProperty]
		public string InviteAcceptInfo
		{
			get
			{
				return this._inviteAcceptInfo;
			}
			set
			{
				if (value != this._inviteAcceptInfo)
				{
					this._inviteAcceptInfo = value;
					base.OnPropertyChangedWithValue<string>(value, "InviteAcceptInfo");
				}
			}
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06000EE7 RID: 3815 RVA: 0x000321CC File Offset: 0x000303CC
		// (set) Token: 0x06000EE8 RID: 3816 RVA: 0x000321D4 File Offset: 0x000303D4
		[DataSourceProperty]
		public int Rank
		{
			get
			{
				return this._rank;
			}
			set
			{
				if (value != this._rank)
				{
					this._rank = value;
					base.OnPropertyChangedWithValue(value, "Rank");
				}
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06000EE9 RID: 3817 RVA: 0x000321F2 File Offset: 0x000303F2
		// (set) Token: 0x06000EEA RID: 3818 RVA: 0x000321FA File Offset: 0x000303FA
		[DataSourceProperty]
		public MBBindingList<StringPairItemWithActionVM> UserActionsList
		{
			get
			{
				return this._userActionsList;
			}
			set
			{
				if (value != this._userActionsList)
				{
					this._userActionsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemWithActionVM>>(value, "UserActionsList");
				}
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06000EEB RID: 3819 RVA: 0x00032218 File Offset: 0x00030418
		// (set) Token: 0x06000EEC RID: 3820 RVA: 0x00032220 File Offset: 0x00030420
		[DataSourceProperty]
		public HintViewModel RankHint
		{
			get
			{
				return this._rankHint;
			}
			set
			{
				if (value != this._rankHint)
				{
					this._rankHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RankHint");
				}
			}
		}

		// Token: 0x0400070B RID: 1803
		private ClanPlayer _member;

		// Token: 0x0400070D RID: 1805
		private Action<MPLobbyClanMemberItemVM> _executeActivate;

		// Token: 0x0400070E RID: 1806
		private bool _isOnline;

		// Token: 0x0400070F RID: 1807
		private bool _isClanLeader;

		// Token: 0x04000710 RID: 1808
		private string _notEligibleInfo;

		// Token: 0x04000711 RID: 1809
		private string _inviteAcceptInfo;

		// Token: 0x04000712 RID: 1810
		private int _rank;

		// Token: 0x04000713 RID: 1811
		private MBBindingList<StringPairItemWithActionVM> _userActionsList;

		// Token: 0x04000714 RID: 1812
		private HintViewModel _rankHint;
	}
}
