using System;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors
{
	public class GoldBarterBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.BarterablesRequested.AddNonSerializedListener(this, new Action<BarterData>(this.CheckForBarters));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

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
