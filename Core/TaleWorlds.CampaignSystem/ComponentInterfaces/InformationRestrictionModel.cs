﻿using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class InformationRestrictionModel : GameModel
	{
		public abstract bool DoesPlayerKnowDetailsOf(Settlement settlement);

		public abstract bool DoesPlayerKnowDetailsOf(Hero hero);
	}
}
