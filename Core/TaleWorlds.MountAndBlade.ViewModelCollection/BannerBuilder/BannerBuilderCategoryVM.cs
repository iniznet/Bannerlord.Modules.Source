using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.BannerBuilder
{
	public class BannerBuilderCategoryVM : ViewModel
	{
		public BannerBuilderCategoryVM(BannerIconGroup category, Action<BannerBuilderItemVM> onItemSelection)
		{
			this.ItemsList = new MBBindingList<BannerBuilderItemVM>();
			this._category = category;
			this._onItemSelection = onItemSelection;
			this.IsPattern = this._category.IsPattern;
			this.IsEnabled = true;
			this.PopulateItems();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Title = this._category.Name.ToString();
		}

		private void PopulateItems()
		{
			this.ItemsList.Clear();
			if (this.IsPattern)
			{
				for (int i = 0; i < this._category.AllBackgrounds.Count; i++)
				{
					KeyValuePair<int, string> keyValuePair = this._category.AllBackgrounds.ElementAt(i);
					this.ItemsList.Add(new BannerBuilderItemVM(keyValuePair.Key, keyValuePair.Value, this._onItemSelection));
				}
				return;
			}
			for (int j = 0; j < this._category.AllIcons.Count; j++)
			{
				KeyValuePair<int, BannerIconData> keyValuePair2 = this._category.AllIcons.ElementAt(j);
				this.ItemsList.Add(new BannerBuilderItemVM(keyValuePair2.Key, keyValuePair2.Value, this._onItemSelection));
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPattern
		{
			get
			{
				return this._isPattern;
			}
			set
			{
				if (value != this._isPattern)
				{
					this._isPattern = value;
					base.OnPropertyChangedWithValue(value, "IsPattern");
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
		public MBBindingList<BannerBuilderItemVM> ItemsList
		{
			get
			{
				return this._itemsList;
			}
			set
			{
				if (value != this._itemsList)
				{
					this._itemsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<BannerBuilderItemVM>>(value, "ItemsList");
				}
			}
		}

		private readonly BannerIconGroup _category;

		private readonly Action<BannerBuilderItemVM> _onItemSelection;

		private string _title;

		private bool _isPattern;

		private bool _isEnabled;

		private MBBindingList<BannerBuilderItemVM> _itemsList;
	}
}
