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
	public class MPLobbyClanMemberItemVM : MPLobbyPlayerBaseVM
	{
		public PlayerId Id { get; private set; }

		public MPLobbyClanMemberItemVM(PlayerId playerId)
			: base(playerId, "", null, null)
		{
			this.Id = playerId;
			this.RefreshValues();
		}

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

		private void ExecuteSelection()
		{
			Action<MPLobbyClanMemberItemVM> executeActivate = this._executeActivate;
			if (executeActivate == null)
			{
				return;
			}
			executeActivate(this);
		}

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

		private ClanPlayer _member;

		private Action<MPLobbyClanMemberItemVM> _executeActivate;

		private bool _isOnline;

		private bool _isClanLeader;

		private string _notEligibleInfo;

		private string _inviteAcceptInfo;

		private int _rank;

		private MBBindingList<StringPairItemWithActionVM> _userActionsList;

		private HintViewModel _rankHint;
	}
}
