using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000F6 RID: 246
	public abstract class GenericOptionDataVM : ViewModel
	{
		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x060015B4 RID: 5556 RVA: 0x00046117 File Offset: 0x00044317
		public bool IsNative
		{
			get
			{
				return this.Option.IsNative();
			}
		}

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x060015B5 RID: 5557 RVA: 0x00046124 File Offset: 0x00044324
		public bool IsAction
		{
			get
			{
				return this.Option.IsAction();
			}
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x00046134 File Offset: 0x00044334
		protected GenericOptionDataVM(OptionsVM optionsVM, IOptionData option, TextObject name, TextObject description, OptionsVM.OptionsDataType typeID)
		{
			this._nameObj = name;
			this._descriptionObj = description;
			this._optionsVM = optionsVM;
			this.Option = option;
			this.OptionTypeID = (int)typeID;
			this.Hint = new HintViewModel();
			this.RefreshValues();
			this.UpdateEnableState();
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x00046191 File Offset: 0x00044391
		public virtual void UpdateData(bool initUpdate)
		{
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x00046193 File Offset: 0x00044393
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameObj.ToString();
			this.Description = this._descriptionObj.ToString();
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x000461BD File Offset: 0x000443BD
		public object GetOptionType()
		{
			return this.Option.GetOptionType();
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x000461CA File Offset: 0x000443CA
		public IOptionData GetOptionData()
		{
			return this.Option;
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x000461D2 File Offset: 0x000443D2
		public void ResetToDefault()
		{
			this.SetValue(this.Option.GetDefaultValue());
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x000461E8 File Offset: 0x000443E8
		public void UpdateEnableState()
		{
			ValueTuple<string, bool> isDisabledAndReasonID = this.Option.GetIsDisabledAndReasonID();
			if (!string.IsNullOrEmpty(isDisabledAndReasonID.Item1))
			{
				this.Hint.HintText = Module.CurrentModule.GlobalTextManager.FindText(isDisabledAndReasonID.Item1, null);
			}
			else
			{
				this.Hint.HintText = TextObject.Empty;
			}
			this.IsEnabled = !isDisabledAndReasonID.Item2;
		}

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x060015BD RID: 5565 RVA: 0x00046250 File Offset: 0x00044450
		// (set) Token: 0x060015BE RID: 5566 RVA: 0x00046258 File Offset: 0x00044458
		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x060015BF RID: 5567 RVA: 0x0004627B File Offset: 0x0004447B
		// (set) Token: 0x060015C0 RID: 5568 RVA: 0x00046283 File Offset: 0x00044483
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x060015C1 RID: 5569 RVA: 0x000462A6 File Offset: 0x000444A6
		// (set) Token: 0x060015C2 RID: 5570 RVA: 0x000462AE File Offset: 0x000444AE
		[DataSourceProperty]
		public string[] ImageIDs
		{
			get
			{
				return this._imageIDs;
			}
			set
			{
				if (value != this._imageIDs)
				{
					this._imageIDs = value;
					base.OnPropertyChangedWithValue<string[]>(value, "ImageIDs");
				}
			}
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x060015C3 RID: 5571 RVA: 0x000462CC File Offset: 0x000444CC
		// (set) Token: 0x060015C4 RID: 5572 RVA: 0x000462D4 File Offset: 0x000444D4
		[DataSourceProperty]
		public int OptionTypeID
		{
			get
			{
				return this._optionTypeId;
			}
			set
			{
				if (value != this._optionTypeId)
				{
					this._optionTypeId = value;
					base.OnPropertyChangedWithValue(value, "OptionTypeID");
				}
			}
		}

		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x060015C5 RID: 5573 RVA: 0x000462F2 File Offset: 0x000444F2
		// (set) Token: 0x060015C6 RID: 5574 RVA: 0x000462FA File Offset: 0x000444FA
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x060015C7 RID: 5575 RVA: 0x00046318 File Offset: 0x00044518
		// (set) Token: 0x060015C8 RID: 5576 RVA: 0x00046320 File Offset: 0x00044520
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x060015C9 RID: 5577
		public abstract void UpdateValue();

		// Token: 0x060015CA RID: 5578
		public abstract void Cancel();

		// Token: 0x060015CB RID: 5579
		public abstract bool IsChanged();

		// Token: 0x060015CC RID: 5580
		public abstract void SetValue(float value);

		// Token: 0x060015CD RID: 5581
		public abstract void ResetData();

		// Token: 0x060015CE RID: 5582
		public abstract void ApplyValue();

		// Token: 0x04000A5B RID: 2651
		private TextObject _nameObj;

		// Token: 0x04000A5C RID: 2652
		private TextObject _descriptionObj;

		// Token: 0x04000A5D RID: 2653
		protected OptionsVM _optionsVM;

		// Token: 0x04000A5E RID: 2654
		protected IOptionData Option;

		// Token: 0x04000A5F RID: 2655
		private string _description;

		// Token: 0x04000A60 RID: 2656
		private string _name;

		// Token: 0x04000A61 RID: 2657
		private int _optionTypeId = -1;

		// Token: 0x04000A62 RID: 2658
		private string[] _imageIDs;

		// Token: 0x04000A63 RID: 2659
		private bool _isEnabled = true;

		// Token: 0x04000A64 RID: 2660
		private HintViewModel _hint;
	}
}
