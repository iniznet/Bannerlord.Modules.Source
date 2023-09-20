using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000216 RID: 534
	public class UncharitableTag : ConversationTag
	{
		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x06001E48 RID: 7752 RVA: 0x0008739C File Offset: 0x0008559C
		public override string StringId
		{
			get
			{
				return "UncharitableTag";
			}
		}

		// Token: 0x06001E49 RID: 7753 RVA: 0x000873A3 File Offset: 0x000855A3
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetTraitLevel(DefaultTraits.Generosity) + character.GetTraitLevel(DefaultTraits.Mercy) < 0;
		}

		// Token: 0x04000998 RID: 2456
		public const string Id = "UncharitableTag";
	}
}
