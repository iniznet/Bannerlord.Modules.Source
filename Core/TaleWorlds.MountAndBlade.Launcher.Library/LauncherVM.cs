using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Launcher.Library.UserDatas;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherVM : ViewModel
	{
		public string GameTypeArgument
		{
			get
			{
				if (!this.IsMultiplayer)
				{
					return "/singleplayer";
				}
				return "/multiplayer";
			}
		}

		public string ContinueGameArgument
		{
			get
			{
				if (!this._isContinueSelected)
				{
					return "";
				}
				return " /continuegame";
			}
		}

		public LauncherVM(UserDataManager userDataManager, Action onClose, Action onMinimize)
		{
			this._userDataManager = userDataManager;
			this._newsManager = new NewsManager();
			this._newsManager.SetNewsSourceURL(this.GetApplicableNewsSourceURL());
			this._onClose = onClose;
			this._onMinimize = onMinimize;
			this.PlayText = "P L A Y";
			this.ContinueText = "C O N T I N U E";
			this.LaunchText = "L A U N C H";
			this.SingleplayerText = "Singleplayer";
			this.MultiplayerText = "Multiplayer";
			this.DigitalCompanionText = "Digital Companion";
			this.NewsText = "News";
			this.DlcText = "DLC";
			this.ModsText = "Mods";
			this.VersionText = ApplicationVersion.FromParametersFile(null).ToString();
			this.IsSingleplayerAvailable = this.GameModExists("Sandbox");
			this.IsDigitalCompanionAvailable = Program.IsDigitalCompanionAvailable();
			bool flag = !this.IsSingleplayerAvailable || this._userDataManager.UserData.GameType == GameType.Multiplayer;
			this.ConfirmStart = new LauncherConfirmStartVM(new Action(this.ExecuteConfirmUnverifiedDLLStart));
			this.News = new LauncherNewsVM(this._newsManager, flag);
			this.ModsData = new LauncherModsVM(userDataManager);
			this.Hint = new LauncherInformationVM();
			this.IsSingleplayer = !flag;
			this.IsMultiplayer = flag;
			this.IsDigitalCompanion = false;
			this.Refresh();
			this._isInitialized = true;
		}

		private void UpdateAndSaveUserModsData(bool isMultiplayer)
		{
			UserData userData = this._userDataManager.UserData;
			UserGameTypeData userGameTypeData = (isMultiplayer ? userData.MultiplayerData : userData.SingleplayerData);
			userGameTypeData.ModDatas.Clear();
			foreach (LauncherModuleVM launcherModuleVM in this.ModsData.Modules)
			{
				userGameTypeData.ModDatas.Add(new UserModData(launcherModuleVM.Info.Id, launcherModuleVM.Info.Version.ToString(), launcherModuleVM.IsSelected));
			}
			this._userDataManager.SaveUserData();
		}

		private bool GameModExists(string modId)
		{
			List<ModuleInfo> list = ModuleHelper.GetModules().ToList<ModuleInfo>();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Id == modId)
				{
					return true;
				}
			}
			return false;
		}

		private void OnBeforeGameTypeChange(bool preSelectionIsMultiplayer, bool newSelectionIsMultiplayer)
		{
			if (!this._isInitialized)
			{
				return;
			}
			this._userDataManager.UserData.GameType = (newSelectionIsMultiplayer ? GameType.Multiplayer : GameType.Singleplayer);
			this.UpdateAndSaveUserModsData(preSelectionIsMultiplayer);
		}

		private void OnAfterGameTypeChange(bool isMultiplayer, bool isSingleplayer, bool isDigitalCompanion)
		{
			this.IsMultiplayer = isMultiplayer;
			this.IsSingleplayer = isSingleplayer;
			this.IsDigitalCompanion = isDigitalCompanion;
			this.Refresh();
		}

		private void ExecuteStartGame(int mode)
		{
			this._isContinueSelected = mode == 1;
			this.UpdateAndSaveUserModsData(this.IsMultiplayer);
			List<SubModuleInfo> list = new List<SubModuleInfo>();
			List<DependentVersionMissmatchItem> list2 = new List<DependentVersionMissmatchItem>();
			if (this.IsSingleplayer)
			{
				foreach (LauncherModuleVM launcherModuleVM in this.ModsData.Modules)
				{
					if (launcherModuleVM.IsSelected)
					{
						foreach (LauncherSubModule launcherSubModule in launcherModuleVM.SubModules)
						{
							if (!string.IsNullOrEmpty(launcherSubModule.Info.DLLName) && launcherSubModule.Info.DLLExists && !launcherSubModule.Info.IsTWCertifiedDLL)
							{
								list.Add(launcherSubModule.Info);
							}
						}
						List<Tuple<DependedModule, ApplicationVersion>> list3 = new List<Tuple<DependedModule, ApplicationVersion>>();
						foreach (DependedModule dependedModule in launcherModuleVM.Info.DependedModules)
						{
							ApplicationVersion applicationVersionOfModule = this.GetApplicationVersionOfModule(dependedModule.ModuleId);
							if (!dependedModule.Version.IsSame(applicationVersionOfModule))
							{
								list3.Add(new Tuple<DependedModule, ApplicationVersion>(dependedModule, applicationVersionOfModule));
							}
						}
						if (list3.Count > 0)
						{
							list2.Add(new DependentVersionMissmatchItem(launcherModuleVM.Name, list3));
						}
					}
				}
			}
			if (this.IsDigitalCompanion)
			{
				Program.StartDigitalCompanion();
				return;
			}
			if (list.Count > 0 || list2.Count > 0)
			{
				this.ConfirmStart.EnableWith(list, list2);
				return;
			}
			Program.StartGame();
		}

		private ApplicationVersion GetApplicationVersionOfModule(string id)
		{
			return this.ModsData.Modules.FirstOrDefault((LauncherModuleVM m) => m.Info.Id == id).Info.Version;
		}

		private void ExecuteConfirmUnverifiedDLLStart()
		{
			Program.StartGame();
		}

		private void ExecuteClose()
		{
			this.UpdateAndSaveUserModsData(this.IsMultiplayer);
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose();
		}

		private void ExecuteMinimize()
		{
			Action onMinimize = this._onMinimize;
			if (onMinimize == null)
			{
				return;
			}
			onMinimize();
		}

		private void Refresh()
		{
			this.News.Refresh(this.IsMultiplayer);
			this.ModsData.Refresh(this.IsDigitalCompanion, this.IsMultiplayer);
			this.VersionText = ApplicationVersion.FromParametersFile(null).ToString();
		}

		private string GetApplicableNewsSourceURL()
		{
			int geoID = Kernel32.GetUserGeoID(Kernel32.GeoTypeId.Nation);
			RegionInfo regionInfo = (from x in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
				select new RegionInfo(x.ToString())).FirstOrDefault((RegionInfo r) => r.GeoId == geoID);
			bool flag = string.Equals((regionInfo != null) ? regionInfo.TwoLetterISORegionName : null, "cn", StringComparison.OrdinalIgnoreCase);
			bool isInPreviewMode = this._newsManager.IsInPreviewMode;
			string text = (flag ? "zh" : "en");
			this._newsManager.UpdateLocalizationID(text);
			if (!isInPreviewMode)
			{
				return "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_" + text + ".json";
			}
			return "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_" + text + "_preview.json";
		}

		[DataSourceProperty]
		public bool IsSingleplayer
		{
			get
			{
				return this._isSingleplayer;
			}
			set
			{
				if (this._isSingleplayer != value)
				{
					this.OnBeforeGameTypeChange(this._isMultiplayer, !value);
					this._isSingleplayer = value;
					base.OnPropertyChangedWithValue(value, "IsSingleplayer");
					if (value)
					{
						this.OnAfterGameTypeChange(false, true, false);
					}
				}
			}
		}

		[DataSourceProperty]
		public bool IsMultiplayer
		{
			get
			{
				return this._isMultiplayer;
			}
			set
			{
				if (this._isMultiplayer != value)
				{
					this.OnBeforeGameTypeChange(this._isMultiplayer, value);
					this._isMultiplayer = value;
					base.OnPropertyChangedWithValue(value, "IsMultiplayer");
					if (value)
					{
						this.OnAfterGameTypeChange(true, false, false);
					}
				}
			}
		}

		[DataSourceProperty]
		public bool IsDigitalCompanion
		{
			get
			{
				return this._isDigitalCompanion;
			}
			set
			{
				if (this._isDigitalCompanion != value)
				{
					this._isDigitalCompanion = value;
					base.OnPropertyChangedWithValue(value, "IsDigitalCompanion");
					if (value)
					{
						this.OnAfterGameTypeChange(false, false, true);
					}
				}
			}
		}

		[DataSourceProperty]
		public bool IsSingleplayerAvailable
		{
			get
			{
				return this._isSingleplayerAvailable;
			}
			set
			{
				if (value != this._isSingleplayerAvailable)
				{
					this._isSingleplayerAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsSingleplayerAvailable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDigitalCompanionAvailable
		{
			get
			{
				return this._isDigitalCompanionAvailable;
			}
			set
			{
				if (value != this._isDigitalCompanionAvailable)
				{
					this._isDigitalCompanionAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsDigitalCompanionAvailable");
				}
			}
		}

		[DataSourceProperty]
		public string VersionText
		{
			get
			{
				return this._versionText;
			}
			set
			{
				if (value != this._versionText)
				{
					this._versionText = value;
					base.OnPropertyChangedWithValue<string>(value, "VersionText");
				}
			}
		}

		[DataSourceProperty]
		public LauncherNewsVM News
		{
			get
			{
				return this._news;
			}
			set
			{
				if (value != this._news)
				{
					this._news = value;
					base.OnPropertyChangedWithValue<LauncherNewsVM>(value, "News");
				}
			}
		}

		[DataSourceProperty]
		public LauncherConfirmStartVM ConfirmStart
		{
			get
			{
				return this._confirmStart;
			}
			set
			{
				if (value != this._confirmStart)
				{
					this._confirmStart = value;
					base.OnPropertyChangedWithValue<LauncherConfirmStartVM>(value, "ConfirmStart");
				}
			}
		}

		[DataSourceProperty]
		public LauncherModsVM ModsData
		{
			get
			{
				return this._modsData;
			}
			set
			{
				if (value != this._modsData)
				{
					this._modsData = value;
					base.OnPropertyChangedWithValue<LauncherModsVM>(value, "ModsData");
				}
			}
		}

		[DataSourceProperty]
		public LauncherInformationVM Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (this._hint != value)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<LauncherInformationVM>(value, "Hint");
				}
			}
		}

		[DataSourceProperty]
		public string PlayText
		{
			get
			{
				return this._playText;
			}
			set
			{
				if (this._playText != value)
				{
					this._playText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayText");
				}
			}
		}

		[DataSourceProperty]
		public string ContinueText
		{
			get
			{
				return this._continueText;
			}
			set
			{
				if (this._continueText != value)
				{
					this._continueText = value;
					base.OnPropertyChangedWithValue<string>(value, "ContinueText");
				}
			}
		}

		[DataSourceProperty]
		public string LaunchText
		{
			get
			{
				return this._launchText;
			}
			set
			{
				if (this._launchText != value)
				{
					this._launchText = value;
					base.OnPropertyChangedWithValue<string>(value, "LaunchText");
				}
			}
		}

		[DataSourceProperty]
		public string SingleplayerText
		{
			get
			{
				return this._singleplayerText;
			}
			set
			{
				if (this._singleplayerText != value)
				{
					this._singleplayerText = value;
					base.OnPropertyChangedWithValue<string>(value, "SingleplayerText");
				}
			}
		}

		[DataSourceProperty]
		public string DigitalCompanionText
		{
			get
			{
				return this._digitalCompanionText;
			}
			set
			{
				if (this._digitalCompanionText != value)
				{
					this._digitalCompanionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DigitalCompanionText");
				}
			}
		}

		[DataSourceProperty]
		public string MultiplayerText
		{
			get
			{
				return this._multiplayerText;
			}
			set
			{
				if (this._multiplayerText != value)
				{
					this._multiplayerText = value;
					base.OnPropertyChangedWithValue<string>(value, "MultiplayerText");
				}
			}
		}

		[DataSourceProperty]
		public string NewsText
		{
			get
			{
				return this._newsText;
			}
			set
			{
				if (this._newsText != value)
				{
					this._newsText = value;
					base.OnPropertyChangedWithValue<string>(value, "NewsText");
				}
			}
		}

		[DataSourceProperty]
		public string DlcText
		{
			get
			{
				return this._dlcText;
			}
			set
			{
				if (this._dlcText != value)
				{
					this._dlcText = value;
					base.OnPropertyChangedWithValue<string>(value, "DlcText");
				}
			}
		}

		[DataSourceProperty]
		public string ModsText
		{
			get
			{
				return this._modsText;
			}
			set
			{
				if (this._modsText != value)
				{
					this._modsText = value;
					base.OnPropertyChangedWithValue<string>(value, "ModsText");
				}
			}
		}

		private UserDataManager _userDataManager;

		private NewsManager _newsManager;

		private readonly Action _onClose;

		private readonly Action _onMinimize;

		private bool _isInitialized;

		private bool _isContinueSelected;

		private const string _newsSourceURLBase = "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_";

		private bool _isMultiplayer;

		private bool _isSingleplayer;

		private bool _isDigitalCompanion;

		private bool _isSingleplayerAvailable;

		private bool _isDigitalCompanionAvailable;

		private LauncherNewsVM _news;

		private LauncherModsVM _modsData;

		private LauncherConfirmStartVM _confirmStart;

		private LauncherInformationVM _hint;

		private string _playText;

		private string _continueText;

		private string _launchText;

		private string _singleplayerText;

		private string _multiplayerText;

		private string _digitalCompanionText;

		private string _newsText;

		private string _dlcText;

		private string _modsText;

		private string _versionText;
	}
}
