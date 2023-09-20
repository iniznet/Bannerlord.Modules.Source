using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.ClassLoadout
{
	public class AlternativeUsageItemOptionVM : SelectorItemVM
	{
		public AlternativeUsageItemOptionVM(string usageType, TextObject s, TextObject hint, SelectorVM<AlternativeUsageItemOptionVM> parentSelector, int index)
			: base(s, hint)
		{
			this.UsageType = usageType;
			this._index = index;
			this._parentSelector = parentSelector;
		}

		private void ExecuteSelection()
		{
			this._parentSelector.SelectedIndex = this._index;
		}

		[DataSourceProperty]
		public string UsageType
		{
			get
			{
				return this._usageType;
			}
			set
			{
				if (value != this._usageType)
				{
					this._usageType = value;
					base.OnPropertyChangedWithValue<string>(value, "UsageType");
				}
			}
		}

		private int _index;

		private SelectorVM<AlternativeUsageItemOptionVM> _parentSelector;

		private string _usageType;
	}
}
