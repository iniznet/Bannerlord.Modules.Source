using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	// Token: 0x02000051 RID: 81
	public class MissionScoreboardSideVM : ViewModel
	{
		// Token: 0x060006A7 RID: 1703 RVA: 0x0001ACDC File Offset: 0x00018EDC
		public MissionScoreboardSideVM(MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide, Action<MissionScoreboardPlayerVM> executeActivate, bool isSingleSide, bool isSecondSide)
		{
			this._executeActivate = executeActivate;
			this._missionScoreboardSide = missionScoreboardSide;
			this._playersMap = new Dictionary<MissionPeer, MissionScoreboardPlayerVM>();
			this.Players = new MBBindingList<MissionScoreboardPlayerVM>();
			this.PlayerSortController = new MissionScoreboardPlayerSortControllerVM(ref this._players);
			this._avatarHeaderIndex = missionScoreboardSide.GetHeaderIds().IndexOf("avatar");
			int score = missionScoreboardSide.GetScore(null);
			string[] valuesOf = missionScoreboardSide.GetValuesOf(null);
			string[] headerIds = missionScoreboardSide.GetHeaderIds();
			this._bot = new MissionScoreboardPlayerVM(valuesOf, headerIds, score, this._executeActivate);
			foreach (MissionPeer missionPeer in missionScoreboardSide.Players)
			{
				this.AddPlayer(missionPeer);
			}
			this.UpdateBotAttributes();
			this.UpdateRoundAttributes();
			string text = ((this._missionScoreboardSide.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(text);
			this.UseSecondary = this._missionScoreboardSide.Side == BattleSideEnum.Defender;
			this.IsSingleSide = isSingleSide;
			this.IsSecondSide = isSecondSide;
			this.CultureId = text;
			this.TeamColor = "0x" + @object.Color2.ToString("X");
			this.ShowAttackerOrDefenderIcons = Mission.Current.HasMissionBehavior<MissionMultiplayerSiegeClient>();
			this.IsAttacker = missionScoreboardSide.Side == BattleSideEnum.Attacker;
			this.RefreshValues();
			NetworkCommunicator.OnPeerAveragePingUpdated += this.OnPeerPingUpdated;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0001AED0 File Offset: 0x000190D0
		public override void RefreshValues()
		{
			base.RefreshValues();
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>((this._missionScoreboardSide.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			if (this.IsSingleSide)
			{
				this.Name = MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			}
			else
			{
				this.Name = @object.Name.ToString();
			}
			this.EntryProperties = new MBBindingList<MissionScoreboardHeaderItemVM>();
			string[] headerIds = this._missionScoreboardSide.GetHeaderIds();
			string[] headerNames = this._missionScoreboardSide.GetHeaderNames();
			for (int i = 0; i < headerIds.Length; i++)
			{
				this.EntryProperties.Add(new MissionScoreboardHeaderItemVM(this, headerIds[i], headerNames[i], headerIds[i] == "avatar", this._irregularHeaderIDs.Contains(headerIds[i])));
			}
			this.UpdatePlayersText();
			MissionScoreboardPlayerSortControllerVM playerSortController = this.PlayerSortController;
			if (playerSortController == null)
			{
				return;
			}
			playerSortController.RefreshValues();
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0001AFB0 File Offset: 0x000191B0
		public void Tick(float dt)
		{
			foreach (MissionScoreboardPlayerVM missionScoreboardPlayerVM in this.Players)
			{
				missionScoreboardPlayerVM.Tick(dt);
			}
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0001AFFC File Offset: 0x000191FC
		public override void OnFinalize()
		{
			base.OnFinalize();
			NetworkCommunicator.OnPeerAveragePingUpdated -= this.OnPeerPingUpdated;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0001B035 File Offset: 0x00019235
		public void UpdateRoundAttributes()
		{
			this.RoundsWon = this._missionScoreboardSide.SideScore;
			this.SortPlayers();
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x0001B050 File Offset: 0x00019250
		public void UpdateBotAttributes()
		{
			int num = ((this._missionScoreboardSide.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			if (num > 0)
			{
				int score = this._missionScoreboardSide.GetScore(null);
				string[] valuesOf = this._missionScoreboardSide.GetValuesOf(null);
				this._bot.UpdateAttributes(valuesOf, score);
				if (!this.Players.Contains(this._bot))
				{
					this.Players.Add(this._bot);
				}
			}
			else if (num == 0 && this.Players.Contains(this._bot))
			{
				this.Players.Remove(this._bot);
			}
			this.SortPlayers();
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0001B100 File Offset: 0x00019300
		public void UpdatePlayerAttributes(MissionPeer player)
		{
			if (this._playersMap.ContainsKey(player))
			{
				int score = this._missionScoreboardSide.GetScore(player);
				string[] valuesOf = this._missionScoreboardSide.GetValuesOf(player);
				this._playersMap[player].UpdateAttributes(valuesOf, score);
			}
			this.SortPlayers();
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0001B14E File Offset: 0x0001934E
		public void RemovePlayer(MissionPeer peer)
		{
			this.Players.Remove(this._playersMap[peer]);
			this._playersMap.Remove(peer);
			this.SortPlayers();
			this.UpdatePlayersText();
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0001B184 File Offset: 0x00019384
		public void AddPlayer(MissionPeer peer)
		{
			if (!this._playersMap.ContainsKey(peer))
			{
				int score = this._missionScoreboardSide.GetScore(peer);
				string[] valuesOf = this._missionScoreboardSide.GetValuesOf(peer);
				string[] headerIds = this._missionScoreboardSide.GetHeaderIds();
				MissionScoreboardPlayerVM missionScoreboardPlayerVM = new MissionScoreboardPlayerVM(peer, valuesOf, headerIds, score, this._executeActivate);
				this._playersMap.Add(peer, missionScoreboardPlayerVM);
				this.Players.Add(missionScoreboardPlayerVM);
			}
			this.SortPlayers();
			this.UpdatePlayersText();
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0001B1FC File Offset: 0x000193FC
		private void UpdatePlayersText()
		{
			TextObject textObject = new TextObject("{=R28ac5ij}{NUMBER} Players", null);
			textObject.SetTextVariable("NUMBER", this.Players.Count);
			this.PlayersText = textObject.ToString();
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0001B238 File Offset: 0x00019438
		private void SortPlayers()
		{
			this.PlayerSortController.SortByCurrentState();
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x0001B248 File Offset: 0x00019448
		private void OnPeerPingUpdated(NetworkCommunicator peer)
		{
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component != null)
			{
				this.UpdatePlayerAttributes(component);
			}
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x0001B268 File Offset: 0x00019468
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == ManagedOptions.ManagedOptionsType.EnableGenericAvatars)
			{
				foreach (MissionScoreboardPlayerVM missionScoreboardPlayerVM in this.Players)
				{
					missionScoreboardPlayerVM.RefreshAvatar();
				}
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060006B4 RID: 1716 RVA: 0x0001B2B8 File Offset: 0x000194B8
		// (set) Token: 0x060006B5 RID: 1717 RVA: 0x0001B2C0 File Offset: 0x000194C0
		[DataSourceProperty]
		public MBBindingList<MissionScoreboardPlayerVM> Players
		{
			get
			{
				return this._players;
			}
			set
			{
				if (this._players != value)
				{
					this._players = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionScoreboardPlayerVM>>(value, "Players");
				}
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x0001B2DE File Offset: 0x000194DE
		// (set) Token: 0x060006B7 RID: 1719 RVA: 0x0001B2E6 File Offset: 0x000194E6
		[DataSourceProperty]
		public MBBindingList<MissionScoreboardHeaderItemVM> EntryProperties
		{
			get
			{
				return this._entryProperties;
			}
			set
			{
				if (value != this._entryProperties)
				{
					this._entryProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionScoreboardHeaderItemVM>>(value, "EntryProperties");
				}
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x0001B304 File Offset: 0x00019504
		// (set) Token: 0x060006B9 RID: 1721 RVA: 0x0001B30C File Offset: 0x0001950C
		[DataSourceProperty]
		public MissionScoreboardPlayerSortControllerVM PlayerSortController
		{
			get
			{
				return this._playerSortController;
			}
			set
			{
				if (value != this._playerSortController)
				{
					this._playerSortController = value;
					base.OnPropertyChangedWithValue<MissionScoreboardPlayerSortControllerVM>(value, "PlayerSortController");
				}
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060006BA RID: 1722 RVA: 0x0001B32A File Offset: 0x0001952A
		// (set) Token: 0x060006BB RID: 1723 RVA: 0x0001B332 File Offset: 0x00019532
		[DataSourceProperty]
		public bool IsSingleSide
		{
			get
			{
				return this._isSingleSide;
			}
			set
			{
				if (value != this._isSingleSide)
				{
					this._isSingleSide = value;
					base.OnPropertyChangedWithValue(value, "IsSingleSide");
				}
			}
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060006BC RID: 1724 RVA: 0x0001B350 File Offset: 0x00019550
		// (set) Token: 0x060006BD RID: 1725 RVA: 0x0001B358 File Offset: 0x00019558
		[DataSourceProperty]
		public bool IsSecondSide
		{
			get
			{
				return this._isSecondSide;
			}
			set
			{
				if (value != this._isSecondSide)
				{
					this._isSecondSide = value;
					base.OnPropertyChangedWithValue(value, "IsSecondSide");
				}
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x0001B376 File Offset: 0x00019576
		// (set) Token: 0x060006BF RID: 1727 RVA: 0x0001B37E File Offset: 0x0001957E
		[DataSourceProperty]
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChangedWithValue(value, "UseSecondary");
				}
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x0001B39C File Offset: 0x0001959C
		// (set) Token: 0x060006C1 RID: 1729 RVA: 0x0001B3A4 File Offset: 0x000195A4
		[DataSourceProperty]
		public bool ShowAttackerOrDefenderIcons
		{
			get
			{
				return this._showAttackerOrDefenderIcons;
			}
			set
			{
				if (value != this._showAttackerOrDefenderIcons)
				{
					this._showAttackerOrDefenderIcons = value;
					base.OnPropertyChangedWithValue(value, "ShowAttackerOrDefenderIcons");
				}
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x0001B3C2 File Offset: 0x000195C2
		// (set) Token: 0x060006C3 RID: 1731 RVA: 0x0001B3CA File Offset: 0x000195CA
		[DataSourceProperty]
		public bool IsAttacker
		{
			get
			{
				return this._isAttacker;
			}
			set
			{
				if (value != this._isAttacker)
				{
					this._isAttacker = value;
					base.OnPropertyChangedWithValue(value, "IsAttacker");
				}
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x0001B3E8 File Offset: 0x000195E8
		// (set) Token: 0x060006C5 RID: 1733 RVA: 0x0001B3F0 File Offset: 0x000195F0
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x0001B413 File Offset: 0x00019613
		// (set) Token: 0x060006C7 RID: 1735 RVA: 0x0001B41B File Offset: 0x0001961B
		[DataSourceProperty]
		public string PlayersText
		{
			get
			{
				return this._playersText;
			}
			set
			{
				if (value != this._playersText)
				{
					this._playersText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayersText");
				}
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x0001B43E File Offset: 0x0001963E
		// (set) Token: 0x060006C9 RID: 1737 RVA: 0x0001B446 File Offset: 0x00019646
		[DataSourceProperty]
		public string CultureId
		{
			get
			{
				return this._cultureId;
			}
			set
			{
				if (value != this._cultureId)
				{
					this._cultureId = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureId");
				}
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x0001B469 File Offset: 0x00019669
		// (set) Token: 0x060006CB RID: 1739 RVA: 0x0001B471 File Offset: 0x00019671
		[DataSourceProperty]
		public int RoundsWon
		{
			get
			{
				return this._roundsWon;
			}
			set
			{
				if (this._roundsWon != value)
				{
					this._roundsWon = value;
					base.OnPropertyChangedWithValue(value, "RoundsWon");
				}
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060006CC RID: 1740 RVA: 0x0001B48F File Offset: 0x0001968F
		// (set) Token: 0x060006CD RID: 1741 RVA: 0x0001B497 File Offset: 0x00019697
		[DataSourceProperty]
		public string TeamColor
		{
			get
			{
				return this._teamColor;
			}
			set
			{
				if (value != this._teamColor)
				{
					this._teamColor = value;
					base.OnPropertyChangedWithValue<string>(value, "TeamColor");
				}
			}
		}

		// Token: 0x04000364 RID: 868
		private readonly MissionScoreboardComponent.MissionScoreboardSide _missionScoreboardSide;

		// Token: 0x04000365 RID: 869
		private readonly Dictionary<MissionPeer, MissionScoreboardPlayerVM> _playersMap;

		// Token: 0x04000366 RID: 870
		private MissionScoreboardPlayerVM _bot;

		// Token: 0x04000367 RID: 871
		private Action<MissionScoreboardPlayerVM> _executeActivate;

		// Token: 0x04000368 RID: 872
		private const string _avatarHeaderId = "avatar";

		// Token: 0x04000369 RID: 873
		private readonly int _avatarHeaderIndex;

		// Token: 0x0400036A RID: 874
		private List<string> _irregularHeaderIDs = new List<string> { "name", "avatar", "score", "kill", "assist" };

		// Token: 0x0400036B RID: 875
		private MBBindingList<MissionScoreboardPlayerVM> _players;

		// Token: 0x0400036C RID: 876
		private MBBindingList<MissionScoreboardHeaderItemVM> _entryProperties;

		// Token: 0x0400036D RID: 877
		private MissionScoreboardPlayerSortControllerVM _playerSortController;

		// Token: 0x0400036E RID: 878
		private bool _isSingleSide;

		// Token: 0x0400036F RID: 879
		private bool _isSecondSide;

		// Token: 0x04000370 RID: 880
		private bool _useSecondary;

		// Token: 0x04000371 RID: 881
		private bool _showAttackerOrDefenderIcons;

		// Token: 0x04000372 RID: 882
		private bool _isAttacker;

		// Token: 0x04000373 RID: 883
		private int _roundsWon;

		// Token: 0x04000374 RID: 884
		private string _name;

		// Token: 0x04000375 RID: 885
		private string _cultureId;

		// Token: 0x04000376 RID: 886
		private string _teamColor;

		// Token: 0x04000377 RID: 887
		private string _playersText;
	}
}
