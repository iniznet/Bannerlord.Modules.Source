using System;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003EF RID: 1007
	public class CommentOnDestroyMobilePartyBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CDB RID: 15579 RVA: 0x00121A72 File Offset: 0x0011FC72
		public override void RegisterEvents()
		{
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
		}

		// Token: 0x06003CDC RID: 15580 RVA: 0x00121A8B File Offset: 0x0011FC8B
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003CDD RID: 15581 RVA: 0x00121A90 File Offset: 0x0011FC90
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
