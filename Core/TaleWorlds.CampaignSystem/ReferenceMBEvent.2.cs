using System;

namespace TaleWorlds.CampaignSystem
{
	public class ReferenceMBEvent<T1, T2, T3> : ReferenceIMBEvent<T1, T2, T3>, IMbEventBase
	{
		public void AddNonSerializedListener(object owner, ReferenceAction<T1, T2, T3> action)
		{
			ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> eventHandlerRec = new ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3>(owner, action);
			ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		internal void Invoke(T1 t1, T2 t2, ref T3 t3)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2, ref t3);
		}

		private void InvokeList(ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> list, T1 t1, T2 t2, ref T3 t3)
		{
			while (list != null)
			{
				list.Action(t1, t2, ref t3);
				list = list.Next;
			}
		}

		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

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

		private ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<T1, T2, T3> _nonSerializedListenerList;

		internal class EventHandlerRec<TS, TQ, TR>
		{
			internal ReferenceAction<TS, TQ, TR> Action { get; private set; }

			internal object Owner { get; private set; }

			public EventHandlerRec(object owner, ReferenceAction<TS, TQ, TR> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			public ReferenceMBEvent<T1, T2, T3>.EventHandlerRec<TS, TQ, TR> Next;
		}
	}
}
