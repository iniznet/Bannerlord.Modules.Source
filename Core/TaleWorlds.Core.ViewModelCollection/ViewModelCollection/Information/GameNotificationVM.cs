using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class GameNotificationVM : ViewModel
	{
		private float CurrentNotificationOnScreenTime
		{
			get
			{
				float num = 1f;
				num += (float)this.CurrentNotification.ExtraTimeInMs / 1000f;
				int numberOfWords = this.GetNumberOfWords(this.CurrentNotification.GameNotificationText);
				if (numberOfWords > 4)
				{
					num += (float)(numberOfWords - 4) / 5f;
				}
				return num + 1f / (float)(this._items.Count + 1);
			}
		}

		public GameNotificationVM()
		{
			MBInformationManager.FiringQuickInformation += this.AddGameNotification;
			this._items = new List<GameNotificationItemVM>();
			this.CurrentNotification = new GameNotificationItemVM("NULL", 0, null, "NULL");
			this.GotNotification = false;
		}

		public void ClearNotifications()
		{
			this._items.Clear();
			this.GotNotification = false;
			this._timer = this.CurrentNotificationOnScreenTime * 2f;
		}

		public void Tick(float dt)
		{
			this._timer += dt;
			if (this.GotNotification && this._timer >= this.CurrentNotificationOnScreenTime)
			{
				this._timer = 0f;
				if (this._items.Count > 0)
				{
					this.CurrentNotification = this._items[0];
					this._items.RemoveAt(0);
					return;
				}
				this.GotNotification = false;
			}
		}

		public void AddGameNotification(string notificationText, int extraTimeInMs, BasicCharacterObject announcerCharacter, string soundId)
		{
			GameNotificationItemVM gameNotificationItemVM = new GameNotificationItemVM(notificationText, extraTimeInMs, announcerCharacter, soundId);
			if (!this._items.Any((GameNotificationItemVM i) => i.GameNotificationText == notificationText) && (!this.GotNotification || this.CurrentNotification.GameNotificationText != notificationText))
			{
				if (this.GotNotification)
				{
					this._items.Add(gameNotificationItemVM);
					return;
				}
				this.CurrentNotification = gameNotificationItemVM;
				this.TotalTime = this.CurrentNotificationOnScreenTime;
				this.GotNotification = true;
				this._timer = 0f;
			}
		}

		public GameNotificationItemVM CurrentNotification
		{
			get
			{
				return this._currentNotification;
			}
			set
			{
				if (this._currentNotification != value)
				{
					this._currentNotification = value;
					int notificationId = this.NotificationId;
					this.NotificationId = notificationId + 1;
					base.OnPropertyChangedWithValue<GameNotificationItemVM>(value, "CurrentNotification");
					if (value != null)
					{
						Action<GameNotificationItemVM> receiveNewNotification = this.ReceiveNewNotification;
						if (receiveNewNotification == null)
						{
							return;
						}
						receiveNewNotification(this.CurrentNotification);
					}
				}
			}
		}

		[DataSourceProperty]
		public bool GotNotification
		{
			get
			{
				return this._gotNotification;
			}
			set
			{
				if (value != this._gotNotification)
				{
					this._gotNotification = value;
					base.OnPropertyChangedWithValue(value, "GotNotification");
				}
			}
		}

		[DataSourceProperty]
		public int NotificationId
		{
			get
			{
				return this._notificationId;
			}
			set
			{
				if (value != this._notificationId)
				{
					this._notificationId = value;
					base.OnPropertyChangedWithValue(value, "NotificationId");
				}
			}
		}

		[DataSourceProperty]
		public float TotalTime
		{
			get
			{
				return this._totalTime;
			}
			set
			{
				if (value != this._totalTime)
				{
					this._totalTime = value;
					base.OnPropertyChangedWithValue(value, "TotalTime");
				}
			}
		}

		public event Action<GameNotificationItemVM> ReceiveNewNotification;

		private int GetNumberOfWords(string text)
		{
			string text2 = text.Trim();
			int num = 0;
			int i = 0;
			while (i < text2.Length)
			{
				while (i < text2.Length && !char.IsWhiteSpace(text2[i]))
				{
					i++;
				}
				num++;
				while (i < text2.Length && char.IsWhiteSpace(text2[i]))
				{
					i++;
				}
			}
			return num;
		}

		private readonly List<GameNotificationItemVM> _items;

		private bool _gotNotification;

		private const float MinimumDisplayTimeInSeconds = 1f;

		private const float ExtraDisplayTimeInSeconds = 1f;

		private float _timer;

		private int _notificationId;

		private GameNotificationItemVM _currentNotification;

		private float _totalTime;
	}
}
