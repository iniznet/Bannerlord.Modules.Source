using System;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation.Tags;

namespace StoryMode
{
	// Token: 0x02000007 RID: 7
	public class IsArzagosTag : ConversationTag
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000021C1 File Offset: 0x000003C1
		public override string StringId
		{
			get
			{
				return "IsArzagosTag";
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000021C8 File Offset: 0x000003C8
		public override bool IsApplicableTo(CharacterObject character)
		{
			return StoryModeHeroes.AntiImperialMentor.CharacterObject == character;
		}

		// Token: 0x04000004 RID: 4
		public const string Id = "IsArzagosTag";
	}
}
