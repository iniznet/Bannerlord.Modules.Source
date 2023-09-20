using System;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200003C RID: 60
	public class MbEvent<T> : IMbEvent<T>, IMbEventBase
	{
		// Token: 0x06000725 RID: 1829 RVA: 0x0001FDA4 File Offset: 0x0001DFA4
		public void AddNonSerializedListener(object owner, Action<T> action)
		{
			MbEvent<T>.EventHandlerRec<T> eventHandlerRec = new MbEvent<T>.EventHandlerRec<T>(owner, action);
			MbEvent<T>.EventHandlerRec<T> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x0001FDCE File Offset: 0x0001DFCE
		public void Invoke(T t)
		{
			this.InvokeList(this._nonSerializedListenerList, t);
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x0001FDDD File Offset: 0x0001DFDD
		private void InvokeList(MbEvent<T>.EventHandlerRec<T> list, T t)
		{
			while (list != null)
			{
				list.Action(t);
				list = list.Next;
			}
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0001FDF8 File Offset: 0x0001DFF8
		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0001FE08 File Offset: 0x0001E008
		private void ClearListenerOfList(ref MbEvent<T>.EventHandlerRec<T> list, object o)
		{
			MbEvent<T>.EventHandlerRec<T> eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			MbEvent<T>.EventHandlerRec<T> eventHandlerRec2 = list;
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

		// Token: 0x04000283 RID: 643
		private MbEvent<T>.EventHandlerRec<T> _nonSerializedListenerList;

		// Token: 0x0200048B RID: 1163
		internal class EventHandlerRec<TS>
		{
			// Token: 0x17000D59 RID: 3417
			// (get) Token: 0x0600402C RID: 16428 RVA: 0x001311D5 File Offset: 0x0012F3D5
			// (set) Token: 0x0600402D RID: 16429 RVA: 0x001311DD File Offset: 0x0012F3DD
			internal Action<TS> Action { get; private set; }

			// Token: 0x17000D5A RID: 3418
			// (get) Token: 0x0600402E RID: 16430 RVA: 0x001311E6 File Offset: 0x0012F3E6
			// (set) Token: 0x0600402F RID: 16431 RVA: 0x001311EE File Offset: 0x0012F3EE
			internal object Owner { get; private set; }

			// Token: 0x06004030 RID: 16432 RVA: 0x001311F7 File Offset: 0x0012F3F7
			public EventHandlerRec(object owner, Action<TS> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			// Token: 0x040013C2 RID: 5058
			public MbEvent<T>.EventHandlerRec<TS> Next;
		}
	}
}
