using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.EventSystem
{
	// Token: 0x020000AE RID: 174
	public class EventManager
	{
		// Token: 0x06000646 RID: 1606 RVA: 0x000137B9 File Offset: 0x000119B9
		public EventManager()
		{
			this._eventsByType = new DictionaryByType();
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000137CC File Offset: 0x000119CC
		public void RegisterEvent<T>(Action<T> eventObjType)
		{
			if (typeof(T).IsSubclassOf(typeof(EventBase)))
			{
				this._eventsByType.Add<T>(eventObjType);
				return;
			}
			Debug.FailedAssert("Events have to derived from EventSystemBase", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\EventSystem\\EventManager.cs", "RegisterEvent", 31);
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0001380C File Offset: 0x00011A0C
		public void UnregisterEvent<T>(Action<T> eventObjType)
		{
			if (typeof(T).IsSubclassOf(typeof(EventBase)))
			{
				this._eventsByType.Remove<T>(eventObjType);
				return;
			}
			Debug.FailedAssert("Events have to derived from EventSystemBase", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\EventSystem\\EventManager.cs", "UnregisterEvent", 48);
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0001384C File Offset: 0x00011A4C
		public void TriggerEvent<T>(T eventObj)
		{
			this._eventsByType.InvokeActions<T>(eventObj);
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x0001385A File Offset: 0x00011A5A
		public void Clear()
		{
			this._eventsByType.Clear();
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00013867 File Offset: 0x00011A67
		public IDictionary<Type, object> GetCloneOfEventDictionary()
		{
			return this._eventsByType.GetClone();
		}

		// Token: 0x040001E5 RID: 485
		private readonly DictionaryByType _eventsByType;
	}
}
