using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Inventory
{
	// Token: 0x020000D1 RID: 209
	public abstract class InventoryListener
	{
		// Token: 0x060012AE RID: 4782
		public abstract int GetGold();

		// Token: 0x060012AF RID: 4783
		public abstract TextObject GetTraderName();

		// Token: 0x060012B0 RID: 4784
		public abstract void SetGold(int gold);

		// Token: 0x060012B1 RID: 4785
		public abstract PartyBase GetOppositeParty();

		// Token: 0x060012B2 RID: 4786
		public abstract void OnTransaction();
	}
}
