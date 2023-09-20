using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class HonorTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "HonorTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Honor) > 0;
		}

		public const string Id = "HonorTag";
	}
}
