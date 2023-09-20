using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	public class ExposureOptionVM : ViewModel
	{
		public ExposureOptionVM(Action<bool> onClose = null)
		{
			this.TitleText = Module.CurrentModule.GlobalTextManager.FindText("str_exposure_option_title", null).ToString();
			TextObject textObject = Module.CurrentModule.GlobalTextManager.FindText("str_exposure_option_explainer", null);
			textObject.SetTextVariable("newline", "\n");
			this.ExplanationText = textObject.ToString();
			this._onClose = onClose;
			this.InitialValue = 0f;
			this.Value = this.InitialValue;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CancelText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
			this.AcceptText = new TextObject("{=Y94H6XnK}Accept", null).ToString();
		}

		public void ExecuteConfirm()
		{
			this.InitialValue = this.Value;
			NativeOptions.SetConfig(NativeOptions.NativeOptionsType.ExposureCompensation, this.Value);
			Action<bool> onClose = this._onClose;
			if (onClose != null)
			{
				onClose(true);
			}
			this.Visible = false;
		}

		public void ExecuteCancel()
		{
			this.Value = this.InitialValue;
			NativeOptions.SetConfig(NativeOptions.NativeOptionsType.ExposureCompensation, this.InitialValue);
			this.Visible = false;
			Action<bool> onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose(false);
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
		public string ExplanationText
		{
			get
			{
				return this._explanationText;
			}
			set
			{
				if (value != this._explanationText)
				{
					this._explanationText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExplanationText");
				}
			}
		}

		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		[DataSourceProperty]
		public string AcceptText
		{
			get
			{
				return this._acceptText;
			}
			set
			{
				if (value != this._acceptText)
				{
					this._acceptText = value;
					base.OnPropertyChangedWithValue<string>(value, "AcceptText");
				}
			}
		}

		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (this._value != value)
				{
					this._value = value;
					base.OnPropertyChangedWithValue(value, "Value");
					NativeOptions.SetConfig(NativeOptions.NativeOptionsType.ExposureCompensation, this.Value);
				}
			}
		}

		public float InitialValue
		{
			get
			{
				return this._initialValue;
			}
			set
			{
				if (this._initialValue != value)
				{
					this._initialValue = value;
					base.OnPropertyChangedWithValue(value, "InitialValue");
				}
			}
		}

		public bool Visible
		{
			get
			{
				return this._visible;
			}
			set
			{
				if (this._visible != value)
				{
					this._visible = value;
					base.OnPropertyChangedWithValue(value, "Visible");
					if (value)
					{
						this.Value = NativeOptions.GetConfig(NativeOptions.NativeOptionsType.ExposureCompensation);
					}
				}
			}
		}

		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetConfirmInputKey(HotKey hotkey)
		{
			this.ConfirmInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
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
		public InputKeyItemVM ConfirmInputKey
		{
			get
			{
				return this._confirmInputKey;
			}
			set
			{
				if (value != this._confirmInputKey)
				{
					this._confirmInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ConfirmInputKey");
				}
			}
		}

		private readonly Action<bool> _onClose;

		private string _titleText;

		private string _explanationText;

		private string _cancelText;

		private string _acceptText;

		private float _initialValue;

		private float _value;

		private bool _visible;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _confirmInputKey;
	}
}
