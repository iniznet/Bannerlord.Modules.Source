using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class GameNotificationItemVM : ViewModel
	{
		public GameNotificationItemVM(string notificationText, int extraTimeInMs, BasicCharacterObject announcerCharacter, string soundId)
		{
			this.GameNotificationText = notificationText;
			this.NotificationSoundId = soundId;
			this.Announcer = ((announcerCharacter != null) ? new ImageIdentifierVM(CharacterCode.CreateFrom(announcerCharacter)) : new ImageIdentifierVM(ImageIdentifierType.Null));
			this.CharacterNameText = ((announcerCharacter != null) ? announcerCharacter.Name.ToString() : "");
			this.ExtraTimeInMs = extraTimeInMs;
		}

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

		private string _gameNotificationText;

		private string _characterNameText;

		private string _notificationSoundId;

		private ImageIdentifierVM _announcer;

		private int _extraTimeInMs;
	}
}
