using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200003E RID: 62
	public class ReferenceMBEvent<T1, T2> : ReferenceIMBEvent<T1, T2>, IMbEventBase
	{
		// Token: 0x0600072C RID: 1836 RVA: 0x0001FE6C File Offset: 0x0001E06C
		public void AddNonSerializedListener(object owner, ReferenceAction<T1, T2> action)
		{
			ReferenceMBEvent<T1, T2>.EventHandlerRec<T1, T2> eventHandlerRec = new ReferenceMBEvent<T1, T2>.EventHandlerRec<T1, T2>(owner, action);
			ReferenceMBEvent<T1, T2>.EventHandlerRec<T1, T2> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x0001FE96 File Offset: 0x0001E096
		internal void Invoke(T1 t1, ref T2 t2)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, ref t2);
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x0001FEA6 File Offset: 0x0001E0A6
		private void InvokeList(ReferenceMBEvent<T1, T2>.EventHandlerRec<T1, T2> list, T1 t1, ref T2 t2)
		{
			while (list != null)
			{
				list.Action(t1, ref t2);
				list = list.Next;
			}
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x0001FEC2 File Offset: 0x0001E0C2
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x0001FED4 File Offset: 0x0001E0D4
		private void ClearListenerOfList(ref ReferenceMBEvent<T1, T2>.EventHandlerRec<T1, T2> list, object o)
		{
			ReferenceMBEvent<T1, T2>.EventHandlerRec<T1, T2> eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			ReferenceMBEvent<T1, T2>.EventHandlerRec<T1, T2> eventHandlerRec2 = list;
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

		// Token: 0x04000284 RID: 644
		private ReferenceMBEvent<T1, T2>.EventHandlerRec<T1, T2> _nonSerializedListenerList;

		// Token: 0x0200048C RID: 1164
		internal class EventHandlerRec<TS, TQ>
		{
			// Token: 0x17000D5B RID: 3419
			// (get) Token: 0x06004031 RID: 16433 RVA: 0x0013120D File Offset: 0x0012F40D
			// (set) Token: 0x06004032 RID: 16434 RVA: 0x00131215 File Offset: 0x0012F415
			internal ReferenceAction<TS, TQ> Action { get; private set; }

			// Token: 0x17000D5C RID: 3420
			// (get) Token: 0x06004033 RID: 16435 RVA: 0x0013121E File Offset: 0x0012F41E
			// (set) Token: 0x06004034 RID: 16436 RVA: 0x00131226 File Offset: 0x0012F426
			internal object Owner { get; private set; }

			// Token: 0x06004035 RID: 16437 RVA: 0x0013122F File Offset: 0x0012F42F
			public EventHandlerRec(object owner, ReferenceAction<TS, TQ> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013C5 RID: 5061
			public ReferenceMBEvent<T1, T2>.EventHandlerRec<TS, TQ> Next;
		}
	}
}
