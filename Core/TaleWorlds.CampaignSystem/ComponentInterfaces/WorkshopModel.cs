﻿using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class WorkshopModel : GameModel
	{
		public abstract int DaysForPlayerSaveWorkshopFromBankruptcy { get; }

		public abstract int CapitalLowLimit { get; }

		public abstract int InitialCapital { get; }

		public abstract int GetMaxWorkshopCountForClanTier(int tier);

		public abstract int DailyExpense { get; }

		public abstract int GetCostForPlayer(Workshop workshop);

		public abstract int WarehouseCapacity { get; }

		public abstract int DefaultWorkshopCountInSettlement { get; }

		public abstract int GetCostForNotable(Workshop workshop);

		public abstract int MaximumWorkshopsPlayerCanHave { get; }

		public abstract ExplainedNumber GetEffectiveConversionSpeedOfProduction(Workshop workshop, float speed, bool includeDescriptions);

		public abstract Hero GetNotableOwnerForWorkshop(Settlement settlement);

		public abstract int GetConvertProductionCost(WorkshopType workshopType);

		public abstract bool CanPlayerSellWorkshop(Workshop workshop, out TextObject explanation);

		public abstract float GetTradeXpPerWarehouseProduction(EquipmentElement production);
	}
}
