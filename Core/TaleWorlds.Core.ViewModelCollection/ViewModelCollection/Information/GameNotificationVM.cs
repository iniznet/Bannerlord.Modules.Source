using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x02000015 RID: 21
	public class GameNotificationVM : ViewModel
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x00003B80 File Offset: 0x00001D80
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

		// Token: 0x060000F1 RID: 241 RVA: 0x00003BE4 File Offset: 0x00001DE4
		public GameNotificationVM()
		{
			MBInformationManager.FiringQuickInformation += this.AddGameNotification;
			this._items = new List<GameNotificationItemVM>();
			this.CurrentNotification = new GameNotificationItemVM("NULL", 0, null, "NULL");
			this.GotNotification = false;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00003C31 File Offset: 0x00001E31
		public void ClearNotifications()
		{
			this._items.Clear();
			this.GotNotification = false;
			this._timer = this.CurrentNotificationOnScreenTime * 2f;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00003C58 File Offset: 0x00001E58
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

		// Token: 0x060000F4 RID: 244 RVA: 0x00003CC8 File Offset: 0x00001EC8
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

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x00003D65 File Offset: 0x00001F65
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x00003D70 File Offset: 0x00001F70
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

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00003DC2 File Offset: 0x00001FC2
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x00003DCA File Offset: 0x00001FCA
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

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00003DE8 File Offset: 0x00001FE8
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00003DF0 File Offset: 0x00001FF0
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

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00003E0E File Offset: 0x0000200E
		// (set) Token: 0x060000FC RID: 252 RVA: 0x00003E16 File Offset: 0x00002016
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

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000FD RID: 253 RVA: 0x00003E34 File Offset: 0x00002034
		// (remove) Token: 0x060000FE RID: 254 RVA: 0x00003E6C File Offset: 0x0000206C
		public event Action<GameNotificationItemVM> ReceiveNewNotification;

		// Token: 0x060000FF RID: 255 RVA: 0x00003EA4 File Offset: 0x000020A4
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

		// Token: 0x0400005C RID: 92
		private readonly List<GameNotificationItemVM> _items;

		// Token: 0x0400005D RID: 93
		private bool _gotNotification;

		// Token: 0x0400005E RID: 94
		private const float MinimumDisplayTimeInSeconds = 1f;

		// Token: 0x0400005F RID: 95
		private const float ExtraDisplayTimeInSeconds = 1f;

		// Token: 0x04000060 RID: 96
		private float _timer;

		// Token: 0x04000061 RID: 97
		private int _notificationId;

		// Token: 0x04000062 RID: 98
		private GameNotificationItemVM _currentNotification;

		// Token: 0x04000063 RID: 99
		private float _totalTime;
	}
}
