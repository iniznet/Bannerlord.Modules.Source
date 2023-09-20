using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace StoryMode
{
	public abstract class StoryModeQuestBase : QuestBase
	{
		public override bool IsSpecialQuest
		{
			get
			{
				return true;
			}
		}

		public override bool IsRemainingTimeHidden
		{
			get
			{
				return true;
			}
		}

		protected StoryModeQuestBase(string questId, Hero questGiver, CampaignTime duration)
			: base(questId, questGiver, duration, 0)
		{
		}

		protected override void OnTimedOut()
		{
			base.OnTimedOut();
			TextObject textObject = new TextObject("{=JTPmw3cb}You couldn't complete the quest in time.", null);
			base.AddLog(textObject, false);
		}
	}
}
