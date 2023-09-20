using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Inquiries
{
	// Token: 0x020000D1 RID: 209
	public abstract class PopUpBaseVM : ViewModel
	{
		// Token: 0x0600138C RID: 5004 RVA: 0x0004081B File Offset: 0x0003EA1B
		public PopUpBaseVM(Action closeQuery)
		{
			this._closeQuery = closeQuery;
		}

		// Token: 0x0600138D RID: 5005
		public abstract void ExecuteAffirmativeAction();

		// Token: 0x0600138E RID: 5006
		public abstract void ExecuteNegativeAction();

		// Token: 0x0600138F RID: 5007 RVA: 0x0004082A File Offset: 0x0003EA2A
		public virtual void OnTick(float dt)
		{
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x0004082C File Offset: 0x0003EA2C
		public virtual void OnClearData()
		{
			this.TitleText = null;
			this.PopUpLabel = null;
			this.ButtonOkLabel = null;
			this.ButtonCancelLabel = null;
			this.IsButtonOkShown = false;
			this.IsButtonCancelShown = false;
			this.IsButtonOkEnabled = false;
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x0004085F File Offset: 0x0003EA5F
		public void ForceRefreshKeyVisuals()
		{
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.RefreshValues();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.RefreshValues();
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x00040882 File Offset: 0x0003EA82
		public void CloseQuery()
		{
			Action closeQuery = this._closeQuery;
			if (closeQuery == null)
			{
				return;
			}
			closeQuery();
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x00040894 File Offset: 0x0003EA94
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey == null)
			{
				return;
			}
			cancelInputKey.OnFinalize();
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06001394 RID: 5012 RVA: 0x000408BD File Offset: 0x0003EABD
		// (set) Token: 0x06001395 RID: 5013 RVA: 0x000408C5 File Offset: 0x0003EAC5
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

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06001396 RID: 5014 RVA: 0x000408E8 File Offset: 0x0003EAE8
		// (set) Token: 0x06001397 RID: 5015 RVA: 0x000408F0 File Offset: 0x0003EAF0
		[DataSourceProperty]
		public string PopUpLabel
		{
			get
			{
				return this._popUpLabel;
			}
			set
			{
				if (value != this._popUpLabel)
				{
					this._popUpLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "PopUpLabel");
				}
			}
		}

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06001398 RID: 5016 RVA: 0x00040913 File Offset: 0x0003EB13
		// (set) Token: 0x06001399 RID: 5017 RVA: 0x0004091B File Offset: 0x0003EB1B
		[DataSourceProperty]
		public string ButtonOkLabel
		{
			get
			{
				return this._buttonOkLabel;
			}
			set
			{
				if (value != this._buttonOkLabel)
				{
					this._buttonOkLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "ButtonOkLabel");
				}
			}
		}

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x0600139A RID: 5018 RVA: 0x0004093E File Offset: 0x0003EB3E
		// (set) Token: 0x0600139B RID: 5019 RVA: 0x00040946 File Offset: 0x0003EB46
		[DataSourceProperty]
		public string ButtonCancelLabel
		{
			get
			{
				return this._buttonCancelLabel;
			}
			set
			{
				if (value != this._buttonCancelLabel)
				{
					this._buttonCancelLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "ButtonCancelLabel");
				}
			}
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x0600139C RID: 5020 RVA: 0x00040969 File Offset: 0x0003EB69
		// (set) Token: 0x0600139D RID: 5021 RVA: 0x00040971 File Offset: 0x0003EB71
		[DataSourceProperty]
		public bool IsButtonOkShown
		{
			get
			{
				return this._isButtonOkShown;
			}
			set
			{
				if (value != this._isButtonOkShown)
				{
					this._isButtonOkShown = value;
					base.OnPropertyChangedWithValue(value, "IsButtonOkShown");
				}
			}
		}

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x0600139E RID: 5022 RVA: 0x0004098F File Offset: 0x0003EB8F
		// (set) Token: 0x0600139F RID: 5023 RVA: 0x00040997 File Offset: 0x0003EB97
		[DataSourceProperty]
		public bool IsButtonCancelShown
		{
			get
			{
				return this._isButtonCancelShown;
			}
			set
			{
				if (value != this._isButtonCancelShown)
				{
					this._isButtonCancelShown = value;
					base.OnPropertyChangedWithValue(value, "IsButtonCancelShown");
				}
			}
		}

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x060013A0 RID: 5024 RVA: 0x000409B5 File Offset: 0x0003EBB5
		// (set) Token: 0x060013A1 RID: 5025 RVA: 0x000409BD File Offset: 0x0003EBBD
		[DataSourceProperty]
		public bool IsButtonOkEnabled
		{
			get
			{
				return this._isButtonOkEnabled;
			}
			set
			{
				if (value != this._isButtonOkEnabled)
				{
					this._isButtonOkEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsButtonOkEnabled");
				}
			}
		}

		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x060013A2 RID: 5026 RVA: 0x000409DB File Offset: 0x0003EBDB
		// (set) Token: 0x060013A3 RID: 5027 RVA: 0x000409E3 File Offset: 0x0003EBE3
		[DataSourceProperty]
		public bool IsButtonCancelEnabled
		{
			get
			{
				return this._isButtonCancelEnabled;
			}
			set
			{
				if (value != this._isButtonCancelEnabled)
				{
					this._isButtonCancelEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsButtonCancelEnabled");
				}
			}
		}

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x060013A4 RID: 5028 RVA: 0x00040A01 File Offset: 0x0003EC01
		// (set) Token: 0x060013A5 RID: 5029 RVA: 0x00040A09 File Offset: 0x0003EC09
		[DataSourceProperty]
		public HintViewModel ButtonOkHint
		{
			get
			{
				return this._buttonOkHint;
			}
			set
			{
				if (value != this._buttonOkHint)
				{
					this._buttonOkHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ButtonOkHint");
				}
			}
		}

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x060013A6 RID: 5030 RVA: 0x00040A27 File Offset: 0x0003EC27
		// (set) Token: 0x060013A7 RID: 5031 RVA: 0x00040A2F File Offset: 0x0003EC2F
		[DataSourceProperty]
		public HintViewModel ButtonCancelHint
		{
			get
			{
				return this._buttonCancelHint;
			}
			set
			{
				if (value != this._buttonCancelHint)
				{
					this._buttonCancelHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ButtonCancelHint");
				}
			}
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x00040A4D File Offset: 0x0003EC4D
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x00040A5C File Offset: 0x0003EC5C
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x060013AA RID: 5034 RVA: 0x00040A6B File Offset: 0x0003EC6B
		// (set) Token: 0x060013AB RID: 5035 RVA: 0x00040A73 File Offset: 0x0003EC73
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

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x060013AC RID: 5036 RVA: 0x00040A91 File Offset: 0x0003EC91
		// (set) Token: 0x060013AD RID: 5037 RVA: 0x00040A99 File Offset: 0x0003EC99
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

		// Token: 0x04000966 RID: 2406
		protected Action _affirmativeAction;

		// Token: 0x04000967 RID: 2407
		protected Action _negativeAction;

		// Token: 0x04000968 RID: 2408
		private Action _closeQuery;

		// Token: 0x04000969 RID: 2409
		private string _titleText;

		// Token: 0x0400096A RID: 2410
		private string _popUpLabel;

		// Token: 0x0400096B RID: 2411
		private string _buttonOkLabel;

		// Token: 0x0400096C RID: 2412
		private string _buttonCancelLabel;

		// Token: 0x0400096D RID: 2413
		private bool _isButtonOkShown;

		// Token: 0x0400096E RID: 2414
		private bool _isButtonCancelShown;

		// Token: 0x0400096F RID: 2415
		private bool _isButtonOkEnabled;

		// Token: 0x04000970 RID: 2416
		private bool _isButtonCancelEnabled;

		// Token: 0x04000971 RID: 2417
		private HintViewModel _buttonOkHint;

		// Token: 0x04000972 RID: 2418
		private HintViewModel _buttonCancelHint;

		// Token: 0x04000973 RID: 2419
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000974 RID: 2420
		private InputKeyItemVM _doneInputKey;
	}
}
