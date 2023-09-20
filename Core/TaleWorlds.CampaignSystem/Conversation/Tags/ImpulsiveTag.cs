using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200023D RID: 573
	public class ImpulsiveTag : ConversationTag
	{
		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x06001EBD RID: 7869 RVA: 0x00087BE1 File Offset: 0x00085DE1
		public override string StringId
		{
			get
			{
				return "ImpulsiveTag";
			}
		}

		// Token: 0x06001EBE RID: 7870 RVA: 0x00087BE8 File Offset: 0x00085DE8
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Calculating) < 0;
		}

		// Token: 0x040009C0 RID: 2496
		public const string Id = "ImpulsiveTag";
	}
}
