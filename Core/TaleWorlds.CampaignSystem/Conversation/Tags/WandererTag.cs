using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200022D RID: 557
	public class WandererTag : ConversationTag
	{
		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x06001E8D RID: 7821 RVA: 0x000878F1 File Offset: 0x00085AF1
		public override string StringId
		{
			get
			{
				return "WandererTag";
			}
		}

		// Token: 0x06001E8E RID: 7822 RVA: 0x000878F8 File Offset: 0x00085AF8
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsWanderer;
		}

		// Token: 0x040009B0 RID: 2480
		public const string Id = "WandererTag";
	}
}
