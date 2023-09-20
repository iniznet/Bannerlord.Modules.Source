using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPLobbyPopupVM : ViewModel
	{
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject titleObj = this._titleObj;
			this.Title = ((titleObj != null) ? titleObj.ToString() : null) ?? "";
			TextObject messageObj = this._messageObj;
			this.Message = ((messageObj != null) ? messageObj.ToString() : null) ?? "";
		}

		public void ShowMessage(TextObject title, TextObject message)
		{
			this.IsEnabled = true;
			this._titleObj = title;
			this._messageObj = message;
			this.RefreshValues();
		}

		public void ShowInquiry(TextObject title, TextObject message, Action onAccepted, Action onDeclined)
		{
			this.IsEnabled = true;
			this.IsInquiry = true;
			this._titleObj = title;
			this._messageObj = message;
			this._onAccepted = onAccepted;
			this._onDeclined = onDeclined;
			this.RefreshValues();
		}

		public void ExecuteAccept()
		{
			this.IsEnabled = false;
			this.IsInquiry = false;
			Action onAccepted = this._onAccepted;
			if (onAccepted == null)
			{
				return;
			}
			onAccepted();
		}

		public void ExecuteDecline()
		{
			this.IsEnabled = false;
			this.IsInquiry = false;
			Action onDeclined = this._onDeclined;
			if (onDeclined == null)
			{
				return;
			}
			onDeclined();
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
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
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
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInquiry
		{
			get
			{
				return this._isInquiry;
			}
			set
			{
				if (value != this._isInquiry)
				{
					this._isInquiry = value;
					base.OnPropertyChangedWithValue(value, "IsInquiry");
				}
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public string Message
		{
			get
			{
				return this._message;
			}
			set
			{
				if (value != this._message)
				{
					this._message = value;
					base.OnPropertyChangedWithValue<string>(value, "Message");
				}
			}
		}

		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		private TextObject _titleObj;

		private TextObject _messageObj;

		private Action _onAccepted;

		private Action _onDeclined;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _cancelInputKey;

		private bool _isEnabled;

		private bool _isInquiry;

		private string _title;

		private string _message;
	}
}
