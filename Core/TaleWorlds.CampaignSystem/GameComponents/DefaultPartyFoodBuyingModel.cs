using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000121 RID: 289
	public class DefaultPartyFoodBuyingModel : PartyFoodBuyingModel
	{
		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x06001652 RID: 5714 RVA: 0x0006AE37 File Offset: 0x00069037
		public override float MinimumDaysFoodToLastWhileBuyingFoodFromTown
		{
			get
			{
				return 30f;
			}
		}

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x06001653 RID: 5715 RVA: 0x0006AE3E File Offset: 0x0006903E
		public override float MinimumDaysFoodToLastWhileBuyingFoodFromVillage
		{
			get
			{
				return 8f;
			}
		}

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x06001654 RID: 5716 RVA: 0x0006AE45 File Offset: 0x00069045
		public override float LowCostFoodPriceAverage
		{
			get
			{
				return 30f;
			}
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x0006AE4C File Offset: 0x0006904C
		public override void FindItemToBuy(MobileParty mobileParty, Settlement settlement, out ItemRosterElement itemElement, out float itemElementsPrice)
		{
			itemElement = ItemRosterElement.Invalid;
			itemElementsPrice = 0f;
			float num = 0f;
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			int num2 = -1;
			for (int i = 0; i < settlement.ItemRoster.Count; i++)
			{
				ItemRosterElement elementCopyAtIndex = settlement.ItemRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Amount > 0)
				{
					bool flag = elementCopyAtIndex.EquipmentElement.Item.HasHorseComponent && elementCopyAtIndex.EquipmentElement.Item.HorseComponent.IsLiveStock;
					if (elementCopyAtIndex.EquipmentElement.Item.IsFood || flag)
					{
						int itemPrice = settlementComponent.GetItemPrice(elementCopyAtIndex.EquipmentElement, mobileParty, false);
						int itemValue = elementCopyAtIndex.EquipmentElement.ItemValue;
						if ((itemPrice < 120 || flag) && mobileParty.LeaderHero.Gold >= itemPrice)
						{
							object obj = (flag ? ((120f - (float)(itemPrice / elementCopyAtIndex.EquipmentElement.Item.HorseComponent.MeatCount)) * 0.0083f) : ((float)(120 - itemPrice) * 0.0083f));
							float num3 = (flag ? ((100f - (float)(itemValue / elementCopyAtIndex.EquipmentElement.Item.HorseComponent.MeatCount)) * 0.01f) : ((float)(100 - itemValue) * 0.01f));
							object obj2 = obj;
							float num4 = obj2 * obj2 * num3 * num3;
							if (num4 > 0f)
							{
								if (MBRandom.RandomFloat * (num + num4) >= num)
								{
									num2 = i;
									itemElementsPrice = (float)itemPrice;
								}
								num += num4;
							}
						}
					}
				}
			}
			if (num2 != -1)
			{
				itemElement = settlement.ItemRoster.GetElementCopyAtIndex(num2);
			}
		}
	}
}
