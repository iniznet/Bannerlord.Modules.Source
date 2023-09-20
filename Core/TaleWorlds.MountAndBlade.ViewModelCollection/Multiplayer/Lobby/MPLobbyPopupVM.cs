using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x02000060 RID: 96
	public class MPLobbyPopupVM : ViewModel
	{
		// Token: 0x0600081E RID: 2078 RVA: 0x0001EEAC File Offset: 0x0001D0AC
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject titleObj = this._titleObj;
			this.Title = ((titleObj != null) ? titleObj.ToString() : null) ?? "";
			TextObject messageObj = this._messageObj;
			this.Message = ((messageObj != null) ? messageObj.ToString() : null) ?? "";
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x0001EF01 File Offset: 0x0001D101
		public void ShowMessage(TextObject title, TextObject message)
		{
			this.IsEnabled = true;
			this._titleObj = title;
			this._messageObj = message;
			this.RefreshValues();
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0001EF1E File Offset: 0x0001D11E
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

		// Token: 0x06000821 RID: 2081 RVA: 0x0001EF51 File Offset: 0x0001D151
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

		// Token: 0x06000822 RID: 2082 RVA: 0x0001EF71 File Offset: 0x0001D171
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

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000823 RID: 2083 RVA: 0x0001EF91 File Offset: 0x0001D191
		// (set) Token: 0x06000824 RID: 2084 RVA: 0x0001EF99 File Offset: 0x0001D199
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

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000825 RID: 2085 RVA: 0x0001EFB7 File Offset: 0x0001D1B7
		// (set) Token: 0x06000826 RID: 2086 RVA: 0x0001EFBF File Offset: 0x0001D1BF
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

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000827 RID: 2087 RVA: 0x0001EFDD File Offset: 0x0001D1DD
		// (set) Token: 0x06000828 RID: 2088 RVA: 0x0001EFE5 File Offset: 0x0001D1E5
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

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000829 RID: 2089 RVA: 0x0001F008 File Offset: 0x0001D208
		// (set) Token: 0x0600082A RID: 2090 RVA: 0x0001F010 File Offset: 0x0001D210
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

		// Token: 0x04000417 RID: 1047
		private TextObject _titleObj;

		// Token: 0x04000418 RID: 1048
		private TextObject _messageObj;

		// Token: 0x04000419 RID: 1049
		private Action _onAccepted;

		// Token: 0x0400041A RID: 1050
		private Action _onDeclined;

		// Token: 0x0400041B RID: 1051
		private bool _isEnabled;

		// Token: 0x0400041C RID: 1052
		private bool _isInquiry;

		// Token: 0x0400041D RID: 1053
		private string _title;

		// Token: 0x0400041E RID: 1054
		private string _message;
	}
}
