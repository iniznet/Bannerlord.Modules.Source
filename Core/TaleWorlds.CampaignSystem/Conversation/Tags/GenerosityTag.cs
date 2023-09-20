using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000238 RID: 568
	public class GenerosityTag : ConversationTag
	{
		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x06001EAE RID: 7854 RVA: 0x00087AFB File Offset: 0x00085CFB
		public override string StringId
		{
			get
			{
				return "GenerosityTag";
			}
		}

		// Token: 0x06001EAF RID: 7855 RVA: 0x00087B02 File Offset: 0x00085D02
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Generosity) > 0;
		}

		// Token: 0x040009BB RID: 2491
		public const string Id = "GenerosityTag";
	}
}
