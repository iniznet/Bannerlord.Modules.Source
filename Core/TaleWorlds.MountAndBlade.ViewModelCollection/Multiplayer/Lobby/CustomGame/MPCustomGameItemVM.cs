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
	// Token: 0x0200008C RID: 140
	public class MPCustomGameItemVM : ViewModel
	{
		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06000CC8 RID: 3272 RVA: 0x0002CDFB File Offset: 0x0002AFFB
		// (set) Token: 0x06000CC9 RID: 3273 RVA: 0x0002CE03 File Offset: 0x0002B003
		public GameServerEntry GameServerInfo { get; private set; }

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06000CCA RID: 3274 RVA: 0x0002CE0C File Offset: 0x0002B00C
		// (set) Token: 0x06000CCB RID: 3275 RVA: 0x0002CE14 File Offset: 0x0002B014
		public PremadeGameEntry PremadeGameInfo { get; private set; }

		// Token: 0x06000CCC RID: 3276 RVA: 0x0002CE20 File Offset: 0x0002B020
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

		// Token: 0x06000CCD RID: 3277 RVA: 0x0002CE8C File Offset: 0x0002B08C
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

		// Token: 0x06000CCE RID: 3278 RVA: 0x0002CEE8 File Offset: 0x0002B0E8
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

		// Token: 0x06000CCF RID: 3279 RVA: 0x0002CF24 File Offset: 0x0002B124
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

		// Token: 0x06000CD0 RID: 3280 RVA: 0x0002CF6C File Offset: 0x0002B16C
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

		// Token: 0x06000CD1 RID: 3281 RVA: 0x0002D090 File Offset: 0x0002B290
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

		// Token: 0x06000CD2 RID: 3282 RVA: 0x0002D24C File Offset: 0x0002B44C
		private void ExecuteJoin()
		{
			this._onJoin();
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x0002D259 File Offset: 0x0002B459
		private void ExecuteViewHostOptions()
		{
			if (this._onRequestActions != null)
			{
				this._onRequestActions(this.GameServerInfo);
				this.LoadedModulesHint.ExecuteEndHint();
			}
		}

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06000CD4 RID: 3284 RVA: 0x0002D27F File Offset: 0x0002B47F
		// (set) Token: 0x06000CD5 RID: 3285 RVA: 0x0002D287 File Offset: 0x0002B487
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

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06000CD6 RID: 3286 RVA: 0x0002D2A4 File Offset: 0x0002B4A4
		// (set) Token: 0x06000CD7 RID: 3287 RVA: 0x0002D2AC File Offset: 0x0002B4AC
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

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06000CD8 RID: 3288 RVA: 0x0002D2C9 File Offset: 0x0002B4C9
		// (set) Token: 0x06000CD9 RID: 3289 RVA: 0x0002D2D1 File Offset: 0x0002B4D1
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

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06000CDA RID: 3290 RVA: 0x0002D2EE File Offset: 0x0002B4EE
		// (set) Token: 0x06000CDB RID: 3291 RVA: 0x0002D2F6 File Offset: 0x0002B4F6
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

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06000CDC RID: 3292 RVA: 0x0002D313 File Offset: 0x0002B513
		// (set) Token: 0x06000CDD RID: 3293 RVA: 0x0002D31B File Offset: 0x0002B51B
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

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06000CDE RID: 3294 RVA: 0x0002D338 File Offset: 0x0002B538
		// (set) Token: 0x06000CDF RID: 3295 RVA: 0x0002D340 File Offset: 0x0002B540
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

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06000CE0 RID: 3296 RVA: 0x0002D35D File Offset: 0x0002B55D
		// (set) Token: 0x06000CE1 RID: 3297 RVA: 0x0002D365 File Offset: 0x0002B565
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

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06000CE2 RID: 3298 RVA: 0x0002D382 File Offset: 0x0002B582
		// (set) Token: 0x06000CE3 RID: 3299 RVA: 0x0002D38A File Offset: 0x0002B58A
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

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06000CE4 RID: 3300 RVA: 0x0002D3A7 File Offset: 0x0002B5A7
		// (set) Token: 0x06000CE5 RID: 3301 RVA: 0x0002D3AF File Offset: 0x0002B5AF
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

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06000CE6 RID: 3302 RVA: 0x0002D3D1 File Offset: 0x0002B5D1
		// (set) Token: 0x06000CE7 RID: 3303 RVA: 0x0002D3D9 File Offset: 0x0002B5D9
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

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06000CE8 RID: 3304 RVA: 0x0002D3FB File Offset: 0x0002B5FB
		// (set) Token: 0x06000CE9 RID: 3305 RVA: 0x0002D403 File Offset: 0x0002B603
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

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06000CEA RID: 3306 RVA: 0x0002D425 File Offset: 0x0002B625
		// (set) Token: 0x06000CEB RID: 3307 RVA: 0x0002D42D File Offset: 0x0002B62D
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

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06000CEC RID: 3308 RVA: 0x0002D44F File Offset: 0x0002B64F
		// (set) Token: 0x06000CED RID: 3309 RVA: 0x0002D457 File Offset: 0x0002B657
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

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06000CEE RID: 3310 RVA: 0x0002D479 File Offset: 0x0002B679
		// (set) Token: 0x06000CEF RID: 3311 RVA: 0x0002D481 File Offset: 0x0002B681
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

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06000CF0 RID: 3312 RVA: 0x0002D4A3 File Offset: 0x0002B6A3
		// (set) Token: 0x06000CF1 RID: 3313 RVA: 0x0002D4AB File Offset: 0x0002B6AB
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

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06000CF2 RID: 3314 RVA: 0x0002D4CD File Offset: 0x0002B6CD
		// (set) Token: 0x06000CF3 RID: 3315 RVA: 0x0002D4D5 File Offset: 0x0002B6D5
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

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06000CF4 RID: 3316 RVA: 0x0002D4F7 File Offset: 0x0002B6F7
		// (set) Token: 0x06000CF5 RID: 3317 RVA: 0x0002D4FF File Offset: 0x0002B6FF
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

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06000CF6 RID: 3318 RVA: 0x0002D521 File Offset: 0x0002B721
		// (set) Token: 0x06000CF7 RID: 3319 RVA: 0x0002D529 File Offset: 0x0002B729
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

		// Token: 0x0400061D RID: 1565
		public const string PingTimeoutText = "-";

		// Token: 0x0400061E RID: 1566
		public static Action<PlayerId> OnPlayerProfileRequested;

		// Token: 0x0400061F RID: 1567
		private Action _onJoin;

		// Token: 0x04000620 RID: 1568
		private Action<GameServerEntry> _onRequestActions;

		// Token: 0x04000623 RID: 1571
		private string _randomString;

		// Token: 0x04000624 RID: 1572
		public static readonly string OfficialServerHostName = "TaleWorlds";

		// Token: 0x04000625 RID: 1573
		private bool _isPasswordProtected;

		// Token: 0x04000626 RID: 1574
		private bool _isClanMatchItem;

		// Token: 0x04000627 RID: 1575
		private bool _isOfficialServer;

		// Token: 0x04000628 RID: 1576
		private bool _isByOfficialServerProvider;

		// Token: 0x04000629 RID: 1577
		private bool _isCommunityServer;

		// Token: 0x0400062A RID: 1578
		private bool _isPingInfoAvailable;

		// Token: 0x0400062B RID: 1579
		private int _playerCount;

		// Token: 0x0400062C RID: 1580
		private int _maxPlayerCount;

		// Token: 0x0400062D RID: 1581
		private string _hostText;

		// Token: 0x0400062E RID: 1582
		private string _nameText;

		// Token: 0x0400062F RID: 1583
		private string _gameTypeText;

		// Token: 0x04000630 RID: 1584
		private string _playerCountText;

		// Token: 0x04000631 RID: 1585
		private string _pingText;

		// Token: 0x04000632 RID: 1586
		private string _firstFactionName;

		// Token: 0x04000633 RID: 1587
		private string _secondFactionName;

		// Token: 0x04000634 RID: 1588
		private string _regionName;

		// Token: 0x04000635 RID: 1589
		private string _premadeMatchTypeText;

		// Token: 0x04000636 RID: 1590
		private BasicTooltipViewModel _loadedModulesHint;
	}
}
