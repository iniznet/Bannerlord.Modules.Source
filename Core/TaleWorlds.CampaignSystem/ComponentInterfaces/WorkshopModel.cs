using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001C4 RID: 452
	public abstract class WorkshopModel : GameModel
	{
		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x06001B4D RID: 6989
		public abstract int MaxWorkshopLevel { get; }

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x06001B4E RID: 6990
		public abstract int DaysForPlayerSaveWorkshopFromBankruptcy { get; }

		// Token: 0x06001B4F RID: 6991
		public abstract int GetInitialCapital(int level);

		// Token: 0x06001B50 RID: 6992
		public abstract int GetDailyExpense(int level);

		// Token: 0x06001B51 RID: 6993
		public abstract float GetPolicyEffectToProduction(Town town);

		// Token: 0x06001B52 RID: 6994
		public abstract int GetUpgradeCost(int currentLevel);

		// Token: 0x06001B53 RID: 6995
		public abstract int GetMaxWorkshopCountForTier(int tier);

		// Token: 0x06001B54 RID: 6996
		public abstract int GetBuyingCostForPlayer(Workshop workshop);

		// Token: 0x06001B55 RID: 6997
		public abstract int GetSellingCost(Workshop workshop);

		// Token: 0x06001B56 RID: 6998
		public abstract Hero SelectNextOwnerForWorkshop(Town town, Workshop workshop, Hero excludedHero, int requiredGold = 0);

		// Token: 0x06001B57 RID: 6999
		public abstract int GetConvertProductionCost(WorkshopType workshopType);

		// Token: 0x06001B58 RID: 7000
		public abstract bool CanPlayerSellWorkshop(Workshop workshop, out TextObject explanation);
	}
}
