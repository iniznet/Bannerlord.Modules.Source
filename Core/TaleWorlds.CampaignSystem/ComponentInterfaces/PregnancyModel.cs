﻿using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class PregnancyModel : GameModel
	{
		public abstract float GetDailyChanceOfPregnancyForHero(Hero hero);

		public abstract float PregnancyDurationInDays { get; }

		public abstract float MaternalMortalityProbabilityInLabor { get; }

		public abstract float StillbirthProbability { get; }

		public abstract float DeliveringFemaleOffspringProbability { get; }

		public abstract float DeliveringTwinsProbability { get; }
	}
}
