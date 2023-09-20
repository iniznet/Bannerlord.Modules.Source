using System;
using TaleWorlds.CampaignSystem.Settlements.Workshops;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class ChangeOwnerOfWorkshopAction
	{
		private static void ApplyInternal(Workshop workshop, Hero newOwner, WorkshopType workshopType, int capital, int cost)
		{
			Hero owner = workshop.Owner;
			workshop.ChangeOwnerOfWorkshop(newOwner, workshopType, capital);
			if (newOwner == Hero.MainHero)
			{
				GiveGoldAction.ApplyBetweenCharacters(newOwner, owner, cost, false);
			}
			if (owner == Hero.MainHero)
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, cost, false);
			}
			CampaignEventDispatcher.Instance.OnWorkshopOwnerChanged(workshop, owner);
		}

		public static void ApplyByBankruptcy(Workshop workshop, Hero newOwner, WorkshopType workshopType, int cost)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshopType, Campaign.Current.Models.WorkshopModel.InitialCapital, cost);
		}

		public static void ApplyByPlayerBuying(Workshop workshop)
		{
			int costForPlayer = Campaign.Current.Models.WorkshopModel.GetCostForPlayer(workshop);
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, Hero.MainHero, workshop.WorkshopType, Campaign.Current.Models.WorkshopModel.InitialCapital, costForPlayer);
		}

		public static void ApplyByPlayerSelling(Workshop workshop, Hero newOwner, WorkshopType workshopType)
		{
			int costForNotable = Campaign.Current.Models.WorkshopModel.GetCostForNotable(workshop);
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshopType, Campaign.Current.Models.WorkshopModel.InitialCapital, costForNotable);
		}

		public static void ApplyByDeath(Workshop workshop, Hero newOwner)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshop.WorkshopType, workshop.Capital, 0);
		}

		public static void ApplyByWar(Workshop workshop, Hero newOwner, WorkshopType workshopType)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshopType, Campaign.Current.Models.WorkshopModel.InitialCapital, 0);
		}
	}
}
