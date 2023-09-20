using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x02000032 RID: 50
	public class OrderOfBattleHeroAssignedToFormationEvent : EventBase
	{
		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x00013D0A File Offset: 0x00011F0A
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x00013D12 File Offset: 0x00011F12
		public Agent AssignedHero { get; private set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x00013D1B File Offset: 0x00011F1B
		// (set) Token: 0x0600044B RID: 1099 RVA: 0x00013D23 File Offset: 0x00011F23
		public Formation AssignedFormation { get; private set; }

		// Token: 0x0600044C RID: 1100 RVA: 0x00013D2C File Offset: 0x00011F2C
		public OrderOfBattleHeroAssignedToFormationEvent(Agent assignedHero, Formation assignedFormation)
		{
			this.AssignedHero = assignedHero;
			this.AssignedFormation = assignedFormation;
		}
	}
}
