using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000044 RID: 68
	public class MbEvent<T1, T2, T3> : IMbEvent<T1, T2, T3>, IMbEventBase
	{
		// Token: 0x06000741 RID: 1857 RVA: 0x000200D0 File Offset: 0x0001E2D0
		public void AddNonSerializedListener(object owner, Action<T1, T2, T3> action)
		{
			MbEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> eventHandlerRec = new MbEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3>(owner, action);
			MbEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x000200FA File Offset: 0x0001E2FA
		internal void Invoke(T1 t1, T2 t2, T3 t3)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2, t3);
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x0002010B File Offset: 0x0001E30B
		private void InvokeList(MbEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> list, T1 t1, T2 t2, T3 t3)
		{
			while (list != null)
			{
				list.Action(t1, t2, t3);
				list = list.Next;
			}
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x00020129 File Offset: 0x0001E329
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x00020138 File Offset: 0x0001E338
		private void ClearListenerOfList(ref MbEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> list, object o)
		{
			MbEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			MbEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> eventHandlerRec2 = list;
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

		// Token: 0x04000287 RID: 647
		private MbEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> _nonSerializedListenerList;

		// Token: 0x0200048F RID: 1167
		internal class EventHandlerRec<TS, TQ, TR>
		{
			// Token: 0x17000D61 RID: 3425
			// (get) Token: 0x06004040 RID: 16448 RVA: 0x001312B5 File Offset: 0x0012F4B5
			// (set) Token: 0x06004041 RID: 16449 RVA: 0x001312BD File Offset: 0x0012F4BD
			internal Action<TS, TQ, TR> Action { get; private set; }

			// Token: 0x17000D62 RID: 3426
			// (get) Token: 0x06004042 RID: 16450 RVA: 0x001312C6 File Offset: 0x0012F4C6
			// (set) Token: 0x06004043 RID: 16451 RVA: 0x001312CE File Offset: 0x0012F4CE
			internal object Owner { get; private set; }

			// Token: 0x06004044 RID: 16452 RVA: 0x001312D7 File Offset: 0x0012F4D7
			public EventHandlerRec(object owner, Action<TS, TQ, TR> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013CE RID: 5070
			public MbEvent<T1, T2, T3>.EventHandlerRec<TS, TQ, TR> Next;
		}
	}
}
