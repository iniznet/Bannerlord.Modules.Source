using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Selector
{
	// Token: 0x02000012 RID: 18
	public class SelectorVM<T> : ViewModel where T : SelectorItemVM
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x000033E5 File Offset: 0x000015E5
		public SelectorVM(int selectedIndex, Action<SelectorVM<T>> onChange)
		{
			this.ItemList = new MBBindingList<T>();
			this.HasSingleItem = true;
			this._onChange = onChange;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000340D File Offset: 0x0000160D
		public SelectorVM(IEnumerable<string> list, int selectedIndex, Action<SelectorVM<T>> onChange)
		{
			this.ItemList = new MBBindingList<T>();
			this.Refresh(list, selectedIndex, onChange);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00003430 File Offset: 0x00001630
		public SelectorVM(IEnumerable<TextObject> list, int selectedIndex, Action<SelectorVM<T>> onChange)
		{
			this.ItemList = new MBBindingList<T>();
			this.Refresh(list, selectedIndex, onChange);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00003454 File Offset: 0x00001654
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

		// Token: 0x060000CB RID: 203 RVA: 0x000034F8 File Offset: 0x000016F8
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

		// Token: 0x060000CC RID: 204 RVA: 0x0000359C File Offset: 0x0000179C
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

		// Token: 0x060000CD RID: 205 RVA: 0x00003620 File Offset: 0x00001820
		public void SetOnChangeAction(Action<SelectorVM<T>> onChange)
		{
			this._onChange = onChange;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00003629 File Offset: 0x00001829
		public void AddItem(T item)
		{
			this.ItemList.Add(item);
			this.HasSingleItem = this.ItemList.Count <= 1;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00003650 File Offset: 0x00001850
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

		// Token: 0x060000D0 RID: 208 RVA: 0x000036B4 File Offset: 0x000018B4
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

		// Token: 0x060000D1 RID: 209 RVA: 0x00003728 File Offset: 0x00001928
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

		// Token: 0x060000D2 RID: 210 RVA: 0x000037B4 File Offset: 0x000019B4
		public T GetCurrentItem()
		{
			MBBindingList<T> itemList = this._itemList;
			if (itemList != null && itemList.Count > 0 && this.SelectedIndex >= 0 && this.SelectedIndex < this._itemList.Count)
			{
				return this._itemList[this.SelectedIndex];
			}
			return default(T);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000380F File Offset: 0x00001A0F
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._itemList.ApplyActionOnAllItems(delegate(T x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00003841 File Offset: 0x00001A41
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x00003849 File Offset: 0x00001A49
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00003867 File Offset: 0x00001A67
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00003870 File Offset: 0x00001A70
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x000038F2 File Offset: 0x00001AF2
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x000038FA File Offset: 0x00001AFA
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

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00003922 File Offset: 0x00001B22
		// (set) Token: 0x060000DB RID: 219 RVA: 0x0000392A File Offset: 0x00001B2A
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

		// Token: 0x0400004F RID: 79
		private Action<SelectorVM<T>> _onChange;

		// Token: 0x04000050 RID: 80
		private MBBindingList<T> _itemList;

		// Token: 0x04000051 RID: 81
		private int _selectedIndex = -1;

		// Token: 0x04000052 RID: 82
		private T _selectedItem;

		// Token: 0x04000053 RID: 83
		private bool _hasSingleItem;
	}
}
