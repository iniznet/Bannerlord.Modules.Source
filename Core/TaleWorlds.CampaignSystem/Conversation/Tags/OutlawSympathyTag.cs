using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x020001FB RID: 507
	public class OutlawSympathyTag : ConversationTag
	{
		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x06001DF7 RID: 7671 RVA: 0x00086D17 File Offset: 0x00084F17
		public override string StringId
		{
			get
			{
				return "OutlawSympathyTag";
			}
		}

		// Token: 0x06001DF8 RID: 7672 RVA: 0x00086D1E File Offset: 0x00084F1E
		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsWanderer && character.HeroObject.GetTraitLevel(DefaultTraits.RogueSkills) > 0;
		}

		// Token: 0x0400097D RID: 2429
		public const string Id = "OutlawSympathyTag";
	}
}
