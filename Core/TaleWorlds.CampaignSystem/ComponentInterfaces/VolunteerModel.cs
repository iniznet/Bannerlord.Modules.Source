﻿using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class VolunteerModel : GameModel
	{
		public abstract int MaximumIndexHeroCanRecruitFromHero(Hero buyerHero, Hero sellerHero, int useValueAsRelation = -101);

		public abstract float GetDailyVolunteerProductionProbability(Hero hero, int index, Settlement settlement);

		public abstract CharacterObject GetBasicVolunteer(Hero hero);

		public abstract bool CanHaveRecruits(Hero hero);

		public abstract int MaxVolunteerTier { get; }
	}
}
