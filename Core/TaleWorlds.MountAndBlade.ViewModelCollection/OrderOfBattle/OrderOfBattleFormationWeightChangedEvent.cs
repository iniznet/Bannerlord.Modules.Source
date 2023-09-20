using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x02000034 RID: 52
	public class OrderOfBattleFormationWeightChangedEvent : EventBase
	{
		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000450 RID: 1104 RVA: 0x00013D62 File Offset: 0x00011F62
		// (set) Token: 0x06000451 RID: 1105 RVA: 0x00013D6A File Offset: 0x00011F6A
		public Formation Formation { get; private set; }

		// Token: 0x06000452 RID: 1106 RVA: 0x00013D73 File Offset: 0x00011F73
		public OrderOfBattleFormationWeightChangedEvent(Formation formation)
		{
			this.Formation = formation;
		}
	}
}
