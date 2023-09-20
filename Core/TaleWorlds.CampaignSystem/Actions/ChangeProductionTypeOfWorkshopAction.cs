using System;
using TaleWorlds.CampaignSystem.Settlements.Workshops;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000431 RID: 1073
	public static class ChangeProductionTypeOfWorkshopAction
	{
		// Token: 0x06003EBC RID: 16060 RVA: 0x0012BFE0 File Offset: 0x0012A1E0
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
