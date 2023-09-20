using System;
using TaleWorlds.CampaignSystem.Settlements.Workshops;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class ChangeProductionTypeOfWorkshopAction
	{
		public static void Apply(Workshop workshop, WorkshopType newWorkshopType, bool ignoreCost = false)
		{
			int num = (ignoreCost ? 0 : Campaign.Current.Models.WorkshopModel.GetConvertProductionCost(newWorkshopType));
			workshop.ChangeWorkshopProduction(newWorkshopType);
			if (num > 0)
			{
				GiveGoldAction.ApplyBetweenCharacters(workshop.Owner, null, num, false);
			}
			CampaignEventDispatcher.Instance.OnWorkshopTypeChanged(workshop);
		}
	}
}
