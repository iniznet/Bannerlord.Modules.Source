using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000435 RID: 1077
	public static class ChangeVillageStateAction
	{
		// Token: 0x06003EC6 RID: 16070 RVA: 0x0012C124 File Offset: 0x0012A324
		private static void ApplyInternal(Village village, Village.VillageStates newState, MobileParty raiderParty)
		{
			Village.VillageStates villageState = village.VillageState;
			if (newState != villageState)
			{
				village.VillageState = newState;
				CampaignEventDispatcher.Instance.OnVillageStateChanged(village, villageState, village.VillageState, raiderParty);
				IPartyVisual visuals = village.Settlement.Party.Visuals;
				if (visuals == null)
				{
					return;
				}
				visuals.RefreshLevelMask(village.Settlement.Party);
			}
		}

		// Token: 0x06003EC7 RID: 16071 RVA: 0x0012C17B File Offset: 0x0012A37B
		public static void ApplyBySettingToNormal(Settlement settlement)
		{
			ChangeVillageStateAction.ApplyInternal(settlement.Village, Village.VillageStates.Normal, null);
		}

		// Token: 0x06003EC8 RID: 16072 RVA: 0x0012C18A File Offset: 0x0012A38A
		public static void ApplyBySettingToBeingRaided(Settlement settlement, MobileParty raider)
		{
			ChangeVillageStateAction.ApplyInternal(settlement.Village, Village.VillageStates.BeingRaided, raider);
		}

		// Token: 0x06003EC9 RID: 16073 RVA: 0x0012C199 File Offset: 0x0012A399
		public static void ApplyBySettingToBeingForcedForSupplies(Settlement settlement, MobileParty raider)
		{
			ChangeVillageStateAction.ApplyInternal(settlement.Village, Village.VillageStates.ForcedForSupplies, raider);
		}

		// Token: 0x06003ECA RID: 16074 RVA: 0x0012C1A8 File Offset: 0x0012A3A8
		public static void ApplyBySettingToBeingForcedForVolunteers(Settlement settlement, MobileParty raider)
		{
			ChangeVillageStateAction.ApplyInternal(settlement.Village, Village.VillageStates.ForcedForVolunteers, raider);
		}

		// Token: 0x06003ECB RID: 16075 RVA: 0x0012C1B7 File Offset: 0x0012A3B7
		public static void ApplyBySettingToLooted(Settlement settlement, MobileParty raider)
		{
			ChangeVillageStateAction.ApplyInternal(settlement.Village, Village.VillageStates.Looted, raider);
		}
	}
}
