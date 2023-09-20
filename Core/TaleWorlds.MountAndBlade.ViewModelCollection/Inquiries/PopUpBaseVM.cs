using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Inquiries
{
	public abstract class PopUpBaseVM : ViewModel
	{
		public PopUpBaseVM(Action closeQuery)
		{
			this._closeQuery = closeQuery;
		}

		public abstract void ExecuteAffirmativeAction();

		public abstract void ExecuteNegativeAction();

		public virtual void OnTick(float dt)
		{
		}

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

		public void CloseQuery()
		{
			Action closeQuery = this._closeQuery;
			if (closeQuery == null)
			{
				return;
			}
			closeQuery();
		}

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
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
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
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		protected Action _affirmativeAction;

		protected Action _negativeAction;

		private Action _closeQuery;

		private string _titleText;

		private string _popUpLabel;

		private string _buttonOkLabel;

		private string _buttonCancelLabel;

		private bool _isButtonOkShown;

		private bool _isButtonCancelShown;

		private bool _isButtonOkEnabled;

		private bool _isButtonCancelEnabled;

		private HintViewModel _buttonOkHint;

		private HintViewModel _buttonCancelHint;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;
	}
}
