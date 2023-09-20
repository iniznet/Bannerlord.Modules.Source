using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x020000A0 RID: 160
	public class MPLobbyClanSendPostPopupVM : ViewModel
	{
		// Token: 0x06000F59 RID: 3929 RVA: 0x00033438 File Offset: 0x00031638
		public MPLobbyClanSendPostPopupVM(MPLobbyClanSendPostPopupVM.PostPopupMode popupMode)
		{
			this._popupMode = popupMode;
			this.RefreshValues();
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x00033450 File Offset: 0x00031650
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

		// Token: 0x06000F5B RID: 3931 RVA: 0x000334B7 File Offset: 0x000316B7
		public void ExecuteOpenPopup()
		{
			this.IsSelected = true;
			this.PostData = "";
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x000334CB File Offset: 0x000316CB
		public void ExecuteClosePopup()
		{
			this.IsSelected = false;
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x000334D4 File Offset: 0x000316D4
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

		// Token: 0x06000F5E RID: 3934 RVA: 0x0003350F File Offset: 0x0003170F
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

		// Token: 0x06000F5F RID: 3935 RVA: 0x00033538 File Offset: 0x00031738
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x00033547 File Offset: 0x00031747
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06000F61 RID: 3937 RVA: 0x00033556 File Offset: 0x00031756
		// (set) Token: 0x06000F62 RID: 3938 RVA: 0x0003355E File Offset: 0x0003175E
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

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06000F63 RID: 3939 RVA: 0x0003357B File Offset: 0x0003177B
		// (set) Token: 0x06000F64 RID: 3940 RVA: 0x00033583 File Offset: 0x00031783
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

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06000F65 RID: 3941 RVA: 0x000335A0 File Offset: 0x000317A0
		// (set) Token: 0x06000F66 RID: 3942 RVA: 0x000335A8 File Offset: 0x000317A8
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

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06000F67 RID: 3943 RVA: 0x000335C5 File Offset: 0x000317C5
		// (set) Token: 0x06000F68 RID: 3944 RVA: 0x000335CD File Offset: 0x000317CD
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

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06000F69 RID: 3945 RVA: 0x000335EF File Offset: 0x000317EF
		// (set) Token: 0x06000F6A RID: 3946 RVA: 0x000335F7 File Offset: 0x000317F7
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

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06000F6B RID: 3947 RVA: 0x00033619 File Offset: 0x00031819
		// (set) Token: 0x06000F6C RID: 3948 RVA: 0x00033621 File Offset: 0x00031821
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

		// Token: 0x0400073E RID: 1854
		private MPLobbyClanSendPostPopupVM.PostPopupMode _popupMode;

		// Token: 0x0400073F RID: 1855
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000740 RID: 1856
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000741 RID: 1857
		private bool _isSelected;

		// Token: 0x04000742 RID: 1858
		private string _titleText;

		// Token: 0x04000743 RID: 1859
		private string _postData;

		// Token: 0x04000744 RID: 1860
		private string _sendText;

		// Token: 0x020001FC RID: 508
		public enum PostPopupMode
		{
			// Token: 0x04000E38 RID: 3640
			Information,
			// Token: 0x04000E39 RID: 3641
			Announcement
		}
	}
}
