using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaleWorlds.Library
{
	// Token: 0x02000061 RID: 97
	public class MBBindingList<T> : Collection<T>, IMBBindingList, IList, ICollection, IEnumerable
	{
		// Token: 0x06000301 RID: 769 RVA: 0x00009DEC File Offset: 0x00007FEC
		public MBBindingList()
			: base(new List<T>(64))
		{
			this._list = (List<T>)base.Items;
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06000302 RID: 770 RVA: 0x00009E0C File Offset: 0x0000800C
		// (remove) Token: 0x06000303 RID: 771 RVA: 0x00009E2D File Offset: 0x0000802D
		public event ListChangedEventHandler ListChanged
		{
			add
			{
				if (this._eventHandlers == null)
				{
					this._eventHandlers = new List<ListChangedEventHandler>();
				}
				this._eventHandlers.Add(value);
			}
			remove
			{
				if (this._eventHandlers != null)
				{
					this._eventHandlers.Remove(value);
				}
			}
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00009E44 File Offset: 0x00008044
		protected override void ClearItems()
		{
			base.ClearItems();
			this.FireListChanged(ListChangedType.Reset, -1);
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00009E54 File Offset: 0x00008054
		protected override void InsertItem(int index, T item)
		{
			base.InsertItem(index, item);
			this.FireListChanged(ListChangedType.ItemAdded, index);
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00009E66 File Offset: 0x00008066
		protected override void RemoveItem(int index)
		{
			this.FireListChanged(ListChangedType.ItemBeforeDeleted, index);
			base.RemoveItem(index);
			this.FireListChanged(ListChangedType.ItemDeleted, index);
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00009E7F File Offset: 0x0000807F
		protected override void SetItem(int index, T item)
		{
			base.SetItem(index, item);
			this.FireListChanged(ListChangedType.ItemChanged, index);
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00009E91 File Offset: 0x00008091
		private void FireListChanged(ListChangedType type, int index)
		{
			this.OnListChanged(new ListChangedEventArgs(type, index));
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00009EA0 File Offset: 0x000080A0
		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			if (this._eventHandlers != null)
			{
				foreach (ListChangedEventHandler listChangedEventHandler in this._eventHandlers)
				{
					listChangedEventHandler(this, e);
				}
			}
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00009EFC File Offset: 0x000080FC
		public void Sort()
		{
			this._list.Sort();
			this.FireListChanged(ListChangedType.Sorted, -1);
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00009F11 File Offset: 0x00008111
		public void Sort(IComparer<T> comparer)
		{
			if (!this.IsOrdered(comparer))
			{
				this._list.Sort(comparer);
				this.FireListChanged(ListChangedType.Sorted, -1);
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00009F30 File Offset: 0x00008130
		public bool IsOrdered(IComparer<T> comparer)
		{
			for (int i = 1; i < this._list.Count; i++)
			{
				if (comparer.Compare(this._list[i - 1], this._list[i]) == 1)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00009F7C File Offset: 0x0000817C
		public void ApplyActionOnAllItems(Action<T> action)
		{
			foreach (T t in this._list)
			{
				action(t);
			}
		}

		// Token: 0x04000101 RID: 257
		private readonly List<T> _list;

		// Token: 0x04000102 RID: 258
		private List<ListChangedEventHandler> _eventHandlers;
	}
}
