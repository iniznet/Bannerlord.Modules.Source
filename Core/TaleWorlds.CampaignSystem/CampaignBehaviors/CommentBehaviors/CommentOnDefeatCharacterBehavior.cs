using System;
using TaleWorlds.CampaignSystem.LogEntries;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors
{
	// Token: 0x020003EE RID: 1006
	public class CommentOnDefeatCharacterBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003CD7 RID: 15575 RVA: 0x00121A41 File Offset: 0x0011FC41
		public override void RegisterEvents()
		{
			CampaignEvents.CharacterDefeated.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnCharacterDefeated));
		}

		// Token: 0x06003CD8 RID: 15576 RVA: 0x00121A5A File Offset: 0x0011FC5A
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003CD9 RID: 15577 RVA: 0x00121A5C File Offset: 0x0011FC5C
		private void OnCharacterDefeated(Hero winner, Hero loser)
		{
			LogEntry.AddLogEntry(new DefeatCharacterLogEntry(winner, loser));
		}
	}
}
