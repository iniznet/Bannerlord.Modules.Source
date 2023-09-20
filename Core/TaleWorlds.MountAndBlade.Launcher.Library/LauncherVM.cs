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
	// Token: 0x02000017 RID: 23
	public class LauncherVM : ViewModel
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00004AFB File Offset: 0x00002CFB
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

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00004B10 File Offset: 0x00002D10
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

		// Token: 0x060000CD RID: 205 RVA: 0x00004B28 File Offset: 0x00002D28
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

		// Token: 0x060000CE RID: 206 RVA: 0x00004C8C File Offset: 0x00002E8C
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

		// Token: 0x060000CF RID: 207 RVA: 0x00004D48 File Offset: 0x00002F48
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

		// Token: 0x060000D0 RID: 208 RVA: 0x00004D88 File Offset: 0x00002F88
		private void OnBeforeGameTypeChange(bool preSelectionIsMultiplayer, bool newSelectionIsMultiplayer)
		{
			if (!this._isInitialized)
			{
				return;
			}
			this._userDataManager.UserData.GameType = (newSelectionIsMultiplayer ? GameType.Multiplayer : GameType.Singleplayer);
			this.UpdateAndSaveUserModsData(preSelectionIsMultiplayer);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00004DB1 File Offset: 0x00002FB1
		private void OnAfterGameTypeChange(bool isMultiplayer, bool isSingleplayer, bool isDigitalCompanion)
		{
			this.IsMultiplayer = isMultiplayer;
			this.IsSingleplayer = isSingleplayer;
			this.IsDigitalCompanion = isDigitalCompanion;
			this.Refresh();
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00004DD0 File Offset: 0x00002FD0
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

		// Token: 0x060000D3 RID: 211 RVA: 0x00004FC0 File Offset: 0x000031C0
		private ApplicationVersion GetApplicationVersionOfModule(string id)
		{
			return this.ModsData.Modules.FirstOrDefault((LauncherModuleVM m) => m.Info.Id == id).Info.Version;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00005000 File Offset: 0x00003200
		private void ExecuteConfirmUnverifiedDLLStart()
		{
			Program.StartGame();
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00005007 File Offset: 0x00003207
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

		// Token: 0x060000D6 RID: 214 RVA: 0x00005025 File Offset: 0x00003225
		private void ExecuteMinimize()
		{
			Action onMinimize = this._onMinimize;
			if (onMinimize == null)
			{
				return;
			}
			onMinimize();
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00005038 File Offset: 0x00003238
		private void Refresh()
		{
			this.News.Refresh(this.IsMultiplayer);
			this.ModsData.Refresh(this.IsDigitalCompanion, this.IsMultiplayer);
			this.VersionText = ApplicationVersion.FromParametersFile(null).ToString();
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00005088 File Offset: 0x00003288
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

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00005145 File Offset: 0x00003345
		// (set) Token: 0x060000DA RID: 218 RVA: 0x0000514D File Offset: 0x0000334D
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
					this.OnBeforeGameTypeChange(this._isMultiplayer, value);
					this._isSingleplayer = value;
					base.OnPropertyChangedWithValue(value, "IsSingleplayer");
					if (value)
					{
						this.OnAfterGameTypeChange(false, true, false);
					}
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00005184 File Offset: 0x00003384
		// (set) Token: 0x060000DC RID: 220 RVA: 0x0000518C File Offset: 0x0000338C
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
					this.OnBeforeGameTypeChange(this._isMultiplayer, !value);
					this._isMultiplayer = value;
					base.OnPropertyChangedWithValue(value, "IsMultiplayer");
					if (value)
					{
						this.OnAfterGameTypeChange(true, false, false);
					}
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000DD RID: 221 RVA: 0x000051C6 File Offset: 0x000033C6
		// (set) Token: 0x060000DE RID: 222 RVA: 0x000051CE File Offset: 0x000033CE
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

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000DF RID: 223 RVA: 0x000051F8 File Offset: 0x000033F8
		// (set) Token: 0x060000E0 RID: 224 RVA: 0x00005200 File Offset: 0x00003400
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

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x0000521E File Offset: 0x0000341E
		// (set) Token: 0x060000E2 RID: 226 RVA: 0x00005226 File Offset: 0x00003426
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00005244 File Offset: 0x00003444
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x0000524C File Offset: 0x0000344C
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

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000526F File Offset: 0x0000346F
		// (set) Token: 0x060000E6 RID: 230 RVA: 0x00005277 File Offset: 0x00003477
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

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00005295 File Offset: 0x00003495
		// (set) Token: 0x060000E8 RID: 232 RVA: 0x0000529D File Offset: 0x0000349D
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x000052BB File Offset: 0x000034BB
		// (set) Token: 0x060000EA RID: 234 RVA: 0x000052C3 File Offset: 0x000034C3
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000EB RID: 235 RVA: 0x000052E1 File Offset: 0x000034E1
		// (set) Token: 0x060000EC RID: 236 RVA: 0x000052E9 File Offset: 0x000034E9
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00005307 File Offset: 0x00003507
		// (set) Token: 0x060000EE RID: 238 RVA: 0x0000530F File Offset: 0x0000350F
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000EF RID: 239 RVA: 0x00005332 File Offset: 0x00003532
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x0000533A File Offset: 0x0000353A
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

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x0000535D File Offset: 0x0000355D
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x00005365 File Offset: 0x00003565
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

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00005388 File Offset: 0x00003588
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x00005390 File Offset: 0x00003590
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

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x000053B3 File Offset: 0x000035B3
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x000053BB File Offset: 0x000035BB
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x000053DE File Offset: 0x000035DE
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x000053E6 File Offset: 0x000035E6
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

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00005409 File Offset: 0x00003609
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00005411 File Offset: 0x00003611
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00005434 File Offset: 0x00003634
		// (set) Token: 0x060000FC RID: 252 RVA: 0x0000543C File Offset: 0x0000363C
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000FD RID: 253 RVA: 0x0000545F File Offset: 0x0000365F
		// (set) Token: 0x060000FE RID: 254 RVA: 0x00005467 File Offset: 0x00003667
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

		// Token: 0x04000068 RID: 104
		private UserDataManager _userDataManager;

		// Token: 0x04000069 RID: 105
		private NewsManager _newsManager;

		// Token: 0x0400006A RID: 106
		private readonly Action _onClose;

		// Token: 0x0400006B RID: 107
		private readonly Action _onMinimize;

		// Token: 0x0400006C RID: 108
		private bool _isInitialized;

		// Token: 0x0400006D RID: 109
		private bool _isContinueSelected;

		// Token: 0x0400006E RID: 110
		private const string _newsSourceURLBase = "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_";

		// Token: 0x0400006F RID: 111
		private bool _isMultiplayer;

		// Token: 0x04000070 RID: 112
		private bool _isSingleplayer;

		// Token: 0x04000071 RID: 113
		private bool _isDigitalCompanion;

		// Token: 0x04000072 RID: 114
		private bool _isSingleplayerAvailable;

		// Token: 0x04000073 RID: 115
		private bool _isDigitalCompanionAvailable;

		// Token: 0x04000074 RID: 116
		private LauncherNewsVM _news;

		// Token: 0x04000075 RID: 117
		private LauncherModsVM _modsData;

		// Token: 0x04000076 RID: 118
		private LauncherConfirmStartVM _confirmStart;

		// Token: 0x04000077 RID: 119
		private LauncherInformationVM _hint;

		// Token: 0x04000078 RID: 120
		private string _playText;

		// Token: 0x04000079 RID: 121
		private string _continueText;

		// Token: 0x0400007A RID: 122
		private string _launchText;

		// Token: 0x0400007B RID: 123
		private string _singleplayerText;

		// Token: 0x0400007C RID: 124
		private string _multiplayerText;

		// Token: 0x0400007D RID: 125
		private string _digitalCompanionText;

		// Token: 0x0400007E RID: 126
		private string _newsText;

		// Token: 0x0400007F RID: 127
		private string _dlcText;

		// Token: 0x04000080 RID: 128
		private string _modsText;

		// Token: 0x04000081 RID: 129
		private string _versionText;
	}
}
