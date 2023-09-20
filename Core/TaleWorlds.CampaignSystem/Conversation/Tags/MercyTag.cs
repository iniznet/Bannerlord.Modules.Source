using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class MercyTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "MercyTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Mercy) > 0;
		}

		public const string Id = "MercyTag";
	}
}
