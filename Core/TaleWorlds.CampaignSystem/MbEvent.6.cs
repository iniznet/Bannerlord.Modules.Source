using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000048 RID: 72
	public class MbEvent<T1, T2, T3, T4, T5> : IMbEvent<T1, T2, T3, T4, T5>, IMbEventBase
	{
		// Token: 0x0600074F RID: 1871 RVA: 0x0002026C File Offset: 0x0001E46C
		public void AddNonSerializedListener(object owner, Action<T1, T2, T3, T4, T5> action)
		{
			MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> eventHandlerRec = new MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5>(owner, action);
			MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00020296 File Offset: 0x0001E496
		internal void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2, t3, t4, t5);
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x000202AB File Offset: 0x0001E4AB
		private void InvokeList(MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			while (list != null)
			{
				list.Action(t1, t2, t3, t4, t5);
				list = list.Next;
			}
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x000202CD File Offset: 0x0001E4CD
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x000202DC File Offset: 0x0001E4DC
		private void ClearListenerOfList(ref MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> list, object o)
		{
			MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> eventHandlerRec2 = list;
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

		// Token: 0x04000289 RID: 649
		private MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> _nonSerializedListenerList;

		// Token: 0x02000491 RID: 1169
		internal class EventHandlerRec<TA, TB, TC, TD, TE>
		{
			// Token: 0x17000D65 RID: 3429
			// (get) Token: 0x0600404A RID: 16458 RVA: 0x00131325 File Offset: 0x0012F525
			// (set) Token: 0x0600404B RID: 16459 RVA: 0x0013132D File Offset: 0x0012F52D
			internal Action<TA, TB, TC, TD, TE> Action { get; private set; }

			// Token: 0x17000D66 RID: 3430
			// (get) Token: 0x0600404C RID: 16460 RVA: 0x00131336 File Offset: 0x0012F536
			// (set) Token: 0x0600404D RID: 16461 RVA: 0x0013133E File Offset: 0x0012F53E
			internal object Owner { get; private set; }

			// Token: 0x0600404E RID: 16462 RVA: 0x00131347 File Offset: 0x0012F547
			public EventHandlerRec(object owner, Action<TA, TB, TC, TD, TE> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013D4 RID: 5076
			public MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<TA, TB, TC, TD, TE> Next;
		}
	}
}
