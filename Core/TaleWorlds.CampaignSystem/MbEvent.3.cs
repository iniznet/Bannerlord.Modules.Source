using System;

namespace TaleWorlds.CampaignSystem
{
	public class MbEvent<T1, T2> : IMbEvent<T1, T2>, IMbEventBase
	{
		public void AddNonSerializedListener(object owner, Action<T1, T2> action)
		{
			MbEvent<T1, T2>.EventHandlerRec<T1, T2> eventHandlerRec = new MbEvent<T1, T2>.EventHandlerRec<T1, T2>(owner, action);
			MbEvent<T1, T2>.EventHandlerRec<T1, T2> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		internal void Invoke(T1 t1, T2 t2)
		{
			this.InvokeList(this._nonSerializedListenerList, t1, t2);
		}

		private void InvokeList(MbEvent<T1, T2>.EventHandlerRec<T1, T2> list, T1 t1, T2 t2)
		{
			while (list != null)
			{
				list.Action(t1, t2);
				list = list.Next;
			}
		}

		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

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

		private MbEvent<T1, T2>.EventHandlerRec<T1, T2> _nonSerializedListenerList;

		internal class EventHandlerRec<TS, TQ>
		{
			internal Action<TS, TQ> Action { get; private set; }

			internal object Owner { get; private set; }

			public EventHandlerRec(object owner, Action<TS, TQ> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			public MbEvent<T1, T2>.EventHandlerRec<TS, TQ> Next;
		}
	}
}
