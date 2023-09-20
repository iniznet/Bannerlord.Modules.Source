using System;
using TaleWorlds.Library.EventSystem;

namespace SandBox.View.Map
{
	// Token: 0x02000052 RID: 82
	public class MainMapCameraMoveEvent : EventBase
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000360 RID: 864 RVA: 0x0001CEC7 File Offset: 0x0001B0C7
		// (set) Token: 0x06000361 RID: 865 RVA: 0x0001CECF File Offset: 0x0001B0CF
		public bool RotationChanged { get; private set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000362 RID: 866 RVA: 0x0001CED8 File Offset: 0x0001B0D8
		// (set) Token: 0x06000363 RID: 867 RVA: 0x0001CEE0 File Offset: 0x0001B0E0
		public bool PositionChanged { get; private set; }

		// Token: 0x06000364 RID: 868 RVA: 0x0001CEE9 File Offset: 0x0001B0E9
		public MainMapCameraMoveEvent(bool rotationChanged, bool positionChanged)
		{
			this.RotationChanged = rotationChanged;
			this.PositionChanged = positionChanged;
		}
	}
}
