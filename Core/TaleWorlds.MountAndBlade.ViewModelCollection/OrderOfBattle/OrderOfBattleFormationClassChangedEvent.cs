using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x02000033 RID: 51
	public class OrderOfBattleFormationClassChangedEvent : EventBase
	{
		// Token: 0x17000144 RID: 324
		// (get) Token: 0x0600044D RID: 1101 RVA: 0x00013D42 File Offset: 0x00011F42
		// (set) Token: 0x0600044E RID: 1102 RVA: 0x00013D4A File Offset: 0x00011F4A
		public Formation Formation { get; private set; }

		// Token: 0x0600044F RID: 1103 RVA: 0x00013D53 File Offset: 0x00011F53
		public OrderOfBattleFormationClassChangedEvent(Formation formation)
		{
			this.Formation = formation;
		}
	}
}
