﻿using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class PrisonBreakModel : GameModel
	{
		public abstract bool CanPlayerStagePrisonBreak(Settlement settlement);

		public abstract int GetPrisonBreakStartCost(Hero prisonerHero);

		public abstract int GetRelationRewardOnPrisonBreak(Hero prisonerHero);

		public abstract float GetRogueryRewardOnPrisonBreak(Hero prisonerHero, bool isSuccess);
	}
}
