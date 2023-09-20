using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public class CampaignOptionItemVM : ViewModel
	{
		public ICampaignOptionData OptionData { get; private set; }

		public CampaignOptionItemVM(ICampaignOptionData optionData)
		{
			this.OptionData = optionData;
			this.OptionData.GetEnableState();
			this.Hint = new HintViewModel();
			this._dataType = this.OptionData.GetDataType();
			if (this._dataType == CampaignOptionDataType.Boolean)
			{
				this._optionDataAsBoolean = this.OptionData as BooleanCampaignOptionData;
				this.ValueAsBoolean = this._optionDataAsBoolean.GetValue() != 0f;
				this.OptionType = 0;
			}
			else if (this._dataType == CampaignOptionDataType.Numeric)
			{
				this._optionDataAsNumeric = this.OptionData as NumericCampaignOptionData;
				this.OptionType = 1;
				this.MinRange = this._optionDataAsNumeric.MinValue;
				this.MaxRange = this._optionDataAsNumeric.MaxValue;
				this.IsDiscrete = this._optionDataAsNumeric.IsDiscrete;
				this.ValueAsRange = this._optionDataAsNumeric.GetValue();
			}
			else if (this._dataType == CampaignOptionDataType.Selection)
			{
				this._optionDataAsSelection = this.OptionData as SelectionCampaignOptionData;
				List<TextObject> selections = this._optionDataAsSelection.Selections;
				int num = (int)this._optionDataAsSelection.GetValue();
				this.SelectionSelector = new SelectorVM<SelectorItemVM>(selections, num, new Action<SelectorVM<SelectorItemVM>>(this.OnSelectionOptionValueChanged));
				this.OptionType = 2;
				this.SelectionSelector.SelectedIndex = (int)this._optionDataAsSelection.GetValue();
			}
			else if (this._dataType == CampaignOptionDataType.Action)
			{
				this._optionDataAsAction = this.OptionData as ActionCampaignOptionData;
				this.HideOptionName = true;
				this.OptionType = 3;
			}
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.OptionData.GetName();
			this.RefreshDisabledStatus();
		}

		public void RefreshDisabledStatus()
		{
			string description = this.OptionData.GetDescription();
			TextObject textObject = new TextObject("{=!}" + description, null);
			CampaignOptionDisableStatus isDisabledWithReason = this.OptionData.GetIsDisabledWithReason();
			this.IsDisabled = isDisabledWithReason.IsDisabled;
			if (!string.IsNullOrEmpty(isDisabledWithReason.DisabledReason))
			{
				string text = textObject.ToString();
				string disabledReason = isDisabledWithReason.DisabledReason;
				textObject = GameTexts.FindText("str_string_newline_string", null).CopyTextObject();
				textObject.SetTextVariable("STR1", text);
				textObject.SetTextVariable("STR2", disabledReason);
			}
			if (this.IsDisabled && isDisabledWithReason.ValueIfDisabled != -1f)
			{
				this.SetValue(isDisabledWithReason.ValueIfDisabled);
			}
			this.Hint.HintText = textObject;
		}

		public void ExecuteAction()
		{
			ActionCampaignOptionData optionDataAsAction = this._optionDataAsAction;
			if (optionDataAsAction == null)
			{
				return;
			}
			optionDataAsAction.ExecuteAction();
		}

		public void OnSelectionOptionValueChanged(SelectorVM<SelectorItemVM> selector)
		{
			if (selector.SelectedIndex >= 0 && this._optionDataAsSelection != null)
			{
				this._optionDataAsSelection.SetValue((float)selector.SelectedIndex);
				Action<CampaignOptionItemVM> onValueChanged = this._onValueChanged;
				if (onValueChanged == null)
				{
					return;
				}
				onValueChanged(this);
			}
		}

		public void SetValue(float value)
		{
			if (this._dataType == CampaignOptionDataType.Boolean)
			{
				this.ValueAsBoolean = value != 0f;
				return;
			}
			if (this._dataType == CampaignOptionDataType.Numeric)
			{
				this.ValueAsRange = value;
				return;
			}
			if (this._dataType == CampaignOptionDataType.Selection)
			{
				this.SelectionSelector.SelectedIndex = (int)value;
			}
		}

		public void SetOnValueChangedCallback(Action<CampaignOptionItemVM> onValueChanged)
		{
			this._onValueChanged = onValueChanged;
		}

		[DataSourceProperty]
		public bool HideOptionName
		{
			get
			{
				return this._hideOptionName;
			}
			set
			{
				if (value != this._hideOptionName)
				{
					this._hideOptionName = value;
					base.OnPropertyChangedWithValue(value, "HideOptionName");
				}
			}
		}

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

		[DataSourceProperty]
		public int OptionType
		{
			get
			{
				return this._optionType;
			}
			set
			{
				if (value != this._optionType)
				{
					this._optionType = value;
					base.OnPropertyChangedWithValue(value, "OptionType");
				}
			}
		}

		[DataSourceProperty]
		public bool ValueAsBoolean
		{
			get
			{
				return this._valueAsBoolean;
			}
			set
			{
				if (value != this._valueAsBoolean)
				{
					this._optionDataAsBoolean.SetValue(value ? 1f : 0f);
					this._valueAsBoolean = value;
					base.OnPropertyChangedWithValue(value, "ValueAsBoolean");
					Action<CampaignOptionItemVM> onValueChanged = this._onValueChanged;
					if (onValueChanged == null)
					{
						return;
					}
					onValueChanged(this);
				}
			}
		}

		[DataSourceProperty]
		public bool IsDiscrete
		{
			get
			{
				return this._isDiscrete;
			}
			set
			{
				if (value != this._isDiscrete)
				{
					this._isDiscrete = value;
					base.OnPropertyChangedWithValue(value, "IsDiscrete");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		[DataSourceProperty]
		public float MinRange
		{
			get
			{
				return this._minRange;
			}
			set
			{
				if (value != this._minRange)
				{
					this._minRange = value;
					base.OnPropertyChangedWithValue(value, "MinRange");
				}
			}
		}

		[DataSourceProperty]
		public float MaxRange
		{
			get
			{
				return this._maxRange;
			}
			set
			{
				if (value != this._maxRange)
				{
					this._maxRange = value;
					base.OnPropertyChangedWithValue(value, "MaxRange");
				}
			}
		}

		[DataSourceProperty]
		public float ValueAsRange
		{
			get
			{
				return this._valueAsRange;
			}
			set
			{
				if (value != this._valueAsRange)
				{
					this._valueAsRange = value;
					this._optionDataAsNumeric.SetValue(value);
					base.OnPropertyChangedWithValue(value, "ValueAsRange");
					this.ValueAsString = value.ToString("F1");
					Action<CampaignOptionItemVM> onValueChanged = this._onValueChanged;
					if (onValueChanged == null)
					{
						return;
					}
					onValueChanged(this);
				}
			}
		}

		[DataSourceProperty]
		public string ValueAsString
		{
			get
			{
				return this._valueAsString;
			}
			set
			{
				if (value != this._valueAsString)
				{
					this._valueAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "ValueAsString");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> SelectionSelector
		{
			get
			{
				return this._selectionSelector;
			}
			set
			{
				if (value != this._selectionSelector)
				{
					this._selectionSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "SelectionSelector");
				}
			}
		}

		private ActionCampaignOptionData _optionDataAsAction;

		private BooleanCampaignOptionData _optionDataAsBoolean;

		private NumericCampaignOptionData _optionDataAsNumeric;

		private SelectionCampaignOptionData _optionDataAsSelection;

		private Action<CampaignOptionItemVM> _onValueChanged;

		private CampaignOptionDataType _dataType;

		private bool _hideOptionName;

		private int _optionType;

		private string _name;

		private HintViewModel _hint;

		private bool _isDiscrete;

		private bool _isDisabled;

		private float _minRange;

		private float _maxRange;

		private bool _valueAsBoolean;

		private float _valueAsRange;

		private string _valueAsString;

		private SelectorVM<SelectorItemVM> _selectionSelector;
	}
}
