using System;
using TaleWorlds.Library.EventSystem;

namespace SandBox.ViewModelCollection.Missions.NameMarker
{
	// Token: 0x02000027 RID: 39
	public class MissionNameMarkerToggleEvent : EventBase
	{
		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000324 RID: 804 RVA: 0x0000F8C3 File Offset: 0x0000DAC3
		// (set) Token: 0x06000325 RID: 805 RVA: 0x0000F8CB File Offset: 0x0000DACB
		public bool NewState { get; private set; }

		// Token: 0x06000326 RID: 806 RVA: 0x0000F8D4 File Offset: 0x0000DAD4
		public MissionNameMarkerToggleEvent(bool newState)
		{
			this.NewState = newState;
		}
	}
}
