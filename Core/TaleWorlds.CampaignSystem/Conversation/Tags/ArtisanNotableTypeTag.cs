using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200022B RID: 555
	public class ArtisanNotableTypeTag : ConversationTag
	{
		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x06001E87 RID: 7815 RVA: 0x000878A6 File Offset: 0x00085AA6
		public override string StringId
		{
			get
			{
				return "ArtisanNotableTypeTag";
			}
		}

		// Token: 0x06001E88 RID: 7816 RVA: 0x000878AD File Offset: 0x00085AAD
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.Artisan;
		}

		// Token: 0x040009AE RID: 2478
		public const string Id = "ArtisanNotableTypeTag";
	}
}
