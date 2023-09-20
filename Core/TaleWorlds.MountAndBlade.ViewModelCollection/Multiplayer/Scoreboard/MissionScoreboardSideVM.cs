using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	public class MissionScoreboardSideVM : ViewModel
	{
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

		public void Tick(float dt)
		{
			foreach (MissionScoreboardPlayerVM missionScoreboardPlayerVM in this.Players)
			{
				missionScoreboardPlayerVM.Tick(dt);
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			NetworkCommunicator.OnPeerAveragePingUpdated -= this.OnPeerPingUpdated;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		public void UpdateRoundAttributes()
		{
			this.RoundsWon = this._missionScoreboardSide.SideScore;
			this.SortPlayers();
		}

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

		public void RemovePlayer(MissionPeer peer)
		{
			this.Players.Remove(this._playersMap[peer]);
			this._playersMap.Remove(peer);
			this.SortPlayers();
			this.UpdatePlayersText();
		}

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

		private void UpdatePlayersText()
		{
			TextObject textObject = new TextObject("{=R28ac5ij}{NUMBER} Players", null);
			textObject.SetTextVariable("NUMBER", this.Players.Count);
			this.PlayersText = textObject.ToString();
		}

		private void SortPlayers()
		{
			this.PlayerSortController.SortByCurrentState();
		}

		private void OnPeerPingUpdated(NetworkCommunicator peer)
		{
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (component != null)
			{
				this.UpdatePlayerAttributes(component);
			}
		}

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

		private readonly MissionScoreboardComponent.MissionScoreboardSide _missionScoreboardSide;

		private readonly Dictionary<MissionPeer, MissionScoreboardPlayerVM> _playersMap;

		private MissionScoreboardPlayerVM _bot;

		private Action<MissionScoreboardPlayerVM> _executeActivate;

		private const string _avatarHeaderId = "avatar";

		private readonly int _avatarHeaderIndex;

		private List<string> _irregularHeaderIDs = new List<string> { "name", "avatar", "score", "kill", "assist" };

		private MBBindingList<MissionScoreboardPlayerVM> _players;

		private MBBindingList<MissionScoreboardHeaderItemVM> _entryProperties;

		private MissionScoreboardPlayerSortControllerVM _playerSortController;

		private bool _isSingleSide;

		private bool _isSecondSide;

		private bool _useSecondary;

		private bool _showAttackerOrDefenderIcons;

		private bool _isAttacker;

		private int _roundsWon;

		private string _name;

		private string _cultureId;

		private string _teamColor;

		private string _playersText;
	}
}
