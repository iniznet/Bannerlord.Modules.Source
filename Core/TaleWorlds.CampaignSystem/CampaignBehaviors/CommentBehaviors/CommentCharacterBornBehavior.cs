using System;
using TaleWorlds.CampaignSystem.LogEntries;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003E5 RID: 997
	public class CommentCharacterBornBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CB2 RID: 15538 RVA: 0x0012158B File Offset: 0x0011F78B
		public override void RegisterEvents()
		{
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.HeroCreated));
		}

		// Token: 0x06003CB3 RID: 15539 RVA: 0x001215A4 File Offset: 0x0011F7A4
		private void HeroCreated(Hero hero, bool isBornNaturally)
		{
			if (isBornNaturally)
			{
				LogEntry.AddLogEntry(new CharacterBornLogEntry(hero));
			}
		}

		// Token: 0x06003CB4 RID: 15540 RVA: 0x001215B4 File Offset: 0x0011F7B4
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
