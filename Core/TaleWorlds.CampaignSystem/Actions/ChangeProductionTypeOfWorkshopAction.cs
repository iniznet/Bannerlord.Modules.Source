﻿using System;
using TaleWorlds.CampaignSystem.Settlements.Workshops;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class ChangeProductionTypeOfWorkshopAction
	{
		public static void Apply(Workshop workshop, WorkshopType workshopType, bool ignoreCost = false)
		{
			int num = (ignoreCost ? 0 : Campaign.Current.Models.WorkshopModel.GetConvertProductionCost(workshopType));
			workshop.SetWorkshop(workshop.Owner, workshopType, workshop.Capital, workshop.Upgradable, 0, 1, null);
			if (num > 0)
			{
				GiveGoldAction.ApplyBetweenCharacters(workshop.Owner, null, num, false);
			}
		}
	}
}