using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	public class CampaignTutorial
	{
		public TextObject Description
		{
			get
			{
				return GameTexts.FindText("str_campaign_tutorial_description", this.TutorialTypeId);
			}
		}

		public TextObject Title
		{
			get
			{
				return GameTexts.FindText("str_campaign_tutorial_title", this.TutorialTypeId);
			}
		}

		public CampaignTutorial(string tutorialType, int priority)
		{
			this.TutorialTypeId = tutorialType;
			this.Priority = priority;
		}

		public readonly string TutorialTypeId;

		public readonly int Priority;
	}
}
