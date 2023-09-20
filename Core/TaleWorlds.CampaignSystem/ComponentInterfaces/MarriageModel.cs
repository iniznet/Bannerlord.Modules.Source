﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class MarriageModel : GameModel
	{
		public abstract bool IsCoupleSuitableForMarriage(Hero firstHero, Hero secondHero);

		public abstract int GetEffectiveRelationIncrease(Hero firstHero, Hero secondHero);

		public abstract Clan GetClanAfterMarriage(Hero firstHero, Hero secondHero);

		public abstract bool IsSuitableForMarriage(Hero hero);

		public abstract bool IsClanSuitableForMarriage(Clan clan);

		public abstract float NpcCoupleMarriageChance(Hero firstHero, Hero secondHero);

		public abstract bool ShouldNpcMarriageBetweenClansBeAllowed(Clan consideringClan, Clan targetClan);

		public abstract List<Hero> GetAdultChildrenSuitableForMarriage(Hero hero);

		public abstract int MinimumMarriageAgeMale { get; }

		public abstract int MinimumMarriageAgeFemale { get; }
	}
}
