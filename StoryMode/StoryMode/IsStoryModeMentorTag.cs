using System;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation.Tags;

namespace StoryMode
{
	// Token: 0x02000008 RID: 8
	public class IsStoryModeMentorTag : ConversationTag
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001A RID: 26 RVA: 0x000021DF File Offset: 0x000003DF
		public override string StringId
		{
			get
			{
				return "IsStoryModeMentorTag";
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000021E6 File Offset: 0x000003E6
		public override bool IsApplicableTo(CharacterObject character)
		{
			return StoryModeHeroes.AntiImperialMentor.CharacterObject == character || StoryModeHeroes.ImperialMentor.CharacterObject == character;
		}

		// Token: 0x04000005 RID: 5
		public const string Id = "IsStoryModeMentorTag";
	}
}
