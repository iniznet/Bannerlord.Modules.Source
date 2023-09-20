using System;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors
{
	// Token: 0x020003F8 RID: 1016
	public class GoldBarterBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003D06 RID: 15622 RVA: 0x00122588 File Offset: 0x00120788
		public override void RegisterEvents()
		{
			CampaignEvents.BarterablesRequested.AddNonSerializedListener(this, new Action<BarterData>(this.CheckForBarters));
		}

		// Token: 0x06003D07 RID: 15623 RVA: 0x001225A1 File Offset: 0x001207A1
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003D08 RID: 15624 RVA: 0x001225A4 File Offset: 0x001207A4
		public void CheckForBarters(BarterData args)
		{
			if ((args.OffererHero != null && args.OtherHero != null && args.OffererHero.Clan != args.OtherHero.Clan) || (args.OffererHero == null && args.OffererParty != null) || (args.OtherHero == null && args.OtherParty != null))
			{
				int num = ((args.OffererHero != null) ? args.OffererHero.Gold : args.OffererParty.MobileParty.PartyTradeGold);
				int num2 = ((args.OtherHero != null) ? args.OtherHero.Gold : args.OtherParty.MobileParty.PartyTradeGold);
				Barterable barterable = new GoldBarterable(args.OffererHero, args.OtherHero, args.OffererParty, args.OtherParty, num);
				args.AddBarterable<GoldBarterGroup>(barterable, false);
				Barterable barterable2 = new GoldBarterable(args.OtherHero, args.OffererHero, args.OtherParty, args.OffererParty, num2);
				args.AddBarterable<GoldBarterGroup>(barterable2, false);
			}
		}
	}
}
