using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200023B RID: 571
	public class DeviousTag : ConversationTag
	{
		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x06001EB7 RID: 7863 RVA: 0x00087B85 File Offset: 0x00085D85
		public override string StringId
		{
			get
			{
				return "DeviousTag";
			}
		}

		// Token: 0x06001EB8 RID: 7864 RVA: 0x00087B8C File Offset: 0x00085D8C
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Honor) < 0;
		}

		// Token: 0x040009BE RID: 2494
		public const string Id = "DeviousTag";
	}
}
