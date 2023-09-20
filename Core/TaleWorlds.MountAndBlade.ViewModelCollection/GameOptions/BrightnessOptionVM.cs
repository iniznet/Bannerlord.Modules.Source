using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000F4 RID: 244
	public class BrightnessOptionVM : ViewModel
	{
		// Token: 0x0600157B RID: 5499 RVA: 0x000459BC File Offset: 0x00043BBC
		public BrightnessOptionVM(Action<bool> onClose = null)
		{
			this.TitleText = Module.CurrentModule.GlobalTextManager.FindText("str_brightness_option_title", null).ToString();
			TextObject textObject = Module.CurrentModule.GlobalTextManager.FindText("str_brightness_option_explainer", null);
			textObject.SetTextVariable("newline", "\n");
			this.ExplanationText = textObject.ToString();
			this._onClose = onClose;
			this.RefreshOptionValues();
			this.RefreshValues();
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x00045A38 File Offset: 0x00043C38
		private void RefreshOptionValues()
		{
			this.InitialValue = 50;
			this.InitialValue1 = NativeOptions.GetConfig(NativeOptions.NativeOptionsType.BrightnessMax);
			this.InitialValue2 = NativeOptions.GetConfig(NativeOptions.NativeOptionsType.BrightnessMin);
			if (NativeOptions.GetConfig(NativeOptions.NativeOptionsType.BrightnessCalibrated) < 2f)
			{
				this.Value1 = 0;
				this.Value2 = 0;
				return;
			}
			this.Value1 = MathF.Round((this.InitialValue1 - 1f) / 0.003f) - 2;
			this.Value2 = MathF.Round(this.InitialValue2 / 0.003f) + 2;
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x00045ABC File Offset: 0x00043CBC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CancelText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
			this.AcceptText = new TextObject("{=Y94H6XnK}Accept", null).ToString();
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x00045AF0 File Offset: 0x00043CF0
		public void ExecuteConfirm()
		{
			this.InitialValue = this.Value;
			NativeOptions.SetConfig(NativeOptions.NativeOptionsType.Brightness, (float)this.Value);
			float num = (float)(this.Value1 + 2) * 0.003f + 1f;
			float num2 = (float)(this.Value2 - 2) * 0.003f;
			this.InitialValue1 = num;
			this.InitialValue2 = num2;
			NativeOptions.SetConfig(NativeOptions.NativeOptionsType.BrightnessMax, num);
			NativeOptions.SetConfig(NativeOptions.NativeOptionsType.BrightnessMin, num2);
			NativeOptions.SetConfig(NativeOptions.NativeOptionsType.BrightnessCalibrated, 2f);
			Action<bool> onClose = this._onClose;
			if (onClose != null)
			{
				onClose(true);
			}
			this.Visible = false;
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x00045B80 File Offset: 0x00043D80
		public void ExecuteCancel()
		{
			this.Value = this.InitialValue;
			NativeOptions.SetConfig(NativeOptions.NativeOptionsType.Brightness, (float)this.InitialValue);
			NativeOptions.SetConfig(NativeOptions.NativeOptionsType.BrightnessMax, this.InitialValue1);
			NativeOptions.SetConfig(NativeOptions.NativeOptionsType.BrightnessMin, this.InitialValue2);
			this.Visible = false;
			Action<bool> onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose(false);
		}

		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x06001580 RID: 5504 RVA: 0x00045BD9 File Offset: 0x00043DD9
		// (set) Token: 0x06001581 RID: 5505 RVA: 0x00045BE1 File Offset: 0x00043DE1
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

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x06001582 RID: 5506 RVA: 0x00045C04 File Offset: 0x00043E04
		// (set) Token: 0x06001583 RID: 5507 RVA: 0x00045C0C File Offset: 0x00043E0C
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

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x06001584 RID: 5508 RVA: 0x00045C2F File Offset: 0x00043E2F
		// (set) Token: 0x06001585 RID: 5509 RVA: 0x00045C37 File Offset: 0x00043E37
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

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x06001586 RID: 5510 RVA: 0x00045C5A File Offset: 0x00043E5A
		// (set) Token: 0x06001587 RID: 5511 RVA: 0x00045C62 File Offset: 0x00043E62
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

		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06001588 RID: 5512 RVA: 0x00045C85 File Offset: 0x00043E85
		// (set) Token: 0x06001589 RID: 5513 RVA: 0x00045C8D File Offset: 0x00043E8D
		public int Value
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
				}
			}
		}

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x0600158A RID: 5514 RVA: 0x00045CAB File Offset: 0x00043EAB
		// (set) Token: 0x0600158B RID: 5515 RVA: 0x00045CB3 File Offset: 0x00043EB3
		public int InitialValue
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

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x0600158C RID: 5516 RVA: 0x00045CD1 File Offset: 0x00043ED1
		// (set) Token: 0x0600158D RID: 5517 RVA: 0x00045CD9 File Offset: 0x00043ED9
		public float InitialValue1
		{
			get
			{
				return this._initialValue1;
			}
			set
			{
				if (this._initialValue1 != value)
				{
					this._initialValue1 = value;
					base.OnPropertyChangedWithValue(value, "InitialValue1");
				}
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x0600158E RID: 5518 RVA: 0x00045CF7 File Offset: 0x00043EF7
		// (set) Token: 0x0600158F RID: 5519 RVA: 0x00045CFF File Offset: 0x00043EFF
		public float InitialValue2
		{
			get
			{
				return this._initialValue2;
			}
			set
			{
				if (this._initialValue2 != value)
				{
					this._initialValue2 = value;
					base.OnPropertyChangedWithValue(value, "InitialValue2");
				}
			}
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x06001590 RID: 5520 RVA: 0x00045D1D File Offset: 0x00043F1D
		// (set) Token: 0x06001591 RID: 5521 RVA: 0x00045D28 File Offset: 0x00043F28
		public int Value1
		{
			get
			{
				return this._value1;
			}
			set
			{
				if (this._value1 != value)
				{
					float num = (float)(value + 2) * 0.003f + 1f;
					NativeOptions.SetConfig(NativeOptions.NativeOptionsType.BrightnessMax, num);
					this._value1 = value;
					base.OnPropertyChangedWithValue(value, "Value1");
				}
			}
		}

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x06001592 RID: 5522 RVA: 0x00045D6A File Offset: 0x00043F6A
		// (set) Token: 0x06001593 RID: 5523 RVA: 0x00045D74 File Offset: 0x00043F74
		public int Value2
		{
			get
			{
				return this._value2;
			}
			set
			{
				if (this._value2 != value)
				{
					float num = (float)(value - 2) * 0.003f;
					NativeOptions.SetConfig(NativeOptions.NativeOptionsType.BrightnessMin, num);
					this._value2 = value;
					base.OnPropertyChangedWithValue(value, "Value2");
				}
			}
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x06001594 RID: 5524 RVA: 0x00045DB0 File Offset: 0x00043FB0
		// (set) Token: 0x06001595 RID: 5525 RVA: 0x00045DB8 File Offset: 0x00043FB8
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
						this.RefreshOptionValues();
					}
				}
			}
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x00045DDF File Offset: 0x00043FDF
		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x00045DEE File Offset: 0x00043FEE
		public void SetConfirmInputKey(HotKey hotkey)
		{
			this.ConfirmInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x06001598 RID: 5528 RVA: 0x00045DFD File Offset: 0x00043FFD
		// (set) Token: 0x06001599 RID: 5529 RVA: 0x00045E05 File Offset: 0x00044005
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

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x0600159A RID: 5530 RVA: 0x00045E23 File Offset: 0x00044023
		// (set) Token: 0x0600159B RID: 5531 RVA: 0x00045E2B File Offset: 0x0004402B
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

		// Token: 0x04000A43 RID: 2627
		private readonly Action<bool> _onClose;

		// Token: 0x04000A44 RID: 2628
		private string _titleText;

		// Token: 0x04000A45 RID: 2629
		private string _explanationText;

		// Token: 0x04000A46 RID: 2630
		private string _cancelText;

		// Token: 0x04000A47 RID: 2631
		private string _acceptText;

		// Token: 0x04000A48 RID: 2632
		private int _initialValue;

		// Token: 0x04000A49 RID: 2633
		private float _initialValue1;

		// Token: 0x04000A4A RID: 2634
		private float _initialValue2;

		// Token: 0x04000A4B RID: 2635
		private int _value;

		// Token: 0x04000A4C RID: 2636
		private int _value1;

		// Token: 0x04000A4D RID: 2637
		private int _value2;

		// Token: 0x04000A4E RID: 2638
		private bool _visible;

		// Token: 0x04000A4F RID: 2639
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000A50 RID: 2640
		private InputKeyItemVM _confirmInputKey;
	}
}
