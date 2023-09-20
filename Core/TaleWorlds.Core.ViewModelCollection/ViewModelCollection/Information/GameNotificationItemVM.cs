using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x02000014 RID: 20
	public class GameNotificationItemVM : ViewModel
	{
		// Token: 0x060000E5 RID: 229 RVA: 0x00003A50 File Offset: 0x00001C50
		public GameNotificationItemVM(string notificationText, int extraTimeInMs, BasicCharacterObject announcerCharacter, string soundId)
		{
			this.GameNotificationText = notificationText;
			this.NotificationSoundId = soundId;
			this.Announcer = ((announcerCharacter != null) ? new ImageIdentifierVM(CharacterCode.CreateFrom(announcerCharacter)) : new ImageIdentifierVM(ImageIdentifierType.Null));
			this.CharacterNameText = ((announcerCharacter != null) ? announcerCharacter.Name.ToString() : "");
			this.ExtraTimeInMs = extraTimeInMs;
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00003AB0 File Offset: 0x00001CB0
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x00003AB8 File Offset: 0x00001CB8
		[DataSourceProperty]
		public ImageIdentifierVM Announcer
		{
			get
			{
				return this._announcer;
			}
			set
			{
				if (value != this._announcer)
				{
					this._announcer = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Announcer");
				}
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00003AD6 File Offset: 0x00001CD6
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00003ADE File Offset: 0x00001CDE
		[DataSourceProperty]
		public int ExtraTimeInMs
		{
			get
			{
				return this._extraTimeInMs;
			}
			set
			{
				if (value != this._extraTimeInMs)
				{
					this._extraTimeInMs = value;
					base.OnPropertyChangedWithValue(value, "ExtraTimeInMs");
				}
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00003AFC File Offset: 0x00001CFC
		// (set) Token: 0x060000EB RID: 235 RVA: 0x00003B04 File Offset: 0x00001D04
		[DataSourceProperty]
		public string GameNotificationText
		{
			get
			{
				return this._gameNotificationText;
			}
			set
			{
				if (value != this._gameNotificationText)
				{
					this._gameNotificationText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameNotificationText");
				}
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00003B27 File Offset: 0x00001D27
		// (set) Token: 0x060000ED RID: 237 RVA: 0x00003B2F File Offset: 0x00001D2F
		[DataSourceProperty]
		public string CharacterNameText
		{
			get
			{
				return this._characterNameText;
			}
			set
			{
				if (value != this._characterNameText)
				{
					this._characterNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "CharacterNameText");
				}
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000EE RID: 238 RVA: 0x00003B52 File Offset: 0x00001D52
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00003B5A File Offset: 0x00001D5A
		[DataSourceProperty]
		public string NotificationSoundId
		{
			get
			{
				return this._notificationSoundId;
			}
			set
			{
				if (value != this._notificationSoundId)
				{
					this._notificationSoundId = value;
					base.OnPropertyChangedWithValue<string>(value, "NotificationSoundId");
				}
			}
		}

		// Token: 0x04000057 RID: 87
		private string _gameNotificationText;

		// Token: 0x04000058 RID: 88
		private string _characterNameText;

		// Token: 0x04000059 RID: 89
		private string _notificationSoundId;

		// Token: 0x0400005A RID: 90
		private ImageIdentifierVM _announcer;

		// Token: 0x0400005B RID: 91
		private int _extraTimeInMs;
	}
}
