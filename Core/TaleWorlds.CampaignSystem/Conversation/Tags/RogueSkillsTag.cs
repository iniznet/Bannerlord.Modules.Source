using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000240 RID: 576
	public class RogueSkillsTag : ConversationTag
	{
		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x06001EC6 RID: 7878 RVA: 0x00087C6B File Offset: 0x00085E6B
		public override string StringId
		{
			get
			{
				return "RogueSkillsTag";
			}
		}

		// Token: 0x06001EC7 RID: 7879 RVA: 0x00087C72 File Offset: 0x00085E72
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.RogueSkills) > 0;
		}

		// Token: 0x040009C3 RID: 2499
		public const string Id = "RogueSkillsTag";
	}
}
