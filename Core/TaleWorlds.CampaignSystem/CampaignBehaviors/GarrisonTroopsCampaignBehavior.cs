using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.LinQuick;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000393 RID: 915
	public class GarrisonTroopsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003689 RID: 13961 RVA: 0x000F37CD File Offset: 0x000F19CD
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUpEvent));
		}

		// Token: 0x0600368A RID: 13962 RVA: 0x000F37FD File Offset: 0x000F19FD
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600368B RID: 13963 RVA: 0x000F3800 File Offset: 0x000F1A00
		public void OnNewGameCreatedPartialFollowUpEvent(CampaignGameStarter starter, int i)
		{
			List<Settlement> list = Campaign.Current.Settlements.WhereQ((Settlement x) => x.IsFortification).ToList<Settlement>();
			int count = list.Count;
			int num = count / 100 + ((count % 100 > i) ? 1 : 0);
			int num2 = count / 100 * i;
			for (int j = 0; j < i; j++)
			{
				num2 += ((count % 100 > j) ? 1 : 0);
			}
			for (int k = 0; k < num; k++)
			{
				list[num2 + k].AddGarrisonParty(true);
			}
		}

		// Token: 0x0600368C RID: 13964 RVA: 0x000F38A0 File Offset: 0x000F1AA0
		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (!Campaign.Current.GameStarted)
			{
				return;
			}
			if (mobileParty != null && mobileParty != MobileParty.MainParty && mobileParty.IsLordParty && !mobileParty.IsDisbanding && mobileParty.LeaderHero != null && FactionManager.IsAlliedWithFaction(mobileParty.MapFaction, settlement.MapFaction) && (settlement.OwnerClan != Clan.PlayerClan || Hero.MainHero == Hero.MainHero.MapFaction.Leader) && settlement.IsFortification)
			{
				int num = Campaign.Current.Models.SettlementGarrisonModel.FindNumberOfTroopsToLeaveToGarrison(mobileParty, mobileParty.CurrentSettlement);
				if (num > 0)
				{
					LeaveTroopsToSettlementAction.Apply(mobileParty, settlement, num, true);
					return;
				}
				if (mobileParty.LeaderHero.Clan == settlement.OwnerClan && mobileParty.CanPayMoreWage())
				{
					int num2 = Campaign.Current.Models.SettlementGarrisonModel.FindNumberOfTroopsToTakeFromGarrison(mobileParty, mobileParty.CurrentSettlement, 0f);
					if (num2 > 0)
					{
						LeaveTroopsToSettlementAction.Apply(mobileParty, settlement, -num2, false);
					}
				}
			}
		}
	}
}
