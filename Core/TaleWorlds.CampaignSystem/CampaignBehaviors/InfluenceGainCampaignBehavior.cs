using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class InfluenceGainCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnPrisonerDonatedToSettlementEvent.AddNonSerializedListener(this, new Action<MobileParty, FlattenedTroopRoster, Settlement>(this.OnPrisonerDonatedToSettlement));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnPrisonerDonatedToSettlement(MobileParty donatingParty, FlattenedTroopRoster donatedPrisoners, Settlement donatedSettlement)
		{
			float num = 0f;
			foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in donatedPrisoners)
			{
				if (flattenedTroopRosterElement.Troop.IsHero)
				{
					num += Campaign.Current.Models.PrisonerDonationModel.CalculateRelationGainAfterHeroPrisonerDonate(donatingParty.Party, flattenedTroopRosterElement.Troop.HeroObject, donatedSettlement);
				}
				else
				{
					num += Campaign.Current.Models.PrisonerDonationModel.CalculateInfluenceGainAfterPrisonerDonation(donatingParty.Party, flattenedTroopRosterElement.Troop, donatedSettlement);
				}
			}
			GainKingdomInfluenceAction.ApplyForDonatePrisoners(donatingParty, num);
		}
	}
}
