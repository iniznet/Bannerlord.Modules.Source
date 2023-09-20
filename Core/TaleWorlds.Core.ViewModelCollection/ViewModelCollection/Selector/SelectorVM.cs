using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Selector
{
	public class SelectorVM<T> : ViewModel where T : SelectorItemVM
	{
		public SelectorVM(int selectedIndex, Action<SelectorVM<T>> onChange)
		{
			this.ItemList = new MBBindingList<T>();
			this.HasSingleItem = true;
			this._onChange = onChange;
		}

		public SelectorVM(IEnumerable<string> list, int selectedIndex, Action<SelectorVM<T>> onChange)
		{
			this.ItemList = new MBBindingList<T>();
			this.Refresh(list, selectedIndex, onChange);
		}

		public SelectorVM(IEnumerable<TextObject> list, int selectedIndex, Action<SelectorVM<T>> onChange)
		{
			this.ItemList = new MBBindingList<T>();
			this.Refresh(list, selectedIndex, onChange);
		}

		public void Refresh(IEnumerable<string> list, int selectedIndex, Action<SelectorVM<T>> onChange)
		{
			this.ItemList.Clear();
			this._selectedIndex = -1;
			foreach (string text in list)
			{
				T t = (T)((object)Activator.CreateInstance(typeof(T), new object[] { text }));
				this.ItemList.Add(t);
			}
			this.HasSingleItem = this.ItemList.Count <= 1;
			this._onChange = onChange;
			this.SelectedIndex = selectedIndex;
		}

		public void Refresh(IEnumerable<TextObject> list, int selectedIndex, Action<SelectorVM<T>> onChange)
		{
			this.ItemList.Clear();
			this._selectedIndex = -1;
			foreach (TextObject textObject in list)
			{
				T t = (T)((object)Activator.CreateInstance(typeof(T), new object[] { textObject }));
				this.ItemList.Add(t);
			}
			this.HasSingleItem = this.ItemList.Count <= 1;
			this._onChange = onChange;
			this.SelectedIndex = selectedIndex;
		}

		public void Refresh(IEnumerable<T> list, int selectedIndex, Action<SelectorVM<T>> onChange)
		{
			this.ItemList.Clear();
			this._selectedIndex = -1;
			foreach (T t in list)
			{
				this.ItemList.Add(t);
			}
			this.HasSingleItem = this.ItemList.Count <= 1;
			this._onChange = onChange;
			this.SelectedIndex = selectedIndex;
		}

		public void SetOnChangeAction(Action<SelectorVM<T>> onChange)
		{
			this._onChange = onChange;
		}

		public void AddItem(T item)
		{
			this.ItemList.Add(item);
			this.HasSingleItem = this.ItemList.Count <= 1;
		}

		public void ExecuteRandomize()
		{
			MBBindingList<T> itemList = this.ItemList;
			T t;
			if (itemList == null)
			{
				t = default(T);
			}
			else
			{
				t = itemList.GetRandomElementWithPredicate((T i) => i.CanBeSelected);
			}
			T t2 = t;
			if (t2 != null)
			{
				this.SelectedIndex = this.ItemList.IndexOf(t2);
			}
		}

		public void ExecuteSelectNextItem()
		{
			MBBindingList<T> itemList = this.ItemList;
			if (itemList != null && itemList.Count > 0)
			{
				for (int num = (this.SelectedIndex + 1) % this.ItemList.Count; num != this.SelectedIndex; num = (num + 1) % this.ItemList.Count)
				{
					if (this.ItemList[num].CanBeSelected)
					{
						this.SelectedIndex = num;
						return;
					}
				}
			}
		}

		public void ExecuteSelectPreviousItem()
		{
			MBBindingList<T> itemList = this.ItemList;
			if (itemList != null && itemList.Count > 0)
			{
				for (int num = ((this.SelectedIndex - 1 >= 0) ? (this.SelectedIndex - 1) : (this.ItemList.Count - 1)); num != this.SelectedIndex; num = ((num - 1 >= 0) ? (num - 1) : (this.ItemList.Count - 1)))
				{
					if (this.ItemList[num].CanBeSelected)
					{
						this.SelectedIndex = num;
						return;
					}
				}
			}
		}

		public T GetCurrentItem()
		{
			MBBindingList<T> itemList = this._itemList;
			if (itemList != null && itemList.Count > 0 && this.SelectedIndex >= 0 && this.SelectedIndex < this._itemList.Count)
			{
				return this._itemList[this.SelectedIndex];
			}
			return default(T);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this._itemList.ApplyActionOnAllItems(delegate(T x)
			{
				x.RefreshValues();
			});
		}

		[DataSourceProperty]
		public MBBindingList<T> ItemList
		{
			get
			{
				return this._itemList;
			}
			set
			{
				if (value != this._itemList)
				{
					this._itemList = value;
					base.OnPropertyChangedWithValue<MBBindingList<T>>(value, "ItemList");
				}
			}
		}

		[DataSourceProperty]
		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
			set
			{
				if (value != this._selectedIndex)
				{
					this._selectedIndex = value;
					base.OnPropertyChangedWithValue(value, "SelectedIndex");
					if (this.SelectedItem != null)
					{
						this.SelectedItem.IsSelected = false;
					}
					this.SelectedItem = this.GetCurrentItem();
					if (this.SelectedItem != null)
					{
						this.SelectedItem.IsSelected = true;
					}
					Action<SelectorVM<T>> onChange = this._onChange;
					if (onChange == null)
					{
						return;
					}
					onChange(this);
				}
			}
		}

		[DataSourceProperty]
		public T SelectedItem
		{
			get
			{
				return this._selectedItem;
			}
			set
			{
				if (value != this._selectedItem)
				{
					this._selectedItem = value;
					base.OnPropertyChangedWithValue<T>(value, "SelectedItem");
				}
			}
		}

		[DataSourceProperty]
		public bool HasSingleItem
		{
			get
			{
				return this._hasSingleItem;
			}
			set
			{
				if (value != this._hasSingleItem)
				{
					this._hasSingleItem = value;
					base.OnPropertyChangedWithValue(value, "HasSingleItem");
				}
			}
		}

		private Action<SelectorVM<T>> _onChange;

		private MBBindingList<T> _itemList;

		private int _selectedIndex = -1;

		private T _selectedItem;

		private bool _hasSingleItem;
	}
}
