using System;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation.Tags;

namespace StoryMode
{
	// Token: 0x02000006 RID: 6
	public class IsIstianaTag : ConversationTag
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000021A3 File Offset: 0x000003A3
		public override string StringId
		{
			get
			{
				return "IsIstianaTag";
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000021AA File Offset: 0x000003AA
		public override bool IsApplicableTo(CharacterObject character)
		{
			return StoryModeHeroes.ImperialMentor.CharacterObject == character;
		}

		// Token: 0x04000003 RID: 3
		public const string Id = "IsIstianaTag";
	}
}
