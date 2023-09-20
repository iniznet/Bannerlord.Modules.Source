using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000032 RID: 50
	public class MbEvent : IMbEvent
	{
		// Token: 0x06000352 RID: 850 RVA: 0x000193CC File Offset: 0x000175CC
		public void AddNonSerializedListener(object owner, Action action)
		{
			MbEvent.EventHandlerRec eventHandlerRec = new MbEvent.EventHandlerRec(owner, action);
			MbEvent.EventHandlerRec nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x000193F6 File Offset: 0x000175F6
		public void Invoke()
		{
			this.InvokeList(this._nonSerializedListenerList);
		}

		// Token: 0x06000354 RID: 852 RVA: 0x00019404 File Offset: 0x00017604
		private void InvokeList(MbEvent.EventHandlerRec list)
		{
			while (list != null)
			{
				list.Action();
				list = list.Next;
			}
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0001941E File Offset: 0x0001761E
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00019430 File Offset: 0x00017630
		private void ClearListenerOfList(ref MbEvent.EventHandlerRec list, object o)
		{
			MbEvent.EventHandlerRec eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			MbEvent.EventHandlerRec eventHandlerRec2 = list;
			if (eventHandlerRec2 == eventHandlerRec)
			{
				list = eventHandlerRec2.Next;
				return;
			}
			while (eventHandlerRec2 != null)
			{
				if (eventHandlerRec2.Next == eventHandlerRec)
				{
					eventHandlerRec2.Next = eventHandlerRec.Next;
				}
				else
				{
					eventHandlerRec2 = eventHandlerRec2.Next;
				}
			}
		}

		// Token: 0x04000181 RID: 385
		private MbEvent.EventHandlerRec _nonSerializedListenerList;

		// Token: 0x02000487 RID: 1159
		internal class EventHandlerRec
		{
			// Token: 0x17000D55 RID: 3413
			// (get) Token: 0x06004004 RID: 16388 RVA: 0x00130E58 File Offset: 0x0012F058
			// (set) Token: 0x06004005 RID: 16389 RVA: 0x00130E60 File Offset: 0x0012F060
			internal Action Action { get; private set; }

			// Token: 0x17000D56 RID: 3414
			// (get) Token: 0x06004006 RID: 16390 RVA: 0x00130E69 File Offset: 0x0012F069
			// (set) Token: 0x06004007 RID: 16391 RVA: 0x00130E71 File Offset: 0x0012F071
			internal object Owner { get; private set; }

			// Token: 0x06004008 RID: 16392 RVA: 0x00130E7A File Offset: 0x0012F07A
			public EventHandlerRec(object owner, Action action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013A5 RID: 5029
			public MbEvent.EventHandlerRec Next;
		}
	}
}
