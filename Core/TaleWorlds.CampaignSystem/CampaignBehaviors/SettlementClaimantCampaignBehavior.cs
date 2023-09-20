using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003D1 RID: 977
	public class SettlementClaimantCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003AE4 RID: 15076 RVA: 0x00113B5C File Offset: 0x00111D5C
		public override void RegisterEvents()
		{
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
		}

		// Token: 0x06003AE5 RID: 15077 RVA: 0x00113B8C File Offset: 0x00111D8C
		private void DailyTickSettlement(Settlement settlement)
		{
			if (settlement.Town != null && settlement.Town.IsOwnerUnassigned && settlement.OwnerClan != null && settlement.OwnerClan.Kingdom != null)
			{
				Kingdom kingdom = settlement.OwnerClan.Kingdom;
				if (kingdom.UnresolvedDecisions.FirstOrDefault((KingdomDecision x) => x is SettlementClaimantDecision) == null)
				{
					kingdom.AddDecision(new SettlementClaimantDecision(kingdom.RulingClan, settlement, null, null), true);
				}
			}
		}

		// Token: 0x06003AE6 RID: 15078 RVA: 0x00113C10 File Offset: 0x00111E10
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003AE7 RID: 15079 RVA: 0x00113C14 File Offset: 0x00111E14
		public void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (settlement.IsVillage && settlement.Party.MapEvent != null && !FactionManager.IsAtWarAgainstFaction(settlement.Party.MapEvent.AttackerSide.LeaderParty.MapFaction, newOwner.MapFaction))
			{
				settlement.Party.MapEvent.FinalizeEvent();
			}
			if (detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege)
			{
				int num = 0;
				if (newOwner != null)
				{
					foreach (Settlement settlement2 in newOwner.MapFaction.Settlements)
					{
						if (settlement2.CanBeClaimed > num)
						{
							num = settlement2.CanBeClaimed;
						}
					}
				}
				settlement.CanBeClaimed = num + 2;
			}
			if (openToClaim && newOwner.MapFaction.IsKingdomFaction && (newOwner.MapFaction as Kingdom).Clans.Count > 1 && settlement.Town != null)
			{
				settlement.Town.IsOwnerUnassigned = true;
			}
			foreach (Kingdom kingdom in Kingdom.All)
			{
				foreach (KingdomDecision kingdomDecision in kingdom.UnresolvedDecisions.ToList<KingdomDecision>())
				{
					SettlementClaimantDecision settlementClaimantDecision;
					SettlementClaimantPreliminaryDecision settlementClaimantPreliminaryDecision;
					if ((settlementClaimantDecision = kingdomDecision as SettlementClaimantDecision) != null)
					{
						if (settlementClaimantDecision.Settlement == settlement)
						{
							kingdom.RemoveDecision(kingdomDecision);
						}
					}
					else if ((settlementClaimantPreliminaryDecision = kingdomDecision as SettlementClaimantPreliminaryDecision) != null && settlementClaimantPreliminaryDecision.Settlement == settlement && settlementClaimantPreliminaryDecision.Settlement == settlement)
					{
						kingdom.RemoveDecision(kingdomDecision);
					}
				}
			}
			if (oldOwner.Clan == Clan.PlayerClan && (newOwner == null || newOwner.Clan != Clan.PlayerClan))
			{
				foreach (ItemRosterElement itemRosterElement in settlement.Stash)
				{
					settlement.ItemRoster.AddToCounts(itemRosterElement.EquipmentElement.Item, itemRosterElement.Amount);
				}
				settlement.Stash.Clear();
			}
		}
	}
}
