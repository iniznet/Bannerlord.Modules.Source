using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000213 RID: 531
	public class FriendlyRelationshipTag : ConversationTag
	{
		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x06001E3F RID: 7743 RVA: 0x00087240 File Offset: 0x00085440
		public override string StringId
		{
			get
			{
				return "FriendlyRelationshipTag";
			}
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x00087248 File Offset: 0x00085448
		public override bool IsApplicableTo(CharacterObject character)
		{
			if (!character.IsHero)
			{
				return false;
			}
			float unmodifiedClanLeaderRelationshipWithPlayer = character.HeroObject.GetUnmodifiedClanLeaderRelationshipWithPlayer();
			int num = ConversationTagHelper.TraitCompatibility(character.HeroObject, Hero.MainHero, DefaultTraits.Mercy);
			int num2 = ConversationTagHelper.TraitCompatibility(character.HeroObject, Hero.MainHero, DefaultTraits.Honor);
			int num3 = ConversationTagHelper.TraitCompatibility(character.HeroObject, Hero.MainHero, DefaultTraits.Valor);
			return (num + num2 + num3 > 0 && unmodifiedClanLeaderRelationshipWithPlayer >= 5f) || unmodifiedClanLeaderRelationshipWithPlayer >= 20f;
		}

		// Token: 0x04000995 RID: 2453
		public const string Id = "FriendlyRelationshipTag";
	}
}
