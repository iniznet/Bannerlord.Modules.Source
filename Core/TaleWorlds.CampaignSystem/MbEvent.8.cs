using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200004C RID: 76
	public class MbEvent<T1, T2, T3, T4, T5, T6, T7> : IMbEvent<T1, T2, T3, T4, T5, T6, T7>, IMbEventBase
	{
		// Token: 0x0600075D RID: 1885 RVA: 0x00020418 File Offset: 0x0001E618
		public void AddNonSerializedListener(object owner, Action<T1, T2, T3, T4, T5, T6, T7> action)
		{
			MbEvent<T1, T2, T3, T4, T5, T6, T7>.EventHandlerRec<T1, T2, T3, T4, T5, T6, T7> eventHandlerRec = new MbEvent<T1, T2, T3, T4, T5, T6, T7>.EventHandlerRec<T1, T2, T3, T4, T5, T6, T7>(owner, action);
			MbEvent<T1, T2, T3, T4, T5, T6, T7>.EventHandlerRec<T1, T2, T3, T4, T5, T6, T7> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x00020444 File Offset: 0x0001E644
		internal void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2, t3, t4, t5, t6, t7);
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x00020468 File Offset: 0x0001E668
		private void InvokeList(MbEvent<T1, T2, T3, T4, T5, T6, T7>.EventHandlerRec<T1, T2, T3, T4, T5, T6, T7> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
		{
			while (list != null)
			{
				list.Action(t1, t2, t3, t4, t5, t6, t7);
				list = list.Next;
			}
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x0002048E File Offset: 0x0001E68E
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x000204A0 File Offset: 0x0001E6A0
		private void ClearListenerOfList(ref MbEvent<T1, T2, T3, T4, T5, T6, T7>.EventHandlerRec<T1, T2, T3, T4, T5, T6, T7> list, object o)
		{
			MbEvent<T1, T2, T3, T4, T5, T6, T7>.EventHandlerRec<T1, T2, T3, T4, T5, T6, T7> eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			MbEvent<T1, T2, T3, T4, T5, T6, T7>.EventHandlerRec<T1, T2, T3, T4, T5, T6, T7> eventHandlerRec2 = list;
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

		// Token: 0x0400028B RID: 651
		private MbEvent<T1, T2, T3, T4, T5, T6, T7>.EventHandlerRec<T1, T2, T3, T4, T5, T6, T7> _nonSerializedListenerList;

		// Token: 0x02000493 RID: 1171
		internal class EventHandlerRec<TA, TB, TC, TD, TE, TF, TG>
		{
			// Token: 0x17000D69 RID: 3433
			// (get) Token: 0x06004054 RID: 16468 RVA: 0x00131395 File Offset: 0x0012F595
			// (set) Token: 0x06004055 RID: 16469 RVA: 0x0013139D File Offset: 0x0012F59D
			internal Action<TA, TB, TC, TD, TE, TF, TG> Action { get; private set; }

			// Token: 0x17000D6A RID: 3434
			// (get) Token: 0x06004056 RID: 16470 RVA: 0x001313A6 File Offset: 0x0012F5A6
			// (set) Token: 0x06004057 RID: 16471 RVA: 0x001313AE File Offset: 0x0012F5AE
			internal object Owner { get; private set; }

			// Token: 0x06004058 RID: 16472 RVA: 0x001313B7 File Offset: 0x0012F5B7
			public EventHandlerRec(object owner, Action<TA, TB, TC, TD, TE, TF, TG> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013DA RID: 5082
			public MbEvent<T1, T2, T3, T4, T5, T6, T7>.EventHandlerRec<TA, TB, TC, TD, TE, TF, TG> Next;
		}
	}
}
