using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200004A RID: 74
	public class MbEvent<T1, T2, T3, T4, T5, T6> : IMbEvent<T1, T2, T3, T4, T5, T6>, IMbEventBase
	{
		// Token: 0x06000756 RID: 1878 RVA: 0x00020340 File Offset: 0x0001E540
		public void AddNonSerializedListener(object owner, Action<T1, T2, T3, T4, T5, T6> action)
		{
			MbEvent<T1, T2, T3, T4, T5, T6>.EventHandlerRec<T1, T2, T3, T4, T5, T6> eventHandlerRec = new MbEvent<T1, T2, T3, T4, T5, T6>.EventHandlerRec<T1, T2, T3, T4, T5, T6>(owner, action);
			MbEvent<T1, T2, T3, T4, T5, T6>.EventHandlerRec<T1, T2, T3, T4, T5, T6> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x0002036A File Offset: 0x0001E56A
		internal void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2, t3, t4, t5, t6);
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x00020381 File Offset: 0x0001E581
		private void InvokeList(MbEvent<T1, T2, T3, T4, T5, T6>.EventHandlerRec<T1, T2, T3, T4, T5, T6> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
		{
			while (list != null)
			{
				list.Action(t1, t2, t3, t4, t5, t6);
				list = list.Next;
			}
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x000203A5 File Offset: 0x0001E5A5
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x000203B4 File Offset: 0x0001E5B4
		private void ClearListenerOfList(ref MbEvent<T1, T2, T3, T4, T5, T6>.EventHandlerRec<T1, T2, T3, T4, T5, T6> list, object o)
		{
			MbEvent<T1, T2, T3, T4, T5, T6>.EventHandlerRec<T1, T2, T3, T4, T5, T6> eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			MbEvent<T1, T2, T3, T4, T5, T6>.EventHandlerRec<T1, T2, T3, T4, T5, T6> eventHandlerRec2 = list;
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

		// Token: 0x0400028A RID: 650
		private MbEvent<T1, T2, T3, T4, T5, T6>.EventHandlerRec<T1, T2, T3, T4, T5, T6> _nonSerializedListenerList;

		// Token: 0x02000492 RID: 1170
		internal class EventHandlerRec<TA, TB, TC, TD, TE, TF>
		{
			// Token: 0x17000D67 RID: 3431
			// (get) Token: 0x0600404F RID: 16463 RVA: 0x0013135D File Offset: 0x0012F55D
			// (set) Token: 0x06004050 RID: 16464 RVA: 0x00131365 File Offset: 0x0012F565
			internal Action<TA, TB, TC, TD, TE, TF> Action { get; private set; }

			// Token: 0x17000D68 RID: 3432
			// (get) Token: 0x06004051 RID: 16465 RVA: 0x0013136E File Offset: 0x0012F56E
			// (set) Token: 0x06004052 RID: 16466 RVA: 0x00131376 File Offset: 0x0012F576
			internal object Owner { get; private set; }

			// Token: 0x06004053 RID: 16467 RVA: 0x0013137F File Offset: 0x0012F57F
			public EventHandlerRec(object owner, Action<TA, TB, TC, TD, TE, TF> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013D7 RID: 5079
			public MbEvent<T1, T2, T3, T4, T5, T6>.EventHandlerRec<TA, TB, TC, TD, TE, TF> Next;
		}
	}
}
