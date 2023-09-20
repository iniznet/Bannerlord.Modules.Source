using System;
using TaleWorlds.Core;
using TaleWorlds.Library.EventSystem;

namespace SandBox.ViewModelCollection.MapSiege
{
	// Token: 0x02000032 RID: 50
	public class PlayerStartEngineConstructionEvent : EventBase
	{
		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060003D6 RID: 982 RVA: 0x00011B1A File Offset: 0x0000FD1A
		// (set) Token: 0x060003D7 RID: 983 RVA: 0x00011B22 File Offset: 0x0000FD22
		public SiegeEngineType Engine { get; private set; }

		// Token: 0x060003D8 RID: 984 RVA: 0x00011B2B File Offset: 0x0000FD2B
		public PlayerStartEngineConstructionEvent(SiegeEngineType engine)
		{
			this.Engine = engine;
		}
	}
}
