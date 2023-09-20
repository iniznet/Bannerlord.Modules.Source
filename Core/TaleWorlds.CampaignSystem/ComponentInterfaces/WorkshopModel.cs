using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class WorkshopModel : GameModel
	{
		public abstract int MaxWorkshopLevel { get; }

		public abstract int DaysForPlayerSaveWorkshopFromBankruptcy { get; }

		public abstract int GetInitialCapital(int level);

		public abstract int GetDailyExpense(int level);

		public abstract float GetPolicyEffectToProduction(Town town);

		public abstract int GetUpgradeCost(int currentLevel);

		public abstract int GetMaxWorkshopCountForTier(int tier);

		public abstract int GetBuyingCostForPlayer(Workshop workshop);

		public abstract int GetSellingCost(Workshop workshop);

		public abstract Hero SelectNextOwnerForWorkshop(Town town, Workshop workshop, Hero excludedHero, int requiredGold = 0);

		public abstract int GetConvertProductionCost(WorkshopType workshopType);

		public abstract bool CanPlayerSellWorkshop(Workshop workshop, out TextObject explanation);
	}
}
