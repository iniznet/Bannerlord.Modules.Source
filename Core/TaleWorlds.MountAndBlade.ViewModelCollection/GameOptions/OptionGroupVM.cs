using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000FB RID: 251
	public class OptionGroupVM : ViewModel
	{
		// Token: 0x06001619 RID: 5657 RVA: 0x00046E94 File Offset: 0x00045094
		public OptionGroupVM(TextObject groupName, OptionsVM optionsBase, IEnumerable<IOptionData> optionsList)
		{
			this._groupName = groupName;
			this.Options = new MBBindingList<GenericOptionDataVM>();
			foreach (IOptionData optionData in optionsList)
			{
				this.Options.Add(optionsBase.GetOptionItem(optionData));
			}
			this.RefreshValues();
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x00046F08 File Offset: 0x00045108
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._groupName.ToString();
			this.Options.ApplyActionOnAllItems(delegate(GenericOptionDataVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x00046F58 File Offset: 0x00045158
		internal List<IOptionData> GetManagedOptions()
		{
			List<IOptionData> list = new List<IOptionData>();
			foreach (GenericOptionDataVM genericOptionDataVM in this.Options)
			{
				if (!genericOptionDataVM.IsNative)
				{
					list.Add(genericOptionDataVM.GetOptionData());
				}
			}
			return list;
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x00046FBC File Offset: 0x000451BC
		internal bool IsChanged()
		{
			return this.Options.Any((GenericOptionDataVM o) => o.IsChanged());
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x00046FE8 File Offset: 0x000451E8
		internal void Cancel()
		{
			this.Options.ApplyActionOnAllItems(delegate(GenericOptionDataVM o)
			{
				o.Cancel();
			});
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x00047014 File Offset: 0x00045214
		internal void InitializeDependentConfigs(Action<IOptionData, float> updateDependentConfigs)
		{
			this.Options.ApplyActionOnAllItems(delegate(GenericOptionDataVM o)
			{
				updateDependentConfigs(o.GetOptionData(), o.GetOptionData().GetValue(false));
			});
		}

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x0600161F RID: 5663 RVA: 0x00047045 File Offset: 0x00045245
		// (set) Token: 0x06001620 RID: 5664 RVA: 0x0004704D File Offset: 0x0004524D
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

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x06001621 RID: 5665 RVA: 0x00047070 File Offset: 0x00045270
		// (set) Token: 0x06001622 RID: 5666 RVA: 0x00047078 File Offset: 0x00045278
		[DataSourceProperty]
		public MBBindingList<GenericOptionDataVM> Options
		{
			get
			{
				return this._options;
			}
			set
			{
				if (value != this._options)
				{
					this._options = value;
					base.OnPropertyChangedWithValue<MBBindingList<GenericOptionDataVM>>(value, "Options");
				}
			}
		}

		// Token: 0x04000A83 RID: 2691
		private readonly TextObject _groupName;

		// Token: 0x04000A84 RID: 2692
		private const string ControllerIdentificationModifier = "_controller";

		// Token: 0x04000A85 RID: 2693
		private string _name;

		// Token: 0x04000A86 RID: 2694
		private MBBindingList<GenericOptionDataVM> _options;
	}
}
