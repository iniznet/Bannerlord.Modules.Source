using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	// Token: 0x02000003 RID: 3
	public class DCSHelperVM : ViewModel
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000016 RID: 22 RVA: 0x0000233C File Offset: 0x0000053C
		public IEnumerable<DCSHelperMapItemVM> SelectedMaps
		{
			get
			{
				return this.MapList.Where((DCSHelperMapItemVM map) => map.IsSelected);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002368 File Offset: 0x00000568
		public IEnumerable<DCSHelperMapItemVM> SelectableMaps
		{
			get
			{
				return this.MapList.Where((DCSHelperMapItemVM map) => !map.IsSelected);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002394 File Offset: 0x00000594
		public DCSHelperVM(string hostAddress, string fullName = null)
		{
			this._hostAddress = hostAddress;
			this._fullName = fullName;
			this._texts = new DCSHelperVM.Texts();
			this._gauntletLayer = new GauntletLayer(20, "GauntletLayer", false);
			this.IsDownloading = false;
			this.ShowProgress = false;
			this.PanelTitleText = this._texts.DownloadPanel;
			this.DownloadButtonText = this._texts.Download;
			this.CloseButtonText = this._texts.Close;
			this.HostAddressText = this._texts.GetPanelSubtitle(this.Truncate(this._fullName, 40) ?? this._hostAddress);
			this.ToggleSelectionButtonText = this._texts.SelectAll;
			this.MapList = new MBBindingList<DCSHelperMapItemVM>();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002459 File Offset: 0x00000659
		public DCSHelperVM(GameServerEntry server)
			: this(string.Format("{0}:{1}", server.Address, server.Port), server.ServerName)
		{
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002484 File Offset: 0x00000684
		private string Truncate(string str, int maxLength = 40)
		{
			if (str == null)
			{
				return str;
			}
			string text = str.Trim();
			if (text.Length > maxLength)
			{
				return text.Substring(0, maxLength).TrimEnd(Array.Empty<char>()) + "...";
			}
			return text;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000024C4 File Offset: 0x000006C4
		private async Task RefreshMapList()
		{
			this.MapList.Clear();
			MapListResponse mapListResponse = await DedicatedCustomServerClientHelperSubModule.Instance.GetMapListFromHost(this._hostAddress);
			foreach (MapListItemResponse mapListItemResponse in mapListResponse.Maps)
			{
				UniqueSceneId uniqueSceneId = ((mapListItemResponse.UniqueToken != null && mapListItemResponse.Revision != null) ? new UniqueSceneId(mapListItemResponse.UniqueToken, mapListItemResponse.Revision) : null);
				DCSHelperMapItemVM dcshelperMapItemVM = new DCSHelperMapItemVM(mapListItemResponse.Name, delegate(DCSHelperMapItemVM map)
				{
					this.OnMapSelected(map, false);
				}, mapListItemResponse.Name == mapListResponse.CurrentlyPlaying, uniqueSceneId);
				dcshelperMapItemVM.RefreshLocalMapData();
				this.MapList.Add(dcshelperMapItemVM);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000250C File Offset: 0x0000070C
		private void OnMapSelected(DCSHelperMapItemVM mapItem, bool forceSelection = false)
		{
			if (!this.IsDownloading || forceSelection)
			{
				bool readyToDownload = this.ReadyToDownload;
				mapItem.IsSelected = !mapItem.IsSelected;
				this.ToggleSelectionButtonText = (this.SelectedMaps.Any<DCSHelperMapItemVM>() ? this._texts.UnselectAll : this._texts.SelectAll);
				if (readyToDownload != this.ReadyToDownload)
				{
					base.OnPropertyChangedWithValue(this.ReadyToDownload, "ReadyToDownload");
				}
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002580 File Offset: 0x00000780
		private void ToggleSelection()
		{
			IEnumerable<DCSHelperMapItemVM> enumerable = this.SelectedMaps;
			if (!enumerable.Any<DCSHelperMapItemVM>())
			{
				enumerable = this.SelectableMaps;
			}
			foreach (DCSHelperMapItemVM dcshelperMapItemVM in enumerable)
			{
				dcshelperMapItemVM.ExecuteToggleSelection();
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000025DC File Offset: 0x000007DC
		public async Task OpenPopup()
		{
			this._gauntletLayer.LoadMovie("DCSHelper", this);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TopScreen.AddLayer(this._gauntletLayer);
			ScreenManager.TrySetFocus(this._gauntletLayer);
			this.IsLoading = true;
			try
			{
				await this.RefreshMapList();
				this.IsLoading = false;
			}
			catch (Exception ex)
			{
				this.ShowFailedToRetrieveInquiry(ex.Message);
				this.ExecuteClosePopup();
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002624 File Offset: 0x00000824
		public async Task ExecuteDownloadMap()
		{
			Queue<DCSHelperMapItemVM> remainingMaps = new Queue<DCSHelperMapItemVM>(this.SelectedMaps);
			int totalMapCount = remainingMaps.Count;
			if (totalMapCount != 0)
			{
				List<DCSHelperMapItemVM> downloadedMaps = new List<DCSHelperMapItemVM>();
				this.IsDownloading = true;
				this.ShowProgress = false;
				this.DownloadButtonText = this._texts.Downloading;
				this.CloseButtonText = this._texts.Cancel;
				while (remainingMaps.Any<DCSHelperMapItemVM>())
				{
					DCSHelperVM.<>c__DisplayClass17_0 CS$<>8__locals1 = new DCSHelperVM.<>c__DisplayClass17_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.mapItem = remainingMaps.Dequeue();
					ModLogger.Log(string.Concat(new string[]
					{
						"Download Panel: Downloading map '",
						CS$<>8__locals1.mapItem.MapName,
						"' from host '",
						this._hostAddress,
						"'"
					}), 0, 4);
					this.ProgressCounterText = this._texts.GetProgressCounter(downloadedMaps.Count + 1, totalMapCount);
					try
					{
						bool flag = !ModHelpers.DoesSceneFolderAlreadyExist(CS$<>8__locals1.mapItem.MapName);
						if (!flag)
						{
							flag = await this.WaitForConfirmationToReplace(CS$<>8__locals1.mapItem.MapName);
						}
						if (flag)
						{
							CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
							this._cancellationTokenSource = cancellationTokenSource;
							using (cancellationTokenSource)
							{
								await Task.Run(delegate
								{
									DCSHelperVM.<>c__DisplayClass17_0.<<ExecuteDownloadMap>b__0>d <<ExecuteDownloadMap>b__0>d;
									<<ExecuteDownloadMap>b__0>d.<>4__this = CS$<>8__locals1;
									<<ExecuteDownloadMap>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
									<<ExecuteDownloadMap>b__0>d.<>1__state = -1;
									AsyncTaskMethodBuilder <>t__builder = <<ExecuteDownloadMap>b__0>d.<>t__builder;
									<>t__builder.Start<DCSHelperVM.<>c__DisplayClass17_0.<<ExecuteDownloadMap>b__0>d>(ref <<ExecuteDownloadMap>b__0>d);
									return <<ExecuteDownloadMap>b__0>d.<>t__builder.Task;
								});
							}
							CancellationTokenSource cancellationTokenSource2 = null;
							downloadedMaps.Add(CS$<>8__locals1.mapItem);
							this.OnMapSelected(CS$<>8__locals1.mapItem, true);
							CS$<>8__locals1.mapItem.RefreshLocalMapData();
						}
						if (!remainingMaps.Any<DCSHelperMapItemVM>())
						{
							this.ShowDownloadCompleteInquiry(downloadedMaps);
						}
					}
					catch (Exception ex)
					{
						remainingMaps.Clear();
						CancellationTokenSource cancellationTokenSource3 = this._cancellationTokenSource;
						if (cancellationTokenSource3 != null && !cancellationTokenSource3.IsCancellationRequested)
						{
							this.ShowDownloadFailedInquiry(ex.Message);
						}
					}
					this._cancellationTokenSource = null;
					CS$<>8__locals1 = null;
				}
				this.IsDownloading = false;
				this.DownloadButtonText = this._texts.Download;
				this.CloseButtonText = this._texts.Close;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x0000266C File Offset: 0x0000086C
		private Task<bool> WaitForConfirmationToReplace(string mapName)
		{
			DCSHelperVM.<>c__DisplayClass18_0 CS$<>8__locals1 = new DCSHelperVM.<>c__DisplayClass18_0();
			CS$<>8__locals1.taskSource = new TaskCompletionSource<bool>();
			InformationManager.ShowInquiry(new InquiryData(this._texts.DownloadPanel, this._texts.GetReplacementConfirmationMessage(mapName), true, true, this._texts.Yes, this._texts.No, CS$<>8__locals1.<WaitForConfirmationToReplace>g__getAction|0(true), CS$<>8__locals1.<WaitForConfirmationToReplace>g__getAction|0(false), "", 0f, null, null, null), false, false);
			return CS$<>8__locals1.taskSource.Task;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000026EC File Offset: 0x000008EC
		private void ShowDownloadCompleteInquiry(List<DCSHelperMapItemVM> downloadedMaps)
		{
			InformationManager.ShowInquiry(new InquiryData(this._texts.DownloadComplete, (downloadedMaps.Count == 1) ? this._texts.GetDownloadCompleteMessageSingular(downloadedMaps.Single<DCSHelperMapItemVM>().MapName) : this._texts.GetDownloadCompleteMessagePlural(downloadedMaps.Count), true, false, this._texts.Dismiss, "", null, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002764 File Offset: 0x00000964
		private void ShowDownloadFailedInquiry(string reason)
		{
			InformationManager.ShowInquiry(new InquiryData(this._texts.DownloadFailed, reason, false, true, "", this._texts.Dismiss, null, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000027AC File Offset: 0x000009AC
		private void ShowFailedToRetrieveInquiry(string reason)
		{
			InformationManager.ShowInquiry(new InquiryData(this._texts.DownloadPanel, reason, false, true, "", this._texts.Dismiss, null, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000027F2 File Offset: 0x000009F2
		public void ExecuteCloseOrCancel()
		{
			if (this.IsDownloading)
			{
				this.ExecuteCancelDownload();
				return;
			}
			this.ExecuteClosePopup();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002809 File Offset: 0x00000A09
		public void ExecuteClosePopup()
		{
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			ScreenManager.TopScreen.RemoveLayer(this._gauntletLayer);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002828 File Offset: 0x00000A28
		public void ExecuteCancelDownload()
		{
			if (this._cancellationTokenSource != null && !this._cancellationTokenSource.IsCancellationRequested)
			{
				try
				{
					this._cancellationTokenSource.Cancel();
				}
				catch (Exception ex)
				{
					ModLogger.Warn("Failed to cancel download: " + ex.Message);
				}
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002880 File Offset: 0x00000A80
		private void OnProgressUpdate(ProgressUpdate update)
		{
			this.ProgressText = string.Concat(new string[]
			{
				update.MegaBytesRead.ToString("0.##"),
				" MB / ",
				update.TotalMegaBytes.ToString("0.##"),
				" MB (",
				(update.ProgressRatio * 100f).ToString("0.##"),
				"%)"
			});
			this.DownloadRatio = update.ProgressRatio;
			this.ShowProgress = true;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002911 File Offset: 0x00000B11
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002919 File Offset: 0x00000B19
		[DataSourceProperty]
		public bool IsLoading
		{
			get
			{
				return this._isLoading;
			}
			set
			{
				if (value != this._isLoading)
				{
					this._isLoading = value;
					base.OnPropertyChangedWithValue(value, "IsLoading");
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002937 File Offset: 0x00000B37
		// (set) Token: 0x0600002B RID: 43 RVA: 0x0000293F File Offset: 0x00000B3F
		[DataSourceProperty]
		public bool IsDownloading
		{
			get
			{
				return this._isDownloading;
			}
			set
			{
				if (value != this._isDownloading)
				{
					this._isDownloading = value;
					base.OnPropertyChangedWithValue(value, "IsDownloading");
					base.OnPropertyChangedWithValue(this.ReadyToDownload, "ReadyToDownload");
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000296E File Offset: 0x00000B6E
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002976 File Offset: 0x00000B76
		[DataSourceProperty]
		public bool ShowProgress
		{
			get
			{
				return this._showProgress;
			}
			set
			{
				if (value != this._showProgress)
				{
					this._showProgress = value;
					base.OnPropertyChangedWithValue(value, "ShowProgress");
				}
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002994 File Offset: 0x00000B94
		[DataSourceProperty]
		public bool ReadyToDownload
		{
			get
			{
				return !this._isDownloading && this.SelectedMaps.Count<DCSHelperMapItemVM>() != 0;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000029AE File Offset: 0x00000BAE
		// (set) Token: 0x06000030 RID: 48 RVA: 0x000029B6 File Offset: 0x00000BB6
		[DataSourceProperty]
		public string PanelTitleText
		{
			get
			{
				return this._panelTitleText;
			}
			set
			{
				if (value != this._panelTitleText)
				{
					this._panelTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "PanelTitleText");
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000031 RID: 49 RVA: 0x000029D9 File Offset: 0x00000BD9
		// (set) Token: 0x06000032 RID: 50 RVA: 0x000029E1 File Offset: 0x00000BE1
		[DataSourceProperty]
		public string DownloadButtonText
		{
			get
			{
				return this._downloadButtonText;
			}
			set
			{
				if (value != this._downloadButtonText)
				{
					this._downloadButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "DownloadButtonText");
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002A04 File Offset: 0x00000C04
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002A0C File Offset: 0x00000C0C
		[DataSourceProperty]
		public string CloseButtonText
		{
			get
			{
				return this._closeButtonText;
			}
			set
			{
				if (value != this._closeButtonText)
				{
					this._closeButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "CloseButtonText");
				}
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002A2F File Offset: 0x00000C2F
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002A37 File Offset: 0x00000C37
		[DataSourceProperty]
		public string ToggleSelectionButtonText
		{
			get
			{
				return this._toggleSelectionButtonText;
			}
			set
			{
				if (value != this._toggleSelectionButtonText)
				{
					this._toggleSelectionButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "ToggleSelectionButtonText");
				}
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002A5A File Offset: 0x00000C5A
		// (set) Token: 0x06000038 RID: 56 RVA: 0x00002A62 File Offset: 0x00000C62
		[DataSourceProperty]
		public string HostAddressText
		{
			get
			{
				return this._hostAddressText;
			}
			set
			{
				if (value != this._hostAddressText)
				{
					this._hostAddressText = value;
					base.OnPropertyChangedWithValue<string>(value, "HostAddressText");
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002A85 File Offset: 0x00000C85
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002A8D File Offset: 0x00000C8D
		[DataSourceProperty]
		public string ProgressCounterText
		{
			get
			{
				return this._progressCounterText;
			}
			set
			{
				if (value != this._progressCounterText)
				{
					this._progressCounterText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProgressCounterText");
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002AB0 File Offset: 0x00000CB0
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002AB8 File Offset: 0x00000CB8
		[DataSourceProperty]
		public string ProgressText
		{
			get
			{
				return this._progressText;
			}
			set
			{
				if (value != this._progressText)
				{
					this._progressText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProgressText");
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002ADB File Offset: 0x00000CDB
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002AE3 File Offset: 0x00000CE3
		[DataSourceProperty]
		public float DownloadRatio
		{
			get
			{
				return this._downloadRatio;
			}
			set
			{
				if (value != this._downloadRatio)
				{
					this._downloadRatio = value;
					base.OnPropertyChangedWithValue(value, "DownloadRatio");
				}
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002B01 File Offset: 0x00000D01
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002B09 File Offset: 0x00000D09
		[DataSourceProperty]
		public MBBindingList<DCSHelperMapItemVM> MapList
		{
			get
			{
				return this._mapList;
			}
			set
			{
				if (value != this._mapList)
				{
					this._mapList = value;
					base.OnPropertyChangedWithValue<MBBindingList<DCSHelperMapItemVM>>(value, "MapList");
				}
			}
		}

		// Token: 0x0400000B RID: 11
		private readonly string _hostAddress;

		// Token: 0x0400000C RID: 12
		private readonly string _fullName;

		// Token: 0x0400000D RID: 13
		private readonly DCSHelperVM.Texts _texts;

		// Token: 0x0400000E RID: 14
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400000F RID: 15
		private CancellationTokenSource _cancellationTokenSource;

		// Token: 0x04000010 RID: 16
		private bool _isLoading;

		// Token: 0x04000011 RID: 17
		private bool _isDownloading;

		// Token: 0x04000012 RID: 18
		private bool _showProgress;

		// Token: 0x04000013 RID: 19
		private string _panelTitleText;

		// Token: 0x04000014 RID: 20
		private string _downloadButtonText;

		// Token: 0x04000015 RID: 21
		private string _closeButtonText;

		// Token: 0x04000016 RID: 22
		private string _toggleSelectionButtonText;

		// Token: 0x04000017 RID: 23
		private string _hostAddressText;

		// Token: 0x04000018 RID: 24
		private string _progressCounterText;

		// Token: 0x04000019 RID: 25
		private string _progressText;

		// Token: 0x0400001A RID: 26
		private float _downloadRatio;

		// Token: 0x0400001B RID: 27
		private MBBindingList<DCSHelperMapItemVM> _mapList;

		// Token: 0x0200000A RID: 10
		private class Texts
		{
			// Token: 0x17000024 RID: 36
			// (get) Token: 0x0600006A RID: 106 RVA: 0x000030B4 File Offset: 0x000012B4
			// (set) Token: 0x0600006B RID: 107 RVA: 0x000030BC File Offset: 0x000012BC
			public string Download { get; private set; }

			// Token: 0x17000025 RID: 37
			// (get) Token: 0x0600006C RID: 108 RVA: 0x000030C5 File Offset: 0x000012C5
			// (set) Token: 0x0600006D RID: 109 RVA: 0x000030CD File Offset: 0x000012CD
			public string Downloading { get; private set; }

			// Token: 0x17000026 RID: 38
			// (get) Token: 0x0600006E RID: 110 RVA: 0x000030D6 File Offset: 0x000012D6
			// (set) Token: 0x0600006F RID: 111 RVA: 0x000030DE File Offset: 0x000012DE
			public string Cancel { get; private set; }

			// Token: 0x17000027 RID: 39
			// (get) Token: 0x06000070 RID: 112 RVA: 0x000030E7 File Offset: 0x000012E7
			// (set) Token: 0x06000071 RID: 113 RVA: 0x000030EF File Offset: 0x000012EF
			public string Close { get; private set; }

			// Token: 0x17000028 RID: 40
			// (get) Token: 0x06000072 RID: 114 RVA: 0x000030F8 File Offset: 0x000012F8
			// (set) Token: 0x06000073 RID: 115 RVA: 0x00003100 File Offset: 0x00001300
			public string Dismiss { get; private set; }

			// Token: 0x17000029 RID: 41
			// (get) Token: 0x06000074 RID: 116 RVA: 0x00003109 File Offset: 0x00001309
			// (set) Token: 0x06000075 RID: 117 RVA: 0x00003111 File Offset: 0x00001311
			public string SelectAll { get; private set; }

			// Token: 0x1700002A RID: 42
			// (get) Token: 0x06000076 RID: 118 RVA: 0x0000311A File Offset: 0x0000131A
			// (set) Token: 0x06000077 RID: 119 RVA: 0x00003122 File Offset: 0x00001322
			public string UnselectAll { get; private set; }

			// Token: 0x1700002B RID: 43
			// (get) Token: 0x06000078 RID: 120 RVA: 0x0000312B File Offset: 0x0000132B
			// (set) Token: 0x06000079 RID: 121 RVA: 0x00003133 File Offset: 0x00001333
			public string DownloadPanel { get; private set; }

			// Token: 0x1700002C RID: 44
			// (get) Token: 0x0600007A RID: 122 RVA: 0x0000313C File Offset: 0x0000133C
			// (set) Token: 0x0600007B RID: 123 RVA: 0x00003144 File Offset: 0x00001344
			public string DownloadComplete { get; private set; }

			// Token: 0x1700002D RID: 45
			// (get) Token: 0x0600007C RID: 124 RVA: 0x0000314D File Offset: 0x0000134D
			// (set) Token: 0x0600007D RID: 125 RVA: 0x00003155 File Offset: 0x00001355
			public string DownloadFailed { get; private set; }

			// Token: 0x1700002E RID: 46
			// (get) Token: 0x0600007E RID: 126 RVA: 0x0000315E File Offset: 0x0000135E
			// (set) Token: 0x0600007F RID: 127 RVA: 0x00003166 File Offset: 0x00001366
			public string Yes { get; private set; }

			// Token: 0x1700002F RID: 47
			// (get) Token: 0x06000080 RID: 128 RVA: 0x0000316F File Offset: 0x0000136F
			// (set) Token: 0x06000081 RID: 129 RVA: 0x00003177 File Offset: 0x00001377
			public string No { get; private set; }

			// Token: 0x17000030 RID: 48
			// (get) Token: 0x06000082 RID: 130 RVA: 0x00003180 File Offset: 0x00001380
			private TextObject PanelSubtitle
			{
				get
				{
					return new TextObject("{=GkwbPV4s}Maps available for '{SERVER_NAME}'", null);
				}
			}

			// Token: 0x17000031 RID: 49
			// (get) Token: 0x06000083 RID: 131 RVA: 0x0000318D File Offset: 0x0000138D
			private TextObject ProgressCounter
			{
				get
				{
					return new TextObject("{=qMfaQ3fz}{DOWNLOADED_COUNT} of {TOTAL_COUNT}", null);
				}
			}

			// Token: 0x17000032 RID: 50
			// (get) Token: 0x06000084 RID: 132 RVA: 0x0000319A File Offset: 0x0000139A
			private TextObject DownloadCompleteMessageSingular
			{
				get
				{
					return new TextObject("{=wdxXylLz}The map '{MAP_NAME}' has been downloaded into the {MODULE_NAME} module", null).SetTextVariable("MODULE_NAME", "DedicatedCustomServerHelper");
				}
			}

			// Token: 0x17000033 RID: 51
			// (get) Token: 0x06000085 RID: 133 RVA: 0x000031B6 File Offset: 0x000013B6
			private TextObject DownloadCompleteMessagePlural
			{
				get
				{
					return new TextObject("{=zifpttFx}{MAP_COUNT} maps have been downloaded into the {MODULE_NAME} module", null).SetTextVariable("MODULE_NAME", "DedicatedCustomServerHelper");
				}
			}

			// Token: 0x17000034 RID: 52
			// (get) Token: 0x06000086 RID: 134 RVA: 0x000031D2 File Offset: 0x000013D2
			private TextObject ReplacementConfirmationMessage
			{
				get
				{
					return new TextObject("{=zzX5Lkaq}'{MAP_NAME}' already exists within the helper module, should it be deleted and replaced? This action is IRREVERSIBLE.", null);
				}
			}

			// Token: 0x06000087 RID: 135 RVA: 0x000031DF File Offset: 0x000013DF
			public Texts()
			{
				this.Refresh();
			}

			// Token: 0x06000088 RID: 136 RVA: 0x000031F0 File Offset: 0x000013F0
			public void Refresh()
			{
				this.Download = new TextObject("{=a9HJ7K6I}Download", null).ToString();
				this.Downloading = new TextObject("{=adg8E1oP}Downloading...", null).ToString();
				this.Cancel = GameTexts.FindText("str_cancel", null).ToString();
				this.Close = GameTexts.FindText("str_close", null).ToString();
				this.Dismiss = GameTexts.FindText("str_dismiss", null).ToString();
				this.SelectAll = new TextObject("{=977S9OkT}Select all", null).ToString();
				this.UnselectAll = new TextObject("{=dOoPRBjm}Unselect all", null).ToString();
				this.DownloadPanel = new TextObject("{=vLSXeRnK}Download Panel", null).ToString();
				this.DownloadComplete = new TextObject("{=qhrPpmhu}Download Complete", null).ToString();
				this.DownloadFailed = new TextObject("{=7DKw0JRu}Download Failed", null).ToString();
				this.Yes = GameTexts.FindText("str_yes", null).ToString();
				this.No = GameTexts.FindText("str_no", null).ToString();
			}

			// Token: 0x06000089 RID: 137 RVA: 0x00003305 File Offset: 0x00001505
			public string GetPanelSubtitle(string serverName)
			{
				return this.PanelSubtitle.SetTextVariable("SERVER_NAME", serverName).ToString();
			}

			// Token: 0x0600008A RID: 138 RVA: 0x0000331D File Offset: 0x0000151D
			public string GetProgressCounter(int downloadedCount, int totalCount)
			{
				return this.ProgressCounter.SetTextVariable("DOWNLOADED_COUNT", downloadedCount).SetTextVariable("TOTAL_COUNT", totalCount).ToString();
			}

			// Token: 0x0600008B RID: 139 RVA: 0x00003340 File Offset: 0x00001540
			public string GetDownloadCompleteMessageSingular(string mapName)
			{
				return this.DownloadCompleteMessageSingular.SetTextVariable("MAP_NAME", mapName).ToString();
			}

			// Token: 0x0600008C RID: 140 RVA: 0x00003358 File Offset: 0x00001558
			public string GetDownloadCompleteMessagePlural(int mapCount)
			{
				return this.DownloadCompleteMessagePlural.SetTextVariable("MAP_COUNT", mapCount).ToString();
			}

			// Token: 0x0600008D RID: 141 RVA: 0x00003370 File Offset: 0x00001570
			public string GetReplacementConfirmationMessage(string mapName)
			{
				return this.ReplacementConfirmationMessage.SetTextVariable("MAP_NAME", mapName).ToString();
			}
		}
	}
}
