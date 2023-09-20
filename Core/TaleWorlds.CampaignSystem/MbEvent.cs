using System;

namespace TaleWorlds.CampaignSystem
{
	public class MbEvent : IMbEvent
	{
		public void AddNonSerializedListener(object owner, Action action)
		{
			MbEvent.EventHandlerRec eventHandlerRec = new MbEvent.EventHandlerRec(owner, action);
			MbEvent.EventHandlerRec nonSerializedListenerList = this._nonSerializedListenerList;
			this._nonSerializedListenerList = eventHandlerRec;
			eventHandlerRec.Next = nonSerializedListenerList;
		}

		public void Invoke()
		{
			this.InvokeList(this._nonSerializedListenerList);
		}

		private void InvokeList(MbEvent.EventHandlerRec list)
		{
			while (list != null)
			{
				list.Action();
				list = list.Next;
			}
		}

		public void ClearListeners(object o)
		{
			this.ClearListenerOfList(ref this._nonSerializedListenerList, o);
		}

		private void ClearListenerOfList(ref MbEvent.EventHandlerRec list, object o)
		{
			MbEvent.EventHandlerRec eventHandlerRec = list;
			while (eventHandlerRec != null && eventHandlerRec.Owner != o)
			{
				eventHandlerRec = eventHandlerRec.Next;
			}
			if (eventHandlerRec == null)
			{
				return;
			}
			MbEvent.EventHandlerRec eventHandlerRec2 = list;
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

		private MbEvent.EventHandlerRec _nonSerializedListenerList;

		internal class EventHandlerRec
		{
			internal Action Action { get; private set; }

			internal object Owner { get; private set; }

			public EventHandlerRec(object owner, Action action)
			{
				this.Action = action;
				this.Owner = owner;
			}

			public MbEvent.EventHandlerRec Next;
		}
	}
}
