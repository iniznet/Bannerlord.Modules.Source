using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Options;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	public class GroupedOptionCategoryVM : ViewModel
	{
		public IEnumerable<GenericOptionDataVM> AllOptions
		{
			get
			{
				return this.BaseOptions.Concat(this.Groups.SelectMany((OptionGroupVM g) => g.Options));
			}
		}

		public GroupedOptionCategoryVM(OptionsVM options, TextObject name, OptionCategory category, bool isEnabled, bool isResetSupported = false)
		{
			this._category = category;
			this._nameTextObject = name;
			this._options = options;
			this.IsEnabled = isEnabled;
			this.IsResetSupported = isResetSupported;
			this.InitializeOptions();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameTextObject.ToString();
			this.BaseOptions.ApplyActionOnAllItems(delegate(GenericOptionDataVM b)
			{
				b.RefreshValues();
			});
			this.Groups.ApplyActionOnAllItems(delegate(OptionGroupVM g)
			{
				g.RefreshValues();
			});
			this.ResetText = new TextObject("{=RVIKFCno}Reset to Defaults", null).ToString();
		}

		private void InitializeOptions()
		{
			this.BaseOptions = new MBBindingList<GenericOptionDataVM>();
			this.Groups = new MBBindingList<OptionGroupVM>();
			if (this._category == null)
			{
				return;
			}
			if (this._category.Groups != null)
			{
				foreach (OptionGroup optionGroup in this._category.Groups)
				{
					this.Groups.Add(new OptionGroupVM(optionGroup.GroupName, this._options, optionGroup.Options));
				}
			}
			if (this._category.BaseOptions != null)
			{
				foreach (IOptionData optionData in this._category.BaseOptions)
				{
					this.BaseOptions.Add(this._options.GetOptionItem(optionData));
				}
			}
		}

		internal IEnumerable<IOptionData> GetManagedOptions()
		{
			List<IOptionData> managedOptions = new List<IOptionData>();
			this.Groups.ApplyActionOnAllItems(delegate(OptionGroupVM g)
			{
				managedOptions.AppendList(g.GetManagedOptions());
			});
			return managedOptions.AsReadOnly();
		}

		internal void InitializeDependentConfigs(Action<IOptionData, float> updateDependentConfigs)
		{
			this.Groups.ApplyActionOnAllItems(delegate(OptionGroupVM g)
			{
				g.InitializeDependentConfigs(updateDependentConfigs);
			});
		}

		internal bool IsChanged()
		{
			if (!this.BaseOptions.Any((GenericOptionDataVM b) => b.IsChanged()))
			{
				return this.Groups.Any((OptionGroupVM g) => g.IsChanged());
			}
			return true;
		}

		internal void Cancel()
		{
			this.BaseOptions.ApplyActionOnAllItems(delegate(GenericOptionDataVM b)
			{
				b.Cancel();
			});
			this.Groups.ApplyActionOnAllItems(delegate(OptionGroupVM g)
			{
				g.Cancel();
			});
		}

		public void ResetData()
		{
			this.BaseOptions.ApplyActionOnAllItems(delegate(GenericOptionDataVM b)
			{
				b.ResetData();
			});
			foreach (OptionGroupVM optionGroupVM in this.Groups)
			{
				optionGroupVM.Options.ApplyActionOnAllItems(delegate(GenericOptionDataVM o)
				{
					o.ResetData();
				});
			}
		}

		public void ExecuteResetToDefault()
		{
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=oZc8oEAP}Reset this category to default", null).ToString(), new TextObject("{=CCBcdzGa}This will reset ALL options of this category to their default states. You won't be able to undo this action. {newline} {newline}Are you sure?", null).ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), new Action(this.ResetToDefault), null, "", 0f, null, null, null), false, false);
		}

		private void ResetToDefault()
		{
			this.BaseOptions.ApplyActionOnAllItems(delegate(GenericOptionDataVM b)
			{
				b.ResetToDefault();
			});
			this.Groups.ApplyActionOnAllItems(delegate(OptionGroupVM g)
			{
				g.Options.ApplyActionOnAllItems(delegate(GenericOptionDataVM o)
				{
					o.ResetToDefault();
				});
			});
		}

		public GenericOptionDataVM GetOption(ManagedOptions.ManagedOptionsType optionType)
		{
			return this.AllOptions.FirstOrDefault((GenericOptionDataVM o) => !o.IsNative && (ManagedOptions.ManagedOptionsType)o.GetOptionType() == optionType);
		}

		public GenericOptionDataVM GetOption(NativeOptions.NativeOptionsType optionType)
		{
			return this.AllOptions.FirstOrDefault((GenericOptionDataVM o) => o.IsNative && (NativeOptions.NativeOptionsType)o.GetOptionType() == optionType);
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
		public bool IsResetSupported
		{
			get
			{
				return this._isResetSupported;
			}
			set
			{
				if (value != this._isResetSupported)
				{
					this._isResetSupported = value;
					base.OnPropertyChangedWithValue(value, "IsResetSupported");
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
		public string ResetText
		{
			get
			{
				return this._resetText;
			}
			set
			{
				if (value != this._resetText)
				{
					this._resetText = value;
					base.OnPropertyChangedWithValue<string>(value, "ResetText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<OptionGroupVM> Groups
		{
			get
			{
				return this._groups;
			}
			set
			{
				if (value != this._groups)
				{
					this._groups = value;
					base.OnPropertyChangedWithValue<MBBindingList<OptionGroupVM>>(value, "Groups");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<GenericOptionDataVM> BaseOptions
		{
			get
			{
				return this._baseOptions;
			}
			set
			{
				if (value != this._baseOptions)
				{
					this._baseOptions = value;
					base.OnPropertyChangedWithValue<MBBindingList<GenericOptionDataVM>>(value, "BaseOptions");
				}
			}
		}

		private readonly OptionCategory _category;

		private readonly TextObject _nameTextObject;

		protected readonly OptionsVM _options;

		private bool _isEnabled;

		private bool _isResetSupported;

		private string _name;

		private string _resetText;

		private MBBindingList<GenericOptionDataVM> _baseOptions;

		private MBBindingList<OptionGroupVM> _groups;
	}
}
