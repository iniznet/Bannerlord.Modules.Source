using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000239 RID: 569
	public class UngratefulTag : ConversationTag
	{
		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x06001EB1 RID: 7857 RVA: 0x00087B29 File Offset: 0x00085D29
		public override string StringId
		{
			get
			{
				return "UngratefulTag";
			}
		}

		// Token: 0x06001EB2 RID: 7858 RVA: 0x00087B30 File Offset: 0x00085D30
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Generosity) < 0;
		}

		// Token: 0x040009BC RID: 2492
		public const string Id = "UngratefulTag";
	}
}
