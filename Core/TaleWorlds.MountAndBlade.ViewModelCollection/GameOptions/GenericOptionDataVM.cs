using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	public abstract class GenericOptionDataVM : ViewModel
	{
		public bool IsNative
		{
			get
			{
				return this.Option.IsNative();
			}
		}

		public bool IsAction
		{
			get
			{
				return this.Option.IsAction();
			}
		}

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

		public virtual void UpdateData(bool initUpdate)
		{
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameObj.ToString();
			this.Description = this._descriptionObj.ToString();
		}

		public object GetOptionType()
		{
			return this.Option.GetOptionType();
		}

		public IOptionData GetOptionData()
		{
			return this.Option;
		}

		public void ResetToDefault()
		{
			this.SetValue(this.Option.GetDefaultValue());
		}

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

		public abstract void UpdateValue();

		public abstract void Cancel();

		public abstract bool IsChanged();

		public abstract void SetValue(float value);

		public abstract void ResetData();

		public abstract void ApplyValue();

		private TextObject _nameObj;

		private TextObject _descriptionObj;

		protected OptionsVM _optionsVM;

		protected IOptionData Option;

		private string _description;

		private string _name;

		private int _optionTypeId = -1;

		private string[] _imageIDs;

		private bool _isEnabled = true;

		private HintViewModel _hint;
	}
}
