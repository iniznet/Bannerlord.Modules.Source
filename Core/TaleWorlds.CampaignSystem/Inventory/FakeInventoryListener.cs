using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Inventory
{
	// Token: 0x020000D2 RID: 210
	public class FakeInventoryListener : InventoryListener
	{
		// Token: 0x060012B4 RID: 4788 RVA: 0x000548D4 File Offset: 0x00052AD4
		public override int GetGold()
		{
			return 0;
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x000548D7 File Offset: 0x00052AD7
		public override TextObject GetTraderName()
		{
			return TextObject.Empty;
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x000548DE File Offset: 0x00052ADE
		public override void SetGold(int gold)
		{
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x000548E0 File Offset: 0x00052AE0
		public override void OnTransaction()
		{
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x000548E2 File Offset: 0x00052AE2
		public override PartyBase GetOppositeParty()
		{
			return null;
		}
	}
}
