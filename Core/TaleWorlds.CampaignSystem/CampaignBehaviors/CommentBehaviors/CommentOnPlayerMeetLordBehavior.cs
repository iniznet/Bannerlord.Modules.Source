using System;
using TaleWorlds.CampaignSystem.LogEntries;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003F4 RID: 1012
	public class CommentOnPlayerMeetLordBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CEF RID: 15599 RVA: 0x00121C1D File Offset: 0x0011FE1D
		public override void RegisterEvents()
		{
			CampaignEvents.OnPlayerMetHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnPlayerMetCharacter));
		}

		// Token: 0x06003CF0 RID: 15600 RVA: 0x00121C36 File Offset: 0x0011FE36
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003CF1 RID: 15601 RVA: 0x00121C38 File Offset: 0x0011FE38
		private void OnPlayerMetCharacter(Hero hero)
		{
			if (hero.Mother != Hero.MainHero && hero.Father != Hero.MainHero)
			{
				LogEntry.AddLogEntry(new PlayerMeetLordLogEntry(hero));
			}
		}
	}
}
