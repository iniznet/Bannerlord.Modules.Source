using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.TeamSelection
{
	// Token: 0x0200004C RID: 76
	public class TeamSelectTeamInstanceVM : ViewModel
	{
		// Token: 0x0600063F RID: 1599 RVA: 0x00019CE8 File Offset: 0x00017EE8
		public TeamSelectTeamInstanceVM(MissionScoreboardComponent missionScoreboardComponent, Team team, BasicCultureObject culture, BannerCode bannercode, Action<Team> onSelect, bool useSecondary)
		{
			this.Team = team;
			this.UseSecondary = useSecondary;
			this._onSelect = onSelect;
			this._culture = culture;
			Mission mission = Mission.Current;
			this.IsSiege = mission != null && mission.HasMissionBehavior<MissionMultiplayerSiegeClient>();
			if (this.Team != null && this.Team.Side != BattleSideEnum.None)
			{
				this._missionScoreboardComponent = missionScoreboardComponent;
				this._missionScoreboardComponent.OnRoundPropertiesChanged += this.UpdateTeamScores;
				this._missionScoreboardSide = this._missionScoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == this.Team.Side);
				this.IsAttacker = this.Team.Side == BattleSideEnum.Attacker;
				this.UpdateTeamScores();
			}
			this.CultureId = ((culture == null) ? "" : culture.StringId);
			if (team == null)
			{
				this.IsDisabled = true;
			}
			if (bannercode == null)
			{
				this.Banner = new ImageIdentifierVM(ImageIdentifierType.Null);
			}
			else
			{
				this.Banner = new ImageIdentifierVM(bannercode, true);
			}
			this._friends = new List<MPPlayerVM>();
			this.FriendAvatars = new MBBindingList<MPPlayerVM>();
			this.RefreshValues();
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00019E02 File Offset: 0x00018002
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DisplayedPrimary = ((this._culture == null) ? new TextObject("{=pSheKLB4}Spectator", null).ToString() : this._culture.Name.ToString());
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x00019E3A File Offset: 0x0001803A
		public override void OnFinalize()
		{
			if (this._missionScoreboardComponent != null)
			{
				this._missionScoreboardComponent.OnRoundPropertiesChanged -= this.UpdateTeamScores;
			}
			this._missionScoreboardComponent = null;
			this._missionScoreboardSide = null;
			base.OnFinalize();
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00019E6F File Offset: 0x0001806F
		private void UpdateTeamScores()
		{
			if (this._missionScoreboardSide != null)
			{
				this.Score = this._missionScoreboardSide.SideScore;
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x00019E8C File Offset: 0x0001808C
		public void RefreshFriends(IEnumerable<MissionPeer> friends)
		{
			List<MissionPeer> list = friends.ToList<MissionPeer>();
			List<MPPlayerVM> list2 = new List<MPPlayerVM>();
			foreach (MPPlayerVM mpplayerVM in this._friends)
			{
				if (!list.Contains(mpplayerVM.Peer))
				{
					list2.Add(mpplayerVM);
				}
			}
			foreach (MPPlayerVM mpplayerVM2 in list2)
			{
				this._friends.Remove(mpplayerVM2);
			}
			List<MissionPeer> list3 = this._friends.Select((MPPlayerVM x) => x.Peer).ToList<MissionPeer>();
			foreach (MissionPeer missionPeer in list)
			{
				if (!list3.Contains(missionPeer))
				{
					this._friends.Add(new MPPlayerVM(missionPeer));
				}
			}
			this.FriendAvatars.Clear();
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "RefreshFriends");
			for (int i = 0; i < this._friends.Count; i++)
			{
				if (i < 6)
				{
					this.FriendAvatars.Add(this._friends[i]);
				}
				else
				{
					mbstringBuilder.AppendLine<string>(this._friends[i].Peer.DisplayedName);
				}
			}
			int num = this._friends.Count - 6;
			if (num > 0)
			{
				this.HasExtraFriends = true;
				TextObject textObject = new TextObject("{=hbwp3g3k}+{FRIEND_COUNT} {newline} {?PLURAL}friends{?}friend{\\?}", null);
				textObject.SetTextVariable("FRIEND_COUNT", num);
				textObject.SetTextVariable("PLURAL", (num == 1) ? 0 : 1);
				this.FriendsExtraText = textObject.ToString();
				this.FriendsExtraHint = new HintViewModel(textObject, null);
				return;
			}
			mbstringBuilder.Release();
			this.HasExtraFriends = false;
			this.FriendsExtraText = "";
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x0001A0C4 File Offset: 0x000182C4
		public void SetIsDisabled(bool isCurrentTeam, bool disabledForBalance)
		{
			this.IsDisabled = isCurrentTeam || disabledForBalance;
			if (isCurrentTeam)
			{
				this.LockText = new TextObject("{=SoQcsslF}CURRENT TEAM", null).ToString();
				return;
			}
			if (disabledForBalance)
			{
				this.LockText = new TextObject("{=qe46yXVJ}LOCKED FOR BALANCE", null).ToString();
			}
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x0001A102 File Offset: 0x00018302
		[UsedImplicitly]
		public void ExecuteSelectTeam()
		{
			if (this._onSelect != null)
			{
				this._onSelect(this.Team);
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000646 RID: 1606 RVA: 0x0001A11D File Offset: 0x0001831D
		// (set) Token: 0x06000647 RID: 1607 RVA: 0x0001A125 File Offset: 0x00018325
		[DataSourceProperty]
		public string CultureId
		{
			get
			{
				return this._cultureId;
			}
			set
			{
				if (this._cultureId != value)
				{
					this._cultureId = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureId");
				}
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000648 RID: 1608 RVA: 0x0001A148 File Offset: 0x00018348
		// (set) Token: 0x06000649 RID: 1609 RVA: 0x0001A150 File Offset: 0x00018350
		[DataSourceProperty]
		public int Score
		{
			get
			{
				return this._score;
			}
			set
			{
				if (value != this._score)
				{
					this._score = value;
					base.OnPropertyChangedWithValue(value, "Score");
				}
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x0600064A RID: 1610 RVA: 0x0001A16E File Offset: 0x0001836E
		// (set) Token: 0x0600064B RID: 1611 RVA: 0x0001A176 File Offset: 0x00018376
		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (this._isDisabled != value)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x0600064C RID: 1612 RVA: 0x0001A194 File Offset: 0x00018394
		// (set) Token: 0x0600064D RID: 1613 RVA: 0x0001A19C File Offset: 0x0001839C
		[DataSourceProperty]
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (this._useSecondary != value)
				{
					this._useSecondary = value;
					base.OnPropertyChangedWithValue(value, "UseSecondary");
				}
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x0001A1BA File Offset: 0x000183BA
		// (set) Token: 0x0600064F RID: 1615 RVA: 0x0001A1C2 File Offset: 0x000183C2
		[DataSourceProperty]
		public bool IsAttacker
		{
			get
			{
				return this._isAttacker;
			}
			set
			{
				if (this._isAttacker != value)
				{
					this._isAttacker = value;
					base.OnPropertyChangedWithValue(value, "IsAttacker");
				}
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000650 RID: 1616 RVA: 0x0001A1E0 File Offset: 0x000183E0
		// (set) Token: 0x06000651 RID: 1617 RVA: 0x0001A1E8 File Offset: 0x000183E8
		[DataSourceProperty]
		public bool IsSiege
		{
			get
			{
				return this._isSiege;
			}
			set
			{
				if (this._isSiege != value)
				{
					this._isSiege = value;
					base.OnPropertyChangedWithValue(value, "IsSiege");
				}
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000652 RID: 1618 RVA: 0x0001A206 File Offset: 0x00018406
		// (set) Token: 0x06000653 RID: 1619 RVA: 0x0001A20E File Offset: 0x0001840E
		[DataSourceProperty]
		public string DisplayedPrimary
		{
			get
			{
				return this._displayedPrimary;
			}
			set
			{
				this._displayedPrimary = value;
				base.OnPropertyChangedWithValue<string>(value, "DisplayedPrimary");
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000654 RID: 1620 RVA: 0x0001A223 File Offset: 0x00018423
		// (set) Token: 0x06000655 RID: 1621 RVA: 0x0001A22B File Offset: 0x0001842B
		[DataSourceProperty]
		public string DisplayedSecondary
		{
			get
			{
				return this._displayedSecondary;
			}
			set
			{
				this._displayedSecondary = value;
				base.OnPropertyChangedWithValue<string>(value, "DisplayedSecondary");
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000656 RID: 1622 RVA: 0x0001A240 File Offset: 0x00018440
		// (set) Token: 0x06000657 RID: 1623 RVA: 0x0001A248 File Offset: 0x00018448
		[DataSourceProperty]
		public string DisplayedSecondarySub
		{
			get
			{
				return this._displayedSecondarySub;
			}
			set
			{
				this._displayedSecondarySub = value;
				base.OnPropertyChangedWithValue<string>(value, "DisplayedSecondarySub");
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x0001A25D File Offset: 0x0001845D
		// (set) Token: 0x06000659 RID: 1625 RVA: 0x0001A265 File Offset: 0x00018465
		[DataSourceProperty]
		public string LockText
		{
			get
			{
				return this._lockText;
			}
			set
			{
				this._lockText = value;
				base.OnPropertyChangedWithValue<string>(value, "LockText");
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x0600065A RID: 1626 RVA: 0x0001A27A File Offset: 0x0001847A
		// (set) Token: 0x0600065B RID: 1627 RVA: 0x0001A284 File Offset: 0x00018484
		[DataSourceProperty]
		public ImageIdentifierVM Banner
		{
			get
			{
				return this._banner;
			}
			set
			{
				if (value != this._banner && (value == null || this._banner == null || this._banner.Id != value.Id))
				{
					this._banner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner");
				}
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x0600065C RID: 1628 RVA: 0x0001A2D0 File Offset: 0x000184D0
		// (set) Token: 0x0600065D RID: 1629 RVA: 0x0001A2D8 File Offset: 0x000184D8
		[DataSourceProperty]
		public MBBindingList<MPPlayerVM> FriendAvatars
		{
			get
			{
				return this._friendAvatars;
			}
			set
			{
				if (this._friendAvatars != value)
				{
					this._friendAvatars = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPPlayerVM>>(value, "FriendAvatars");
				}
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x0600065E RID: 1630 RVA: 0x0001A2F6 File Offset: 0x000184F6
		// (set) Token: 0x0600065F RID: 1631 RVA: 0x0001A2FE File Offset: 0x000184FE
		[DataSourceProperty]
		public bool HasExtraFriends
		{
			get
			{
				return this._hasExtraFriends;
			}
			set
			{
				if (this._hasExtraFriends != value)
				{
					this._hasExtraFriends = value;
					base.OnPropertyChangedWithValue(value, "HasExtraFriends");
				}
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x0001A31C File Offset: 0x0001851C
		// (set) Token: 0x06000661 RID: 1633 RVA: 0x0001A324 File Offset: 0x00018524
		[DataSourceProperty]
		public string FriendsExtraText
		{
			get
			{
				return this._friendsExtraText;
			}
			set
			{
				if (this._friendsExtraText != value)
				{
					this._friendsExtraText = value;
					base.OnPropertyChangedWithValue<string>(value, "FriendsExtraText");
				}
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x0001A347 File Offset: 0x00018547
		// (set) Token: 0x06000663 RID: 1635 RVA: 0x0001A34F File Offset: 0x0001854F
		[DataSourceProperty]
		public HintViewModel FriendsExtraHint
		{
			get
			{
				return this._friendsExtraHint;
			}
			set
			{
				if (this._friendsExtraHint != value)
				{
					this._friendsExtraHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FriendsExtraHint");
				}
			}
		}

		// Token: 0x04000329 RID: 809
		private const int MaxFriendAvatarCount = 6;

		// Token: 0x0400032A RID: 810
		public readonly Team Team;

		// Token: 0x0400032B RID: 811
		public readonly Action<Team> _onSelect;

		// Token: 0x0400032C RID: 812
		private readonly List<MPPlayerVM> _friends;

		// Token: 0x0400032D RID: 813
		private MissionScoreboardComponent _missionScoreboardComponent;

		// Token: 0x0400032E RID: 814
		private MissionScoreboardComponent.MissionScoreboardSide _missionScoreboardSide;

		// Token: 0x0400032F RID: 815
		private readonly BasicCultureObject _culture;

		// Token: 0x04000330 RID: 816
		private bool _isDisabled;

		// Token: 0x04000331 RID: 817
		private string _displayedPrimary;

		// Token: 0x04000332 RID: 818
		private string _displayedSecondary;

		// Token: 0x04000333 RID: 819
		private string _displayedSecondarySub;

		// Token: 0x04000334 RID: 820
		private string _lockText;

		// Token: 0x04000335 RID: 821
		private string _cultureId;

		// Token: 0x04000336 RID: 822
		private int _score;

		// Token: 0x04000337 RID: 823
		private ImageIdentifierVM _banner;

		// Token: 0x04000338 RID: 824
		private MBBindingList<MPPlayerVM> _friendAvatars;

		// Token: 0x04000339 RID: 825
		private bool _hasExtraFriends;

		// Token: 0x0400033A RID: 826
		private bool _useSecondary;

		// Token: 0x0400033B RID: 827
		private bool _isAttacker;

		// Token: 0x0400033C RID: 828
		private bool _isSiege;

		// Token: 0x0400033D RID: 829
		private string _friendsExtraText;

		// Token: 0x0400033E RID: 830
		private HintViewModel _friendsExtraHint;
	}
}
