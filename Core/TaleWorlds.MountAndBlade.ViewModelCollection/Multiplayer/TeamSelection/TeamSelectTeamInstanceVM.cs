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
	public class TeamSelectTeamInstanceVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DisplayedPrimary = ((this._culture == null) ? new TextObject("{=pSheKLB4}Spectator", null).ToString() : this._culture.Name.ToString());
		}

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

		private void UpdateTeamScores()
		{
			if (this._missionScoreboardSide != null)
			{
				this.Score = this._missionScoreboardSide.SideScore;
			}
		}

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

		[UsedImplicitly]
		public void ExecuteSelectTeam()
		{
			if (this._onSelect != null)
			{
				this._onSelect(this.Team);
			}
		}

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

		private const int MaxFriendAvatarCount = 6;

		public readonly Team Team;

		public readonly Action<Team> _onSelect;

		private readonly List<MPPlayerVM> _friends;

		private MissionScoreboardComponent _missionScoreboardComponent;

		private MissionScoreboardComponent.MissionScoreboardSide _missionScoreboardSide;

		private readonly BasicCultureObject _culture;

		private bool _isDisabled;

		private string _displayedPrimary;

		private string _displayedSecondary;

		private string _displayedSecondarySub;

		private string _lockText;

		private string _cultureId;

		private int _score;

		private ImageIdentifierVM _banner;

		private MBBindingList<MPPlayerVM> _friendAvatars;

		private bool _hasExtraFriends;

		private bool _useSecondary;

		private bool _isAttacker;

		private bool _isSiege;

		private string _friendsExtraText;

		private HintViewModel _friendsExtraHint;
	}
}
