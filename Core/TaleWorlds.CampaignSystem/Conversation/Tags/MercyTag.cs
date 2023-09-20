using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000236 RID: 566
	public class MercyTag : ConversationTag
	{
		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x06001EA8 RID: 7848 RVA: 0x00087A9F File Offset: 0x00085C9F
		public override string StringId
		{
			get
			{
				return "MercyTag";
			}
		}

		// Token: 0x06001EA9 RID: 7849 RVA: 0x00087AA6 File Offset: 0x00085CA6
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Mercy) > 0;
		}

		// Token: 0x040009B9 RID: 2489
		public const string Id = "MercyTag";
	}
}
