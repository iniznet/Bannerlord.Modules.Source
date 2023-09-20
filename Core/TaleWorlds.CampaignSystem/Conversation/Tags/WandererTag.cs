using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class WandererTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "WandererTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsWanderer;
		}

		public const string Id = "WandererTag";
	}
}
