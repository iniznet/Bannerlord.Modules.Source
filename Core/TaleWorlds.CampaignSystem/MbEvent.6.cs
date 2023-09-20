using System;

namespace TaleWorlds.CampaignSystem
{
	public class MbEvent<T1, T2, T3, T4, T5> : IMbEvent<T1, T2, T3, T4, T5>, IMbEventBase
	{
		public void AddNonSerializedListener(object owner, Action<T1, T2, T3, T4, T5> action)
		{
			MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> eventHandlerRec = new MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5>(owner, action);
			MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		internal void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2, t3, t4, t5);
		}

		private void InvokeList(MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			while (list != null)
			{
				list.Action(t1, t2, t3, t4, t5);
				list = list.Next;
			}
		}

		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

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

		private MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<T1, T2, T3, T4, T5> _nonSerializedListenerList;

		internal class EventHandlerRec<TA, TB, TC, TD, TE>
		{
			internal Action<TA, TB, TC, TD, TE> Action { get; private set; }

			internal object Owner { get; private set; }

			public EventHandlerRec(object owner, Action<TA, TB, TC, TD, TE> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			public MbEvent<T1, T2, T3, T4, T5>.EventHandlerRec<TA, TB, TC, TD, TE> Next;
		}
	}
}
