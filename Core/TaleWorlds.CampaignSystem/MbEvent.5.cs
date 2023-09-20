using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000046 RID: 70
	public class MbEvent<T1, T2, T3, T4> : IMbEvent<T1, T2, T3, T4>, IMbEventBase
	{
		// Token: 0x06000748 RID: 1864 RVA: 0x0002019C File Offset: 0x0001E39C
		public void AddNonSerializedListener(object owner, Action<T1, T2, T3, T4> action)
		{
			MbEvent<T1, T2, T3, T4>.EventHandlerRec<T1, T2, T3, T4> eventHandlerRec = new MbEvent<T1, T2, T3, T4>.EventHandlerRec<T1, T2, T3, T4>(owner, action);
			MbEvent<T1, T2, T3, T4>.EventHandlerRec<T1, T2, T3, T4> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x000201C6 File Offset: 0x0001E3C6
		internal void Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2, t3, t4);
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x000201D9 File Offset: 0x0001E3D9
		private void InvokeList(MbEvent<T1, T2, T3, T4>.EventHandlerRec<T1, T2, T3, T4> list, T1 t1, T2 t2, T3 t3, T4 t4)
		{
			while (list != null)
			{
				list.Action(t1, t2, t3, t4);
				list = list.Next;
			}
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x000201F9 File Offset: 0x0001E3F9
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00020208 File Offset: 0x0001E408
		private void ClearListenerOfList(ref MbEvent<T1, T2, T3, T4>.EventHandlerRec<T1, T2, T3, T4> list, object o)
		{
			MbEvent<T1, T2, T3, T4>.EventHandlerRec<T1, T2, T3, T4> eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			MbEvent<T1, T2, T3, T4>.EventHandlerRec<T1, T2, T3, T4> eventHandlerRec2 = list;
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

		// Token: 0x04000288 RID: 648
		private MbEvent<T1, T2, T3, T4>.EventHandlerRec<T1, T2, T3, T4> _nonSerializedListenerList;

		// Token: 0x02000490 RID: 1168
		internal class EventHandlerRec<TA, TB, TC, TD>
		{
			// Token: 0x17000D63 RID: 3427
			// (get) Token: 0x06004045 RID: 16453 RVA: 0x001312ED File Offset: 0x0012F4ED
			// (set) Token: 0x06004046 RID: 16454 RVA: 0x001312F5 File Offset: 0x0012F4F5
			internal Action<TA, TB, TC, TD> Action { get; private set; }

			// Token: 0x17000D64 RID: 3428
			// (get) Token: 0x06004047 RID: 16455 RVA: 0x001312FE File Offset: 0x0012F4FE
			// (set) Token: 0x06004048 RID: 16456 RVA: 0x00131306 File Offset: 0x0012F506
			internal object Owner { get; private set; }

			// Token: 0x06004049 RID: 16457 RVA: 0x0013130F File Offset: 0x0012F50F
			public EventHandlerRec(object owner, Action<TA, TB, TC, TD> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013D1 RID: 5073
			public MbEvent<T1, T2, T3, T4>.EventHandlerRec<TA, TB, TC, TD> Next;
		}
	}
}
