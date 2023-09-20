using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000F5 RID: 245
	public class ExposureOptionVM : ViewModel
	{
		// Token: 0x0600159C RID: 5532 RVA: 0x00045E4C File Offset: 0x0004404C
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

		// Token: 0x0600159D RID: 5533 RVA: 0x00045ED6 File Offset: 0x000440D6
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CancelText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
			this.AcceptText = new TextObject("{=Y94H6XnK}Accept", null).ToString();
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x00045F0A File Offset: 0x0004410A
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

		// Token: 0x0600159F RID: 5535 RVA: 0x00045F3E File Offset: 0x0004413E
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

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x060015A0 RID: 5536 RVA: 0x00045F71 File Offset: 0x00044171
		// (set) Token: 0x060015A1 RID: 5537 RVA: 0x00045F79 File Offset: 0x00044179
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

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x060015A2 RID: 5538 RVA: 0x00045F9C File Offset: 0x0004419C
		// (set) Token: 0x060015A3 RID: 5539 RVA: 0x00045FA4 File Offset: 0x000441A4
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

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x060015A4 RID: 5540 RVA: 0x00045FC7 File Offset: 0x000441C7
		// (set) Token: 0x060015A5 RID: 5541 RVA: 0x00045FCF File Offset: 0x000441CF
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

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x060015A6 RID: 5542 RVA: 0x00045FF2 File Offset: 0x000441F2
		// (set) Token: 0x060015A7 RID: 5543 RVA: 0x00045FFA File Offset: 0x000441FA
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

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x060015A8 RID: 5544 RVA: 0x0004601D File Offset: 0x0004421D
		// (set) Token: 0x060015A9 RID: 5545 RVA: 0x00046025 File Offset: 0x00044225
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

		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x060015AA RID: 5546 RVA: 0x00046050 File Offset: 0x00044250
		// (set) Token: 0x060015AB RID: 5547 RVA: 0x00046058 File Offset: 0x00044258
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

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x060015AC RID: 5548 RVA: 0x00046076 File Offset: 0x00044276
		// (set) Token: 0x060015AD RID: 5549 RVA: 0x0004607E File Offset: 0x0004427E
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

		// Token: 0x060015AE RID: 5550 RVA: 0x000460AD File Offset: 0x000442AD
		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x060015AF RID: 5551 RVA: 0x000460BC File Offset: 0x000442BC
		public void SetConfirmInputKey(HotKey hotkey)
		{
			this.ConfirmInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x060015B0 RID: 5552 RVA: 0x000460CB File Offset: 0x000442CB
		// (set) Token: 0x060015B1 RID: 5553 RVA: 0x000460D3 File Offset: 0x000442D3
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

		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x060015B2 RID: 5554 RVA: 0x000460F1 File Offset: 0x000442F1
		// (set) Token: 0x060015B3 RID: 5555 RVA: 0x000460F9 File Offset: 0x000442F9
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

		// Token: 0x04000A51 RID: 2641
		private readonly Action<bool> _onClose;

		// Token: 0x04000A52 RID: 2642
		private string _titleText;

		// Token: 0x04000A53 RID: 2643
		private string _explanationText;

		// Token: 0x04000A54 RID: 2644
		private string _cancelText;

		// Token: 0x04000A55 RID: 2645
		private string _acceptText;

		// Token: 0x04000A56 RID: 2646
		private float _initialValue;

		// Token: 0x04000A57 RID: 2647
		private float _value;

		// Token: 0x04000A58 RID: 2648
		private bool _visible;

		// Token: 0x04000A59 RID: 2649
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000A5A RID: 2650
		private InputKeyItemVM _confirmInputKey;
	}
}
