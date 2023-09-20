using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x0200003C RID: 60
	public class MapNotificationItemBaseVM : ViewModel
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000519 RID: 1305 RVA: 0x0001A2A3 File Offset: 0x000184A3
		// (set) Token: 0x0600051A RID: 1306 RVA: 0x0001A2AB File Offset: 0x000184AB
		public INavigationHandler NavigationHandler { get; private set; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x0001A2B4 File Offset: 0x000184B4
		// (set) Token: 0x0600051C RID: 1308 RVA: 0x0001A2BC File Offset: 0x000184BC
		private protected Action<Vec2> FastMoveCameraToPosition { protected get; private set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x0600051D RID: 1309 RVA: 0x0001A2C5 File Offset: 0x000184C5
		// (set) Token: 0x0600051E RID: 1310 RVA: 0x0001A2CD File Offset: 0x000184CD
		public InformationData Data { get; private set; }

		// Token: 0x0600051F RID: 1311 RVA: 0x0001A2D8 File Offset: 0x000184D8
		public MapNotificationItemBaseVM(InformationData data)
		{
			this.Data = data;
			this.ForceInspection = false;
			this.SoundId = data.SoundEventPath;
			this.RefreshValues();
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001A328 File Offset: 0x00018528
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = this.Data.TitleText.ToString();
			this.DescriptionText = this.Data.DescriptionText.ToString();
			this._removeHintText = this._removeHintTextObject.ToString();
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001A378 File Offset: 0x00018578
		public void SetNavigationHandler(INavigationHandler navigationHandler)
		{
			this.NavigationHandler = navigationHandler;
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0001A381 File Offset: 0x00018581
		public void SetFastMoveCameraToPosition(Action<Vec2> fastMoveCameraToPosition)
		{
			this.FastMoveCameraToPosition = fastMoveCameraToPosition;
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0001A38A File Offset: 0x0001858A
		public void ExecuteAction()
		{
			Action onInspect = this._onInspect;
			if (onInspect == null)
			{
				return;
			}
			onInspect();
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x0001A39C File Offset: 0x0001859C
		public void ExecuteRemove()
		{
			Action<MapNotificationItemBaseVM> onRemove = this.OnRemove;
			if (onRemove != null)
			{
				onRemove(this);
			}
			Action<MapNotificationItemBaseVM> onFocus = this.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(null);
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001A3C1 File Offset: 0x000185C1
		public void ExecuteSetFocused()
		{
			this.IsFocused = true;
			Action<MapNotificationItemBaseVM> onFocus = this.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(this);
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x0001A3DB File Offset: 0x000185DB
		public void ExecuteSetUnfocused()
		{
			this.IsFocused = false;
			Action<MapNotificationItemBaseVM> onFocus = this.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(null);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x0001A3F5 File Offset: 0x000185F5
		public virtual void ManualRefreshRelevantStatus()
		{
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x0001A3F7 File Offset: 0x000185F7
		internal void GoToMapPosition(Vec2 position)
		{
			Action<Vec2> fastMoveCameraToPosition = this.FastMoveCameraToPosition;
			if (fastMoveCameraToPosition == null)
			{
				return;
			}
			fastMoveCameraToPosition(position);
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0001A40A File Offset: 0x0001860A
		public void SetRemoveInputKey(HotKey hotKey)
		{
			this.RemoveInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x0600052A RID: 1322 RVA: 0x0001A419 File Offset: 0x00018619
		// (set) Token: 0x0600052B RID: 1323 RVA: 0x0001A421 File Offset: 0x00018621
		[DataSourceProperty]
		public InputKeyItemVM RemoveInputKey
		{
			get
			{
				return this._removeInputKey;
			}
			set
			{
				if (value != this._removeInputKey)
				{
					this._removeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RemoveInputKey");
				}
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x0600052C RID: 1324 RVA: 0x0001A43F File Offset: 0x0001863F
		// (set) Token: 0x0600052D RID: 1325 RVA: 0x0001A447 File Offset: 0x00018647
		[DataSourceProperty]
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChangedWithValue(value, "IsFocused");
					Action<MapNotificationItemBaseVM> onFocus = this.OnFocus;
					if (onFocus == null)
					{
						return;
					}
					onFocus(this);
				}
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x0600052E RID: 1326 RVA: 0x0001A476 File Offset: 0x00018676
		// (set) Token: 0x0600052F RID: 1327 RVA: 0x0001A47E File Offset: 0x0001867E
		[DataSourceProperty]
		public string NotificationIdentifier
		{
			get
			{
				return this._notificationIdentifier;
			}
			set
			{
				if (value != this._notificationIdentifier)
				{
					this._notificationIdentifier = value;
					base.OnPropertyChangedWithValue<string>(value, "NotificationIdentifier");
				}
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000530 RID: 1328 RVA: 0x0001A4A1 File Offset: 0x000186A1
		// (set) Token: 0x06000531 RID: 1329 RVA: 0x0001A4A9 File Offset: 0x000186A9
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
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000532 RID: 1330 RVA: 0x0001A4CC File Offset: 0x000186CC
		// (set) Token: 0x06000533 RID: 1331 RVA: 0x0001A4D4 File Offset: 0x000186D4
		[DataSourceProperty]
		public bool ForceInspection
		{
			get
			{
				return this._forceInspection;
			}
			set
			{
				if (value != this._forceInspection)
				{
					Game game = Game.Current;
					if (game != null && !game.IsDevelopmentMode)
					{
						this._forceInspection = value;
						base.OnPropertyChangedWithValue(value, "ForceInspection");
					}
				}
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000534 RID: 1332 RVA: 0x0001A508 File Offset: 0x00018708
		// (set) Token: 0x06000535 RID: 1333 RVA: 0x0001A510 File Offset: 0x00018710
		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000536 RID: 1334 RVA: 0x0001A533 File Offset: 0x00018733
		// (set) Token: 0x06000537 RID: 1335 RVA: 0x0001A53B File Offset: 0x0001873B
		[DataSourceProperty]
		public string SoundId
		{
			get
			{
				return this._soundId;
			}
			set
			{
				if (value != this._soundId)
				{
					this._soundId = value;
					base.OnPropertyChangedWithValue<string>(value, "SoundId");
				}
			}
		}

		// Token: 0x04000226 RID: 550
		internal Action<MapNotificationItemBaseVM> OnRemove;

		// Token: 0x04000227 RID: 551
		internal Action<MapNotificationItemBaseVM> OnFocus;

		// Token: 0x04000228 RID: 552
		protected Action _onInspect;

		// Token: 0x0400022A RID: 554
		private readonly TextObject _removeHintTextObject = new TextObject("{=Bcs9s2tC}Right Click to Remove", null);

		// Token: 0x0400022B RID: 555
		private string _removeHintText;

		// Token: 0x0400022C RID: 556
		private InputKeyItemVM _removeInputKey;

		// Token: 0x0400022D RID: 557
		private bool _isFocused;

		// Token: 0x0400022E RID: 558
		private string _titleText;

		// Token: 0x0400022F RID: 559
		private string _descriptionText;

		// Token: 0x04000230 RID: 560
		private string _soundId;

		// Token: 0x04000231 RID: 561
		private bool _forceInspection;

		// Token: 0x04000232 RID: 562
		private string _notificationIdentifier = "Default";
	}
}
