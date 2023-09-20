using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Options;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000F7 RID: 247
	public class GroupedOptionCategoryVM : ViewModel
	{
		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x060015CF RID: 5583 RVA: 0x0004633E File Offset: 0x0004453E
		public IEnumerable<GenericOptionDataVM> AllOptions
		{
			get
			{
				return this.BaseOptions.Concat(this.Groups.SelectMany((OptionGroupVM g) => g.Options));
			}
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x00046375 File Offset: 0x00044575
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

		// Token: 0x060015D1 RID: 5585 RVA: 0x000463B0 File Offset: 0x000445B0
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

		// Token: 0x060015D2 RID: 5586 RVA: 0x00046440 File Offset: 0x00044640
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

		// Token: 0x060015D3 RID: 5587 RVA: 0x00046538 File Offset: 0x00044738
		internal IEnumerable<IOptionData> GetManagedOptions()
		{
			List<IOptionData> managedOptions = new List<IOptionData>();
			this.Groups.ApplyActionOnAllItems(delegate(OptionGroupVM g)
			{
				managedOptions.AppendList(g.GetManagedOptions());
			});
			return managedOptions.AsReadOnly();
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x00046578 File Offset: 0x00044778
		internal void InitializeDependentConfigs(Action<IOptionData, float> updateDependentConfigs)
		{
			this.Groups.ApplyActionOnAllItems(delegate(OptionGroupVM g)
			{
				g.InitializeDependentConfigs(updateDependentConfigs);
			});
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x000465AC File Offset: 0x000447AC
		internal bool IsChanged()
		{
			if (!this.BaseOptions.Any((GenericOptionDataVM b) => b.IsChanged()))
			{
				return this.Groups.Any((OptionGroupVM g) => g.IsChanged());
			}
			return true;
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x00046614 File Offset: 0x00044814
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

		// Token: 0x060015D7 RID: 5591 RVA: 0x00046678 File Offset: 0x00044878
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

		// Token: 0x060015D8 RID: 5592 RVA: 0x00046710 File Offset: 0x00044910
		public void ExecuteResetToDefault()
		{
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=oZc8oEAP}Reset this category to default", null).ToString(), new TextObject("{=CCBcdzGa}This will reset ALL options of this category to their default states. You won't be able to undo this action. {newline} {newline}Are you sure?", null).ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), new Action(this.ResetToDefault), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x060015D9 RID: 5593 RVA: 0x00046788 File Offset: 0x00044988
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

		// Token: 0x060015DA RID: 5594 RVA: 0x000467EC File Offset: 0x000449EC
		public GenericOptionDataVM GetOption(ManagedOptions.ManagedOptionsType optionType)
		{
			return this.AllOptions.FirstOrDefault((GenericOptionDataVM o) => !o.IsNative && (ManagedOptions.ManagedOptionsType)o.GetOptionType() == optionType);
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x00046820 File Offset: 0x00044A20
		public GenericOptionDataVM GetOption(NativeOptions.NativeOptionsType optionType)
		{
			return this.AllOptions.FirstOrDefault((GenericOptionDataVM o) => o.IsNative && (NativeOptions.NativeOptionsType)o.GetOptionType() == optionType);
		}

		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x060015DC RID: 5596 RVA: 0x00046851 File Offset: 0x00044A51
		// (set) Token: 0x060015DD RID: 5597 RVA: 0x00046859 File Offset: 0x00044A59
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

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x060015DE RID: 5598 RVA: 0x00046877 File Offset: 0x00044A77
		// (set) Token: 0x060015DF RID: 5599 RVA: 0x0004687F File Offset: 0x00044A7F
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

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x060015E0 RID: 5600 RVA: 0x0004689D File Offset: 0x00044A9D
		// (set) Token: 0x060015E1 RID: 5601 RVA: 0x000468A5 File Offset: 0x00044AA5
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

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x060015E2 RID: 5602 RVA: 0x000468C8 File Offset: 0x00044AC8
		// (set) Token: 0x060015E3 RID: 5603 RVA: 0x000468D0 File Offset: 0x00044AD0
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

		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x060015E4 RID: 5604 RVA: 0x000468F3 File Offset: 0x00044AF3
		// (set) Token: 0x060015E5 RID: 5605 RVA: 0x000468FB File Offset: 0x00044AFB
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

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x060015E6 RID: 5606 RVA: 0x00046919 File Offset: 0x00044B19
		// (set) Token: 0x060015E7 RID: 5607 RVA: 0x00046921 File Offset: 0x00044B21
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

		// Token: 0x04000A65 RID: 2661
		private readonly OptionCategory _category;

		// Token: 0x04000A66 RID: 2662
		private readonly TextObject _nameTextObject;

		// Token: 0x04000A67 RID: 2663
		protected readonly OptionsVM _options;

		// Token: 0x04000A68 RID: 2664
		private bool _isEnabled;

		// Token: 0x04000A69 RID: 2665
		private bool _isResetSupported;

		// Token: 0x04000A6A RID: 2666
		private string _name;

		// Token: 0x04000A6B RID: 2667
		private string _resetText;

		// Token: 0x04000A6C RID: 2668
		private MBBindingList<GenericOptionDataVM> _baseOptions;

		// Token: 0x04000A6D RID: 2669
		private MBBindingList<OptionGroupVM> _groups;
	}
}
