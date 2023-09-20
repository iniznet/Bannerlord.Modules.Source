using System;

namespace TaleWorlds.CampaignSystem
{
	public class MbEvent<T> : IMbEvent<T>, IMbEventBase
	{
		public void AddNonSerializedListener(object owner, Action<T> action)
		{
			MbEvent<T>.EventHandlerRec<T> eventHandlerRec = new MbEvent<T>.EventHandlerRec<T>(owner, action);
			MbEvent<T>.EventHandlerRec<T> nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		public void Invoke(T t)
		{
			this.InvokeList(this._nonSerializedListenerList, t);
		}

		private void InvokeList(MbEvent<T>.EventHandlerRec<T> list, T t)
		{
			while (list != null)
			{
				list.Action(t);
				list = list.Next;
			}
		}

		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

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

		private MbEvent<T>.EventHandlerRec<T> _nonSerializedListenerList;

		internal class EventHandlerRec<TS>
		{
			internal Action<TS> Action { get; private set; }

			internal object Owner { get; private set; }

			public EventHandlerRec(object owner, Action<TS> action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			public MbEvent<T>.EventHandlerRec<TS> Next;
		}
	}
}
