using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace StoryMode
{
	// Token: 0x02000014 RID: 20
	public abstract class StoryModeQuestBase : QuestBase
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00004C74 File Offset: 0x00002E74
		public override bool IsSpecialQuest
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00004C77 File Offset: 0x00002E77
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004C7A File Offset: 0x00002E7A
		protected StoryModeQuestBase(string questId, Hero questGiver, CampaignTime duration)
			: base(questId, questGiver, duration, 0)
		{
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004C88 File Offset: 0x00002E88
		protected override void OnTimedOut()
		{
			base.OnTimedOut();
			TextObject textObject = new TextObject("{=JTPmw3cb}You couldn't complete the quest in time.", null);
			base.AddLog(textObject, false);
		}
	}
}
