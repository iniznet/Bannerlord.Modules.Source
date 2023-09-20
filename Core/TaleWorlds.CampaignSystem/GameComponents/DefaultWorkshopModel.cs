using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200014F RID: 335
	public class DefaultWorkshopModel : WorkshopModel
	{
		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x0600181E RID: 6174 RVA: 0x0007A933 File Offset: 0x00078B33
		public override int MaxWorkshopLevel
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x0600181F RID: 6175 RVA: 0x0007A936 File Offset: 0x00078B36
		public override int DaysForPlayerSaveWorkshopFromBankruptcy
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x0007A939 File Offset: 0x00078B39
		public override int GetInitialCapital(int level)
		{
			return 10000;
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x0007A940 File Offset: 0x00078B40
		public override int GetDailyExpense(int level)
		{
			return 100;
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x0007A944 File Offset: 0x00078B44
		public override float GetPolicyEffectToProduction(Town town)
		{
			float num = 1f;
			if (town.Settlement.OwnerClan.Kingdom != null)
			{
				if (town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.ForgivenessOfDebts))
				{
					num -= 0.05f;
				}
				if (town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.StateMonopolies))
				{
					num -= 0.1f;
				}
			}
			return num;
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x0007A9BC File Offset: 0x00078BBC
		public override int GetUpgradeCost(int currentLevel)
		{
			return 5000;
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x0007A9C3 File Offset: 0x00078BC3
		public override int GetMaxWorkshopCountForTier(int tier)
		{
			return 1 + tier;
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x0007A9C8 File Offset: 0x00078BC8
		public override int GetBuyingCostForPlayer(Workshop workshop)
		{
			return workshop.WorkshopType.EquipmentCost + (int)workshop.Settlement.Prosperity * 3 + this.GetInitialCapital(workshop.Level);
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x0007A9F1 File Offset: 0x00078BF1
		public override int GetSellingCost(Workshop workshop)
		{
			return (workshop.WorkshopType.EquipmentCost + (int)workshop.Settlement.Prosperity / 2 + workshop.Capital) / 2;
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x0007AA18 File Offset: 0x00078C18
		public override Hero SelectNextOwnerForWorkshop(Town town, Workshop workshop, Hero excludedHero, int requiredGold = 0)
		{
			Hero hero = null;
			int num = int.MaxValue;
			float num2 = 0f;
			foreach (Hero hero2 in town.Settlement.Notables)
			{
				if (hero2 != excludedHero && hero2.Gold >= requiredGold)
				{
					int count = hero2.OwnedWorkshops.Count;
					float num3 = hero2.Power / (float)count;
					if (num3 > num2 || (MathF.Abs(num3 - num2) < 1E-45f && count < num))
					{
						hero = hero2;
						num = count;
					}
				}
			}
			return hero;
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x0007AAC8 File Offset: 0x00078CC8
		public override int GetConvertProductionCost(WorkshopType workshopType)
		{
			return 2000;
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x0007AAD0 File Offset: 0x00078CD0
		public override bool CanPlayerSellWorkshop(Workshop workshop, out TextObject explanation)
		{
			int sellingCost = Campaign.Current.Models.WorkshopModel.GetSellingCost(workshop);
			Hero hero = Campaign.Current.Models.WorkshopModel.SelectNextOwnerForWorkshop(workshop.Settlement.Town, workshop, workshop.Owner, sellingCost);
			explanation = ((hero == null) ? new TextObject("{=oqPf2Gdp}There isn't any prospective buyer in the town.", null) : TextObject.Empty);
			return hero != null;
		}
	}
}
