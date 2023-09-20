using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000218 RID: 536
	public class ChivalrousTag : ConversationTag
	{
		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06001E4E RID: 7758 RVA: 0x000873F2 File Offset: 0x000855F2
		public override string StringId
		{
			get
			{
				return "ChivalrousTag";
			}
		}

		// Token: 0x06001E4F RID: 7759 RVA: 0x000873F9 File Offset: 0x000855F9
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetTraitLevel(DefaultTraits.Honor) + character.GetTraitLevel(DefaultTraits.Valor) > 0;
		}

		// Token: 0x0400099A RID: 2458
		public const string Id = "ChivalrousTag";
	}
}
