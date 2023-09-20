using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.LinQuick;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class GarrisonTroopsCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUpEvent));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

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

		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (!Campaign.Current.GameStarted)
			{
				return;
			}
			if (mobileParty != null && mobileParty != MobileParty.MainParty && mobileParty.IsLordParty && !mobileParty.IsDisbanding && mobileParty.LeaderHero != null && settlement.IsFortification && FactionManager.IsAlliedWithFaction(mobileParty.MapFaction, settlement.MapFaction) && (settlement.OwnerClan != Clan.PlayerClan || settlement.Town.IsOwnerUnassigned))
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
