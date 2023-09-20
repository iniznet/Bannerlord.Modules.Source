using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000048 RID: 72
	public class MultiplayerReportPlayerVM : ViewModel
	{
		// Token: 0x060005EF RID: 1519 RVA: 0x00018D9B File Offset: 0x00016F9B
		public MultiplayerReportPlayerVM(Action<string, PlayerId, string, PlayerReportType, string> onReportDone, Action onCancel)
		{
			this._onReportDone = onReportDone;
			this._onCancel = onCancel;
			this.ReportReasons = new SelectorVM<SelectorItemVM>(0, null);
			this.RefreshValues();
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x00018DDC File Offset: 0x00016FDC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DisabledReasonHint = new HintViewModel(new TextObject("{=klkYFik9}You've already reported this player.", null), null);
			this.ReportReasonText = new TextObject("{=cw5QyeRU}Report Reason", null).ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.MuteDescriptionText = new TextObject("{=gGa3ZhqN}This player will be muted automatically.", null).ToString();
			List<string> list = new List<string> { new TextObject("{=koX9okuG}None", null).ToString() };
			foreach (object obj in Enum.GetValues(typeof(PlayerReportType)))
			{
				list.Add(GameTexts.FindText("str_multiplayer_report_reason", ((int)obj).ToString()).ToString());
			}
			this.ReportReasons.Refresh(list, 0, new Action<SelectorVM<SelectorItemVM>>(this.OnReasonSelectionChange));
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x00018EEC File Offset: 0x000170EC
		private void OnReasonSelectionChange(SelectorVM<SelectorItemVM> obj)
		{
			this.CanSendReport = obj.SelectedItem != null && obj.SelectedIndex != 0;
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x00018F08 File Offset: 0x00017108
		public void OpenNewReportWithGamePlayerId(string gameId, PlayerId playerId, string playerName, bool isRequestedFromMission)
		{
			this.ReportReasons.SelectedIndex = 0;
			this.ReportMessage = "";
			this._currentGameId = gameId;
			this._currentPlayerId = playerId;
			this._currentPlayerName = playerName;
			this.IsRequestedFromMission = isRequestedFromMission;
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x00018F40 File Offset: 0x00017140
		public void ExecuteDone()
		{
			if (this.CanSendReport && this.ReportReasons.SelectedIndex > 0 && this._currentGameId != string.Empty && this._currentPlayerId != PlayerId.Empty)
			{
				if (this.ReportMessage.Length > 500)
				{
					this.ReportMessage = this.ReportMessage.Substring(0, 500);
				}
				Action<string, PlayerId, string, PlayerReportType, string> onReportDone = this._onReportDone;
				if (onReportDone != null)
				{
					onReportDone.DynamicInvokeWithLog(new object[]
					{
						this._currentGameId,
						this._currentPlayerId,
						this._currentPlayerName,
						(PlayerReportType)(this.ReportReasons.SelectedIndex - 1),
						this.ReportMessage
					});
				}
				this._currentGameId = string.Empty;
				this._currentPlayerId = PlayerId.Empty;
				this._currentPlayerName = string.Empty;
			}
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x00019033 File Offset: 0x00017233
		public void ExecuteCancel()
		{
			Action onCancel = this._onCancel;
			if (onCancel == null)
			{
				return;
			}
			onCancel.DynamicInvokeWithLog(Array.Empty<object>());
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0001904B File Offset: 0x0001724B
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x00019074 File Offset: 0x00017274
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00019083 File Offset: 0x00017283
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x060005F8 RID: 1528 RVA: 0x00019092 File Offset: 0x00017292
		// (set) Token: 0x060005F9 RID: 1529 RVA: 0x0001909A File Offset: 0x0001729A
		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChanged("CancelInputKey");
				}
			}
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x060005FA RID: 1530 RVA: 0x000190B7 File Offset: 0x000172B7
		// (set) Token: 0x060005FB RID: 1531 RVA: 0x000190BF File Offset: 0x000172BF
		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChanged("DoneInputKey");
				}
			}
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x000190DC File Offset: 0x000172DC
		// (set) Token: 0x060005FD RID: 1533 RVA: 0x000190E4 File Offset: 0x000172E4
		[DataSourceProperty]
		public string ReportMessage
		{
			get
			{
				return this._reportMessage;
			}
			set
			{
				if (this._reportMessage != value)
				{
					this._reportMessage = value;
					base.OnPropertyChangedWithValue<string>(value, "ReportMessage");
				}
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x060005FE RID: 1534 RVA: 0x00019107 File Offset: 0x00017307
		// (set) Token: 0x060005FF RID: 1535 RVA: 0x0001910F File Offset: 0x0001730F
		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (this._doneText != value)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000600 RID: 1536 RVA: 0x00019132 File Offset: 0x00017332
		// (set) Token: 0x06000601 RID: 1537 RVA: 0x0001913A File Offset: 0x0001733A
		[DataSourceProperty]
		public string ReportReasonText
		{
			get
			{
				return this._reportReasonText;
			}
			set
			{
				if (this._reportReasonText != value)
				{
					this._reportReasonText = value;
					base.OnPropertyChangedWithValue<string>(value, "ReportReasonText");
				}
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000602 RID: 1538 RVA: 0x0001915D File Offset: 0x0001735D
		// (set) Token: 0x06000603 RID: 1539 RVA: 0x00019165 File Offset: 0x00017365
		[DataSourceProperty]
		public bool CanSendReport
		{
			get
			{
				return this._canSendReport;
			}
			set
			{
				if (this._canSendReport != value)
				{
					this._canSendReport = value;
					base.OnPropertyChangedWithValue(value, "CanSendReport");
				}
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000604 RID: 1540 RVA: 0x00019183 File Offset: 0x00017383
		// (set) Token: 0x06000605 RID: 1541 RVA: 0x0001918B File Offset: 0x0001738B
		[DataSourceProperty]
		public bool IsRequestedFromMission
		{
			get
			{
				return this._isRequestedFromMission;
			}
			set
			{
				if (value != this._isRequestedFromMission)
				{
					this._isRequestedFromMission = value;
					base.OnPropertyChangedWithValue(value, "IsRequestedFromMission");
				}
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x000191A9 File Offset: 0x000173A9
		// (set) Token: 0x06000607 RID: 1543 RVA: 0x000191B1 File Offset: 0x000173B1
		[DataSourceProperty]
		public string MuteDescriptionText
		{
			get
			{
				return this._muteDescriptionText;
			}
			set
			{
				if (value != this._muteDescriptionText)
				{
					this._muteDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "MuteDescriptionText");
				}
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x000191D4 File Offset: 0x000173D4
		// (set) Token: 0x06000609 RID: 1545 RVA: 0x000191DC File Offset: 0x000173DC
		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> ReportReasons
		{
			get
			{
				return this._reportReasons;
			}
			set
			{
				if (this._reportReasons != value)
				{
					this._reportReasons = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "ReportReasons");
				}
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x000191FA File Offset: 0x000173FA
		// (set) Token: 0x0600060B RID: 1547 RVA: 0x00019202 File Offset: 0x00017402
		[DataSourceProperty]
		public HintViewModel DisabledReasonHint
		{
			get
			{
				return this._disabledReasonHint;
			}
			set
			{
				if (this._disabledReasonHint != value)
				{
					this._disabledReasonHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisabledReasonHint");
				}
			}
		}

		// Token: 0x040002FF RID: 767
		private readonly Action<string, PlayerId, string, PlayerReportType, string> _onReportDone;

		// Token: 0x04000300 RID: 768
		private readonly Action _onCancel;

		// Token: 0x04000301 RID: 769
		private string _currentGameId = string.Empty;

		// Token: 0x04000302 RID: 770
		private PlayerId _currentPlayerId;

		// Token: 0x04000303 RID: 771
		private string _currentPlayerName = string.Empty;

		// Token: 0x04000304 RID: 772
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000305 RID: 773
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000306 RID: 774
		private string _reportMessage;

		// Token: 0x04000307 RID: 775
		private string _reportReasonText;

		// Token: 0x04000308 RID: 776
		private string _doneText;

		// Token: 0x04000309 RID: 777
		private string _muteDescriptionText;

		// Token: 0x0400030A RID: 778
		private bool _canSendReport;

		// Token: 0x0400030B RID: 779
		private bool _isRequestedFromMission;

		// Token: 0x0400030C RID: 780
		private HintViewModel _disabledReasonHint;

		// Token: 0x0400030D RID: 781
		private SelectorVM<SelectorItemVM> _reportReasons;
	}
}
