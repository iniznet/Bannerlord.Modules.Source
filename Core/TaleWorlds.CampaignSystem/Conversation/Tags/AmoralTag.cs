using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000217 RID: 535
	public class AmoralTag : ConversationTag
	{
		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x06001E4B RID: 7755 RVA: 0x000873C7 File Offset: 0x000855C7
		public override string StringId
		{
			get
			{
				return "AmoralTag";
			}
		}

		// Token: 0x06001E4C RID: 7756 RVA: 0x000873CE File Offset: 0x000855CE
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetTraitLevel(DefaultTraits.Honor) + character.GetTraitLevel(DefaultTraits.Mercy) < 0;
		}

		// Token: 0x04000999 RID: 2457
		public const string Id = "AmoralTag";
	}
}
