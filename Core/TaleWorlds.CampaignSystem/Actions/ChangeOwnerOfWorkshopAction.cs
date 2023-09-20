using System;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200042F RID: 1071
	public static class ChangeOwnerOfWorkshopAction
	{
		// Token: 0x06003EB5 RID: 16053 RVA: 0x0012BE9C File Offset: 0x0012A09C
		private static void ApplyInternal(Workshop workshop, Hero newOwner, WorkshopType workshopType, int capital, bool upgradable, int cost, TextObject customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail detail)
		{
			Hero owner = workshop.Owner;
			workshop.SetWorkshop(newOwner, workshopType, capital, upgradable, 0, 1, customName);
			if (cost > 0 && detail != ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.Death)
			{
				GiveGoldAction.ApplyBetweenCharacters(newOwner, owner, cost, false);
			}
		}

		// Token: 0x06003EB6 RID: 16054 RVA: 0x0012BED2 File Offset: 0x0012A0D2
		public static void ApplyByBankruptcy(Workshop workshop, Hero newOwner, WorkshopType workshopType, int capital, bool upgradable, int cost, TextObject customName = null)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshopType, capital, upgradable, cost, customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.Bankruptcy);
		}

		// Token: 0x06003EB7 RID: 16055 RVA: 0x0012BEE4 File Offset: 0x0012A0E4
		public static void ApplyByTrade(Workshop workshop, Hero newOwner, WorkshopType workshopType, int capital, bool upgradable, int cost, TextObject customName = null)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshopType, capital, upgradable, cost, customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.Trade);
		}

		// Token: 0x06003EB8 RID: 16056 RVA: 0x0012BEF6 File Offset: 0x0012A0F6
		public static void ApplyByDeath(Workshop workshop, Hero newOwner, TextObject customName = null)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshop.WorkshopType, workshop.Capital, workshop.Upgradable, 0, customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.Death);
		}

		// Token: 0x06003EB9 RID: 16057 RVA: 0x0012BF14 File Offset: 0x0012A114
		public static void ApplyByWarDeclaration(Workshop workshop, Hero newOwner, WorkshopType workshopType, int capital, bool upgradable, TextObject customName = null)
		{
			ChangeOwnerOfWorkshopAction.ApplyInternal(workshop, newOwner, workshopType, capital, upgradable, 0, customName, ChangeOwnerOfWorkshopAction.ChangeOwnerOfWorkshopDetail.WarDeclaration);
		}

		// Token: 0x02000758 RID: 1880
		public enum ChangeOwnerOfWorkshopDetail
		{
			// Token: 0x04001E2A RID: 7722
			Death,
			// Token: 0x04001E2B RID: 7723
			Bankruptcy,
			// Token: 0x04001E2C RID: 7724
			Trade,
			// Token: 0x04001E2D RID: 7725
			WarDeclaration
		}
	}
}
