using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class UncharitableTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "UncharitableTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetTraitLevel(DefaultTraits.Generosity) + character.GetTraitLevel(DefaultTraits.Mercy) < 0;
		}

		public const string Id = "UncharitableTag";
	}
}
