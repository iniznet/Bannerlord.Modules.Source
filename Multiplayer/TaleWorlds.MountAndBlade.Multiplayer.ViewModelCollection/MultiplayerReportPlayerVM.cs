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

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MultiplayerReportPlayerVM : ViewModel
	{
		public MultiplayerReportPlayerVM(Action<string, PlayerId, string, PlayerReportType, string> onReportDone, Action onCancel)
		{
			this._onReportDone = onReportDone;
			this._onCancel = onCancel;
			this.ReportReasons = new SelectorVM<SelectorItemVM>(0, null);
			this.RefreshValues();
		}

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

		private void OnReasonSelectionChange(SelectorVM<SelectorItemVM> obj)
		{
			this.CanSendReport = obj.SelectedItem != null && obj.SelectedIndex != 0;
		}

		public void OpenNewReportWithGamePlayerId(string gameId, PlayerId playerId, string playerName, bool isRequestedFromMission)
		{
			this.ReportReasons.SelectedIndex = 0;
			this.ReportMessage = "";
			this._currentGameId = gameId;
			this._currentPlayerId = playerId;
			this._currentPlayerName = playerName;
			this.IsRequestedFromMission = isRequestedFromMission;
		}

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
					Common.DynamicInvokeWithLog(onReportDone, new object[]
					{
						this._currentGameId,
						this._currentPlayerId,
						this._currentPlayerName,
						this.ReportReasons.SelectedIndex - 1,
						this.ReportMessage
					});
				}
				this._currentGameId = string.Empty;
				this._currentPlayerId = PlayerId.Empty;
				this._currentPlayerName = string.Empty;
			}
		}

		public void ExecuteCancel()
		{
			Action onCancel = this._onCancel;
			if (onCancel == null)
			{
				return;
			}
			Common.DynamicInvokeWithLog(onCancel, Array.Empty<object>());
		}

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

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

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

		private readonly Action<string, PlayerId, string, PlayerReportType, string> _onReportDone;

		private readonly Action _onCancel;

		private string _currentGameId = string.Empty;

		private PlayerId _currentPlayerId;

		private string _currentPlayerName = string.Empty;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private string _reportMessage;

		private string _reportReasonText;

		private string _doneText;

		private string _muteDescriptionText;

		private bool _canSendReport;

		private bool _isRequestedFromMission;

		private HintViewModel _disabledReasonHint;

		private SelectorVM<SelectorItemVM> _reportReasons;
	}
}
