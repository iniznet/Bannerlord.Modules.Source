using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003E6 RID: 998
	public class CommentChildbirthBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CB6 RID: 15542 RVA: 0x001215BE File Offset: 0x0011F7BE
		public override void RegisterEvents()
		{
			CampaignEvents.OnGivenBirthEvent.AddNonSerializedListener(this, new Action<Hero, List<Hero>, int>(this.OnGivenBirthEvent));
		}

		// Token: 0x06003CB7 RID: 15543 RVA: 0x001215D7 File Offset: 0x0011F7D7
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003CB8 RID: 15544 RVA: 0x001215DC File Offset: 0x0011F7DC
		private void OnGivenBirthEvent(Hero mother, List<Hero> aliveChildren, int stillbornCount)
		{
			if (mother.IsHumanPlayerCharacter || mother.Clan == Hero.MainHero.Clan)
			{
				for (int i = 0; i < stillbornCount; i++)
				{
					ChildbirthLogEntry childbirthLogEntry = new ChildbirthLogEntry(mother, null);
					LogEntry.AddLogEntry(childbirthLogEntry);
					Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ChildBornMapNotification(null, childbirthLogEntry.GetEncyclopediaText()));
				}
				foreach (Hero hero in aliveChildren)
				{
					ChildbirthLogEntry childbirthLogEntry2 = new ChildbirthLogEntry(mother, hero);
					LogEntry.AddLogEntry(childbirthLogEntry2);
					Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ChildBornMapNotification(hero, childbirthLogEntry2.GetEncyclopediaText()));
				}
			}
		}
	}
}
