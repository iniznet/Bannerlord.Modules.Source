using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.EventSystem
{
	// Token: 0x020000AF RID: 175
	public class DictionaryByType
	{
		// Token: 0x0600064C RID: 1612 RVA: 0x00013874 File Offset: 0x00011A74
		public void Add<T>(Action<T> value)
		{
			object obj;
			if (!this._eventsByType.TryGetValue(typeof(T), out obj))
			{
				obj = new List<Action<T>>();
				this._eventsByType[typeof(T)] = obj;
			}
			((List<Action<T>>)obj).Add(value);
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x000138C4 File Offset: 0x00011AC4
		public void Remove<T>(Action<T> value)
		{
			object obj;
			if (this._eventsByType.TryGetValue(typeof(T), out obj))
			{
				List<Action<T>> list = (List<Action<T>>)obj;
				list.Remove(value);
				this._eventsByType[typeof(T)] = list;
				return;
			}
			Debug.FailedAssert("Event: " + typeof(T).Name + " were not registered in the first place", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\EventSystem\\EventManager.cs", "Remove", 106);
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00013940 File Offset: 0x00011B40
		public void InvokeActions<T>(T item)
		{
			object obj;
			if (this._eventsByType.TryGetValue(typeof(T), out obj))
			{
				foreach (Action<T> action in ((List<Action<T>>)obj))
				{
					action(item);
				}
			}
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x000139AC File Offset: 0x00011BAC
		public List<Action<T>> Get<T>()
		{
			return (List<Action<T>>)this._eventsByType[typeof(T)];
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x000139C8 File Offset: 0x00011BC8
		public bool TryGet<T>(out List<Action<T>> value)
		{
			object obj;
			if (this._eventsByType.TryGetValue(typeof(T), out obj))
			{
				value = (List<Action<T>>)obj;
				return true;
			}
			value = null;
			return false;
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x000139FC File Offset: 0x00011BFC
		public IDictionary<Type, object> GetClone()
		{
			return new Dictionary<Type, object>(this._eventsByType);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00013A09 File Offset: 0x00011C09
		public void Clear()
		{
			this._eventsByType.Clear();
		}

		// Token: 0x040001E6 RID: 486
		private readonly IDictionary<Type, object> _eventsByType = new Dictionary<Type, object>();
	}
}
