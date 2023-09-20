using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200023A RID: 570
	public class HonorTag : ConversationTag
	{
		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x06001EB4 RID: 7860 RVA: 0x00087B57 File Offset: 0x00085D57
		public override string StringId
		{
			get
			{
				return "HonorTag";
			}
		}

		// Token: 0x06001EB5 RID: 7861 RVA: 0x00087B5E File Offset: 0x00085D5E
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Honor) > 0;
		}

		// Token: 0x040009BD RID: 2493
		public const string Id = "HonorTag";
	}
}
