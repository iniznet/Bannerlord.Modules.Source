using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class FriendlyRelationshipTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "FriendlyRelationshipTag";
			}
		}

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

		public const string Id = "FriendlyRelationshipTag";
	}
}
