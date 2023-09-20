using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame
{
	public class MPCustomGameItemVM : ViewModel
	{
		public GameServerEntry GameServerInfo { get; private set; }

		public PremadeGameEntry PremadeGameInfo { get; private set; }

		public MPCustomGameItemVM(GameServerEntry gameServerInfo, Action onJoin, Action<GameServerEntry> onRequestActions)
		{
			this._onJoin = onJoin;
			this._onRequestActions = onRequestActions;
			this.GameServerInfo = gameServerInfo;
			string text = new TextObject("{=vBkrw5VV}Random", null).ToString();
			this._randomString = "-- " + text + " --";
			this.LoadedModulesHint = new BasicTooltipViewModel(() => this.GetLoadedModulesTooltipProperties());
			this.UpdateGameServerInfo();
		}

		public MPCustomGameItemVM(PremadeGameEntry premadeGameInfo, Action onJoin)
		{
			this._onJoin = onJoin;
			this.PremadeGameInfo = premadeGameInfo;
			this.IsClanMatchItem = true;
			this.IsPingInfoAvailable = false;
			string text = new TextObject("{=vBkrw5VV}Random", null).ToString();
			this._randomString = "-- " + text + " --";
			this.UpdatePremadeGameInfo();
		}

		private async void UpdateGameServerInfo()
		{
			this.IsPasswordProtected = this.GameServerInfo.PasswordProtected;
			this.PlayerCount = this.GameServerInfo.PlayerCount;
			this.MaxPlayerCount = this.GameServerInfo.MaxPlayerCount;
			this.NameText = this.GameServerInfo.ServerName;
			TextObject textObject = GameTexts.FindText("str_multiplayer_official_game_type_name", this.GameServerInfo.GameType);
			this.GameTypeText = (textObject.ToString().StartsWith("ERROR: ") ? new TextObject("{=MT4b8H9h}Unknown", null).ToString() : textObject.ToString());
			GameTexts.SetVariable("LEFT", this.PlayerCount);
			GameTexts.SetVariable("RIGHT", this.MaxPlayerCount);
			this.PlayerCountText = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
			this.IsOfficialServer = this.GameServerInfo.IsOfficial;
			this.IsByOfficialServerProvider = this.GameServerInfo.ByOfficialProvider;
			this.IsCommunityServer = !this.IsOfficialServer && !this.IsByOfficialServerProvider;
			this.HostText = this.GameServerInfo.HostName;
			this.IsPingInfoAvailable = MPCustomGameVM.IsPingInfoAvailable;
			await this.UpdatePingText();
		}

		private async Task UpdatePingText()
		{
			if (this.IsPingInfoAvailable)
			{
				long num = await NetworkMain.GameClient.GetPingToServer(this.GameServerInfo.Address);
				this.PingText = ((num < 0L) ? "-" : num.ToString());
			}
			else
			{
				this.PingText = "-";
			}
		}

		private void UpdatePremadeGameInfo()
		{
			this.IsPasswordProtected = this.PremadeGameInfo.IsPasswordProtected;
			this.NameText = this.PremadeGameInfo.Name;
			this.GameTypeText = (this.GameTypeText = GameTexts.FindText("str_multiplayer_official_game_type_name", this.PremadeGameInfo.GameType).ToString());
			this.RegionName = this.PremadeGameInfo.Region;
			this.FirstFactionName = ((this.PremadeGameInfo.FactionA == Parameters.RandomSelectionString) ? this._randomString : this.PremadeGameInfo.FactionA);
			this.SecondFactionName = ((this.PremadeGameInfo.FactionB == Parameters.RandomSelectionString) ? this._randomString : this.PremadeGameInfo.FactionB);
			this.HostText = MPCustomGameItemVM.OfficialServerHostName;
			this.IsOfficialServer = true;
			if (this.PremadeGameInfo.PremadeGameType == PremadeGameType.Clan)
			{
				this.PremadeMatchTypeText = new TextObject("{=YNkPy4ta}Clan Match", null).ToString();
				return;
			}
			if (this.PremadeGameInfo.PremadeGameType == PremadeGameType.Practice)
			{
				this.PremadeMatchTypeText = new TextObject("{=H5tiRTya}Practice", null).ToString();
			}
		}

		private List<TooltipProperty> GetLoadedModulesTooltipProperties()
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (this.GameServerInfo != null)
			{
				if (this.GameServerInfo.LoadedModules.Count > 0)
				{
					list.Add(new TooltipProperty(string.Empty, new TextObject("{=JXyxj1J5}Modules", null).ToString(), 1, false, TooltipProperty.TooltipPropertyFlags.Title));
					string text = " " + new TextObject("{=oYS9sabI}(optional)", null).ToString();
					foreach (ModuleInfoModel moduleInfoModel in this.GameServerInfo.LoadedModules)
					{
						string text2 = moduleInfoModel.Version;
						if (moduleInfoModel.IsOptional)
						{
							text2 += text;
						}
						list.Add(new TooltipProperty(moduleInfoModel.Name, text2, 0, false, TooltipProperty.TooltipPropertyFlags.None));
					}
				}
				TextObject textObject = (this.GameServerInfo.AllowsOptionalModules ? new TextObject("{=BBmEESTT}This server allows optional modules.", null) : new TextObject("{=sEbeLmZP}This server does not allow optional modules.", null));
				list.Add(new TooltipProperty("", textObject.ToString(), -1, false, TooltipProperty.TooltipPropertyFlags.None));
				if (this.IsCommunityServer)
				{
					list.Add(new TooltipProperty("", string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.DefaultSeperator));
					TextObject textObject2 = new TextObject("{=W51HSyXy}Press {VIEW_OPTIONS_KEY} to view options", null);
					string text3 = HotKeyManager.GetCategory("MultiplayerHotkeyCategory").GetHotKey("PreviewCosmeticItem").ToString();
					textObject2.SetTextVariable("VIEW_OPTIONS_KEY", GameTexts.FindText("str_game_key_text", text3.ToLower()));
					list.Add(new TooltipProperty(string.Empty, textObject2.ToString(), -1, false, TooltipProperty.TooltipPropertyFlags.None)
					{
						OnlyShowWhenNotExtended = true
					});
				}
			}
			return list;
		}

		private void ExecuteJoin()
		{
			this._onJoin();
		}

		private void ExecuteViewHostOptions()
		{
			if (this._onRequestActions != null)
			{
				this._onRequestActions(this.GameServerInfo);
				this.LoadedModulesHint.ExecuteEndHint();
			}
		}

		[DataSourceProperty]
		public bool IsPasswordProtected
		{
			get
			{
				return this._isPasswordProtected;
			}
			set
			{
				if (value != this._isPasswordProtected)
				{
					this._isPasswordProtected = value;
					base.OnPropertyChanged("IsPasswordProtected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsClanMatchItem
		{
			get
			{
				return this._isClanMatchItem;
			}
			set
			{
				if (value != this._isClanMatchItem)
				{
					this._isClanMatchItem = value;
					base.OnPropertyChanged("IsClanMatchItem");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOfficialServer
		{
			get
			{
				return this._isOfficialServer;
			}
			set
			{
				if (value != this._isOfficialServer)
				{
					this._isOfficialServer = value;
					base.OnPropertyChanged("IsOfficialServer");
				}
			}
		}

		[DataSourceProperty]
		public bool IsByOfficialServerProvider
		{
			get
			{
				return this._isByOfficialServerProvider;
			}
			set
			{
				if (value != this._isByOfficialServerProvider)
				{
					this._isByOfficialServerProvider = value;
					base.OnPropertyChanged("IsByOfficialServerProvider");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCommunityServer
		{
			get
			{
				return this._isCommunityServer;
			}
			set
			{
				if (value != this._isCommunityServer)
				{
					this._isCommunityServer = value;
					base.OnPropertyChanged("IsCommunityServer");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPingInfoAvailable
		{
			get
			{
				return this._isPingInfoAvailable;
			}
			set
			{
				if (value != this._isPingInfoAvailable)
				{
					this._isPingInfoAvailable = value;
					base.OnPropertyChanged("IsPingInfoAvailable");
				}
			}
		}

		[DataSourceProperty]
		public int PlayerCount
		{
			get
			{
				return this._playerCount;
			}
			set
			{
				if (value != this._playerCount)
				{
					this._playerCount = value;
					base.OnPropertyChanged("PlayerCount");
				}
			}
		}

		[DataSourceProperty]
		public int MaxPlayerCount
		{
			get
			{
				return this._maxPlayerCount;
			}
			set
			{
				if (value != this._maxPlayerCount)
				{
					this._maxPlayerCount = value;
					base.OnPropertyChanged("MaxPlayerCount");
				}
			}
		}

		[DataSourceProperty]
		public string HostText
		{
			get
			{
				return this._hostText;
			}
			set
			{
				if (value != this._hostText)
				{
					this._hostText = value;
					base.OnPropertyChanged("HostText");
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
					base.OnPropertyChanged("NameText");
				}
			}
		}

		[DataSourceProperty]
		public string GameTypeText
		{
			get
			{
				return this._gameTypeText;
			}
			set
			{
				if (value != this._gameTypeText)
				{
					this._gameTypeText = value;
					base.OnPropertyChanged("GameTypeText");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerCountText
		{
			get
			{
				return this._playerCountText;
			}
			set
			{
				if (value != this._playerCountText)
				{
					this._playerCountText = value;
					base.OnPropertyChanged("PlayerCountText");
				}
			}
		}

		[DataSourceProperty]
		public string PingText
		{
			get
			{
				return this._pingText;
			}
			set
			{
				if (value != this._pingText)
				{
					this._pingText = value;
					base.OnPropertyChanged("PingText");
				}
			}
		}

		[DataSourceProperty]
		public string FirstFactionName
		{
			get
			{
				return this._firstFactionName;
			}
			set
			{
				if (value != this._firstFactionName)
				{
					this._firstFactionName = value;
					base.OnPropertyChanged("FirstFactionName");
				}
			}
		}

		[DataSourceProperty]
		public string SecondFactionName
		{
			get
			{
				return this._secondFactionName;
			}
			set
			{
				if (value != this._secondFactionName)
				{
					this._secondFactionName = value;
					base.OnPropertyChanged("SecondFactionName");
				}
			}
		}

		[DataSourceProperty]
		public string RegionName
		{
			get
			{
				return this._regionName;
			}
			set
			{
				if (value != this._regionName)
				{
					this._regionName = value;
					base.OnPropertyChanged("RegionName");
				}
			}
		}

		[DataSourceProperty]
		public string PremadeMatchTypeText
		{
			get
			{
				return this._premadeMatchTypeText;
			}
			set
			{
				if (value != this._premadeMatchTypeText)
				{
					this._premadeMatchTypeText = value;
					base.OnPropertyChanged("PremadeMatchTypeText");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel LoadedModulesHint
		{
			get
			{
				return this._loadedModulesHint;
			}
			set
			{
				if (value != this._loadedModulesHint)
				{
					this._loadedModulesHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "LoadedModulesHint");
				}
			}
		}

		public const string PingTimeoutText = "-";

		public static Action<PlayerId> OnPlayerProfileRequested;

		private Action _onJoin;

		private Action<GameServerEntry> _onRequestActions;

		private string _randomString;

		public static readonly string OfficialServerHostName = "TaleWorlds";

		private bool _isPasswordProtected;

		private bool _isClanMatchItem;

		private bool _isOfficialServer;

		private bool _isByOfficialServerProvider;

		private bool _isCommunityServer;

		private bool _isPingInfoAvailable;

		private int _playerCount;

		private int _maxPlayerCount;

		private string _hostText;

		private string _nameText;

		private string _gameTypeText;

		private string _playerCountText;

		private string _pingText;

		private string _firstFactionName;

		private string _secondFactionName;

		private string _regionName;

		private string _premadeMatchTypeText;

		private BasicTooltipViewModel _loadedModulesHint;
	}
}
