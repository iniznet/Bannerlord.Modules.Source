using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000042 RID: 66
	public class MbEvent<T1, T2> : IMbEvent<T1, T2>, IMbEventBase
	{
		// Token: 0x0600073A RID: 1850 RVA: 0x00020004 File Offset: 0x0001E204
		public void AddNonSerializedListener(object owner, Action<T1, T2> action)
		{
			MbEvent<T1, T2>.EventHandlerRec<T1, T2> eventHandlerRec = new MbEvent<T1, T2>.EventHandlerRec<T1, T2>(owner, action);
			MbEvent<T1, T2>.EventHandlerRec<T1, T2> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x0002002E File Offset: 0x0001E22E
		internal void Invoke(T1 t1, T2 t2)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2);
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x0002003E File Offset: 0x0001E23E
		private void InvokeList(MbEvent<T1, T2>.EventHandlerRec<T1, T2> list, T1 t1, T2 t2)
		{
			while (list != null)
			{
				list.Action(t1, t2);
				list = list.Next;
			}
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x0002005A File Offset: 0x0001E25A
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x0002006C File Offset: 0x0001E26C
		private void ClearListenerOfList(ref MbEvent<T1, T2>.EventHandlerRec<T1, T2> list, object o)
		{
			MbEvent<T1, T2>.EventHandlerRec<T1, T2> eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			MbEvent<T1, T2>.EventHandlerRec<T1, T2> eventHandlerRec2 = list;
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

		// Token: 0x04000286 RID: 646
		private MbEvent<T1, T2>.EventHandlerRec<T1, T2> _nonSerializedListenerList;

		// Token: 0x0200048E RID: 1166
		internal class EventHandlerRec<TS, TQ>
		{
			// Token: 0x17000D5F RID: 3423
			// (get) Token: 0x0600403B RID: 16443 RVA: 0x0013127D File Offset: 0x0012F47D
			// (set) Token: 0x0600403C RID: 16444 RVA: 0x00131285 File Offset: 0x0012F485
			internal Action<TS, TQ> Action { get; private set; }

			// Token: 0x17000D60 RID: 3424
			// (get) Token: 0x0600403D RID: 16445 RVA: 0x0013128E File Offset: 0x0012F48E
			// (set) Token: 0x0600403E RID: 16446 RVA: 0x00131296 File Offset: 0x0012F496
			internal object Owner { get; private set; }

			// Token: 0x0600403F RID: 16447 RVA: 0x0013129F File Offset: 0x0012F49F
			public EventHandlerRec(object owner, Action<TS, TQ> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013CB RID: 5067
			public MbEvent<T1, T2>.EventHandlerRec<TS, TQ> Next;
		}
	}
}
