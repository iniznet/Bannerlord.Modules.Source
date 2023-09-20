using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaleWorlds.Library
{
	public class MBBindingList<T> : Collection<T>, IMBBindingList, IList, ICollection, IEnumerable
	{
		public MBBindingList()
			: base(new List<T>(64))
		{
			this._list = (List<T>)base.Items;
		}

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

		protected override void ClearItems()
		{
			base.ClearItems();
			this.FireListChanged(ListChangedType.Reset, -1);
		}

		protected override void InsertItem(int index, T item)
		{
			base.InsertItem(index, item);
			this.FireListChanged(ListChangedType.ItemAdded, index);
		}

		protected override void RemoveItem(int index)
		{
			this.FireListChanged(ListChangedType.ItemBeforeDeleted, index);
			base.RemoveItem(index);
			this.FireListChanged(ListChangedType.ItemDeleted, index);
		}

		protected override void SetItem(int index, T item)
		{
			base.SetItem(index, item);
			this.FireListChanged(ListChangedType.ItemChanged, index);
		}

		private void FireListChanged(ListChangedType type, int index)
		{
			this.OnListChanged(new ListChangedEventArgs(type, index));
		}

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

		public void Sort()
		{
			this._list.Sort();
			this.FireListChanged(ListChangedType.Sorted, -1);
		}

		public void Sort(IComparer<T> comparer)
		{
			if (!this.IsOrdered(comparer))
			{
				this._list.Sort(comparer);
				this.FireListChanged(ListChangedType.Sorted, -1);
			}
		}

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

		public void ApplyActionOnAllItems(Action<T> action)
		{
			foreach (T t in this._list)
			{
				action(t);
			}
		}

		private readonly List<T> _list;

		private List<ListChangedEventHandler> _eventHandlers;
	}
}
