using System;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors
{
	// Token: 0x020003F7 RID: 1015
	public class FiefBarterBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003D02 RID: 15618 RVA: 0x0012244B File Offset: 0x0012064B
		public override void RegisterEvents()
		{
			CampaignEvents.BarterablesRequested.AddNonSerializedListener(this, new Action<BarterData>(this.CheckForBarters));
		}

		// Token: 0x06003D03 RID: 15619 RVA: 0x00122464 File Offset: 0x00120664
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003D04 RID: 15620 RVA: 0x00122468 File Offset: 0x00120668
		public void CheckForBarters(BarterData args)
		{
			if (args.OffererHero != null && args.OtherHero != null && args.OffererHero.GetPerkValue(DefaultPerks.Trade.EverythingHasAPrice))
			{
				foreach (Settlement settlement in Settlement.All)
				{
					if (!settlement.IsVillage)
					{
						Clan ownerClan = settlement.OwnerClan;
						if (((ownerClan != null) ? ownerClan.Leader : null) == args.OffererHero && !args.OtherHero.Clan.IsUnderMercenaryService)
						{
							Barterable barterable = new FiefBarterable(settlement, args.OffererHero, args.OtherHero);
							args.AddBarterable<FiefBarterGroup>(barterable, false);
						}
						else
						{
							Clan ownerClan2 = settlement.OwnerClan;
							if (((ownerClan2 != null) ? ownerClan2.Leader : null) == args.OtherHero && !args.OffererHero.Clan.IsUnderMercenaryService)
							{
								Barterable barterable2 = new FiefBarterable(settlement, args.OtherHero, args.OffererHero);
								args.AddBarterable<FiefBarterGroup>(barterable2, false);
							}
						}
					}
				}
			}
		}
	}
}
