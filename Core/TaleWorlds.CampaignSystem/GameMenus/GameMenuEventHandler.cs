using System;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	// Token: 0x020000E4 RID: 228
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class GameMenuEventHandler : Attribute
	{
		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x060013BE RID: 5054 RVA: 0x000579F1 File Offset: 0x00055BF1
		// (set) Token: 0x060013BF RID: 5055 RVA: 0x000579F9 File Offset: 0x00055BF9
		public string MenuId { get; private set; }

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x00057A02 File Offset: 0x00055C02
		// (set) Token: 0x060013C1 RID: 5057 RVA: 0x00057A0A File Offset: 0x00055C0A
		public string MenuOptionId { get; private set; }

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x060013C2 RID: 5058 RVA: 0x00057A13 File Offset: 0x00055C13
		// (set) Token: 0x060013C3 RID: 5059 RVA: 0x00057A1B File Offset: 0x00055C1B
		public GameMenuEventHandler.EventType Type { get; private set; }

		// Token: 0x060013C4 RID: 5060 RVA: 0x00057A24 File Offset: 0x00055C24
		public GameMenuEventHandler(string menuId, string menuOptionId, GameMenuEventHandler.EventType type)
		{
			this.MenuId = menuId;
			this.MenuOptionId = menuOptionId;
			this.Type = type;
		}

		// Token: 0x020004F1 RID: 1265
		public enum EventType
		{
			// Token: 0x04001537 RID: 5431
			OnCondition,
			// Token: 0x04001538 RID: 5432
			OnConsequence
		}
	}
}
