using System;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class ChangeOwnerOfWorkshopAction
	{
		private static void ApplyInternal(Workshop workshop, Hero newOwner, WorkshopType workshopType, int capital, bool upgradable, int cost, TextObject customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail detail)
		{
			Hero owner = workshop.Owner;
			workshop.SetWorkshop(newOwner, workshopType, capital, upgradable, 0, 1, customName);
			if (cost > 0 && detail != ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.Death)
			{
				GiveGoldAction.ApplyBetweenCharacters(newOwner, owner, cost, false);
			}
		}

		public static void ApplyByBankruptcy(Workshop workshop, Hero newOwner, WorkshopType workshopType, int capital, bool upgradable, int cost, TextObject customName = null)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshopType, capital, upgradable, cost, customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.Bankruptcy);
		}

		public static void ApplyByTrade(Workshop workshop, Hero newOwner, WorkshopType workshopType, int capital, bool upgradable, int cost, TextObject customName = null)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshopType, capital, upgradable, cost, customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.Trade);
		}

		public static void ApplyByDeath(Workshop workshop, Hero newOwner, TextObject customName = null)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshop.WorkshopType, workshop.Capital, workshop.Upgradable, 0, customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.Death);
		}

		public static void ApplyByWarDeclaration(Workshop workshop, Hero newOwner, WorkshopType workshopType, int capital, bool upgradable, TextObject customName = null)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshopType, capital, upgradable, 0, customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.WarDeclaration);
		}

		public enum ChangeOwnerOfWorkshopDetail
		{
			Death,
			Bankruptcy,
			Trade,
			WarDeclaration
		}
	}
}
