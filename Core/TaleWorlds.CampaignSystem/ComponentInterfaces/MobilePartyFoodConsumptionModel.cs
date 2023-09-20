﻿using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class MobilePartyFoodConsumptionModel : GameModel
	{
		public abstract int NumberOfMenOnMapToEatOneFood { get; }

		public abstract ExplainedNumber CalculateDailyBaseFoodConsumptionf(MobileParty party, bool includeDescription = false);

		public abstract ExplainedNumber CalculateDailyFoodConsumptionf(MobileParty party, ExplainedNumber baseConsumption);

		public abstract bool DoesPartyConsumeFood(MobileParty mobileParty);
	}
}
