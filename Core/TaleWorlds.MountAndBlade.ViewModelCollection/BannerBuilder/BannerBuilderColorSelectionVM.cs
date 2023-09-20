using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.BannerBuilder
{
	public class BannerBuilderColorSelectionVM : ViewModel
	{
		public BannerBuilderColorSelectionVM()
		{
			this.Items = new MBBindingList<BannerBuilderColorItemVM>();
			this.PopulateItems();
		}

		public void EnableWith(int selectedColorID, Action<BannerBuilderColorItemVM> onSelection)
		{
			this._onSelection = onSelection;
			this.Items.ApplyActionOnAllItems(delegate(BannerBuilderColorItemVM i)
			{
				i.IsSelected = i.ColorID == selectedColorID;
			});
			this.IsEnabled = true;
		}

		private void OnItemSelection(BannerBuilderColorItemVM item)
		{
			Action<BannerBuilderColorItemVM> onSelection = this._onSelection;
			if (onSelection != null)
			{
				onSelection(item);
			}
			this._onSelection = null;
			this.IsEnabled = false;
		}

		private void PopulateItems()
		{
			this.Items.Clear();
			MBReadOnlyDictionary<int, BannerColor> readOnlyColorPalette = BannerManager.Instance.ReadOnlyColorPalette;
			for (int i = 0; i < readOnlyColorPalette.Count; i++)
			{
				KeyValuePair<int, BannerColor> keyValuePair = readOnlyColorPalette.ElementAt(i);
				this.Items.Add(new BannerBuilderColorItemVM(new Action<BannerBuilderColorItemVM>(this.OnItemSelection), keyValuePair.Key, keyValuePair.Value));
			}
		}

		[DataSourceProperty]
		public MBBindingList<BannerBuilderColorItemVM> Items
		{
			get
			{
				return this._items;
			}
			set
			{
				if (value != this._items)
				{
					this._items = value;
					base.OnPropertyChangedWithValue<MBBindingList<BannerBuilderColorItemVM>>(value, "Items");
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

		private Action<BannerBuilderColorItemVM> _onSelection;

		private MBBindingList<BannerBuilderColorItemVM> _items;

		private bool _isEnabled;
	}
}
