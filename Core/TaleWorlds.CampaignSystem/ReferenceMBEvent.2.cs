using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000040 RID: 64
	public class ReferenceMBEvent<T1, T2, T3> : ReferenceIMBEvent<T1, T2, T3>, IMbEventBase
	{
		// Token: 0x06000733 RID: 1843 RVA: 0x0001FF38 File Offset: 0x0001E138
		public void AddNonSerializedListener(object owner, ReferenceAction<T1, T2, T3> action)
		{
			ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> eventHandlerRec = new ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3>(owner, action);
			ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x0001FF62 File Offset: 0x0001E162
		internal void Invoke(T1 t1, T2 t2, ref T3 t3)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2, ref t3);
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x0001FF73 File Offset: 0x0001E173
		private void InvokeList(ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> list, T1 t1, T2 t2, ref T3 t3)
		{
			while (list != null)
			{
				list.Action(t1, t2, ref t3);
				list = list.Next;
			}
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x0001FF91 File Offset: 0x0001E191
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x0001FFA0 File Offset: 0x0001E1A0
		private void ClearListenerOfList(ref ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> list, object o)
		{
			ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> eventHandlerRec2 = list;
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

		// Token: 0x04000285 RID: 645
		private ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> _nonSerializedListenerList;

		// Token: 0x0200048D RID: 1165
		internal class EventHandlerRec<TS, TQ, TR>
		{
			// Token: 0x17000D5D RID: 3421
			// (get) Token: 0x06004036 RID: 16438 RVA: 0x00131245 File Offset: 0x0012F445
			// (set) Token: 0x06004037 RID: 16439 RVA: 0x0013124D File Offset: 0x0012F44D
			internal ReferenceAction<TS, TQ, TR> Action { get; private set; }

			// Token: 0x17000D5E RID: 3422
			// (get) Token: 0x06004038 RID: 16440 RVA: 0x00131256 File Offset: 0x0012F456
			// (set) Token: 0x06004039 RID: 16441 RVA: 0x0013125E File Offset: 0x0012F45E
			internal object Owner { get; private set; }

			// Token: 0x0600403A RID: 16442 RVA: 0x00131267 File Offset: 0x0012F467
			public EventHandlerRec(object owner, ReferenceAction<TS, TQ, TR> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013C8 RID: 5064
			public ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<TS, TQ, TR> Next;
		}
	}
}
