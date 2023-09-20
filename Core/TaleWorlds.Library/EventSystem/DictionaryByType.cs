using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.EventSystem
{
	public class DictionaryByType
	{
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

		public List<Action<T>> Get<T>()
		{
			return (List<Action<T>>)this._eventsByType[typeof(T)];
		}

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

		public IDictionary<Type, object> GetClone()
		{
			return new Dictionary<Type, object>(this._eventsByType);
		}

		public void Clear()
		{
			this._eventsByType.Clear();
		}

		private readonly IDictionary<Type, object> _eventsByType = new Dictionary<Type, object>();
	}
}
