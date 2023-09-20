using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200022F RID: 559
	public class NonviolentProfessionTag : ConversationTag
	{
		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x06001E93 RID: 7827 RVA: 0x00087980 File Offset: 0x00085B80
		public override string StringId
		{
			get
			{
				return "NonviolentProfessionTag";
			}
		}

		// Token: 0x06001E94 RID: 7828 RVA: 0x00087987 File Offset: 0x00085B87
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && (character.Occupation == Occupation.Artisan || character.Occupation == Occupation.Merchant || character.Occupation == Occupation.Headman);
		}

		// Token: 0x040009B2 RID: 2482
		public const string Id = "NonviolentProfessionTag";
	}
}
