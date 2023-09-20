using System;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	public class CommentOnDestroyMobilePartyBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			Hero hero = ((destroyerParty != null) ? destroyerParty.LeaderHero : null);
			IFaction faction = ((destroyerParty != null) ? destroyerParty.MapFaction : null);
			if (hero == Hero.MainHero || mobileParty.LeaderHero == Hero.MainHero || (faction != null && mobileParty.MapFaction != null && faction.IsKingdomFaction && mobileParty.MapFaction.IsKingdomFaction))
			{
				LogEntry.AddLogEntry(new DestroyMobilePartyLogEntry(mobileParty, destroyerParty));
			}
		}
	}
}
