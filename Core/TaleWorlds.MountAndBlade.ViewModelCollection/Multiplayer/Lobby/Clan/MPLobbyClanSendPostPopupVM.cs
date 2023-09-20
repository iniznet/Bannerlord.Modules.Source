using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	public class MPLobbyClanSendPostPopupVM : ViewModel
	{
		public MPLobbyClanSendPostPopupVM(MPLobbyClanSendPostPopupVM.PostPopupMode popupMode)
		{
			this._popupMode = popupMode;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SendText = new TextObject("{=qTYsYJ9V}Send", null).ToString();
			if (this._popupMode == MPLobbyClanSendPostPopupVM.PostPopupMode.Information)
			{
				this.TitleText = new TextObject("{=zravuI1b}Type Clan Information", null).ToString();
				return;
			}
			if (this._popupMode == MPLobbyClanSendPostPopupVM.PostPopupMode.Announcement)
			{
				this.TitleText = new TextObject("{=g5W32uf4}Type Your Announcement", null).ToString();
			}
		}

		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
			this.PostData = "";
		}

		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
		}

		public void ExecuteSend()
		{
			if (this._popupMode == MPLobbyClanSendPostPopupVM.PostPopupMode.Information)
			{
				NetworkMain.GameClient.SetClanInformationText(this.PostData);
			}
			else if (this._popupMode == MPLobbyClanSendPostPopupVM.PostPopupMode.Announcement)
			{
				NetworkMain.GameClient.AddClanAnnouncement(this.PostData);
			}
			this.ExecuteClosePopup();
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
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChanged("IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChanged("TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string PostData
		{
			get
			{
				return this._postData;
			}
			set
			{
				if (value != this._postData)
				{
					this._postData = value;
					base.OnPropertyChanged("PostData");
				}
			}
		}

		[DataSourceProperty]
		public string SendText
		{
			get
			{
				return this._sendText;
			}
			set
			{
				if (value != this._sendText)
				{
					this._sendText = value;
					base.OnPropertyChanged("SendText");
				}
			}
		}

		private MPLobbyClanSendPostPopupVM.PostPopupMode _popupMode;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private bool _isSelected;

		private string _titleText;

		private string _postData;

		private string _sendText;

		public enum PostPopupMode
		{
			Information,
			Announcement
		}
	}
}
