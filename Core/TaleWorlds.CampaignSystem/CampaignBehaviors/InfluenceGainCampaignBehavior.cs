using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A0 RID: 928
	public class InfluenceGainCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003739 RID: 14137 RVA: 0x000F8606 File Offset: 0x000F6806
		public override void RegisterEvents()
		{
			CampaignEvents.OnPrisonerDonatedToSettlementEvent.AddNonSerializedListener(this, new Action<MobileParty, FlattenedTroopRoster, Settlement>(this.OnPrisonerDonatedToSettlement));
		}

		// Token: 0x0600373A RID: 14138 RVA: 0x000F861F File Offset: 0x000F681F
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600373B RID: 14139 RVA: 0x000F8624 File Offset: 0x000F6824
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
