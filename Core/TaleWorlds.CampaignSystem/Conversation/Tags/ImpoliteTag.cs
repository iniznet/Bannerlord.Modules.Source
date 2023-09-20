using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class ImpoliteTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "ImpoliteTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			if (!character.IsHero)
			{
				return false;
			}
			int heroRelation = CharacterRelationManager.GetHeroRelation(character.HeroObject, Hero.MainHero);
			return (character.HeroObject.IsLord || character.HeroObject.IsMerchant || character.HeroObject.IsGangLeader) && Clan.PlayerClan.Renown < 100f && heroRelation < 1 && character.GetTraitLevel(DefaultTraits.Mercy) + character.GetTraitLevel(DefaultTraits.Generosity) < 0;
		}

		public const string Id = "ImpoliteTag";
	}
}
