using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.EventSystem
{
	public class EventManager
	{
		public EventManager()
		{
			this._eventsByType = new DictionaryByType();
		}

		public void RegisterEvent<T>(Action<T> eventObjType)
		{
			if (typeof(T).IsSubclassOf(typeof(EventBase)))
			{
				this._eventsByType.Add<T>(eventObjType);
				return;
			}
			Debug.FailedAssert("Events have to derived from EventSystemBase", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\EventSystem\\EventManager.cs", "RegisterEvent", 31);
		}

		public void UnregisterEvent<T>(Action<T> eventObjType)
		{
			if (typeof(T).IsSubclassOf(typeof(EventBase)))
			{
				this._eventsByType.Remove<T>(eventObjType);
				return;
			}
			Debug.FailedAssert("Events have to derived from EventSystemBase", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\EventSystem\\EventManager.cs", "UnregisterEvent", 48);
		}

		public void TriggerEvent<T>(T eventObj)
		{
			this._eventsByType.InvokeActions<T>(eventObj);
		}

		public void Clear()
		{
			this._eventsByType.Clear();
		}

		public IDictionary<Type, object> GetCloneOfEventDictionary()
		{
			return this._eventsByType.GetClone();
		}

		private readonly DictionaryByType _eventsByType;
	}
}
