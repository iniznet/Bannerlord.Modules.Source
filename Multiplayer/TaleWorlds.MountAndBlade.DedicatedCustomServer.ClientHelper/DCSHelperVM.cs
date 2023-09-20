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
	public class DCSHelperVM : ViewModel
	{
		public IEnumerable<DCSHelperMapItemVM> SelectedMaps
		{
			get
			{
				return this.MapList.Where((DCSHelperMapItemVM map) => map.IsSelected);
			}
		}

		public IEnumerable<DCSHelperMapItemVM> SelectableMaps
		{
			get
			{
				return this.MapList.Where((DCSHelperMapItemVM map) => !map.IsSelected);
			}
		}

		public DCSHelperVM(string hostAddress, string fullName = null)
		{
			this._hostAddress = hostAddress;
			this._fullName = fullName;
			this._texts = new DCSHelperVM.Texts();
			this.IsDownloading = false;
			this.ShowProgress = false;
			this.PanelTitleText = this._texts.DownloadPanel;
			this.DownloadButtonText = this._texts.Download;
			this.CloseButtonText = this._texts.Close;
			this.HostAddressText = this._texts.GetPanelSubtitle(this.Truncate(this._fullName, 40) ?? this._hostAddress);
			this.ToggleSelectionButtonText = this._texts.SelectAll;
			this.MapList = new MBBindingList<DCSHelperMapItemVM>();
		}

		public DCSHelperVM(GameServerEntry server)
			: this(string.Format("{0}:{1}", server.Address, server.Port), server.ServerName)
		{
		}

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

		public async Task OpenPopup()
		{
			this._gauntletLayer = new GauntletLayer(20, "GauntletLayer", false);
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

		private Task<bool> WaitForConfirmationToReplace(string mapName)
		{
			DCSHelperVM.<>c__DisplayClass18_0 CS$<>8__locals1 = new DCSHelperVM.<>c__DisplayClass18_0();
			CS$<>8__locals1.taskSource = new TaskCompletionSource<bool>();
			InformationManager.ShowInquiry(new InquiryData(this._texts.DownloadPanel, this._texts.GetReplacementConfirmationMessage(mapName), true, true, this._texts.Yes, this._texts.No, CS$<>8__locals1.<WaitForConfirmationToReplace>g__getAction|0(true), CS$<>8__locals1.<WaitForConfirmationToReplace>g__getAction|0(false), "", 0f, null, null, null), false, false);
			return CS$<>8__locals1.taskSource.Task;
		}

		private void ShowDownloadCompleteInquiry(List<DCSHelperMapItemVM> downloadedMaps)
		{
			InformationManager.ShowInquiry(new InquiryData(this._texts.DownloadComplete, (downloadedMaps.Count == 1) ? this._texts.GetDownloadCompleteMessageSingular(downloadedMaps.Single<DCSHelperMapItemVM>().MapName) : this._texts.GetDownloadCompleteMessagePlural(downloadedMaps.Count), true, false, this._texts.Dismiss, "", null, null, "", 0f, null, null, null), false, false);
		}

		private void ShowDownloadFailedInquiry(string reason)
		{
			InformationManager.ShowInquiry(new InquiryData(this._texts.DownloadFailed, reason, false, true, "", this._texts.Dismiss, null, null, "", 0f, null, null, null), false, false);
		}

		private void ShowFailedToRetrieveInquiry(string reason)
		{
			InformationManager.ShowInquiry(new InquiryData(this._texts.DownloadPanel, reason, false, true, "", this._texts.Dismiss, null, null, "", 0f, null, null, null), false, false);
		}

		public void ExecuteCloseOrCancel()
		{
			if (this.IsDownloading)
			{
				this.ExecuteCancelDownload();
				return;
			}
			this.ExecuteClosePopup();
		}

		public void ExecuteClosePopup()
		{
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			ScreenManager.TopScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
		}

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

		[DataSourceProperty]
		public bool ReadyToDownload
		{
			get
			{
				return !this._isDownloading && this.SelectedMaps.Count<DCSHelperMapItemVM>() != 0;
			}
		}

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

		private readonly string _hostAddress;

		private readonly string _fullName;

		private readonly DCSHelperVM.Texts _texts;

		private GauntletLayer _gauntletLayer;

		private CancellationTokenSource _cancellationTokenSource;

		private bool _isLoading;

		private bool _isDownloading;

		private bool _showProgress;

		private string _panelTitleText;

		private string _downloadButtonText;

		private string _closeButtonText;

		private string _toggleSelectionButtonText;

		private string _hostAddressText;

		private string _progressCounterText;

		private string _progressText;

		private float _downloadRatio;

		private MBBindingList<DCSHelperMapItemVM> _mapList;

		private class Texts
		{
			public string Download { get; private set; }

			public string Downloading { get; private set; }

			public string Cancel { get; private set; }

			public string Close { get; private set; }

			public string Dismiss { get; private set; }

			public string SelectAll { get; private set; }

			public string UnselectAll { get; private set; }

			public string DownloadPanel { get; private set; }

			public string DownloadComplete { get; private set; }

			public string DownloadFailed { get; private set; }

			public string Yes { get; private set; }

			public string No { get; private set; }

			private TextObject PanelSubtitle
			{
				get
				{
					return new TextObject("{=GkwbPV4s}Maps available for '{SERVER_NAME}'", null);
				}
			}

			private TextObject ProgressCounter
			{
				get
				{
					return new TextObject("{=qMfaQ3fz}{DOWNLOADED_COUNT} of {TOTAL_COUNT}", null);
				}
			}

			private TextObject DownloadCompleteMessageSingular
			{
				get
				{
					return new TextObject("{=wdxXylLz}The map '{MAP_NAME}' has been downloaded into the {MODULE_NAME} module", null).SetTextVariable("MODULE_NAME", "Multiplayer");
				}
			}

			private TextObject DownloadCompleteMessagePlural
			{
				get
				{
					return new TextObject("{=zifpttFx}{MAP_COUNT} maps have been downloaded into the {MODULE_NAME} module", null).SetTextVariable("MODULE_NAME", "Multiplayer");
				}
			}

			private TextObject ReplacementConfirmationMessage
			{
				get
				{
					return new TextObject("{=DluuLzfU}'{MAP_NAME}' already exists within the {MODULE_NAME} module, should it be deleted and replaced? This action is IRREVERSIBLE.", null).SetTextVariable("MODULE_NAME", "Multiplayer");
				}
			}

			public Texts()
			{
				this.Refresh();
			}

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

			public string GetPanelSubtitle(string serverName)
			{
				return this.PanelSubtitle.SetTextVariable("SERVER_NAME", serverName).ToString();
			}

			public string GetProgressCounter(int downloadedCount, int totalCount)
			{
				return this.ProgressCounter.SetTextVariable("DOWNLOADED_COUNT", downloadedCount).SetTextVariable("TOTAL_COUNT", totalCount).ToString();
			}

			public string GetDownloadCompleteMessageSingular(string mapName)
			{
				return this.DownloadCompleteMessageSingular.SetTextVariable("MAP_NAME", mapName).ToString();
			}

			public string GetDownloadCompleteMessagePlural(int mapCount)
			{
				return this.DownloadCompleteMessagePlural.SetTextVariable("MAP_COUNT", mapCount).ToString();
			}

			public string GetReplacementConfirmationMessage(string mapName)
			{
				return this.ReplacementConfirmationMessage.SetTextVariable("MAP_NAME", mapName).ToString();
			}
		}
	}
}
