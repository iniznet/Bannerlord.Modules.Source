using System;
using TaleWorlds.Library.EventSystem;

namespace SandBox.View.Map
{
	public class EventSystemMapTestEvent : EventBase
	{
		public int TestNum { get; private set; }

		public EventSystemMapTestEvent(int testNum)
		{
			this.TestNum = testNum;
		}
	}
}
