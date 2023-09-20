using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000214 RID: 532
	public class HostileRelationshipTag : ConversationTag
	{
		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06001E42 RID: 7746 RVA: 0x000872D0 File Offset: 0x000854D0
		public override string StringId
		{
			get
			{
				return "HostileRelationshipTag";
			}
		}

		// Token: 0x06001E43 RID: 7747 RVA: 0x000872D8 File Offset: 0x000854D8
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
			return (num + num2 + num3 < -1 && unmodifiedClanLeaderRelationshipWithPlayer <= -5f) || unmodifiedClanLeaderRelationshipWithPlayer <= -20f;
		}

		// Token: 0x04000996 RID: 2454
		public const string Id = "HostileRelationshipTag";
	}
}
