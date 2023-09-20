using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
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

		private void ExecuteAccept()
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

		private void ExecuteDecline()
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

		private TextObject _titleObj;

		private TextObject _messageObj;

		private Action _onAccepted;

		private Action _onDeclined;

		private bool _isEnabled;

		private bool _isInquiry;

		private string _title;

		private string _message;
	}
}
